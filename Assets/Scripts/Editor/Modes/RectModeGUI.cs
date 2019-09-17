using UnityEngine;

public class RectModeGUI : ModeGUI<RectDesign>
{
	private readonly string[] TRIANGLE_CORNERPLUG_OPTIONS = new string[] {
		"A0", "B0", "B1", "A1"
	};
	private readonly string[] TRIANGLE_EDGEPLUG_OPTIONS = new string[] {
		"Backward", "Below", "Forward", "Above"
	};
	private readonly string[] CORNERPLUG_LABELS = new string[] {
		"Above Backward", "Below Backward", "Below Forward", "Above Forward"
	};

	private readonly string[] EDGEPLUG_LABELS = new string[] {
		"Backward", "Below", "Forward", "Above"
	};

	public RectModeGUI()
	{
		_title = "Selected Face";
		_preview = new MeshPreview();
		_preview.setVoxelFrame = true;
		_preview.setVoxelFlip = true;
		_preview.setFramePos = new Vector3(-1, 0, 0);
		_preview.setFrameAltPos = new Vector3(1, 0, 0);
		_mode = 0;
		_modes = new PanelGUI[] {
			new SelectionPanel<RectDesign>("Face Selection"),
			new VertexPanel("Vertices", Quaternion.Euler(0, 90f, 0)),
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
				SelectionPanel<RectDesign> select = (SelectionPanel<RectDesign>)_modes[_mode];
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
