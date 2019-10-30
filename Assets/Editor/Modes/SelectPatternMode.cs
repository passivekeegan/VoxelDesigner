using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class SelectPatternMode : ModeScreen
{
	private Rect _rect_patscrollbackground;
	private Rect _rect_patscroll;
	private Rect _rect_patcontent;
	private Vector2 _patternscroll;
	private List<VoxelPattern> _patterns;
	private ReorderableList _patternlist;

	public SelectPatternMode(string title, MeshPreview preview)
	{
		map = null;
		pattern = new VoxelPattern();
		design = null;

		_title = title;
		_preview = preview;
		_patterns = new List<VoxelPattern>();
	}

	public override void Enable()
	{
		_patternscroll = Vector2.zero;
		_patterns.Clear();

		_patternlist = new ReorderableList(_patterns, typeof(VoxelPattern), false, false, false, false);
		_patternlist.showDefaultBackground = false;
		_patternlist.headerHeight = 0f;
		_patternlist.footerHeight = 0f;
		_patternlist.elementHeight = (2 * EVx.MED_PAD) + EVx.MED_LINE;
		_patternlist.drawElementBackgroundCallback += DrawPatternElementBackground;
		_patternlist.drawElementCallback += DrawPatternElement;
		_patternlist.onSelectCallback += SelectPatternElement;
		_patternlist.drawNoneElementCallback += DrawPatternNone;

		UpdatePatternList();
		_preview.ClearMesh();
	}

	public override void Disable()
	{
		_patternscroll = Vector2.zero;
		_patternlist = null;
		_patterns.Clear();
		_preview.ClearMesh();
	}

	public override void DrawGUI(Rect rect)
	{
		//udpate pattern
		if (_patternlist.index < 0 || _patternlist.index >= _patterns.Count) {
			pattern = new VoxelPattern();
			design = null;
		}
		//update layout rects
		UpdateRect(rect);
		//map list
		EVx.DrawRect(_rect_patscrollbackground, "LightBlackBorder");
		_patternscroll = GUI.BeginScrollView(_rect_patscroll, _patternscroll, _rect_patcontent);
		_patternlist.DoList(_rect_patcontent);
		GUI.EndScrollView();
	}

	public override bool IsInputValid(ModeScreen panel)
	{
		return (panel.map != null);
	}

	private void UpdateRect(Rect rect)
	{
		_rect_patscrollbackground = rect;
		_rect_patscroll = EVx.GetPaddedRect(_rect_patscrollbackground, 1);
		_rect_patcontent = EVx.GetScrollViewRect(_patternlist, _rect_patscroll.width, _rect_patscroll.height);
	}

	private void UpdatePatternList()
	{
		_patterns.Clear();
		if (map == null) {
			return;
		}
		map.GetPatternList(_patterns);
		int index = _patterns.IndexOf(pattern);
		if (index < 0) {
			return;
		}
		_patternlist.index = index;
	}

	#region Pattern Reorderable List
	private void DrawPatternNone(Rect rect)
	{
		rect = EVx.GetPaddedRect(rect, EVx.MED_PAD, 0);
		GUI.Label(rect, "No VoxelPatterns.", "ListElement");
	}
	private void DrawPatternElement(Rect rect, int index, bool active, bool focused)
	{
		rect = EVx.GetPaddedRect(rect, EVx.MED_PAD, 0);
		GUI.Label(rect, _patterns[index].ToString(), "LeftListElement");
	}
	private void DrawPatternElementBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_patterns.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool selected = (_patterns.Count > 0 && _patternlist.index == index);
		if (selected) {
			EVx.DrawRect(rect, "SelectedListBackground", hover, active, true, false);
		}
		else {
			EVx.DrawRect(rect, "SelectableListBackground", hover, active, false, false);
		}
	}
	private void SelectPatternElement(ReorderableList list)
	{
		int index = list.index;
		if (index < 0 || index >= _patterns.Count) {
			return;
		}
		pattern = _patterns[index];
		design = map.GetVoxelDesign(pattern);
		if (design == null) {
			pattern = new VoxelPattern();
			list.index = -1;
		}
		_preview.UpdateMeshComponents(design, null, null);
	}
	#endregion
}
