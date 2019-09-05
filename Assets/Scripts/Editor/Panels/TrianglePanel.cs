using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class TrianglePanel : PanelGUI
{
	public VoxelComponent target;
	public List<int> selectlist;
	public HashSet<int> selectset;

	private Rect _rect_title;
	private Rect _rect_tri_scroll;
	private Rect _rect_tri_content;
	private Rect _rect_panel;

	private Vector2 _scroll;
	private string[] _optionstrs;
	private string[] _cornerplug_labels;
	private string[] _edgeplug_labels;
	private TriIndexType[] _options;
	
	private List<Triangle> _triangles;
	private ReorderableList _trianglelist;
	private MeshPreview _preview;
	private PreviewVertexDisplay _selectdisplay;

	public TrianglePanel(MeshPreview preview, string title, string[] cornerplug_labels, string[] edgeplug_labels)
	{
		_title = title;
		_preview = preview;
		selectlist = new List<int>();
		selectset = new HashSet<int>();
		_cornerplug_labels = cornerplug_labels;
		_edgeplug_labels = edgeplug_labels;
		_triangles = new List<Triangle>();
		_selectdisplay = new PreviewVertexDisplay();
		//refresh options available
		RefreshOptionLabels();
	}

	public override void Enable()
	{
		_update_mesh = true;
		_repaint_menu = true;
		_render_mesh = true;
		_scroll = Vector2.zero;
		_selectdisplay.Enable();
		selectlist.Clear();
		selectset.Clear();
		//initialize triangle list
		_trianglelist = new ReorderableList(_triangles, typeof(Triangle), true, false, false, false);
		_trianglelist.showDefaultBackground = false;
		_trianglelist.headerHeight = 0;
		_trianglelist.footerHeight = 0;
		_trianglelist.elementHeight = (4 * VxlGUI.MED_BAR) + (5 * VxlGUI.SM_SPACE);
		_trianglelist.drawNoneElementCallback += DrawTriangleNoneElement;
		_trianglelist.drawElementBackgroundCallback += DrawTriangleElementBackground;
		_trianglelist.drawElementCallback += DrawTriangleElement;
		_trianglelist.onReorderCallbackWithDetails += ReorderTriangleElements;
		_trianglelist.onSelectCallback += SelectTriangle;
	}

	public override void Disable()
	{
		target = null;

		_trianglelist = null;
		_triangles.Clear();
		_update_mesh = false;
		_repaint_menu = false;
		_render_mesh = false;
		selectlist.Clear();
		selectset.Clear();
		_selectdisplay.Disable();
	}

	public override void DrawGUI(Rect rect)
	{
		RefreshOptionLabels();
		UpdateTriangleList();
		UpdateRects(rect);
		EditorGUI.BeginDisabledGroup(target == null);
		//draw title
		VxlGUI.DrawRect(_rect_title, "DarkGradient");
		GUI.Label(_rect_title, "Triangles", GUI.skin.GetStyle("LeftLightHeader"));
		//draw triangle list
		VxlGUI.DrawRect(_rect_tri_scroll, "DarkWhite");
		_scroll = GUI.BeginScrollView(_rect_tri_scroll, _scroll, _rect_tri_content);
		_trianglelist.DoList(_rect_tri_content);
		GUI.EndScrollView();
		//draw button panel
		VxlGUI.DrawRect(_rect_panel, "DarkGradient");
		float button_width = Mathf.Min(60, _rect_panel.width / 4f);
		//draw add button
		if (GUI.Button(VxlGUI.GetRightElement(_rect_panel, 0, button_width), "Add", GUI.skin.GetStyle("LightButton"))) {
			AddTriangle();
		}
		//draw add triangle button
		EditorGUI.BeginDisabledGroup(_preview.secondaryCount != 3);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_panel, 1, button_width), "S Add", GUI.skin.GetStyle("LightButton"))) {
			AddSecondaryTriangle();
		}
		EditorGUI.EndDisabledGroup();
		//draw flip button
		EditorGUI.BeginDisabledGroup((_triangles.Count <= 0) || (selectlist.Count <= 0));
		if (GUI.Button(VxlGUI.GetRightElement(_rect_panel, 2, button_width), "Flip", GUI.skin.GetStyle("LightButton"))) {
			FlipSelectedTriangles();
		}
		//draw delete button
		if (GUI.Button(VxlGUI.GetLeftElement(_rect_panel, 0, button_width), "Delete", GUI.skin.GetStyle("LightButton"))) {
			DeleteSelectedTriangles();
		}
		EditorGUI.EndDisabledGroup();
		EditorGUI.EndDisabledGroup();
	}

	public override void DrawPreview(Rect rect)
	{
		//update preview displays
		_selectdisplay.selected = _preview.secondaryCount;
		_selectdisplay.superselected = _preview.secondarySuper;

		rect = VxlGUI.GetPaddedRect(rect, VxlGUI.MED_PAD);
		float width = Mathf.Min(200, rect.width / 2f);
		_selectdisplay.DrawGUI(VxlGUI.GetAboveRightElement(rect, 0, width, 0, rect.height / 2f));
	}


	public void UpdateTriangleList()
	{
		//clear list
		_triangles.Clear();
		//check if valid list target
		if (target == null || target.triangles == null) {
			_scroll = Vector2.zero;
			_trianglelist.index = -1;
			return;
		}
		//fill list
		for (int k = 0;k < target.triangles.Count;k++) {
			_triangles.Add(target.triangles[k]);
		}
	}

	private void UpdateRects(Rect rect)
	{
		_rect_title = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR);
		_rect_tri_scroll = VxlGUI.GetSandwichedRectY(rect, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.SM_SPACE + VxlGUI.MED_BAR);
		_rect_tri_content = VxlGUI.GetScrollViewRect(_trianglelist, _rect_tri_scroll.width, _rect_tri_scroll.height);
		_rect_panel = VxlGUI.GetBelowElement(rect, 0, VxlGUI.MED_BAR);
	}
	private void RefreshOptionLabels()
	{
		List<TriIndexType> options = new List<TriIndexType>() {
			TriIndexType.Vertex
		};
		List<string> optionstrs = new List<string>() {
			"Vertex"
		};
		if (target != null) {
			if (target.cornerplugs != null && target.cornerplugs.Count > 0) {
				options.Add(TriIndexType.CornerPlug);
				optionstrs.Add("CornerPlug");
			}
			if (target.edgeplugs != null && target.edgeplugs.Count > 0) {
				options.Add(TriIndexType.EdgePlug);
				optionstrs.Add("EdgePlug");
			}
		}
		_options = options.ToArray();
		_optionstrs = optionstrs.ToArray();
	}
	private int OptionIndex(TriIndexType type)
	{
		for (int k = 0;k < _options.Length;k++) {
			if (type == _options[k]) {
				return k;
			}
		}
		return -1;
	}
	private int GetVertexCount(TriIndexType type)
	{
		if (target == null) {
			return 0;
		}
		if (type == TriIndexType.Vertex) {
			if (target.vertices != null) {
				return target.vertices.Count;
			}
		}
		else if (type == TriIndexType.CornerPlug) {
			if (target.hasCornerPlug) {
				return CountList(target.cornerplugs);
			}
		}
		else if (type == TriIndexType.EdgePlug) {
			if (target.hasEdgePlug) {
				return CountList(target.edgeplugs);
			}
		}
		return 0;
	}
	private int CountList(List<int> list)
	{
		int count = 0;
		if (list != null) {
			for (int k = 0; k < list.Count; k++) {
				count += list[k];
			}
		}
		return count;
	}

	private void AddTriangle()
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		Undo.RecordObject(target, "Add New Triangle");
		target.triangles.Add(Triangle.empty);
		_repaint_menu = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void AddSecondaryTriangle()
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		if (_preview.secondaryCount != 3) {
			return;
		}
		Undo.RecordObject(target, "Add Secondary Triangle");
		target.triangles.Add(_preview.GetSelectedTriangle());
		_repaint_menu = true;
		_update_mesh = true;
		_render_mesh = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void DeleteSelectedTriangles()
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		List<Triangle> triangles = target.triangles;
		if (triangles.Count <= 0 || selectlist.Count <= 0) {
			return;
		}
		Undo.RecordObject(target, "Delete Selected Triangles");
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
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void FlipSelectedTriangles()
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		List<Triangle> triangles = target.triangles;
		if (triangles.Count <= 0 || selectlist.Count <= 0) {
			return;
		}
		Undo.RecordObject(target, "Flip Selected Triangles");
		for (int k = 0;k < selectlist.Count;k++) {
			int index = selectlist[k];
			if (index < 0 || index >= triangles.Count) {
				continue;
			}
			triangles[index] = Triangle.Flip(triangles[index]);
		}
		_update_mesh = true;
		_repaint_menu = true;
		_render_mesh = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	

	#region TriangleBuild ReorderableList
	private void DrawTriangleNoneElement(Rect rect)
	{
		GUI.Label(rect, "No Triangles Found.", GUI.skin.GetStyle("RightListLabel"));
	}
	private void DrawTriangleElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_triangles.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = selectset.Contains(index);
		bool valid = true;
		if (index >= 0 && index < _triangles.Count) {
			if (target != null && !target.IsValid()) {
				valid = _triangles[index].IsValid(target.vertices.Count, target.cornerplugs, target.edgeplugs);
			}
		}
		if (valid) {
			VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
		}
		else {
			VxlGUI.DrawRect(rect, "SelectableRed", hover, active, on, focus);	
		}
	}
	private void DrawTriangleElement(Rect rect, int index, bool active, bool focus)
	{
		rect = VxlGUI.GetPaddedRect(rect, VxlGUI.SM_SPACE);
		//draw indexed label
		GUI.Label(
			VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0),
			"Tri: " + index,
			GUI.skin.GetStyle("LeftListLabel")
		);
		EditorGUI.BeginChangeCheck();
		//draw triangle index 0
		TriIndex index0 = DrawTriangleEditElement(
			VxlGUI.GetAboveElement(rect, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0),
			_triangles[index].type0,
			_triangles[index].vertex0
		);
		//draw triangle index 1
		TriIndex index1 = DrawTriangleEditElement(
			VxlGUI.GetAboveElement(rect, 2, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0),
			_triangles[index].type1,
			_triangles[index].vertex1
		);
		//draw triangle index 2
		TriIndex index2 = DrawTriangleEditElement(
			VxlGUI.GetAboveElement(rect, 3, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0),
			_triangles[index].type2,
			_triangles[index].vertex2
		);

		if (EditorGUI.EndChangeCheck()) {
			if (target != null) {
				if (target == null || target.triangles == null) {
					return;
				}
				if (index < 0 || index >= target.triangles.Count) {
					return;
				}
				Undo.RecordObject(target, "Update Triangle Build");
				target.triangles[index] = new Triangle(index0, index1, index2);
				_update_mesh = true;
				_render_mesh = true;
				_repaint_menu = true;
				//dirty target object
				EditorUtility.SetDirty(target);
			}
		}
	}
	private TriIndex DrawTriangleEditElement(Rect rect, TriIndexType type, ushort vertexcode)
	{
		rect = VxlGUI.GetRightColumn(rect, 0, 0.95f);
		float element_factor = 0.6f;
		float segment_width = (rect.width - (2 * VxlGUI.SM_SPACE)) / 3;
		float unit_width = (rect.width - (5 * VxlGUI.SM_SPACE)) / 6f;
		//Type Options
		Rect segrect = VxlGUI.GetRightElement(rect, 2, segment_width, VxlGUI.SM_SPACE, 0);
		EditorGUI.LabelField(
			VxlGUI.GetLeftColumn(segrect, 0, 1 - element_factor), 
			"Type:",
			GUI.skin.GetStyle("RightDarkText")
		);
		int type_index = EditorGUI.Popup(
			VxlGUI.GetRightColumn(segrect, 0, element_factor),
			OptionIndex(type),
			_optionstrs,
			GUI.skin.GetStyle("DarkDropdown")
		);
		if (type_index >= 0 && type_index < _options.Length) {
			type = _options[type_index];
		}
		if (type == TriIndexType.CornerPlug || type == TriIndexType.EdgePlug) {
			//Axis Options
			segrect = VxlGUI.GetRightElement(rect, 1, segment_width, VxlGUI.SM_SPACE, 0);
			EditorGUI.LabelField(
				VxlGUI.GetLeftColumn(segrect, 0, 1 - element_factor),
				"Axis:",
				GUI.skin.GetStyle("RightDarkText")
			);
			string[] labels;
			if (type == TriIndexType.CornerPlug) {
				labels = _cornerplug_labels;
			}
			else {
				labels = _edgeplug_labels;
			}
			int axis_index = EditorGUI.Popup(
				VxlGUI.GetRightColumn(segrect, 0, element_factor),
				TriIndex.DecodeAxiIndex(vertexcode), 
				labels, 
				GUI.skin.GetStyle("DarkDropdown")
			);
			//Socket Options
			segrect = VxlGUI.GetRightElement(rect, 0, segment_width, VxlGUI.SM_SPACE, 0);
			EditorGUI.LabelField(
				VxlGUI.GetLeftColumn(segrect, 0, 1 - element_factor), 
				"Socket:",
				GUI.skin.GetStyle("RightDarkText")
			);
			int socket_index = EditorGUI.IntField(
				VxlGUI.GetRightColumn(segrect, 0, element_factor),
				TriIndex.DecodeIndex(vertexcode),
				GUI.skin.GetStyle("DarkNumberField")
			);
			vertexcode = TriIndex.EncodeIndex((byte) axis_index, (byte) socket_index);
		}
		else {
			//Vertex Index Options
			segrect = VxlGUI.GetRightElement(rect, 0, segment_width, VxlGUI.SM_SPACE, 0);
			EditorGUI.LabelField(
				VxlGUI.GetLeftColumn(segrect, 0, 1 - element_factor),
				"Index:",
				GUI.skin.GetStyle("RightDarkText")
			);
			vertexcode = (ushort) EditorGUI.IntField(
				VxlGUI.GetRightColumn(segrect, 0, element_factor),
				vertexcode,
				GUI.skin.GetStyle("DarkNumberField")
			);
		}
		return new TriIndex(type, vertexcode);
	}
	private void ReorderTriangleElements(ReorderableList list, int old_index, int new_index)
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		if (old_index < 0 || new_index < 0 || old_index == new_index) {
			return;
		}
		List<Triangle> triangles = target.triangles;
		if (old_index >= triangles.Count || new_index >= triangles.Count) {
			return;
		}
		Undo.RecordObject(target, "Reorder Triangles");
		Triangle old_tri = triangles[old_index];
		triangles.RemoveAt(old_index);
		triangles.Insert(new_index, old_tri);
		//maintain selection
		ReorderMaintainSelection(old_index, new_index, selectlist, selectset);
		//dirty target object
		_repaint_menu = true;
		EditorUtility.SetDirty(target);
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
		_update_mesh = true;
		_render_mesh = true;
	}
	#endregion
}
