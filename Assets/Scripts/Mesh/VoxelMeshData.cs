using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMeshData
{
	public bool flat_shaded;
	public MapObject map;

	public List<Vector3> vertices;
	public List<Vector3> normals;
	public List<int> triangles;

	//(voxel ijl => VoxelSlot)
	private Dictionary<IJL, VoxelSlot> _voxelslots;
	//(component key => component data)
	private Dictionary<IJL, ComponentData> _cornerdata;
	private Dictionary<IJL, ComponentData> _lateraldata;
	private Dictionary<IJL, ComponentData> _longitudedata;
	private Dictionary<IJL, ComponentData> _rectdata;
	private Dictionary<IJL, ComponentData> _hexagondata;
	//(component key)
	private HashSet<IJL> _dirtycorners;
	private HashSet<IJL> _dirtylaterals;
	private HashSet<IJL> _dirtylongitudes;
	private HashSet<IJL> _dirtyrects;
	private HashSet<IJL> _dirtyhexagons;

	//(vertices index)
	private List<VertexData> _vertexslots;
	private List<TriangleData> _trislots;

	//(corner key => vertex slot index)
	private Dictionary<IJL, int> _cornertable;
	private Dictionary<IJL, int> _edgetable;
	private Dictionary<IJL, int> _facetable;

	public VoxelMeshData()
	{
		vertices = new List<Vector3>();
		normals = new List<Vector3>();
		triangles = new List<int>();
		_vertexslots = new List<VertexData>();
		_trislots = new List<TriangleData>();

		_voxelslots = new Dictionary<IJL, VoxelSlot>();
		_cornerdata = new Dictionary<IJL, ComponentData>();
		_lateraldata = new Dictionary<IJL, ComponentData>();
		_longitudedata = new Dictionary<IJL, ComponentData>();
		_rectdata = new Dictionary<IJL, ComponentData>();
		_hexagondata = new Dictionary<IJL, ComponentData>();

		_dirtycorners = new HashSet<IJL>();
		_dirtylaterals = new HashSet<IJL>();
		_dirtylongitudes = new HashSet<IJL>();
		_dirtyrects = new HashSet<IJL>();
		_dirtyhexagons = new HashSet<IJL>();

		_cornertable = new Dictionary<IJL, int>();
		_edgetable = new Dictionary<IJL, int>();
		_facetable = new Dictionary<IJL, int>();
	}

	public void Clear()
	{
		vertices.Clear();
		normals.Clear();
		triangles.Clear();
		_vertexslots.Clear();
		_trislots.Clear();

		_voxelslots.Clear();
		_cornerdata.Clear();
		_lateraldata.Clear();
		_longitudedata.Clear();
		_rectdata.Clear();
		_hexagondata.Clear();

		_dirtycorners.Clear();
		_dirtylaterals.Clear();
		_dirtylongitudes.Clear();
		_dirtyrects.Clear();
		_dirtyhexagons.Clear();

		_cornertable.Clear();
		_edgetable.Clear();
		_facetable.Clear();
	}

	//Create multi-update version of this
	public void UpdateVoxelData(IJL ijl, Voxel voxel, bool draw_voxel)
	{
		if (_voxelslots.ContainsKey(ijl)) { //voxel at location already exists
			if (voxel.IsEmpty()) {//voxel is empty
				_voxelslots.Remove(ijl);//delete voxel
			}
			else {
				_voxelslots[ijl] = new VoxelSlot(draw_voxel, voxel);//update voxel
			}
			DirtyVoxel(ijl);
		}
		else {//voxel at location does not exist
			if (!voxel.IsEmpty()) {//voxel is not empty
				_voxelslots.Add(ijl, new VoxelSlot(draw_voxel, voxel));//add voxel
				DirtyVoxel(ijl);
			}
		}
	}

	#region Dirty Methods
	public void UpdateDirtyComponents()
	{
		//process dirty corners
		UpdateDirtyCorners();
		//process dirty lateral edges
		UpdateDirtyLaterals();
		//process dirty longitude edges
		UpdateDirtyLongitudes();
		//process dirty rect faces
		UpdateDirtyRectFaces();
		//process dirty hexagon faces
		UpdateDirtyHexagonFaces();
	}
	private void DirtyVoxel(IJL ijl)
	{
		IJL voxel_key = VMD.VoxelKey(ijl);
		//dirty corners
		for (int d = 0; d < 6; d++) {
			for (int lvl = 0;lvl < 2;lvl++) {
				IJL corner_key = voxel_key + VMD.KeyTransform_VoxelCorner[d, lvl];
				//dirty corner
				_dirtycorners.Add(corner_key);
				//dirty adjacent lateral edge
				_dirtylaterals.Add(corner_key + VMD.KeyTransform_CornerLateral[d]);
				//dirty adjacent longitude edge
				_dirtylongitudes.Add(corner_key + VMD.KeyTransform_CornerLongitude[lvl]);
				//dirty adjecent rect faces
				_dirtyrects.Add(corner_key + VMD.KeyTransform_CornerRect[d, lvl]);
			}
		}
		//dirty lateral edges
		for (int d = 0;d < 6;d++) {
			for (int lvl = 0; lvl < 2; lvl++) {
				IJL lateral_key = voxel_key + VMD.KeyTransform_VoxelLateral[d, lvl];
				//dirty lateral edge
				_dirtylaterals.Add(lateral_key);
				//dirty adjecent rect faces
				_dirtyrects.Add(lateral_key + VMD.KeyTransform_LateralRect[lvl]);
				//dirty adjacent hexagon faces
				_dirtyhexagons.Add(lateral_key + VMD.KeyTransform_LateralHexagon[d]);
			}
		}
		//dirty longitude edges
		for (int d = 0;d < 6;d++) {
			IJL long_key = voxel_key + VMD.KeyTransform_VoxelLongitude[d];
			//dirty longitude edge
			_dirtylongitudes.Add(long_key);
			//dirty adjacent rect face
			_dirtyrects.Add(long_key + VMD.KeyTransform_LongitudeRect[d]);
		}

		//dirty rect faces
		for (int d = 0;d < 6;d++) {
			//dirty rect face
			_dirtyrects.Add(voxel_key + VMD.KeyTransform_VoxelRect[d]);
		}
		//dirty above hexagon face
		_dirtyhexagons.Add(voxel_key + VMD.KeyTransform_VoxelHexagon[0]);
		//dirty below hexagon face
		_dirtyhexagons.Add(voxel_key + VMD.KeyTransform_VoxelHexagon[1]);
	}
	private void UpdateDirtyCorners()
	{
		foreach (IJL corner_key in _dirtycorners) {
			ComponentData data = ClassifyCorner(corner_key);
			if (_cornerdata.ContainsKey(corner_key)) {
				if (data.id == 0) {
					//delete component data
					_cornerdata.Remove(corner_key);
				}
				else {
					//update component data
					_cornerdata[corner_key] = data;
				}
			}
			else {
				if (data.id != 0) {
					//add component data
					_cornerdata.Add(corner_key, data);
				}
			}
		}
		_dirtycorners.Clear();
	}
	private void UpdateDirtyLaterals()
	{
		//process dirty edges
		foreach (IJL edge_key in _dirtylaterals) {
			ComponentData data = ClassifyLateralEdge(edge_key);
			if (_lateraldata.ContainsKey(edge_key)) {
				if (data.id == 0) {
					//delete component data
					_lateraldata.Remove(edge_key);
				}
				else {
					//update component data
					_lateraldata[edge_key] = data;
				}
			}
			else {
				if (data.id != 0) {
					//add component data
					_lateraldata.Add(edge_key, data);
				}
			}
		}
		_dirtylaterals.Clear();
	}
	private void UpdateDirtyLongitudes()
	{
		//process dirty edges
		foreach (IJL edge_key in _dirtylongitudes) {
			ComponentData data = ClassifyLongitudeEdge(edge_key);
			if (_longitudedata.ContainsKey(edge_key)) {
				if (data.id == 0) {
					//delete component data
					_longitudedata.Remove(edge_key);
				}
				else {
					//update component data
					_longitudedata[edge_key] = data;
				}
			}
			else {
				if (data.id != 0) {
					//add component data
					_longitudedata.Add(edge_key, data);
				}
			}
		}
		_dirtylongitudes.Clear();
	}
	private void UpdateDirtyRectFaces()
	{
		foreach (IJL face_key in _dirtyrects) {
			ComponentData data = ClassifyRectFace(face_key);
			if (_rectdata.ContainsKey(face_key)) {
				if (data.id == 0) {
					//delete component data
					_rectdata.Remove(face_key);
				}
				else {
					//update component data
					_rectdata[face_key] = data;
				}
			}
			else {
				if (data.id != 0) {
					//add component data
					_rectdata.Add(face_key, data);
				}
			}
		}
		_dirtyrects.Clear();
	}
	private void UpdateDirtyHexagonFaces()
	{
		foreach (IJL face_key in _dirtyhexagons) {
			ComponentData data = ClassifyHexagonFace(face_key);
			if (_hexagondata.ContainsKey(face_key)) {
				if (data.id == 0) {
					//delete component data
					_hexagondata.Remove(face_key);
				}
				else {
					//update component data
					_hexagondata[face_key] = data;
				}
			}
			else {
				if (data.id != 0) {
					//add component data
					_hexagondata.Add(face_key, data);
				}
			}
		}
		_dirtyhexagons.Clear();
	}
	#endregion

	#region Classify Corner
	private ComponentData ClassifyCorner(IJL corner_key)
	{
		if (map == null) {
			return ComponentData.empty;
		}
		CornerPattern corner_pat = GetCornerPattern(corner_key);
		//get id
		int id = map.GetCornerID(corner_pat);
		if (id <= 0) {
			return ComponentData.empty;
		}
		//get rotation shift
		CornerPattern map_pat = map.GetCornerPattern(corner_pat);
		PatternMatch match = CornerPattern.CalculateTransform(map_pat, corner_pat, VMD.IsKeyTopHeavy(corner_key));
		if (match.shift < 0) {
			return ComponentData.empty;
		}
		//calculate draw
		bool draw = ShouldDrawCorner(corner_key);
		//calculate vertex
		Vector3 vertex = VMD.KeyVertex(corner_key);
		return new ComponentData(draw, id, vertex, match);
	}
	private CornerPattern GetCornerPattern(IJL corner_key)
	{
		//find voxel keys in proper order
		int d0 = 4;int d1 = 0;int d2 = 2;
		if (!VMD.IsKeyTopHeavy(corner_key)) {
			d0 += 1;d1 += 1;d2 += 1;
		}
		//Above 0
		IJL voxel_key = VMD.VoxelKeyIJL(corner_key + VMD.KeyTransform_CornerVoxel[d0, 0]);
		int a0 = GetVoxelID(voxel_key);
		//Above 1
		voxel_key = VMD.VoxelKeyIJL(corner_key + VMD.KeyTransform_CornerVoxel[d1, 0]);
		int a1 = GetVoxelID(voxel_key);
		//Above 2
		voxel_key = VMD.VoxelKeyIJL(corner_key + VMD.KeyTransform_CornerVoxel[d2, 0]);
		int a2 = GetVoxelID(voxel_key);
		//Below 0
		voxel_key = VMD.VoxelKeyIJL(corner_key + VMD.KeyTransform_CornerVoxel[d0, 1]);
		int b0 = GetVoxelID(voxel_key);
		//Below 1
		voxel_key = VMD.VoxelKeyIJL(corner_key + VMD.KeyTransform_CornerVoxel[d1, 1]);
		int b1 = GetVoxelID(voxel_key);
		//Below 2
		voxel_key = VMD.VoxelKeyIJL(corner_key + VMD.KeyTransform_CornerVoxel[d2, 1]);
		int b2 = GetVoxelID(voxel_key);
		return new CornerPattern(a0, a1, a2, b0, b1, b2);
	}
	private bool ShouldDrawCorner(IJL corner_key)
	{
		bool topheavy = VMD.IsKeyTopHeavy(corner_key);
		int d0 = 4; int d1 = 0; int d2 = 2;
		if (!topheavy) {
			d0 += 1;d1 += 1;d2 += 1;
		}
		//find above voxel keys in proper order
		IJL voxel_key0 = corner_key + VMD.KeyTransform_CornerVoxel[d0, 0];
		IJL voxel_key1 = corner_key + VMD.KeyTransform_CornerVoxel[d1, 0];
		IJL voxel_key2 = corner_key + VMD.KeyTransform_CornerVoxel[d2, 0];
		//encode above level
		int encoding = EncodeVoxelLevel(voxel_key0, voxel_key1, voxel_key2);
		//if encoding isn't zero then return ShouldDrawCornerLevel
		if (encoding != 0) {
			return ShouldDrawTripleLevel(encoding, topheavy);
		}
		//find below voxel keys in proper order
		voxel_key0 = corner_key + VMD.KeyTransform_CornerVoxel[d0, 1];
		voxel_key1 = corner_key + VMD.KeyTransform_CornerVoxel[d1, 1];
		voxel_key2 = corner_key + VMD.KeyTransform_CornerVoxel[d2, 1];
		//encode below level
		encoding = EncodeVoxelLevel(voxel_key0, voxel_key1, voxel_key2);
		//return ShouldDrawCornerLevel
		return ShouldDrawTripleLevel(encoding, topheavy);
	}
	#endregion

	#region Classify Lateral Edge
	private ComponentData ClassifyLateralEdge(IJL edge_key)
	{
		if (map == null) {
			return ComponentData.empty;
		}
		int axi = VMD.ClassifyLatticKeyAxi(edge_key);
		if (axi < 0) {
			return ComponentData.empty;
		}
		//get pattern from voxel data
		LateralPattern edge_pat = GetLateralPattern(edge_key, axi);
		//get pattern from map
		LateralPattern map_pat = map.GetLateralPattern(edge_pat);
		//get id
		int id = map.GetLateralID(map_pat);
		if (id <= 0) {
			return ComponentData.empty;
		}
		//calculate shift
		PatternMatch match = LateralPattern.CalculateTransform(map_pat, edge_pat, axi - 2);
		if (match.shift < 0) {
			return ComponentData.empty;
		}
		//calculate draw
		bool draw = ShouldDrawLateral(edge_key, match.shift + 2);
		//calculate vertex
		Vector3 vertex = VMD.KeyVertex(edge_key);
		return new ComponentData(draw, id, vertex, match);
	}
	private LateralPattern GetLateralPattern(IJL edge_key, int axi)
	{
		if (map == null || axi < 2 || axi > 7) {
			return LateralPattern.empty;
		}
		//corner plugs
		KeyPacket2 cornerpacket = VMD.GetLateralCornerKeys(edge_key, axi);
		if (!cornerpacket.valid) {
			return LateralPattern.empty;
		}
		int p0 = GetEdgeSocketPlug(0, ref cornerpacket);
		int p1 = GetEdgeSocketPlug(1, ref cornerpacket);

		int d = axi - 2;
		int oppd = Vx.D[3, d];
		//Above 0
		IJL voxel_key = VMD.VoxelKeyIJL(edge_key + VMD.KeyTransform_LateralVoxel[oppd, 0]);
		int a0 = GetVoxelID(voxel_key);
		//Below 0
		voxel_key = VMD.VoxelKeyIJL(edge_key + VMD.KeyTransform_LateralVoxel[oppd, 1]);
		int b0 = GetVoxelID(voxel_key);
		//Above 1
		voxel_key = VMD.VoxelKeyIJL(edge_key + VMD.KeyTransform_LateralVoxel[d, 0]);
		int a1 = GetVoxelID(voxel_key);
		//Below 1
		voxel_key = VMD.VoxelKeyIJL(edge_key + VMD.KeyTransform_LateralVoxel[d, 1]);
		int b1 = GetVoxelID(voxel_key);
		return new LateralPattern(false, false, p0, p1, a0, b0, a1, b1);
	}
	private bool ShouldDrawLateral(IJL edge_key, int axi)
	{
		if (axi < 2 || axi > 7) {
			return false;
		}
		int d = axi - 2;
		int oppd = Vx.D[3, d];
		IJL voxel_key0 = edge_key + VMD.KeyTransform_LateralVoxel[oppd, 0];
		IJL voxel_key1 = edge_key + VMD.KeyTransform_LateralVoxel[d, 0];
		//encode above level
		int encoding = EncodeVoxelLevel(voxel_key0, voxel_key1);
		//if encoding isn't zero then return ShouldDrawCornerLevel
		if (encoding != 0) {
			return ShouldDrawDoubleLevel(encoding, axi);
		}
		//find below voxel keys in proper order
		voxel_key0 = edge_key + VMD.KeyTransform_LateralVoxel[oppd, 1];
		voxel_key1 = edge_key + VMD.KeyTransform_LateralVoxel[d, 1];
		//encode below level
		encoding = EncodeVoxelLevel(voxel_key0, voxel_key1);
		//return ShouldDrawCornerLevel
		return ShouldDrawDoubleLevel(encoding, axi);
	}
	#endregion

	#region Classify Longitude Edge
	private ComponentData ClassifyLongitudeEdge(IJL edge_key)
	{
		if (map == null) {
			return ComponentData.empty;
		}
		//get pattern from voxel data
		LongitudePattern edge_pat = GetLongitudePattern(edge_key, 0);
		//get pattern from map
		LongitudePattern map_pat = map.GetLongitudePattern(edge_pat);
		//get id
		int id = map.GetLongitudeID(map_pat);
		if (id <= 0) {
			return ComponentData.empty;
		}
		//calculate shift
		PatternMatch match = LongitudePattern.CalculateTransform(map_pat, edge_pat, VMD.IsKeyTopHeavy(edge_key));
		if (match.shift < 0) {
			return ComponentData.empty;
		}
		//calculate draw
		bool draw = ShouldDrawLongitude(edge_key);
		//calculate vertex
		Vector3 vertex = VMD.KeyVertex(edge_key);
		return new ComponentData(draw, id, vertex, match);
	}
	private LongitudePattern GetLongitudePattern(IJL edge_key, int axi)
	{
		if (map == null || axi < 0 || axi > 1) {
			return LongitudePattern.empty;
		}
		//corner plugs
		KeyPacket2 cornerpacket = VMD.GetLongitudeCornerKeys(edge_key, axi);
		if (!cornerpacket.valid) {
			return LongitudePattern.empty;
		}
		int p0 = GetEdgeSocketPlug(0, ref cornerpacket);
		int p1 = GetEdgeSocketPlug(1, ref cornerpacket);

		int d0 = 4; int d1 = 0; int d2 = 2;
		if (!VMD.IsKeyTopHeavy(edge_key)) {
			d0 += 1;d1 += 1;d2 += 1;
		}
		//Inside
		IJL voxel_ijl = VMD.VoxelKeyIJL(edge_key + VMD.KeyTransform_LongitudeVoxel[d0]);
		int vin = GetVoxelID(voxel_ijl);
		//Outside 0
		voxel_ijl = VMD.VoxelKeyIJL(edge_key + VMD.KeyTransform_LongitudeVoxel[d1]);
		int vout0 = GetVoxelID(voxel_ijl);
		//Outside 1
		voxel_ijl = VMD.VoxelKeyIJL(edge_key + VMD.KeyTransform_LongitudeVoxel[d2]);
		int vout1 = GetVoxelID(voxel_ijl);
		return new LongitudePattern(false, false, p0, p1, vin, vout0, vout1);
	}
	private bool ShouldDrawLongitude(IJL edge_key)
	{
		bool topheavy = VMD.IsKeyTopHeavy(edge_key);
		int d0 = 4; int d1 = 0; int d2 = 2;
		if (!topheavy) {
			d0 += 1; d1 += 1; d2 += 1;
		}
		//find voxel keys in proper order
		IJL voxel_key0 = edge_key + VMD.KeyTransform_LongitudeVoxel[d0];
		IJL voxel_key1 = edge_key + VMD.KeyTransform_LongitudeVoxel[d1];
		IJL voxel_key2 = edge_key + VMD.KeyTransform_LongitudeVoxel[d2];
		//encode above level
		int encoding = EncodeVoxelLevel(voxel_key0, voxel_key1, voxel_key2);
		return ShouldDrawTripleLevel(encoding, topheavy);
	}
	#endregion

	#region Classify Rect Face
	private ComponentData ClassifyRectFace(IJL face_key)
	{
		if (map == null) {
			return ComponentData.empty;
		}
		int axi = VMD.ClassifyLatticKeyAxi(face_key);
		if (axi < 0) {
			return ComponentData.empty;
		}
		//get pattern from voxel data
		RectPattern face_pat = GetRectPattern(face_key, axi);
		//get pattern from map
		RectPattern map_pat = map.GetRectPattern(face_pat);
		//get id
		int id = map.GetRectID(map_pat);
		if (id <= 0) {
			return ComponentData.empty;
		}
		//pattern match
		PatternMatch match = RectPattern.CalculateTransform(map_pat, face_pat, axi - 2);
		if (match.shift < 0) {
			return ComponentData.empty;
		}
		//draw
		bool draw = ShouldDrawRect(face_key, match.shift + 2);
		//vertex
		Vector3 vertex = VMD.KeyVertex(face_key);
		return new ComponentData(draw, id, vertex, match);
	}
	private RectPattern GetRectPattern(IJL face_key, int axi)
	{
		if (map == null || axi < 2 || axi > 7) {
			return RectPattern.empty;
		}
		//corner plugs
		KeyPacket4 cornerpacket = VMD.GetRectCornerKeys(face_key, axi);
		int a0 = GetRectSocketPlug(0, ref cornerpacket);
		int b0 = GetRectSocketPlug(1, ref cornerpacket);
		int b1 = GetRectSocketPlug(2, ref cornerpacket);
		int a1 = GetRectSocketPlug(3, ref cornerpacket);

		int d = axi - 2;
		int oppd = Vx.D[3, d];
		//inside voxel
		IJL voxel_key = VMD.VoxelKeyIJL(face_key + VMD.KeyTransform_RectVoxel[oppd]);
		int vin = GetVoxelID(voxel_key);
		//outside voxel
		voxel_key = VMD.VoxelKeyIJL(face_key + VMD.KeyTransform_RectVoxel[d]);
		int vout = GetVoxelID(voxel_key);
		return new RectPattern(false, false, a0, b0, a1, b1, vin, vout);
	}
	private int GetRectSocketPlug(int index, ref KeyPacket4 cornerpacket)
	{
		if (!cornerpacket.valid || index < 0 || index > 3) {
			return -1;
		}
		KeyPacket packet;
		if (index == 0) {
			packet = cornerpacket.pk0;
		}
		else if (index == 1) {
			packet = cornerpacket.pk1;
		}
		else if (index == 2) {
			packet = cornerpacket.pk2;
		}
		else {
			packet = cornerpacket.pk3;
		}
		if (!_cornerdata.ContainsKey(packet.key)) {
			return -1;
		}
		ComponentData data = _cornerdata[packet.key];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			return -1;
		}
		int socketaxi = Vx.NROTAXI[data.match.shift, packet.socketaxi];
		return design.GetFaceSocketCount(data.match.invx, data.match.invy, packet.socketlevel, socketaxi);
	}
	private bool ShouldDrawRect(IJL face_key, int axi)
	{
		if (axi < 2 || axi > 7) {
			return false;
		}
		int d = axi - 2;
		int oppd = Vx.D[3, d];
		//find voxel keys in proper order
		IJL voxel_key0 = face_key + VMD.KeyTransform_RectVoxel[oppd];
		IJL voxel_key1 = face_key + VMD.KeyTransform_RectVoxel[d];
		//encode above level
		int encoding = EncodeVoxelLevel(voxel_key0, voxel_key1);
		return ShouldDrawDoubleLevel(encoding, axi);
	}
	#endregion

	#region Classify Hexagon Face
	private ComponentData ClassifyHexagonFace(IJL face_key)
	{
		if (map == null) {
			return ComponentData.empty;
		}
		//get pattern from voxel data
		HexagonPattern face_pat = GetHexagonPattern(face_key, 0);
		//get pattern from map
		HexagonPattern map_pat = map.GetHexagonPattern(face_pat);
		//get id
		int id = map.GetHexagonID(map_pat);
		if (id <= 0) {
			return ComponentData.empty;
		}
		//pattern match
		PatternMatch match = HexagonPattern.CalculateTransform(map_pat, face_pat);
		if (match.shift < 0) {
			return ComponentData.empty;
		}
		//draw
		bool draw = ShouldDrawHexagon(face_key);
		//vertex
		Vector3 vertex = VMD.KeyVertex(face_key);
		return new ComponentData(draw, id, vertex, match);
	}
	private HexagonPattern GetHexagonPattern(IJL face_key, int axi)
	{
		if (map == null || axi < 0 || axi > 1) {
			return HexagonPattern.empty;
		}
		//corner plugs
		KeyPacket6 cornerpacket = VMD.GetHexagonCornerKeys(face_key, axi);
		int p0 = GetHexagonSocketPlug(0, ref cornerpacket);
		int p1 = GetHexagonSocketPlug(1, ref cornerpacket);
		int p2 = GetHexagonSocketPlug(2, ref cornerpacket);
		int p3 = GetHexagonSocketPlug(3, ref cornerpacket);
		int p4 = GetHexagonSocketPlug(4, ref cornerpacket);
		int p5 = GetHexagonSocketPlug(5, ref cornerpacket);
		//above voxel
		IJL voxel_key = VMD.VoxelKeyIJL(face_key + VMD.KeyTransform_HexagonVoxel[0]);
		int va = GetVoxelID(voxel_key);
		//below voxel
		voxel_key = VMD.VoxelKeyIJL(face_key + VMD.KeyTransform_HexagonVoxel[1]);
		int vb = GetVoxelID(voxel_key);
		return new HexagonPattern(false, false, p0, p1, p2, p3, p4, p5, va, vb);
	}
	private int GetHexagonSocketPlug(int index, ref KeyPacket6 cornerpacket)
	{
		if (!cornerpacket.valid || index < 0 || index > 5) {
			return -1;
		}
		KeyPacket packet;
		if (index == 0) {
			packet = cornerpacket.pk0;
		}
		else if (index == 1) {
			packet = cornerpacket.pk1;
		}
		else if (index == 2) {
			packet = cornerpacket.pk2;
		}
		else if (index == 3) {
			packet = cornerpacket.pk3;
		}
		else if (index == 4) {
			packet = cornerpacket.pk4;
		}
		else {
			packet = cornerpacket.pk5;
		}
		if (!_cornerdata.ContainsKey(packet.key)) {
			return -1;
		}
		ComponentData data = _cornerdata[packet.key];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			return -1;
		}
		int socketaxi = Vx.NROTAXI[data.match.shift, packet.socketaxi];
		return design.GetFaceSocketCount(data.match.invx, data.match.invy, packet.socketlevel, socketaxi);
	}
	private bool ShouldDrawHexagon(IJL face_key)
	{
		//find above voxel key
		IJL voxel_key = face_key + VMD.KeyTransform_HexagonVoxel[0];
		//encode above level
		int encoding = EncodeVoxelLevel(voxel_key);
		if (encoding != 0) {
			return (encoding == 1);
		}
		//find below voxel key
		voxel_key = face_key + VMD.KeyTransform_HexagonVoxel[1];
		//encode below level
		encoding = EncodeVoxelLevel(voxel_key);
		return (encoding == 1);
	}
	#endregion

	#region Classify General
	private int GetVoxelID(IJL voxel_key)
	{
		if (_voxelslots.ContainsKey(voxel_key)) {
			return (int)_voxelslots[voxel_key].voxel.id;
		}
		return 0;
	}
	private int EncodeVoxelLevel(IJL voxel_key0)
	{
		int encoding = 0;
		IJL voxel_ijl0 = VMD.VoxelKeyIJL(voxel_key0);
		if (_voxelslots.ContainsKey(voxel_ijl0)) {
			if (_voxelslots[voxel_ijl0].draw) {
				encoding += 1;
			}
			else {
				encoding += 2;
			}
		}
		return encoding;
	}
	private int EncodeVoxelLevel(IJL voxel_key0, IJL voxel_key1)
	{
		int encoding = 0;
		IJL voxel_ijl0 = VMD.VoxelKeyIJL(voxel_key0);
		if (_voxelslots.ContainsKey(voxel_ijl0)) {
			if (_voxelslots[voxel_ijl0].draw) {
				encoding += 1;
			}
			else {
				encoding += 2;
			}
		}
		IJL voxel_ijl1 = VMD.VoxelKeyIJL(voxel_key1);
		if (_voxelslots.ContainsKey(voxel_ijl1)) {
			if (_voxelslots[voxel_ijl1].draw) {
				encoding += 3;
			}
			else {
				encoding += 6;
			}
		}
		return encoding;
	}
	private int EncodeVoxelLevel(IJL voxel_key0, IJL voxel_key1, IJL voxel_key2)
	{
		int encoding = 0;
		IJL voxel_ijl0 = VMD.VoxelKeyIJL(voxel_key0);
		if (_voxelslots.ContainsKey(voxel_ijl0)) {
			if (_voxelslots[voxel_ijl0].draw) {
				encoding += 9;
			}
			else {
				encoding += 18;
			}
		}
		IJL voxel_ijl1 = VMD.VoxelKeyIJL(voxel_key1);
		if (_voxelslots.ContainsKey(voxel_ijl1)) {
			if (_voxelslots[voxel_ijl1].draw) {
				encoding += 3;
			}
			else {
				encoding += 6;
			}
		}
		IJL voxel_ijl2 = VMD.VoxelKeyIJL(voxel_key2);
		if (_voxelslots.ContainsKey(voxel_ijl2)) {
			if (_voxelslots[voxel_ijl2].draw) {
				encoding += 1;
			}
			else {
				encoding += 2;
			}
		}
		return encoding;
	}
	private int GetEdgeSocketPlug(int index, ref KeyPacket2 cornerpacket)
	{
		if (!cornerpacket.valid || index < 0 || index > 1) {
			return -1;
		}
		KeyPacket packet;
		if (index == 0) {
			packet = cornerpacket.pk0;
		}
		else {
			packet = cornerpacket.pk1;
		}
		if (!_cornerdata.ContainsKey(packet.key)) {
			return -1;
		}
		ComponentData data = _cornerdata[packet.key];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			return -1;
		}
		PatternMatch match = data.match;
		int socketaxi = Vx.NROTAXI[match.shift, packet.socketaxi];
		return design.GetEdgeSocketCount(data.match.invx, data.match.invy, socketaxi);
	}
	private bool ShouldDrawDoubleLevel(int encoded_level, int axi)
	{
		switch (encoded_level) {
			case 1:
			case 3:
			case 4:
				return true;
			case 5:
				return (axi == 1) || (axi == 2) || (axi == 3) || (axi == 4);
			case 7:
				return (axi == 0) || (axi == 5) || (axi == 6) || (axi == 7);
			default:
				return false;
		}
	}
	private bool ShouldDrawTripleLevel(int encoded_level, bool topheavy)
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
	#endregion

	public void GenerateMesh()
	{
		_vertexslots.Clear();
		_trislots.Clear();
		_cornertable.Clear();
		if (map == null) {
			return;
		}
		GenerateCornerMesh();
		GenerateLateralMesh();
		GenerateLongitudeMesh();
		GenerateRectMesh();
		GenerateHexagonMesh();

		ExtractMeshData();
	}

	private void ExtractMeshData()
	{
		vertices.Clear();
		normals.Clear();
		triangles.Clear();
		for (int k = 0; k < _vertexslots.Count; k++) {
			VertexData data = _vertexslots[k];
			if (data.hastri) {
				vertices.Add(data.vertex);
				normals.Add(data.normal.normalized);
				_vertexslots[k] = new VertexData(data.hastri, vertices.Count - 1, data.vertex, data.normal);
			}
		}
		int vertex_count = vertices.Count;
		for (int k = 0; k < _trislots.Count; k++) {
			TriangleData data = _trislots[k];
			int slot0_index = data.vertex0;
			int slot1_index = data.vertex1;
			int slot2_index = data.vertex2;
			if (slot0_index < 0 || slot1_index < 0 || slot2_index < 0) {
				continue;
			}
			int tri_index0 = _vertexslots[data.vertex0].index;
			if (tri_index0 < 0 || tri_index0 >= vertex_count) {
				continue;
			}
			int tri_index1 = _vertexslots[data.vertex1].index;
			if (tri_index1 < 0 || tri_index1 >= vertex_count) {
				continue;
			}
			int tri_index2 = _vertexslots[data.vertex2].index;
			if (tri_index2 < 0 || tri_index2 >= vertex_count) {
				continue;
			}
			triangles.Add(tri_index0);
			triangles.Add(tri_index1);
			triangles.Add(tri_index2);
		}
	}


	private Vector3 TransformVertex(Vector3 vertex, Vector3 refl, ref PatternMatch match)
	{
		//reflect x
		if (match.invx) {
			float y = vertex.y;
			vertex = (2 * Vector3.Dot(refl, vertex) * refl) - vertex;
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

	private void GenerateCornerMesh()
	{
		foreach (KeyValuePair<IJL, ComponentData> data in _cornerdata) {
			CornerDesign design = map.GetCorner(data.Value.id);
			if (design == null || !design.IsValid()) {
				continue;
			}
			bool draw = data.Value.draw;
			Vector3 corner_vertex = data.Value.vertex;
			PatternMatch match = data.Value.match;
			List<Vector3> corner_vertices = design.vertices;
			int vertex_count = corner_vertices.Count;
			//register corner
			int cornerslot_index = _vertexslots.Count;
			if (vertex_count > 0) {
				_cornertable.Add(data.Key, cornerslot_index);
			}
			//add vertices
			for (int k = 0; k < vertex_count; k++) {
				Vector3 vertex = TransformVertex(corner_vertices[k], Vx.ReflVector, ref match);
				_vertexslots.Add(new VertexData(corner_vertex + vertex));
			}
			if (flat_shaded && !draw) {
				continue;
			}
			//add triangles
			bool fliptri = (match.invx && !match.invy) || (!match.invx && match.invy);
			List<Triangle> corner_triangles = design.triangles;
			for (int k = 0; k < corner_triangles.Count; k++) {
				Triangle tri = corner_triangles[k];
				//check validity
				if (!tri.IsValid(vertex_count, null, null)) {
					continue;
				}
				int slot0_index = cornerslot_index + tri.vertex0;
				int slot1_index = cornerslot_index + tri.vertex1;
				int slot2_index = cornerslot_index + tri.vertex2;
				AddTriangle(slot0_index, slot1_index, slot2_index, fliptri, draw);
			}
		}
	}

	private void GenerateLateralMesh()
	{
		CornerPacket2 edgecorners;
		foreach (KeyValuePair<IJL, ComponentData> data in _lateraldata) {
			LateralDesign design = map.GetLateral(data.Value.id);
			if (design == null || !design.IsValid()) {
				continue;
			}
			bool draw = data.Value.draw;
			Vector3 lateral_vertex = data.Value.vertex;
			PatternMatch match = data.Value.match;
			List<Vector3> lateral_vertices = design.vertices;
			int vertex_count = lateral_vertices.Count;
			int edge_index = _vertexslots.Count;
			//add vertices
			for (int k = 0; k < vertex_count; k++) {
				Vector3 vertex = TransformVertex(lateral_vertices[k], Vector3.right, ref match);
				_vertexslots.Add(new VertexData(lateral_vertex + vertex));
			}
			if (flat_shaded && !draw) {
				continue;
			}
			GetLateralCornerPackets(data.Key, ref match, out edgecorners);
			if (!edgecorners.valid) {
				continue;
			}
			//add triangles
			bool fliptri = (match.invx && !match.invy) || (!match.invx && match.invy);
			List<int> cornerplugs = design.cornerplugs;
			List<int> edgeplugs = design.edgeplugs;
			List<Triangle> lateral_triangles = design.triangles;
			for (int k = 0; k < lateral_triangles.Count; k++) {
				Triangle tri = lateral_triangles[k];
				//check validity
				if (!tri.IsValid(vertex_count, cornerplugs, edgeplugs)) {
					continue;
				}
				int slot0_index = TriangleSlotIndex(edge_index, tri.type0, tri.vertex0, fliptri, ref edgecorners);
				int slot1_index = TriangleSlotIndex(edge_index, tri.type1, tri.vertex1, fliptri, ref edgecorners);
				int slot2_index = TriangleSlotIndex(edge_index, tri.type2, tri.vertex2, fliptri, ref edgecorners);
				AddTriangle(slot0_index, slot1_index, slot2_index, fliptri, draw);
			}
		}
	}

	private void GenerateLongitudeMesh()
	{
		CornerPacket2 edgecorners;
		foreach (KeyValuePair<IJL, ComponentData> data in _longitudedata) {
			LongitudeDesign design = map.GetLongitude(data.Value.id);
			if (design == null || !design.IsValid()) {
				continue;
			}
			bool draw = data.Value.draw;
			Vector3 long_vertex = data.Value.vertex;
			PatternMatch match = data.Value.match;
			List<Vector3> long_vertices = design.vertices;
			int vertex_count = long_vertices.Count;
			int edge_index = _vertexslots.Count;
			//add vertices
			for (int k = 0; k < vertex_count; k++) {
				Vector3 vertex = TransformVertex(long_vertices[k], Vx.ReflVector, ref match);
				_vertexslots.Add(new VertexData(long_vertex + vertex));
			}
			if (flat_shaded && !draw) {
				continue;
			}
			GetLongitudeCornerPackets(data.Key, ref match, out edgecorners);
			if (!edgecorners.valid) {
				continue;
			}
			//add triangles
			bool fliptri = (match.invx && !match.invy) || (!match.invx && match.invy);
			List<int> cornerplugs = design.cornerplugs;
			List<int> edgeplugs = design.edgeplugs;
			List<Triangle> lateral_triangles = design.triangles;
			for (int k = 0; k < lateral_triangles.Count; k++) {
				Triangle tri = lateral_triangles[k];
				//check validity
				if (!tri.IsValid(vertex_count, cornerplugs, edgeplugs)) {
					continue;
				}
				int slot0_index = TriangleSlotIndex(edge_index, tri.type0, tri.vertex0, fliptri, ref edgecorners);
				int slot1_index = TriangleSlotIndex(edge_index, tri.type1, tri.vertex1, fliptri, ref edgecorners);
				int slot2_index = TriangleSlotIndex(edge_index, tri.type2, tri.vertex2, fliptri, ref edgecorners);
				AddTriangle(slot0_index, slot1_index, slot2_index, fliptri, draw);
			}
		}
	}

	private void GenerateRectMesh()
	{
		CornerPacket4 rectcorners;
		foreach (KeyValuePair<IJL, ComponentData> data in _rectdata) {
			RectDesign design = map.GetRect(data.Value.id);
			if (design == null || !design.IsValid()) {
				continue;
			}
			bool draw = data.Value.draw;
			Vector3 rect_vertex = data.Value.vertex;
			PatternMatch match = data.Value.match;
			List<Vector3> rect_vertices = design.vertices;
			int vertex_count = rect_vertices.Count;
			int rect_index = _vertexslots.Count;
			//add vertices
			for (int k = 0; k < vertex_count; k++) {
				Vector3 vertex = TransformVertex(rect_vertices[k], Vector3.right, ref match);
				_vertexslots.Add(new VertexData(rect_vertex + vertex));
			}
			if (flat_shaded && !draw) {
				continue;
			}
			GetRectCornerPackets(data.Key, ref match, out rectcorners);
			if (!rectcorners.valid) {
				continue;
			}
			//add triangles
			bool fliptri = (!match.invx && match.invy) || (match.invx && !match.invy);
			List<Triangle> rect_triangles = design.triangles;
			for (int k = 0; k < rect_triangles.Count; k++) {
				Triangle tri = rect_triangles[k];
				//check validity
				if (!tri.IsValid(vertex_count, design.cornerplugs, design.edgeplugs)) {
					continue;
				}
				int slot0_index = TriangleSlotIndex(rect_index, tri.type0, tri.vertex0, fliptri, ref match, ref rectcorners);
				int slot1_index = TriangleSlotIndex(rect_index, tri.type1, tri.vertex1, fliptri, ref match, ref rectcorners);
				int slot2_index = TriangleSlotIndex(rect_index, tri.type2, tri.vertex2, fliptri, ref match, ref rectcorners);
				AddTriangle(slot0_index, slot1_index, slot2_index, fliptri, draw);
			}
		}
	}

	private void GenerateHexagonMesh()
	{
		CornerPacket6 hexagoncorners;
		foreach (KeyValuePair<IJL, ComponentData> data in _hexagondata) {
			HexagonDesign design = map.GetHexagon(data.Value.id);
			if (design == null || !design.IsValid()) {
				continue;
			}
			bool draw = data.Value.draw;
			Vector3 hexagon_vertex = data.Value.vertex;
			PatternMatch match = data.Value.match;
			List<Vector3> hexagon_vertices = design.vertices;
			int vertex_count = hexagon_vertices.Count;
			int hexagon_index = _vertexslots.Count;
			//add vertices
			for (int k = 0; k < vertex_count; k++) {
				Vector3 vertex = TransformVertex(hexagon_vertices[k], Vx.ReflVector, ref match);
				_vertexslots.Add(new VertexData(hexagon_vertex + vertex));
			}
			if (flat_shaded && !draw) {
				continue;
			}
			GetHexagonCornerPackets(data.Key, ref match, out hexagoncorners);
			if (!hexagoncorners.valid) {
				continue;
			}
			//add triangles
			bool fliptri = (match.invx && !match.invy) || (!match.invx && match.invy);
			List<Triangle> hexagon_triangles = design.triangles;
			for (int k = 0; k < hexagon_triangles.Count; k++) {
				Triangle tri = hexagon_triangles[k];
				//check validity
				if (!tri.IsValid(vertex_count, design.cornerplugs, design.edgeplugs)) {
					continue;
				}
				int slot0_index = TriangleSlotIndex(hexagon_index, tri.type0, tri.vertex0, fliptri, ref hexagoncorners);
				int slot1_index = TriangleSlotIndex(hexagon_index, tri.type1, tri.vertex1, fliptri, ref hexagoncorners);
				int slot2_index = TriangleSlotIndex(hexagon_index, tri.type2, tri.vertex2, fliptri, ref hexagoncorners);
				AddTriangle(slot0_index, slot1_index, slot2_index, fliptri, draw);
			}
		}
	}


	private void GetHexagonCornerPackets(IJL hexagon_key, ref PatternMatch match, out CornerPacket6 packets)
	{
		int axi = 0;
		if (match.invy) {
			axi = 1;
		}
		KeyPacket6 hexagon_packet = VMD.GetHexagonCornerKeys(hexagon_key, axi);
		KeyPacket packet = hexagon_packet.pk0;
		CornerPacket cp0 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packet = hexagon_packet.pk1;
		CornerPacket cp1 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packet = hexagon_packet.pk2;
		CornerPacket cp2 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packet = hexagon_packet.pk3;
		CornerPacket cp3 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packet = hexagon_packet.pk4;
		CornerPacket cp4 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packet = hexagon_packet.pk5;
		CornerPacket cp5 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packets = new CornerPacket6(true, cp0, cp1, cp2, cp3, cp4, cp5);
	}

	private void GetRectCornerPackets(IJL rect_key, ref PatternMatch match, out CornerPacket4 packets)
	{
		KeyPacket4 corner_packets = VMD.GetRectCornerKeys(rect_key, match.shift + 2);
		KeyPacket packet = corner_packets.pk0;
		CornerPacket cp0 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packet = corner_packets.pk1;
		CornerPacket cp1 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packet = corner_packets.pk2;
		CornerPacket cp2 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packet = corner_packets.pk3;
		CornerPacket cp3 = GetFaceCornerPacket(packet.key, packet.socketlevel, packet.socketaxi);
		packets = new CornerPacket4(true, cp0, cp1, cp2, cp3);
	}

	private void GetLateralCornerPackets(IJL edge_key, ref PatternMatch match, out CornerPacket2 packets)
	{
		KeyPacket2 corner_packets = VMD.GetLateralCornerKeys(edge_key, match.shift + 2);
		KeyPacket packet = corner_packets.pk0;
		CornerPacket cp0 = GetEdgeCornerPacket(packet.key, packet.socketaxi);
		packet = corner_packets.pk1;
		CornerPacket cp1 = GetEdgeCornerPacket(packet.key, packet.socketaxi);
		packets = new CornerPacket2(true, cp0, cp1);
	}

	private void GetLongitudeCornerPackets(IJL edge_key, ref PatternMatch match, out CornerPacket2 packets)
	{
		//transform this
		int axi = 0;
		if (match.invy) {
			axi = 1;
		}
		KeyPacket2 corner_packets = VMD.GetLongitudeCornerKeys(edge_key, axi);
		KeyPacket packet = corner_packets.pk0;
		CornerPacket cp0 = GetEdgeCornerPacket(packet.key, packet.socketaxi);
		packet = corner_packets.pk1;
		CornerPacket cp1 = GetEdgeCornerPacket(packet.key, packet.socketaxi);
		packets = new CornerPacket2(true, cp0, cp1);
	}

	private CornerPacket GetEdgeCornerPacket(IJL corner_key, int axi)
	{
		if (axi < 0 || axi > 7) {
			return CornerPacket.empty;
		}
		//get design
		if (!_cornerdata.ContainsKey(corner_key)) {
			return CornerPacket.empty;
		}
		ComponentData data = _cornerdata[corner_key];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			return CornerPacket.empty;
		}
		//get slotindex
		if (!_cornertable.ContainsKey(corner_key)) {
			return CornerPacket.empty;
		}
		int slotindex = _cornertable[corner_key];
		//get axi
		PatternMatch match = data.match;
		axi = Vx.NROTAXI[match.shift, axi];
		int socketaxi_index = design.EdgeSocketIndex(axi);
		if (socketaxi_index < 0) {
			return CornerPacket.empty;
		}
		return new CornerPacket(design, slotindex, socketaxi_index, match.invx, match.invy);
	}

	private CornerPacket GetFaceCornerPacket(IJL corner_key, int axilevel, int axi)
	{
		if (axi < 0 || axi > 7) {
			return CornerPacket.empty;
		}
		//get design
		if (!_cornerdata.ContainsKey(corner_key)) {
			return CornerPacket.empty;
		}
		ComponentData data = _cornerdata[corner_key];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			return CornerPacket.empty;
		}
		//get slotindex
		if (!_cornertable.ContainsKey(corner_key)) {
			return CornerPacket.empty;
		}
		int slotindex = _cornertable[corner_key];
		//get axi
		PatternMatch match = data.match;
		axi = Vx.NROTAXI[match.shift, axi];
		int socketaxi_index = design.FaceSocketIndex(axilevel, axi);
		if (socketaxi_index < 0) {
			return CornerPacket.empty;
		}
		return new CornerPacket(design, slotindex, socketaxi_index, match.invx, match.invy);
	}

	private int TriangleSlotIndex(int base_index, TriIndexType type, ushort vertex, bool flip, ref CornerPacket2 packet2)
	{
		if (base_index < 0 || !packet2.valid) {
			return -1;
		}
		int axi_index, socket_index, slot_index;
		switch (type) {
			case TriIndexType.CornerPlug:
				axi_index = TriIndex.DecodeAxiIndex(vertex);
				socket_index = TriIndex.DecodeIndex(vertex);
				CornerPacket corner = packet2[axi_index];
				if (!corner.isValid) {
					return -1;
				}
				slot_index = corner.GetEdgeSocketSlotIndex(socket_index, flip);
				break;
			case TriIndexType.EdgePlug:
				return -1;
			default:
				slot_index = base_index + vertex;
				break;
		}
		if (slot_index < 0 || slot_index >= _vertexslots.Count) {
			return -1;
		}
		return slot_index;
	}
	private int TriangleSlotIndex(int base_index, TriIndexType type, ushort vertex, bool flip, ref PatternMatch match, ref CornerPacket4 packet4)
	{
		if (base_index < 0 || !packet4.valid) {
			return -1;
		}
		int axi_index, socket_index, slot_index;
		switch (type) {
			case TriIndexType.CornerPlug:
				axi_index = TriIndex.DecodeAxiIndex(vertex);
				socket_index = TriIndex.DecodeIndex(vertex);
				if (match.invx) {
					switch(axi_index) {
						case 0:
							axi_index = 3;
							break;
						case 1:
							axi_index = 2;
							break;
						case 2:
							axi_index = 1;
							break;
						case 3:
							axi_index = 0;
							break;
					}
				}
				if (match.invy) {
					switch (axi_index) {
						case 0:
							axi_index = 1;
							break;
						case 1:
							axi_index = 0;
							break;
						case 2:
							axi_index = 3;
							break;
						case 3:
							axi_index = 2;
							break;
					}
				}
				CornerPacket corner = packet4[axi_index];
				if (!corner.isValid) {
					return -1;
				}
				slot_index = corner.GetFaceSocketSlotIndex(socket_index, flip);
				break;
			case TriIndexType.EdgePlug:
				return -1;
			default:
				slot_index = base_index + vertex;
				break;
		}
		if (slot_index < 0 || slot_index >= _vertexslots.Count) {
			return -1;
		}
		return slot_index;
	}
	private int TriangleSlotIndex(int base_index, TriIndexType type, ushort vertex, bool flip, ref CornerPacket6 packet6)
	{
		if (base_index < 0 || !packet6.valid) {
			return -1;
		}
		int axi_index, socket_index, slot_index;
		switch (type) {
			case TriIndexType.CornerPlug:
				axi_index = TriIndex.DecodeAxiIndex(vertex);
				socket_index = TriIndex.DecodeIndex(vertex);
				CornerPacket corner = packet6[axi_index];
				if (!corner.isValid) {
					return -1;
				}
				slot_index = corner.GetFaceSocketSlotIndex(socket_index, flip);
				break;
			case TriIndexType.EdgePlug:
				return -1;
			default:
				slot_index = base_index + vertex;
				break;
		}
		if (slot_index < 0 || slot_index >= _vertexslots.Count) {
			return -1;
		}
		return slot_index;
	}

	private void AddTriangle(int slot0_index, int slot1_index, int slot2_index, bool flip, bool draw)
	{
		if (flat_shaded && !draw) {
			return;
		}
		if (slot0_index < 0 || slot1_index < 0 || slot2_index < 0) {
			return;
		}
		int slot_count = _vertexslots.Count;
		if (slot0_index >= slot_count || slot1_index >= slot_count || slot2_index >= slot_count) {
			return;
		}
		VertexData slot0 = _vertexslots[slot0_index];
		VertexData slot1 = _vertexslots[slot1_index];
		VertexData slot2 = _vertexslots[slot2_index];
		Vector3 v0 = slot1.vertex - slot0.vertex;
		Vector3 v1 = slot2.vertex - slot1.vertex;
		Vector3 v2 = slot0.vertex - slot2.vertex;
		Vector3 normal = Vector3.Cross(v0, -v2).normalized;
		if (flip) {
			normal = -normal;
		}
		if (flat_shaded) {
			//vertex slot 0
			if (slot0.hastri) {
				_vertexslots.Add(new VertexData(
					true, -1, slot0.vertex, normal)
				);
				slot0_index = _vertexslots.Count - 1;
			}
			else {
				_vertexslots[slot0_index] = new VertexData(
					true, slot0.index, slot0.vertex, normal
				);
			}
			//vertex slot 1
			if (slot1.hastri) {
				_vertexslots.Add(new VertexData(
					true, -1, slot1.vertex, normal)
				);
				slot1_index = _vertexslots.Count - 1;
			}
			else {
				_vertexslots[slot1_index] = new VertexData(
					true, slot1.index, slot1.vertex, normal
				);
			}
			//vertex slot 2
			if (slot2.hastri) {
				_vertexslots.Add(new VertexData(
					true, -1, slot2.vertex, normal)
				);
				slot2_index = _vertexslots.Count - 1;
			}
			else {
				_vertexslots[slot2_index] = new VertexData(
					true, slot2.index, slot2.vertex, normal
				);
			}
		}
		else {
			//vertex slot 0
			float factor = Vector3.Angle(v0, -v2) / 180f;
			_vertexslots[slot0_index] = new VertexData(
				 draw || slot0.hastri, slot0.index, slot0.vertex, slot0.normal + (factor * normal)
			);
			//vertex slot 1
			factor = Vector3.Angle(v1, -v0) / 180f;
			_vertexslots[slot1_index] = new VertexData(
				 draw || slot1.hastri, slot1.index, slot1.vertex, slot1.normal + (factor * normal)
			);
			//vertex slot 2
			factor = Vector3.Angle(v2, -v1) / 180f;
			_vertexslots[slot2_index] = new VertexData(
				 draw || slot2.hastri, slot2.index, slot2.vertex, slot2.normal + (factor * normal)
			);
		}
		//add triangle
		if (draw) {
			if (flip) {
				_trislots.Add(new TriangleData(slot0_index, slot2_index, slot1_index));
			}
			else {
				_trislots.Add(new TriangleData(slot0_index, slot1_index, slot2_index));
			}
		}
	}
}
