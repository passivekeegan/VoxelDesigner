using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class RectMapPanel : PanelGUI
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
	private List<RectFaceDesign> _faces;
	private ReorderableList _facelist;

	//Pattern List
	private RectFacePattern _addpat;
	private List<RectFacePattern> _patterns;
	private ReorderableList _patternlist;


	public RectMapPanel(string title)
	{
		_title = title;
		_faces = new List<RectFaceDesign>();
		_patterns = new List<RectFacePattern>();
	}

	public override void Enable()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_addpat = RectFacePattern.empty;
		_selectscroll = Vector2.zero;
		_patscroll = Vector2.zero;

		_facelist = new ReorderableList(_faces, typeof(RectFaceDesign), false, false, false, false);
		_facelist.showDefaultBackground = false;
		_facelist.headerHeight = 0;
		_facelist.footerHeight = 0;
		_facelist.elementHeight = VxlGUI.MED_BAR;
		_facelist.drawNoneElementCallback += DrawFaceNoneElement;
		_facelist.drawElementCallback += DrawFaceElement;
		_facelist.drawElementBackgroundCallback += DrawFaceElementBackground;

		_patternlist = new ReorderableList(_patterns, typeof(RectFacePattern), false, false, false, false);
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

		bool invalid_addpat = _addpat.IsEmpty() || !_addpat.IsValid();
		bool invalid_faceindex = _facelist.count <= 0 || _facelist.index < 0 || _facelist.index >= _facelist.count;
		bool invalid_exists = target == null || target.RectPatternExists(_addpat);
		EditorGUI.BeginDisabledGroup(target == null || invalid_addpat || invalid_faceindex || invalid_exists);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_patpanel, 0, button_width), "Add", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Add Rect Pattern");
				target.AddRectPattern(_faces[_facelist.index], _addpat);
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
				Undo.RecordObject(target, "Delete Rect Pattern");
				target.DeleteRectPattern(_faces[_facelist.index], _patterns[_patternlist.index]);
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
		GUI.Label(_rect_pattemplateheader, "Add Rect Pattern", GUI.skin.GetStyle("LeftLightHeader"));
		VxlGUI.DrawRect(_rect_pattemplate, "DarkWhite");
		Rect rect_content = VxlGUI.GetPaddedRect(_rect_pattemplate, VxlGUI.MED_PAD);

		float two = rect_content.width / 2f;
		//row 0 - a1 a0
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Above:", GUI.skin.GetStyle("LeftLightHeader"));
		int a1 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 1, Mathf.Min(two / 2f, 40)), _addpat.a1_p);
		int a0 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 0, Mathf.Min(two / 2f, 40)), _addpat.a0_p);
		//row 1 - b1 b0
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Below:", GUI.skin.GetStyle("LeftLightHeader"));
		int b1 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 1, Mathf.Min(two / 2f, 40)), _addpat.b1_p);
		int b0 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 0, Mathf.Min(two / 2f, 40)), _addpat.b0_p);
		//row 2 - vin vout
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Voxels:", GUI.skin.GetStyle("LeftLightHeader"));
		int vin = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 1, Mathf.Min(two / 2f, 40)), _addpat.vin);
		int vout = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 0, Mathf.Min(two / 2f, 40)), _addpat.vout);
		//row 3 - flipx flipy
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		bool flipx = EditorGUI.Foldout(VxlGUI.GetLeftElement(rect_row, 0, two), _addpat.xflip, "FlipX", true, GUI.skin.GetStyle("LightFoldout"));
		bool flipy = EditorGUI.Foldout(VxlGUI.GetLeftElement(rect_row, 1, two), _addpat.yflip, "FlipY", true, GUI.skin.GetStyle("LightFoldout"));
		//apply change
		if (EditorGUI.EndChangeCheck()) {
			_addpat = new RectFacePattern(flipx, flipy, a0, b0, a1, b1, vin, vout);
			_repaint_menu = false;
		}
	}

	private void UpdateLayoutRects(Rect rect)
	{
		Rect rect_left = VxlGUI.GetLeftColumn(rect, VxlGUI.MED_SPACE, 0.5f);
		float template_height = (2 * VxlGUI.MED_PAD) + (4 * VxlGUI.MED_BAR) + (3 * VxlGUI.SM_SPACE);
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
		target.AddRectFacesToList(_faces);
	}

	private void DrawFaceNoneElement(Rect rect)
	{
		GUI.Label(rect, "No Rects Found.", GUI.skin.GetStyle("RightListLabel"));
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
		target.AddRectPatternsToList(_faces[index], _patterns);
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

		float two = rect_content.width / 2f;
		//row 0 - a1 a0
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Above:", GUI.skin.GetStyle("LeftLightHeader"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 1, two / 2f), _patterns[index].a1_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 0, two / 2f), _patterns[index].a0_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//row 1 - b1 b0
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Below:", GUI.skin.GetStyle("LeftLightHeader"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 1, two / 2f), _patterns[index].b1_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 0, two / 2f), _patterns[index].b0_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//row 2 - vin vout
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Voxels:", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 1, two / 2f), _patterns[index].vin.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 0, two / 2f), _patterns[index].vout.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//row 3 - flipx flipy
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two / 2f), "FlipX", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, two / 2f), _patterns[index].xflip.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 2, two / 2f), "FlipY", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 3, two / 2f), _patterns[index].yflip.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
	}
	#endregion
}
