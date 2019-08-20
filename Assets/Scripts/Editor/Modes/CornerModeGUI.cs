using System.Collections.Generic;

public class CornerModeGUI : ModeGUI<CornerDesign>
{
	private readonly string[] EDGESOCKET_LABELS = new string[] {
		"Above Socket", "Below Socket", "D0 Socket", "D2 Socket", "D4 Socket"
	};
	private readonly string[] FACESOCKET_LABELS = new string[] {
		"Above D0 Socket", "Below D0 Socket",
		"Above D1 Socket", "Below D1 Socket",
		"Above D2 Socket", "Below D2 Socket",
		"Above D3 Socket", "Below D3 Socket",
		"Above D4 Socket", "Below D4 Socket",
		"Above D5 Socket", "Below D5 Socket",
		"Middle D3 Socket",
		"Middle D5 Socket",
		"Middle D1 Socket"
	};

	public CornerModeGUI()
	{
		_title = "Selected Corner";
		_mode = 0;
		_modes = new PanelGUI[] {
			new SelectionPanel<CornerDesign>("Corner Selection"),
			new VertexPanel("Vertices"),
			new TrianglePanel("Triangles", new string[0], new string[0]),
			new SocketPanel("Edge Sockets", EDGESOCKET_LABELS, CornerDesign.EDGESOCKET_CNT, PreviewDrawMode.EdgeSocket),
			new SocketPanel("Face Sockets", FACESOCKET_LABELS, CornerDesign.FACESOCKET_CNT, PreviewDrawMode.FaceSocket)
		};
		_mode_labels = new string[] {
			"Selection", "Vertex", "Triangle", "Edge Socket", "Face Socket"
		};
	}

	protected override void UpdateModes()
	{
		if (_modes == null || _mode < 0 || _mode >= _modes.Length) {
			return;
		}
		switch (_mode) {
			case 0:
				SelectionPanel<CornerDesign> select = (SelectionPanel<CornerDesign>)_modes[_mode];
				if (selected != select.selected) {
					selected = select.selected;
				}
				break;
			case 1:
				VertexPanel vertex = (VertexPanel) _modes[_mode];
				vertex.target = selected;
				break;
			case 2:
				TrianglePanel tri = (TrianglePanel )_modes[_mode];
				tri.target = selected;
				break;
			case 3:
				SocketPanel edge = (SocketPanel)_modes[_mode];
				edge.target = selected;
				if (selected != null) {
					edge.sockets = selected.edgesockets;
				}
				else {
					edge.sockets = null;
				}
				break;
			case 4:
				SocketPanel face = (SocketPanel)_modes[_mode];
				face.target = selected;
				if (selected != null) {
					face.sockets = selected.facesockets;
				}
				else {
					face.sockets = null;
				}
				break;
		}
	}
}