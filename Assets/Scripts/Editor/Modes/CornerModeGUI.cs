using UnityEngine;

public class CornerModeGUI : ModeGUI<CornerDesign>
{
	private readonly string[] EDGESOCKET_LABELS = new string[] {
		"Above", "Below", "D0", "D2", "D4"
	};
	private readonly string[] FACESOCKET_LABELS = new string[] {
		"Above D0", "Below D0",
		"Above D2", "Below D2",
		"Above D4", "Below D4",
		"Middle D3",
		"Middle D5",
		"Middle D1"
	};

	public CornerModeGUI()
	{
		_title = "Selected Corner";
		_preview = new MeshPreview();
		_preview.setVoxelFrame = true;
		_preview.setVoxelFlip = true;
		_preview.setFramePos = new Vector3(-1, -0.5f, -Vx.SQRT3_R1);
		_preview.setFrameAltPos = new Vector3(-1, 0.5f, -Vx.SQRT3_R1);
		_mode = 0;
		_modes = new PanelGUI[] {
			new SelectionPanel<CornerDesign>("Corner Selection"),
			new VertexPanel("Vertices", Quaternion.Euler(0, 60f, 0)),
			new TrianglePanel(_preview, "Triangles", new string[0], new string[0]),
			new SocketPanel(_preview, "Edge Sockets", true, EDGESOCKET_LABELS),
			new SocketPanel(_preview, "Face Sockets", false, FACESOCKET_LABELS)
		};
		_mode_labels = new string[] {
			"Selection", "Vertex", "Triangle", "Edge Socket", "Face Socket"
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
			case 3:
				SocketPanel edge = (SocketPanel)_modes[_mode];
				_preview.vertexMode = VertexMode.PrimarySecondarySelet;
				_preview.UpdatePrimarySocketSelection(edge.inverseX, edge.inverseY, true, edge.axiSocket, edge.selectlist);
				break;
			case 4:
				SocketPanel face = (SocketPanel)_modes[_mode];
				_preview.vertexMode = VertexMode.PrimarySecondarySelet;
				_preview.UpdatePrimarySocketSelection(face.inverseX, face.inverseY, false, face.axiSocket, face.selectlist);
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
				break;
			case 4:
				SocketPanel face = (SocketPanel)_modes[_mode];
				face.target = selected;
				break;
		}
	}
}