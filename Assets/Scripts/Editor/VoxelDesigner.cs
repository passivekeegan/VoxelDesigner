using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System;

public class VoxelDesigner : EditorWindow
{
	private readonly static string[] MODES = new string[] {
		"Corner", "Lateral", "Longitude", "Rect", "Hexagon", "Mapping"
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
	private LateralModeGUI _latgui;
	private LongitudeModeGUI _longgui;
	private RectModeGUI _rectgui;
	private HexagonModeGUI _hexgui;
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
		InitializeMode(_cornergui);
		_latgui = new LateralModeGUI();
		InitializeMode(_latgui);
		_longgui = new LongitudeModeGUI();
		InitializeMode(_longgui);
		_rectgui = new RectModeGUI();
		InitializeMode(_rectgui);
		_hexgui = new HexagonModeGUI();
		InitializeMode(_hexgui);
		_mappinggui = new MappingModeGUI();
		InitializeMode(_mappinggui);
		UpdateLayoutRects();
		EnableMode(_mode);
	}

	private void InitializeMode(CornerModeGUI modegui)
	{
		modegui.vertexMesh = vertex_mesh;
		modegui.vertexMaterial = vertex_material;
		modegui.meshMaterial = mesh_material;
		modegui.axiMaterial = axi_material;
		modegui.axiMesh = axi_mesh;
		modegui.originMesh = vertex_mesh;
		modegui.voxelMesh = voxel_mesh;
	}
	private void InitializeMode(LateralModeGUI modegui)
	{
		modegui.vertexMesh = vertex_mesh;
		modegui.vertexMaterial = vertex_material;
		modegui.meshMaterial = mesh_material;
		modegui.axiMaterial = axi_material;
		modegui.axiMesh = axi_mesh;
		modegui.originMesh = vertex_mesh;
		modegui.voxelMesh = voxel_mesh;
	}
	private void InitializeMode(LongitudeModeGUI modegui)
	{
		modegui.vertexMesh = vertex_mesh;
		modegui.vertexMaterial = vertex_material;
		modegui.meshMaterial = mesh_material;
		modegui.axiMaterial = axi_material;
		modegui.axiMesh = axi_mesh;
		modegui.originMesh = vertex_mesh;
		modegui.voxelMesh = voxel_mesh;
	}
	private void InitializeMode(RectModeGUI modegui)
	{
		modegui.vertexMesh = vertex_mesh;
		modegui.vertexMaterial = vertex_material;
		modegui.meshMaterial = mesh_material;
		modegui.axiMaterial = axi_material;
		modegui.axiMesh = axi_mesh;
		modegui.originMesh = vertex_mesh;
		modegui.voxelMesh = voxel_mesh;
	}
	private void InitializeMode(HexagonModeGUI modegui)
	{
		modegui.vertexMesh = vertex_mesh;
		modegui.vertexMaterial = vertex_material;
		modegui.meshMaterial = mesh_material;
		modegui.axiMaterial = axi_material;
		modegui.axiMesh = axi_mesh;
		modegui.originMesh = vertex_mesh;
		modegui.voxelMesh = voxel_mesh;
	}
	private void InitializeMode(MappingModeGUI modegui)
	{
		modegui.vertexMesh = vertex_mesh;
		modegui.vertexMaterial = vertex_material;
		modegui.meshMaterial = mesh_material;
		modegui.axiMaterial = axi_material;
		modegui.axiMesh = axi_mesh;
		modegui.originMesh = vertex_mesh;
		modegui.voxelMesh = voxel_mesh;
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
				DrawMode(_latgui, _rect_menucontent, _rect_preview);
				break;
			case 2:
				DrawMode(_longgui, _rect_menucontent, _rect_preview);
				break;
			case 3:
				DrawMode(_rectgui, _rect_menucontent, _rect_preview);
				break;
			case 4:
				DrawMode(_hexgui, _rect_menucontent, _rect_preview);
				break;
			case 5:
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
				_latgui.Enable();
				break;
			case 2:
				_longgui.Enable();
				break;
			case 3:
				_rectgui.Enable();
				break;
			case 4:
				_hexgui.Enable();
				break;
			case 5:
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
				_latgui.Disable();
				break;
			case 2:
				_longgui.Disable();
				break;
			case 3:
				_rectgui.Disable();
				break;
			case 4:
				_hexgui.Disable();
				break;
			case 5:
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
		_rect_mode = VxlGUI.GetAboveElement(window_rect, 0, VxlGUI.MODE);
		Rect content_rect = VxlGUI.GetSandwichedRectY(window_rect, VxlGUI.MODE, 0);
		float pixels = Mathf.Clamp(0.5f * content_rect.width, VxlGUI.MINWIDTH_WINDOW, VxlGUI.MAXWIDTH_WINDOW);
		_rect_menucontent = VxlGUI.GetLeftElement(content_rect, 0, pixels);
		_rect_preview = VxlGUI.GetRightElement(content_rect, 0, content_rect.width - pixels);
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

