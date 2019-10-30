using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditMeshMode : ModeScreen
{
	private int _submode;
	private Rect _rect_title;
	private Rect _rect_vertex;
	private Rect _rect_triangle;
	private Rect _rect_import;
	private Rect _rect_submode;
	private SubModeScreen[] _submodes;

	public EditMeshMode(string title, MeshPreview preview, Mesh vertex_mesh, Material vertex_material)
	{
		_title = title;
		_preview = preview;
		_submodes = new SubModeScreen[] {
			new VertexSubMode("Vertex", _preview, vertex_mesh, vertex_material),
			new TriangleSubMode("Triangle", _preview, vertex_mesh, vertex_material),
			new ImportSubMode("Import", _preview)
		};
	}

	public override void Enable()
	{
		_submode = 0;
		_submodes[_submode].Enable(map, design);
	}

	public override void Disable()
	{
		_submodes[_submode].Disable();
	}

	public override void DrawGUI(Rect rect)
	{
		//update layout rects
		UpdateLayoutRects(rect);


		//draw title
		GUI.Label(_rect_title, pattern.ToString(), "LeftListElement");
		int submode = _submode;
		if (GUI.Toggle(_rect_vertex, submode == 0, "Vertex", "DarkField10")) {
			submode = 0;
		}
		if (GUI.Toggle(_rect_triangle, submode == 1, "Triangle", "DarkField10")) {
			submode = 1;
		}
		if (GUI.Toggle(_rect_import, submode == 2, "Import", "DarkField10")) {
			submode = 2;
		}
		if (_submode != submode) {
			_submodes[_submode].Disable();
			_submode = submode;
			_submodes[_submode].Enable(map, design);
		}
		//draw panel
		_submodes[_submode].DrawGUI(_rect_submode);
	}

	public override bool IsInputValid(ModeScreen panel)
	{
		return (panel.map != null) && (panel.design != null) && (!panel.pattern.isEmpty);
	}

	private void UpdateLayoutRects(Rect rect)
	{
		_rect_title = EVx.GetAboveElement(rect, 0, EVx.MED_LINE);
		Rect rect_modes = EVx.GetAboveElement(rect, 0, EVx.LRG_LINE, EVx.MED_LINE + EVx.SM_SPACE);
		_rect_vertex = EVx.GetPaddedRect(EVx.GetLeftElement(rect_modes, 0, rect_modes.width / 3f), 1);
		_rect_triangle = EVx.GetPaddedRect(EVx.GetLeftElement(rect_modes, 1, rect_modes.width / 3f), 1);
		_rect_import = EVx.GetPaddedRect(EVx.GetLeftElement(rect_modes, 2, rect_modes.width / 3f), 1);
		_rect_submode = EVx.GetSandwichedRectY(rect, EVx.LRG_LINE + EVx.MED_LINE + (2 * EVx.SM_SPACE), 0);
	}
}
