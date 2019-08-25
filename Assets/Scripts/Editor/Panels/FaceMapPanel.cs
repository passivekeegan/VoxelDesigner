using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class FaceMapPanel : PanelGUI
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
	private List<FaceDesign> _faces;
	private ReorderableList _facelist;

	//Pattern List
	private FacePattern _addpat;
	private List<FacePattern> _patterns;
	private ReorderableList _patternlist;


	public FaceMapPanel(string title)
	{
		_title = title;
		_faces = new List<FaceDesign>();
		_patterns = new List<FacePattern>();
	}

	public override void Enable()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_addpat = FacePattern.emptyHexagon;
		_selectscroll = Vector2.zero;
		_patscroll = Vector2.zero;

		_facelist = new ReorderableList(_faces, typeof(FaceDesign), false, false, false, false);
		_facelist.showDefaultBackground = false;
		_facelist.headerHeight = 0;
		_facelist.footerHeight = 0;
		_facelist.elementHeight = VxlGUI.MED_BAR;
		_facelist.drawNoneElementCallback += DrawFaceNoneElement;
		_facelist.drawElementCallback += DrawFaceElement;
		_facelist.drawElementBackgroundCallback += DrawFaceElementBackground;

		_patternlist = new ReorderableList(_patterns, typeof(FacePattern), false, false, false, false);
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
		_facelist = null;
		_faces.Clear();
	}

	public override void DrawGUI(Rect rect)
	{
		//update both visual lists
		UpdateFaceList();
		UpdatePatternList();
		//update layout rects
		UpdateLayoutRects(rect);

		//draw key selection header
		VxlGUI.DrawRect(_rect_selectheader, "DarkGradient");
		GUI.Label(_rect_selectheader, _title, GUI.skin.GetStyle("LeftLightHeader"));
		//draw key selection list
		VxlGUI.DrawRect(_rect_selectscroll, "DarkWhite");
		_selectscroll = GUI.BeginScrollView(_rect_selectscroll, _selectscroll, _rect_select_content);
		_facelist.DoList(_rect_select_content);
		GUI.EndScrollView();
		//draw add pattern template
		DrawAddPatternPanel();

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

		bool invalid_addpat =  _addpat.IsEmpty() || !_addpat.IsValid();
		bool invalid_faceindex = _facelist.count <= 0 || _facelist.index < 0 || _facelist.index >= _facelist.count;
		bool invalid_exists = target == null || target.FacePatternExists(_addpat);
		EditorGUI.BeginDisabledGroup(target == null || invalid_addpat || invalid_faceindex || invalid_exists);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_patpanel, 0, button_width), "Add", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Add Face Pattern");
				target.AddFacePattern(_faces[_facelist.index], _addpat);
				_repaint_menu = false;
				//set target dirty
				EditorUtility.SetDirty(target);
				//update pattern list
				UpdatePatternList();
			}

		}
		EditorGUI.EndDisabledGroup();

		bool invalid_patindex = _patternlist.count <= 0 || _patternlist.index < 0 || _patternlist.index >= _patternlist.count;
		EditorGUI.BeginDisabledGroup(target == null || invalid_faceindex || invalid_patindex);
		if (GUI.Button(VxlGUI.GetLeftElement(_rect_patpanel, 0, button_width), "Delete", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Delete Face Pattern");
				target.DeleteFacePattern((FaceDesign)_faces[_facelist.index], _patterns[_patternlist.index]);
				_repaint_menu = false;
				//set target dirty
				EditorUtility.SetDirty(target);
				//update pattern list
				UpdatePatternList();
			}
		}
		EditorGUI.EndDisabledGroup();
	}

	private void DrawAddPatternPanel()
	{
		EditorGUI.BeginChangeCheck();
		VxlGUI.DrawRect(_rect_pattemplateheader, "DarkGradient");
		GUI.Label(_rect_pattemplateheader, "Add Template", GUI.skin.GetStyle("LeftLightHeader"));
		VxlGUI.DrawRect(_rect_pattemplate, "DarkWhite");
		Rect rect_content = VxlGUI.GetPaddedRect(_rect_pattemplate, VxlGUI.MED_PAD);
		//row 0
		int type_index = 0;
		if (_addpat.hexagon) {
			type_index = 1;
		}
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftColumn(rect_row, 0, 0.5f), "Face Type:", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		type_index = EditorGUI.Popup(VxlGUI.GetRightColumn(rect_row, 0, 0.5f), type_index, new string[] { "Rect", "Hexagon"}, GUI.skin.GetStyle("DarkDropDown"));
		//row 1
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(rect_row, "Corner Plugs:", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		//row 2-3
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		float label_factor = 0.4f;
		int p0, p1, p2, p3, p4, p5;
		float element_width;
		Rect element_rect;
		if (_addpat.hexagon) {
			element_width = rect_row.width / 3f;
			//P4
			element_rect = VxlGUI.GetLeftElement(rect_row, 0, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P4", GUI.skin.GetStyle("MidDarkLabel"));
			p4 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p4, GUI.skin.GetStyle("DarkNumberField"));
			//P5
			element_rect = VxlGUI.GetLeftElement(rect_row, 1, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P5", GUI.skin.GetStyle("MidDarkLabel"));
			p5 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p5, GUI.skin.GetStyle("DarkNumberField"));
			//P0
			element_rect = VxlGUI.GetLeftElement(rect_row, 2, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P0", GUI.skin.GetStyle("MidDarkLabel"));
			p0 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p0, GUI.skin.GetStyle("DarkNumberField"));
			//row 3
			rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
			//P3
			element_rect = VxlGUI.GetLeftElement(rect_row, 0, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P3", GUI.skin.GetStyle("MidDarkLabel"));
			p3 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p3, GUI.skin.GetStyle("DarkNumberField"));
			//P2
			element_rect = VxlGUI.GetLeftElement(rect_row, 1, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P2", GUI.skin.GetStyle("MidDarkLabel"));
			p2 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p2, GUI.skin.GetStyle("DarkNumberField"));
			//P1
			element_rect = VxlGUI.GetLeftElement(rect_row, 2, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P1", GUI.skin.GetStyle("MidDarkLabel"));
			p1 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p1, GUI.skin.GetStyle("DarkNumberField"));
		}
		else {
			element_width = rect_row.width / 2f;
			//P2
			element_rect = VxlGUI.GetLeftElement(rect_row, 0, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P2", GUI.skin.GetStyle("MidDarkLabel"));
			p2 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p2, GUI.skin.GetStyle("DarkNumberField"));
			//P0
			element_rect = VxlGUI.GetLeftElement(rect_row, 1, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P0", GUI.skin.GetStyle("MidDarkLabel"));
			p0 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p0, GUI.skin.GetStyle("DarkNumberField"));
			//row 3
			rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
			//P3
			element_rect = VxlGUI.GetLeftElement(rect_row, 0, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P3", GUI.skin.GetStyle("MidDarkLabel"));
			p3 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p3, GUI.skin.GetStyle("DarkNumberField"));
			//P1
			element_rect = VxlGUI.GetLeftElement(rect_row, 1, element_width);
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "P1", GUI.skin.GetStyle("MidDarkLabel"));
			p1 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.p1, GUI.skin.GetStyle("DarkNumberField"));
			p4 = 0;
			p5 = 0;
		}
		//row 4
		rect_row = VxlGUI.GetAboveElement(rect_content, 4, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(rect_row, "Voxel IDs:", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		//row 5
		int v0 = _addpat.v0;
		int v1 = _addpat.v1;
		element_width = rect_row.width / 2f;
		rect_row = VxlGUI.GetAboveElement(rect_content, 5, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		//Inside Voxel ID
		element_rect = VxlGUI.GetLeftElement(rect_row, 0, element_width);
		if (_addpat.hexagon) {
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "Above", GUI.skin.GetStyle("MidDarkLabel"));
			v1 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.v1, GUI.skin.GetStyle("DarkNumberField"));
		}
		else {
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "In", GUI.skin.GetStyle("MidDarkLabel"));
			v0 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.v0, GUI.skin.GetStyle("DarkNumberField"));
		}
		
		//Outside Voxel ID
		element_rect = VxlGUI.GetLeftElement(rect_row, 1, element_width);
		if (_addpat.hexagon) {
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "Below", GUI.skin.GetStyle("MidDarkLabel"));
			v0 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.v0, GUI.skin.GetStyle("DarkNumberField"));
		}
		else {
			GUI.Label(VxlGUI.GetLeftColumn(element_rect, 0, label_factor), "Out", GUI.skin.GetStyle("MidDarkLabel"));
			v1 = EditorGUI.IntField(VxlGUI.GetRightColumn(element_rect, 0, 1 - label_factor), _addpat.v1, GUI.skin.GetStyle("DarkNumberField"));
		}
		//apply change
		if (EditorGUI.EndChangeCheck()) {
			if (type_index == 1) {
				_addpat = new FacePattern(p0, p1, p2, p3, p4, p5, v0, v1);
			}
			else {
				_addpat = new FacePattern(p0, p1, p2, p3, v0, v1);
			}
			_repaint_menu = false;
		}
		
	}

	private void UpdateLayoutRects(Rect rect)
	{
		Rect rect_left = VxlGUI.GetLeftColumn(rect, VxlGUI.MED_SPACE, 0.5f);
		float template_height = (2 * VxlGUI.MED_PAD) + (6 * VxlGUI.MED_BAR) + (5 * VxlGUI.SM_SPACE);
		_rect_selectheader = VxlGUI.GetAboveElement(rect_left, 0, VxlGUI.MED_BAR);
		_rect_selectscroll = VxlGUI.GetSandwichedRectY(rect_left, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.MED_SPACE + VxlGUI.MED_BAR + VxlGUI.SM_SPACE + template_height);
		_rect_select_content = VxlGUI.GetScrollViewRect(_facelist, _rect_selectscroll.width, _rect_selectscroll.height);
		_rect_pattemplateheader = VxlGUI.GetBelowElement(rect_left, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE + template_height);
		_rect_pattemplate = VxlGUI.GetBelowElement(rect_left, 0, template_height);
		
		Rect rect_right = VxlGUI.GetRightColumn(rect, VxlGUI.MED_SPACE, 0.5f);
		_rect_patheader = VxlGUI.GetAboveElement(rect_right, 0, VxlGUI.MED_BAR);
		_rect_patscroll = VxlGUI.GetSandwichedRectY(rect_right, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.SM_SPACE + VxlGUI.MED_BAR);
		_rect_patcontent = VxlGUI.GetScrollViewRect(_patternlist, _rect_patscroll.width, _rect_patscroll.height);
		_rect_patpanel = VxlGUI.GetBelowElement(rect_right, 0, VxlGUI.MED_BAR);
	}

	#region Face List
	public void UpdateFaceList()
	{
		//clear list
		_faces.Clear();
		//valid target check
		if (target == null) {
			return;
		}
		//fill list
		target.AddFacesToList(_faces);
	}

	private void DrawFaceNoneElement(Rect rect)
	{
		GUI.Label(rect, "No Faces Found.", GUI.skin.GetStyle("RightListLabel"));
	}

	private void DrawFaceElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_facelist.count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (_facelist.count > 0 && _facelist.index == index);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}

	private void DrawFaceElement(Rect rect, int index, bool active, bool focus)
	{
		string name = _faces[index].objname;
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
		int index = _facelist.index;
		if (_faces.Count <= 0 || index < 0 || index >= _facelist.count) {
			return;
		}
		//fill list
		target.AddFacePatternsToList(_faces[index], _patterns);
	}

	private float PatternHeight(int index)
	{
		if (_patternlist.count > 0) {
			return (6 * VxlGUI.SM_BAR) + (2 * VxlGUI.MED_PAD);
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
		//row 0
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.SM_BAR);
		string label;
		if (_patterns[index].hexagon) {
			label = "Hexagon";
		}
		else {
			label = "Rect";
		}
		GUI.Label(rect_row, "Face Type: " + label, GUI.skin.GetStyle("LeftDarkLabel"));
		//row 1
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.SM_BAR);
		GUI.Label(rect_row, label + "Plugs:", GUI.skin.GetStyle("LeftDarkLabel"));
		//row 2
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.SM_BAR);
		float element_width;
		if (_patterns[index].hexagon) {
			element_width = rect_row.width / 3f;
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, element_width), "P4: " + _patterns[index].p4, GUI.skin.GetStyle("MidDarkLabel"));
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, element_width), "P5: " + _patterns[index].p5, GUI.skin.GetStyle("MidDarkLabel"));
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 2, element_width), "P0: " + _patterns[index].p0, GUI.skin.GetStyle("MidDarkLabel"));
			//row 3
			rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.SM_BAR);
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, element_width), "P3: " + _patterns[index].p3, GUI.skin.GetStyle("MidDarkLabel"));
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, element_width), "P2: " + _patterns[index].p2, GUI.skin.GetStyle("MidDarkLabel"));
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 2, element_width), "P1: " + _patterns[index].p1, GUI.skin.GetStyle("MidDarkLabel"));
		}
		else {
			element_width = rect_row.width / 2f;
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, element_width), "P2: " + _patterns[index].p2, GUI.skin.GetStyle("MidDarkLabel"));
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, element_width), "P0: " + _patterns[index].p0, GUI.skin.GetStyle("MidDarkLabel"));
			//row 3
			rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.SM_BAR);
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, element_width), "P3: " + _patterns[index].p3, GUI.skin.GetStyle("MidDarkLabel"));
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, element_width), "P1: " + _patterns[index].p1, GUI.skin.GetStyle("MidDarkLabel"));
		}
		//row 4
		rect_row = VxlGUI.GetAboveElement(rect_content, 4, VxlGUI.SM_BAR);
		GUI.Label(rect_row, "Voxel IDs:", GUI.skin.GetStyle("LeftDarkLabel"));
		//row 5
		rect_row = VxlGUI.GetAboveElement(rect_content, 5, VxlGUI.SM_BAR);
		element_width = rect_row.width / 2f;
		if (_patterns[index].hexagon) {
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, element_width), "Above: " + _patterns[index].v1, GUI.skin.GetStyle("MidDarkLabel"));
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, element_width), "Below: " + _patterns[index].v0, GUI.skin.GetStyle("MidDarkLabel"));
		}
		else {
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, element_width), "Inside: " + _patterns[index].v0, GUI.skin.GetStyle("MidDarkLabel"));
			GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, element_width), "Outside: " + _patterns[index].v1, GUI.skin.GetStyle("MidDarkLabel"));
		}
	}
	#endregion
}