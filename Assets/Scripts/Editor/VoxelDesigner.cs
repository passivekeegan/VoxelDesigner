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
	public Mesh vertex_mesh;
	public Mesh axi_mesh;
	public Mesh voxel_mesh;
	public GUISkin skin;
	public Material axi_material;
	public Material vertex_material;
	public Material mesh_material;

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
		DisableMode(_mode);
	}

	public void Intialize()
	{
		_mode = 0;
		_cornergui = new CornerModeGUI();
		_cornergui.vertexMesh = vertex_mesh;
		_cornergui.vertexMaterial = vertex_material;
		_cornergui.meshMaterial = mesh_material;
		_cornergui.axiMaterial = axi_material;
		_cornergui.axiMesh = axi_mesh;
		_cornergui.originMesh = vertex_mesh;
		_cornergui.voxelMesh = voxel_mesh;
		_edgegui = new EdgeModeGUI();
		_edgegui.vertexMesh = vertex_mesh;
		_edgegui.vertexMaterial = vertex_material;
		_edgegui.meshMaterial = mesh_material;
		_edgegui.axiMaterial = axi_material;
		_edgegui.axiMesh = axi_mesh;
		_edgegui.originMesh = vertex_mesh;
		_edgegui.voxelMesh = voxel_mesh;
		_facegui = new FaceModeGUI();
		_facegui.vertexMesh = vertex_mesh;
		_facegui.vertexMaterial = vertex_material;
		_facegui.meshMaterial = mesh_material;
		_facegui.axiMaterial = axi_material;
		_facegui.axiMesh = axi_mesh;
		_facegui.originMesh = vertex_mesh;
		_facegui.voxelMesh = voxel_mesh;
		_mappinggui = new MappingModeGUI();
		_mappinggui.vertexMesh = vertex_mesh;
		_mappinggui.vertexMaterial = vertex_material;
		_mappinggui.meshMaterial = mesh_material;
		_mappinggui.axiMaterial = axi_material;
		_mappinggui.axiMesh = axi_mesh;
		_mappinggui.originMesh = vertex_mesh;
		_mappinggui.voxelMesh = voxel_mesh;

		UpdateLayoutRects();
		EnableMode(_mode);
	}

	private void OnGUI()
	{
		GUI.skin = skin;
		//update important rects
		UpdateLayoutRects();
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
			_repaint_menu = true;
			_update_mesh = true;
			_render_mesh = true;
		}
		//draw mode menu and mode preview
		switch (_mode) {
			case 0:
				DrawMode(_cornergui, _rect_menucontent, _rect_preview);
				break;
			case 1:
				DrawMode(_edgegui, _rect_menucontent, _rect_preview);
				break;
			case 2:
				DrawMode(_facegui, _rect_menucontent, _rect_preview);
				break;
			case 3:
				DrawMode(_mappinggui, _rect_menucontent, _rect_preview);
				break;
		}
		if (Event.current.type == EventType.MouseMove) {
			_repaint_menu = true;
		}
		if (_repaint_menu) {
			_repaint_menu = false;
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
		_repaint_menu = true;
		_update_mesh = true;
		_render_mesh = true;
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

	private void DrawMode(PanelGUI modegui, Rect rect, Rect preview_rect)
	{
		modegui.updateMesh = modegui.updateMesh || _update_mesh;
		_update_mesh = false;
		modegui.renderMesh = modegui.renderMesh || _render_mesh;
		_render_mesh = false;
		modegui.DrawGUI(rect);
		modegui.DrawPreview(preview_rect);
		_repaint_menu = _repaint_menu || modegui.repaintMenu;
		modegui.repaintMenu = false;
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

