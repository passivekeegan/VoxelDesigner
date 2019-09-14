using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DesignMapPanel : PanelGUI
{
	public MappingObject target;

	private Vector2 _scroll;
	private Rect _rect_header;
	private Rect _rect_scroll;
	private Rect _rect_content;
	private Rect _rect_corner;
	private Rect _rect_lateral;
	private Rect _rect_longitude;
	private Rect _rect_rect;
	private Rect _rect_hexagon;
	private DesignMapTypePanel<CornerDesign> _corner_col;
	private DesignMapTypePanel<LatEdgeDesign> _lat_col;
	private DesignMapTypePanel<LongEdgeDesign> _long_col;
	private DesignMapTypePanel<RectFaceDesign> _rect_col;
	private DesignMapTypePanel<HexagonFaceDesign> _hex_col;


	public DesignMapPanel(string title)
	{
		_title = title;
		_corner_col = new DesignMapTypePanel<CornerDesign>("Corner");
		_lat_col = new DesignMapTypePanel<LatEdgeDesign>("Lateral");
		_long_col = new DesignMapTypePanel<LongEdgeDesign>("Longitude");
		_rect_col = new DesignMapTypePanel<RectFaceDesign>("Rect");
		_hex_col = new DesignMapTypePanel<HexagonFaceDesign>("Hexagon");
		
	}

	public override void Enable()
	{
		_scroll = Vector2.zero;
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_corner_col.Enable();
		_lat_col.Enable();
		_long_col.Enable();
		_rect_col.Enable();
		_hex_col.Enable();
	}

	public override void Disable()
	{
		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
		target = null;

		_corner_col.Disable();
		_lat_col.Disable();
		_long_col.Disable();
		_rect_col.Disable();
		_hex_col.Disable();
	}

	public override bool updateMesh {
		get => _update_mesh || _corner_col.updateMesh || _lat_col.updateMesh || _long_col.updateMesh || _rect_col.updateMesh || _hex_col.updateMesh;
		set {
			_update_mesh = value;
			_corner_col.updateMesh = value;
			_lat_col.updateMesh = value;
			_long_col.updateMesh = value;
			_rect_col.updateMesh = value;
			_hex_col.updateMesh = value;
		}
	}

	public override bool renderMesh {
		get => _render_mesh || _corner_col.renderMesh || _lat_col.renderMesh || _long_col.renderMesh || _rect_col.renderMesh || _hex_col.renderMesh;
		set {
			_render_mesh = value;
			_corner_col.renderMesh = value;
			_lat_col.renderMesh = value;
			_long_col.renderMesh = value;
			_rect_col.renderMesh = value;
			_hex_col.renderMesh = value;
		}
	}

	public override void DrawGUI(Rect rect)
	{
		//update targets
		_corner_col.target = target;
		_lat_col.target = target;
		_long_col.target = target;
		_rect_col.target = target;
		_hex_col.target = target;
		//update lists
		_corner_col.UpdateLists();
		_lat_col.UpdateLists();
		_long_col.UpdateLists();
		_rect_col.UpdateLists();
		_hex_col.UpdateLists();
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


		_scroll = GUI.BeginScrollView(_rect_scroll, _scroll, _rect_content);
		//draw columns
		_corner_col.DrawGUI(_rect_corner);
		_lat_col.DrawGUI(_rect_lateral);
		_long_col.DrawGUI(_rect_longitude);
		_rect_col.DrawGUI(_rect_rect);
		_hex_col.DrawGUI(_rect_hexagon);
		GUI.EndScrollView();
		
		//end selection disable
		EditorGUI.EndDisabledGroup();
	}

	private void UpdateLayoutRects(Rect rect)
	{
		_rect_header = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR);
		_rect_scroll = VxlGUI.GetSandwichedRectY(rect, VxlGUI.MED_BAR + VxlGUI.MED_SPACE, 0);

		float col_width = Mathf.Max(200, (_rect_scroll.width - (4 * VxlGUI.SM_SPACE)) / 5f);
		float content_width = (5 * col_width) + (4 * VxlGUI.SM_SPACE);
		_rect_content = VxlGUI.GetHorizontalScrollViewRect(_rect_scroll.height, _rect_scroll.width, content_width);
		_rect_corner = VxlGUI.GetLeftElement(_rect_content, 0, col_width, VxlGUI.SM_SPACE, 0);
		_rect_lateral = VxlGUI.GetLeftElement(_rect_content, 1, col_width, VxlGUI.SM_SPACE, 0);
		_rect_longitude = VxlGUI.GetLeftElement(_rect_content, 2, col_width, VxlGUI.SM_SPACE, 0);
		_rect_rect = VxlGUI.GetLeftElement(_rect_content, 3, col_width, VxlGUI.SM_SPACE, 0);
		_rect_hexagon = VxlGUI.GetLeftElement(_rect_content, 4, col_width, VxlGUI.SM_SPACE, 0);
	}
}