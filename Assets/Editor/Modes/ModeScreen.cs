using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModeScreen
{
	public VoxelMapping map;
	public VoxelDesign design;
	public VoxelPattern pattern;

	protected string _title;
	protected MeshPreview _preview;

	public string title {
		get {
			return _title;
		}
	}
	public virtual bool IsInputValid(ModeScreen panel)
	{
		return true;
	}
	public abstract void Enable();
	public abstract void Disable();
	public abstract void DrawGUI(Rect rect);
}
