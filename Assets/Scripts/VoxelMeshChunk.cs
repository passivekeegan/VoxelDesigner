using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMeshData
{
	public bool flat_shaded;
	public VoxelMapping mapping;

	public List<Vector3> vertices { get; private set; }
	public List<Vector3> normals { get; private set; }
	public List<int> triangles { get; private set; }

	private List<int> _vertexslots;
	private List<VertexData> _vertexdata;
	private List<TriangleData> _tridata;
	private Dictionary<IJL, BData> _blockdata;

	private HashSet<IJL> _dirtyblocks;
	private Dictionary<IJL, VoxelData> _voxeldata;
	private ProximityLattice _vertexlattice;

	public ChunkMeshData()
	{
		vertices = new List<Vector3>();
		normals = new List<Vector3>();
		triangles = new List<int>();

		_vertexslots = new List<int>();
		_vertexdata = new List<VertexData>();
		_tridata = new List<TriangleData>();
		_voxeldata = new Dictionary<IJL, VoxelData>();

		_dirtyblocks = new HashSet<IJL>();
		_blockdata = new Dictionary<IJL, BData>();
		_vertexlattice = new ProximityLattice(0, 1);
	}

	public void Clear()
	{
		vertices.Clear();
		normals.Clear();
		triangles.Clear();

		_vertexslots.Clear();
		_vertexdata.Clear();
		_tridata.Clear();
		_voxeldata.Clear();

		_dirtyblocks.Clear();
		_blockdata.Clear();
		_vertexlattice.Clear(true);
	}

	public void UpdateVoxelData(IJL ijl, VoxelData data)
	{
		if (_voxeldata.ContainsKey(ijl)) { //voxel at location already exists
			if (data.id == 0) {//voxel is empty
				_voxeldata.Remove(ijl);//delete voxel
				DirtyVoxel(ijl);
			}
			else {
				VoxelData olddata = _voxeldata[ijl];
				_voxeldata[ijl] = data;//update voxel
				if (olddata.draw != data.draw || olddata.id != data.id) {
					DirtyVoxel(ijl);
				}
			}
		}
		else {//voxel at location does not exist
			if (data.id != 0) {//voxel is not empty
				_voxeldata.Add(ijl, data);//add voxel
				DirtyVoxel(ijl);
			}
		}
	}

	private void DirtyVoxel(IJL voxel_ijl)
	{
		IJL voxel_key = Vx.VoxelKey(voxel_ijl);
		for (int d = 0; d < 6; d++) {
			for (int lvl = 0; lvl < 2; lvl++) {
				_dirtyblocks.Add(voxel_key + Vx.KeyTransform_VoxelBlock[d, lvl]);
			}
		}
	}

	private void ClassifyBlocks()
	{
		foreach (IJL block_key in _dirtyblocks) {
			bool topheavy = IsBlockKeyTopHeavy(block_key);
			VoxelPattern pat = GetVoxelPattern(block_key, topheavy);
			if (pat.isEmpty) {
				continue;
			}
			VoxelDesign design = mapping.GetVoxelDesign(pat);
			if (design == null || !design.isValid) {
				continue;
			}
			//classify pattern
			VoxelPattern map_pat = mapping.GetVoxelPattern(pat);
			if (map_pat.isEmpty) {
				continue;
			}
			//get pattern transformation
			PatternMatch match = VoxelPattern.CalculateTransform(map_pat, pat, topheavy);
			if (match.shift < 0) {
				continue;
			}
			int vertex_count = design.vertices.Count;
			if (vertex_count <= 0) {
				continue;
			}
			if (_blockdata.ContainsKey(block_key)) {
				continue;
			}
			_blockdata.Add(block_key, new BData(design, match));
		}
		_dirtyblocks.Clear();
	}

	public void GenerateMesh()
	{
		vertices.Clear();
		triangles.Clear();
		normals.Clear();
		_vertexslots.Clear();
		_vertexdata.Clear();
		_blockdata.Clear();
		_tridata.Clear();
		_vertexlattice.Clear(false);

		if (mapping == null) {
			return;
		}

		_vertexlattice.proximity = mapping.vertexWeldDistance;

		ClassifyBlocks();
		CalculateMeshData();
		FinalizeMesh();
	}

	private void CalculateMeshData()
	{
		foreach (KeyValuePair<IJL, BData> blockdata in _blockdata) {
			IJL block_key = blockdata.Key;
			PatternMatch match = blockdata.Value.match;
			VoxelDesign design = blockdata.Value.design;

			int block_index = _vertexslots.Count;
			bool topheavy = IsBlockKeyTopHeavy(block_key);
			//get draw value
			bool draw = ShouldDrawBlock(block_key, topheavy);
			Vector3 block_vertex = Vx.KeyVertex(block_key);
			List<Point> points = design.vertices;
			for (int k = 0; k < points.Count; k++) {
				Point point = points[k];
				Vector3 vertex = block_vertex + TransformVertex(point.vertex, ref match);
				int data_index = -1;
				if (point.shared) {
					data_index = _vertexlattice.GetCloseVertexIndex(vertex);
				}
				if (data_index < 0) {
					data_index = _vertexdata.Count;
					_vertexdata.Add(new VertexData(false, -1, vertex, Vector3.zero));
					if (point.shared) {
						_vertexlattice.AddVertex(data_index, vertex);
					}
				}
				_vertexslots.Add(data_index);
			}
			if (flat_shaded && !draw) {
				continue;
			}
			//add triangles
			bool flip = (match.invx && !match.invy) || (!match.invx && match.invy);
			List<Triangle> tris = design.triangles;
			for (int k = 0; k < tris.Count; k++) {
				Triangle tri = tris[k];
				if (!tri.IsValid(points.Count)) {
					continue;
				}
				AddTriangle(block_index + tri.vertex0, block_index + tri.vertex1, block_index + tri.vertex2, flip, draw);
			}
		}
	}

	private VoxelPattern GetVoxelPattern(IJL block_key, bool topheavy)
	{
		int d0 = 4; int d1 = 0; int d2 = 2;
		if (!topheavy) {
			d0 += 1; d1 += 1; d2 += 1;
		}
		//above 0
		IJL voxel_ijl = Vx.VoxelKeyIJL(block_key + Vx.KeyTransform_VoxelBlock[d0, 0]);
		int a0 = GetVoxelID(voxel_ijl);
		//above 1
		voxel_ijl = Vx.VoxelKeyIJL(block_key + Vx.KeyTransform_VoxelBlock[d1, 0]);
		int a1 = GetVoxelID(voxel_ijl);
		//above 2
		voxel_ijl = Vx.VoxelKeyIJL(block_key + Vx.KeyTransform_VoxelBlock[d2, 0]);
		int a2 = GetVoxelID(voxel_ijl);
		//below 0
		voxel_ijl = Vx.VoxelKeyIJL(block_key + Vx.KeyTransform_VoxelBlock[d0, 1]);
		int b0 = GetVoxelID(voxel_ijl);
		//below 1
		voxel_ijl = Vx.VoxelKeyIJL(block_key + Vx.KeyTransform_VoxelBlock[d1, 1]);
		int b1 = GetVoxelID(voxel_ijl);
		//below 2
		voxel_ijl = Vx.VoxelKeyIJL(block_key + Vx.KeyTransform_VoxelBlock[d2, 1]);
		int b2 = GetVoxelID(voxel_ijl);
		return new VoxelPattern(a0, a1, a2, b0, b1, b2);
	}

	private void FinalizeMesh()
	{
		for (int k = 0; k < _vertexdata.Count; k++) {
			VertexData data = _vertexdata[k];
			if (data.hastri) {
				vertices.Add(data.vertex);
				normals.Add(data.normal.normalized);
				_vertexdata[k] = new VertexData(true, vertices.Count - 1, data.vertex, data.normal);
			}
		}
		int slot_count = _vertexslots.Count;
		int vertex_count = vertices.Count;
		for (int k = 0; k < _tridata.Count; k++) {
			TriangleData data = _tridata[k];
			int slot0_index = data.vertex0;
			int slot1_index = data.vertex1;
			int slot2_index = data.vertex2;
			if (slot0_index < 0 || slot1_index < 0 || slot2_index < 0) {
				continue;
			}
			if (slot0_index >= slot_count || slot1_index >= slot_count || slot2_index >= slot_count) {
				continue;
			}
			int data_index0 = _vertexslots[slot0_index];
			int tri_index0 = _vertexdata[data_index0].index;
			if (tri_index0 < 0 || tri_index0 >= vertex_count) {
				continue;
			}

			int data_index1 = _vertexslots[slot1_index];
			int tri_index1 = _vertexdata[data_index1].index;
			if (tri_index1 < 0 || tri_index1 >= vertex_count) {
				continue;
			}
			int data_index2 = _vertexslots[slot2_index];
			int tri_index2 = _vertexdata[_vertexslots[slot2_index]].index;
			if (tri_index2 < 0 || tri_index2 >= vertex_count) {
				continue;
			}
			triangles.Add(tri_index0);
			triangles.Add(tri_index1);
			triangles.Add(tri_index2);
		}
	}

	private void AddTriangle(int slotindex0, int slotindex1, int slotindex2, bool flip, bool draw)
	{
		if (flat_shaded && !draw) {
			return;
		}
		int vertex0 = _vertexslots[slotindex0];
		int vertex1 = _vertexslots[slotindex1];
		int vertex2 = _vertexslots[slotindex2];
		VertexData data0 = _vertexdata[vertex0];
		VertexData data1 = _vertexdata[vertex1];
		VertexData data2 = _vertexdata[vertex2];
		Vector3 normal = Triangle.Normal(data0.vertex, data1.vertex, data2.vertex);
		if (flip) {
			normal = -normal;
		}
		if (flat_shaded) {
			//vertex slot 0
			if (data0.hastri) {
				_vertexdata.Add(new VertexData(true, -1, data0.vertex, normal));
				_vertexslots.Add(_vertexdata.Count - 1);
				slotindex0 = _vertexslots.Count - 1;
			}
			else {
				_vertexdata[vertex0] = new VertexData(true, -1, data0.vertex, normal);
			}
			//vertex slot 1
			if (data1.hastri) {
				_vertexdata.Add(new VertexData(true, -1, data1.vertex, normal));
				_vertexslots.Add(_vertexdata.Count - 1);
				slotindex1 = _vertexslots.Count - 1;
			}
			else {
				_vertexdata[vertex1] = new VertexData(true, -1, data1.vertex, normal);
			}
			//vertex slot 2
			if (data2.hastri) {
				_vertexdata.Add(new VertexData(true, -1, data2.vertex, normal));
				_vertexslots.Add(_vertexdata.Count - 1);
				slotindex2 = _vertexslots.Count - 1;
			}
			else {
				_vertexdata[vertex2] = new VertexData(true, -1, data2.vertex, normal);
			}
		}
		else {
			Vector3 v0 = data1.vertex - data0.vertex;
			Vector3 v1 = data2.vertex - data1.vertex;
			Vector3 v2 = data0.vertex - data2.vertex;
			//vertex slot 0
			float factor = Vector3.Angle(v0, -v2) / 180f;
			_vertexdata[vertex0] = new VertexData(draw || data0.hastri, -1, data0.vertex, data0.normal + (factor * normal));
			//vertex slot 1
			factor = Vector3.Angle(v1, -v0) / 180f;
			_vertexdata[vertex1] = new VertexData(draw || data1.hastri, -1, data1.vertex, data1.normal + (factor * normal));
			//vertex slot 2
			factor = Vector3.Angle(v2, -v1) / 180f;
			_vertexdata[vertex2] = new VertexData(draw || data2.hastri, -1, data2.vertex, data2.normal + (factor * normal));
		}
		//add triangle
		if (draw) {
			if (flip) {
				_tridata.Add(new TriangleData(slotindex0, slotindex2, slotindex1));
			}
			else {
				_tridata.Add(new TriangleData(slotindex0, slotindex1, slotindex2));
			}
		}
	}

	public Vector3 TransformVertex(Vector3 vertex, ref PatternMatch match)
	{
		//reflect x
		if (match.invx) {
			float y = vertex.y;
			vertex = (2 * Vector3.Dot(Vx.ReflVector, vertex) * Vx.ReflVector) - vertex;
			vertex = new Vector3(vertex.x, y, vertex.z);
		}
		//reflect y
		if (match.invy) {
			vertex = Vector3.Scale(new Vector3(1, -1, 1), vertex);
		}
		//rotate
		vertex = Vx.AxiRot[match.shift] * vertex;
		return vertex;
	}

	public int TransformAxi(int axi, ref PatternMatch match)
	{
		//reflect x
		if (match.invx) {
			axi = Vx.XREFLAXI[0, axi];
		}
		//reflect y
		if (match.invy) {
			axi = Vx.YREFLAXI[axi];
		}
		//rotate
		axi = Vx.ROTAXI[match.shift, axi];
		return axi;
	}

	private int GetVoxelID(IJL voxel_ijl)
	{
		if (!_voxeldata.ContainsKey(voxel_ijl)) {
			return 0;
		}
		return _voxeldata[voxel_ijl].id;
	}

	private static bool IsBlockKeyTopHeavy(IJL block_key)
	{
		int rem_i = (block_key.i + 2) % 6;
		int rem_j = ((3 * block_key.j) - block_key.i - 8) % 12;
		return (rem_i == 0) && (rem_j == 0);
	}

	private bool ShouldDrawBlock(IJL block_key, bool topheavy)
	{
		int d0 = 4; int d1 = 0; int d2 = 2;
		if (!topheavy) {
			d0 += 1; d1 += 1; d2 += 1;
		}
		//find above voxel keys in proper order
		IJL voxel_key0 = block_key + Vx.KeyTransform_VoxelBlock[d0, 0];
		IJL voxel_key1 = block_key + Vx.KeyTransform_VoxelBlock[d1, 0];
		IJL voxel_key2 = block_key + Vx.KeyTransform_VoxelBlock[d2, 0];
		//encode above level
		int encoding = EncodeVoxelLevel(voxel_key0, voxel_key1, voxel_key2);
		//if encoding isn't zero then return ShouldDrawCornerLevel
		if (encoding != 0) {
			return ShouldDrawLevel(encoding, topheavy);
		}
		//find below voxel keys in proper order
		voxel_key0 = block_key + Vx.KeyTransform_VoxelBlock[d0, 1];
		voxel_key1 = block_key + Vx.KeyTransform_VoxelBlock[d1, 1];
		voxel_key2 = block_key + Vx.KeyTransform_VoxelBlock[d2, 1];
		//encode below level
		encoding = EncodeVoxelLevel(voxel_key0, voxel_key1, voxel_key2);
		//return ShouldDrawCornerLevel
		return ShouldDrawLevel(encoding, topheavy);
	}

	private int EncodeVoxelLevel(IJL voxel_key0, IJL voxel_key1, IJL voxel_key2)
	{
		int encoding = 0;
		IJL voxel_ijl0 = Vx.VoxelKeyIJL(voxel_key0);
		if (_voxeldata.ContainsKey(voxel_ijl0)) {
			if (_voxeldata[voxel_ijl0].draw) {
				encoding += 9;
			}
			else {
				encoding += 18;
			}
		}
		IJL voxel_ijl1 = Vx.VoxelKeyIJL(voxel_key1);
		if (_voxeldata.ContainsKey(voxel_ijl1)) {
			if (_voxeldata[voxel_ijl1].draw) {
				encoding += 3;
			}
			else {
				encoding += 6;
			}
		}
		IJL voxel_ijl2 = Vx.VoxelKeyIJL(voxel_key2);
		if (_voxeldata.ContainsKey(voxel_ijl2)) {
			if (_voxeldata[voxel_ijl2].draw) {
				encoding += 1;
			}
			else {
				encoding += 2;
			}
		}
		return encoding;
	}

	private bool ShouldDrawLevel(int encoded_level, bool topheavy)
	{
		switch (encoded_level) {
			case 1:
				return true;
			case 3:
				return true;
			case 4:
				return true;
			case 5:
				return topheavy;
			case 7:
				return !topheavy;
			case 9:
				return true;
			case 10:
				return true;
			case 11:
				return topheavy;
			case 12:
				return true;
			case 13:
				return true;
			case 14:
				return topheavy;
			case 15:
				return true;
			case 16:
				return true;
			case 17:
				return topheavy;
			case 19:
				return !topheavy;
			case 22:
				return !topheavy;
			case 25:
				return !topheavy;
			default:
				return false;
		}
	}
}


public struct BData
{
	public VoxelDesign design;
	public PatternMatch match;

	public BData(VoxelDesign design, PatternMatch match)
	{
		this.design = design;
		this.match = match;
	}
}
