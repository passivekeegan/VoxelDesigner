using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class SocketPanel : PanelGUI
{
	public VoxelComponent target;
	public List<List<int>> sockets;

	private int _primary_index;
	private int _secondary_index;

	private int _socketcnt;
	private PreviewDrawMode _drawmode;
	private Vector2 _scroll;
	private Rect _rect_title;
	private Rect _rect_scroll;
	private Rect _rect_content;
	private List<Rect> _socketrects;
	private List<string> _socket_labels;
	private List<List<int>> _sockets;
	private List<ReorderableList> _socketlists;

	public SocketPanel(string title, string[] labels, int socket_count, PreviewDrawMode drawmode)
	{
		_title = title;
		_drawmode = drawmode;
		_primary_index = -1;
		_secondary_index = -1;
		_socketcnt = socket_count;
		_socketrects = new List<Rect>();
		_socket_labels = new List<string>();
		_sockets = new List<List<int>>();
		_socketlists = new List<ReorderableList>();
		for (int k = 0; k < _socketcnt; k++) {
			//initialize socket rects
			_socketrects.Add(new Rect());
			//initialize socket labels
			string label = "";
			if (labels != null && k < labels.Length && labels[k] != null) {
				label = labels[k].Trim();
			}
			_socket_labels.Add(label);
			_sockets.Add(new List<int>());
			_socketlists.Add(null);
		}
}

	public override void Enable()
	{
		_scroll = Vector2.zero;
		_primary_index = -1;
		_secondary_index = -1;
		for (int k = 0; k < _socketcnt; k++) {
			//initialize socket int lists and reorderable lists
			int list_index = k;
			ReorderableList reorderlist = new ReorderableList(_sockets[k], typeof(int), true, false, false, false);
			reorderlist.showDefaultBackground = false;
			reorderlist.headerHeight = 0;
			reorderlist.footerHeight = 0;
			reorderlist.elementHeight = VxlGUI.MED_BAR;
			reorderlist.drawNoneElementCallback += DrawSocketNoneElement;
			reorderlist.onAddCallback += (ReorderableList list) => {
				AddSocketElement(list, list_index);
			};
			reorderlist.onRemoveCallback += (ReorderableList list) => {
				DeleteSocketElement(list, list_index);
			};
			reorderlist.drawElementCallback += (Rect rect, int index, bool active, bool focused) => {
				DrawSocketElement(list_index, rect, index, active, focused);
			};
			reorderlist.drawElementBackgroundCallback += (Rect rect, int index, bool active, bool focused) => {
				DrawSocketElementBackground(list_index, rect, index, active, focused);
			};
			reorderlist.onReorderCallbackWithDetails += (ReorderableList list, int old_index, int new_index) => {
				ReorderSocketElement(list, list_index, old_index, new_index);
			};
			reorderlist.onSelectCallback += (ReorderableList list) => {
				SelectSocketElement(list_index, list);
			};
			_socketlists[k] = reorderlist;
		}
	}

	public override void Disable()
	{
		target = null;
		_primary_index = -1;
		_secondary_index = -1;
		for (int k = 0; k < _socketcnt; k++) {
			_socketlists[k] = null;
			_sockets[k].Clear();
		}
	}

	public override PreviewDrawMode previewMode {
		get {
			return _drawmode;
		}
	}

	public override int primary_index {
		get {
			return _primary_index;
		}
	}
	public override int secondary_index {
		get {
			return _secondary_index;
		}
	}

	public override void DrawGUI(Rect rect)
	{
		//update socket lists
		UpdateSocketList();
		//calculate rects
		RecalculateRects(rect);
		//disable panel condition
		EditorGUI.BeginDisabledGroup(!ValidSocketCheck());
		//Title
		VxlGUI.DrawRect(_rect_title, "DarkGradient");
		GUI.Label(_rect_title, _title, GUI.skin.GetStyle("LeftLightHeader"));
		//socket lists
		VxlGUI.DrawRect(_rect_scroll, "DarkWhite");
		_scroll = GUI.BeginScrollView(_rect_scroll, _scroll, _rect_content);
		for (int k = 0;k < _socketcnt;k++) {
			//title
			Rect rect_sockettitle = VxlGUI.GetAboveElement(_socketrects[k], 0, VxlGUI.MED_BAR);
			VxlGUI.DrawRect(rect_sockettitle, "DarkGradient");
			GUI.Label(rect_sockettitle, _socket_labels[k], GUI.skin.GetStyle("LeftLightSubHeader"));
			//calculate bottom rects
			Rect rect_bottom = VxlGUI.GetSandwichedRectY(_socketrects[k], VxlGUI.MED_BAR + VxlGUI.SM_SPACE, 0);
			Rect rect_panel = VxlGUI.GetLeftElement(rect_bottom, 0, VxlGUI.SM_BAR);
			//add button
			Rect rect_add = VxlGUI.GetAboveElement(rect_panel, 0, VxlGUI.SM_BAR);
			VxlGUI.DrawRect(rect_add, "DarkGradient");
			if (GUI.Button(VxlGUI.GetPaddedRect(rect_add, 2), "", GUI.skin.GetStyle("Plus"))) {
				_socketlists[k].onAddCallback(_socketlists[k]);
			}
			//delete button
			Rect rect_delete = VxlGUI.GetAboveElement(rect_panel, 1, VxlGUI.SM_BAR, VxlGUI.SM_SPACE, 0);
			EditorGUI.BeginDisabledGroup(_socketlists[k].index < 0 || _socketlists[k].index >= _socketlists[k].count);
			VxlGUI.DrawRect(rect_delete, "DarkGradient");
			if (GUI.Button(VxlGUI.GetPaddedRect(rect_delete, 2), "", GUI.skin.GetStyle("Minus"))) {
				_socketlists[k].onRemoveCallback(_socketlists[k]);
			}
			EditorGUI.EndDisabledGroup();
			//socket list
			Rect rect_socketlist = VxlGUI.GetSandwichedRectX(rect_bottom, VxlGUI.SM_BAR + VxlGUI.SM_SPACE, 0);
			VxlGUI.DrawRect(rect_socketlist, "DarkWhite");
			_socketlists[k].DoList(rect_socketlist);
		}
		GUI.EndScrollView();
		EditorGUI.EndDisabledGroup();
	}

	private bool ValidSocketCheck()
	{
		if (target == null || sockets == null || sockets.Count != _socketcnt) {
			return false;
		}
		for (int k = 0;k < _socketcnt;k++) {
			if (sockets[k] == null) {
				return false;
			}
		}
		return true;
	}

	private void UpdateSocketList()
	{
		//clear lists
		for (int k = 0;k < _socketcnt;k++) {
			_sockets[k].Clear();
		}
		//check if valid sockets to update
		if (!ValidSocketCheck()) {
			return;
		}
		//update socket lists
		for (int k = 0; k < _socketcnt; k++) {
			_sockets[k].AddRange(sockets[k]);
		}
	}

	private void RecalculateRects(Rect rect)
	{
		_rect_title = VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR);
		_rect_scroll = VxlGUI.GetSandwichedRectY(rect, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, 0);

		GUIStyle vscroll_style = GUI.skin.verticalScrollbar;
		float list_width = _rect_scroll.width;
		if (vscroll_style != null) {
			list_width -= vscroll_style.fixedWidth;
		}
		float height = 0f;
		for (int k = 0; k < _socketcnt; k++) {
			if (k > 0) {
				height += VxlGUI.MED_SPACE;
			}
			float list_height = Mathf.Max(2 * (VxlGUI.MED_BAR + VxlGUI.SM_SPACE), _socketlists[k].GetHeight()) + VxlGUI.SM_SPACE + VxlGUI.MED_BAR + (2 * VxlGUI.MED_PAD);
			_socketrects[k] = VxlGUI.GetPaddedRect(new Rect(0, height, list_width, list_height), VxlGUI.MED_PAD);
			height += list_height;
		}
		float width = _rect_scroll.width;
		if (height > _rect_scroll.height && vscroll_style != null) {
			width = list_width;
		}
		_rect_content = new Rect(0, 0, width, height);
	}

	#region Socket ReorderableList
	private void DrawSocketNoneElement(Rect rect)
	{
		EditorGUI.LabelField(rect, "No Sockets.", GUI.skin.GetStyle("RightListLabel"));
	}
	private void DrawSocketElement(int list_index, Rect rect, int index, bool isActive, bool isFocused)
	{
		if (target == null || sockets == null) {
			return;
		}
		if (list_index < 0 || list_index >= sockets.Count || sockets[list_index] == null) {
			return;
		}
		if (index < 0 || index >= sockets[list_index].Count) {
			return;
		}
		float max_element_width = 100f;
		float field_width = Mathf.Min(max_element_width, 0.5f * rect.width);
		//draw left label
		EditorGUI.LabelField(
			VxlGUI.GetLeftElement(rect, 0, rect.width - field_width),
			"Socket " + index
		);
		//draw int field
		EditorGUI.BeginChangeCheck();
		int vertex = EditorGUI.IntField(
			VxlGUI.GetRightElement(rect, 0, field_width),
			sockets[list_index][index]
		);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(target, "Update Socket");
			sockets[list_index][index] = vertex;
			update = true;
			repaint = true;
			//dirty target object
			EditorUtility.SetDirty(target);
		}
	}
	private void DrawSocketElementBackground(int list_index, Rect rect, int index, bool active, bool focus)
	{
		if (list_index < 0 || list_index >= _socketcnt) {
			return;
		}
		bool hover = (_sockets[list_index].Count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (_sockets[list_index].Count > 0) && (_socketlists[list_index].index == index);
		bool valid = true;
		
		if (index >= 0 && index < _sockets[list_index].Count) {
			int vertex_cnt = 0;
			if (target != null && target.vertices != null) {
				vertex_cnt = target.vertices.Count;
			}
			valid = (_sockets[list_index][index] >= 0) && (_sockets[list_index][index] < vertex_cnt);
		}
		if (valid) {
			VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
		}
		else {
			VxlGUI.DrawRect(rect, "SelectableRed", hover, active, on, focus);
		}
	}
	private void AddSocketElement(ReorderableList list, int list_index)
	{
		if (target == null || sockets == null) {
			return;
		}
		if (list_index < 0 || list_index >= sockets.Count || sockets[list_index] == null) {
			return;
		}
		Undo.RecordObject(target, "Add Socket");
		int index = list.index;
		if (index < 0 || index >= sockets[list_index].Count) {
			sockets[list_index].Add(-1);
			list.index = sockets[list_index].Count - 1;
		}
		else {
			sockets[list_index].Insert(index, -1);
		}
		update = true;
		repaint = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void DeleteSocketElement(ReorderableList list, int list_index)
	{
		if (target == null || sockets == null) {
			return;
		}
		if (list_index < 0 || list_index >= sockets.Count || sockets[list_index] == null) {
			return;
		}
		int index = list.index;
		if (index < 0 || index >= sockets[list_index].Count) {
			return;
		}
		Undo.RecordObject(target, "Delete Socket");
		sockets[list_index].RemoveAt(index);
		if (sockets[list_index].Count > 0) {
			if (index > 0) {
				list.index = list.index - 1;
			}
		}
		else {
			list.index = -1;
		}
		update = true;
		repaint = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void ReorderSocketElement(ReorderableList list, int list_index, int old_index, int new_index)
	{
		if (target == null || sockets == null) {
			return;
		}
		if (list_index < 0 || list_index >= sockets.Count || sockets[list_index] == null) {
			return;
		}
		int cnt = sockets[list_index].Count;
		if (old_index < 0 || new_index < 0 || old_index >= cnt || new_index >= cnt || old_index == new_index) {
			return;
		}
		Undo.RecordObject(target, "Reorder Sockets");
		int old_socket = sockets[list_index][old_index];
		int new_socket = sockets[list_index][new_index];
		sockets[list_index][old_index] = new_socket;
		sockets[list_index][new_index] = old_socket;
		list.index = new_index;
		update = true;
		repaint = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}

	private void SelectSocketElement(int list_index, ReorderableList list)
	{
		_primary_index = list_index;
		_secondary_index = list.index;
		for (int k = 0;k < _socketlists.Count;k++) {
			if (k == list_index) {
				continue;
			}
			_socketlists[k].index = -1;
		}
	}
	#endregion
}
