using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class HexagonMapPanel : PanelGUI
{
	public MapObject target;

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
	private List<HexagonDesign> _faces;
	private ReorderableList _facelist;

	//Pattern List
	private HexagonPattern _addpat;
	private List<HexagonPattern> _patterns;
	private ReorderableList _patternlist;


	public HexagonMapPanel(string title)
	{
		_title = title;
		_faces = new List<HexagonDesign>();
		_patterns = new List<HexagonPattern>();
	}

	public override void Enable()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_addpat = HexagonPattern.empty;
		_selectscroll = Vector2.zero;
		_patscroll = Vector2.zero;

		_facelist = new ReorderableList(_faces, typeof(HexagonDesign), false, false, false, false);
		_facelist.showDefaultBackground = false;
		_facelist.headerHeight = 0;
		_facelist.footerHeight = 0;
		_facelist.elementHeight = VxlGUI.MED_BAR;
		_facelist.drawNoneElementCallback += DrawFaceNoneElement;
		_facelist.drawElementCallback += DrawFaceElement;
		_facelist.drawElementBackgroundCallback += DrawFaceElementBackground;

		_patternlist = new ReorderableList(_patterns, typeof(HexagonPattern), false, false, false, false);
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
		bool invalid_exists = target == null || target.HexagonPatternExists(_addpat);
		EditorGUI.BeginDisabledGroup(target == null || invalid_addpat || invalid_faceindex || invalid_exists);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_patpanel, 0, button_width), "Add", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Add Hexagon Pattern");
				target.AddHexagonPattern(_faces[_facelist.index], _addpat);
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
				Undo.RecordObject(target, "Delete Hexagon Pattern");
				target.DeleteHexagonPattern(_faces[_facelist.index], _patterns[_patternlist.index]);
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
		int c4 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 2, Mathf.Min(two / 3f, 40)), _addpat.c4_p);
		int c5 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 1, Mathf.Min(two / 3f, 40)), _addpat.c5_p);
		int c0 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 0, Mathf.Min(two / 3f, 40)), _addpat.c0_p);
		//row 1 - b1 b0
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Below:", GUI.skin.GetStyle("LeftLightHeader"));
		int c3 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 2, Mathf.Min(two / 3f, 40)), _addpat.c3_p);
		int c2 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 1, Mathf.Min(two / 3f, 40)), _addpat.c2_p);
		int c1 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 0, Mathf.Min(two / 3f, 40)), _addpat.c1_p);
		//row 2 - vin vout
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Voxels:", GUI.skin.GetStyle("LeftLightHeader"));
		int vb = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 1, Mathf.Min(two / 2f, 40)), _addpat.vb);
		int va = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 0, Mathf.Min(two / 2f, 40)), _addpat.va);
		//row 3 - flipx flipy
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		bool flipx = EditorGUI.Foldout(VxlGUI.GetLeftElement(rect_row, 0, two), _addpat.xflip, "FlipX", true, GUI.skin.GetStyle("LightFoldout"));
		bool flipy = EditorGUI.Foldout(VxlGUI.GetLeftElement(rect_row, 1, two), _addpat.yflip, "FlipY", true, GUI.skin.GetStyle("LightFoldout"));
		//apply change
		if (EditorGUI.EndChangeCheck()) {
			_addpat = new HexagonPattern(flipx, flipy, c0, c1, c2, c3, c4, c5, va, vb);
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
		target.AddHexagonsToList(_faces);
	}

	private void DrawFaceNoneElement(Rect rect)
	{
		GUI.Label(rect, "No Hexagons Found.", GUI.skin.GetStyle("RightListLabel"));
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
		target.AddHexagonPatternsToList(_faces[index], _patterns);
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
		//row 0 - (4) (5) (0)
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Plugs:", GUI.skin.GetStyle("LeftLightHeader"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 2, two / 3f), _patterns[index].c4_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 1, two / 3f), _patterns[index].c5_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 0, two / 3f), _patterns[index].c0_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//row 1 - (3) (2) (1)
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetRightElement(rect_row, 2, two / 3f), _patterns[index].c3_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 1, two / 3f), _patterns[index].c2_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 0, two / 3f), _patterns[index].c1_p.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//row 2 - above below
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Voxels:", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 1, two / 2f), _patterns[index].vb.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 0, two / 2f), _patterns[index].va.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//row 3 - flipx flipy
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.SM_BAR);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two / 2f), "FlipX", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, two / 2f), _patterns[index].xflip.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 2, two / 2f), "FlipY", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 3, two / 2f), _patterns[index].yflip.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
	}
	#endregion
}
