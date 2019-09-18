using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PreviewMeshDisplay : PanelGUI
{
	public bool invertx;
	public bool inverty;

	private bool _expanded;
	private Vector2 _scroll;

	public override void Enable()
	{
		_scroll = Vector2.zero;
		_expanded = false;

	}

	public override void Disable()
	{
		_scroll = Vector2.zero;
	}

	public override void DrawGUI(Rect rect)
	{
		float width = Mathf.Min(200, rect.width);
		if (!_expanded) {
			//calculate rect
			rect = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR);
			//draw background
			VxlGUI.DrawRect(rect, "DarkTransparentGrey");
			//draw foldout toggle label
			EditorGUI.BeginChangeCheck();
			_expanded = EditorGUI.Foldout(rect, _expanded, "Mesh", true, GUI.skin.GetStyle("LightBoldFoldout"));
			if (EditorGUI.EndChangeCheck()) {
				_repaint_menu = true;
			}
		}
		else {
			float content_height = (2 * VxlGUI.MED_BAR) + VxlGUI.SM_SPACE;
			float panel_height = Mathf.Min(rect.height, VxlGUI.SM_SPACE + VxlGUI.MED_BAR + content_height);
			float scroll_height = Mathf.Max(0, panel_height - VxlGUI.SM_SPACE - VxlGUI.MED_BAR);
			Rect panel_rect = VxlGUI.GetAboveElement(rect, 0, panel_height);
			Rect rect_scroll = VxlGUI.GetSandwichedRectY(panel_rect, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, 0);
			Rect content_rect = VxlGUI.GetVerticalScrollViewRect(rect_scroll.width, rect_scroll.height, content_height);

			VxlGUI.DrawRect(panel_rect, "DarkTransparentGrey");
			EditorGUI.BeginChangeCheck();
			Rect row_rect = VxlGUI.GetAboveElement(panel_rect, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
			_expanded = EditorGUI.Foldout(row_rect, _expanded, "Mesh", true, GUI.skin.GetStyle("LightBoldFoldout"));
			Rect rect_content = VxlGUI.GetBelowRightElement(content_rect, 0, content_rect.width - VxlGUI.MED_BAR, 0, content_rect.height);
			if (EditorGUI.EndChangeCheck()) {
				_repaint_menu = true;
			}

			EditorGUI.BeginChangeCheck();
			_scroll = GUI.BeginScrollView(rect_scroll, _scroll, content_rect);

			//invert x
			int level = 0;
			row_rect = VxlGUI.GetAboveElement(rect_content, level, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
			invertx = EditorGUI.Foldout(row_rect, invertx, "Invert X", true, GUI.skin.GetStyle("LightFoldout"));
			level += 1;
			//invert y
			row_rect = VxlGUI.GetAboveElement(rect_content, level, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
			inverty = EditorGUI.Foldout(row_rect, inverty, "Invert Y", true, GUI.skin.GetStyle("LightFoldout"));
			level += 1;

			GUI.EndScrollView();
			if (EditorGUI.EndChangeCheck()) {
				_repaint_menu = true;
				_render_mesh = true;
				_update_mesh = true;
			}
		}
	}

	
}
