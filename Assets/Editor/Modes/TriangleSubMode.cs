using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class TriangleSubMode : SubModeScreen
{
	public List<int> selectlist;
	public HashSet<int> selectset;

	private Rect _rect_triscrollbackground;
	private Rect _rect_triscroll;
	private Rect _rect_tricontent;
	private Rect _rect_panel;
	private Rect _rect_delete;
	private Rect _rect_add;
	private Rect _rect_flip;
	private Vector2 _triscroll;
	private List<Triangle> _triangles;
	private HashSet<int> _vertexselect;
	private HashSet<int> _vertexsuper;
	private ReorderableList _trilist;
	private VertexGUI _vertexgui;

	public TriangleSubMode(string title, MeshPreview preview, Mesh vertex_mesh, Material vertex_material)
	{
		_map = null;
		_design = null;
		selectlist = new List<int>();
		selectset = new HashSet<int>();
		_vertexselect = new HashSet<int>();
		_vertexsuper = new HashSet<int>();

		_title = title;
		_preview = preview;
		_triangles = new List<Triangle>();

		_vertexgui = new VertexGUI(true, 3, 0.01f, false);
		_vertexgui.vertexmesh = vertex_mesh;
		_vertexgui.vertexmaterial = vertex_material;
	}

	public override void Enable(VoxelMapping map, VoxelDesign design)
	{
		_map = map;
		_design = design;
		_triscroll = Vector2.zero;
		_triangles.Clear();
		selectlist.Clear();
		selectset.Clear();
		_vertexselect.Clear();
		_vertexsuper.Clear();
		//initialize vertex build list
		_trilist = new ReorderableList(_triangles, typeof(Triangle), true, false, false, false);
		_trilist.showDefaultBackground = false;
		_trilist.headerHeight = 0;
		_trilist.footerHeight = 0;
		_trilist.elementHeightCallback += GetTriangleHeight;
		_trilist.drawNoneElementCallback += DrawTriangleNoneElement;
		_trilist.drawElementBackgroundCallback += DrawTriangleElementBackground;
		_trilist.drawElementCallback += DrawTriangleElement;
		_trilist.onReorderCallbackWithDetails += ReorderTriangleList;
		_trilist.onSelectCallback += SelectTriangle;

		RefreshTriangleList();
		Undo.undoRedoPerformed += UndoRedoPerformed;

		_vertexgui.Enable();
		_vertexgui.map = map;
		_vertexgui.design = design;
		_preview.AddPreviewGUI(_vertexgui);
		_preview.UpdateMeshComponents(design, null, null);
	}

	public override void Disable()
	{
		_preview.RemovePreviewGUI(_vertexgui);
		_vertexgui.Disable();
		_preview.ClearMesh();

		Undo.undoRedoPerformed -= UndoRedoPerformed;
		_map = null;
		_design = null;
		_triscroll = Vector2.zero;
		_triangles.Clear();
		selectlist.Clear();
		selectset.Clear();
		_vertexselect.Clear();
		_vertexsuper.Clear();
	}

	public override void DrawGUI(Rect rect)
	{
		//update layout rects
		UpdateLayoutRects(rect);
		//draw list
		_triscroll = GUI.BeginScrollView(_rect_triscroll, _triscroll, _rect_tricontent);
		_trilist.DoList(_rect_tricontent);
		GUI.EndScrollView();
		//draw panel
		EVx.DrawRect(_rect_panel, "LightBlack");
		//draw delete button
		EditorGUI.BeginDisabledGroup(selectset.Count <= 0);
		if (GUI.Button(_rect_delete, "Delete", "WhiteField10")) {
			if (DeleteSelectedTriangles()) {
				RefreshTriangleList();
				UpdateTriangleVertexSelection();
				_vertexgui.UpdatePrimarySelection(_vertexselect, _vertexsuper);
				_preview.UpdateMeshComponents(_design, _vertexselect, _vertexsuper);
			}
		}
		if (GUI.Button(_rect_flip, "Flip", "WhiteField10")) {
			if (FlipSelectedTriangles()) {
				RefreshTriangleList();
				UpdateTriangleVertexSelection();
				_vertexgui.UpdatePrimarySelection(_vertexselect, _vertexsuper);
				_preview.UpdateMeshComponents(_design, _vertexselect, _vertexsuper);
			}
		}
		EditorGUI.EndDisabledGroup();
		//draw add button
		if (GUI.Button(_rect_add, "Add", "WhiteField10")) {
			if (AddTriangleElement()) {
				RefreshTriangleList();
				UpdateTriangleVertexSelection();
				_vertexgui.UpdatePrimarySelection(_vertexselect, _vertexsuper);
				_preview.UpdateMeshComponents(_design, _vertexselect, _vertexsuper);
			}
		}
	}

	private void UpdateLayoutRects(Rect rect)
	{
		_rect_triscrollbackground = EVx.GetSandwichedRectY(rect, 0, EVx.LRG_LINE + 4);
		_rect_triscroll = EVx.GetPaddedRect(_rect_triscrollbackground, 1);
		_rect_tricontent = EVx.GetScrollViewRect(_trilist, _rect_triscroll.width, _rect_triscroll.height);
		_rect_panel = EVx.GetBelowElement(rect, 0, EVx.LRG_LINE);
		float button_width = Mathf.Min(80, _rect_panel.width / 3f);
		_rect_delete = EVx.GetPaddedRect(EVx.GetLeftElement(_rect_panel, 0, button_width), 4);
		_rect_add = EVx.GetPaddedRect(EVx.GetRightElement(_rect_panel, 0, button_width), 4);
		_rect_flip = EVx.GetPaddedRect(EVx.GetRightElement(_rect_panel, 1, button_width), 4);
	}

	private void RefreshTriangleList()
	{
		_triangles.Clear();
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return;
		}
		List<Triangle> triangles = _design.triangles;
		for (int k = 0; k < triangles.Count; k++) {
			_triangles.Add(triangles[k]);
		}
	}

	private void UpdateTriangleVertexSelection()
	{
		_vertexselect.Clear();
		_vertexsuper.Clear();
		if (_design == null || !_design.isValid) {
			return;
		} 
		List<Point> points = _design.vertices;
		List<Triangle> triangles = _design.triangles;
		for (int k = 0; k < selectlist.Count;k++) {
			int tri_index = selectlist[k];
			if (tri_index < 0 || tri_index >= triangles.Count) {
				continue;
			}
			Triangle tri = triangles[tri_index];
			if (!tri.IsValid(points.Count)) {
				continue;
			}
			if (!selectset.Contains(tri_index)) {
				continue;
			}
			bool is_super = (k == selectlist.Count - 1);
			AddVertexIndex(tri.vertex0, is_super);
			AddVertexIndex(tri.vertex1, is_super);
			AddVertexIndex(tri.vertex2, is_super);
		}
	}

	private void AddVertexIndex(int vertex_index, bool is_super)
	{
		_vertexselect.Add(vertex_index);
		if (is_super) {
			_vertexsuper.Add(vertex_index);
		}
	}

	private void UndoRedoPerformed()
	{
		RefreshTriangleList();
		UpdateTriangleVertexSelection();
		_vertexgui.UpdatePrimarySelection(_vertexselect, _vertexsuper);
		_preview.UpdateMeshComponents(_design, _vertexselect, _vertexsuper);
	}

	#region Triangle ReorderableList
	private float GetTriangleHeight(int index)
	{
		if (_triangles.Count > 0) {
			return 2 * (EVx.MED_PAD + EVx.MED_LINE);
		}
		else {
			return (2 * EVx.MED_PAD) + EVx.MED_LINE;
		}
	}
	private void DrawTriangleNoneElement(Rect rect)
	{
		rect = EVx.GetPaddedRect(rect, EVx.MED_PAD, 0);
		GUI.Label(rect, "No Triangles.", "LeftListElement");
	}
	private void DrawTriangleElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_triangles.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool selected = (_triangles.Count > 0) && selectset.Contains(index);
		if (selected) {
			bool superselected = selectlist[selectlist.Count - 1] == index;
			EVx.DrawRect(rect, "SelectedListBackground", hover, active, superselected, false);
		}
		else {
			EVx.DrawRect(rect, "SelectableListBackground", hover, active, false, false);
		}
	}
	private void DrawTriangleElement(Rect rect, int index, bool active, bool focus)
	{
		rect = EVx.GetPaddedRect(rect, EVx.MED_PAD, 0);
		Triangle triangle = _triangles[index];
		Rect row_rect = EVx.GetAboveElement(rect, 0, EVx.MED_LINE);
		GUI.Label(row_rect, "Triangle " + index, "LeftListElement");

		EditorGUI.BeginChangeCheck();
		row_rect = EVx.GetAboveElement(rect, 1, EVx.MED_LINE);
		float toggle_width = row_rect.width / 3f;
		Rect rect_field = EVx.GetPaddedRect(EVx.GetLeftElement(row_rect, 0, toggle_width), 1);
		int vertex0 = EditorGUI.IntField(rect_field, triangle.vertex0, "RightDarkField10");
		rect_field = EVx.GetPaddedRect(EVx.GetLeftElement(row_rect, 1, toggle_width), 1);
		int vertex1 = EditorGUI.IntField(rect_field, triangle.vertex1, "RightDarkField10");
		rect_field = EVx.GetPaddedRect(EVx.GetLeftElement(row_rect, 2, toggle_width), 1);
		int vertex2 = EditorGUI.IntField(rect_field, triangle.vertex2, "RightDarkField10");
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(_map, "Updated Triangle");
			_design.UpdateTriangle(index, new Triangle(vertex0, vertex1, vertex2));
			EditorUtility.SetDirty(_map);
			RefreshTriangleList();
			UpdateTriangleVertexSelection();
			_vertexgui.UpdatePrimarySelection(_vertexselect, _vertexsuper);
			_preview.UpdateMeshComponents(_design, _vertexselect, _vertexsuper);
		}
	}
	private void ReorderTriangleList(ReorderableList list, int old_index, int new_index)
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return;
		}
		if (old_index < 0 || new_index < 0 || old_index == new_index) {
			return;
		}
		List<Triangle> triangles = _design.triangles;
		if (old_index >= triangles.Count || new_index >= triangles.Count) {
			return;
		}
		Undo.RecordObject(_map, "Reorder Triangles");
		Triangle old_tri = triangles[old_index];
		triangles.RemoveAt(old_index);
		triangles.Insert(new_index, old_tri);
		//maintain selection
		ReorderMaintainSelection(old_index, new_index, selectlist, selectset);
		//dirty target object
		EditorUtility.SetDirty(_map);
		RefreshTriangleList();
		UpdateTriangleVertexSelection();
		_vertexgui.UpdatePrimarySelection(_vertexselect, _vertexsuper);
		_preview.UpdateMeshComponents(_design, _vertexselect, _vertexsuper);
	}
	private void SelectTriangle(ReorderableList list)
	{
		int index = list.index;
		if (index < 0 || index >= _triangles.Count) {
			selectset.Clear();
			selectlist.Clear();
			return;
		}
		SelectListElement(index, selectlist, selectset);
		UpdateTriangleVertexSelection();
		_vertexgui.UpdatePrimarySelection(_vertexselect, _vertexsuper);
		_preview.UpdateMeshComponents(_design, _vertexselect, _vertexsuper);
	}

	private bool AddTriangleElement()
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return false;
		}
		Undo.RecordObject(_map, "Add New Triangle");
		_design.triangles.Add(new Triangle(-1, -1, -1));
		//dirty target object
		EditorUtility.SetDirty(_map);
		return true;
	}
	private bool DeleteSelectedTriangles()
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return false;
		}
		List<Triangle> triangles = _design.triangles;
		if (triangles.Count <= 0 || selectlist.Count <= 0) {
			return false;
		}
		Undo.RecordObject(_map, "Delete Selected Triangles");
		int index = 0;
		int deleted = 0;
		while (index < triangles.Count) {
			if (selectset.Contains(index + deleted)) {
				//delete triangle
				triangles.RemoveAt(index);
				deleted += 1;
			}
			else {
				index += 1;
			}
		}
		selectset.Clear();
		selectlist.Clear();
		//dirty target object
		EditorUtility.SetDirty(_map);
		return true;
	}
	private bool FlipSelectedTriangles()
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return false;
		}
		List<Triangle> triangles = _design.triangles;
		if (triangles.Count <= 0 || selectlist.Count <= 0) {
			return false;
		}
		Undo.RecordObject(_map, "Flip Selected Triangles");
		for (int k = 0; k < selectlist.Count; k++) {
			int index = selectlist[k];
			if (index < 0 || index >= triangles.Count) {
				continue;
			}
			triangles[index] = Triangle.Flip(triangles[index]);
		}
		//dirty target object
		EditorUtility.SetDirty(_map);
		return true;
	}
	#endregion
}
