public class MappingModeGUI : ModeGUI<MappingObject>
{
	public MappingModeGUI()
	{
		_title = "Selected Mapping";
		_mode = 0;
		_modes = new PanelGUI[] {
			new SelectionPanel<MappingObject>("Mapping Selection"),
			new DesignMapPanel("Design Selection"),
			new CornerMapPanel("Corner Mapping"),
			new EdgeMapPanel("Edge Mapping"),
			new FaceMapPanel("Face Mapping")
		};
		_mode_labels = new string[] {
			"Select", "Designs", "Corner", "Edge", "Face"
		};
	}
	protected override void UpdateModes()
	{
		if (_modes == null || _mode < 0 || _mode >= _modes.Length) {
			return;
		}
		switch (_mode) {
			case 0:
				SelectionPanel<MappingObject> select = (SelectionPanel<MappingObject>)_modes[_mode];
				if (_selected != select.selected) {
					_selected = select.selected;
				}
				break;
			case 1:
				DesignMapPanel dmpanel = (DesignMapPanel)_modes[_mode];
				dmpanel.target = _selected;
				break;
			case 2:
				CornerMapPanel cmpanel = (CornerMapPanel)_modes[_mode];
				cmpanel.target = _selected;
				break;
			case 3:
				EdgeMapPanel empanel = (EdgeMapPanel)_modes[_mode];
				empanel.target = _selected;
				break;
			case 4:
				FaceMapPanel fmpanel = (FaceMapPanel)_modes[_mode];
				fmpanel.target = _selected;
				break;
		}
	}
}