﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewVertexDisplay : PanelGUI
{
	public int superselected;
	public int selected;

	private Vector2 _scroll;

	public PreviewVertexDisplay()
	{
		superselected = -1;
		selected = 0;
		_scroll = Vector2.zero;
	}
	public override void Enable()
	{
		_scroll = Vector2.zero;
	}

	public override void Disable()
	{
		_scroll = Vector2.zero;
	}

	public override void DrawGUI(Rect rect)
	{
		if (selected <= 0 || superselected < 0) {
			return;
		}
		//calculate rects and height
		float height = (2 * VxlGUI.MED_BAR) + VxlGUI.SM_SPACE;
		Rect superrect = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR);
		Rect selectrect = VxlGUI.GetAboveElement(rect, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
		Rect content_rect = VxlGUI.GetScrollViewRect(rect.width, rect.height, height);

		_scroll = GUI.BeginScrollView(rect, _scroll, content_rect);
		GUI.Label(VxlGUI.GetAboveElement(content_rect, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0), "Selected Vertex: " + superselected.ToString(), GUI.skin.GetStyle("SecondarySuperLabel"));
		GUI.Label(VxlGUI.GetAboveElement(content_rect, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0), "Selected: " + selected.ToString(), GUI.skin.GetStyle("SecondarySelectLabel"));
		GUI.EndScrollView();
	}
}
