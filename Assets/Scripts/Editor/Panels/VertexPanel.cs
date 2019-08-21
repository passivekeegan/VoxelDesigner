using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class VertexPanel : PanelGUI
{
	public VoxelComponent target;

	private Rect _rect_vertex_title;
	private Rect _rect_vertex_scrollview;
	private Rect _rect_vertex_content;
	private Rect _rect_vertex_panel;
	private Rect _rect_operation_title;
	private Rect _rect_operation_content;
	private Rect _rect_operation_scrollview;
	private Rect _rect_operations_panel;

	private Vector2 _vertex_scroll;
	private Vector2 _operation_scroll;
	private List<VertexVector> _vertices;
	private List<VectorOperation> _operations;
	private ReorderableList _vertexlist;
	private ReorderableList _operationlist;

	public VertexPanel(string title)
	{
		_title = title;
		_vertices = new List<VertexVector>();
		_operations = new List<VectorOperation>();
	}

	public override void Enable()
	{
		_vertex_scroll = Vector2.zero;
		_operation_scroll = Vector2.zero;
		//initialize vertex build list
		_vertexlist = new ReorderableList(_vertices, typeof(VertexVector), true, false, false, false);
		_vertexlist.showDefaultBackground = false;
		_vertexlist.headerHeight = 0;
		_vertexlist.footerHeight = 0;
		_vertexlist.elementHeight = VxlGUI.MED_BAR;
		_vertexlist.drawNoneElementCallback += DrawVertexNoneElement;
		_vertexlist.drawElementBackgroundCallback += DrawVertexElementBackground;
		_vertexlist.onAddCallback += AddVertexElement;
		_vertexlist.onRemoveCallback += DeleteVertexElement;
		_vertexlist.drawElementCallback += DrawVertexElement;
		_vertexlist.onReorderCallbackWithDetails += ReorderVertexList;
		//initialize vertex operation list
		_operationlist = new ReorderableList(_operations, typeof(VectorOperation), true, false, false, false);
		_operationlist.showDefaultBackground = false;
		_operationlist.headerHeight = 0;
		_operationlist.footerHeight = 0;
		_operationlist.elementHeight = VxlGUI.MED_BAR;
		_operationlist.drawNoneElementCallback += DrawOperationNoneElement;
		_operationlist.drawElementBackgroundCallback += DrawOperationElementBackground;
		_operationlist.onAddCallback += AddOperationElement;
		_operationlist.onRemoveCallback += DeleteOperationElement;
		_operationlist.drawElementCallback += DrawOperationElement;
		_operationlist.onReorderCallbackWithDetails += ReorderOperationList;
	}

	public override void Disable()
	{
		target = null;

		_operationlist = null;
		_operations.Clear();
		_vertexlist = null;
		_vertices.Clear();
	}

	public override int primary_index {
		get {
			return _vertexlist.index;
		}
	}

	public override PreviewDrawMode previewMode {
		get {
			return PreviewDrawMode.Vertex;
		}
	}

	public override void DrawGUI(Rect rect)
	{
		//update lists
		UpdateVertexList();
		UpdateOperationList();
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
			_vertexlist.onAddCallback(_vertexlist);
		}
		//disable operation check
		EditorGUI.BeginDisabledGroup(_vertexlist.index < 0 || _vertexlist.index >= _vertexlist.count);
		//draw vertex delete button
		if (GUI.Button(VxlGUI.GetLeftElement(_rect_vertex_panel, 0, button_width), "Delete", GUI.skin.GetStyle("LightButton"))) {
			_vertexlist.onRemoveCallback(_vertexlist);
		}
		//draw operation title
		VxlGUI.DrawRect(_rect_operation_title, "DarkGradient");
		GUI.Label(_rect_operation_title, "Vertex Operations", GUI.skin.GetStyle("LeftLightHeader"));
		//draw operation list
		VxlGUI.DrawRect(_rect_operation_scrollview, "DarkWhite");
		_operation_scroll = GUI.BeginScrollView(_rect_operation_scrollview, _operation_scroll, _rect_operation_content);
		_operationlist.DoList(_rect_operation_content);
		GUI.EndScrollView();
		//draw operation button panel
		button_width = Mathf.Min(60f, _rect_operations_panel.width / 2f);
		VxlGUI.DrawRect(_rect_operations_panel, "DarkGradient");
		//draw operation add button
		if (GUI.Button(VxlGUI.GetRightElement(_rect_operations_panel, 0, button_width), "Add", GUI.skin.GetStyle("LightButton"))) {
			_operationlist.onAddCallback(_operationlist);
		}
		//draw operation delete button
		EditorGUI.BeginDisabledGroup(_operationlist.index < 0 || _operationlist.index >= _operationlist.count);
		if (GUI.Button(VxlGUI.GetLeftElement(_rect_operations_panel, 0, button_width), "Delete", GUI.skin.GetStyle("LightButton"))) {
			_operationlist.onRemoveCallback(_operationlist);
		}
		EditorGUI.EndDisabledGroup();
		EditorGUI.EndDisabledGroup();
		EditorGUI.EndDisabledGroup();
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
		for (int k = 0;k < target.vertices.Count;k++) {
			_vertices.Add(target.vertices[k]);
		}
		if (_vertexlist.index >= _vertices.Count) {
			_vertexlist.index = -1;
		}
	}
	private void UpdateOperationList()
	{
		//clear list
		_operations.Clear();
		//check valid list target
		List<VectorOperation> operations = GetSelectedOperation();
		if (operations == null) {
			_operation_scroll = Vector2.zero;
			_operationlist.index = -1;
			return;
		}
		//fill list
		for (int k = 0; k < operations.Count; k++) {
			_operations.Add(operations[k]);
		}
		if (_operationlist.index >= _operations.Count) {
			_operationlist.index = -1;
		}
	}
	private void UpdateRects(Rect rect)
	{
		//left column - vertex
		Rect left_rect = VxlGUI.GetLeftColumn(rect, VxlGUI.SM_SPACE, 0.4f);
		_rect_vertex_title = VxlGUI.GetAboveElement(left_rect, 0, VxlGUI.MED_BAR);
		_rect_vertex_scrollview = VxlGUI.GetSandwichedRectY(left_rect, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.SM_SPACE + VxlGUI.MED_BAR);
		_rect_vertex_content = VxlGUI.GetScrollViewRect(_vertexlist, left_rect.width, _rect_vertex_scrollview.height);
		_rect_vertex_panel = VxlGUI.GetBelowElement(left_rect, 0, VxlGUI.MED_BAR);
		//right column - operation
		Rect right_rect = VxlGUI.GetRightColumn(rect, VxlGUI.SM_SPACE, 0.6f);
		_rect_operation_title = VxlGUI.GetAboveElement(right_rect, 0, VxlGUI.MED_BAR);
		_rect_operation_scrollview = VxlGUI.GetSandwichedRectY(right_rect, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.SM_SPACE + VxlGUI.MED_BAR);
		_rect_operation_content = VxlGUI.GetScrollViewRect(_operationlist, right_rect.width, _rect_operation_scrollview.height);
		_rect_operations_panel = VxlGUI.GetBelowElement(right_rect, 0, VxlGUI.MED_BAR);
	}
	private List<VectorOperation> GetSelectedOperation()
	{
		if (target == null || target.vertices == null) {
			return null;
		}
		int vertex_index = _vertexlist.index;
		if (vertex_index < 0 || vertex_index >= target.vertices.Count) {
			return null;
		}
		return target.vertices[vertex_index].operations;
	}

	#region VertexBuild ReorderableList
	private void DrawVertexNoneElement(Rect rect)
	{
		EditorGUI.LabelField(rect, "No VertexBuilds Found.", GUI.skin.GetStyle("RightListLabel"));
	}
	private void DrawVertexElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_vertices.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (_vertices.Count > 0 && _vertexlist.index == index);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}
	private void AddVertexElement(ReorderableList list)
	{
		if (target == null || target.vertices == null) {
			return;
		}
		Undo.RecordObject(target, "Insert New VertexBuild");
		int index = list.index;
		if (index < 0 || index >= target.vertices.Count) {
			target.vertices.Add(VertexVector.empty);
			list.index = target.vertices.Count - 1;
		}
		else {
			target.vertices.Insert(index, VertexVector.empty);
		}
		update = true;
		repaint = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void DeleteVertexElement(ReorderableList list)
	{
		if (target == null || target.vertices == null) {
			return;
		}
		int index = list.index;
		if (index < 0 || index >= target.vertices.Count) {
			return;
		}
		Undo.RecordObject(target, "Delete VertexBuild");
		target.vertices.RemoveAt(index);
		if (target.vertices.Count > 0) {
			if (index > 0) {
				list.index = list.index - 1;
			}
		}
		else {
			list.index = -1;
		}
		update = true;
		repaint = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void DrawVertexElement(Rect rect, int index, bool active, bool focus)
	{
		float button_width = 0.5f * rect.width;
		//draw left label
		EditorGUI.LabelField(
			VxlGUI.GetLeftElement(rect, 0, button_width),
			"Vertex " + index,
			GUI.skin.GetStyle("LeftListLabel")
		);
		int ops = 0;
		if (index >= 0 && index < _vertices.Count) {
			if (_vertices[index].operations != null) {
				ops = _vertices[index].operations.Count;
			}
		}
		//draw right label
		EditorGUI.LabelField(
			VxlGUI.GetRightElement(rect, 0, button_width),
			ops + " Ops",
			GUI.skin.GetStyle("RightListLabel")
		);
	}

	private void ReorderVertexList(ReorderableList list, int old_index, int new_index)
	{
		if (target == null || target.vertices == null) {
			return;
		}
		int cnt = target.vertices.Count;
		if (old_index < 0 || new_index < 0 || old_index >= cnt || new_index >= cnt || old_index == new_index) {
			return;
		}
		Undo.RecordObject(target, "Reorder VertexBuild List");
		VertexVector old_vertex = target.vertices[old_index];
		VertexVector new_vertex = target.vertices[new_index];
		target.vertices[old_index] = new_vertex;
		target.vertices[new_index] = old_vertex;
		list.index = new_index;
		update = true;
		repaint = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	#endregion

	#region VertexOperation ReorderableList
	private void DrawOperationNoneElement(Rect rect)
	{
		GUI.Label(rect, "No VertexOperations Found.", GUI.skin.GetStyle("RightListLabel"));
	}
	private void DrawOperationElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_operations.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (_operations.Count > 0 && _operationlist.index == index);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}
	private void AddOperationElement(ReorderableList list)
	{
		List<VectorOperation> operations = GetSelectedOperation();
		if (operations == null) {
			return;
		}
		Undo.RecordObject(target, "Add New Vertex Operation");
		int index = list.index;
		if (index < 0 || index >= operations.Count) {
			operations.Add(VectorOperation.empty);
			list.index = operations.Count - 1;
		}
		else {
			operations.Insert(index, VectorOperation.empty);
		}
		repaint = true;
		update = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void DeleteOperationElement(ReorderableList list)
	{
		int index = list.index;
		List<VectorOperation> operations = GetSelectedOperation();
		if (operations == null || index < 0 || index >= operations.Count) {
			return;
		}
		Undo.RecordObject(target, "Delete VertexOperation");
		operations.RemoveAt(index);
		if (operations.Count > 0) {
			if (index > 0) {
				list.index = list.index - 1;
			}
		}
		else {
			list.index = -1;
		}
		repaint = true;
		update = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void DrawOperationElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		if (target == null || target.vertices == null) {
			return;
		}
		float max_element_width = 100f;
		float index_width = Mathf.Min(2 * rect.height, rect.width / 4f);
		Rect right_panel = VxlGUI.GetRightElement(VxlGUI.GetPaddedRect(rect, 1), 0, Mathf.Min((2 * max_element_width) + rect.height + (2 * VxlGUI.SM_SPACE), rect.width - index_width));
		float element_width = Mathf.Min(max_element_width, (right_panel.width - right_panel.height - VxlGUI.SM_SPACE - VxlGUI.MED_SPACE) / 2f);
		EditorGUI.LabelField(
			VxlGUI.GetLeftElement(rect, 0, index_width),
			index.ToString(),
			GUI.skin.GetStyle("LeftListLabel")
		);
		//draw toggle background
		Rect rect_toggle = VxlGUI.GetLeftElement(right_panel, 0, right_panel.height);
		//check for changes
		EditorGUI.BeginChangeCheck();
		//draw custom toggle controls
		bool use_custom = EditorGUI.Toggle(
			rect_toggle,
			_operations[index].use_custom,
			GUI.skin.GetStyle("Toggle")
		);
		//draw modifier controls
		float custom_modifier = _operations[index].custom_modifier;
		ModifierType modifier = _operations[index].modifier;
		if (use_custom) {
			custom_modifier = EditorGUI.FloatField(
				VxlGUI.GetRightElement(right_panel, 1, element_width, VxlGUI.SM_SPACE, 0),
				custom_modifier,
				GUI.skin.GetStyle("DarkNumberField")
			);
		}
		else {
			modifier = (ModifierType)EditorGUI.EnumPopup(
				VxlGUI.GetRightElement(right_panel, 1, element_width, VxlGUI.SM_SPACE, 0),
				"",
				modifier,
				GUI.skin.GetStyle("DarkDropdown")
			);
		}
		//draw vector type controls
		VectorType vector = (VectorType)EditorGUI.EnumPopup(
			VxlGUI.GetRightElement(right_panel, 0, element_width),
			"",
			_operations[index].vector,
			GUI.skin.GetStyle("DarkDropdown")
		);
		//apply changes
		if (EditorGUI.EndChangeCheck()) {
			List<VectorOperation> operations = GetSelectedOperation();
			if (operations == null || index < 0 || index >= operations.Count) {
				return;
			}
			Undo.RecordObject(target, "Updated Vertex Operation.");
			operations[index] = new VectorOperation(use_custom, custom_modifier, modifier, vector);
			repaint = true;
			update = true;
			//dirty target object
			EditorUtility.SetDirty(target);
		}
	}

	private void ReorderOperationList(ReorderableList list, int old_index, int new_index)
	{
		List<VectorOperation> operations = GetSelectedOperation();
		if (operations == null) {
			return;
		}
		int cnt = operations.Count;
		if (old_index < 0 || new_index < 0 || old_index >= cnt || new_index >= cnt || old_index == new_index) {
			return;
		}
		Undo.RecordObject(target, "Reorder VertexOperation List");
		VectorOperation old_op = operations[old_index];
		VectorOperation new_op = operations[new_index];
		operations[old_index] = new_op;
		operations[new_index] = old_op;
		list.index = new_index;
		update = true;
		repaint = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	#endregion
}
