using UnityEngine;

public abstract class PanelGUI
{
	public bool repaint;
	public bool update;
	
	protected string _title;

	public abstract void DrawGUI(Rect rect);

	public virtual PreviewDrawMode previewMode {
		get {
			return PreviewDrawMode.None;
		}
	}

	public virtual int primary_index {
		get {
			return -1;
		}
	}

	public virtual int secondary_index {
		get {
			return -1;
		}
	}

	public abstract void Enable();
	public abstract void Disable();
}
