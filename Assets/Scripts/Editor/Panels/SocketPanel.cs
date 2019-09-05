using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class SocketPanel: PanelGUI
{
	public VoxelComponent target;
	public List<int> selectlist;
	public HashSet<int> selectset;

	private MeshPreview _preview;
	private int _selectsocket;
	private int _socketcnt;
	private SocketType _type;
	private Vector2 _scroll;
	private Rect _rect_title;
	private Rect _rect_scroll;
	private Rect _rect_content;
	private List<Rect> _socketrects;
	private List<string> _socket_labels;
	private List<List<int>> _sockets;
	private List<ReorderableList> _socketlists;

	public SocketPanel(MeshPreview preview, string title, string[] labels, int socket_count, SocketType type)
	{
		_title = title;
		_preview = preview;
		selectlist = new List<int>();
		selectset = new HashSet<int>();
		_selectsocket = -1;
		_type = type;

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
		selectlist.Clear();
		selectset.Clear();
		_selectsocket = -1;

		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_scroll = Vector2.zero;
		for (int k = 0; k < _socketcnt; k++) {
			int socket_index = k;
			//initialize socket int lists and reorderable lists
			ReorderableList reorderlist = new ReorderableList(_sockets[k], typeof(int), true, false, false, false);
			reorderlist.showDefaultBackground = false;
			reorderlist.headerHeight = 0;
			reorderlist.footerHeight = 0;
			reorderlist.elementHeight = VxlGUI.MED_BAR;
			reorderlist.drawNoneElementCallback += DrawSocketNoneElement;
			reorderlist.drawElementCallback += (Rect rect, int index, bool active, bool focused) => {
				DrawSocketElement(rect, socket_index, index, active, focused);
			};
			reorderlist.drawElementBackgroundCallback += (Rect rect, int index, bool active, bool focused) => {
				DrawSocketElementBackground(rect, socket_index, index, active, focused);
			};
			reorderlist.onReorderCallbackWithDetails += (ReorderableList list, int old_index, int new_index) => {
				ReorderSocketElement(socket_index, old_index, new_index);
			};
			reorderlist.onSelectCallback += (ReorderableList list) => {
				SelectSocketElement(socket_index, list.index);
			};
			_socketlists[k] = reorderlist;
		}
	}

	public override void Disable()
	{
		selectlist.Clear();
		selectset.Clear();
		_selectsocket = -1;

		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
		target = null;
		for (int k = 0; k < _socketcnt; k++) {
			_socketlists[k] = null;
			_sockets[k].Clear();
		}
	}

	public override void DrawGUI(Rect rect)
	{
		//update socket lists
		UpdateSocketList();
		//calculate rects
		RecalculateRects(rect);
		//disable panel condition
		EditorGUI.BeginDisabledGroup(target == null || !target.IsValid());
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
				AddSocketElement(k);
			}
			//delete button
			Rect rect_delete = VxlGUI.GetAboveElement(rect_panel, 1, VxlGUI.SM_BAR, VxlGUI.SM_SPACE, 0);
			EditorGUI.BeginDisabledGroup(_selectsocket != k || selectlist.Count <= 0);
			VxlGUI.DrawRect(rect_delete, "DarkGradient");
			if (GUI.Button(VxlGUI.GetPaddedRect(rect_delete, 2), "", GUI.skin.GetStyle("Minus"))) {
				if (_selectsocket == k) {
					DeleteSelectedSocketVertex();
				}
			}
			EditorGUI.EndDisabledGroup();
			//add secondary button
			Rect rect_secondary = VxlGUI.GetAboveElement(rect_panel, 2, VxlGUI.SM_BAR, VxlGUI.SM_SPACE, 0);
			EditorGUI.BeginDisabledGroup(_preview.secondaryCount <= 0);
			VxlGUI.DrawRect(rect_secondary, "DarkGradient");
			if (GUI.Button(VxlGUI.GetPaddedRect(rect_secondary, 2), "", GUI.skin.GetStyle("Select2Plus"))) {
				AddSecondarySelection(k);
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

	private void AddSecondarySelection(int socket_index)
	{
		if (_preview == null || _preview.secondaryCount <= 0) {
			return;
		}
		List<int> socket = target.GetSocket(_type, socket_index);
		if (socket == null) {
			return;
		}
		Undo.RecordObject(target, "Add Secondary List To Socket.");
		_preview.AppendSecondarySelection(socket);
		//dirty target object
		_repaint_menu = true;
		_render_mesh = true;
		EditorUtility.SetDirty(target);
	}

	private void UpdateSocketList()
	{
		//clear lists
		for (int k = 0;k < _socketcnt;k++) {
			_sockets[k].Clear();
		}
		//check if valid sockets to update
		if (target == null || !target.IsValid()) {
			return;
		}
		//update socket lists
		for (int k = 0; k < _socketcnt; k++) {
			List<int> socket = GetSocket(k);
			if (socket == null) {
				continue;
			}
			_sockets[k].AddRange(socket);
		}
	}

	public int selectedSocket {
		get {
			return _selectsocket;
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
			float min_listheight = (3 * VxlGUI.MED_BAR) + (2 * VxlGUI.SM_SPACE);
			float list_height = Mathf.Max(min_listheight, _socketlists[k].GetHeight()) + VxlGUI.SM_SPACE + VxlGUI.MED_BAR + (2 * VxlGUI.MED_PAD);
			_socketrects[k] = VxlGUI.GetPaddedRect(new Rect(0, height, list_width, list_height), VxlGUI.MED_PAD);
			height += list_height;
		}
		float width = _rect_scroll.width;
		if (height > _rect_scroll.height && vscroll_style != null) {
			width = list_width;
		}
		_rect_content = new Rect(0, 0, width, height);
	}
	private void AddSocketElement(int socket_index)
	{
		List<int> socket = GetSocket(socket_index);
		if (socket == null) {
			return;
		}
		Undo.RecordObject(target, "Add New Socket Vertex.");
		socket.Add(-1);
		_repaint_menu = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private void DeleteSelectedSocketVertex()
	{
		List<int> socket = GetSocket(_selectsocket);
		if (socket == null || socket.Count <= 0 || selectlist.Count <= 0) {
			return;
		}
		Undo.RecordObject(target, "Delete Selected Socket Vertex");
		int index = 0;
		int deleted = 0;
		while (index < socket.Count) {
			if (selectset.Contains(index + deleted)) {
				//delete triangle
				socket.RemoveAt(index);
				deleted += 1;
			}
			else {
				index += 1;
			}
		}
		selectset.Clear();
		selectlist.Clear();
		_render_mesh = true;
		_repaint_menu = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}
	private List<int> GetSocket(int index)
	{
		if (target == null || !target.IsValid() || index < 0) {
			return null;
		}
		if (!System.Enum.IsDefined(typeof(SocketType), _type)) {
			return null; 
		}
		List<List<int>> sockets;
		switch (_type) {
			case SocketType.Face:
				sockets = target.facesockets;
				break;
			default:
				sockets = target.edgesockets;
				break;
		}
		if (index >= sockets.Count) {
			return null;
		}
		return sockets[index];
	}

	#region Socket ReorderableList
	private void DrawSocketNoneElement(Rect rect)
	{
		GUI.Label(rect, "No Socket Vertex.", GUI.skin.GetStyle("RightListLabel"));
	}
	private void DrawSocketElement(Rect rect, int socket_index, int index, bool isActive, bool isFocused)
	{
		List<int> socket = GetSocket(socket_index);
		if (socket == null || index < 0 || index >= socket.Count) {
			return;
		}
		float max_element_width = 100f;
		float field_width = Mathf.Min(max_element_width, 0.5f * rect.width);
		//draw left label
		GUI.Label(
			VxlGUI.GetLeftElement(rect, 0, rect.width - field_width),
			"Socket " + index
		);
		//draw int field
		EditorGUI.BeginChangeCheck();
		int vertex = EditorGUI.IntField(
			VxlGUI.GetRightElement(rect, 0, field_width),
			socket[index]
		);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(target, "Update Socket");
			socket[index] = vertex;
			_update_mesh = true;
			_render_mesh = true;
			_repaint_menu = true;
			//dirty target object
			EditorUtility.SetDirty(target);
		}
	}
	private void DrawSocketElementBackground(Rect rect, int socket_index, int index, bool active, bool focus)
	{
		List<int> socket = GetSocket(socket_index);
		if (socket == null) {
			return;
		}
		bool hover = (socket.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (socket_index == _selectsocket) && selectset.Contains(index);
		bool valid = true;
		if (index >= 0 && index < socket.Count) {
			valid = (socket[index] >= 0) && (socket[index] < target.vertices.Count);
		}
		if (valid) {
			VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
		}
		else {
			VxlGUI.DrawRect(rect, "SelectableRed", hover, active, on, focus);
		}
	}
	private void ReorderSocketElement(int socket_index, int old_index, int new_index)
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		if (old_index < 0 || new_index < 0 || old_index == new_index) {
			return;
		}
		List<int> socket = GetSocket(socket_index);
		if (socket == null || old_index >= socket.Count || new_index >= socket.Count) {
			return;
		}
		Undo.RecordObject(target, "Reorder Socket");
		int old_vertex = socket[old_index];
		socket.RemoveAt(old_index);
		socket.Insert(new_index, old_vertex);
		//maintain selection
		if (socket_index == _selectsocket) {
			ReorderMaintainSelection(old_index, new_index, selectlist, selectset);
		}
		//dirty target object
		_render_mesh = true;
		_repaint_menu = true;
		EditorUtility.SetDirty(target);
	}
	private void SelectSocketElement(int socket_index, int index)
	{
		if (socket_index != _selectsocket) {
			selectlist.Clear();
			selectlist.Clear();
			_selectsocket = socket_index;
		}
		List<int> sockets = GetSocket(_selectsocket);
		if (sockets == null || index < 0 || index >= sockets.Count) {
			selectset.Clear();
			selectlist.Clear();
			_selectsocket = -1;
			return;
		}
		SelectListElement(index, selectlist, selectset);
		_render_mesh = true;
		_repaint_menu = true;
	}
	#endregion
}