using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


public static class EVx
{
	public const int BAR_LARGE = 32;
	public const int SM_LINE = 10;
	public const int MED_LINE = 20;
	public const int LRG_LINE = 30;

	public const int SM_PAD = 4;
	public const int MED_PAD = 4;

	public const int SM_SPACE = 2;

	private const int VECTORLABEL_WIDTH = 16;

	public static Rect GetAboveElement(Rect rect, int index, float element_height)
	{
		return GetAboveElement(rect, index, element_height, 0, 0);
	}
	public static Rect GetAboveElement(Rect rect, int index, float element_height, float offset)
	{
		return GetAboveElement(rect, index, element_height, 0, offset);
	}
	public static Rect GetAboveElement(Rect rect, int index, float element_height, float space, float offset)
	{
		return new Rect(
			rect.x,
			rect.y + offset + (index * (element_height + space)),
			rect.width,
			element_height
		);
	}
	public static Rect GetBelowElement(Rect rect, int index, float element_height)
	{
		return GetBelowElement(rect, index, element_height, 0, 0);
	}
	public static Rect GetBelowElement(Rect rect, int index, float element_height, float offset)
	{
		return GetBelowElement(rect, index, element_height, 0, offset);
	}
	public static Rect GetBelowElement(Rect rect, int index, float element_height, float space, float offset)
	{
		return new Rect(
			rect.x,
			rect.yMax + space - offset - ((index + 1) * (element_height + space)),
			rect.width,
			element_height
		);
	}
	public static Rect GetLeftElement(Rect rect, int index, float element_width)
	{
		return GetLeftElement(rect, index, element_width, 0, 0);
	}
	public static Rect GetLeftElement(Rect rect, int index, float element_width, float offset)
	{
		return GetLeftElement(rect, index, element_width, 0, offset);
	}
	public static Rect GetLeftElement(Rect rect, int index, float element_width, float space, float offset)
	{
		return new Rect(
			rect.x + offset + (index * (element_width + space)),
			rect.y,
			element_width,
			rect.height
		);
	}
	public static Rect GetRightElement(Rect rect, int index, float element_width)
	{
		return GetRightElement(rect, index, element_width, 0, 0);
	}
	public static Rect GetRightElement(Rect rect, int index, float element_width, float offset)
	{
		return GetRightElement(rect, index, element_width, 0, offset);
	}
	public static Rect GetRightElement(Rect rect, int index, float element_width, float space, float offset)
	{
		return new Rect(
			rect.xMax + space - offset - ((index + 1) * (element_width + space)),
			rect.y,
			element_width,
			rect.height
		);
	}

	public static Rect GetAboveLeftElement(Rect rect, float xoffset, float element_width, float yoffset, float element_height)
	{
		return new Rect(
			rect.x + xoffset,
			rect.y + yoffset,
			element_width,
			element_height
		);
	}
	public static Rect GetAboveRightElement(Rect rect, float xoffset, float element_width, float yoffset, float element_height)
	{
		return new Rect(
			rect.xMax - xoffset - element_width,
			rect.y + yoffset,
			element_width,
			element_height
		);
	}
	public static Rect GetBelowLeftElement(Rect rect, float xoffset, float element_width, float yoffset, float element_height)
	{
		return new Rect(
			rect.x + xoffset,
			rect.yMax - yoffset - element_height,
			element_width,
			element_height
		);
	}
	public static Rect GetBelowRightElement(Rect rect, float xoffset, float element_width, float yoffset, float element_height)
	{
		return new Rect(
			rect.xMax - xoffset - element_width,
			rect.yMax - yoffset - element_height,
			element_width,
			element_height
		);
	}

	public static Rect GetPaddedRect(Rect rect, float padding)
	{
		return new Rect(
			rect.x + padding,
			rect.y + padding,
			rect.width - padding - padding,
			rect.height - padding - padding
		);
	}

	public static Rect GetPaddedRect(Rect rect, float vertical, float horizontal)
	{
		return new Rect(
			rect.x + horizontal,
			rect.y + vertical,
			rect.width - horizontal - horizontal,
			rect.height - vertical - vertical
		);
	}

	public static Rect GetPaddedRect(Rect rect, float above, float below, float left, float right)
	{
		return new Rect(
			rect.x + left,
			rect.y + above,
			rect.width - left - right,
			rect.height - above - below
		);
	}

	public static Rect GetSandwichedRectX(Rect rect, float before, float after)
	{
		return new Rect(
			rect.x + before,
			rect.y,
			rect.width - before - after,
			rect.height
		);
	}

	public static Rect GetSandwichedRectY(Rect rect, float before, float after)
	{
		return new Rect(
			rect.x,
			rect.y + before,
			rect.width,
			rect.height - before - after
		);
	}
	public static Rect GetMiddleX(Rect rect, float width)
	{
		return new Rect(rect.center.x - (width / 2f), rect.y, width, rect.height);
	}
	public static void DrawRect(Rect rect, string name)
	{
		DrawRect(rect, name, false, false, false, false);
	}
	public static void DrawRect(Rect rect, string name, bool hover, bool active, bool on, bool focus)
	{
		if (Event.current.type == EventType.Repaint) {
			GUIStyle style = GUI.skin.GetStyle(name);
			if (style != null) {
				style.Draw(rect, hover, active, on, focus);
			}
		}
	}
	public static Rect GetGridRect(Rect rect, int i, int j, int rows, int cols)
	{
		if (i < 0 || i >= rows || j < 0 || j >= cols) {
			return new Rect();
		}
		float col_width = rect.width / ((float)cols);
		float row_height = rect.height / ((float)rows);
		return new Rect(rect.x + (j * col_width), rect.y + (i * row_height), col_width, row_height);
	}
	public static Rect GetScrollViewRect(float content_width, float content_height, float scroll_width, float scroll_height)
	{
		float width = content_width;
		float height = content_height;
		GUIStyle hscroll_style = GUI.skin.horizontalScrollbar;
		GUIStyle vscroll_style = GUI.skin.verticalScrollbar;
		//vertical
		if (vscroll_style != null && content_height > scroll_height) {
			width -= vscroll_style.fixedWidth;
		}
		//horizontal
		if (hscroll_style != null && content_width > scroll_width) {
			height -= hscroll_style.fixedHeight;
		}
		return new Rect(0, 0, width, height);
	}
	public static Rect GetScrollViewRect(ReorderableList list, float width, float scrollheight)
	{
		float height = 0;
		if (list != null) {
			height += list.GetHeight();
		}
		GUIStyle vscroll_style = GUI.skin.verticalScrollbar;
		if (vscroll_style != null && height > scrollheight) {
			width -= vscroll_style.fixedWidth;
		}
		return new Rect(0, 0, width, height);
	}
	public static Rect GetDisplayRowRect(Rect rect, int index, int level, float indent_width, float row_height, float row_space)
	{
		Rect row_rect = GetAboveElement(rect, index, row_height, row_space, 0);
		return GetSandwichedRectX(row_rect, level * indent_width, 0);
	}
	public static Vector3 DrawVector3Field(Rect rect, Vector3 value, string labelstyle, string fieldstyle)
	{
		float field_width = rect.width / 3f;
		float x = DrawVectorElementField(GetLeftElement(rect, 0, field_width), value.x, "X", labelstyle, fieldstyle);
		float y = DrawVectorElementField(GetLeftElement(rect, 1, field_width), value.y, "Y", labelstyle, fieldstyle);
		float z = DrawVectorElementField(GetLeftElement(rect, 2, field_width), value.z, "Z", labelstyle, fieldstyle);
		return new Vector3(x, y, z);
	}
	public static float DrawVectorElementField(Rect rect, float value, string label, string labelstyle, string fieldstyle)
	{
		//draw vector element label
		float label_width = Mathf.Min(VECTORLABEL_WIDTH, 0.3f * rect.width);
		Rect rect_field = GetLeftElement(rect, 0, label_width);
		if (string.IsNullOrWhiteSpace(labelstyle)) {
			GUI.Label(rect_field, label);
		}
		else {
			GUI.Label(rect_field, label, labelstyle);
		}
		//draw vector element field
		rect_field = GetSandwichedRectX(rect, label_width, 0);
		if (string.IsNullOrWhiteSpace(fieldstyle)) {
			return EditorGUI.DelayedFloatField(rect_field, value);
		}
		else {
			return EditorGUI.DelayedFloatField(rect_field, value, fieldstyle);
		}
	}
}
