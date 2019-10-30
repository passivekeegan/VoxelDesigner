using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VertexGUI : PreviewGUI
{
	protected const int MAX_WIDTH = 200;

	public static string primarykey = "_Primary";
	public static string secondarykey = "_Secondary";

	public bool refresh_vertexlist;
	public VoxelDesign design;
	public VoxelMapping map;
	public Mesh vertexmesh;
	public Material vertexmaterial;
	public Color colour_superprimary = new Color(0.01228193f, 0.8679245f, 0.5222418f);//03DD85
	public Color colour_supersecondary = new Color(0.5638335f, 0.252314f, 0.8490566f);
	public Color colour_selectprimary = new Color(1, 0.9937412f, 0.1098039f);//FFFD1C
	public Color colour_selectsecondary = new Color(0.1098039f, 0.566f, 1);
	public Color colour_normal = new Color(0.9529412f, 0.9529412f, 0.9529412f);//F3F3F3

	private bool _enableshare;
	private bool _limitselection;
	private int _selectlimit;

	private float _pointradius;
	private Rect _rect_scroll;
	private Rect _rect_content;
	private Rect _rect_vertex;
	private Rect _rect_share;
	private Rect _rect_unshare;
	private Rect _rect_selected;
	private Rect _rect_background;
	private Vector2 _scroll;
	private MaterialPropertyBlock _propblock;
	private HashSet<int> _primaryset;
	private HashSet<int> _primarysuper;
	private HashSet<int> _secondaryset;
	private HashSet<int> _secondarysuper;
	private List<int> _primarylist;
	private List<int> _secondarylist;

	public VertexGUI(bool limitselection, int selectlimit, float pointradius, bool enable_share)
	{
		refresh_vertexlist = true;
		_enableshare = enable_share;
		_limitselection = limitselection;
		_selectlimit = Mathf.Max(0, selectlimit);
		_pointradius = pointradius;

		_propblock = new MaterialPropertyBlock();
		_primaryset = new HashSet<int>();
		_primarysuper = new HashSet<int>();
		_secondaryset = new HashSet<int>();
		_secondarysuper = new HashSet<int>();
		_primarylist = new List<int>();
		_secondarylist = new List<int>();
	}

	public override void Enable()
	{
		_scroll = Vector2.zero;
		_propblock.Clear();
		ClearSelections();
	}

	public override void Disable()
	{
		_scroll = Vector2.zero;
		_propblock.Clear();
		ClearSelections();
	}

	public void ClearSelections()
	{
		_primarylist.Clear();
		_secondarylist.Clear();
		_primaryset.Clear();
		_secondaryset.Clear();
		_primarysuper.Clear();
		_secondarysuper.Clear();
	}

	public void UpdatePrimarySelection(List<int> primarylist)
	{
		updaterender = true;
		_primarylist.Clear();
		_primaryset.Clear();
		_primarysuper.Clear();
		if (primarylist == null) {
			return;
		}
		for (int k = 0;k < primarylist.Count;k++) {
			int index = primarylist[k];
			if (!_primaryset.Add(index)) {
				continue;
			}
			if (k == primarylist.Count - 1) {
				_primarysuper.Add(index);
			}
			_primarylist.Add(index);
		}
	}
	public void UpdatePrimarySelection(HashSet<int> primaryset, HashSet<int> primarysuper)
	{
		updaterender = true;
		_primarylist.Clear();
		_primaryset.Clear();
		_primarysuper.Clear();
		if (primaryset == null) {
			return;
		}
		foreach (int index in primaryset) {
			if (!_primaryset.Add(index)) {
				continue;
			}
			if (primarysuper != null && primarysuper.Contains(index)) {
				_primarysuper.Add(index);
			}
			_primarylist.Add(index);
		}
	}

	public override void DrawGUI(Rect rect)
	{
		UpdateLayoutRects(rect);

		if (_secondarylist.Count < 1) {
			return;
		}
		EVx.DrawRect(_rect_background, "MediumBlackTransparent");
		_scroll = GUI.BeginScrollView(_rect_scroll, _scroll, _rect_content);
		GUI.Label(_rect_vertex, "Vertex " + _secondarylist[_secondarylist.Count - 1]);
		GUI.Label(_rect_selected, "Selected " + _secondarylist.Count);

		if (_enableshare) {
			if (GUI.Button(_rect_share, "Share", "WhiteField10")) {
				UpdateShared(true);
			}
			if (GUI.Button(_rect_unshare, "Un-Share", "WhiteField10")) {
				UpdateShared(false);
			}
		}
		GUI.EndScrollView();
	}

	private void UpdateShared(bool value)
	{
		if (map == null || design == null || !map.isValid || !design.isValid || _secondarylist.Count <= 0) {
			return;
		}
		List<Point> points = design.vertices;
		if (points.Count <= 0) {
			return;
		}
		Undo.RecordObject(map, "Updated Selected Vertex Sharing");
		for (int k = 0;k < _secondarylist.Count;k++) {
			int index = _secondarylist[k];
			if (index < 0 || index >= points.Count) {
				continue;
			}
			Point point = points[index];
			if (point.shared == value) {
				continue;
			}
			points[index] = new Point(value, point.vertex);
			updaterender = true;
			refresh_vertexlist = true;
		}
		EditorUtility.SetDirty(map);
	}

	public override void DrawDisplayObjects(PreviewRenderUtility prevutility)
	{
		if (design == null || !design.isValid || vertexmesh == null || vertexmaterial == null) {
			return;
		}
		Vector3 scale = _pointradius * Vector3.one;
		List<Point> vertices = design.vertices;
		for (int k = 0;k < vertices.Count;k++) {
			bool primary_superselect = false;
			bool primary_select = _primaryset.Contains(k);
			if (primary_select) {
				primary_superselect = _primarysuper.Contains(k);
			}
			bool secondary_superselect = false;
			bool secondary_select = _secondaryset.Contains(k);
			if (secondary_select) {
				secondary_superselect = _secondarysuper.Contains(k);
			}
			SetMaterialBlock(primary_select, primary_superselect, secondary_select, secondary_superselect);
			prevutility.DrawMesh(vertexmesh, Matrix4x4.TRS(vertices[k].vertex, Quaternion.identity, scale), vertexmaterial, 0, _propblock);
		}
	}

	public override void ProcessInput(PreviewRenderUtility prevutility, Rect rect)
	{
		switch (Event.current.type) {
			case EventType.MouseDown:
				if (!_rect_background.Contains(Event.current.mousePosition) && Event.current.button == 0 && _selectlimit > 0) {
					float x = Mathf.Clamp01((Event.current.mousePosition.x - rect.xMin) / rect.width);
					float y = 1 - Mathf.Clamp01((Event.current.mousePosition.y - rect.yMin) / rect.height);
					Vector2 viewport_point = new Vector2(x, y);
					Ray ray = prevutility.camera.ViewportPointToRay(viewport_point);
					SelectVertex(ray, Event.current.shift);
				}
				break;
		}
	}

	private void SetMaterialBlock(bool primary_select, bool primary_superselect, bool secondary_select, bool secondary_superselect)
	{
		if (primary_superselect) {
			_propblock.SetColor(primarykey, colour_superprimary);
			if (secondary_superselect) {
				_propblock.SetColor(secondarykey, colour_supersecondary);
			}
			else if (secondary_select) {
				_propblock.SetColor(secondarykey, colour_selectsecondary);
			}
			else {
				_propblock.SetColor(secondarykey, colour_superprimary);
			}
		}
		else if (primary_select) {
			_propblock.SetColor(primarykey, colour_selectprimary);
			if (secondary_superselect) {
				_propblock.SetColor(secondarykey, colour_supersecondary);
			}
			else if (secondary_select) {
				_propblock.SetColor(secondarykey, colour_selectsecondary);
			}
			else {
				_propblock.SetColor(secondarykey, colour_selectprimary);
			}
		}
		else {
			if (secondary_superselect) {
				_propblock.SetColor(primarykey, colour_supersecondary);
				_propblock.SetColor(secondarykey, colour_supersecondary);
			}
			else if (secondary_select) {
				_propblock.SetColor(primarykey, colour_selectsecondary);
				_propblock.SetColor(secondarykey, colour_selectsecondary);
			}
			else {
				_propblock.SetColor(primarykey, colour_normal);
				_propblock.SetColor(secondarykey, colour_normal);
			}
		}
	}

	private void UpdateLayoutRects(Rect rect)
	{
		int row = 1;
		int column = 0;
		rect = EVx.GetGridRect(rect, row, column, 2, 3);
		float width = Mathf.Min(rect.width, MAX_WIDTH);
		float height = Mathf.Min(rect.height, 2 * (EVx.MED_LINE + EVx.MED_PAD));
		if (row == 0) {
			if (column == 0) {
				_rect_background = new Rect(rect.x, rect.y, width, height);
			}
			else if (column == 2) {
				_rect_background = new Rect(rect.x + rect.width - width, rect.y, width, height);
			}
			else {
				_rect_background = new Rect(rect.x + ((rect.width - width) / 2f), rect.y, width, height);
			}
		}
		else {
			if (column == 0) {
				_rect_background = new Rect(rect.x, rect.y + rect.height - height, width, height);
			}
			else if (column == 2) {
				_rect_background = new Rect(rect.x + rect.width - width, rect.y + rect.height - height, width, height);
			}
			else {
				_rect_background = new Rect(rect.x + ((rect.width - width) / 2f), rect.y + rect.height - height, width, height);
			}
		}
		_rect_scroll = EVx.GetPaddedRect(_rect_background, EVx.MED_PAD);
		_rect_content = EVx.GetScrollViewRect(_rect_scroll.width, 2 * EVx.MED_LINE, _rect_scroll.width, _rect_scroll.height);
		float button_width = Mathf.Min(60f, _rect_content.width / 3f);
		Rect row_rect = EVx.GetAboveElement(_rect_content, 0, EVx.MED_LINE);
		if (_enableshare) {
			_rect_vertex = EVx.GetLeftElement(row_rect, 0, row_rect.width - button_width);
			_rect_share = EVx.GetRightElement(row_rect, 0, button_width);
		}
		else {
			_rect_vertex = row_rect;
		}
		row_rect = EVx.GetAboveElement(_rect_content, 1, EVx.MED_LINE);
		if (_enableshare) {
			_rect_selected = EVx.GetLeftElement(row_rect, 0, row_rect.width - button_width);
			_rect_unshare = EVx.GetRightElement(row_rect, 0, button_width);
		}
		else {
			_rect_selected = row_rect;
		}
	}

	#region Vertex Selection
	private void SelectVertex(Ray ray, bool shift)
	{
		updaterender = true;
		if (design == null || !design.isValid || ray.direction == Vector3.zero || _selectlimit <= 0) {
			_secondaryset.Clear();
			_secondarysuper.Clear();
			_secondarylist.Clear();
			_primaryset.Clear();
			_primarysuper.Clear();
			_primarylist.Clear();
			return;
		}
		if (!shift) {
			_secondaryset.Clear();
			_secondarysuper.Clear();
			_secondarylist.Clear();
		}
		int vertex = VertexCast(ray);
		if (vertex < 0) {
			return;
		}
		if (_secondaryset.Add(vertex)) {
			//super selection set
			int remove_index = _secondarylist.Count - _selectlimit;
			//if (remove_index >= 0) {
			//	_secondarysuper.Remove(_secondarylist[remove_index]);
			//}
			if (!_secondarysuper.Contains(vertex)) {
				_secondarysuper.Clear();
			}
			_secondarysuper.Add(vertex);
			//selection set
			if (_limitselection && remove_index >= 0) {
				_secondaryset.Remove(_secondarylist[remove_index]);
			}
			//selection list
			_secondarylist.Add(vertex);
			if (_limitselection && remove_index >= 0) {
				_secondarylist.RemoveAt(remove_index);
			}
		}
		else {
			//selection set
			_secondaryset.Remove(vertex);
			//selection list
			_secondarylist.Remove(vertex);
			//maintain super selection
			if (_secondarysuper.Remove(vertex)) {
				int add_index = _secondarylist.Count - 1;
				if (add_index >= 0) {
					_secondarysuper.Add(_secondarylist[add_index]);
				}
			}
		}
	}

	private int VertexCast(Ray ray)
	{
		if (ray.direction == Vector3.zero || design == null || !design.isValid) {
			return -1;
		}
		float radius_sqr = _pointradius * _pointradius;
		float nearest_t = Mathf.Infinity;
		int nearest_vertex = -1;
		List<Point> vertices = design.vertices;
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertices[k].vertex;
			Vector3 point_vector = ray.origin - vertex;
			float proj_pv = Vector3.Dot(ray.direction, point_vector);
			float sqr_magn = Vector3.SqrMagnitude(point_vector - (proj_pv * ray.direction));
			//check if ray intersects point
			if (sqr_magn > radius_sqr) {
				continue;
			}
			//get t of impact and check if in range
			float t = Mathf.Abs(proj_pv) - Mathf.Sqrt(Mathf.Max(0, radius_sqr - sqr_magn));
			if (t < 0 || t >= nearest_t) {
				continue;
			}
			nearest_t = t;
			nearest_vertex = k;
		}
		return nearest_vertex;
	}
	#endregion
}
