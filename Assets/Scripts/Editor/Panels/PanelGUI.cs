using UnityEngine;

public abstract class PanelGUI
{
	protected bool _repaint_menu;
	protected bool _update_mesh;
	protected bool _render_mesh;
	
	protected string _title;

	public abstract void DrawGUI(Rect rect);

	public virtual PreviewDrawMode previewMode {
		get {
			return PreviewDrawMode.None;
		}
	}

	public virtual bool repaintMenu {
		get => _repaint_menu;
		set => _repaint_menu = value;
	}

	public virtual bool updateMesh {
		get => _update_mesh;
		set => _update_mesh = value;
	}

	public virtual bool renderMesh {
		get => _render_mesh;
		set => _render_mesh = value;
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
