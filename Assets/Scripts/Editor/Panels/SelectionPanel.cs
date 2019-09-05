using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

public class SelectionPanel<T> : PanelGUI where T : VoxelObject
{
	public T selected { get; private set; }

	private Rect _rect_rename;
	private Rect _rect_guidheader;
	private Rect _rect_guidscroll;
	private Rect _rect_guidcontent;
	private Rect _rect_guidfooter;
	private Rect _rect_unsavedheader;
	private Rect _rect_unsavedscroll;
	private Rect _rect_unsavedcontent;
	private Rect _rect_unsavedfooter;

	private bool _unsaved_selected;
	private int _index;
	private Vector2 _scroll_guid;
	private Vector2 _scroll_unsaved;
	private List<string> _guids;
	private List<string> _guidstrs;
	private List<T> _unsaveds;
	private ReorderableList _guidlist;
	private ReorderableList _unsavedlist;

	public SelectionPanel(string title)
	{
		_title = title;
		selected = null;
		_unsaved_selected = false;
		_index = -1;
		_guids = new List<string>();
		_guidstrs = new List<string>();
		_unsaveds = new List<T>();
	}

	public override void Enable()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_scroll_guid = Vector2.zero;
		_scroll_unsaved = Vector2.zero;
		//initialize guid reorderable list
		_guidlist = new ReorderableList(_guids, typeof(string), false, false, false, false);
		_guidlist.showDefaultBackground = false;
		_guidlist.headerHeight = 0f;
		_guidlist.footerHeight = 0f;
		_guidlist.elementHeight = VxlGUI.MED_BAR;
		_guidlist.drawElementCallback += DrawGUIDElement;
		_guidlist.drawElementBackgroundCallback += DrawGUIDElementBackground;
		_guidlist.onSelectCallback += SelectGUIDElement;
		_guidlist.drawNoneElementCallback += DrawGUIDNone;
		//initialize unsaved objects list
		_unsavedlist = new ReorderableList(_unsaveds, typeof(T), false, false, false, false);
		_unsavedlist.showDefaultBackground = false;
		_unsavedlist.headerHeight = 0f;
		_unsavedlist.footerHeight = 0f;
		_unsavedlist.drawElementCallback += DrawUnsavedElement;
		_unsavedlist.drawElementBackgroundCallback += DrawUnsavedElementBackground;
		_unsavedlist.elementHeightCallback += UnsavedElementHeight;
		_unsavedlist.onSelectCallback += SelectUnsavedElement;
		_unsavedlist.drawNoneElementCallback += DrawUnsavedNone;
		//refresh project guid list
		RefreshAssets();
	}

	public override void Disable()
	{
		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
		_guidlist = null;
		_guids.Clear();
		_guidstrs.Clear();
		_unsavedlist = null;
		_unsaveds.Clear();
	}

	public override void DrawGUI(Rect rect)
	{
		UpdateRects(rect);

		//rename - do later
		EditorGUI.BeginDisabledGroup(selected == null);
		VxlGUI.DrawRect(_rect_rename, "LightBlack");
		EditorGUI.BeginChangeCheck();
		GUI.Label(VxlGUI.GetLeftColumn(_rect_rename, 0, 0.5f), "Rename:", GUI.skin.GetStyle("RightLightLabel"));
		string name = "";
		if (selected != null) {
			name = selected.objname;
		}
		name = EditorGUI.DelayedTextField(
			VxlGUI.GetPaddedRect(VxlGUI.GetRightColumn(_rect_rename, 0, 0.5f), VxlGUI.SM_PAD), 
			name, 
			GUI.skin.GetStyle("RightLightText")
		);
		if (EditorGUI.EndChangeCheck()) {
			if (RenameSelectedUnsaved(name) || RenameSelectedAsset(name)) {
				if (!_unsaved_selected) {
					SetSelectedFromGUIDList();
				}
				RefreshAssets();
			}
		}
		EditorGUI.EndDisabledGroup();

		//guid list
		VxlGUI.DrawRect(_rect_guidheader, "DarkGradient");
		VxlGUI.DrawRect(_rect_guidfooter, "DarkGradient");
		VxlGUI.DrawRect(_rect_guidscroll, "DarkWhite");
		GUI.Label(_rect_guidheader, "Project Assets", GUI.skin.GetStyle("LeftLightHeader"));
		_scroll_guid = GUI.BeginScrollView(_rect_guidscroll, _scroll_guid, _rect_guidcontent);
		_guidlist.DoList(_rect_guidcontent);
		GUI.EndScrollView();
		float width = Mathf.Min(60f, _rect_guidfooter.width);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_guidfooter, 0, width), "Refresh", GUI.skin.GetStyle("LightButton"))) {
			RefreshAssets();
		}

		//unsaved list
		VxlGUI.DrawRect(_rect_unsavedheader, "DarkGradient");
		VxlGUI.DrawRect(_rect_unsavedfooter, "DarkGradient");
		VxlGUI.DrawRect(_rect_unsavedscroll, "DarkWhite");
		GUI.Label(_rect_unsavedheader, "Unsaved Objects", GUI.skin.GetStyle("LeftLightHeader"));
		_scroll_unsaved = GUI.BeginScrollView(_rect_unsavedscroll, _scroll_unsaved, _rect_unsavedcontent);
		if (_unsavedlist != null) {
			_unsavedlist.DoList(_rect_unsavedcontent);
		}
		GUI.EndScrollView();
		width = Mathf.Min(60f, _rect_unsavedfooter.width / 3f);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_unsavedfooter, 0, width), "Add", GUI.skin.GetStyle("LightButton"))) {
			AddUnsaved();
		}
		EditorGUI.BeginDisabledGroup(!_unsaved_selected || _index < 0 || _index >= _unsaveds.Count);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_unsavedfooter, 1, width), "Save", GUI.skin.GetStyle("LightButton"))) {
			SaveAsset();
			RefreshAssets();
			SetSelectedFromGUIDList();
		}
		if (GUI.Button(VxlGUI.GetLeftElement(_rect_unsavedfooter, 0, width), "Delete", GUI.skin.GetStyle("LightButton"))) {
			DeleteUnsaved();
		}
		EditorGUI.EndDisabledGroup();
	}

	private void UpdateRects(Rect rect)
	{
		_rect_rename = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR + VxlGUI.LRG_SPACE - VxlGUI.MED_SPACE);
		Rect rect_bottom = VxlGUI.GetSandwichedRectY(rect, VxlGUI.MED_BAR + VxlGUI.LRG_SPACE, 0);
		Rect rect_left = VxlGUI.GetLeftColumn(rect_bottom, VxlGUI.SM_SPACE, 0.5f);
		Rect rect_right = VxlGUI.GetRightColumn(rect_bottom, VxlGUI.SM_SPACE, 0.5f);
		_rect_guidheader = VxlGUI.GetAboveElement(rect_left, 0, VxlGUI.MED_BAR);
		_rect_guidfooter = VxlGUI.GetBelowElement(rect_left, 0, VxlGUI.MED_BAR);
		_rect_guidscroll = VxlGUI.GetSandwichedRectY(rect_left, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.MED_BAR + VxlGUI.SM_SPACE);
		_rect_guidcontent = VxlGUI.GetScrollViewRect(_guidlist, rect_left.width, _rect_guidscroll.height);

		_rect_unsavedheader = VxlGUI.GetAboveElement(rect_right, 0, VxlGUI.MED_BAR);
		_rect_unsavedfooter = VxlGUI.GetBelowElement(rect_right, 0, VxlGUI.MED_BAR);
		_rect_unsavedscroll = VxlGUI.GetSandwichedRectY(rect_right, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.MED_BAR + VxlGUI.SM_SPACE);
		_rect_unsavedcontent = VxlGUI.GetScrollViewRect(_unsavedlist, rect_right.width, _rect_unsavedscroll.height);
	}

	#region GUID Asset Methods
	public void RefreshAssets()
	{
		_guids.Clear();
		_guidstrs.Clear();
		string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).ToString());
		for (int k = 0; k < guids.Length; k++) {
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
	}
	private bool RenameSelectedAsset(string text)
	{
		if (_unsaved_selected) {
			return false;
		}
		if (string.IsNullOrWhiteSpace(text)) {
			return false;
		}
		if (selected == null) {
			return false;
		}
		char[] invalids = Path.GetInvalidFileNameChars();
		if (text.IndexOfAny(invalids) >= 0) {
			return false;
		}
		text = text.Trim();
		if (string.Equals(selected.objname.Trim(), text)) {
			return false;
		}
		string path = AssetDatabase.GetAssetPath(selected);
		string returned_path = AssetDatabase.RenameAsset(path, text);
		if (returned_path.Length > 0) {
			return false;
		}
		selected.objname = text;
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		_repaint_menu = true;
		return true;
	}
	#endregion

	#region Unsaved Methods
	private void SaveAsset()
	{
		if (!_unsaved_selected || _index < 0 || _index >= _unsaveds.Count || selected == null) {
			return;
		}
		if (selected == null || string.IsNullOrEmpty(selected.objname)) {
			return;
		}
		string path = EditorUtility.SaveFilePanel("Save " + typeof(T).ToString(), Application.dataPath, selected.objname + ".asset", "asset");
		if (string.IsNullOrEmpty(path)) {
			return;
		}
		//get relative path
		path = "Assets" + path.Replace(Application.dataPath, "");
		path = AssetDatabase.GenerateUniqueAssetPath(path);
		if (string.IsNullOrEmpty(path)) {
			return;
		}
		AssetDatabase.CreateAsset(selected, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		_unsaveds.RemoveAt(_index);
		_unsaved_selected = false;
		_index = -1;
		_repaint_menu = true;
	}
	private void SetSelectedFromGUIDList()
	{
		if (selected == null) {
			return;
		}
		_unsaved_selected = false;
		_index = -1;
		string objguid;
		long localid;
		if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(selected, out objguid, out localid)) {
			return;
		}
		for (int k = 0; k < _guids.Count; k++) {
			if (!string.Equals(objguid, _guids[k])) {
				continue;
			}
			_index = k;
			return;
		}
		selected = null;
	}
	private void AddUnsaved()
	{
		_unsaved_selected = true;
		selected = ScriptableObject.CreateInstance<T>();
		selected.Initialize();
		_index = _unsaveds.Count;
		_unsaveds.Add(selected);
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
	}
	private void DeleteUnsaved()
	{
		if (!_unsaved_selected || _index < 0 || _index >= _unsaveds.Count) {
			return;
		}
		GameObject.DestroyImmediate(_unsaveds[_index]);
		_unsaveds.RemoveAt(_index);
		selected = null;
		_index = -1;
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
	}
	private bool RenameSelectedUnsaved(string text)
	{
		if (!_unsaved_selected) {
			return false;
		}
		if (string.IsNullOrWhiteSpace(text)) {
			return false;
		}
		if (selected == null) {
			return false;
		}
		char[] invalids = Path.GetInvalidFileNameChars();
		if (text.IndexOfAny(invalids) >= 0) {
			return false;
		}
		text = text.Trim();
		if (string.Equals(selected.objname.Trim(), text)) {
			return false;
		}
		Undo.RecordObject(selected, "Rename Unsaved Selected " + typeof(T).ToString());
		selected.objname = text;
		_repaint_menu = true;
		return true;
	}
	#endregion

	#region GUID Reorderable List
	private void DrawGUIDNone(Rect rect)
	{
		EditorGUI.LabelField(rect, "No " + typeof(T).ToString() + " Found.", GUI.skin.GetStyle("RightListLabel"));
	}

	private void DrawGUIDElement(Rect rect, int index, bool active, bool focused)
	{
		string name = _guidstrs[index];
		if (string.IsNullOrEmpty(name)) {
			name = "Invalid Name";
		}
		EditorGUI.LabelField(rect, name, GUI.skin.GetStyle("RightSelectableLabel"));
	}
	private void DrawGUIDElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_guids.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (!_unsaved_selected && _index == index && _index >= 0);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}
	private void SelectGUIDElement(ReorderableList list)
	{
		_index = list.index;
		if (_unsaved_selected) {
			selected = null;
		}
		_unsaved_selected = false;
		if (_index < 0 || _index >= _guids.Count) {
			selected = null;
			return;
		}
		string path = AssetDatabase.GUIDToAssetPath(_guids[_index]);
		if (string.IsNullOrEmpty(path)) {
			selected = null;
			return;
		}
		T loaded_obj = AssetDatabase.LoadAssetAtPath<T>(path);
		if (loaded_obj == null) {
			selected = null;
			return;
		}
		if (selected == loaded_obj) {
			return;
		}
		selected = loaded_obj;
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
	}

	#endregion

	#region Unsaved Reorderable List
	private void DrawUnsavedNone(Rect rect)
	{
		EditorGUI.LabelField(rect, "No Unsaved " + typeof(T).ToString() + ".", GUI.skin.GetStyle("RightListLabel"));
	}

	private void DrawUnsavedElement(Rect rect, int index, bool active, bool focus)
	{
		string name = _unsaveds[index].objname;
		if (string.IsNullOrEmpty(name)) {
			name = "Invalid Name";
		}
		EditorGUI.LabelField(rect, name, GUI.skin.GetStyle("RightSelectableLabel"));
	}

	private void DrawUnsavedElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_unsaveds.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (_unsaved_selected && _index == index && _index >= 0);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}

	private float UnsavedElementHeight(int index)
	{
		GUIStyle label = GUI.skin.GetStyle("RightSelectableLabel");
		if (label == null) {
			return 0;
		}
		return label.fixedHeight;
	}

	private void SelectUnsavedElement(ReorderableList list)
	{
		_index = list.index;
		_unsaved_selected = true;
		if (_index < 0 || _index >= _unsaveds.Count) {
			_update_mesh = true;
			_render_mesh = true;
			_repaint_menu = true;
			selected = null;
			return;
		}
		if (selected == _unsaveds[_index]) {
			return;
		}
		selected = _unsaveds[_index];
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
	}
	#endregion
}
