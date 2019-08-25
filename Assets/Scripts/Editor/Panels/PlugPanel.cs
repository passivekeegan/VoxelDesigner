using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class PlugPanel : PanelGUI
{
	public VoxelComponent target;
	public List<int> plugs;

	private Rect _rect_title;
	private Rect _rect_scroll;
	private Rect _rect_content;

	private int _plug_cnt;
	private Vector2 _scroll;
	private List<int> _plugs;
	private List<string> _plug_labels;
	private ReorderableList _pluglist;

	public PlugPanel(string title, string[] labels, int plug_count)
	{
		_title = title;
		_plug_cnt = plug_count;
		_plug_labels = new List<string>(_plug_cnt);
		for (int k = 0;k < _plug_cnt;k++) {
			string label = "";
			if (labels != null && k < labels.Length && labels[k] != null) {
				label = labels[k];
			}
			_plug_labels.Add(label);
		}
		_plugs = new List<int>(_plug_cnt);
	}

	public override void Enable()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_scroll = Vector2.zero;
		_pluglist = new ReorderableList(_plugs, typeof(int), false, false, false, false);
		_pluglist.showDefaultBackground = false;
		_pluglist.headerHeight = 0;
		_pluglist.footerHeight = 0;
		_pluglist.elementHeight = VxlGUI.MED_BAR;
		_pluglist.drawNoneElementCallback += DrawPlugNoneElement;
		_pluglist.drawElementCallback += DrawPlugElement;
	}

	public override void Disable()
	{
		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
		_pluglist = null;
		_plugs.Clear();
	}

	public override void DrawGUI(Rect rect)
	{
		//update plugs
		UpdatePlugs();
		//update layout rects
		UpdateRects(rect);

		EditorGUI.BeginDisabledGroup(target == null || plugs == null);
		//Draw title
		VxlGUI.DrawRect(_rect_title, "DarkGradient");
		GUI.Label(_rect_title, _title, GUI.skin.GetStyle("LeftLightHeader"));
		//draw content
		VxlGUI.DrawRect(_rect_scroll, "DarkWhite");
		_scroll = GUI.BeginScrollView(_rect_scroll, _scroll, _rect_content);
		_pluglist.DoList(_rect_content);
		GUI.EndScrollView();
		EditorGUI.EndDisabledGroup();
	}

	private void UpdatePlugs()
	{
		//clear plugs
		_plugs.Clear();
		//check valid target
		if (target == null || plugs == null || plugs.Count != _plug_cnt) {
			_scroll = Vector2.zero;
			_pluglist.index = -1;
			return;
		}
		//fill list
		_plugs.AddRange(plugs);
	}

	private void UpdateRects(Rect rect)
	{
		_rect_title = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR);
		_rect_scroll = VxlGUI.GetSandwichedRectY(rect, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, 0);
		_rect_content = VxlGUI.GetScrollViewRect(_pluglist, _rect_scroll.width, _rect_scroll.height);
	}

	#region Plug ReorderableList
	private void DrawPlugNoneElement(Rect rect)
	{
		GUI.Label(rect, "No " + _title + " Found.", GUI.skin.GetStyle("RightListLabel"));
	}
	private void DrawPlugElement(Rect rect, int index, bool active, bool focus)
	{
		rect = VxlGUI.GetPaddedRect(rect, 1);
		float field_width = Mathf.Min(100f, (rect.width - VxlGUI.SM_SPACE) / 2f);
		//draw left label
		GUI.Label(VxlGUI.GetSandwichedRectX(rect, 0, field_width + VxlGUI.SM_SPACE), _plug_labels[index], GUI.skin.GetStyle("LeftListLabel"));
		//draw right int field
		EditorGUI.BeginChangeCheck();
		int plug = Mathf.Max(EditorGUI.IntField(VxlGUI.GetRightElement(rect, 0, field_width), _plugs[index], GUI.skin.GetStyle("DarkNumberField")), -1);
		if ((EditorGUI.EndChangeCheck() || _plugs[index] != plug)) {
			if (target == null || plugs == null || plugs.Count != _plug_cnt) {
				return;
			}
			Undo.RecordObject(target, "Modify Plugs");
			plugs[index] = plug;
			_update_mesh = true;
			_render_mesh = true;
			_repaint_menu = true;
			//dirty target object
			EditorUtility.SetDirty(target);
		}
	}
	#endregion
}
