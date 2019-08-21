﻿using System.Collections.Generic;

public class EdgeModeGUI : ModeGUI<EdgeDesign>
{
	private readonly string[] TRIANGLE_CORNERPLUG_OPTIONS = new string[] {
		"Forward", "Backward"
	};
	private readonly string[] CORNERPLUG_LABELS = new string[] {
		"Forwards Plug", "Backwards Plug"
	};
	private readonly string[] FACESOCKET_LABELS = new string[] {
		"Above Socket", "Below Socket", "D0 Socket", "D3 Socket"
	};

	public EdgeModeGUI()
	{
		_title = "Selected Edge";
		_mode = 0;
		_modes = new PanelGUI[] {
			new SelectionPanel<EdgeDesign>("Edge Selection"),
			new VertexPanel("Vertices"),
			new TrianglePanel("Triangles", TRIANGLE_CORNERPLUG_OPTIONS, new string[0]),
			new PlugPanel("Corner Plugs", CORNERPLUG_LABELS, EdgeDesign.CORNERPLUG_CNT),
			new SocketPanel("Face Sockets", FACESOCKET_LABELS, EdgeDesign.FACESOCKET_CNT, PreviewDrawMode.FaceSocket)
		};
		_mode_labels = new string[] {
			"Select", "Vertex", "Triangle", "Corner Plug", "Face Socket"
		};
	}

	protected override void UpdateModes()
	{
		if (_modes == null || _mode < 0 || _mode >= _modes.Length) {
			return;
		}
		switch (_mode) {
			case 0:
				SelectionPanel<EdgeDesign> select = (SelectionPanel<EdgeDesign>)_modes[_mode];
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