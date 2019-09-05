using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class PreviewAxiDisplay : PanelGUI
{
	public readonly static Color COLOUR_ORIGIN = new Color(1, 1, 1, 0.8f);
	public readonly static Color COLOUR_XAXIS = new Color(1, 102 / 255f, 0, 0.8f);
	public readonly static Color COLOUR_YAXIS = new Color(0, 204 / 255f, 102 / 255f, 0.8f);
	public readonly static Color COLOUR_ZAXIS = new Color(0, 153 / 255f, 1, 0.8f);
	public readonly static Color COLOUR_VOXEL = new Color(1, 1, 1, 0.2f);

	public readonly static Color[] COLOUR_AXI = new Color[] {
		new Color(0, 224 / 255f, 245 / 255f),//above
		new Color(0, 167 / 255f, 250 / 255f),//below
		new Color(153 / 255f, 204 / 255f, 1, 0.8f),//d0
		new Color(102 / 255f, 153 / 255f, 1, 0.8f),//d1
		new Color(102 / 255f, 102 / 255f, 1, 0.8f),//d2
		new Color(153 / 255f, 102 / 255f, 1, 0.8f),//d3
		new Color(153 / 255f, 51 / 255f, 1, 0.8f),//d4
		new Color(102 / 255f, 0, 204 / 255f, 0.8f)//d5
	};

	public Mesh origin_mesh;
	public Mesh axis_mesh;
	public Mesh voxel_mesh;
	public Material material;

	public bool expanded;
	public bool origin_enabled;
	public bool global_enabled;
	public bool voxelaxi_enabled;
	public bool voxelframe_enabled;
	public bool voxelframe_flip;
	public bool edge_vertical;
	public bool face_hexagon;
	public float distance_factor;

	private Vector2 _scroll;
	private int _displaymode;
	private MaterialPropertyBlock _propblock;

	public PreviewAxiDisplay(int displaymode)
	{
		_displaymode = displaymode;
		origin_enabled = true;
		global_enabled = false;
		voxelaxi_enabled = true;
		voxelframe_enabled = false;
		voxelframe_flip = false;
		edge_vertical = false;
		face_hexagon = false;
		distance_factor = 1f;
		_propblock = new MaterialPropertyBlock();
	}


	public override void Enable()
	{
		_propblock.Clear();
		_scroll = Vector2.zero;
	}

	public override void Disable()
	{
		_propblock.Clear();
		_scroll = Vector2.zero;
	}

	private float GetPanelHeight()
	{
		float height = 0;
		if (expanded) {
			//origin
			height += VxlGUI.MED_BAR;
			//global axis
			height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
			//axi
			height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
			if (_displaymode == 0) {
				//frame
				height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
				if (voxelframe_enabled) {
					//flip voxel
					height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
				}
			}
			else if (_displaymode == 1) {
				//frame
				height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
				if (voxelframe_enabled) {
					if (edge_vertical) {
						//vertical edge
						height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
					}
					else {
						//flip voxel
						height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
						//vertical edge
						height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
					}
				}
			}
			else if (_displaymode == 2) {
				//frame
				height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
				if (voxelframe_enabled) {
					//flip voxel
					height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
					//hexagon face
					height += VxlGUI.SM_SPACE + VxlGUI.MED_BAR;
				}
			}

		}
		return height;
	}

	public override void DrawGUI(Rect rect)
	{
		float width = Mathf.Min(200, rect.width);
		rect = new Rect(rect.x, rect.y, width, rect.height);
		if (!expanded) {
			//calculate rect
			rect = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR);
			//draw background
			VxlGUI.DrawRect(rect, "DarkTransparentGrey");
			//draw foldout toggle label
			EditorGUI.BeginChangeCheck();
			expanded = EditorGUI.Foldout(rect, expanded, "Display", true, GUI.skin.GetStyle("LightBoldFoldout"));
			if (EditorGUI.EndChangeCheck()) {
				_repaint_menu = true;
			}
		}
		else {
			float content_height = GetPanelHeight();
			float panel_height = Mathf.Min(rect.height, VxlGUI.SM_SPACE + VxlGUI.MED_BAR + content_height);
			float scroll_height = Mathf.Max(0, panel_height - VxlGUI.SM_SPACE - VxlGUI.MED_BAR);
			Rect panel_rect = VxlGUI.GetAboveElement(rect, 0, panel_height);
			Rect rect_scroll = VxlGUI.GetSandwichedRectY(panel_rect, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, 0);
			Rect content_rect = VxlGUI.GetScrollViewRect(rect_scroll.width, rect_scroll.height, content_height);

			VxlGUI.DrawRect(panel_rect, "DarkTransparentGrey");

			EditorGUI.BeginChangeCheck();
			Rect row_rect = VxlGUI.GetAboveElement(panel_rect, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
			expanded = EditorGUI.Foldout(row_rect, expanded, "Display", true, GUI.skin.GetStyle("LightBoldFoldout"));
			Rect rect_content = VxlGUI.GetBelowRightElement(content_rect, 0, content_rect.width - VxlGUI.MED_BAR, 0, content_rect.height);
			if (EditorGUI.EndChangeCheck()) {
				_repaint_menu = true;
			}

			EditorGUI.BeginChangeCheck();
			_scroll = GUI.BeginScrollView(rect_scroll, _scroll, content_rect);
			//origin
			int level = 0;
			row_rect = VxlGUI.GetAboveElement(rect_content, level, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
			origin_enabled = EditorGUI.Foldout(row_rect, origin_enabled, "Origin", true, GUI.skin.GetStyle("LightFoldout"));
			level += 1;
			//global axis
			row_rect = VxlGUI.GetAboveElement(rect_content, level, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
			global_enabled = EditorGUI.Foldout(row_rect, global_enabled, "Global Axis", true, GUI.skin.GetStyle("LightFoldout"));
			level += 1;
			//axi
			row_rect = VxlGUI.GetAboveElement(rect_content, level, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
			voxelaxi_enabled = EditorGUI.Foldout(row_rect, voxelaxi_enabled, "Voxel Axi", true, GUI.skin.GetStyle("LightFoldout"));
			level += 1;
			
			//extras
			if (_displaymode == 0) {
				//frame
				row_rect = VxlGUI.GetAboveElement(rect_content, level, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
				voxelframe_enabled = EditorGUI.Foldout(row_rect, voxelframe_enabled, "Voxel Frame", true, GUI.skin.GetStyle("LightFoldout"));
				level += 1;
				if (voxelframe_enabled) {
					float y = (level * VxlGUI.MED_BAR) + ((Mathf.Max(0, level - 1) * VxlGUI.SM_SPACE));
					Rect frame_rect = VxlGUI.GetSandwichedRectX(VxlGUI.GetSandwichedRectY(rect_content, y, 0), VxlGUI.MED_BAR, 0);
					row_rect = VxlGUI.GetAboveElement(frame_rect, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
					voxelframe_flip = EditorGUI.Foldout(row_rect, voxelframe_flip, "Flip Voxel", true, GUI.skin.GetStyle("LightFoldout"));
					level += 1;
				}
			}
			else if (_displaymode == 1) {
				//frame
				row_rect = VxlGUI.GetAboveElement(rect_content, level, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
				voxelframe_enabled = EditorGUI.Foldout(row_rect, voxelframe_enabled, "Voxel Frame", true, GUI.skin.GetStyle("LightFoldout"));
				level += 1;
				if (voxelframe_enabled) {
					float y = (level * VxlGUI.MED_BAR) + ((Mathf.Max(0, level - 1) * VxlGUI.SM_SPACE));
					Rect frame_rect = VxlGUI.GetSandwichedRectX(VxlGUI.GetSandwichedRectY(rect_content, y, 0), VxlGUI.MED_BAR, 0);
					row_rect = VxlGUI.GetAboveElement(frame_rect, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
					if (edge_vertical) {
						edge_vertical = EditorGUI.Foldout(row_rect, edge_vertical, "Vertical Edge", true, GUI.skin.GetStyle("LightFoldout"));
						level += 1;
					}
					else {
						voxelframe_flip = EditorGUI.Foldout(row_rect, voxelframe_flip, "Flip Voxel", true, GUI.skin.GetStyle("LightFoldout"));
						level += 1;
						row_rect = VxlGUI.GetAboveElement(frame_rect, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
						edge_vertical = EditorGUI.Foldout(row_rect, edge_vertical, "Vertical Edge", true, GUI.skin.GetStyle("LightFoldout"));
						level += 1;
					}
				}
			}
			else if (_displaymode == 2) {
				//frame
				row_rect = VxlGUI.GetAboveElement(rect_content, level, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
				voxelframe_enabled = EditorGUI.Foldout(row_rect, voxelframe_enabled, "Voxel Frame", true, GUI.skin.GetStyle("LightFoldout"));
				level += 1;
				if (voxelframe_enabled) {
					float y = (level * VxlGUI.MED_BAR) + ((Mathf.Max(0, level - 1) * VxlGUI.SM_SPACE));
					Rect frame_rect = VxlGUI.GetSandwichedRectX(VxlGUI.GetSandwichedRectY(rect_content, y, 0), VxlGUI.MED_BAR, 0);
					row_rect = VxlGUI.GetAboveElement(frame_rect, 0, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
					voxelframe_flip = EditorGUI.Foldout(row_rect, voxelframe_flip, "Flip Voxel", true, GUI.skin.GetStyle("LightFoldout"));
					level += 1;
					row_rect = VxlGUI.GetAboveElement(frame_rect, 1, VxlGUI.MED_BAR, VxlGUI.SM_SPACE, 0);
					face_hexagon = EditorGUI.Foldout(row_rect, face_hexagon, "Hexagon Face", true, GUI.skin.GetStyle("LightFoldout"));
					level += 1;
				}
			}
			GUI.EndScrollView();
			if (EditorGUI.EndChangeCheck()) {
				_repaint_menu = true;
				_render_mesh = true;
			}	
		}
	}

	public void DrawDisplayObjects(PreviewRenderUtility preview)
	{
		if (origin_enabled) {
			DrawOriginDisplayObject(preview);
		}
		if (global_enabled) {
			DrawGlobalAxisDisplayObject(preview);
		}
		if (voxelaxi_enabled) {
			DrawAxiDisplayObject(preview);
		}
		switch (_displaymode) {
			case 0:
				DrawCornerDisplayObjects(preview);
				break;
			case 1:
				DrawEdgeDisplayObjects(preview);
				break;
			case 2:
				DrawFaceDisplayObjects(preview);
				break;
		}
	}

	private void DrawOriginDisplayObject(PreviewRenderUtility preview)
	{
		Vector3 scale = 2 * distance_factor * Vector3.one;
		_propblock.SetColor("_Color", COLOUR_ORIGIN);
		DrawDisplayObject(preview, origin_mesh, Vector3.zero, Quaternion.identity, scale);
	}

	private void DrawGlobalAxisDisplayObject(PreviewRenderUtility preview)
	{
		//y axis
		Vector3 pos = 0.03f * distance_factor * Vector3.up;
		Quaternion rot = Quaternion.identity;
		Vector3 scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_YAXIS);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
		//x axis
		pos = (0.03f * distance_factor * Vector3.right);
		rot = Quaternion.Euler(0, 0, -90);
		scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_XAXIS);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
		//z axis
		pos = (0.03f * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 0, 0);
		scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_ZAXIS);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
	}

	private void DrawAxiDisplayObject(PreviewRenderUtility preview)
	{
		//above
		Vector3 pos = (0.03f * distance_factor * Vector3.up);
		Vector3 scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[0]);
		DrawDisplayObject(preview, axis_mesh, pos, Quaternion.identity, scale);
		//below
		pos = (0.03f * distance_factor * Vector3.down);
		Quaternion rot = Quaternion.Euler(180, 0, 0);
		scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[1]);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
		//d0
		pos = Quaternion.Euler(0, 60, 0) * (0.03f * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 60, 0);
		scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[2]);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
		//d1
		pos = Quaternion.Euler(0, 120, 0) * (0.03f * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 120, 0);
		scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[3]);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
		//d2
		pos = Quaternion.Euler(0, 180, 0) * (0.03f * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 180, 0);
		scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[4]);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
		//d3
		pos = Quaternion.Euler(0, 240, 0) * (0.03f * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 240, 0);
		scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[5]);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
		//d4
		pos = Quaternion.Euler(0, 300, 0) * (0.03f * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 300, 0);
		scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[6]);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
		//d5
		pos = (0.03f * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 0, 0);
		scale = new Vector3(0.01f * distance_factor, 100f, 0.01f * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[7]);
		DrawDisplayObject(preview, axis_mesh, pos, rot, scale);
	}

	private void DrawCornerDisplayObjects(PreviewRenderUtility preview)
	{
		Vector3 pos;
		if (voxelframe_enabled) {
			if (voxelframe_flip) {
				pos = new Vector3(-1, 0.5f, -Vx.SQRT3_R1);
			}
			else {
				pos = new Vector3(-1, -0.5f, -Vx.SQRT3_R1);
			}
			_propblock.SetColor("_Color", COLOUR_VOXEL);
			DrawDisplayObject(preview, voxel_mesh, pos, Quaternion.identity, Vector3.one);
		}
	}

	private void DrawEdgeDisplayObjects(PreviewRenderUtility preview)
	{
		Vector3 pos;
		if (voxelframe_enabled) {
			if (edge_vertical) {
				pos = new Vector3(-1, 0, -Vx.SQRT3_R1);
			}
			else {
				pos = new Vector3(-1, 0, 0);
				if (voxelframe_flip) {
					pos += 0.5f * Vector3.up;
				}
				else {
					pos -= 0.5f * Vector3.up;
				}
			}
			_propblock.SetColor("_Color", COLOUR_VOXEL);
			DrawDisplayObject(preview, voxel_mesh, pos, Quaternion.identity, Vector3.one);
		}
	}

	private void DrawFaceDisplayObjects(PreviewRenderUtility preview)
	{
		Vector3 pos;
		if (voxelframe_enabled) {
			if (face_hexagon) {
				if (voxelframe_flip) {
					pos = new Vector3(0, 0.5f, 0);
				}
				else {
					pos = new Vector3(0, -0.5f, 0);
				}
			}
			else {
				if (voxelframe_flip) {
					pos = new Vector3(1, 0, 0);
				}
				else {
					pos = new Vector3(-1, 0, 0);
				}
			}
			_propblock.SetColor("_Color", COLOUR_VOXEL);
			DrawDisplayObject(preview, voxel_mesh, pos, Quaternion.identity, Vector3.one);
		}
	}

	private void DrawDisplayObject(PreviewRenderUtility preview, Mesh mesh, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (preview == null || mesh == null || scale == Vector3.zero) {
			return;
		}
		preview.DrawMesh(mesh, pos, scale, rot, material, 0, _propblock, null, false);
	}
}
