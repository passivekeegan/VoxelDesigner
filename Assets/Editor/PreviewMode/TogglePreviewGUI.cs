using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class TogglePreviewGUI : PreviewGUI
{
	protected const int ROWS = 2;
	protected const int COLUMNS = 3;
	protected const int MAX_WIDTH = 200;

	protected bool _expanded;
	protected int _row;
	protected int _column;
	protected string _title;
	protected Rect _rect_background;
	protected Rect _rect_title;
	protected Rect _rect_scroll;
	protected Rect _rect_content;
	protected Vector2 _scroll;
	protected List<ToggleOption> _options;

	public bool GetValue(int index)
	{
		if (index < 0 || index >= _options.Count) {
			return false;
		}
		return _options[index].value;
	}

	public void AddOption(ToggleOption option)
	{
		if (!option.isValid) {
			return;
		}
		_options.Add(option);
	}

	public void RemoveOption(int index)
	{
		if (index < 0 || index >= _options.Count) {
			return;
		}
		_options.RemoveAt(index);
	}

	public override void DrawGUI(Rect rect)
	{
		float line_height = GetLineHeight();
		rect = EVx.GetGridRect(rect, _row, _column, ROWS, COLUMNS);
		//update layout rects
		UpdateLayoutRects(rect);
		//draw background
		EVx.DrawRect(_rect_background, "MediumBlackTransparent");
		_expanded = EditorGUI.Foldout(_rect_title, _expanded, _title, true, "LightToggle10");
		if (_expanded) {
			int disabled_level = -1;
			int row = 0;
			_scroll = GUI.BeginScrollView(_rect_scroll, _scroll, _rect_content);
			for (int k = 0; k < _options.Count; k++) {
				ToggleOption option = _options[k];
				if (disabled_level >= 0 && option.level > disabled_level) {
					continue;
				}
				//draw option
				Rect row_rect = EVx.GetDisplayRowRect(_rect_content, row, option.level + 1, line_height, line_height, EVx.SM_SPACE);
				bool value = EditorGUI.Foldout(row_rect, option.value, option.label, true, "LightToggle10");
				row += 1;
				if (value != option.value) {
					_options[k] = new ToggleOption(option, value);
					updatemesh = updatemesh || _options[k].update_mesh;
					updaterender = updaterender || _options[k].update_render;
				}
				if (!value) {
					disabled_level = option.level;
				}
				else {
					disabled_level = -1;
				}
			}
			GUI.EndScrollView();
		}
		else {
			_scroll = Vector2.zero;
		}
	}

	protected void UpdateLayoutRects(Rect rect)
	{
		float line_height = GetLineHeight();
		float max_height = GetPanelHeight();
		float content_height = Mathf.Max(0, max_height - line_height - EVx.SM_SPACE - (2 * EVx.MED_PAD));
		float width = Mathf.Min(rect.width, MAX_WIDTH);
		float height = Mathf.Min(rect.height, max_height);

		if (_row == 0) {
			if (_column == 0) {
				_rect_background = new Rect(rect.x, rect.y, width, height);
			}
			else if (_column == COLUMNS - 1) {
				_rect_background = new Rect(rect.x + rect.width - width, rect.y, width, height);
			}
			else {
				_rect_background = new Rect(rect.x + ((rect.width - width) / 2f), rect.y, width, height);
			}
			Rect panel_rect = EVx.GetPaddedRect(_rect_background, EVx.MED_PAD);
			_rect_title = EVx.GetAboveElement(panel_rect, 0, line_height);
			_rect_scroll = EVx.GetSandwichedRectY(panel_rect, line_height + EVx.SM_SPACE, 0);
		}
		else {
			if (_column == 0) {
				_rect_background = new Rect(rect.x, rect.y + rect.height - height, width, height);
			}
			else if (_column == COLUMNS - 1) {
				_rect_background = new Rect(rect.x + rect.width - width, rect.y + rect.height - height, width, height);
			}
			else {
				_rect_background = new Rect(rect.x + ((rect.width - width) / 2f), rect.y + rect.height - height, width, height);
			}
			Rect panel_rect = EVx.GetPaddedRect(_rect_background, EVx.MED_PAD);
			_rect_title = EVx.GetBelowElement(panel_rect, 0, line_height);
			_rect_scroll = EVx.GetSandwichedRectY(panel_rect, 0, line_height + EVx.SM_SPACE);
		}
		_rect_content = EVx.GetScrollViewRect(_rect_scroll.width, content_height, _rect_scroll.width, _rect_scroll.height);
	}

	protected float GetLineHeight()
	{
		float line_height = EVx.MED_LINE;
		GUIStyle style = GUI.skin.GetStyle("LightToggle10");
		if (style != null) {
			line_height = style.lineHeight;
		}
		return line_height;
	}

	protected float GetPanelHeight()
	{
		float lineheight = GetLineHeight();
		if (!_expanded) {
			return lineheight + (2 * EVx.MED_PAD);
		}
		int count = 0;
		int disabled_level = -1;
		for (int k = 0; k < _options.Count; k++) {
			ToggleOption option = _options[k];
			if (disabled_level >= 0 && option.level > disabled_level) {
				continue;
			}
			count += 1;
			if (!option.value) {
				disabled_level = option.level;
			}
			else {
				disabled_level = -1;
			}
		}
		float height = lineheight + (2 * EVx.MED_PAD);
		if (count > 0) {
			height += count * (lineheight + EVx.SM_SPACE);
		}
		return height;
	}
}