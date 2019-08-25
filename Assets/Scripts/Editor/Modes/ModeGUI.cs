using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class ModeGUI<T> : PanelGUI where T : VoxelObject
{
	public T selected;

	protected int _mode;
	protected string[] _mode_labels;
	protected Rect _rect_submodes;
	protected Rect _rect_content;
	protected Rect _rect_selected;
	protected PanelGUI[] _modes;

	public override void Enable()
	{
		_mode = 0;
		if (_mode >= 0 && _mode < _modes.Length) {
			_modes[_mode].Enable();
		}
	}

	public override void Disable()
	{
		for (int k = 0;k < _modes.Length;k++) {
			_modes[k].Disable();
		}
	}

	public override PreviewDrawMode previewMode {
		get {
			if (_mode < 0 || _mode >= _modes.Length) {
				return PreviewDrawMode.None;
			}
			return _modes[_mode].previewMode;
		}
	}

	public override int primary_index {
		get {
			if (_mode < 0 || _mode >= _modes.Length) {
				return -1;
			}
			return _modes[_mode].primary_index;
		}
	}

	public override int secondary_index {
		get {
			if (_mode < 0 || _mode >= _modes.Length) {
				return -1;
			}
			return _modes[_mode].secondary_index;
		}
	}

	public override bool repaintMenu {
		get {
			if (_repaint_menu) {
				return true;
			}
			if (_mode >= 0 && _mode < _modes.Length) {
				return _modes[_mode].repaintMenu;
			}
			return false;
		}
		set {
			_repaint_menu = value;
			if (_mode >= 0 && _mode < _modes.Length) {
				_modes[_mode].repaintMenu = value;
			}
		}
	}

	public override bool updateMesh {
		get {
			if (_update_mesh) {
				return true;
			}
			if (_mode >= 0 && _mode < _modes.Length) {
				return _modes[_mode].updateMesh;
			}
			return false;
		}
		set {
			_update_mesh = value;
			if (_mode >= 0 && _mode < _modes.Length) {
				_modes[_mode].updateMesh = value;
			}
		}
	}

	public override bool renderMesh {
		get {
			if (_render_mesh) {
				return true;
			}
			if (_mode >= 0 && _mode < _modes.Length) {
				return _modes[_mode].renderMesh;
			}
			return false;
		}
		set {
			_render_mesh = value;
			if (_mode >= 0 && _mode < _modes.Length) {
				_modes[_mode].renderMesh = value;
			}
		}
	}

	public override void DrawGUI(Rect rect)
	{
		UpdateModes();
		//update rects
		UpdateRects(rect);
		//tab buttons
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(270, _rect_submodes.center);
		int newmode = GUI.SelectionGrid(VxlGUI.CenterInvertRect(_rect_submodes), _mode, _mode_labels, _modes.Length, GUI.skin.GetStyle("SubModeToolbar"));
		//mode switch
		if (newmode != _mode) {
			//enable new mode
			_modes[newmode].Enable();
			//disable old mode
			_modes[_mode].Disable();
			_mode = newmode;
			//update new mode
			UpdateModes();
		}
		GUI.matrix = matrix;
		//tab content
		if (_mode >= 0 && _mode < _modes.Length) {
			_modes[_mode].DrawGUI(_rect_content);
		}
		//selection background
		VxlGUI.DrawRect(_rect_selected, "DarkGradient");
		//selection label
		string text = "[None]";
		if (selected != null) {
			text = selected.objname;
		}
		GUI.Label(_rect_selected, _title + ":  " + text, GUI.skin.GetStyle("LeftLightHeader"));
		UpdateModes();
	}

	private void UpdateRects(Rect rect)
	{
		_rect_selected = VxlGUI.GetBelowElement(rect, 0, VxlGUI.MED_BAR);
		Rect rect_panel = VxlGUI.GetPaddedRect(VxlGUI.GetSandwichedRectY(rect, 0, VxlGUI.MED_BAR), VxlGUI.LRG_PAD);
		_rect_submodes = VxlGUI.GetAboveElement(VxlGUI.GetLeftElement(rect_panel, 0, VxlGUI.MED_BAR), 0, Mathf.Min(_modes.Length * 60, rect_panel.height));
		_rect_content = VxlGUI.GetSandwichedRectX(rect_panel, VxlGUI.MED_BAR + (3 * VxlGUI.SM_SPACE), 0);
	}
	protected abstract void UpdateModes();
}