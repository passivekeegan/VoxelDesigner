using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class VertexPanel : PanelGUI
{
	public VoxelComponent target;
	public List<int> selectlist;
	public HashSet<int> selectset;
	

	private Rect _rect_vertex_title;
	private Rect _rect_vertex_scrollview;
	private Rect _rect_vertex_content;
	private Rect _rect_vertex_panel;
	private Rect _rect_import;

	private Quaternion _importrot;
	private Vector2 _vertex_scroll;
	private Mesh _importmesh;
	private List<Vector3> _vertices;
	private ReorderableList _vertexlist;

	public VertexPanel(string title, Quaternion importrot)
	{
		_title = title;
		_importrot = importrot;
		selectlist = new List<int>();
		selectset = new HashSet<int>();
		_vertices = new List<Vector3>();
	}

	public override void Enable()
	{
		_update_mesh = true;
		_repaint_menu = true;
		_render_mesh = true;
		_vertex_scroll = Vector2.zero;
		_importmesh = null;
		selectlist.Clear();
		selectset.Clear();
		//initialize vertex build list
		_vertexlist = new ReorderableList(_vertices, typeof(Vector3), true, false, false, false);
		_vertexlist.showDefaultBackground = false;
		_vertexlist.headerHeight = 0;
		_vertexlist.footerHeight = 0;
		_vertexlist.elementHeight = VxlGUI.MED_BAR;
		_vertexlist.drawNoneElementCallback += DrawVertexNoneElement;
		_vertexlist.drawElementBackgroundCallback += DrawVertexElementBackground;
		_vertexlist.drawElementCallback += DrawVertexElement;
		_vertexlist.onReorderCallbackWithDetails += ReorderVertexList;
		_vertexlist.onSelectCallback += SelectVertex;
	}

	public override void Disable()
	{
		target = null;

		_importmesh = null;
		_vertexlist = null;
		_vertices.Clear();
		selectlist.Clear();
		selectset.Clear();
		_update_mesh = false;
		_repaint_menu = false;
		_render_mesh = false;
	}

	public override void DrawGUI(Rect rect)
	{
		//update lists
		UpdateVertexList();
		//update important layout elements
		UpdateRects(rect);
		//disable panel check
		EditorGUI.BeginDisabledGroup(target == null);
		//draw vertex build title
		VxlGUI.DrawRect(_rect_vertex_title, "DarkGradient");
		GUI.Label(_rect_vertex_title, "Vertex Builds", GUI.skin.GetStyle("LeftLightHeader"));
		//draw vertex build list
		VxlGUI.DrawRect(_rect_vertex_scrollview, "DarkWhite");
		_vertex_scroll = GUI.BeginScrollView(_rect_vertex_scrollview, _vertex_scroll, _rect_vertex_content);
		_vertexlist.DoList(_rect_vertex_content);
		GUI.EndScrollView();
		//draw vertex build button panel
		float button_width = Mathf.Min(60f, _rect_vertex_panel.width / 2f);
		VxlGUI.DrawRect(_rect_vertex_panel, "DarkGradient");
		//draw vertex add button
		if (GUI.Button(VxlGUI.GetRightElement(_rect_vertex_panel, 0, button_width), "Add", GUI.skin.GetStyle("LightButton"))) {
			AddVertexElement();
		}
		//disable operation check
		EditorGUI.BeginDisabledGroup((_vertices.Count <= 0) || (selectlist.Count <= 0));
		//draw vertex delete button
		if (GUI.Button(VxlGUI.GetLeftElement(_rect_vertex_panel, 0, button_width), "Delete", GUI.skin.GetStyle("LightButton"))) {
			DeleteSelectedVertex();
		}
		EditorGUI.EndDisabledGroup();

		//draw import panel
		VxlGUI.DrawRect(_rect_import, "DarkWhite");
		Rect importpad_rect = VxlGUI.GetPaddedRect(_rect_import, VxlGUI.MED_PAD);
		Rect importrow_rect = VxlGUI.GetAboveElement(importpad_rect, 0, VxlGUI.MED_BAR);
		GUI.Label(importrow_rect, "Import Obj Vertex and Triangles", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		importrow_rect = VxlGUI.GetAboveElement(importpad_rect, 1, VxlGUI.MED_BAR);

		float import_width = Mathf.Min(100f, (importrow_rect.width - VxlGUI.SM_SPACE) / 2f);
		_importmesh = (Mesh)EditorGUI.ObjectField(
			VxlGUI.GetSandwichedRectX(importrow_rect, 0, import_width + VxlGUI.SM_SPACE),
			_importmesh,
			typeof(Mesh),
			false
		);
		EditorGUI.BeginDisabledGroup(_importmesh == null);
		if (GUI.Button(VxlGUI.GetRightElement(importrow_rect, 0, import_width), "Import", GUI.skin.GetStyle("DarkButton"))) {
			ImportVertexAndTriangle();
		}
		EditorGUI.EndDisabledGroup();
		EditorGUI.EndDisabledGroup();
	}

	private void ImportVertexAndTriangle()
	{
		if (target == null || _importmesh == null) {
			return;
		}
		Vector3[] mesh_vertices = _importmesh.vertices;
		int[] mesh_triangles = _importmesh.triangles;
		if (mesh_vertices == null || mesh_triangles == null || mesh_vertices.Length <= 0) {
			return;
		}
		List<Vector3> vertices = target.vertices;
		List<Triangle> triangles = target.triangles;
		if (vertices == null || triangles == null) {
			return;
		}
		//maximum mesh vertex count was exceeded
		if (mesh_vertices.Length + vertices.Count > ushort.MaxValue + 1) {
			return;
		}
		Undo.RecordObject(target, "Import Vertices and Triangles");
		//add mesh vertices
		int vertex_index = vertices.Count;
		for (int k = 0; k < mesh_vertices.Length; k++) {
			vertices.Add(_importrot * mesh_vertices[k]);
		}
		//add mesh triangles
		int tri_count = mesh_triangles.Length / 3;
		for (int k = 0; k < tri_count; k++) {
			int index = 3 * k;
			triangles.Add(new Triangle(
				(ushort)(vertex_index + mesh_triangles[index]),
				(ushort)(vertex_index + mesh_triangles[index + 1]),
				(ushort)(vertex_index + mesh_triangles[index + 2])
			));
		}
		_update_mesh = true;
		_repaint_menu = true;
		_render_mesh = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}

	private void ReorderMaintainTriangles(int old_index, int new_index)
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		List<Triangle> triangles = target.triangles;
		for (int k = 0; k < triangles.Count; k++) {
			bool changed = false;
			Triangle tri = triangles[k];
			ushort vertex0 = tri.vertex0;
			if (tri.type0 == TriIndexType.Vertex) {
				vertex0 = (ushort)ReinsertTriangleVertex(vertex0, old_index, new_index);
				changed = changed || (vertex0 != tri.vertex0);
			}
			ushort vertex1 = tri.vertex1;
			if (tri.type1 == TriIndexType.Vertex) {
				vertex1 = (ushort)ReinsertTriangleVertex(vertex1, old_index, new_index);
				changed = changed || (vertex1 != tri.vertex1);
			}
			ushort vertex2 = tri.vertex2;
			if (tri.type2 == TriIndexType.Vertex) {
				vertex2 = (ushort)ReinsertTriangleVertex(vertex2, old_index, new_index);
				changed = changed || (vertex2 != tri.vertex2);
			}
			if (changed) {
				triangles[k] = new Triangle(tri.type0, tri.type1, tri.type2, vertex0, vertex1, vertex2);
				_repaint_menu = true;
				_render_mesh = true;
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

	private void DeleteMaintainTriangles(List<int> deletions)
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		List<Triangle> triangles = target.triangles;
		for (int k = 0; k < triangles.Count; k++) {
			bool changed = false;
			Triangle tri = triangles[k];
			ushort vertex0 = tri.vertex0;
			if (tri.type0 == TriIndexType.Vertex) {
				if (selectset.Contains(vertex0)) {
					//deleted
					vertex0 = ushort.MaxValue;
					changed = true;
				}
				else if (vertex0 >= 0 && vertex0 < deletions.Count) {
					vertex0 -= (ushort)deletions[vertex0];
					changed = true;
				}
			}
			ushort vertex1 = tri.vertex1;
			if (tri.type1 == TriIndexType.Vertex) {
				if (selectset.Contains(vertex1)) {
					//deleted
					vertex1 = ushort.MaxValue;
					changed = true;
				}
				else if (vertex1 >= 0 && vertex1 < deletions.Count) {
					vertex1 -= (ushort)deletions[vertex1];
					changed = true;
				}
			}
			ushort vertex2 = tri.vertex2;
			if (tri.type2 == TriIndexType.Vertex) {
				if (selectset.Contains(vertex2)) {
					//deleted
					vertex2 = ushort.MaxValue;
					changed = true;
				}
				else if (vertex2 >= 0 && vertex2 < deletions.Count) {
					vertex2 -= (ushort)deletions[vertex2];
					changed = true;
				}
			}
			if (changed) {
				triangles[k] = new Triangle(tri.type0, tri.type1, tri.type2, vertex0, vertex1, vertex2);
			}
		}
	}

	private void UpdateVertexList()
	{
		//clear list
		_vertices.Clear();
		//check valid list target
		if (target == null || target.vertices == null) {
			_vertex_scroll = Vector2.zero;
			_vertexlist.index = -1;
			return;
		}
		//fill list
		List<Vector3> vertices = target.vertices;
		for (int k = 0;k < target.vertices.Count;k++) {
			_vertices.Add(vertices[k]);
		}
		if (_vertexlist.index >= _vertices.Count) {
			_vertexlist.index = -1;
		}
	}
	private void UpdateRects(Rect rect)
	{
		float import_height = (2 * VxlGUI.MED_BAR) + (2 * VxlGUI.MED_PAD);
		_rect_import = VxlGUI.GetBelowElement(rect, 0, import_height);
		Rect main_rect = VxlGUI.GetSandwichedRectY(rect, 0, import_height + VxlGUI.SM_SPACE);
		_rect_vertex_title = VxlGUI.GetAboveElement(main_rect, 0, VxlGUI.MED_BAR);
		_rect_vertex_scrollview = VxlGUI.GetSandwichedRectY(main_rect, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.SM_SPACE + VxlGUI.MED_BAR);
		_rect_vertex_content = VxlGUI.GetScrollViewRect(_vertexlist, main_rect.width, _rect_vertex_scrollview.height);
		_rect_vertex_panel = VxlGUI.GetBelowElement(main_rect, 0, VxlGUI.MED_BAR);
	}

	private void AddVertexElement()
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		Undo.RecordObject(target, "Add New Vertex");
		target.vertices.Add(Vector3.zero);
		_update_mesh = true;
		_repaint_menu = true;
		_render_mesh = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void DeleteSelectedVertex()
	{
		if (target == null || target.vertices == null) {
			return;
		}
		List<Vector3> vertices = target.vertices;
		if (vertices.Count <= 0 || selectlist.Count <= 0) {
			return;
		}
		Undo.RecordObject(target, "Delete Selected Vertex");
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
		_update_mesh = true;
		_repaint_menu = true;
		_render_mesh = true;
		EditorUtility.SetDirty(target);
	}

	#region VertexBuild ReorderableList
	private void DrawVertexNoneElement(Rect rect)
	{
		GUI.Label(rect, "No VertexVectors Found.", GUI.skin.GetStyle("RightListLabel"));
	}
	private void DrawVertexElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_vertices.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = selectset.Contains(index);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}
	private void DrawVertexElement(Rect rect, int index, bool active, bool focus)
	{
		EditorGUI.BeginChangeCheck();
		float width = Mathf.Min(rect.width / 4f);
		//draw label
		EditorGUI.LabelField(
			VxlGUI.GetLeftElement(rect, 0, width),
			"Vertex " + index,
			GUI.skin.GetStyle("LeftListLabel")
		);
		Vector3 vertex = EditorGUI.Vector3Field(VxlGUI.GetSandwichedRectX(rect, width, 0), "", _vertices[index]);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(target, "Updated Vertex");
			target.vertices[index] = vertex;
			_repaint_menu = true;
			_render_mesh = true;
			_update_mesh = true;
			EditorUtility.SetDirty(target);
		}
	}
	private void ReorderVertexList(ReorderableList list, int old_index, int new_index)
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		if (old_index < 0 || new_index < 0 || old_index == new_index) {
			return;
		}
		List<Vector3> vertices = target.vertices;
		if (old_index >= vertices.Count || new_index >= vertices.Count) {
			return;
		}
		Undo.RecordObject(target, "Reorder Vertex List");
		Vector3 vertex = vertices[old_index];
		vertices.RemoveAt(old_index);
		vertices.Insert(new_index, vertex);
		//maintain selection
		ReorderMaintainSelection(old_index, new_index, selectlist, selectset);
		//maintain triangles
		ReorderMaintainTriangles(old_index, new_index);
		//dirty target object
		_repaint_menu = true;
		EditorUtility.SetDirty(target);
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
		_render_mesh = true;
	}
	#endregion
}
