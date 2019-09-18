using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class LateralMapPanel : PanelGUI
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
	private List<LateralDesign> _edges;
	private ReorderableList _edgelist;

	//Pattern List
	private LateralPattern _addpat;
	private List<LateralPattern> _patterns;
	private ReorderableList _patternlist;


	public LateralMapPanel(string title)
	{
		_title = title;
		_edges = new List<LateralDesign>();
		_patterns = new List<LateralPattern>();
	}

	public override void Enable()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_addpat = LateralPattern.empty;
		_selectscroll = Vector2.zero;
		_patscroll = Vector2.zero;

		_edgelist = new ReorderableList(_edges, typeof(LateralDesign), false, false, false, false);
		_edgelist.showDefaultBackground = false;
		_edgelist.headerHeight = 0;
		_edgelist.footerHeight = 0;
		_edgelist.elementHeight = VxlGUI.MED_BAR;
		_edgelist.drawNoneElementCallback += DrawEdgeNoneElement;
		_edgelist.drawElementCallback += DrawEdgeElement;
		_edgelist.drawElementBackgroundCallback += DrawEdgeElementBackground;

		_patternlist = new ReorderableList(_patterns, typeof(LateralPattern), false, false, false, false);
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
		GUI.Label(_rect_pattemplateheader, "Add Lateral Edge", GUI.skin.GetStyle("LeftLightHeader"));
		VxlGUI.DrawRect(_rect_pattemplate, "DarkWhite");
		Rect rect_content = VxlGUI.GetPaddedRect(_rect_pattemplate, VxlGUI.MED_PAD);

		EditorGUI.BeginChangeCheck();
		Rect rect_row = VxlGUI.GetAboveElement(rect_content, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);

		float two = rect_row.width / 2f;
		//plugs row
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Plugs:", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		int p0 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 1, Mathf.Min(two / 2f, 40)), _addpat.p0);
		int p1 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 0, Mathf.Min(two / 2f, 40)), _addpat.p1);
		//above voxels
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Above:", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		int a0 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 1, Mathf.Min(two / 2f, 40)), _addpat.a0);
		int a1 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 0, Mathf.Min(two / 2f, 40)), _addpat.a1);
		//below voxels
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Below:", GUI.skin.GetStyle("LeftLrgDarkLabel"));
		int b0 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 1, Mathf.Min(two / 2f, 40)), _addpat.b0);
		int b1 = EditorGUI.IntField(VxlGUI.GetRightElement(rect_row, 0, Mathf.Min(two / 2f, 40)), _addpat.b1);
		//flips
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		bool flipx = EditorGUI.Foldout(VxlGUI.GetLeftElement(rect_row, 0, two), _addpat.xflip, "FlipX", true, GUI.skin.GetStyle("LightFoldout"));
		bool flipy = EditorGUI.Foldout(VxlGUI.GetLeftElement(rect_row, 1, two), _addpat.yflip, "FlipY", true, GUI.skin.GetStyle("LightFoldout"));

		//apply change
		if (EditorGUI.EndChangeCheck()) {
			_addpat = new LateralPattern(flipx, flipy, p0, p1, a0, b0, a1, b1);
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

		bool invalid_addpat = !_addpat.IsValid() || _addpat.IsEmpty();
		bool invalid_edgeindex = _edgelist.count <= 0 || _edgelist.index < 0 || _edgelist.index >= _edgelist.count;
		bool invalid_exists = target != null && target.LateralPatternExists(_addpat);
		EditorGUI.BeginDisabledGroup(target == null || invalid_addpat || invalid_edgeindex || invalid_exists);
		if (GUI.Button(VxlGUI.GetRightElement(_rect_patpanel, 0, button_width), "Add", GUI.skin.GetStyle("LightButton"))) {
			if (target != null) {
				Undo.RecordObject(target, "Add Lateral Pattern");
				target.AddLateralPattern(_edges[_edgelist.index], _addpat);
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
				Undo.RecordObject(target, "Delete Lateral Pattern");
				target.DeleteLateralPattern(_edges[_edgelist.index], _patterns[_patternlist.index]);
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
		float template_height = (2 * VxlGUI.MED_PAD) + (4 * VxlGUI.MED_BAR) + (3 * VxlGUI.SM_SPACE);
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
		target.AddLateralsToList(_edges);
	}

	private void DrawEdgeNoneElement(Rect rect)
	{
		GUI.Label(rect, "No Laterals Found.", GUI.skin.GetStyle("RightListLabel"));
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
		target.AddLateralPatternsToList(_edges[index], _patterns);
	}

	private float PatternHeight(int index)
	{
		if (_patternlist.count > 0) {
			return (2 * VxlGUI.MED_PAD) + (4 * VxlGUI.MED_BAR) + (3 * VxlGUI.SM_SPACE);
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
		float two = rect_row.width / 2f;
		//plugs row
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Plugs:", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 1, two / 2f), _patterns[index].p0.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 0, two / 2f), _patterns[index].p1.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//above voxels
		rect_row = VxlGUI.GetAboveElement(rect_content, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Above:", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 1, two / 2f), _patterns[index].a0.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 0, two / 2f), _patterns[index].a1.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//below voxels
		rect_row = VxlGUI.GetAboveElement(rect_content, 2, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two), "Below:", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 1, two / 2f), _patterns[index].b0.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetRightElement(rect_row, 0, two / 2f), _patterns[index].b1.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		//flips
		rect_row = VxlGUI.GetAboveElement(rect_content, 3, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 0, two / 2f), "FlipX", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 1, two / 2f), _patterns[index].xflip.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 2, two / 2f), "FlipY", GUI.skin.GetStyle("LeftDarkLabel"));
		GUI.Label(VxlGUI.GetLeftElement(rect_row, 3, two / 2f), _patterns[index].yflip.ToString(), GUI.skin.GetStyle("LeftDarkLabel"));
	}
	#endregion
}