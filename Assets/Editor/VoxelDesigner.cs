using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VoxelDesigner : EditorWindow
{
	public GUISkin skin;
	public Mesh vertexmesh;
	public Mesh aximesh;
	public Material aximaterial;
	public Material meshmaterial;
	public Material vertexmaterial;

	private VoxelMapping _map;
	private VoxelDesign _design;

	private bool _repaint;
	private int _mode;
	private Rect _rect_bar;
	private Rect _rect_menu;
	private Rect _rect_preview;
	private ModeScreen[] _modes;
	private MeshPreview _preview;
	private AxiPreviewGUI _axigui;

	[MenuItem("Window/Voxel Designer")]
	public static void Initialize()
	{
		VoxelDesigner designer = (VoxelDesigner)EditorWindow.GetWindow(typeof(VoxelDesigner), true, "Voxel Designer", false);
		designer.wantsMouseMove = true;
		designer.Show();
	}

	private void OnEnable()
	{
		_mode = 0;
		_repaint = false;
		_preview = new MeshPreview();
		_preview.material = meshmaterial;
		_axigui = new AxiPreviewGUI();
		_axigui.origin_mesh = vertexmesh;
		_axigui.axi_mesh = aximesh;
		_axigui.material = aximaterial;

		_modes = new ModeScreen[] {
			new SelectMapMode("Select Map", _preview),
			new SelectPatternMode("Select Pattern", _preview),
			new EditMeshMode("Edit Mesh", _preview, vertexmesh, vertexmaterial)
		};
		Undo.undoRedoPerformed += UndoRedoPerformed;

		_preview.Enable();
		_axigui.Enable();
		_preview.AddPreviewGUI(_axigui);
		_modes[_mode].Enable();
	}

	private void OnDisable()
	{
		Undo.undoRedoPerformed -= UndoRedoPerformed;

		_repaint = false;
		_modes[_mode].Disable();
		_preview.RemovePreviewGUI(_axigui);
		_axigui.Disable();
		_preview.Disable();
	}

	private void OnGUI()
	{
		GUI.skin = skin;
		//update layout
		UpdateLayoutRects();
		//draw top panel
		DrawMainPanel();
		//draw menu
		EVx.DrawRect(_rect_menu, "DarkWhite");
		_modes[_mode].DrawGUI(EVx.GetPaddedRect(_rect_menu, 10));
		//process preview input
		_preview.ProcessInput(_rect_preview);
		//draw preview
		_preview.DrawGUI(_rect_preview);
		_repaint = _repaint || _preview.repaint;
		_preview.repaint = false;

		_repaint = _repaint || Event.current.type == EventType.MouseMove;
		if (_repaint) {
			Repaint();
		}
	}

	private void UpdateLayoutRects()
	{
		Rect window_rect = new Rect(Vector2.zero, position.size);
		_rect_bar = EVx.GetAboveElement(window_rect, 0, EVx.BAR_LARGE);
		Rect rect = EVx.GetSandwichedRectY(window_rect, EVx.BAR_LARGE, 0);
		float menu_width = Mathf.Min(300, 0.5f * rect.width);
		_rect_menu = EVx.GetLeftElement(rect, 0, menu_width);
		_rect_preview = EVx.GetSandwichedRectX(rect, menu_width, 0);
	}

	private void DrawMainPanel()
	{
		_map = _modes[0].map;
		_design = _modes[1].design;
		if (_map == null) {
			_design = null;
		}

		EVx.DrawRect(_rect_bar, "MediumBlack");
		float center_width = Mathf.Min(200, 0.4f * _rect_bar.width);
		float side_width = Mathf.Min(120, 0.3f * _rect_bar.width);
		GUI.Label(EVx.GetMiddleX(_rect_bar, center_width), _modes[_mode].title, "Title");
		//prev button
		if (_mode - 1 >= 0) {
			Rect rect_button = EVx.GetPaddedRect(EVx.GetLeftElement(_rect_bar, 0, side_width), 4);
			EditorGUI.BeginDisabledGroup(!_modes[_mode - 1].IsInputValid(_modes[_mode]));
			if (GUI.Button(rect_button, _modes[_mode - 1].title, "WhiteField")) {
				//changed mode
				ChangeMode(_mode - 1);
			}
			EditorGUI.EndDisabledGroup();
		}
		//next button
		if (_mode + 1 < _modes.Length) {
			Rect rect_button = EVx.GetPaddedRect(EVx.GetRightElement(_rect_bar, 0, side_width), 4);
			EditorGUI.BeginDisabledGroup(!_modes[_mode + 1].IsInputValid(_modes[_mode]));
			if (GUI.Button(rect_button, _modes[_mode + 1].title, "WhiteField")) {
				//changed mode
				ChangeMode(_mode + 1);
			}
			EditorGUI.EndDisabledGroup();
		}
	}

	private void ChangeMode(int newmode)
	{
		if (newmode < 0 || newmode >= _modes.Length || newmode == _mode) {
			return;
		}
		//transfer inputs across screens
		_modes[newmode].map = _modes[_mode].map;
		_modes[newmode].design = _modes[_mode].design;
		_modes[newmode].pattern = _modes[_mode].pattern;
		//disable old mode
		_modes[_mode].Disable();
		//enable new mode
		_modes[newmode].Enable();
		//update mode index
		_mode = newmode;
	}


	private void UndoRedoPerformed()
	{
		Repaint();
	}
}
