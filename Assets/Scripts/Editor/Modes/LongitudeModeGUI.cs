using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongitudeModeGUI : ModeGUI<LongitudeDesign>
{
	private readonly string[] TRIANGLE_CORNERPLUG_OPTIONS = new string[] {
		"Below", "Above"
	};
	private readonly string[] CORNERPLUG_LABELS = new string[] {
		"Below", "Above"
	};
	private readonly string[] FACESOCKET_LABELS = new string[] {
		"D0", "D2", "D4"
	};
	public LongitudeModeGUI()
	{
		_title = "Selected Edge";
		_preview = new MeshPreview();
		_preview.setVoxelFrame = true;
		_preview.setVoxelFlip = false;
		_preview.setFramePos = new Vector3(-1, 0, -Vx.SQRT3_R1);
		_mode = 0;
		_modes = new PanelGUI[] {
			new SelectionPanel<LongitudeDesign>("Edge Selection"),
			new VertexPanel("Vertices", Quaternion.Euler(0, 60f, 0)),
			new TrianglePanel(_preview, "Triangles", TRIANGLE_CORNERPLUG_OPTIONS, new string[0]),
			new PlugPanel("Corner Plugs", CORNERPLUG_LABELS, CORNERPLUG_LABELS.Length),
			new SocketPanel(_preview, "Face Sockets", false, FACESOCKET_LABELS)
		};
		_mode_labels = new string[] {
			"Select", "Vertex", "Triangle", "Corner Plug", "Face Socket"
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
				SelectionPanel<LongitudeDesign> select = (SelectionPanel<LongitudeDesign>)_modes[_mode];
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
				SocketPanel face = (SocketPanel)_modes[_mode];
				face.target = selected;
				break;
		}
	}
}
