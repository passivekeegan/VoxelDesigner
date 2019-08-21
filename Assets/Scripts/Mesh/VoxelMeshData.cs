using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMeshData
{
	public bool flat_shaded;
	public MappingObject map;

	public List<Vector3> vertices;
	public List<int> triangles;

	//(voxel ijl => VoxelSlot)
	private Dictionary<IJL, VoxelSlot> _voxelslots;
	//(component key => component data)
	private Dictionary<IJL, CornerData> _cornerdata;
	private Dictionary<IJL, EdgeData> _edgedata;
	private Dictionary<IJL, FaceData> _facedata;
	//(component key)
	private HashSet<IJL> _dirtycorners;
	private HashSet<IJL> _dirtyedges;
	private HashSet<IJL> _dirtyfaces;

	//(vertices index)
	private List<VertexSlot> _vertexslots;
	//(corner key => vertex slot index)
	private Dictionary<IJL, int> _cornertable;
	private Dictionary<IJL, int> _edgetable;
	private Dictionary<IJL, int> _facetable;

	public VoxelMeshData()
	{
		vertices = new List<Vector3>();
		triangles = new List<int>();

		_voxelslots = new Dictionary<IJL, VoxelSlot>();
		_cornerdata = new Dictionary<IJL, CornerData>();
		_edgedata = new Dictionary<IJL, EdgeData>();
		_facedata = new Dictionary<IJL, FaceData>();
		
		_dirtycorners = new HashSet<IJL>();
		_dirtyedges = new HashSet<IJL>();
		_dirtyfaces = new HashSet<IJL>();

		_vertexslots = new List<VertexSlot>();
		_cornertable = new Dictionary<IJL, int>();
		_edgetable = new Dictionary<IJL, int>();
		_facetable = new Dictionary<IJL, int>();
	}

	public void Clear()
	{

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

	private void DirtyVoxel(IJL ijl)
	{
		IJL voxel_key = Vx.VoxelKey(ijl);
		//dirty corners
		for (int d = 0;d < 6;d++) {
			_dirtycorners.Add(Vx.CornerKey_Above(voxel_key, d));
			_dirtycorners.Add(Vx.CornerKey_Below(voxel_key, d));
			//dirty ajacent edges
			//dirty adjacent faces
		}
		//dirty edges
		for (int d = 0;d < 6;d++) {
			_dirtyedges.Add(Vx.EdgeKey_Above(voxel_key, d));
			_dirtyedges.Add(Vx.EdgeKey_Middle(voxel_key, d));
			_dirtyedges.Add(Vx.EdgeKey_Below(voxel_key, d));
			//dirty adjacent faces
		}
		//dirty faces
		for (int axi = 0;axi < 8;axi++) {
			_dirtyfaces.Add(Vx.FaceKey(voxel_key, axi));
		}
	}

	public void UpdateDirtyComponents()
	{
		//process dirty corners
		foreach (IJL corner_key in _dirtycorners) {
			CornerData data = ClassifyCorner(corner_key);
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
					//add data
					_cornerdata.Add(corner_key, data);
				}
			}
		}
		_dirtycorners.Clear();

		//process dirty edges
		foreach (IJL edge_key in _dirtyedges) {
			EdgeData data = ClassifyEdge(edge_key);
			if (_edgedata.ContainsKey(edge_key)) {
				if (data.id == 0) {
					//delete component data
					_edgedata.Remove(edge_key);
				}
				else {
					//update component data
					_edgedata[edge_key] = data;
				}
			}
			else {
				if (data.id != 0) {
					//add data
					_edgedata.Add(edge_key, data);
				}
			}
		}
		_dirtyedges.Clear();

		//process dirty faces
		foreach (IJL face_key in _dirtyfaces) {
			FaceData data = ClassifyFace(face_key);
			if (_facedata.ContainsKey(face_key)) {
				if (data.id == 0) {
					//delete component data
					_facedata.Remove(face_key);
				}
				else {
					//update component data
					_facedata[face_key] = data;
				}
			}
			else {
				if (data.id != 0) {
					//add data
					_facedata.Add(face_key, data);
				}
			}
		}
		_dirtyfaces.Clear();
	}


	#region Classify Corner
	private CornerData ClassifyCorner(IJL corner_key)
	{
		int id = 0;
		int d = 0;
		bool topheavy = Vx.IsCornerTopHeavy(corner_key);
		if (map != null) {
			CornerPattern corner_pat = GetCornerPattern(corner_key, topheavy);
			//get id
			id =  map.GetCornerID(corner_pat);
			if (id > 0) {
				//get rotation shift
				CornerPattern map_pat = map.GetCornerPattern(corner_pat);
				int shift = CornerPattern.CalculateShift(map_pat, corner_pat);
				if (shift >= 0) {
					d += shift;
				}
			}
		}
		if (topheavy) {
			d = Vx.ND[1, d];
		}
		//calculate draw
		bool draw = ShouldDrawCorner(corner_key, topheavy);
		//calculate vertex
		Vector3 vertex = Vx.CornerVertex(corner_key, topheavy);
		return new CornerData(draw, id, d, vertex);
	}

	private CornerPattern GetCornerPattern(IJL corner_key, bool topheavy)
	{
		//find voxel keys in proper order
		int heavyshift = 0;
		if (!topheavy) {
			heavyshift = 1;
		}
		int d = Vx.D[heavyshift, 4];
		int d2 = Vx.D[heavyshift, 0];
		int d4 = Vx.D[heavyshift, 2];
		//Above 0
		IJL ijl = Vx.IJLFromVoxelKey(Vx.CornerInvKey_Below(corner_key, d));
		int a0 = 0;
		if (_voxelslots.ContainsKey(ijl)) {
			a0 = (int)_voxelslots[ijl].voxel.id;
		}
		//Above 1
		ijl = Vx.IJLFromVoxelKey(Vx.CornerInvKey_Below(corner_key, d2));
		int a1 = 0;
		if (_voxelslots.ContainsKey(ijl)) {
			a1 = (int)_voxelslots[ijl].voxel.id;
		}
		//Above 2
		ijl = Vx.IJLFromVoxelKey(Vx.CornerInvKey_Below(corner_key, d4));
		int a2 = 0;
		if (_voxelslots.ContainsKey(ijl)) {
			a2 = (int)_voxelslots[ijl].voxel.id;
		}
		//Below 0
		ijl = Vx.IJLFromVoxelKey(Vx.CornerInvKey_Above(corner_key, d));
		int b0 = 0;
		if (_voxelslots.ContainsKey(ijl)) {
			b0 = (int)_voxelslots[ijl].voxel.id;
		}
		//Below 1
		ijl = Vx.IJLFromVoxelKey(Vx.CornerInvKey_Above(corner_key, d2));
		int b1 = 0;
		if (_voxelslots.ContainsKey(ijl)) {
			b1 = (int)_voxelslots[ijl].voxel.id;
		}
		//Below 2
		ijl = Vx.IJLFromVoxelKey(Vx.CornerInvKey_Above(corner_key, d4));
		int b2 = 0;
		if (_voxelslots.ContainsKey(ijl)) {
			b2 = (int)_voxelslots[ijl].voxel.id;
		}
		return new CornerPattern(a0, a1, a2, b0, b1, b2);
	}

	private bool ShouldDrawCorner(IJL corner_key, bool topheavy)
	{
		//find above voxel keys in proper order
		IJL voxel_key0, voxel_key1, voxel_key2;
		if (topheavy) {
			voxel_key0 = Vx.CornerInvKey_Above(corner_key, 4);
			voxel_key1 = Vx.CornerInvKey_Above(corner_key, 0);
			voxel_key2 = Vx.CornerInvKey_Above(corner_key, 2);
		}
		else {
			voxel_key0 = Vx.CornerInvKey_Above(corner_key, 5);
			voxel_key1 = Vx.CornerInvKey_Above(corner_key, 1);
			voxel_key2 = Vx.CornerInvKey_Above(corner_key, 3);
		}
		//encode above level
		int encoding = EncodeCornerLevel(voxel_key0, voxel_key1, voxel_key2);
		//if encoding isn't zero then return ShouldDrawCornerLevel
		if (encoding != 0) {
			return ShouldDrawCornerLevel(encoding, topheavy);
		}
		//find below voxel keys in proper order
		if (topheavy) {
			voxel_key0 = Vx.CornerInvKey_Below(corner_key, 4);
			voxel_key1 = Vx.CornerInvKey_Below(corner_key, 0);
			voxel_key2 = Vx.CornerInvKey_Below(corner_key, 2);
		}
		else {
			voxel_key0 = Vx.CornerInvKey_Below(corner_key, 5);
			voxel_key1 = Vx.CornerInvKey_Below(corner_key, 1);
			voxel_key2 = Vx.CornerInvKey_Below(corner_key, 3);
		}
		//encode below level
		encoding = EncodeCornerLevel(voxel_key0, voxel_key1, voxel_key2);
		//return ShouldDrawCornerLevel
		return ShouldDrawCornerLevel(encoding, topheavy);
	}

	private int EncodeCornerLevel(IJL voxel_key0, IJL voxel_key1, IJL voxel_key2)
	{
		int encoding = 0;
		IJL voxel_ijl0 = Vx.IJLFromVoxelKey(voxel_key0);
		if (_voxelslots.ContainsKey(voxel_ijl0)) {
			if (_voxelslots[voxel_ijl0].draw) {
				encoding += 9;
			}
			else {
				encoding += 18;
			}
		}
		IJL voxel_ijl1 = Vx.IJLFromVoxelKey(voxel_key1);
		if (_voxelslots.ContainsKey(voxel_ijl1)) {
			if (_voxelslots[voxel_ijl1].draw) {
				encoding += 3;
			}
			else {
				encoding += 6;
			}
		}
		IJL voxel_ijl2 = Vx.IJLFromVoxelKey(voxel_key2);
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

	private bool ShouldDrawCornerLevel(int encoded_level, bool topheavy)
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

	#region Classify Edge
	private EdgeData ClassifyEdge(IJL edge_key)
	{
		if (map == null) {
			return new EdgeData();
		}
		int axi;
		IJL corner0_key, corner1_key;
		Vx.GetSegmentData(edge_key, out corner0_key, out corner1_key, out axi);
		//get pattern from voxel data
		EdgePattern edge_pat = GetEdgePattern(edge_key, corner0_key, corner1_key, axi);
		//get pattern from map
		EdgePattern map_pat = map.GetEdgePattern(edge_pat);
		//get id
		int id = map.GetEdgeID(map_pat);
		if (id <= 0) {
			return new EdgeData();
		}
		//calculate shift
		int shift = EdgePattern.CalculateShift(map_pat, edge_pat);
		if ((axi == 0 || axi == 1) && Vx.IsEdgeTopHeavy(edge_key)) {
			shift += 1;
		}
		if (EdgePattern.CalculateAxiFlip(map_pat, edge_pat)) {
			axi = Vx.OPPAXI[axi];
		}
		//calculate draw
		bool draw = true;//FIX THIS
		//calculate vertex
		Vector3 vertex = Vector3.Lerp(Vx.CornerVertex(corner0_key), Vx.CornerVertex(corner1_key), 0.5f);//MAKE THIS BETTER
		return new EdgeData(draw, id, axi, shift, vertex);
	}
	private EdgePattern GetEdgePattern(IJL edge_key, IJL corner0_key, IJL corner1_key, int axi)
	{
		if (map == null || axi < 0 || axi >= 8) {
			return EdgePattern.empty;
		}
		if (!_cornerdata.ContainsKey(corner0_key) || !_cornerdata.ContainsKey(corner1_key)) {
			return EdgePattern.empty;
		}
		CornerData data0 = _cornerdata[corner0_key];
		CornerData data1 = _cornerdata[corner1_key];
		CornerDesign design0 = map.GetCorner(data0.id);
		if (design0 == null) {
			return EdgePattern.empty;
		}
		CornerDesign design1 = map.GetCorner(data1.id);
		if (design1 == null) {
			return EdgePattern.empty;
		}
		int socket_count0 = Vx.NROTAXI[data0.shift, axi];
		int socket_count1 = Vx.NROTAXI[data1.shift, Vx.OPPAXI[axi]];
		int p0 = design0.GetEdgeSocketCount(socket_count0);
		int p1 = design1.GetEdgeSocketCount(socket_count1);
		bool vertical = (axi == 0) || (axi == 1);
		int v0 = 0;
		int v1 = 0;
		int v2 = 0;
		int v3 = 0;
		if (vertical) {
			bool topheavy = Vx.IsEdgeTopHeavy(edge_key);
			//find voxel keys in proper order
			int heavyshift = 0;
			if (!topheavy) {
				heavyshift = 1;
			}
			//three
			int d = Vx.D[heavyshift, 4];
			int d2 = Vx.D[heavyshift, 0];
			int d4 = Vx.D[heavyshift, 2];
			//D0 or D1
			IJL ijl = Vx.IJLFromVoxelKey(Vx.EdgeInvKey_Middle(edge_key, d));
			if (_voxelslots.ContainsKey(ijl)) {
				v0 = (int)_voxelslots[ijl].voxel.id;
			}
			//D2 or D3
			ijl = Vx.IJLFromVoxelKey(Vx.EdgeInvKey_Middle(edge_key, d2));
			if (_voxelslots.ContainsKey(ijl)) {
				v1 = (int)_voxelslots[ijl].voxel.id;
			}
			//D4 or D5
			ijl = Vx.IJLFromVoxelKey(Vx.EdgeInvKey_Middle(edge_key, d4));
			if (_voxelslots.ContainsKey(ijl)) {
				v2 = (int)_voxelslots[ijl].voxel.id;
			}
		}
		else {
			//four
			//Above 0
			int voxel0_index = Vx.D[1, axi - 2];
			int voxel1_index = Vx.ND[2, axi - 2];
			IJL ijl = Vx.IJLFromVoxelKey(Vx.EdgeInvKey_Below(edge_key, voxel0_index));
			if (_voxelslots.ContainsKey(ijl)) {
				v0 = (int)_voxelslots[ijl].voxel.id;
			}
			//Below 0
			ijl = Vx.IJLFromVoxelKey(Vx.EdgeInvKey_Above(edge_key, voxel0_index));
			if (_voxelslots.ContainsKey(ijl)) {
				v1 = (int)_voxelslots[ijl].voxel.id;
			}
			//Above 1
			ijl = Vx.IJLFromVoxelKey(Vx.EdgeInvKey_Below(edge_key, voxel1_index));
			if (_voxelslots.ContainsKey(ijl)) {
				v2 = (int)_voxelslots[ijl].voxel.id;
			}
			//Below 1
			ijl = Vx.IJLFromVoxelKey(Vx.EdgeInvKey_Above(edge_key, voxel1_index));
			if (_voxelslots.ContainsKey(ijl)) {
				v3 = (int)_voxelslots[ijl].voxel.id;
			}
		}
		return new EdgePattern(vertical, p0, p1, data0.id, data1.id, v0, v1, v2, v3);
	}
	#endregion

	#region Classify Face
	private FaceData ClassifyFace(IJL face_key)
	{
		if (map == null) {
			return new FaceData();
		}

		FacePacket packet;
		Vx.GetFacePacket(face_key, out packet);
		//get pattern from voxel data
		FacePattern face_pat = GetFacePattern(face_key, ref packet);
		//get pattern from map
		FacePattern map_pat = map.GetFacePattern(face_pat);
		//get id
		int id = map.GetFaceID(map_pat);
		if (id <= 0) {
			return new FaceData();
		}
		//draw
		bool draw = true;
		//axi
		int axi = packet.axi;
		//shift
		int shift = FacePattern.CalculateShift(map_pat, face_pat);
		if (shift < 0) {
			return new FaceData();
		}
		if (!packet.hexagon) {
			if (shift > 0) {
				axi = Vx.OPPAXI[axi];
			}
			shift = axi;
		}
		//vertex
		Vector3 vertex = Vx.FaceVertex(face_key, packet.axi);
		return new FaceData(draw, id, axi, shift, vertex);
	}

	private FacePattern GetFacePattern(IJL face_key, ref FacePacket packet)
	{
		if (packet.hexagon) {
			return GetHexagonPattern(face_key, ref packet);
		}
		else {
			return GetRectPattern(face_key, ref packet);
		}
	}

	private FacePattern GetRectPattern(IJL face_key, ref FacePacket packet)
	{
		if (map == null || packet.axi < 2 || packet.axi >= 8 || packet.hexagon) {
			return FacePattern.emptyRect;
		}

		//corner 0
		int corner_axi = Vx.ROTAXI[2, packet.axi];
		if (!_cornerdata.ContainsKey(packet.ckey0)) {
			return FacePattern.emptyRect;
		}
		CornerData data = _cornerdata[packet.ckey0];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyRect;
		}
		int socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p0 = design.GetFaceSocketCount(-1, socket_axi);

		//corner 1
		corner_axi = Vx.ROTAXI[2, packet.axi];
		if (!_cornerdata.ContainsKey(packet.ckey1)) {
			return FacePattern.emptyRect;
		}
		data = _cornerdata[packet.ckey1];
		design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyRect;
		}
		socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p1 = design.GetFaceSocketCount(1, socket_axi);

		//corner 2
		corner_axi = Vx.NROTAXI[1, packet.axi];
		if (!_cornerdata.ContainsKey(packet.ckey2)) {
			return FacePattern.emptyRect;
		}
		data = _cornerdata[packet.ckey2];
		design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyRect;
		}
		socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p2 = design.GetFaceSocketCount(-1, socket_axi);

		//corner 3
		corner_axi = Vx.NROTAXI[1, packet.axi];
		if (!_cornerdata.ContainsKey(packet.ckey3)) {
			return FacePattern.emptyRect;
		}
		data = _cornerdata[packet.ckey3];
		design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyRect;
		}
		socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p3 = design.GetFaceSocketCount(1, socket_axi);

		//inside voxel
		int v0 = 0;
		IJL ijl = Vx.IJLFromVoxelKey(Vx.FaceInvKey(face_key, packet.axi));
		if (_voxelslots.ContainsKey(ijl)) {
			v0 = (int)_voxelslots[ijl].voxel.id;
		}

		//outside voxel
		int v1 = 0;
		ijl = Vx.IJLFromVoxelKey(Vx.FaceInvKey(face_key, Vx.OPPAXI[packet.axi]));
		if (_voxelslots.ContainsKey(ijl)) {
			v1 = (int)_voxelslots[ijl].voxel.id;
		}
		return new FacePattern(p0, p1, p2, p3, v0, v1);
	}

	private FacePattern GetHexagonPattern(IJL face_key, ref FacePacket packet)
	{
		if (map == null || packet.axi < 0 || packet.axi > 1 || !packet.hexagon) {
			return FacePattern.emptyHexagon;
		}
		//corner 0
		int corner_axi = 5;
		if (!_cornerdata.ContainsKey(packet.ckey0)) {
			return FacePattern.emptyHexagon;
		}
		CornerData data = _cornerdata[packet.ckey0];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyHexagon;
		}
		int socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p0 = design.GetFaceSocketCount(0, socket_axi);

		//corner 1
		corner_axi = 6;
		if (!_cornerdata.ContainsKey(packet.ckey1)) {
			return FacePattern.emptyHexagon;
		}
		data = _cornerdata[packet.ckey1];
		design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyHexagon;
		}
		socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p1 = design.GetFaceSocketCount(0, socket_axi);

		//corner 2
		corner_axi = 7;
		if (!_cornerdata.ContainsKey(packet.ckey2)) {
			return FacePattern.emptyHexagon;
		}
		data = _cornerdata[packet.ckey2];
		design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyHexagon;
		}
		socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p2 = design.GetFaceSocketCount(0, socket_axi);

		//corner 3
		corner_axi = 2;
		if (!_cornerdata.ContainsKey(packet.ckey3)) {
			return FacePattern.emptyHexagon;
		}
		data = _cornerdata[packet.ckey3];
		design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyHexagon;
		}
		socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p3 = design.GetFaceSocketCount(0, socket_axi);

		//corner 4
		corner_axi = 3;
		if (!_cornerdata.ContainsKey(packet.ckey4)) {
			return FacePattern.emptyHexagon;
		}
		data = _cornerdata[packet.ckey4];
		design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyHexagon;
		}
		socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p4 = design.GetFaceSocketCount(0, socket_axi);

		//corner 5
		corner_axi = 4;
		if (!_cornerdata.ContainsKey(packet.ckey5)) {
			return FacePattern.emptyHexagon;
		}
		data = _cornerdata[packet.ckey5];
		design = map.GetCorner(data.id);
		if (design == null) {
			return FacePattern.emptyHexagon;
		}
		socket_axi = Vx.NROTAXI[data.shift, corner_axi];
		int p5 = design.GetFaceSocketCount(0, socket_axi);

		//above voxel
		int v0 = 0;
		IJL ijl = Vx.IJLFromVoxelKey(Vx.FaceInvKey(face_key, Vx.OPPAXI[packet.axi]));
		if (_voxelslots.ContainsKey(ijl)) {
			v0 = (int)_voxelslots[ijl].voxel.id;
		}
		//below voxel
		int v1 = 0;
		ijl = Vx.IJLFromVoxelKey(Vx.FaceInvKey(face_key, packet.axi));
		if (_voxelslots.ContainsKey(ijl)) {
			v1 = (int)_voxelslots[ijl].voxel.id;
		}
		return new FacePattern(p0, p1, p2, p3, p4, p5, v0, v1);
	}
	#endregion



	public void GenerateMesh()
	{
		vertices.Clear();
		triangles.Clear();
		_vertexslots.Clear();
		_cornertable.Clear();

		if (map == null) {
			return;
		}

		GenerateCornerMeshes();
		GenerateEdgeMeshes();
		GenerateFaceMeshes();

		//finish processing things like normals
	}

	private void GenerateCornerMeshes()
	{
		foreach (KeyValuePair<IJL, CornerData> data in _cornerdata) {
			CornerDesign design = map.GetCorner((int)data.Value.id);
			if (design == null || !design.IsValid()) {
				continue;
			}
			int corner_index = _vertexslots.Count;
			if (design.vertices.Count > 0) {
				_cornertable.Add(data.Key, corner_index);
			}
			//add vertices
			int shift = data.Value.shift;
			Vector3 corner_vertex = data.Value.vertex;
			for (int k = 0; k < design.vertices.Count; k++) {
				Vector3 vertex = corner_vertex + design.vertices[k].GenerateVertexVector(shift);
				int index = -1;
				if (data.Value.draw && !flat_shaded) {
					vertices.Add(vertex);
					index = vertices.Count - 1;
				}
				_vertexslots.Add(new VertexSlot(index, vertex));
			}
			//add triangles
			int slot_count = _vertexslots.Count;
			for (int k = 0; k < design.triangles.Count; k++) {
				Triangle tri = design.triangles[k];
				int slot0_index = corner_index + tri.vertex0;
				int slot1_index = corner_index + tri.vertex1;
				int slot2_index = corner_index + tri.vertex2;
				if (slot0_index >= slot_count || slot1_index >= slot_count || slot2_index >= slot_count) {
					continue;
				}
				VertexSlot slot0 = _vertexslots[slot0_index];
				VertexSlot slot1 = _vertexslots[slot1_index];
				VertexSlot slot2 = _vertexslots[slot2_index];
				//add triangle
				if (data.Value.draw) {
					if (flat_shaded) {
						//add triangle and its vertices
						int vertex_index = vertices.Count;
						vertices.Add(slot0.vertex);
						vertices.Add(slot1.vertex);
						vertices.Add(slot2.vertex);
						triangles.Add(vertex_index);
						triangles.Add(vertex_index + 1);
						triangles.Add(vertex_index + 2);
					}
					else if (slot0.index >= 0 && slot1.index >= 0 && slot2.index >= 0) {
						triangles.Add(slot0.index);
						triangles.Add(slot1.index);
						triangles.Add(slot2.index);
					}
				}
				//calculate normal and update normals
			}
		}
	}
	private void GenerateEdgeMeshes()
	{
		EdgePacket edgecorners;
		foreach (KeyValuePair<IJL, EdgeData> data in _edgedata) {
			EdgeDesign design = map.GetEdge(data.Value.id);
			if (design == null || !design.IsValid()) {
				continue;
			}
			//get edge corner data
			if (!GetEdgePacket(data.Key, data.Value.axi, out edgecorners)) {
				continue;
			}

			//ADD IN EDGE INDEX REGISTRY LATER FOR EDGE PLUGS
			//add vertices
			int shift = data.Value.shift;
			int edge_index = _vertexslots.Count;
			Vector3 edge_vertex = data.Value.vertex;
			List<VertexVector> edge_vertices = design.vertices;
			for (int k = 0; k < edge_vertices.Count; k++) {
				Vector3 vertex = edge_vertex + edge_vertices[k].GenerateVertexVector(shift);
				int index = -1;
				if (data.Value.draw && !flat_shaded) {
					vertices.Add(vertex);
					index = vertices.Count - 1;
				}
				_vertexslots.Add(new VertexSlot(index, vertex));
			}
			//add triangles
			List<int> edge_cornerplugs = design.cornerplugs;
			List<int> edge_edgeplugs = design.edgeplugs;
			List<Triangle> edge_tris = design.triangles;
			for (int k = 0; k < edge_tris.Count; k++) {
				Triangle tri = edge_tris[k];
				//check triangle validity
				if (!tri.IsValid(edge_vertices.Count, edge_cornerplugs, edge_edgeplugs)) {
					continue;
				}
				int slot0_index = TriangleSlotIndex(edge_index, tri.type0, tri.vertex0, true, ref edgecorners);
				int slot1_index = TriangleSlotIndex(edge_index, tri.type1, tri.vertex1, true, ref edgecorners);
				int slot2_index = TriangleSlotIndex(edge_index, tri.type2, tri.vertex2, true, ref edgecorners);
				if (slot0_index < 0 || slot1_index < 0 || slot2_index < 0) {
					continue;
				}
				//get vertex slots
				VertexSlot slot0 = _vertexslots[slot0_index];
				VertexSlot slot1 = _vertexslots[slot1_index];
				VertexSlot slot2 = _vertexslots[slot2_index];
				if (data.Value.draw) {
					if (flat_shaded) {
						//add triangle and its vertices
						int vertex_index = vertices.Count;
						vertices.Add(slot0.vertex);
						vertices.Add(slot1.vertex);
						vertices.Add(slot2.vertex);
						triangles.Add(vertex_index);
						triangles.Add(vertex_index + 1);
						triangles.Add(vertex_index + 2);
					}
					else {
						//vertex slot 0
						if (slot0.index < 0) {
							vertices.Add(slot0.vertex);
							_vertexslots[slot0_index] = new VertexSlot(vertices.Count - 1, slot0.vertex);
							slot0 = _vertexslots[slot0_index];
						}
						//vertex slot 1
						if (slot1.index < 0) {
							vertices.Add(slot1.vertex);
							_vertexslots[slot1_index] = new VertexSlot(vertices.Count - 1, slot1.vertex);
							slot1 = _vertexslots[slot1_index];
						}
						//vertex slot 2
						if (slot2.index < 0) {
							vertices.Add(slot2.vertex);
							_vertexslots[slot2_index] = new VertexSlot(vertices.Count - 1, slot2.vertex);
							slot2 = _vertexslots[slot2_index];
						}
						//add triangle
						triangles.Add(slot0.index);
						triangles.Add(slot1.index);
						triangles.Add(slot2.index);
					}
				}

				//calculate normal and update normals
			}
		}
	}
	private void GenerateFaceMeshes()
	{
		HexagonPacket hexagoncorners = HexagonPacket.empty;
		RectPacket rectcorners = RectPacket.empty;
		foreach (KeyValuePair<IJL, FaceData> data in _facedata) {
			FaceDesign design = map.GetFace(data.Value.id);
			if (design == null || !design.IsValid()) {
				continue;
			}
			//get face corner data
			bool hexagon = data.Value.isHexagon;
			if (hexagon && !GetHexagonPacket(data.Key, data.Value.axi, out hexagoncorners)) {
				continue;
			}
			if (!hexagon && !GetRectPacket(data.Key, data.Value.axi, out rectcorners)) {
				continue;
			}
			//add face vertices
			int shift = data.Value.shift;
			int face_index = _vertexslots.Count;
			Vector3 face_vertex = data.Value.vertex;
			List<VertexVector> face_vertices = design.vertices;
			for (int k = 0; k < design.vertices.Count; k++) {
				Vector3 vertex = face_vertex + face_vertices[k].GenerateVertexVector(shift);
				int index = -1;
				if (data.Value.draw && !flat_shaded) {
					vertices.Add(vertex);
					index = vertices.Count - 1;
				}
				_vertexslots.Add(new VertexSlot(index, vertex));
			}

			//add triangles
			List<int> face_cornerplugs = design.cornerplugs;
			List<int> face_edgeplugs = design.edgeplugs;
			List<Triangle> face_tris = design.triangles;
			for (int k = 0; k < face_tris.Count; k++) {
				Triangle tri = face_tris[k];
				//check triangle validity
				if (!tri.IsValid(face_vertices.Count, face_cornerplugs, face_edgeplugs)) {
					continue;
				}
				int slot0_index, slot1_index, slot2_index;
				if (hexagon) {
					slot0_index = TriangleSlotIndex(face_index, tri.type0, tri.vertex0, false, ref hexagoncorners);
					slot1_index = TriangleSlotIndex(face_index, tri.type1, tri.vertex1, false, ref hexagoncorners);
					slot2_index = TriangleSlotIndex(face_index, tri.type2, tri.vertex2, false, ref hexagoncorners);
				}
				else {
					slot0_index = TriangleSlotIndex(face_index, tri.type0, tri.vertex0, false, ref rectcorners);
					slot1_index = TriangleSlotIndex(face_index, tri.type1, tri.vertex1, false, ref rectcorners);
					slot2_index = TriangleSlotIndex(face_index, tri.type2, tri.vertex2, false, ref rectcorners);
				}
				if (slot0_index < 0 || slot1_index < 0 || slot2_index < 0) {
					continue;
				}
				//get vertex slots
				VertexSlot slot0 = _vertexslots[slot0_index];
				VertexSlot slot1 = _vertexslots[slot1_index];
				VertexSlot slot2 = _vertexslots[slot2_index];
				if (data.Value.draw) {
					if (flat_shaded) {
						//add triangle and its vertices
						int vertex_index = vertices.Count;
						vertices.Add(slot0.vertex);
						vertices.Add(slot1.vertex);
						vertices.Add(slot2.vertex);
						triangles.Add(vertex_index);
						triangles.Add(vertex_index + 1);
						triangles.Add(vertex_index + 2);
					}
					else {
						//vertex slot 0
						if (slot0.index < 0) {
							vertices.Add(slot0.vertex);
							_vertexslots[slot0_index] = new VertexSlot(vertices.Count - 1, slot0.vertex);
							slot0 = _vertexslots[slot0_index];
						}
						//vertex slot 1
						if (slot1.index < 0) {
							vertices.Add(slot1.vertex);
							_vertexslots[slot1_index] = new VertexSlot(vertices.Count - 1, slot1.vertex);
							slot1 = _vertexslots[slot1_index];
						}
						//vertex slot 2
						if (slot2.index < 0) {
							vertices.Add(slot2.vertex);
							_vertexslots[slot2_index] = new VertexSlot(vertices.Count - 1, slot2.vertex);
							slot2 = _vertexslots[slot2_index];
						}
						//add triangle
						triangles.Add(slot0.index);
						triangles.Add(slot1.index);
						triangles.Add(slot2.index);
					}
				}

				//calculate normal and update normals
			}
		}
	}

	private int TriangleSlotIndex(int base_index, TriIndexType type, ushort vertex, bool is_edge, ref EdgePacket packet)
	{
		int axi_index, socket_index, slot_index;
		switch (type) {
			case TriIndexType.CornerPlug:
				axi_index = TriIndex.DecodeAxiIndex(vertex);
				socket_index = TriIndex.DecodeIndex(vertex);
				CornerPacket corner = packet[axi_index];
				if (corner.design == null) {
					return -1;
				}
				slot_index = GetSocketSlotIndex(is_edge, socket_index, ref corner);
				break;
			case TriIndexType.EdgePlug:
				return -1;
			default:
				if (base_index < 0) {
					return -1;
				}
				slot_index = base_index + vertex;
				break;
		}
		if (slot_index < 0 || slot_index >= _vertexslots.Count) {
			return -1;
		}
		return slot_index;
	}
	private int TriangleSlotIndex(int base_index, TriIndexType type, ushort vertex, bool is_edge, ref RectPacket packet)
	{
		int axi_index, socket_index, slot_index;
		switch (type) {
			case TriIndexType.CornerPlug:
				axi_index = TriIndex.DecodeAxiIndex(vertex);
				socket_index = TriIndex.DecodeIndex(vertex);
				CornerPacket corner = packet[axi_index];
				if (corner.design == null) {
					return -1;
				}
				slot_index = GetSocketSlotIndex(is_edge, socket_index, ref corner);
				break;
			case TriIndexType.EdgePlug:
				return -1;
			default:
				if (base_index < 0) {
					return -1;
				}
				slot_index = base_index + vertex;
				break;
		}
		if (slot_index < 0 || slot_index >= _vertexslots.Count) {
			return -1;
		}
		return slot_index;
	}
	private int TriangleSlotIndex(int base_index, TriIndexType type, ushort vertex, bool is_edge, ref HexagonPacket packet)
	{
		int axi_index, socket_index, slot_index;
		switch (type) {
			case TriIndexType.CornerPlug:
				axi_index = TriIndex.DecodeAxiIndex(vertex);
				socket_index = TriIndex.DecodeIndex(vertex);
				CornerPacket corner = packet[axi_index];
				if (corner.design == null) {
					return -1;
				}
				slot_index = GetSocketSlotIndex(is_edge, socket_index, ref corner);
				break;
			case TriIndexType.EdgePlug:
				return -1;
			default:
				if (base_index < 0) {
					return -1;
				}
				slot_index = base_index + vertex;
				break;
		}
		if (slot_index < 0 || slot_index >= _vertexslots.Count) {
			return -1;
		}
		return slot_index;
	}

	private int GetSocketSlotIndex(bool is_edgesocket, int socket_index, ref CornerPacket packet)
	{
		int offset_index;
		if (is_edgesocket) {
			offset_index = packet.design.GetEdgeSocketByIndex(packet.socket_axi_index, socket_index);
		}
		else {
			offset_index = packet.design.GetFaceSocketByIndex(packet.socket_axi_index, socket_index);
		}
		if (offset_index < 0 || packet.index < 0) {
			return -1;
		}
		return packet.index + offset_index;
	}

	private bool GetEdgePacket(IJL edge_key, int axi, out EdgePacket edgecorners)
	{
		if (map == null || axi < 0 || axi >= 8) {
			edgecorners = EdgePacket.empty;
			return false;
		}
		//corner 0
		int corner_axi = axi;
		IJL corner_key = Vx.GetEdgeCorner(edge_key, Vx.OPPAXI[corner_axi]);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			edgecorners = EdgePacket.empty;
			return false;
		}
		
		int slot_index = _cornertable[corner_key];
		CornerData data = _cornerdata[corner_key];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			edgecorners = EdgePacket.empty;
			return false;
		}
		int socket_axi_index = design.EdgeSocketIndex(Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet0 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		//corner 1
		corner_axi = Vx.OPPAXI[axi];
		corner_key = Vx.GetEdgeCorner(edge_key, Vx.OPPAXI[corner_axi]);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			edgecorners = EdgePacket.empty;
			return false;
		}

		slot_index = _cornertable[corner_key];
		data = _cornerdata[corner_key];
		design = map.GetCorner(data.id);
		if (design == null) {
			edgecorners = EdgePacket.empty;
			return false;
		}
		socket_axi_index = design.EdgeSocketIndex(Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet1 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		edgecorners = new EdgePacket(packet0, packet1);
		return true;
	}
	private bool GetRectPacket(IJL face_key, int axi, out RectPacket rectcorners)
	{
		if (map == null || axi < 2 || axi >= 8) {
			rectcorners = RectPacket.empty;
			return false;
		}
		//corner 0
		int corner_axi = Vx.ROTAXI[2, axi];//axi + 2
		IJL corner_key = Vx.GetFaceCorner(face_key, axi, 0);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = RectPacket.empty;
			return false;
		}

		int slot_index = _cornertable[corner_key];
		CornerData data = _cornerdata[corner_key];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = RectPacket.empty;
			return false;
		}
		int socket_axi_index = design.FaceSocketIndex(-1, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet0 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		//corner 1
		corner_axi = Vx.ROTAXI[2, axi];//axi + 2
		corner_key = Vx.GetFaceCorner(face_key, axi, 1);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = RectPacket.empty;
			return false;
		}

		slot_index = _cornertable[corner_key];
		data = _cornerdata[corner_key];
		design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = RectPacket.empty;
			return false;
		}
		socket_axi_index = design.FaceSocketIndex(1, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet1 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		//corner 2
		corner_axi = Vx.NROTAXI[1, axi];//axi - 1
		corner_key = Vx.GetFaceCorner(face_key, axi, 2);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = RectPacket.empty;
			return false;
		}
		slot_index = _cornertable[corner_key];
		data = _cornerdata[corner_key];
		design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = RectPacket.empty;
			return false;
		}
		socket_axi_index = design.FaceSocketIndex(-1, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet2 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		//corner 3
		corner_axi = Vx.NROTAXI[1, axi];//axi - 1
		corner_key = Vx.GetFaceCorner(face_key, axi, 3);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = RectPacket.empty;
			return false;
		}

		slot_index = _cornertable[corner_key];
		data = _cornerdata[corner_key];
		design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = RectPacket.empty;
			return false;
		}
		socket_axi_index = design.FaceSocketIndex(1, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet3 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		rectcorners = new RectPacket(packet0, packet1, packet2, packet3);
		return true;
	}
	private bool GetHexagonPacket(IJL face_key, int axi, out HexagonPacket rectcorners)
	{
		if (map == null || axi < 0 || axi >= 2) {
			rectcorners = HexagonPacket.empty;
			return false;
		}
		//corner 0
		int corner_axi = 5;
		IJL corner_key = Vx.GetFaceCorner(face_key, axi, 0);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = HexagonPacket.empty;
			return false;
		}

		int slot_index = _cornertable[corner_key];
		CornerData data = _cornerdata[corner_key];
		CornerDesign design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = HexagonPacket.empty;
			return false;
		}
		int socket_axi_index = design.FaceSocketIndex(0, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet0 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		//corner 1
		corner_axi = 6;
		corner_key = Vx.GetFaceCorner(face_key, axi, 1);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = HexagonPacket.empty;
			return false;
		}

		slot_index = _cornertable[corner_key];
		data = _cornerdata[corner_key];
		design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = HexagonPacket.empty;
			return false;
		}
		socket_axi_index = design.FaceSocketIndex(0, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet1 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		//corner 2
		corner_axi = 7;
		corner_key = Vx.GetFaceCorner(face_key, axi, 2);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = HexagonPacket.empty;
			return false;
		}

		slot_index = _cornertable[corner_key];
		data = _cornerdata[corner_key];
		design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = HexagonPacket.empty;
			return false;
		}
		socket_axi_index = design.FaceSocketIndex(0, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet2 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		//corner 3
		corner_axi = 2;
		corner_key = Vx.GetFaceCorner(face_key, axi, 3);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = HexagonPacket.empty;
			return false;
		}

		slot_index = _cornertable[corner_key];
		data = _cornerdata[corner_key];
		design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = HexagonPacket.empty;
			return false;
		}
		socket_axi_index = design.FaceSocketIndex(0, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet3 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		//corner 4
		corner_axi = 3;
		corner_key = Vx.GetFaceCorner(face_key, axi, 4);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = HexagonPacket.empty;
			return false;
		}

		slot_index = _cornertable[corner_key];
		data = _cornerdata[corner_key];
		design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = HexagonPacket.empty;
			return false;
		}
		socket_axi_index = design.FaceSocketIndex(0, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet4 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		//corner 5
		corner_axi = 4;
		corner_key = Vx.GetFaceCorner(face_key, axi, 5);
		if (!_cornerdata.ContainsKey(corner_key) || !_cornertable.ContainsKey(corner_key)) {
			rectcorners = HexagonPacket.empty;
			return false;
		}

		slot_index = _cornertable[corner_key];
		data = _cornerdata[corner_key];
		design = map.GetCorner(data.id);
		if (design == null) {
			rectcorners = HexagonPacket.empty;
			return false;
		}
		socket_axi_index = design.FaceSocketIndex(0, Vx.NROTAXI[data.shift, corner_axi]);
		CornerPacket packet5 = new CornerPacket(design, slot_index, socket_axi_index, corner_axi);

		rectcorners = new HexagonPacket(packet0, packet1, packet2, packet3, packet4, packet5);
		return true;
	}
}


public struct VoxelSlot
{
	public readonly bool draw;
	public readonly Voxel voxel;

	public VoxelSlot(bool draw, Voxel voxel)
	{
		this.draw = draw;
		this.voxel = voxel;
	}
}

public struct CornerData
{
	public readonly bool draw;
	public readonly int id;
	public readonly int shift;
	public readonly Vector3 vertex;

	public CornerData(bool draw, int id, int shift, Vector3 vertex)
	{
		this.draw = draw;
		this.id = id;
		this.shift = Mathf.Clamp(shift, 0, 5);
		this.vertex = vertex;
	}
}

public struct EdgeData
{
	public readonly bool draw;
	public readonly int id;
	public readonly int axi;
	public readonly int shift;
	public readonly Vector3 vertex;

	public EdgeData(bool draw, int id, int axi, int shift, Vector3 vertex)
	{
		this.draw = draw;
		this.id = id;
		this.axi = Mathf.Clamp(axi, 0, 7);
		this.shift = Mathf.Clamp(shift, 0, 5);
		this.vertex = vertex;
	}
}

public struct FacePacket
{
	public readonly bool hexagon;
	public readonly int axi;
	public readonly IJL ckey0;
	public readonly IJL ckey1;
	public readonly IJL ckey2;
	public readonly IJL ckey3;
	public readonly IJL ckey4;
	public readonly IJL ckey5;

	public FacePacket(int axi, IJL ckey0, IJL ckey1, IJL ckey2, IJL ckey3, IJL ckey4, IJL ckey5)
	{
		this.hexagon = true;
		this.axi = axi;
		this.ckey0 = ckey0;
		this.ckey1 = ckey1;
		this.ckey2 = ckey2;
		this.ckey3 = ckey3;
		this.ckey4 = ckey4;
		this.ckey5 = ckey5;
	}

	public FacePacket(int axi, IJL ckey0, IJL ckey1, IJL ckey2, IJL ckey3)
	{
		this.hexagon = false;
		this.axi = axi;
		this.ckey0 = ckey0;
		this.ckey1 = ckey1;
		this.ckey2 = ckey2;
		this.ckey3 = ckey3;
		this.ckey4 = IJL.zero;
		this.ckey5 = IJL.zero;
	}
}

public struct FaceData
{
	public readonly bool draw;
	public readonly int id;
	public readonly int axi;
	public readonly int shift;
	public readonly Vector3 vertex;

	public FaceData(bool draw, int id, int axi, int shift, Vector3 vertex)
	{
		this.draw = draw;
		this.id = id;
		this.axi = Mathf.Clamp(axi, 0, 8);
		this.shift = Mathf.Clamp(shift, 0, 5);
		this.vertex = vertex;
	}

	public bool isHexagon
	{
		get {
			return (axi == 0 || axi == 1);
		}
	}
}


public struct VertexSlot
{
	public readonly int index;
	public readonly Vector3 vertex;

	public VertexSlot(int index, Vector3 vertex)
	{
		this.index = index;
		this.vertex = vertex;
	}
}