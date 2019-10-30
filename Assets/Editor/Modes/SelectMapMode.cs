using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class SelectMapMode : ModeScreen
{
	private string _mapname;
	private string _mapguid;

	private Rect _rect_guidscrollbackground;
	private Rect _rect_guidscroll;
	private Rect _rect_guidcontent;
	private Rect _rect_panel;
	private Rect _rect_delete;
	private Rect _rect_new;
	private Rect _rect_rename;
	private Vector2 _guidscroll;
	private ChunkMeshData _meshdata;
	private List<string> _guids;
	private List<string> _guidstrs;
	private ReorderableList _guidlist;

	public SelectMapMode(string title, MeshPreview preview)
	{
		map = null;

		_title = title;
		_preview = preview;
		_guids = new List<string>();
		_guidstrs = new List<string>();
		_meshdata = new ChunkMeshData();
	}

	public override void Enable()
	{
		_guidscroll = Vector2.zero;
		_guids.Clear();
		_guidstrs.Clear();

		_guidlist = new ReorderableList(_guids, typeof(string), false, false, false, false);
		_guidlist.showDefaultBackground = false;
		_guidlist.headerHeight = 0f;
		_guidlist.footerHeight = 0f;
		_guidlist.elementHeight = (2 * EVx.MED_PAD) + EVx.MED_LINE;
		_guidlist.drawElementCallback += DrawGUIDElement;
		_guidlist.drawElementBackgroundCallback += DrawGUIDElementBackground;
		_guidlist.onSelectCallback += SelectGUIDElement;
		_guidlist.drawNoneElementCallback += DrawGUIDNone;

		RefreshAssets();
		_preview.DirtyRender();
	}

	public override void Disable()
	{
		_guidscroll = Vector2.zero;
		_meshdata.Clear();
		_guids.Clear();
		_guidstrs.Clear();
		_guidlist = null;
	}

	public override void DrawGUI(Rect rect)
	{
		UpdateLayoutRect(rect);

		//rename field
		EditorGUI.BeginDisabledGroup(map == null || _mapname == null || _mapguid == null);
		EditorGUI.BeginChangeCheck();
		string name = EditorGUI.DelayedTextField(_rect_rename, "", _mapname, "RightDarkField10");
		if (EditorGUI.EndChangeCheck()) {
			if (RenameSelectedAsset(name)) {
				RefreshAssets();
			}	
		}
		EditorGUI.EndDisabledGroup();

		//map list
		EVx.DrawRect(_rect_guidscrollbackground, "LightBlackBorder");
		_guidscroll = GUI.BeginScrollView(_rect_guidscroll, _guidscroll, _rect_guidcontent);
		_guidlist.DoList(_rect_guidcontent);
		GUI.EndScrollView();

		//bottom panel
		EVx.DrawRect(_rect_panel, "LightBlack");
		//new voxel mapping
		if (GUI.Button(_rect_new, "New", "WhiteField10")) {
			if (CreateNewAsset()) {
				RefreshAssets();
				_preview.DirtyRender();
			}
		}
		//delete voxel mapping
		int index = _guidlist.index;
		EditorGUI.BeginDisabledGroup(index < 0 || index >= _guids.Count);
		if (GUI.Button(_rect_delete, "Delete", "WhiteField10")) {
			if (DeleteAsset()) {
				RefreshAssets();
				_preview.DirtyRender();
			}
		}
		EditorGUI.EndDisabledGroup();
	}

	private void UpdateLayoutRect(Rect rect)
	{
		_rect_rename = EVx.GetAboveElement(rect, 0, EVx.MED_LINE);
		_rect_guidscrollbackground = EVx.GetSandwichedRectY(rect, EVx.MED_LINE + 4, EVx.LRG_LINE + 4);
		_rect_guidscroll = EVx.GetPaddedRect(_rect_guidscrollbackground , 1);
		_rect_guidcontent = EVx.GetScrollViewRect(_guidlist, _rect_guidscroll.width, _rect_guidscroll.height);
		_rect_panel = EVx.GetBelowElement(rect, 0, EVx.LRG_LINE);
		float width = Mathf.Min(80, _rect_panel.width / 2f);
		_rect_delete = EVx.GetPaddedRect(EVx.GetLeftElement(_rect_panel, 0, width), 4);
		_rect_new = EVx.GetPaddedRect(EVx.GetRightElement(_rect_panel, 0, width), 4);

	}

	private void UpdateMesh()
	{
		_meshdata.Clear();
		if (map == null) {
			return;
		}
	}

	private void GenerateRandomVoxel()
	{

	}

	#region GUID Asset Methods
	private void RefreshAssets()
	{
		_guids.Clear();
		_guidstrs.Clear();
		int selected_index = -1;
		string select_guid = _mapguid;
		if (string.IsNullOrEmpty(select_guid) && map != null) {
			select_guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(map));
		}
		string[] guids = AssetDatabase.FindAssets("t:VoxelMapping");
		for (int k = 0; k < guids.Length; k++) {
			string name = AssetDatabase.GUIDToAssetPath(guids[k]);
			if (name == null || name.Length <= 0) {
				continue;
			}
			name = System.IO.Path.GetFileNameWithoutExtension(name);
			if (name == null || name.Length <= 0) {
				continue;
			}
			if (string.Equals(guids[k], select_guid)) {
				selected_index = _guids.Count;
			}
			_guids.Add(guids[k]);
			_guidstrs.Add(name);
		}
		_guidlist.index = selected_index;
		if (selected_index < 0) {
			map = null;
			_mapguid = null;
			_mapname = null;
		}
		else {
			_mapguid = guids[selected_index];
			_mapname = _guidstrs[selected_index];
		}
	}

	private bool CreateNewAsset()
	{
		string path = EditorUtility.SaveFilePanel("Save VoxelMapping", Application.dataPath, "voxelmapping.asset", "asset");
		if (string.IsNullOrEmpty(path)) {
			return false;
		}
		//get relative path
		path = "Assets" + path.Replace(Application.dataPath, "");
		path = AssetDatabase.GenerateUniqueAssetPath(path);
		if (string.IsNullOrEmpty(path)) {
			return false;
		}
		VoxelMapping asset = ScriptableObject.CreateInstance<VoxelMapping>();
		asset.Initialize();

		AssetDatabase.CreateAsset(asset, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		map = asset;
		_mapguid = AssetDatabase.AssetPathToGUID(path);
		_mapname = System.IO.Path.GetFileNameWithoutExtension(path);
		return true;
	}


	private bool DeleteAsset()
	{
		if (map == null || _mapguid == null || _mapname == null) {
			return false;
		}
		string path = AssetDatabase.GetAssetPath(map);
		if (string.IsNullOrEmpty(path)) {
			return false;
		}
		AssetDatabase.DeleteAsset(path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		map = null;
		_mapguid = null;
		_mapname = null;
		return true;
	}

	private bool RenameSelectedAsset(string text)
	{
		if (string.IsNullOrWhiteSpace(text)) {
			return false;
		}
		if (map == null) {
			return false;
		}
		char[] invalids = System.IO.Path.GetInvalidFileNameChars();
		if (text.IndexOfAny(invalids) >= 0) {
			return false;
		}
		text = text.Trim();
		string path = AssetDatabase.GetAssetPath(map);
		string file_name = System.IO.Path.GetFileNameWithoutExtension(path);
		if (string.Equals(file_name.Trim(), text)) {
			return false;
		}
		string returned_path = AssetDatabase.RenameAsset(path, text);
		if (returned_path.Length > 0) {
			return false;
		}
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		_mapname = file_name;
		return true;
	}
	#endregion

	#region GUID Reorderable List
	private void DrawGUIDNone(Rect rect)
	{
		rect = EVx.GetPaddedRect(rect, EVx.MED_PAD, 0);
		GUI.Label(rect, "No VoxelMapping Found.", "ListElement");
	}
	private void DrawGUIDElement(Rect rect, int index, bool active, bool focused)
	{
		rect = EVx.GetPaddedRect(rect, EVx.MED_PAD, 0);
		GUI.Label(rect, _guidstrs[index], "LeftListElement");
	}
	private void DrawGUIDElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_guids.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool selected = (_guids.Count > 0 && _guidlist.index == index);
		if (selected) {
			EVx.DrawRect(rect, "SelectedListBackground", hover, active, true, false);
		}
		else {
			EVx.DrawRect(rect, "SelectableListBackground", hover, active, false, false);
		}
	}
	private void SelectGUIDElement(ReorderableList list)
	{
		//load voxel mapping asset at index
		int index = list.index;
		if (index < 0 || index >= _guids.Count) {
			map = null;
			return;
		}
		string path = AssetDatabase.GUIDToAssetPath(_guids[index]);
		if (string.IsNullOrEmpty(path)) {
			map = null;
			return;
		}
		VoxelMapping loaded_map = AssetDatabase.LoadAssetAtPath<VoxelMapping>(path);
		if (loaded_map == null) {
			map = null;
			return;
		}
		if (map == loaded_map) {
			return;
		}
		map = loaded_map;
		_mapguid = _guids[index];
		_mapname = System.IO.Path.GetFileNameWithoutExtension(path);
		_preview.DirtyRender();
	}
	#endregion
}

