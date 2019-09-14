using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonModeGUI : ModeGUI<HexagonFaceDesign>
{
	private readonly string[] TRIANGLE_CORNERPLUG_OPTIONS = new string[] {
		"D0", "D1", "D2", "D3", "D4", "D5"
	};
	private readonly string[] TRIANGLE_EDGEPLUG_OPTIONS = new string[] {
		"D0", "D1", "D2", "D3", "D4", "D5"
	};
	private readonly string[] CORNERPLUG_LABELS = new string[] {
		"D0 Plug", "D1 Plug", "D2 Plug", "D3 Plug", "D4 Plug", "D5 Plug"
	};

	private readonly string[] EDGEPLUG_LABELS = new string[] {
		"D0 Plug", "D1 Plug", "D2 Plug", "D3 Plug", "D4 Plug", "D5 Plug"
	};

	public HexagonModeGUI()
	{
		_title = "Selected Face";
		_preview = new MeshPreview();
		_preview.setVoxelFrame = true;
		_preview.setVoxelFlip = true;
		_preview.setFramePos = new Vector3(0, -0.5f, 0);
		_preview.setFrameAltPos = new Vector3(0, 0.5f, 0);
		_mode = 0;
		_modes = new PanelGUI[] {
			new SelectionPanel<HexagonFaceDesign>("Face Selection"),
			new VertexPanel("Vertices", Quaternion.identity),
			new TrianglePanel(_preview , "Triangles", TRIANGLE_CORNERPLUG_OPTIONS, TRIANGLE_EDGEPLUG_OPTIONS),
			new PlugPanel("Corner Plugs", CORNERPLUG_LABELS, CORNERPLUG_LABELS.Length),
			new PlugPanel("Edge Plugs", EDGEPLUG_LABELS, EDGEPLUG_LABELS.Length)
		};
		_mode_labels = new string[] {
			"Select", "Vertex", "Triangle", "Corner Plug", "Edge Plug"
		};

	}

	protected override void UpdatePreview()
	{
		_preview.target = selected;
		switch (_mode) {
			case 1:
				VertexPanel vertex = (VertexPanel)_modes[_mode];
				_preview.vertexMode = VertexMode.PrimarySelect;
				_preview.UpdatePrimarySelection(vertex.selectlist);
				break;
			case 2:
				TrianglePanel tri = (TrianglePanel)_modes[_mode];
				_preview.vertexMode = VertexMode.PrimarySelectTriangle;
				_preview.UpdatePrimarySelection(tri.selectlist);
				break;
			default:
				_preview.vertexMode = VertexMode.None;
				break;
		}
	}
	protected override void UpdateModes()
	{
		if (_modes == null || _mode < 0 || _mode >= _modes.Length) {
			return;
		}
		switch (_mode) {
			case 0:
				SelectionPanel<HexagonFaceDesign> select = (SelectionPanel<HexagonFaceDesign>)_modes[_mode];
				if (selected != select.selected) {
					selected = select.selected;
				}
				break;
			case 1:
				VertexPanel vertex = (VertexPanel)_modes[_mode];
				vertex.target = selected;
				break;
			case 2:
				TrianglePanel tri = (TrianglePanel)_modes[_mode];
				tri.target = selected;
				break;
			case 3:
				PlugPanel corner = (PlugPanel)_modes[_mode];
				corner.target = selected;
				if (selected != null) {
					corner.plugs = selected.cornerplugs;
				}
				else {
					corner.plugs = null;
				}
				break;
			case 4:
				PlugPanel edge = (PlugPanel)_modes[_mode];
				edge.target = selected;
				if (selected != null) {
					edge.plugs = selected.edgeplugs;
				}
				else {
					edge.plugs = null;
				}
				break;
		}
	}
}
