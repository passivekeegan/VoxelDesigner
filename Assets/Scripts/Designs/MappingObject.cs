using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MappingObject is a class used to map patterns to voxel components.
/// </summary>
public class MappingObject : VoxelObject
{
	public const int CAPACITY_LIMIT = 10000;

	//corner data
	[SerializeField]
	private CornerIntMap _cornerid;
	[SerializeField]
	private IntCornerMap _idcorner;
	[SerializeField]
	private IntCornerPatternMap _cornerpat_map;
	[SerializeField]
	private CornerPatternIntMap _patcorner_map;
	[SerializeField]
	private CornerPatternPatternMap _corner_patpatmap;

	//edge data
	[SerializeField]
	private EdgeIntMap _edgeid;
	[SerializeField]
	private IntEdgeMap _idedge;
	[SerializeField]
	private IntEdgePatternMap _edgepat_map;
	[SerializeField]
	private EdgePatternIntMap _patedge_map;
	[SerializeField]
	private EdgePatternPatternMap _edge_patpatmap;

	//face data
	[SerializeField]
	private FaceIntMap _faceid;
	[SerializeField]
	private IntFaceMap _idface;
	[SerializeField]
	private IntFacePatternMap _facepat_map;
	[SerializeField]
	private FacePatternIntMap _patface_map;
	[SerializeField]
	private FacePatternPatternMap _face_patpatmap;

	#region Initialization
	/// <summary>
	/// Initialize mapping object, should be called when object created.
	/// </summary>
	public override void Initialize()
	{
		objname = "";
		//corners
		InitializeCorner();
		//edges
		InitializeEdge();
		//faces
		InitializeFace();
	}

	/// <summary>
	/// 
	/// </summary>
	private void InitializeCorner()
	{
		_cornerid = new CornerIntMap();
		_idcorner = new IntCornerMap();
		_cornerpat_map = new IntCornerPatternMap();
		_patcorner_map = new CornerPatternIntMap();
		_corner_patpatmap = new CornerPatternPatternMap();
	}

	/// <summary>
	/// 
	/// </summary>
	private void InitializeEdge()
	{
		_edgeid = new EdgeIntMap();
		_idedge = new IntEdgeMap();
		_edgepat_map = new IntEdgePatternMap();
		_patedge_map = new EdgePatternIntMap();
		_edge_patpatmap = new EdgePatternPatternMap();
	}

	/// <summary>
	/// 
	/// </summary>
	private void InitializeFace()
	{
		_faceid = new FaceIntMap();
		_idface = new IntFaceMap();
		_facepat_map = new IntFacePatternMap();
		_patface_map = new FacePatternIntMap();
		_face_patpatmap = new FacePatternPatternMap();
	}
	#endregion

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public override bool IsValid()
	{
		return true;
	}

	#region Corners
	#region Modify Map
	/// <summary>
	/// 
	/// </summary>
	/// <param name="corner"></param>
	public void AddCorner(CornerDesign corner)
	{
		if (corner == null) {
			return;
		}
		if (CornerExists(corner)) {
			return;
		}
		int id = GenerateUniqueCornerID();
		if (id < 1) {
			return;
		}
		_cornerid.Add(corner, id);
		_idcorner.Add(id, corner);
		_cornerpat_map.Add(id, new List<CornerPattern>());
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="oldcorner"></param>
	/// <param name="newcorner"></param>
	public void ReplaceCorner(CornerDesign oldcorner, CornerDesign newcorner)
	{
		if (oldcorner == null || newcorner == null) {
			return;
		}
		if (!CornerExists(oldcorner)) {
			return;
		}
		int oldid = _cornerid[oldcorner];
		if (CornerExists(newcorner)) {
			int newid = _cornerid[newcorner];
			if (oldid == newid) {
				return;
			}
			_cornerid.Remove(oldcorner);
			_cornerid.Remove(newcorner);
			_cornerid.Add(oldcorner, newid);
			_cornerid.Add(newcorner, oldid);
			_idcorner[oldid] = newcorner;
			_idcorner[newid] = oldcorner;
		}
		else {
			_cornerid.Remove(oldcorner);
			_cornerid.Add(newcorner, oldid);
			_idcorner[oldid] = newcorner;
		}
	}
	public void DeleteCorner(CornerDesign corner)
	{
		if (corner == null) {
			return;
		}
		if (!CornerExists(corner)) {
			return;
		}
		int id = _cornerid[corner];
		if (id < 1) {
			return;
		}
		_cornerid.Remove(corner);
		_idcorner.Remove(id);
		List<CornerPattern> patterns = _cornerpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			_patcorner_map.Remove(patterns[k]);
			_corner_patpatmap.Remove(patterns[k]);
		}
		_cornerpat_map.Remove(id);
	}
	public void AddCornerPattern(CornerDesign corner, CornerPattern pattern)
	{
		if (corner == null || pattern.IsEmpty()) {
			return;
		}
		if (!CornerExists(corner) || CornerPatternExists(pattern)) {
			return;
		}
		int id = _cornerid[corner];
		if (id < 1) {
			return;
		}
		_patcorner_map.Add(pattern, id);
		_cornerpat_map[id].Add(pattern);
		_corner_patpatmap.Add(pattern, pattern);
	}
	public void DeleteCornerPattern(CornerDesign corner, CornerPattern pattern)
	{
		if (corner == null || pattern.IsEmpty()) {
			return;
		}
		if (!CornerExists(corner) || !CornerPatternExists(pattern)) {
			return;
		}
		int id = _cornerid[corner];
		if (id < 1) {
			return;
		}
		_patcorner_map.Remove(pattern);
		_cornerpat_map[id].Remove(pattern);
		_corner_patpatmap.Remove(pattern);
	}

	#endregion

	#region Query Map
	public bool CornerExists(CornerDesign corner)
	{
		if (corner == null) {
			return false;
		}
		return _cornerid.ContainsKey(corner);
	}
	public bool CornerPatternExists(CornerPattern pattern)
	{
		if (pattern.IsEmpty()) {
			return true;
		}
		return _patcorner_map.ContainsKey(pattern);
	}

	#endregion

	#region Getters
	public int GetCornerID(CornerPattern pattern)
	{
		if (pattern.IsEmpty() || !_patcorner_map.ContainsKey(pattern)) {
			return 0;
		}
		return _patcorner_map[pattern];
	}
	public CornerDesign GetCorner(CornerPattern pattern)
	{
		if (pattern.IsEmpty() || !_patcorner_map.ContainsKey(pattern)) {
			return null;
		}
		return _idcorner[_patcorner_map[pattern]];
	}
	public CornerDesign GetCorner(int id)
	{
		if (id == 0 || !_idcorner.ContainsKey(id)) {
			return null;
		}
		return _idcorner[id];
	}
	public CornerPattern GetCornerPattern(CornerPattern pattern)
	{
		if (pattern.IsEmpty() || !_corner_patpatmap.ContainsKey(pattern)) {
			return pattern;
		}
		return _corner_patpatmap[pattern];
	}
	#endregion

	#region List Getters
	public void AddCornersToList(List<VoxelComponent> list)
	{
		foreach (KeyValuePair<CornerDesign, int> pair in _cornerid) {
			list.Add(pair.Key);
		}
	}
	public void AddCornersToList(List<CornerDesign> list)
	{
		foreach (KeyValuePair<CornerDesign, int> pair in _cornerid) {
			list.Add(pair.Key);
		}
	}
	public void AddCornerPatternsToList(CornerDesign corner, List<CornerPattern> list)
	{
		if (!_cornerid.ContainsKey(corner)) {
			return;
		}
		int id = _cornerid[corner];
		if (!_cornerpat_map.ContainsKey(id)) {
			return;
		}
		List<CornerPattern> patterns = _cornerpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			list.Add(patterns[k]);
		}
	}
	#endregion

	#region Helpers
	private int GenerateUniqueCornerID()
	{
		for (int id = 1;id <= CAPACITY_LIMIT;id++) {
			if (!_idcorner.ContainsKey(id)) {
				return id;
			}
		}
		return -1;
	}
	#endregion
	#endregion

	#region Edges
	#region Modify Map
	public void AddEdge(EdgeDesign edge)
	{
		if (edge == null) {
			return;
		}
		if (EdgeExists(edge)) {
			return;
		}
		int id = GenerateUniqueEdgeID();
		if (id < 1) {
			return;
		}
		_edgeid.Add(edge, id);
		_idedge.Add(id, edge);
		_edgepat_map.Add(id, new List<EdgePattern>());
	}
	public void ReplaceEdge(EdgeDesign oldedge, EdgeDesign newedge)
	{
		if (oldedge == null || newedge == null) {
			return;
		}
		if (!EdgeExists(oldedge)) {
			return;
		}
		int oldid = _edgeid[oldedge];
		if (EdgeExists(newedge)) {
			int newid = _edgeid[newedge];
			if (oldid == newid) {
				return;
			}
			_edgeid.Remove(oldedge);
			_edgeid.Remove(newedge);
			_edgeid.Add(oldedge, newid);
			_edgeid.Add(newedge, oldid);
			_idedge[oldid] = newedge;
			_idedge[newid] = oldedge;
		}
		else {
			_edgeid.Remove(oldedge);
			_edgeid.Add(newedge, oldid);
			_idedge[oldid] = newedge;
		}
	}
	public void DeleteEdge(EdgeDesign edge)
	{
		if (edge == null) {
			return;
		}
		if (!EdgeExists(edge)) {
			return;
		}
		int id = _edgeid[edge];
		if (id < 1) {
			return;
		}
		_edgeid.Remove(edge);
		_idedge.Remove(id);
		List<EdgePattern> patterns = _edgepat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			_patedge_map.Remove(patterns[k]);
			_edge_patpatmap.Remove(patterns[k]);
		}
		_edgepat_map.Remove(id);
	}
	public void AddEdgePattern(EdgeDesign edge, EdgePattern pattern)
	{
		if (edge == null || !pattern.valid) {
			return;
		}
		if (!EdgeExists(edge) || EdgePatternExists(pattern)) {
			return;
		}
		int id = _edgeid[edge];
		if (id < 1) {
			return;
		}
		_patedge_map.Add(pattern, id);
		_edgepat_map[id].Add(pattern);
		_edge_patpatmap.Add(pattern, pattern);
	}
	public void DeleteEdgePattern(EdgeDesign edge, EdgePattern pattern)
	{
		if (edge == null || !pattern.valid) {
			return;
		}
		if (!EdgeExists(edge) || !EdgePatternExists(pattern) || pattern.IsEmpty()) {
			return;
		}
		int id = _edgeid[edge];
		if (id < 1) {
			return;
		}
		_patedge_map.Remove(pattern);
		_edgepat_map[id].Remove(pattern);
		_edge_patpatmap.Remove(pattern);
	}
	#endregion

	#region Query Map
	public bool EdgeExists(EdgeDesign edge)
	{
		if (edge == null) {
			return false;
		}
		return _edgeid.ContainsKey(edge);
	}
	public bool EdgePatternExists(EdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return true;
		}
		return _patedge_map.ContainsKey(pattern);
	}
	#endregion

	#region Getters
	public int GetEdgeID(EdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return 0;
		}
		if (pattern.c0 >= 0 && pattern.c1 >= 0) {
			if (_patedge_map.ContainsKey(pattern)) {
				return _patedge_map[pattern];
			}
			EdgePattern pattern0 = pattern.GetPatternCopy(true, false);
			EdgePattern pattern1 = pattern.GetPatternCopy(false, true);
			bool t0 = _patedge_map.ContainsKey(pattern0);
			bool t1 = _patedge_map.ContainsKey(pattern1);
			if (t0 && !t1) {
				return _patedge_map[pattern0];
			}
			else if (!t0 && t1) {
				return _patedge_map[pattern1];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_patedge_map.ContainsKey(pattern)) {
				return _patedge_map[pattern];
			}
		}
		else if (pattern.c0 >= 0 || pattern.c1 >= 0) {
			if (_patedge_map.ContainsKey(pattern)) {
				return _patedge_map[pattern];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_patedge_map.ContainsKey(pattern)) {
				return _patedge_map[pattern];
			}
		}
		else {
			if (_patedge_map.ContainsKey(pattern)) {
				return _patedge_map[pattern];
			}
		}
		return 0;
	}
	public EdgeDesign GetEdge(EdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return null;
		}
		return GetEdge(GetEdgeID(pattern));
	}
	public EdgeDesign GetEdge(int id)
	{
		if (id == 0 || !_idedge.ContainsKey(id)) {
			return null;
		}
		return _idedge[id];
	}
	public EdgePattern GetEdgePattern(EdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return EdgePattern.empty;
		}
		if (pattern.c0 >= 0 && pattern.c1 >= 0) {
			if (_edge_patpatmap.ContainsKey(pattern)) {
				return _edge_patpatmap[pattern];
			}
			EdgePattern pattern0 = pattern.GetPatternCopy(true, false);
			EdgePattern pattern1 = pattern.GetPatternCopy(false, true);
			bool t0 = _edge_patpatmap.ContainsKey(pattern0);
			bool t1 = _edge_patpatmap.ContainsKey(pattern1);
			if (t0 && !t1) {
				return _edge_patpatmap[pattern0];
			}
			else if (!t0 && t1) {
				return _edge_patpatmap[pattern1];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_edge_patpatmap.ContainsKey(pattern)) {
				return _edge_patpatmap[pattern];
			}
		}
		else if (pattern.c0 >= 0 || pattern.c1 >= 0) {
			if (_edge_patpatmap.ContainsKey(pattern)) {
				return _edge_patpatmap[pattern];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_edge_patpatmap.ContainsKey(pattern)) {
				return _edge_patpatmap[pattern];
			}
		}
		else {
			if (_edge_patpatmap.ContainsKey(pattern)) {
				return _edge_patpatmap[pattern];
			}
		}
		return EdgePattern.empty;


	}
	#endregion

	#region List Getters
	public void AddEdgesToList(List<VoxelComponent> list)
	{
		foreach (KeyValuePair<EdgeDesign, int> pair in _edgeid) {
			list.Add(pair.Key);
		}
	}
	public void AddEdgesToList(List<EdgeDesign> list)
	{
		foreach (KeyValuePair<EdgeDesign, int> pair in _edgeid) {
			list.Add(pair.Key);
		}
	}
	public void AddEdgePatternsToList(EdgeDesign edge, List<EdgePattern> list)
	{
		if (!_edgeid.ContainsKey(edge)) {
			return;
		}
		int id = _edgeid[edge];
		if (!_edgepat_map.ContainsKey(id)) {
			return;
		}
		List<EdgePattern> patterns = _edgepat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			list.Add(patterns[k]);
		}
	}
	#endregion

	#region Helpers
	private int GenerateUniqueEdgeID()
	{
		for (int id = 1; id <= CAPACITY_LIMIT; id++) {
			if (!_idedge.ContainsKey(id)) {
				return id;
			}
		}
		return -1;
	}
	#endregion
	#endregion

	#region Face
	#region Modify Map
	public void AddFace(FaceDesign face)
	{
		if (face == null) {
			return;
		}
		if (FaceExists(face)) {
			return;
		}
		int id = GenerateUniqueFaceID();
		if (id < 1) {
			return;
		}
		_faceid.Add(face, id);
		_idface.Add(id, face);
		_facepat_map.Add(id, new List<FacePattern>());
	}
	public void ReplaceFace(FaceDesign oldface, FaceDesign newface)
	{
		if (oldface == null || newface == null) {
			return;
		}
		if (!FaceExists(oldface)) {
			return;
		}
		int oldid = _faceid[oldface];
		if (FaceExists(newface)) {
			int newid = _faceid[newface];
			if (oldid == newid) {
				return;
			}
			_faceid.Remove(oldface);
			_faceid.Remove(newface);
			_faceid.Add(oldface, newid);
			_faceid.Add(newface, oldid);
			_idface[oldid] = newface;
			_idface[newid] = oldface;
		}
		else {
			_faceid.Remove(oldface);
			_faceid.Add(newface, oldid);
			_idface[oldid] = newface;
		}
	}
	public void DeleteFace(FaceDesign face)
	{
		if (face == null) {
			return;
		}
		if (!FaceExists(face)) {
			return;
		}
		int id = _faceid[face];
		if (id < 1) {
			return;
		}
		_faceid.Remove(face);
		_idface.Remove(id);
		List<FacePattern> patterns = _facepat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			_patface_map.Remove(patterns[k]);
			_face_patpatmap.Remove(patterns[k]);
		}
		_facepat_map.Remove(id);
	}
	public void AddFacePattern(FaceDesign face, FacePattern pattern)
	{
		if (face == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!FaceExists(face) || FacePatternExists(pattern)) {
			return;
		}
		int id = _faceid[face];
		if (id < 1) {
			return;
		}
		_patface_map.Add(pattern, id);
		_facepat_map[id].Add(pattern);
		_face_patpatmap.Add(pattern, pattern);
	}
	public void DeleteFacePattern(FaceDesign face, FacePattern pattern)
	{
		if (face == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!FaceExists(face) || !FacePatternExists(pattern)) {
			return;
		}
		int id = _faceid[face];
		if (id < 1) {
			return;
		}
		_patface_map.Remove(pattern);
		_facepat_map[id].Remove(pattern);
		_face_patpatmap.Remove(pattern);
	}
	#endregion

	#region Query Map
	public bool FaceExists(FaceDesign face)
	{
		if (face == null) {
			return false;
		}
		return _faceid.ContainsKey(face);
	}
	public bool FacePatternExists(FacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return false;
		}
		return _patface_map.ContainsKey(pattern);
	}
	#endregion

	#region Getters
	public int GetFaceID(FacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return 0;
		}
		if (!_patface_map.ContainsKey(pattern)) {
			return 0;
		}
		return _patface_map[pattern];
	}
	public FaceDesign GetFace(int id)
	{
		if (id <= 0 || !_idface.ContainsKey(id)) {
			return null;
		}
		return _idface[id];
	}
	public FaceDesign GetFace(FacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return null;
		}
		if (!_patface_map.ContainsKey(pattern)) {
			return null;
		}
		return _idface[_patface_map[pattern]];
	}
	public FacePattern GetFacePattern(FacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			if (pattern.hexagon) {
				return FacePattern.emptyHexagon;
			}
			else {
				return FacePattern.emptyRect;
			}
		}
		if (!_face_patpatmap.ContainsKey(pattern)) {
			if (pattern.hexagon) {
				return FacePattern.emptyHexagon;
			}
			else {
				return FacePattern.emptyRect;
			}
		}
		return _face_patpatmap[pattern];
	}
	#endregion

	#region List Getters
	public void AddFacesToList(List<VoxelComponent> list)
	{
		foreach (KeyValuePair<FaceDesign, int> pair in _faceid) {
			list.Add(pair.Key);
		}
	}
	public void AddFacesToList(List<FaceDesign> list)
	{
		foreach (KeyValuePair<FaceDesign, int> pair in _faceid) {
			list.Add(pair.Key);
		}
	}
	public void AddFacePatternsToList(FaceDesign face, List<FacePattern> list)
	{
		if (!_faceid.ContainsKey(face)) {
			return;
		}
		int id = _faceid[face];
		if (!_facepat_map.ContainsKey(id)) {
			return;
		}
		List<FacePattern> patterns = _facepat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			list.Add(patterns[k]);
		}
	}
	#endregion

	#region Helpers
	private int GenerateUniqueFaceID()
	{
		for (int id = 1; id <= CAPACITY_LIMIT; id++) {
			if (!_idface.ContainsKey(id)) {
				return id;
			}
		}
		return -1;
	}
	#endregion
	#endregion
}
