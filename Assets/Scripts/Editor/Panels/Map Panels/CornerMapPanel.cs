using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public class CornerMapPanel : PanelGUI
{
	public MapObject target;

	private Rect _rect_selectheader;
	private Rect _rect_selectscroll;
	private Rect _rect_select_content;
	private Rect _rect_patheader;
	private Rect _rect_patscroll;
	private Rect _rect_patcontent;
	private Rect _rect_patpanel;
	private Vector2 _selectscroll;
	private Vector2 _patscroll;

	private Rect _rect_temparea;
	private Rect _rect_tempheader;
	private Rect _rect_voxelid;
	private Rect _rect_above;
	private Rect _rect_below;
	private Rect _rect_d1;
	private Rect _rect_d3;
	private Rect _rect_d5;
	private Rect _rect_above_d1;
	private Rect _rect_below_d1;
	private Rect _rect_above_d3;
	private Rect _rect_below_d3;
	private Rect _rect_above_d5;
	private Rect _rect_below_d5;
	private Rect _rect_flipx;
	private Rect _rect_flipy;
	private Rect _rect_tempbutton;
	

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
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
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
		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
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
		EditorGUI.BeginChangeCheck();
		VxlGUI.DrawRect(_rect_temparea, "DarkGrey");
		GUI.Label(_rect_tempheader, "Corner Pattern", "PatHeader");
		GUI.Label(_rect_voxelid, "Voxel IDs:", "LeftMedPatLabel");
		GUI.Label(_rect_above, "Above", "RightMedPatLabel");
		GUI.Label(_rect_below, "Below", "RightMedPatLabel");
		GUI.Label(_rect_d5, "D5", "MidMedPatLabel");
		GUI.Label(_rect_d1, "D1", "MidMedPatLabel");
		GUI.Label(_rect_d3, "D3", "MidMedPatLabel");
		int a0 = EditorGUI.IntField(_rect_above_d5, _addpat.a0, "MidPatIntField");
		int a1 = EditorGUI.IntField(_rect_above_d1, _addpat.a1, "MidPatIntField");
		int a2 = EditorGUI.IntField(_rect_above_d3, _addpat.a2, "MidPatIntField");
		int b0 = EditorGUI.IntField(_rect_below_d5, _addpat.b0, "MidPatIntField");
		int b1 = EditorGUI.IntField(_rect_below_d1, _addpat.b1, "MidPatIntField");
		int b2 = EditorGUI.IntField(_rect_below_d3, _addpat.b2, "MidPatIntField");
		bool xflip = EditorGUI.Foldout(_rect_flipx, _addpat.xflip, "FlipX", true, "LightFoldout");
		bool yflip = EditorGUI.Foldout(_rect_flipy, _addpat.yflip, "FlipY", true, "LightFoldout");
		//apply change
		if (EditorGUI.EndChangeCheck()) {
			_addpat = new CornerPattern(xflip, yflip, a0, a1, a2, b0, b1, b2);
			_repaint_menu = true;
		}

		

		bool invalid_corner = _cornerlist.count <= 0 || _cornerlist.index < 0 || _cornerlist.index >= _cornerlist.count;
		bool invalid_exists = target != null && target.CornerPatternExists(_addpat);
		EditorGUI.BeginDisabledGroup(target == null || _addpat.IsEmpty() || invalid_corner || invalid_exists);
		if (GUI.Button(_rect_tempbutton, "Add", GUI.skin.GetStyle("PatButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Add Corner Pattern");
				target.AddCornerPattern(_corners[_cornerlist.index], _addpat);
				_repaint_menu = false;
				//dirty target object
				EditorUtility.SetDirty(target);
				//update pattern list
				UpdatePatternList();
			}
		}
		EditorGUI.EndDisabledGroup();

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
		bool invalid_pattern = _patternlist.count <= 0 || _patternlist.index < 0 || _patternlist.index >= _patternlist.count;
		EditorGUI.BeginDisabledGroup(target == null || invalid_pattern || invalid_corner);
		if (GUI.Button(VxlGUI.GetLeftElement(_rect_patpanel, 0, button_width), "Delete", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Delete Corner Pattern");
				target.DeleteCornerPattern(_corners[_cornerlist.index], _patterns[_patternlist.index]);
				_repaint_menu = false;
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
		float template_height = (2 * VxlGUI.SM_PAD) + (6 * VxlGUI.MED_BAR) + (5 * VxlGUI.MED_SPACE) + (3 * VxlGUI.LRG_SPACE);
		_rect_selectheader = VxlGUI.GetAboveElement(rect_left, 0, VxlGUI.MED_BAR);
		_rect_selectscroll = VxlGUI.GetSandwichedRectY(rect_left, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.SM_SPACE + template_height);
		_rect_select_content = VxlGUI.GetScrollViewRect(_cornerlist, _rect_selectscroll.width, _rect_selectscroll.height);

		Rect rect_right = VxlGUI.GetRightColumn(rect, VxlGUI.MED_SPACE, 0.5f);
		_rect_patheader = VxlGUI.GetAboveElement(rect_right, 0, VxlGUI.MED_BAR);
		_rect_patscroll = VxlGUI.GetSandwichedRectY(rect_right, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, VxlGUI.SM_SPACE + VxlGUI.MED_BAR);
		_rect_patcontent = VxlGUI.GetScrollViewRect(_patternlist, _rect_patscroll.width, _rect_patscroll.height);
		_rect_patpanel = VxlGUI.GetBelowElement(rect_right, 0, VxlGUI.MED_BAR);




		_rect_temparea = VxlGUI.GetBelowElement(rect_left, 0, template_height);
		Rect rect_content = VxlGUI.GetPaddedRect(_rect_temparea, VxlGUI.SM_PAD, VxlGUI.MED_PAD);
		_rect_tempheader = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.MED_BAR, VxlGUI.MED_SPACE, 0);
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.MED_BAR, VxlGUI.MED_SPACE, VxlGUI.LRG_SPACE);
		_rect_voxelid = VxlGUI.GetSandwichedRectX(rect_row, 0, (3 * VxlGUI.INTFIELD) + (2 * VxlGUI.SM_SPACE) + VxlGUI.MED_SPACE);
		_rect_d5 = VxlGUI.GetRightElement(rect_row, 2, VxlGUI.INTFIELD, VxlGUI.SM_SPACE, 0);
		_rect_d1 = VxlGUI.GetRightElement(rect_row, 1, VxlGUI.INTFIELD, VxlGUI.SM_SPACE, 0);
		_rect_d3 = VxlGUI.GetRightElement(rect_row, 0, VxlGUI.INTFIELD, VxlGUI.SM_SPACE, 0);
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.MED_BAR, VxlGUI.MED_SPACE, VxlGUI.LRG_SPACE);
		_rect_above = VxlGUI.GetSandwichedRectX(rect_row, 0, (3 * VxlGUI.INTFIELD) + (2 * VxlGUI.SM_SPACE) + VxlGUI.MED_SPACE);
		_rect_above_d5 = VxlGUI.GetRightElement(rect_row, 2, VxlGUI.INTFIELD, VxlGUI.SM_SPACE, 0);
		_rect_above_d1 = VxlGUI.GetRightElement(rect_row, 1, VxlGUI.INTFIELD, VxlGUI.SM_SPACE, 0);
		_rect_above_d3 = VxlGUI.GetRightElement(rect_row, 0, VxlGUI.INTFIELD, VxlGUI.SM_SPACE, 0);
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR, VxlGUI.MED_SPACE, VxlGUI.LRG_SPACE);
		_rect_below = VxlGUI.GetSandwichedRectX(rect_row, 0, (3 * VxlGUI.INTFIELD) + (2 * VxlGUI.SM_SPACE) + VxlGUI.MED_SPACE);
		_rect_below_d5 = VxlGUI.GetRightElement(rect_row, 2, VxlGUI.INTFIELD, VxlGUI.SM_SPACE, 0);
		_rect_below_d1 = VxlGUI.GetRightElement(rect_row, 1, VxlGUI.INTFIELD, VxlGUI.SM_SPACE, 0);
		_rect_below_d3 = VxlGUI.GetRightElement(rect_row, 0, VxlGUI.INTFIELD, VxlGUI.SM_SPACE, 0);
		rect_row = VxlGUI.GetAboveElement(rect_content, 4, VxlGUI.MED_BAR, VxlGUI.MED_SPACE, 2 * VxlGUI.LRG_SPACE);
		float quarter = rect_row.width / 4f;
		_rect_flipx = VxlGUI.GetSandwichedRectX(VxlGUI.GetLeftColumn(rect_row, 0, 0.5f), quarter - 20, 0);
		_rect_flipy = VxlGUI.GetSandwichedRectX(VxlGUI.GetRightColumn(rect_row, 0, 0.5f), quarter - 20, 0);
		rect_row = VxlGUI.GetAboveElement(rect_content, 5, VxlGUI.MED_BAR, VxlGUI.MED_SPACE, 3 * VxlGUI.LRG_SPACE);
		_rect_tempbutton = VxlGUI.GetRightElement(rect_row, 0, 80); 
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