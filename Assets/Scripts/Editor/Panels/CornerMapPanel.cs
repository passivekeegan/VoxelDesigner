using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public class CornerMapPanel : PanelGUI
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
	private List<CornerDesign> _corners;
	private ReorderableList _cornerlist;

	//Pattern List
	private CornerPattern _addpat;
	private List<CornerPattern> _patterns;
	private ReorderableList _patternlist;

	public CornerMapPanel(string title)
	{
		_title = title;
		_corners = new List<CornerDesign>();
		_patterns = new List<CornerPattern>();
	}

	public override void Enable()
	{
		_addpat = new CornerPattern(0, 0, 0, 0, 0, 0);
		_selectscroll = Vector2.zero;
		_patscroll = Vector2.zero;

		_cornerlist = new ReorderableList(_corners, typeof(CornerDesign), false, false, false, false);
		_cornerlist.showDefaultBackground = false;
		_cornerlist.headerHeight = 0;
		_cornerlist.footerHeight = 0;
		_cornerlist.elementHeight = VxlGUI.MED_BAR;
		_cornerlist.drawNoneElementCallback += DrawCornerNoneElement;
		_cornerlist.drawElementCallback += DrawCornerElement;
		_cornerlist.drawElementBackgroundCallback += DrawCornerElementBackground;
		
		_patternlist = new ReorderableList(_patterns, typeof(CornerPattern), false, false, false, false);
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
		target = null;

		_patternlist = null;
		_patterns.Clear();
		_cornerlist = null;
		_corners.Clear();
	}

	public override void DrawGUI(Rect rect)
	{
		//update both visual lists
		UpdateCornerList();
		UpdatePatternList();
		//update layout rects
		UpdateLayoutRects(rect);

		//draw key selection header
		VxlGUI.DrawRect(_rect_selectheader, "DarkGradient");
		GUI.Label(_rect_selectheader, _title, GUI.skin.GetStyle("LeftLightHeader"));
		//draw key selection list
		VxlGUI.DrawRect(_rect_selectscroll, "DarkWhite");
		_selectscroll = GUI.BeginScrollView(_rect_selectscroll, _selectscroll, _rect_select_content);
		_cornerlist.DoList(_rect_select_content);
		GUI.EndScrollView();
		//draw add pattern template
		VxlGUI.DrawRect(_rect_pattemplateheader, "DarkGradient");
		GUI.Label(_rect_pattemplateheader, "Add Template", GUI.skin.GetStyle("LeftLightHeader"));
		VxlGUI.DrawRect(_rect_pattemplate, "DarkWhite");
		Rect rect_content = VxlGUI.GetPaddedRect(_rect_pattemplate, VxlGUI.MED_PAD);

		EditorGUI.BeginChangeCheck();
		//row 0
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.MED_BAR);
		float element_width = (rect_row.width / 3f) - (2 * VxlGUI.SM_SPACE);
		GUI.Label(rect_row, "Above IDs", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		//row 1
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.MED_BAR);
		int a0 = EditorGUI.IntField(VxlGUI.GetLeftElement(rect_row, 0, element_width, VxlGUI.SM_SPACE, 0), _addpat.a0, GUI.skin.GetStyle("DarkNumberField"));
		int a1 = EditorGUI.IntField(VxlGUI.GetLeftElement(rect_row, 1, element_width, VxlGUI.SM_SPACE, 0), _addpat.a1, GUI.skin.GetStyle("DarkNumberField"));
		int a2 = EditorGUI.IntField(VxlGUI.GetLeftElement(rect_row, 2, element_width, VxlGUI.SM_SPACE, 0), _addpat.a2, GUI.skin.GetStyle("DarkNumberField"));
		//row 3
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR);
		GUI.Label(rect_row, "Below IDs", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		//row 4
		rect_row = VxlGUI.GetAboveElement(rect_content, 4, VxlGUI.MED_BAR);
		int b0 = EditorGUI.IntField(VxlGUI.GetLeftElement(rect_row, 0, element_width, VxlGUI.SM_SPACE, 0), _addpat.b0, GUI.skin.GetStyle("DarkNumberField"));
		int b1 = EditorGUI.IntField(VxlGUI.GetLeftElement(rect_row, 1, element_width, VxlGUI.SM_SPACE, 0), _addpat.b1, GUI.skin.GetStyle("DarkNumberField"));
		int b2 = EditorGUI.IntField(VxlGUI.GetLeftElement(rect_row, 2, element_width, VxlGUI.SM_SPACE, 0), _addpat.b2, GUI.skin.GetStyle("DarkNumberField"));
		
		//apply change
		if (EditorGUI.EndChangeCheck()) {
			_addpat = new CornerPattern(a0, a1, a2, b0, b1, b2);
			repaint = true;
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

		bool invalid_corner = _cornerlist.count <= 0 || _cornerlist.index < 0 || _cornerlist.index >= _cornerlist.count;
		bool invalid_exists = target != null && target.CornerPatternExists(_addpat);
		EditorGUI.BeginDisabledGroup(target == null || _addpat.IsEmpty() || invalid_corner || invalid_exists);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_patpanel, 0, button_width), "Add", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Add Corner Pattern");
				target.AddCornerPattern(_corners[_cornerlist.index], _addpat);
				update = true;
				repaint = true;
				//dirty target object
				EditorUtility.SetDirty(target);
				//update pattern list
				UpdatePatternList();
			}
		}
		EditorGUI.EndDisabledGroup();

		bool invalid_pattern = _patternlist.count <= 0 || _patternlist.index < 0 || _patternlist.index >= _patternlist.count;
		EditorGUI.BeginDisabledGroup(target == null || invalid_pattern || invalid_corner);
		if (GUI.Button(VxlGUI.GetLeftElement(_rect_patpanel, 0, button_width), "Delete", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Delete Corner Pattern");
				target.DeleteCornerPattern(_corners[_cornerlist.index], _patterns[_patternlist.index]);
				update = true;
				repaint = true;
				//dirty target object
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
		float template_height = (2 * VxlGUI.MED_PAD) + (5 * VxlGUI.MED_BAR);
		_rect_selectheader = VxlGUI.GetAboveElement(rect_left, 0, VxlGUI.MED_BAR);
		_rect_selectscroll = VxlGUI.GetSandwichedRectY(rect_left, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.MED_SPACE + VxlGUI.MED_BAR + VxlGUI.SM_SPACE + template_height);
		_rect_select_content = VxlGUI.GetScrollViewRect(_cornerlist, _rect_selectscroll.width, _rect_selectscroll.height);
		_rect_pattemplateheader = VxlGUI.GetBelowElement(rect_left, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE + template_height);
		_rect_pattemplate = VxlGUI.GetBelowElement(rect_left, 0, template_height);

		Rect rect_right = VxlGUI.GetRightColumn(rect, VxlGUI.MED_SPACE, 0.5f);
		_rect_patheader = VxlGUI.GetAboveElement(rect_right, 0, VxlGUI.MED_BAR);
		_rect_patscroll = VxlGUI.GetSandwichedRectY(rect_right, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.SM_SPACE + VxlGUI.MED_BAR);
		_rect_patcontent = VxlGUI.GetScrollViewRect(_patternlist, _rect_patscroll.width, _rect_patscroll.height);
		_rect_patpanel = VxlGUI.GetBelowElement(rect_right, 0, VxlGUI.MED_BAR);
	}

	#region Corner List
	public void UpdateCornerList()
	{
		//clear list
		_corners.Clear();
		//valid target check
		if (target == null) {
			return;
		}
		//fill list
		target.AddCornersToList(_corners);
	}

	private void DrawCornerNoneElement(Rect rect)
	{
		GUI.Label(rect, "No Corners Found.", GUI.skin.GetStyle("RightListLabel"));
	}

	private void DrawCornerElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_cornerlist.count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (_cornerlist.count > 0 && _cornerlist.index == index);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}

	private void DrawCornerElement(Rect rect, int index, bool active, bool focus)
	{
		string name = _corners[index].objname;
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
		int index = _cornerlist.index;
		if (_corners.Count <= 0 || index < 0 || index >= _cornerlist.count) {
			return;
		}
		//fill list
		target.AddCornerPatternsToList(_corners[index], _patterns);
	}

	private float PatternHeight(int index)
	{
		if (_patternlist.count > 0) {
			return (4 * VxlGUI.SM_BAR) + (2 * VxlGUI.MED_PAD);
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
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.SM_BAR);
		float width = (rect_content.width / 3f) - (2 * VxlGUI.SM_SPACE);
		//row 0
		GUI.Label(rect_row, "Above IDs", GUI.skin.GetStyle("LeftDarkLabel"));
		//row 1
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, width, VxlGUI.SM_SPACE, 0), _patterns[index].a0.ToString(), GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, width, VxlGUI.SM_SPACE, 0), _patterns[index].a1.ToString(), GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 2, width, VxlGUI.SM_SPACE, 0), _patterns[index].a2.ToString(), GUI.skin.GetStyle("RightDarkLabel"));
		//row 2
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.SM_BAR);
		GUI.Label(rect_row, "Below IDs", GUI.skin.GetStyle("LeftDarkLabel"));
		//row 3
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, width, VxlGUI.SM_SPACE, 0), _patterns[index].b0.ToString(), GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, width, VxlGUI.SM_SPACE, 0), _patterns[index].b1.ToString(), GUI.skin.GetStyle("RightDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 2, width, VxlGUI.SM_SPACE, 0), _patterns[index].b2.ToString(), GUI.skin.GetStyle("RightDarkLabel"));
	}
	#endregion
}