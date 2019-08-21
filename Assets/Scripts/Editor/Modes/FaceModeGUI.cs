using System.Collections.Generic;

public class FaceModeGUI : ModeGUI<FaceDesign>
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

	public FaceModeGUI()
	{
		_title = "Selected Face";
		_mode = 0;
		_modes = new PanelGUI[] {
			new SelectionPanel<FaceDesign>("Face Selection"),
			new VertexPanel("Vertices"),
			new TrianglePanel("Triangles", TRIANGLE_CORNERPLUG_OPTIONS, TRIANGLE_EDGEPLUG_OPTIONS),
			new PlugPanel("Corner Plugs", CORNERPLUG_LABELS, FaceDesign.CORNERPLUG_CNT),
			new PlugPanel("Edge Plugs", EDGEPLUG_LABELS, FaceDesign.EDGEPLUG_CNT)
		};
		_mode_labels = new string[] {
			"Select", "Vertex", "Triangle", "Corner Plug", "Edge Plug"
		};
	}

	protected override void UpdateModes()
	{
		if (_modes == null || _mode < 0 || _mode >= _modes.Length) {
			return;
		}
		switch (_mode) {
			case 0:
				SelectionPanel<FaceDesign> select = (SelectionPanel<FaceDesign>)_modes[_mode];
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