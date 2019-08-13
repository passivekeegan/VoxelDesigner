using UnityEngine;

public abstract class PanelGUI
{
	public bool repaint;
	public bool update;

	protected string _title;

	public abstract void DrawGUI(Rect rect);

	public abstract void Enable();
	public abstract void Disable();
}
