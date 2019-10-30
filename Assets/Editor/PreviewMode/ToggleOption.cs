public struct ToggleOption
{
	public int level;
	public string label;
	public bool value;
	public bool repaint_menu;
	public bool update_mesh;
	public bool update_render;

	public ToggleOption(ToggleOption oldoption, bool newvalue)
	{
		this.level = oldoption.level;
		this.label = oldoption.label;
		this.value = newvalue;
		this.repaint_menu = oldoption.repaint_menu;
		this.update_mesh = oldoption.update_mesh;
		this.update_render = oldoption.update_render;
	}

	public ToggleOption(int level, string label, bool value, bool repaint_menu, bool update_mesh, bool update_render)
	{
		this.level = level;
		this.label = label;
		this.value = value;
		this.repaint_menu = repaint_menu;
		this.update_mesh = update_mesh;
		this.update_render = update_render;
	}

	public bool isValid {
		get {
			return (!string.IsNullOrEmpty(label)) && level >= 0;
		}
	}

	public static ToggleOption empty {
		get {
			return new ToggleOption(-1, "", false, false, false, false);
		}
	}
}