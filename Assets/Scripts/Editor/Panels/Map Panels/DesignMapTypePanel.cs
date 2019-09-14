using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

public class DesignMapTypePanel<T> : PanelGUI where T : VoxelComponent
{
	public MappingObject target;

	private Rect _rect_header;
	private Rect _rect_guidscroll;
	private Rect _rect_guidcontent;
	private Rect _rect_panel;
	private Rect _rect_libscroll;
	private Rect _rect_libcontent;
	private Vector2 _guidscroll;
	private Vector2 _libscroll;

	//GUID List
	private List<string> _guids;
	private List<string> _guidstrs;
	private ReorderableList _guidlist;
	//Voxel Object List
	private List<VoxelComponent> _libobjs;
	private ReorderableList _libobjlist;

	public DesignMapTypePanel(string title)
	{
		_title = title;
		//initialize guid list
		_guids = new List<string>();
		_guidstrs = new List<string>();
		//initialize library object list
		_libobjs = new List<VoxelComponent>();
	}

	public override void Enable()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_guidscroll = Vector2.zero;
		_libscroll = Vector2.zero;

		//initialize guid list
		_guidlist = new ReorderableList(_guids, typeof(string), false, false, false, false);
		_guidlist.showDefaultBackground = false;
		_guidlist.headerHeight = 0f;
		_guidlist.footerHeight = 0f;
		_guidlist.elementHeight = VxlGUI.MED_BAR;
		_guidlist.drawElementCallback += (Rect rect, int index, bool active, bool focus) => {
			DrawElement(rect, index, active, focus, true);
		};
		_guidlist.drawElementBackgroundCallback += (Rect rect, int index, bool active, bool focus) => {
			DrawElementBackground(rect, index, active, focus, true);
		};
		_guidlist.drawNoneElementCallback += (Rect rect) => {
			DrawListNone(rect, true);
		};
		//initialize library object list
		_libobjlist = new ReorderableList(_libobjs, typeof(VoxelComponent), false, false, false, false);
		_libobjlist.showDefaultBackground = false;
		_libobjlist.headerHeight = 0f;
		_libobjlist.footerHeight = 0f;
		_libobjlist.elementHeight = VxlGUI.MED_BAR;
		_libobjlist.drawElementCallback += (Rect rect, int index, bool active, bool focus) => {
			DrawElement(rect, index, active, focus, false);
		};
		_libobjlist.drawElementBackgroundCallback += (Rect rect, int index, bool active, bool focus) => {
			DrawElementBackground(rect, index, active, focus, false);
		};
		_libobjlist.drawNoneElementCallback += (Rect rect) => {
			DrawListNone(rect, false);
		};
	}

	public override void Disable()
	{
		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
		target = null;

		_guidlist = null;
		_guids.Clear();
		_guidstrs.Clear();
		_libobjlist = null;
		_libobjs.Clear();
	}

	public override void DrawGUI(Rect rect)
	{
		//update and populate lists
		UpdateLists();
		//update layout rects
		UpdateLayoutRect(rect);
		//draw header
		VxlGUI.DrawRect(_rect_header, "DarkGradient");
		GUI.Label(_rect_header, _title, GUI.skin.GetStyle("LeftLightHeader"));
		//GUID
		VxlGUI.DrawRect(_rect_guidscroll, "DarkWhite");
		_guidscroll = GUI.BeginScrollView(_rect_guidscroll, _guidscroll, _rect_guidcontent);
		_guidlist.DoList(_rect_guidcontent);
		GUI.EndScrollView();
		//transfer panel
		bool disable_guid = _guidlist.index < 0 || _guidlist.index >= _guidlist.count;
		bool disable_lib = _libobjlist.index < 0 || _libobjlist.index >= _libobjlist.count;
		VxlGUI.DrawRect(_rect_panel, "DarkGradient");
		float boundwidth = Mathf.Min(60f, _rect_panel.width / 3f);
		//down button
		Rect rect_button = VxlGUI.GetMiddleX(VxlGUI.GetLeftElement(_rect_panel, 0, boundwidth), _rect_panel.height);
		EditorGUI.BeginDisabledGroup(disable_guid);
		if (GUI.Button(rect_button, "", GUI.skin.GetStyle("ArrowDown"))) {
			AddSelected();
		}
		EditorGUI.EndDisabledGroup();
		//up button
		rect_button = VxlGUI.GetMiddleX(VxlGUI.GetMiddleX(_rect_panel, boundwidth), _rect_panel.height);
		EditorGUI.BeginDisabledGroup(disable_lib);
		if (GUI.Button(rect_button, "", GUI.skin.GetStyle("ArrowUp"))) {
			RemoveSelected();
		}
		EditorGUI.EndDisabledGroup();
		//swap button
		rect_button = VxlGUI.GetMiddleX(VxlGUI.GetRightElement(_rect_panel, 0, boundwidth), _rect_panel.height);
		EditorGUI.BeginDisabledGroup(disable_guid || disable_lib);
		if (GUI.Button(rect_button, "", GUI.skin.GetStyle("ArrowSwap"))) {
			SwapSelected();
		}
		EditorGUI.EndDisabledGroup();
		//library
		VxlGUI.DrawRect(_rect_libscroll, "DarkWhite");
		_libscroll = GUI.BeginScrollView(_rect_libscroll, _libscroll, _rect_libcontent);
		_libobjlist.DoList(_rect_libcontent);
		GUI.EndScrollView();
	}

	public override void DrawPreview(Rect rect) { }

	public void UpdateLists()
	{
		//record list guids
		VoxelComponent lib_obj = null;
		if (_libobjlist.index >= 0 && _libobjlist.index < _libobjlist.count) {
			lib_obj = _libobjs[_libobjlist.index];
		}
		string guid_guid = "";
		if (_guidlist.index >= 0 && _guidlist.index < _guidlist.count) {
			guid_guid = _guids[_guidlist.index];
		}
		//update library lists
		_libobjs.Clear();
		if (target != null) {
			if (typeof(T) == typeof(CornerDesign)) {
				target.AddCornersToList(_libobjs);
			}
			else if (typeof(T) == typeof(LatEdgeDesign)) {
				target.AddLateralEdgesToList(_libobjs);
			}
			else if (typeof(T) == typeof(LongEdgeDesign)) {
				target.AddLongitudeEdgesToList(_libobjs);
			}
			else if (typeof(T) == typeof(RectFaceDesign)) {
				target.AddRectFacesToList(_libobjs);
			}
			else if (typeof(T) == typeof(HexagonFaceDesign)) {
				target.AddHexagonFacesToList(_libobjs);
			}
		}
		HashSet<string> lib_guids = new HashSet<string>();
		for (int k = 0; k < _libobjs.Count; k++) {
			string guid;
			long local_id;
			if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_libobjs[k].GetInstanceID(), out guid, out local_id)) {
				continue;
			}
			lib_guids.Add(guid);
		}
		//update guid list
		_guids.Clear();
		_guidstrs.Clear();
		string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).ToString());
		for (int k = 0; k < guids.Length; k++) {
			if (lib_guids.Contains(guids[k])) {
				continue;
			}
			string name = AssetDatabase.GUIDToAssetPath(guids[k]);
			if (name == null || name.Length <= 0) {
				continue;
			}
			name = Path.GetFileNameWithoutExtension(name);
			if (name == null || name.Length <= 0) {
				continue;
			}
			_guids.Add(guids[k]);
			_guidstrs.Add(name);
		}
		//set new list indices
		_libobjlist.index = _libobjs.IndexOf(lib_obj);
		_guidlist.index = _guids.IndexOf(guid_guid);
	}

	private void AddSelected()
	{
		if (target == null) {
			return;
		}
		if (_guidlist.index < 0 || _guidlist.index >= _guidlist.count) {
			return;
		}
		string path = AssetDatabase.GUIDToAssetPath(_guids[_guidlist.index]);
		if (typeof(T) == typeof(CornerDesign)) {
			CornerDesign asset = (CornerDesign)AssetDatabase.LoadAssetAtPath(path, typeof(CornerDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Added CornerDesign to MappingObject");
			target.AddCorner(asset);
			//dirty target object
			EditorUtility.SetDirty(target);
		}
		else if (typeof(T) == typeof(LatEdgeDesign)) {
			LatEdgeDesign asset = (LatEdgeDesign)AssetDatabase.LoadAssetAtPath(path, typeof(LatEdgeDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Added LatEdgeDesign to MappingObject");
			target.AddLateralEdge(asset);
			//dirty target object
			EditorUtility.SetDirty(target);
		}
		else if (typeof(T) == typeof(LongEdgeDesign)) {
			LongEdgeDesign asset = (LongEdgeDesign)AssetDatabase.LoadAssetAtPath(path, typeof(LongEdgeDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Added LongEdgeDesign to MappingObject");
			target.AddLongitudeEdge(asset);
			//dirty target object
			EditorUtility.SetDirty(target);
		}
		else if (typeof(T) == typeof(RectFaceDesign)) {
			RectFaceDesign asset = (RectFaceDesign)AssetDatabase.LoadAssetAtPath(path, typeof(RectFaceDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Added RectFaceDesign to MappingObject");
			target.AddRectFace(asset);
			//dirty target object
			EditorUtility.SetDirty(target);
		}
		else if (typeof(T) == typeof(HexagonFaceDesign)) {
			HexagonFaceDesign asset = (HexagonFaceDesign)AssetDatabase.LoadAssetAtPath(path, typeof(HexagonFaceDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Added HexagonFaceDesign to MappingObject");
			target.AddHexagonFace(asset);
			//dirty target object
			EditorUtility.SetDirty(target);
		}
		else {
			return;
		}
		_repaint_menu = true;
	}

	private void RemoveSelected()
	{
		if (target == null) {
			return;
		}
		if (_libobjlist.index < 0 || _libobjlist.index >= _libobjlist.count) {
			return;
		}
		if (typeof(T) == typeof(CornerDesign)) {
			Undo.RecordObject(target, "Deleted " + typeof(T).ToString() + " from MappingObject");
			target.DeleteCorner((CornerDesign)_libobjs[_libobjlist.index]);
		}
		else if (typeof(T) == typeof(LatEdgeDesign)) {
			Undo.RecordObject(target, "Deleted " + typeof(T).ToString() + " from MappingObject");
			target.DeleteLateralEdge((LatEdgeDesign)_libobjs[_libobjlist.index]);
		}
		else if (typeof(T) == typeof(LongEdgeDesign)) {
			Undo.RecordObject(target, "Deleted " + typeof(T).ToString() + " from MappingObject");
			target.DeleteLongitudeEdge((LongEdgeDesign)_libobjs[_libobjlist.index]);
		}
		else if (typeof(T) == typeof(RectFaceDesign)) {
			Undo.RecordObject(target, "Deleted " + typeof(T).ToString() + " from MappingObject");
			target.DeleteRectFace((RectFaceDesign)_libobjs[_libobjlist.index]);
		}
		else if (typeof(T) == typeof(HexagonFaceDesign)) {
			Undo.RecordObject(target, "Deleted " + typeof(T).ToString() + " from MappingObject");
			target.DeleteHexagonFace((HexagonFaceDesign)_libobjs[_libobjlist.index]);
		}
		else {
			return;
		}
		_repaint_menu = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}

	private void SwapSelected()
	{
		if (target == null) {
			return;
		}
		if (_guidlist.index < 0 || _guidlist.index >= _guidlist.count) {
			return;
		}
		if (_libobjlist.index < 0 || _libobjlist.index >= _libobjlist.count) {
			return;
		}
		string path = AssetDatabase.GUIDToAssetPath(_guids[_guidlist.index]);
		if (typeof(T) == typeof(CornerDesign)) {
			CornerDesign asset = (CornerDesign)AssetDatabase.LoadAssetAtPath(path, typeof(CornerDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Replaced " + typeof(T).ToString() + "'s in MappingObject");
			target.ReplaceCorner((CornerDesign)_libobjs[_libobjlist.index], asset);
		}
		else if (typeof(T) == typeof(LatEdgeDesign)) {
			LatEdgeDesign asset = (LatEdgeDesign)AssetDatabase.LoadAssetAtPath(path, typeof(LatEdgeDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Replaced " + typeof(T).ToString() + "'s in MappingObject");
			target.ReplaceLateralEdge((LatEdgeDesign)_libobjs[_libobjlist.index], asset);
		}
		else if (typeof(T) == typeof(LongEdgeDesign)) {
			LongEdgeDesign asset = (LongEdgeDesign)AssetDatabase.LoadAssetAtPath(path, typeof(LongEdgeDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Replaced " + typeof(T).ToString() + "'s in MappingObject");
			target.ReplaceLongitudeEdge((LongEdgeDesign)_libobjs[_libobjlist.index], asset);
		}
		else if (typeof(T) == typeof(RectFaceDesign)) {
			RectFaceDesign asset = (RectFaceDesign)AssetDatabase.LoadAssetAtPath(path, typeof(RectFaceDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Replaced " + typeof(T).ToString() + "'s in MappingObject");
			target.ReplaceRectFace((RectFaceDesign)_libobjs[_libobjlist.index], asset);
		}
		else if (typeof(T) == typeof(HexagonFaceDesign)) {
			HexagonFaceDesign asset = (HexagonFaceDesign)AssetDatabase.LoadAssetAtPath(path, typeof(HexagonFaceDesign));
			if (asset == null) {
				return;
			}
			Undo.RecordObject(target, "Replaced " + typeof(T).ToString() + "'s in MappingObject");
			target.ReplaceHexagonFace((HexagonFaceDesign)_libobjs[_libobjlist.index], asset);
		}
		else {
			return;
		}
		_repaint_menu = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}

	private void UpdateLayoutRect(Rect rect)
	{
		_rect_header = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR);
		Rect rect_body = VxlGUI.GetSandwichedRectY(rect, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, 0);
		float listheight = Mathf.Max(0, (rect_body.height - VxlGUI.MED_BAR - VxlGUI.SM_SPACE - VxlGUI.SM_SPACE) / 2f);
		_rect_guidscroll = VxlGUI.GetAboveElement(rect_body, 0, listheight);
		_rect_guidcontent = VxlGUI.GetScrollViewRect(_guidlist, _rect_guidscroll.width, _rect_guidscroll.height);
		_rect_panel = VxlGUI.GetMiddleY(rect_body, VxlGUI.MED_BAR);
		_rect_libscroll = VxlGUI.GetBelowElement(rect_body, 0, listheight);
		_rect_libcontent = VxlGUI.GetScrollViewRect(_libobjlist, _rect_libscroll.width, _rect_libscroll.height);
	}

	private void DrawListNone(Rect rect, bool is_guid)
	{
		GUI.Label(rect, "No " + typeof(T).ToString() + " Found.", GUI.skin.GetStyle("RightSelectableSmallLabel"));
	}

	private void DrawElementBackground(Rect rect, int index, bool active, bool focus, bool is_guid)
	{
		bool hover, on;
		if (is_guid) {
			hover = (_guids.Count > 0) && rect.Contains(Event.current.mousePosition);
			on = (_guidlist.index >= 0 && _guidlist.index == index);
		}
		else {
			hover = (_libobjs.Count > 0) && rect.Contains(Event.current.mousePosition);
			on = (_libobjlist.index >= 0 && _libobjlist.index == index);
		}
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}

	private void DrawElement(Rect rect, int index, bool active, bool focus, bool is_guid)
	{
		string name;
		if (is_guid) {
			name = _guidstrs[index];
		}
		else {
			name = _libobjs[index].objname;
		}
		if (string.IsNullOrEmpty(name)) {
			name = "Invalid Name";
		}
		GUI.Label(rect, name, GUI.skin.GetStyle("RightSelectableSmallLabel"));
	}
}