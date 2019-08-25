using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class EdgeMapPanel : PanelGUI
{
	public MappingObject target;

	private Rect _rect_selectheader;
	private Rect _rect_selectscroll;
	private Rect _rect_select_content;
	private Rect _rect_pattemplateheader;
	private Rect _rect_pattemplate;
	private Rect _rect_patheader;
	private Rect _rect_patscroll;
	private Rect _rect_patcontent;
	private Rect _rect_patpanel;
	private Vector2 _selectscroll;
	private Vector2 _patscroll;

	//Mapping CornerDesign List
	private List<EdgeDesign> _edges;
	private ReorderableList _edgelist;

	//Pattern List
	private EdgePattern _addpat;
	private List<EdgePattern> _patterns;
	private ReorderableList _patternlist;


	public EdgeMapPanel(string title)
	{
		_title = title;
		_edges = new List<EdgeDesign>();
		_patterns = new List<EdgePattern>();
	}

	public override void Enable()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_addpat = EdgePattern.empty;
		_selectscroll = Vector2.zero;
		_patscroll = Vector2.zero;

		_edgelist = new ReorderableList(_edges, typeof(EdgeDesign), false, false, false, false);
		_edgelist.showDefaultBackground = false;
		_edgelist.headerHeight = 0;
		_edgelist.footerHeight = 0;
		_edgelist.elementHeight = VxlGUI.MED_BAR;
		_edgelist.drawNoneElementCallback += DrawEdgeNoneElement;
		_edgelist.drawElementCallback += DrawEdgeElement;
		_edgelist.drawElementBackgroundCallback += DrawEdgeElementBackground;

		_patternlist = new ReorderableList(_patterns, typeof(EdgePattern), false, false, false, false);
		_patternlist.showDefaultBackground = false;
		_patternlist.headerHeight = 0;
		_patternlist.footerHeight = 0;
		_patternlist.elementHeightCallback += PatternHeight;
		_patternlist.drawNoneElementCallback += DrawPatternNoneElement;
		_patternlist.drawElementCallback += DrawPatternElement;
		_patternlist.drawElementBackgroundCallback += DrawPatternElementBackground;
	}

	public override void Disable()
	{
		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
		target = null;

		_patternlist = null;
		_patterns.Clear();
		_edgelist = null;
		_edges.Clear();
	}

	public override void DrawGUI(Rect rect)
	{
		//update both visual lists
		UpdateEdgeList();
		UpdatePatternList();
		//update layout rects
		UpdateLayoutRects(rect);

		//draw key selection header
		VxlGUI.DrawRect(_rect_selectheader, "DarkGradient");
		GUI.Label(_rect_selectheader, _title, GUI.skin.GetStyle("LeftLightHeader"));
		//draw key selection list
		VxlGUI.DrawRect(_rect_selectscroll, "DarkWhite");
		_selectscroll = GUI.BeginScrollView(_rect_selectscroll, _selectscroll, _rect_select_content);
		_edgelist.DoList(_rect_select_content);
		GUI.EndScrollView();
		//draw add pattern template
		VxlGUI.DrawRect(_rect_pattemplateheader, "DarkGradient");
		GUI.Label(_rect_pattemplateheader, "Add Template", GUI.skin.GetStyle("LeftLightHeader"));
		VxlGUI.DrawRect(_rect_pattemplate, "DarkWhite");
		Rect rect_content = VxlGUI.GetPaddedRect(_rect_pattemplate, VxlGUI.MED_PAD);

		EditorGUI.BeginChangeCheck();
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftColumn(rect_row, 0, 0.5f), "EdgeType", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		int type_index = 0;
		if (_addpat.vertical) {
			type_index = 1;
		}
		type_index = EditorGUI.Popup(
			VxlGUI.GetRightColumn(rect_row, 0, 0.5f),
			type_index,
			new string[] { "Horizontal", "Vertical" },
			GUI.skin.GetStyle("DarkDropDown")
		);
		//row 1
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(rect_row, "Corner 0", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		//row 2
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		float label_factor = 0.4f;
		Rect left_rect = VxlGUI.GetLeftColumn(rect_row, 0, 0.5f);
		GUI.Label(VxlGUI.GetLeftColumn(left_rect, 0, label_factor), "Plugs:", GUI.skin.GetStyle("MidDarkLabel"));
		int p0 = Mathf.Max(0, EditorGUI.IntField(
			VxlGUI.GetRightColumn(left_rect, 0, 1 - label_factor), 
			_addpat.p0,
			GUI.skin.GetStyle("DarkNumberField")
		));
		Rect right_rect = VxlGUI.GetRightColumn(rect_row, 0, 0.5f);
		GUI.Label(VxlGUI.GetLeftColumn(right_rect, 0, label_factor), "ID:", GUI.skin.GetStyle("MidDarkLabel"));
		int c0 = Mathf.Max(-1, EditorGUI.IntField(
			VxlGUI.GetRightColumn(right_rect, 0, 1 - label_factor), 
			_addpat.c0,
			GUI.skin.GetStyle("DarkNumberField")
		));
		//row 3
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(rect_row, "Corner 1", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		//row 4
		rect_row = VxlGUI.GetAboveElement(rect_content, 4, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		left_rect = VxlGUI.GetLeftColumn(rect_row, 0, 0.5f);
		GUI.Label(VxlGUI.GetLeftColumn(left_rect, 0, label_factor), "Plugs:", GUI.skin.GetStyle("MidDarkLabel"));
		int p1 = Mathf.Max(0, EditorGUI.IntField(
			VxlGUI.GetRightColumn(left_rect, 0, 1 - label_factor), 
			_addpat.p1,
			GUI.skin.GetStyle("DarkNumberField")
		));
		right_rect = VxlGUI.GetRightColumn(rect_row, 0, 0.5f);
		GUI.Label(VxlGUI.GetLeftColumn(right_rect, 0, label_factor), "ID:", GUI.skin.GetStyle("MidDarkLabel"));
		int c1 = Mathf.Max(-1, EditorGUI.IntField(
			VxlGUI.GetRightColumn(right_rect, 0, 1 - label_factor), 
			_addpat.c1,
			GUI.skin.GetStyle("DarkNumberField")
		));
		//row 5
		rect_row = VxlGUI.GetAboveElement(rect_content, 5, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(rect_row, "Voxel IDs", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		//row 6
		rect_row = VxlGUI.GetAboveElement(rect_content, 6, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		float width = rect_row.width / 4f;
		if (type_index == 1) {
			width = rect_row.width / 3f;
		}
		float smlabel_factor = 0.4f;
		//A0 or V0
		string label = "A0";
		if (type_index == 1) {
			label = "V0";
		}
		Rect element_rect = VxlGUI.GetLeftElement(rect_row, 0, width);
		GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, smlabel_factor), label, GUI.skin.GetStyle("MidDarkLabel"));
		int v0 = Mathf.Max(0, EditorGUI.IntField(
			VxlGUI.GetRightColumn(element_rect, 0, 1 - smlabel_factor), 
			_addpat.v0,
			GUI.skin.GetStyle("DarkNumberField")
		));
		//B0 or V1
		label = "B0";
		if (type_index == 1) {
			label = "V1";
		}
		element_rect = VxlGUI.GetLeftElement(rect_row, 1, width);
		GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, smlabel_factor), label, GUI.skin.GetStyle("MidDarkLabel"));
		int v1 = Mathf.Max(0, EditorGUI.IntField(
			VxlGUI.GetRightColumn(element_rect, 0, 1 - smlabel_factor), 
			_addpat.v1,
			GUI.skin.GetStyle("DarkNumberField")
		));
		//A1 or V2
		label = "A1";
		if (type_index == 1) {
			label = "V2";
		}
		element_rect = VxlGUI.GetLeftElement(rect_row, 2, width);
		GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, smlabel_factor), label, GUI.skin.GetStyle("MidDarkLabel"));
		int v2 = Mathf.Max(0, EditorGUI.IntField(
			VxlGUI.GetRightColumn(element_rect, 0, 1 - smlabel_factor), 
			_addpat.v2,
			GUI.skin.GetStyle("DarkNumberField")
		));

		int v3 = _addpat.v3;
		if (type_index != 1) {
			//B1 or V3
			label = "B1";
			if (type_index == 1) {
				label = "V3";
			}
			element_rect = VxlGUI.GetLeftElement(rect_row, 3, width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, smlabel_factor), label, GUI.skin.GetStyle("MidDarkLabel"));
			v3 = Mathf.Max(0, EditorGUI.IntField(
				VxlGUI.GetRightColumn(element_rect, 0, 1 - smlabel_factor), 
				v3,
				GUI.skin.GetStyle("DarkNumberField")
			));
		}
		//apply change
		if (EditorGUI.EndChangeCheck()) {
			_addpat = new EdgePattern((type_index == 1), p0, p1, c0, c1, v0, v1, v2, v3);
			_repaint_menu = false;
		}
		//draw pattern header
		VxlGUI.DrawRect(_rect_patheader, "DarkGradient");
		GUI.Label(_rect_patheader, "Patterns", GUI.skin.GetStyle("LeftLightHeader"));
		//draw pattern list
		VxlGUI.DrawRect(_rect_patscroll, "DarkWhite");
		_patscroll = GUI.BeginScrollView(_rect_patscroll, _patscroll, _rect_patcontent);
		_patternlist.DoList(_rect_patcontent);
		GUI.EndScrollView();
		//draw pattern panel
		VxlGUI.DrawRect(_rect_patpanel, "DarkGradient");
		float button_width = Mathf.Min(60f, _rect_patpanel.width / 2f);

		bool invalid_addpat = !_addpat.valid || _addpat.IsEmpty();
		bool invalid_edgeindex = _edgelist.count <= 0 || _edgelist.index < 0 || _edgelist.index >= _edgelist.count;
		bool invalid_exists = target != null && target.EdgePatternExists(_addpat);
		EditorGUI.BeginDisabledGroup(target == null || invalid_addpat || invalid_edgeindex || invalid_exists);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_patpanel, 0, button_width), "Add", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Add Edge Pattern");
				target.AddEdgePattern(_edges[_edgelist.index], _addpat);
				_repaint_menu = false;
				//dirty target
				EditorUtility.SetDirty(target);
				//update pattern list
				UpdatePatternList();
			}
		}
		EditorGUI.EndDisabledGroup();

		bool invalid_patternindex = _patternlist.count <= 0 || _patternlist.index < 0 || _patternlist.index >= _patternlist.count;
		EditorGUI.BeginDisabledGroup(target == null || invalid_edgeindex || invalid_patternindex);
		if (GUI.Button(VxlGUI.GetLeftElement(_rect_patpanel, 0, button_width), "Delete", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Delete Edge Pattern");
				target.DeleteEdgePattern(_edges[_edgelist.index], _patterns[_patternlist.index]);
				_repaint_menu = false;
				//dirty target
				EditorUtility.SetDirty(target);
				//update pattern list
				UpdatePatternList();
			}
		}
		EditorGUI.EndDisabledGroup();
	}

	private void UpdateLayoutRects(Rect rect)
	{
		Rect rect_left = VxlGUI.GetLeftColumn(rect, VxlGUI.MED_SPACE, 0.5f);
		float template_height = (2 * VxlGUI.MED_PAD) + (7 * VxlGUI.MED_BAR) + (6 * VxlGUI.SM_SPACE);
		_rect_selectheader = VxlGUI.GetAboveElement(rect_left, 0, VxlGUI.MED_BAR);
		_rect_selectscroll = VxlGUI.GetSandwichedRectY(rect_left, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.MED_SPACE + VxlGUI.MED_BAR + VxlGUI.SM_SPACE + template_height);
		_rect_select_content = VxlGUI.GetScrollViewRect(_edgelist, _rect_selectscroll.width, _rect_selectscroll.height);
		_rect_pattemplateheader = VxlGUI.GetBelowElement(rect_left, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE + template_height);
		_rect_pattemplate = VxlGUI.GetBelowElement(rect_left, 0, template_height);

		Rect rect_right = VxlGUI.GetRightColumn(rect, VxlGUI.MED_SPACE, 0.5f);
		_rect_patheader = VxlGUI.GetAboveElement(rect_right, 0, VxlGUI.MED_BAR);
		_rect_patscroll = VxlGUI.GetSandwichedRectY(rect_right, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.SM_SPACE + VxlGUI.MED_BAR);
		_rect_patcontent = VxlGUI.GetScrollViewRect(_patternlist, _rect_patscroll.width, _rect_patscroll.height);
		_rect_patpanel = VxlGUI.GetBelowElement(rect_right, 0, VxlGUI.MED_BAR);
	}

	#region Edge List
	public void UpdateEdgeList()
	{
		//clear list
		_edges.Clear();
		//valid target check
		if (target == null) {
			return;
		}
		//fill list
		target.AddEdgesToList(_edges);
	}

	private void DrawEdgeNoneElement(Rect rect)
	{
		GUI.Label(rect, "No Edges Found.", GUI.skin.GetStyle("RightListLabel"));
	}

	private void DrawEdgeElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_edgelist.count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (_edgelist.count > 0 && _edgelist.index == index);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}

	private void DrawEdgeElement(Rect rect, int index, bool active, bool focus)
	{
		string name = _edges[index].objname;
		if (string.IsNullOrEmpty(name)) {
			name = "Invalid Name";
		}
		EditorGUI.LabelField(rect, name, GUI.skin.GetStyle("RightSelectableLabel"));
	}
	#endregion

	#region Pattern List
	public void UpdatePatternList()
	{
		//clear list
		_patterns.Clear();
		//valid target check
		if (target == null) {
			return;
		}
		int index = _edgelist.index;
		if (_edges.Count <= 0 || index < 0 || index >= _edgelist.count) {
			return;
		}
		//fill list
		target.AddEdgePatternsToList(_edges[index], _patterns);
	}

	private float PatternHeight(int index)
	{
		if (_patternlist.count > 0) {
			return (2 * VxlGUI.MED_PAD) + (7 * VxlGUI.MED_BAR) + (6 * VxlGUI.SM_SPACE);
		}
		else {
			return VxlGUI.MED_BAR;
		}
	}

	private void DrawPatternNoneElement(Rect rect)
	{
		GUI.Label(rect, "No Patterns Found.", GUI.skin.GetStyle("RightListLabel"));
	}

	private void DrawPatternElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_patternlist.count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (_patternlist.count > 0 && _patternlist.index == index);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}

	private void DrawPatternElement(Rect rect, int index, bool active, bool focus)
	{
		Rect rect_content = VxlGUI.GetPaddedRect(rect, VxlGUI.MED_PAD);
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		//row 0
		string label;
		if (_patterns[index].vertical) {
			label = "Vertical";
		}
		else {
			label = "Horizontal";
		}
		GUI.Label(rect_row, "EdgeType: " + label, GUI.skin.GetStyle("LeftDarkLabel"));

		//row 1
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(rect_row, "Corner 0", GUI.skin.GetStyle("LeftDarkLabel"));

		//row 2
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		float label_factor = 0.5f;
		Rect left_rect = VxlGUI.GetLeftColumn(rect_row, 0, 0.5f);
		GUI.Label(VxlGUI.GetLeftColumn(left_rect, 0, label_factor), "Plugs:", GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetRightColumn(left_rect, 0, 1- label_factor), _patterns[index].p0.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		Rect right_rect = VxlGUI.GetRightColumn(rect_row, 0, 0.5f);
		GUI.Label(VxlGUI.GetLeftColumn(right_rect, 0, label_factor), "ID:", GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetRightColumn(right_rect, 0, 1 - label_factor), _patterns[index].c0.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));

		//row 3
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(rect_row, "Corner 1", GUI.skin.GetStyle("LeftDarkLabel"));

		//row 4
		rect_row = VxlGUI.GetAboveElement(rect_content, 4, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		left_rect = VxlGUI.GetLeftColumn(rect_row, 0, 0.5f);
		GUI.Label(VxlGUI.GetLeftColumn(left_rect, 0, label_factor), "Plugs:", GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetRightColumn(left_rect, 0, 1 - label_factor), _patterns[index].p1.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		right_rect = VxlGUI.GetRightColumn(rect_row, 0, 0.5f);
		GUI.Label(VxlGUI.GetLeftColumn(right_rect, 0, label_factor), "ID:", GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetRightColumn(right_rect, 0, 1 - label_factor), _patterns[index].c1.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));

		//row 5
		rect_row = VxlGUI.GetAboveElement(rect_content, 5, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(rect_row, "Voxel IDs", GUI.skin.GetStyle("LeftDarkLabel"));

		//row 6
		rect_row = VxlGUI.GetAboveElement(rect_content, 6, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		float width = rect_row.width / 4f;
		if (_patterns[index].vertical) {
			width = rect_row.width / 3f;
		}
		float smlabel_factor = 0.4f;
		//A0 or V0
		label = "A0:";
		if (_patterns[index].vertical) {
			label = "V0:";
		}
		Rect element_rect = VxlGUI.GetLeftElement(rect_row, 0, width);
		GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, smlabel_factor), label, GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetRightColumn(element_rect, 0, 1 - smlabel_factor), _patterns[index].v0.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//B0 or V1
		label = "B0:";
		if (_patterns[index].vertical) {
			label = "V1:";
		}
		element_rect = VxlGUI.GetLeftElement(rect_row, 1, width);
		GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, smlabel_factor), label, GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetRightColumn(element_rect, 0, 1 - smlabel_factor), _patterns[index].v1.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//A1 or V2
		label = "A1:";
		if (_patterns[index].vertical) {
			label = "V2:";
		}

		element_rect = VxlGUI.GetLeftElement(rect_row, 2, width);
		GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, smlabel_factor), label, GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetRightColumn(element_rect, 0, 1 - smlabel_factor), _patterns[index].v2.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		int v3 = _addpat.v3;
		if (!_patterns[index].vertical) {
			//B1 or V3
			element_rect = VxlGUI.GetLeftElement(rect_row, 3, width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, smlabel_factor), "B1:", GUI.skin.GetStyle("RightDarkLabel"));
			GUI.Label(VxlGUI.GetRightColumn(element_rect, 0, 1 - smlabel_factor), _patterns[index].v3.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		}

	}
	#endregion
}