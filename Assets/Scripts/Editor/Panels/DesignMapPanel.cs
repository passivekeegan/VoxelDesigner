using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DesignMapPanel : PanelGUI
{
	public MappingObject target;

	private Rect _rect_header;
	private Rect _rect_corner;
	private Rect _rect_edge;
	private Rect _rect_face;
	private DesignMapTypePanel<CornerDesign> _corner_col;
	private DesignMapTypePanel<EdgeDesign> _edge_col;
	private DesignMapTypePanel<FaceDesign> _face_col;

	public DesignMapPanel(string title)
	{
		_title = title;
		_corner_col = new DesignMapTypePanel<CornerDesign>("Corner");
		_edge_col = new DesignMapTypePanel<EdgeDesign>("Edge");
		_face_col = new DesignMapTypePanel<FaceDesign>("Face");
		
	}

	public override void Enable()
	{
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_corner_col.Enable();
		_edge_col.Enable();
		_face_col.Enable();
	}

	public override void Disable()
	{
		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
		target = null;

		_corner_col.Disable();
		_edge_col.Disable();
		_face_col.Disable();
	}

	public override bool updateMesh {
		get => _update_mesh || _corner_col.updateMesh || _edge_col.updateMesh || _face_col.updateMesh;
		set {
			_update_mesh = value;
			_corner_col.updateMesh = value;
			_edge_col.updateMesh = value;
			_face_col.updateMesh = value;
		}
	}

	public override bool renderMesh {
		get => _render_mesh || _corner_col.renderMesh || _edge_col.renderMesh || _face_col.renderMesh;
		set {
			_render_mesh = value;
			_corner_col.renderMesh = value;
			_edge_col.renderMesh = value;
			_face_col.renderMesh = value;
		}
	}

	public override void DrawGUI(Rect rect)
	{
		//update targets
		_corner_col.target = target;
		_edge_col.target = target;
		_face_col.target = target;
		//update lists
		_corner_col.UpdateLists();
		_edge_col.UpdateLists();
		_face_col.UpdateLists();
		//update layout rects
		UpdateLayoutRects(rect);
		//disable if no selection
		EditorGUI.BeginDisabledGroup(target == null);
		//draw title panel
		float button_width = Mathf.Min(60f, _rect_header.width / 2f);
		VxlGUI.DrawRect(_rect_header, "DarkGradient");
		GUI.Label(
			VxlGUI.GetLeftElement(_rect_header, 0, _rect_header.width - button_width),
			_title,
			GUI.skin.GetStyle("LeftLightHeader")
		);
		//draw columns
		_corner_col.DrawGUI(_rect_corner);
		_edge_col.DrawGUI(_rect_edge);
		_face_col.DrawGUI(_rect_face);
		//end selection disable
		EditorGUI.EndDisabledGroup();
	}

	private void UpdateLayoutRects(Rect rect)
	{
		_rect_header = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR);
		Rect body = VxlGUI.GetSandwichedRectY(rect, VxlGUI.MED_BAR + VxlGUI.MED_SPACE, 0);
		float col_width = (body.width - (2 * VxlGUI.SM_SPACE)) / 3f;
		_rect_corner = VxlGUI.GetLeftElement(body, 0, col_width, VxlGUI.SM_SPACE, 0);
		_rect_edge = VxlGUI.GetLeftElement(body, 1, col_width, VxlGUI.SM_SPACE, 0);
		_rect_face = VxlGUI.GetLeftElement(body, 2, col_width, VxlGUI.SM_SPACE, 0);
	}
}