using UnityEngine;

public class MappingModeGUI : ModeGUI<MapObject>
{
	public MappingModeGUI()
	{
		_title = "Selected Mapping";
		_mode = 0;
		_modes = new PanelGUI[] {
			new SelectionPanel<MapObject>("Mapping Selection"),
			new DesignMapPanel("Design Selection"),
			new CornerMapPanel("Corner Mapping"),
			new LateralMapPanel("Lateral Mapping"),
			new LongitudeMapPanel("Longitude Mapping"),
			new RectMapPanel("Rect Mapping"),
			new HexagonMapPanel("Hexagon Mapping")
		};
		_mode_labels = new string[] {
			"Select", "Designs", "Corner", "Lateral", "Longitude", "Rect", "Hexagon"
		};
		_preview = new MeshPreview();
		_preview.setVoxelFrame = false;
		_preview.setVoxelFlip = false;
	}
	protected override void UpdateModes()
	{
		if (_modes == null || _mode < 0 || _mode >= _modes.Length) {
			return;
		}
		switch (_mode) {
			case 0:
				SelectionPanel<MapObject> select = (SelectionPanel<MapObject>)_modes[_mode];
				if (selected != select.selected) {
					selected = select.selected;
				}
				break;
			case 1:
				DesignMapPanel dmpanel = (DesignMapPanel)_modes[_mode];
				dmpanel.target = selected;
				break;
			case 2:
				CornerMapPanel cmpanel = (CornerMapPanel)_modes[_mode];
				cmpanel.target = selected;
				break;
			case 3:
				LateralMapPanel empanel = (LateralMapPanel)_modes[_mode];
				empanel.target = selected;
				break;
			case 4:
				LongitudeMapPanel lgmpanel = (LongitudeMapPanel)_modes[_mode];
				lgmpanel.target = selected;
				break;
			case 5:
				RectMapPanel rmpanel = (RectMapPanel)_modes[_mode];
				rmpanel.target = selected;
				break;
			case 6:
				HexagonMapPanel hmpanel = (HexagonMapPanel)_modes[_mode];
				hmpanel.target = selected;
				break;
		}
	}
}