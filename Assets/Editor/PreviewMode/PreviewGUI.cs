using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class PreviewGUI 
{
	public bool updaterender;
	public bool updatemesh;

	public abstract void Enable();
	public abstract void Disable();
	public virtual void DrawGUI(Rect rect) { }
	public virtual void DrawDisplayObjects(PreviewRenderUtility prevutility) { }
	public virtual void ProcessInput(PreviewRenderUtility prevutility, Rect rect) { }
}
