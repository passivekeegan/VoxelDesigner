using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class VertexSubMode : SubModeScreen
{
	public List<int> selectlist;
	public HashSet<int> selectset;

	private Rect _rect_vertexscrollbackground;
	private Rect _rect_vertexscroll;
	private Rect _rect_vertexcontent;
	private Rect _rect_panel;
	private Rect _rect_delete;
	private Rect _rect_add;
	private Vector2 _vertexscroll;
	private List<Point> _vertices;
	private ReorderableList _vertexlist;
	private VertexGUI _vertexgui;


	public VertexSubMode(string title, MeshPreview preview, Mesh vertex_mesh, Material vertex_material)
	{
		_map = null;
		_design = null;
		selectlist = new List<int>();
		selectset = new HashSet<int>();

		_title = title;
		_preview = preview;
		_vertices = new List<Point>();

		_vertexgui = new VertexGUI(false, 1, 0.01f, true);
		_vertexgui.vertexmesh = vertex_mesh;
		_vertexgui.vertexmaterial = vertex_material;
	}

	public override void Enable(VoxelMapping map, VoxelDesign design)
	{
		_map = map;
		_design = design;
		_vertexscroll = Vector2.zero;
		_vertices.Clear();
		selectlist.Clear();
		selectset.Clear();
		//initialize vertex build list
		_vertexlist = new ReorderableList(_vertices, typeof(Point), true, false, false, false);
		_vertexlist.showDefaultBackground = false;
		_vertexlist.headerHeight = 0;
		_vertexlist.footerHeight = 0;
		_vertexlist.elementHeightCallback += GetVertexHeight;
		_vertexlist.drawNoneElementCallback += DrawVertexNoneElement;
		_vertexlist.drawElementBackgroundCallback += DrawVertexElementBackground;
		_vertexlist.drawElementCallback += DrawVertexElement;
		_vertexlist.onReorderCallbackWithDetails += ReorderVertexList;
		_vertexlist.onSelectCallback += SelectVertex;

		RefreshVertexList();

		_vertexgui.Enable();
		_vertexgui.map = map;
		_vertexgui.design = design;
		_preview.AddPreviewGUI(_vertexgui);
		_preview.UpdateMeshComponents(design, null, null);

		Undo.undoRedoPerformed += UndoRedoPerformed;
	}
	public override void Disable()
	{
		_preview.RemovePreviewGUI(_vertexgui);
		_vertexgui.Disable();
		_preview.ClearMesh();

		Undo.undoRedoPerformed -= UndoRedoPerformed;
		_map = null;
		_design = null;
		_vertexscroll = Vector2.zero;
		_vertices.Clear();
		selectlist.Clear();
		selectset.Clear();
	}

	public override void DrawGUI(Rect rect)
	{
		if (_vertexgui.refresh_vertexlist) {
			RefreshVertexList();
			_vertexgui.refresh_vertexlist = false;
		}
		//update layout rects
		UpdateLayoutRects(rect);
		//draw list
		_vertexscroll = GUI.BeginScrollView(_rect_vertexscroll, _vertexscroll, _rect_vertexcontent);
		_vertexlist.DoList(_rect_vertexcontent);
		GUI.EndScrollView();
		//draw panel
		EVx.DrawRect(_rect_panel, "LightBlack");
		//draw delete button
		EditorGUI.BeginDisabledGroup(selectset.Count <= 0);
		if (GUI.Button(_rect_delete, "Delete", "WhiteField10")) {
			if (DeleteSelectedVertex()) {
				RefreshVertexList();
				_vertexgui.UpdatePrimarySelection(selectlist);
			}
		}
		EditorGUI.EndDisabledGroup();
		//draw add button
		if (GUI.Button(_rect_add, "Add", "WhiteField10")) {
			if (AddVertexElement()) {
				RefreshVertexList();
				_vertexgui.UpdatePrimarySelection(selectlist);
			}
		}
	}

	private void UpdateLayoutRects(Rect rect)
	{
		_rect_vertexscrollbackground = EVx.GetSandwichedRectY(rect, 0, EVx.LRG_LINE + 4);
		_rect_vertexscroll = EVx.GetPaddedRect(_rect_vertexscrollbackground, 1);
		_rect_vertexcontent = EVx.GetScrollViewRect(_vertexlist, _rect_vertexscroll.width, _rect_vertexscroll.height);
		_rect_panel = EVx.GetBelowElement(rect, 0, EVx.LRG_LINE);
		float button_width = Mathf.Min(80, _rect_panel.width / 2f);
		_rect_delete = EVx.GetPaddedRect(EVx.GetLeftElement(_rect_panel, 0, button_width), 4);
		_rect_add = EVx.GetPaddedRect(EVx.GetRightElement(_rect_panel, 0, button_width), 4);
	}

	private void RefreshVertexList()
	{
		_vertices.Clear();
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return;
		}
		List<Point> vertices = _design.vertices;
		for (int k = 0;k < vertices.Count;k++) {
			_vertices.Add(vertices[k]);
		}
	}

	private void UndoRedoPerformed()
	{
		RefreshVertexList();
	}

	#region Vertex ReorderableList
	private float GetVertexHeight(int index)
	{
		if (_vertices.Count > 0) {
			return (2 * EVx.MED_PAD) + (2 * (EVx.MED_LINE + EVx.SM_SPACE)) + EVx.SM_LINE + EVx.SM_SPACE;
		}
		else {
			return (2 * EVx.MED_PAD) + EVx.MED_LINE;
		}
	}

	private void DrawVertexNoneElement(Rect rect)
	{
		rect = EVx.GetPaddedRect(rect, EVx.MED_PAD, 0);
		GUI.Label(rect, "No Vertices.", "LeftListElement");
	}
	private void DrawVertexElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_vertices.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool selected = (_vertices.Count > 0) && selectset.Contains(index);
		if (selected) {
			bool superselected = selectlist[selectlist.Count - 1] == index;
			EVx.DrawRect(rect, "SelectedListBackground", hover, active, superselected, false);
		}
		else {
			EVx.DrawRect(rect, "SelectableListBackground", hover, active, false, false);
		}
	}
	private void DrawVertexElement(Rect rect, int index, bool active, bool focus)
	{
		rect = EVx.GetPaddedRect(rect, EVx.MED_PAD, 0);
		Point point = _vertices[index];
		Rect row_rect = EVx.GetAboveElement(rect, 0, EVx.MED_LINE);
		if (point.shared) {
			float shared_width = Mathf.Min(0.5f * row_rect.width, 60f);
			GUI.Label(EVx.GetLeftElement(row_rect, 0, row_rect.width - shared_width), "Vertex " + index, "LeftListElement");
			GUI.Label(EVx.GetRightElement(row_rect, 0, shared_width), "Shared", "RightListElement");
		}
		else {
			GUI.Label(row_rect, "Vertex " + index, "LeftListElement");
		}
		EditorGUI.BeginChangeCheck();
		row_rect = EVx.GetAboveElement(rect, 1, EVx.MED_LINE, EVx.SM_SPACE);
		Vector3 vertex = EVx.DrawVector3Field(row_rect, point.vertex, "VectorLabel", "VectorDarkField10");
		row_rect = EVx.GetAboveElement(rect, 0, EVx.SM_LINE, 0, 2 * (EVx.MED_LINE + EVx.SM_SPACE));
		bool shared = EditorGUI.Foldout(row_rect, point.shared, "shared", true, "DarkToggle");
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(_map, "Updated Vertex");
			_design.UpdateVertex(index, new Point(shared, vertex));
			EditorUtility.SetDirty(_map);
			RefreshVertexList();
			_vertexgui.UpdatePrimarySelection(selectlist);
		}
	}
	private void ReorderVertexList(ReorderableList list, int old_index, int new_index)
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return;
		}
		if (old_index < 0 || new_index < 0 || old_index == new_index) {
			return;
		}
		List<Point> points = _design.vertices;
		if (old_index >= points.Count || new_index >= points.Count) {
			return;
		}
		Undo.RecordObject(_map, "Reorder Vertex List");
		Point point = points[old_index];
		points.RemoveAt(old_index);
		points.Insert(new_index, point);
		//maintain selection
		ReorderMaintainSelection(old_index, new_index, selectlist, selectset);
		//maintain triangles
		ReorderMaintainTriangles(old_index, new_index);
		//dirty target object
		EditorUtility.SetDirty(_map);
		RefreshVertexList();
		_vertexgui.UpdatePrimarySelection(selectlist);
	}
	private void SelectVertex(ReorderableList list)
	{
		int index = list.index;
		if (index < 0 || index >= _vertices.Count) {
			selectset.Clear();
			selectlist.Clear();
			return;
		}
		SelectListElement(index, selectlist, selectset);
		_vertexgui.UpdatePrimarySelection(selectlist);
	}

	private void ReorderMaintainTriangles(int old_index, int new_index)
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return;
		}
		List<Triangle> triangles = _design.triangles;
		for (int k = 0; k < triangles.Count; k++) {
			bool changed = false;
			Triangle tri = triangles[k];
			int vertex0 = ReinsertTriangleVertex(tri.vertex0, old_index, new_index);
			changed = changed || (vertex0 != tri.vertex0);

			int vertex1 = ReinsertTriangleVertex(tri.vertex1, old_index, new_index);
			changed = changed || (vertex1 != tri.vertex1);

			int vertex2 = ReinsertTriangleVertex(tri.vertex2, old_index, new_index);
			changed = changed || (vertex2 != tri.vertex2);
			if (changed) {
				triangles[k] = new Triangle(vertex0, vertex1, vertex2);
			}
		}
	}
	private int ReinsertTriangleVertex(int index, int old_index, int new_index)
	{
		if (index < old_index) {
			if (index >= new_index) {
				return index + 1;
			}
		}
		else if (index > old_index) {
			if (index <= new_index) {
				return index - 1;
			}
		}
		else {
			if (index != new_index) {
				return new_index;
			}
		}
		return index;
	}
	private bool AddVertexElement()
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return false;
		}
		Undo.RecordObject(_map, "Add New Vertex");
		_design.vertices.Add(new Point());
		//dirty target object
		EditorUtility.SetDirty(_map);
		return true;
	}
	private bool DeleteSelectedVertex()
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return false;
		}
		List<Point> vertices = _design.vertices;
		if (vertices.Count <= 0 || selectlist.Count <= 0) {
			return false;
		}
		Undo.RecordObject(_map, "Delete Selected Vertex");
		List<int> deletions = new List<int>(vertices.Count);
		int index = 0;
		int deleted = 0;
		while (index < vertices.Count) {
			deletions.Add(deleted);
			if (selectset.Contains(index + deleted)) {
				//delete vertex
				vertices.RemoveAt(index);
				deleted += 1;
			}
			else {
				index += 1;
			}
		}
		//maintain triangles
		DeleteMaintainTriangles(deletions);
		//clear selection
		selectset.Clear();
		selectlist.Clear();
		//dirty target object
		EditorUtility.SetDirty(_map);
		return true;
	}
	private void DeleteMaintainTriangles(List<int> deletions)
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return;
		}
		List<Triangle> triangles = _design.triangles;
		for (int k = 0; k < triangles.Count; k++) {
			bool changed = false;
			Triangle tri = triangles[k];
			int vertex0 = tri.vertex0;
			if (selectset.Contains(vertex0)) {
				//deleted
				vertex0 = -1;
				changed = true;
			}
			else if (vertex0 >= 0 && vertex0 < deletions.Count) {
				vertex0 -= deletions[vertex0];
				changed = true;
			}
			int vertex1 = tri.vertex1;
			if (selectset.Contains(vertex1)) {
				//deleted
				vertex1 = -1;
				changed = true;
			}
			else if (vertex1 >= 0 && vertex1 < deletions.Count) {
				vertex1 -= deletions[vertex1];
				changed = true;
			}
			int vertex2 = tri.vertex2;
			if (selectset.Contains(vertex2)) {
				//deleted
				vertex2 = -1;
				changed = true;
			}
			else if (vertex2 >= 0 && vertex2 < deletions.Count) {
				vertex2 -= deletions[vertex2];
				changed = true;
			}
			if (changed) {
				triangles[k] = new Triangle(vertex0, vertex1, vertex2);
			}
		}
	}
	#endregion
}
