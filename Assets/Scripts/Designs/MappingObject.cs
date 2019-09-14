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
	//lateral edge data
	[SerializeField]
	private LateralIntMap _latid;
	[SerializeField]
	private IntLateralMap _idlat;
	[SerializeField]
	private IntLateralPatternMap _idlatpat_map;
	[SerializeField]
	private LateralPatternIntMap _latpatid_map;
	[SerializeField]
	private LateralPatternPatternMap _latpatpat_map;
	//longitude edge data
	[SerializeField]
	private LongitudeIntMap _longid;
	[SerializeField]
	private IntLongitudeMap _idlong;
	[SerializeField]
	private IntLongitudePatternMap _idlongpat_map;
	[SerializeField]
	private LongitudePatternIntMap _longpatid_map;
	[SerializeField]
	private LongitudePatternPatternMap _longpatpat_map;
	//rect face data
	[SerializeField]
	private RectIntMap _rectid;
	[SerializeField]
	private IntRectMap _idrect;
	[SerializeField]
	private IntRectPatternMap _idrectpat_map;
	[SerializeField]
	private RectPatternIntMap _rectpatid_map;
	[SerializeField]
	private RectPatternPatternMap _rectpatpat_map;
	//hexagon face data
	[SerializeField]
	private HexagonIntMap _hexid;
	[SerializeField]
	private IntHexagonMap _idhex;
	[SerializeField]
	private IntHexagonPatternMap _idhexpat_map;
	[SerializeField]
	private HexagonPatternIntMap _hexpatid_map;
	[SerializeField]
	private HexagonPatternPatternMap _hexpatpat_map;

	#region Initialization
	/// <summary>
	/// Initialize mapping object, should be called when object created.
	/// </summary>
	public override void Initialize()
	{
		objname = "";
		//corners
		InitializeCorner();
		//lateral edges
		InitializeLateralEdge();
		//longitude edges
		InitializeLongitudeEdge();
		//rect faces
		InitializeRectFace();
		//hexagon faces
		InitializeHexagonFace();
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
	private void InitializeLateralEdge()
	{
		_latid = new LateralIntMap();
		_idlat = new IntLateralMap();
		_idlatpat_map = new IntLateralPatternMap();
		_latpatid_map = new LateralPatternIntMap();
		_latpatpat_map = new LateralPatternPatternMap();
	}

	/// <summary>
	/// 
	/// </summary>
	private void InitializeLongitudeEdge()
	{
		_longid = new LongitudeIntMap();
		_idlong = new IntLongitudeMap();
		_idlongpat_map = new IntLongitudePatternMap();
		_longpatid_map = new LongitudePatternIntMap();
		_longpatpat_map = new LongitudePatternPatternMap();
	}

	/// <summary>
	/// 
	/// </summary>
	private void InitializeRectFace()
	{
		_rectid = new RectIntMap();
		_idrect = new IntRectMap();
		_idrectpat_map = new IntRectPatternMap();
		_rectpatid_map = new RectPatternIntMap();
		_rectpatpat_map = new RectPatternPatternMap();
	}

	/// <summary>
	/// 
	/// </summary>
	private void InitializeHexagonFace()
	{
		_hexid = new HexagonIntMap();
		_idhex = new IntHexagonMap();
		_idhexpat_map = new IntHexagonPatternMap();
		_hexpatid_map = new HexagonPatternIntMap();
		_hexpatpat_map = new HexagonPatternPatternMap();
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

	#region Lateral Edges
	#region Modify Map
	public void AddLateralEdge(LatEdgeDesign edge)
	{
		if (edge == null) {
			return;
		}
		if (LateralEdgeExists(edge)) {
			return;
		}
		int id = GenerateUniqueLateralID();
		if (id < 1) {
			return;
		}
		_latid.Add(edge, id);
		_idlat.Add(id, edge);
		_idlatpat_map.Add(id, new List<LatEdgePattern>());
	}
	public void ReplaceLateralEdge(LatEdgeDesign oldedge, LatEdgeDesign newedge)
	{
		if (oldedge == null || newedge == null) {
			return;
		}
		if (!LateralEdgeExists(oldedge)) {
			return;
		}
		int oldid = _latid[oldedge];
		if (LateralEdgeExists(newedge)) {
			int newid = _latid[newedge];
			if (oldid == newid) {
				return;
			}
			_latid.Remove(oldedge);
			_latid.Remove(newedge);
			_latid.Add(oldedge, newid);
			_latid.Add(newedge, oldid);
			_idlat[oldid] = newedge;
			_idlat[newid] = oldedge;
		}
		else {
			_latid.Remove(oldedge);
			_latid.Add(newedge, oldid);
			_idlat[oldid] = newedge;
		}
	}
	public void DeleteLateralEdge(LatEdgeDesign edge)
	{
		if (edge == null) {
			return;
		}
		if (!LateralEdgeExists(edge)) {
			return;
		}
		int id = _latid[edge];
		if (id < 1) {
			return;
		}
		_latid.Remove(edge);
		_idlat.Remove(id);
		List<LatEdgePattern> patterns = _idlatpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			_latpatid_map.Remove(patterns[k]);
			_latpatpat_map.Remove(patterns[k]);
		}
		_idlatpat_map.Remove(id);
	}
	public void AddLateralPattern(LatEdgeDesign edge, LatEdgePattern pattern)
	{
		if (edge == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!LateralEdgeExists(edge) || LateralPatternExists(pattern)) {
			return;
		}
		int id = _latid[edge];
		if (id < 1) {
			return;
		}
		_latpatid_map.Add(pattern, id);
		_idlatpat_map[id].Add(pattern);
		_latpatpat_map.Add(pattern, pattern);
	}
	public void DeleteLateralPattern(LatEdgeDesign edge, LatEdgePattern pattern)
	{
		if (edge == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!LateralEdgeExists(edge) || !LateralPatternExists(pattern)) {
			return;
		}
		int id = _latid[edge];
		if (id < 1) {
			return;
		}
		_latpatid_map.Remove(pattern);
		_idlatpat_map[id].Remove(pattern);
		_latpatpat_map.Remove(pattern);
	}
	#endregion

	#region Query Map
	public bool LateralEdgeExists(LatEdgeDesign edge)
	{
		if (edge == null) {
			return false;
		}
		return _latid.ContainsKey(edge);
	}
	public bool LateralPatternExists(LatEdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return true;
		}
		return _latpatid_map.ContainsKey(pattern);
	}
	#endregion

	#region Getters
	public int GetLateralID(LatEdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return 0;
		}
		if (pattern.cid0 >= 0 && pattern.cid1 >= 0) {
			if (_latpatid_map.ContainsKey(pattern)) {
				return _latpatid_map[pattern];
			}
			LatEdgePattern pattern0 = pattern.GetPatternCopy(true, false);
			LatEdgePattern pattern1 = pattern.GetPatternCopy(false, true);
			bool t0 = _latpatid_map.ContainsKey(pattern0);
			bool t1 = _latpatid_map.ContainsKey(pattern1);
			if (t0 && !t1) {
				return _latpatid_map[pattern0];
			}
			else if (!t0 && t1) {
				return _latpatid_map[pattern1];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_latpatid_map.ContainsKey(pattern)) {
				return _latpatid_map[pattern];
			}
		}
		else if (pattern.cid0 >= 0 || pattern.cid1 >= 0) {
			if (_latpatid_map.ContainsKey(pattern)) {
				return _latpatid_map[pattern];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_latpatid_map.ContainsKey(pattern)) {
				return _latpatid_map[pattern];
			}
		}
		else {
			if (_latpatid_map.ContainsKey(pattern)) {
				return _latpatid_map[pattern];
			}
		}
		return 0;
	}
	public LatEdgeDesign GetLateralEdge(LatEdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return null;
		}
		return GetLateralEdge(GetLateralID(pattern));
	}
	public LatEdgeDesign GetLateralEdge(int id)
	{
		if (id == 0 || !_idlat.ContainsKey(id)) {
			return null;
		}
		return _idlat[id];
	}
	public LatEdgePattern GetLateralPattern(LatEdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return LatEdgePattern.empty;
		}
		if (pattern.cid0 >= 0 && pattern.cid1 >= 0) {
			if (_latpatpat_map.ContainsKey(pattern)) {
				return _latpatpat_map[pattern];
			}
			LatEdgePattern pattern0 = pattern.GetPatternCopy(true, false);
			LatEdgePattern pattern1 = pattern.GetPatternCopy(false, true);
			bool t0 = _latpatpat_map.ContainsKey(pattern0);
			bool t1 = _latpatpat_map.ContainsKey(pattern1);
			if (t0 && !t1) {
				return _latpatpat_map[pattern0];
			}
			else if (!t0 && t1) {
				return _latpatpat_map[pattern1];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_latpatpat_map.ContainsKey(pattern)) {
				return _latpatpat_map[pattern];
			}
		}
		else if (pattern.cid0 >= 0 || pattern.cid1 >= 0) {
			if (_latpatpat_map.ContainsKey(pattern)) {
				return _latpatpat_map[pattern];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_latpatpat_map.ContainsKey(pattern)) {
				return _latpatpat_map[pattern];
			}
		}
		else {
			if (_latpatpat_map.ContainsKey(pattern)) {
				return _latpatpat_map[pattern];
			}
		}
		return LatEdgePattern.empty;


	}
	#endregion

	#region List Getters
	public void AddLateralEdgesToList(List<VoxelComponent> list)
	{
		foreach (KeyValuePair<LatEdgeDesign, int> pair in _latid) {
			list.Add(pair.Key);
		}
	}
	public void AddLateralEdgesToList(List<LatEdgeDesign> list)
	{
		foreach (KeyValuePair<LatEdgeDesign, int> pair in _latid) {
			list.Add(pair.Key);
		}
	}
	public void AddLateralPatternsToList(LatEdgeDesign edge, List<LatEdgePattern> list)
	{
		if (!_latid.ContainsKey(edge)) {
			return;
		}
		int id = _latid[edge];
		if (!_idlatpat_map.ContainsKey(id)) {
			return;
		}
		List<LatEdgePattern> patterns = _idlatpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			list.Add(patterns[k]);
		}
	}
	#endregion

	#region Helpers
	private int GenerateUniqueLateralID()
	{
		for (int id = 1; id <= CAPACITY_LIMIT; id++) {
			if (!_idlat.ContainsKey(id)) {
				return id;
			}
		}
		return -1;
	}
	#endregion
	#endregion

	#region Longitude Edges
	#region Modify Map
	public void AddLongitudeEdge(LongEdgeDesign edge)
	{
		if (edge == null) {
			return;
		}
		if (LongitudeEdgeExists(edge)) {
			return;
		}
		int id = GenerateUniqueLongitudeID();
		if (id < 1) {
			return;
		}
		_longid.Add(edge, id);
		_idlong.Add(id, edge);
		_idlongpat_map.Add(id, new List<LongEdgePattern>());
	}
	public void ReplaceLongitudeEdge(LongEdgeDesign oldedge, LongEdgeDesign newedge)
	{
		if (oldedge == null || newedge == null) {
			return;
		}
		if (!LongitudeEdgeExists(oldedge)) {
			return;
		}
		int oldid = _longid[oldedge];
		if (LongitudeEdgeExists(newedge)) {
			int newid = _longid[newedge];
			if (oldid == newid) {
				return;
			}
			_longid.Remove(oldedge);
			_longid.Remove(newedge);
			_longid.Add(oldedge, newid);
			_longid.Add(newedge, oldid);
			_idlong[oldid] = newedge;
			_idlong[newid] = oldedge;
		}
		else {
			_longid.Remove(oldedge);
			_longid.Add(newedge, oldid);
			_idlong[oldid] = newedge;
		}
	}
	public void DeleteLongitudeEdge(LongEdgeDesign edge)
	{
		if (edge == null) {
			return;
		}
		if (!LongitudeEdgeExists(edge)) {
			return;
		}
		int id = _longid[edge];
		if (id < 1) {
			return;
		}
		_longid.Remove(edge);
		_idlong.Remove(id);
		List<LongEdgePattern> patterns = _idlongpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			_longpatid_map.Remove(patterns[k]);
			_longpatpat_map.Remove(patterns[k]);
		}
		_idlongpat_map.Remove(id);
	}
	public void AddLongitudePattern(LongEdgeDesign edge, LongEdgePattern pattern)
	{
		if (edge == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!LongitudeEdgeExists(edge) || LongitudePatternExists(pattern)) {
			return;
		}
		int id = _longid[edge];
		if (id < 1) {
			return;
		}
		_longpatid_map.Add(pattern, id);
		_idlongpat_map[id].Add(pattern);
		_longpatpat_map.Add(pattern, pattern);
	}
	public void DeleteLongitudePattern(LongEdgeDesign edge, LongEdgePattern pattern)
	{
		if (edge == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!LongitudeEdgeExists(edge) || !LongitudePatternExists(pattern)) {
			return;
		}
		int id = _longid[edge];
		if (id < 1) {
			return;
		}
		_longpatid_map.Remove(pattern);
		_idlongpat_map[id].Remove(pattern);
		_longpatpat_map.Remove(pattern);
	}
	#endregion

	#region Query Map
	public bool LongitudeEdgeExists(LongEdgeDesign edge)
	{
		if (edge == null) {
			return false;
		}
		return _longid.ContainsKey(edge);
	}
	public bool LongitudePatternExists(LongEdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return true;
		}
		return _longpatid_map.ContainsKey(pattern);
	}
	#endregion

	#region Getters
	public int GetLongitudeID(LongEdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return 0;
		}
		if (pattern.cid0 >= 0 && pattern.cid1 >= 0) {
			if (_longpatid_map.ContainsKey(pattern)) {
				return _longpatid_map[pattern];
			}
			LongEdgePattern pattern0 = pattern.GetPatternCopy(true, false);
			LongEdgePattern pattern1 = pattern.GetPatternCopy(false, true);
			bool t0 = _longpatid_map.ContainsKey(pattern0);
			bool t1 = _longpatid_map.ContainsKey(pattern1);
			if (t0 && !t1) {
				return _longpatid_map[pattern0];
			}
			else if (!t0 && t1) {
				return _longpatid_map[pattern1];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_longpatid_map.ContainsKey(pattern)) {
				return _longpatid_map[pattern];
			}
		}
		else if (pattern.cid0 >= 0 || pattern.cid1 >= 0) {
			if (_longpatid_map.ContainsKey(pattern)) {
				return _longpatid_map[pattern];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_longpatid_map.ContainsKey(pattern)) {
				return _longpatid_map[pattern];
			}
		}
		else {
			if (_longpatid_map.ContainsKey(pattern)) {
				return _longpatid_map[pattern];
			}
		}
		return 0;
	}
	public LongEdgeDesign GetLongitudeEdge(LongEdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return null;
		}
		return GetLongitudeEdge(GetLongitudeID(pattern));
	}
	public LongEdgeDesign GetLongitudeEdge(int id)
	{
		if (id == 0 || !_idlong.ContainsKey(id)) {
			return null;
		}
		return _idlong[id];
	}
	public LongEdgePattern GetLongitudePattern(LongEdgePattern pattern)
	{
		if (pattern.IsEmpty()) {
			return LongEdgePattern.empty;
		}
		if (pattern.cid0 >= 0 && pattern.cid1 >= 0) {
			if (_longpatpat_map.ContainsKey(pattern)) {
				return _longpatpat_map[pattern];
			}
			LongEdgePattern pattern0 = pattern.GetPatternCopy(true, false);
			LongEdgePattern pattern1 = pattern.GetPatternCopy(false, true);
			bool t0 = _longpatpat_map.ContainsKey(pattern0);
			bool t1 = _longpatpat_map.ContainsKey(pattern1);
			if (t0 && !t1) {
				return _longpatpat_map[pattern0];
			}
			else if (!t0 && t1) {
				return _longpatpat_map[pattern1];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_longpatpat_map.ContainsKey(pattern)) {
				return _longpatpat_map[pattern];
			}
		}
		else if (pattern.cid0 >= 0 || pattern.cid1 >= 0) {
			if (_longpatpat_map.ContainsKey(pattern)) {
				return _longpatpat_map[pattern];
			}
			pattern = pattern.GetPatternCopy(false, false);
			if (_longpatpat_map.ContainsKey(pattern)) {
				return _longpatpat_map[pattern];
			}
		}
		else {
			if (_longpatpat_map.ContainsKey(pattern)) {
				return _longpatpat_map[pattern];
			}
		}
		return LongEdgePattern.empty;


	}
	#endregion

	#region List Getters
	public void AddLongitudeEdgesToList(List<VoxelComponent> list)
	{
		foreach (KeyValuePair<LongEdgeDesign, int> pair in _longid) {
			list.Add(pair.Key);
		}
	}
	public void AddLongitudeEdgesToList(List<LongEdgeDesign> list)
	{
		foreach (KeyValuePair<LongEdgeDesign, int> pair in _longid) {
			list.Add(pair.Key);
		}
	}
	public void AddLongitudePatternsToList(LongEdgeDesign edge, List<LongEdgePattern> list)
	{
		if (!_longid.ContainsKey(edge)) {
			return;
		}
		int id = _longid[edge];
		if (!_idlongpat_map.ContainsKey(id)) {
			return;
		}
		List<LongEdgePattern> patterns = _idlongpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			list.Add(patterns[k]);
		}
	}
	#endregion

	#region Helpers
	private int GenerateUniqueLongitudeID()
	{
		for (int id = 1; id <= CAPACITY_LIMIT; id++) {
			if (!_idlong.ContainsKey(id)) {
				return id;
			}
		}
		return -1;
	}
	#endregion
	#endregion

	#region Rect Face
	#region Modify Map
	public void AddRectFace(RectFaceDesign face)
	{
		if (face == null) {
			return;
		}
		if (RectFaceExists(face)) {
			return;
		}
		int id = GenerateUniqueRectID();
		if (id < 1) {
			return;
		}
		_rectid.Add(face, id);
		_idrect.Add(id, face);
		_idrectpat_map.Add(id, new List<RectFacePattern>());
	}
	public void ReplaceRectFace(RectFaceDesign oldface, RectFaceDesign newface)
	{
		if (oldface == null || newface == null) {
			return;
		}
		if (!RectFaceExists(oldface)) {
			return;
		}
		int oldid = _rectid[oldface];
		if (RectFaceExists(newface)) {
			int newid = _rectid[newface];
			if (oldid == newid) {
				return;
			}
			_rectid.Remove(oldface);
			_rectid.Remove(newface);
			_rectid.Add(oldface, newid);
			_rectid.Add(newface, oldid);
			_idrect[oldid] = newface;
			_idrect[newid] = oldface;
		}
		else {
			_rectid.Remove(oldface);
			_rectid.Add(newface, oldid);
			_idrect[oldid] = newface;
		}
	}
	public void DeleteRectFace(RectFaceDesign face)
	{
		if (face == null) {
			return;
		}
		if (!RectFaceExists(face)) {
			return;
		}
		int id = _rectid[face];
		if (id < 1) {
			return;
		}
		_rectid.Remove(face);
		_idrect.Remove(id);
		List<RectFacePattern> patterns = _idrectpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			_rectpatid_map.Remove(patterns[k]);
			_rectpatpat_map.Remove(patterns[k]);
		}
		_idrectpat_map.Remove(id);
	}
	public void AddRectPattern(RectFaceDesign face, RectFacePattern pattern)
	{
		if (face == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!RectFaceExists(face) || RectPatternExists(pattern)) {
			return;
		}
		int id = _rectid[face];
		if (id < 1) {
			return;
		}
		_idrectpat_map[id].Add(pattern);
		_rectpatid_map.Add(pattern, id);
		_rectpatpat_map.Add(pattern, pattern);
	}
	public void DeleteRectPattern(RectFaceDesign face, RectFacePattern pattern)
	{
		if (face == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!RectFaceExists(face) || !RectPatternExists(pattern)) {
			return;
		}
		int id = _rectid[face];
		if (id < 1) {
			return;
		}
		_idrectpat_map[id].Remove(pattern);
		_rectpatid_map.Remove(pattern);
		_rectpatpat_map.Remove(pattern);
	}
	#endregion

	#region Query Map
	public bool RectFaceExists(RectFaceDesign face)
	{
		if (face == null) {
			return false;
		}
		return _rectid.ContainsKey(face);
	}
	public bool RectPatternExists(RectFacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return false;
		}
		return _rectpatid_map.ContainsKey(pattern);
	}
	#endregion

	#region Getters
	public int GetRectID(RectFacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return 0;
		}
		if (!_rectpatid_map.ContainsKey(pattern)) {
			return 0;
		}
		return _rectpatid_map[pattern];
	}
	public RectFaceDesign GetRectFace(int id)
	{
		if (id <= 0 || !_idrect.ContainsKey(id)) {
			return null;
		}
		return _idrect[id];
	}
	public RectFaceDesign GetRectFace(RectFacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return null;
		}
		if (!_rectpatid_map.ContainsKey(pattern)) {
			return null;
		}
		return _idrect[_rectpatid_map[pattern]];
	}
	public RectFacePattern GetRectPattern(RectFacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return RectFacePattern.empty;
		}
		if (!_rectpatpat_map.ContainsKey(pattern)) {
			return RectFacePattern.empty;
		}
		return _rectpatpat_map[pattern];
	}
	#endregion

	#region List Getters
	public void AddRectFacesToList(List<VoxelComponent> list)
	{
		foreach (KeyValuePair<RectFaceDesign, int> pair in _rectid) {
			list.Add(pair.Key);
		}
	}
	public void AddRectFacesToList(List<RectFaceDesign> list)
	{
		foreach (KeyValuePair<RectFaceDesign, int> pair in _rectid) {
			list.Add(pair.Key);
		}
	}
	public void AddRectPatternsToList(RectFaceDesign face, List<RectFacePattern> list)
	{
		if (!_rectid.ContainsKey(face)) {
			return;
		}
		int id = _rectid[face];
		if (!_idrectpat_map.ContainsKey(id)) {
			return;
		}
		List<RectFacePattern> patterns = _idrectpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			list.Add(patterns[k]);
		}
	}
	#endregion

	#region Helpers
	private int GenerateUniqueRectID()
	{
		for (int id = 1; id <= CAPACITY_LIMIT; id++) {
			if (!_idrect.ContainsKey(id)) {
				return id;
			}
		}
		return -1;
	}
	#endregion
	#endregion

	#region HexagonFace
	#region Modify Map
	public void AddHexagonFace(HexagonFaceDesign face)
	{
		if (face == null) {
			return;
		}
		if (HexagonFaceExists(face)) {
			return;
		}
		int id = GenerateUniqueHexagonID();
		if (id < 1) {
			return;
		}
		_hexid.Add(face, id);
		_idhex.Add(id, face);
		_idhexpat_map.Add(id, new List<HexagonFacePattern>());
	}
	public void ReplaceHexagonFace(HexagonFaceDesign oldface, HexagonFaceDesign newface)
	{
		if (oldface == null || newface == null) {
			return;
		}
		if (!HexagonFaceExists(oldface)) {
			return;
		}
		int oldid = _hexid[oldface];
		if (HexagonFaceExists(newface)) {
			int newid = _hexid[newface];
			if (oldid == newid) {
				return;
			}
			_hexid.Remove(oldface);
			_hexid.Remove(newface);
			_hexid.Add(oldface, newid);
			_hexid.Add(newface, oldid);
			_idhex[oldid] = newface;
			_idhex[newid] = oldface;
		}
		else {
			_hexid.Remove(oldface);
			_hexid.Add(newface, oldid);
			_idhex[oldid] = newface;
		}
	}
	public void DeleteHexagonFace(HexagonFaceDesign face)
	{
		if (face == null) {
			return;
		}
		if (!HexagonFaceExists(face)) {
			return;
		}
		int id = _hexid[face];
		if (id < 1) {
			return;
		}
		_hexid.Remove(face);
		_idhex.Remove(id);
		List<HexagonFacePattern> patterns = _idhexpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			_hexpatid_map.Remove(patterns[k]);
			_hexpatpat_map.Remove(patterns[k]);
		}
		_idhexpat_map.Remove(id);
	}
	public void AddHexagonPattern(HexagonFaceDesign face, HexagonFacePattern pattern)
	{
		if (face == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!HexagonFaceExists(face) || HexagonPatternExists(pattern)) {
			return;
		}
		int id = _hexid[face];
		if (id < 1) {
			return;
		}
		_idhexpat_map[id].Add(pattern);
		_hexpatid_map.Add(pattern, id);
		_hexpatpat_map.Add(pattern, pattern);
	}
	public void DeleteHexagonPattern(HexagonFaceDesign face, HexagonFacePattern pattern)
	{
		if (face == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		if (!HexagonFaceExists(face) || !HexagonPatternExists(pattern)) {
			return;
		}
		int id = _hexid[face];
		if (id < 1) {
			return;
		}
		_idhexpat_map[id].Remove(pattern);
		_hexpatid_map.Remove(pattern);
		_hexpatpat_map.Remove(pattern);
	}
	#endregion

	#region Query Map
	public bool HexagonFaceExists(HexagonFaceDesign face)
	{
		if (face == null) {
			return false;
		}
		return _hexid.ContainsKey(face);
	}
	public bool HexagonPatternExists(HexagonFacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return false;
		}
		return _hexpatid_map.ContainsKey(pattern);
	}
	#endregion

	#region Getters
	public int GetHexagonID(HexagonFacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return 0;
		}
		if (!_hexpatid_map.ContainsKey(pattern)) {
			return 0;
		}
		return _hexpatid_map[pattern];
	}
	public HexagonFaceDesign GetHexagonFace(int id)
	{
		if (id <= 0 || !_idhex.ContainsKey(id)) {
			return null;
		}
		return _idhex[id];
	}
	public HexagonFaceDesign GetHexagonFace(HexagonFacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return null;
		}
		if (!_hexpatid_map.ContainsKey(pattern)) {
			return null;
		}
		return _idhex[_hexpatid_map[pattern]];
	}
	public HexagonFacePattern GetHexagonPattern(HexagonFacePattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return HexagonFacePattern.empty;
		}
		if (!_hexpatpat_map.ContainsKey(pattern)) {
			return HexagonFacePattern.empty;
		}
		return _hexpatpat_map[pattern];
	}
	#endregion

	#region List Getters
	public void AddHexagonFacesToList(List<VoxelComponent> list)
	{
		foreach (KeyValuePair<HexagonFaceDesign, int> pair in _hexid) {
			list.Add(pair.Key);
		}
	}
	public void AddHexagonFacesToList(List<HexagonFaceDesign> list)
	{
		foreach (KeyValuePair<HexagonFaceDesign, int> pair in _hexid) {
			list.Add(pair.Key);
		}
	}
	public void AddHexagonPatternsToList(HexagonFaceDesign face, List<HexagonFacePattern> list)
	{
		if (!_hexid.ContainsKey(face)) {
			return;
		}
		int id = _hexid[face];
		if (!_idhexpat_map.ContainsKey(id)) {
			return;
		}
		List<HexagonFacePattern> patterns = _idhexpat_map[id];
		for (int k = 0; k < patterns.Count; k++) {
			list.Add(patterns[k]);
		}
	}
	#endregion

	#region Helpers
	private int GenerateUniqueHexagonID()
	{
		for (int id = 1; id <= CAPACITY_LIMIT; id++) {
			if (!_idhex.ContainsKey(id)) {
				return id;
			}
		}
		return -1;
	}
	#endregion
	#endregion
}
