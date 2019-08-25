using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System;

public class VoxelDesigner : EditorWindow
{
	private readonly static string[] MODES = new string[] {
		"Corner", "Edge", "Face", "Mapping"
	};

	public Texture2D colourmap;
	public GUISkin skin;

	private bool _repaint_menu;
	private bool _update_mesh;
	private bool _render_mesh;
	private int _mode;
	private Rect _rect_mode;
	private Rect _rect_menucontent;
	private Rect _rect_preview;
	private CornerModeGUI _cornergui;
	private EdgeModeGUI _edgegui;
	private FaceModeGUI _facegui;
	private MappingModeGUI _mappinggui;
	private PreviewPanel _preview;

	[MenuItem("Window/Voxel Designer")]
	public static void Initialize()
	{
		VoxelDesigner designer = (VoxelDesigner) EditorWindow.GetWindow(typeof(VoxelDesigner), true, "Voxel Designer", false);
		designer.wantsMouseMove = true;
		designer.Intialize();
		designer.Show();
	}

	private void OnEnable()
	{
		Undo.undoRedoPerformed += UndoRedoPerformed;
	}

	private void OnDisable()
	{
		Undo.undoRedoPerformed -= UndoRedoPerformed;
		_preview.Disable();
	}

	public void Intialize()
	{
		_repaint_menu = true;
		_update_mesh = false;
		_render_mesh = true;
		_mode = 0;
		_cornergui = new CornerModeGUI();
		_edgegui = new EdgeModeGUI();
		_facegui = new FaceModeGUI();
		_mappinggui = new MappingModeGUI();
		_preview = new PreviewPanel(colourmap);
		_preview.Enable();


		UpdateLayoutRects();
		EnableMode(_mode);
	}

	private void OnGUI()
	{
		GUI.skin = skin;
		//update important rects
		UpdateLayoutRects();
		if (Event.current.type == EventType.MouseMove) {
			_repaint_menu = true;
		}
		//draw mode toolbar
		EditorGUI.BeginChangeCheck();
		int newmode = GUI.Toolbar(_rect_mode, _mode, MODES, GUI.skin.GetStyle("ModeToolbar"));
		//switch modes
		if (EditorGUI.EndChangeCheck() && newmode != _mode) {
			//enable new mode
			EnableMode(newmode);
			//disable old mode
			DisableMode(_mode);
			//switch mode indices
			_mode = newmode;
		}
		//draw mode menu
		switch (_mode) {
			case 0:
				DrawMode(_cornergui);
				break;
			case 1:
				DrawMode(_edgegui);
				break;
			case 2:
				DrawMode(_facegui);
				break;
			case 3:
				DrawMode(_mappinggui);
				break;
		}
		//process preview input
		ProcessPreviewInput();
		//refresh mesh if dirty
		if (_update_mesh) {
			_update_mesh = false;
			_preview.updateMesh = true;
		}
		if (_render_mesh) {
			_render_mesh = false;
			_preview.renderMesh = true;
		}
		//update preview variables
		UpdatePreviewVariables();
		//draw preview panel
		_preview.DrawGUI(_rect_preview);
		if (_repaint_menu) {
			_repaint_menu = true;
			Repaint();
		}
	}

	private void EnableMode(int mode)
	{
		switch (mode) {
			case 0:
				_cornergui.Enable();
				break;
			case 1:
				_edgegui.Enable();
				break;
			case 2:
				_facegui.Enable();
				break;
			case 3:
				_mappinggui.Enable();
				break;
		}
	}

	private void DisableMode(int mode)
	{
		switch (mode) {
			case 0:
				_cornergui.Disable();
				break;
			case 1:
				_edgegui.Disable();
				break;
			case 2:
				_facegui.Disable();
				break;
			case 3:
				_mappinggui.Disable();
				break;
		}
	}

	private void DrawMode(PanelGUI modegui)
	{
		modegui.DrawGUI(_rect_menucontent);
		_repaint_menu = _repaint_menu || modegui.repaintMenu;
		modegui.repaintMenu = false;
		_update_mesh = _update_mesh || modegui.updateMesh;
		modegui.updateMesh = false;
		_render_mesh = _render_mesh || modegui.renderMesh;
		modegui.renderMesh = false;
	}

	private void ProcessPreviewInput()
	{
		if (!_rect_preview.Contains(Event.current.mousePosition)) {
			return;
		}
		switch (Event.current.type) {
			case EventType.MouseDrag:
				if (Event.current.button == 1) {
					_preview.MoveVerticalPreview(Event.current.delta.y);
					_repaint_menu = true;
					_render_mesh = true;
				}
				if (Event.current.button == 2) {
					_preview.RotatePreview(Event.current.delta.x);
					_repaint_menu = true;
					_render_mesh = true;
				}
				break;
			case EventType.ScrollWheel:
				_preview.ZoomPreview(Event.current.delta.y);
				_repaint_menu = true;
				_render_mesh = true;
				break;
			case EventType.MouseDown:
				if (Event.current.button == 0) {
					float x = Mathf.Clamp01((Event.current.mousePosition.x - _rect_preview.xMin) / _rect_preview.width);
					float y = 1 - Mathf.Clamp01((Event.current.mousePosition.y - _rect_preview.yMin) / _rect_preview.height);
					Vector2 viewport_point = new Vector2(x, y);
					int index = _preview.VertexCast(viewport_point);
					if (index < 0) {
						return;
					}
				}
				break;
		}
	}

	private void UpdatePreviewVariables()
	{
		VoxelComponent target = null;
		PreviewDrawMode prevmode = PreviewDrawMode.None;
		int primary_index = -1;
		int secondary_index = -1;
		switch (_mode) {
			case 0:
				target = _cornergui.selected;
				prevmode = _cornergui.previewMode;
				primary_index = _cornergui.primary_index;
				secondary_index = _cornergui.secondary_index;
				break;
			case 1:
				target = _edgegui.selected;
				prevmode = _edgegui.previewMode;
				primary_index = _edgegui.primary_index;
				secondary_index = _edgegui.secondary_index;
				break;
			case 2:
				target = _facegui.selected;
				prevmode = _facegui.previewMode;
				primary_index = _facegui.primary_index;
				secondary_index = _facegui.secondary_index;
				break;
		}
		_preview.target = target;
		_preview.SetDrawMode(prevmode);
		_preview.SetPrimaryIndex(primary_index);
		_preview.SetSecondaryIndex(secondary_index);
	}

	private void UpdateLayoutRects()
	{
		Rect window_rect = new Rect(Vector2.zero, position.size);
		float pixels = Mathf.Clamp(0.5f * window_rect.width, VxlGUI.MINWIDTH_WINDOW, VxlGUI.MAXWIDTH_WINDOW);
		Rect menu_rect = VxlGUI.GetLeftElement(window_rect, 0, pixels);
		_rect_mode = VxlGUI.GetAboveElement(menu_rect, 0, VxlGUI.MODE);
		_rect_menucontent = VxlGUI.GetSandwichedRectY(menu_rect, VxlGUI.MODE, 0);
		_rect_preview = VxlGUI.GetRightElement(window_rect, 0, window_rect.width - pixels);
	}

	//Callbacks
	private void UndoRedoPerformed()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = false;
		Repaint();
	}
}

