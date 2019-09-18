using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public class SocketPanel : PanelGUI
{
	public VoxelComponent target;

	public List<int> selectlist { get; }
	public HashSet<int> selectset { get; }
	


	private Rect _rect_axititle;
	private Rect _rect_sockethead;
	private Rect _rect_sockettitle;
	private Rect _rect_invx;
	private Rect _rect_invy;
	private Rect _rect_socketscroll;
	private Rect _rect_socketcontent;
	private Rect _rect_axiscroll;
	private Rect _rect_axicontent;
	private Rect _rect_socketbuttonpanel;
	private Rect _rect_addsocket;
	private Rect _rect_delsocket;
	private Rect _rect_secondaddsocket;

	private bool _invx;
	private bool _invy;
	private bool _is_edge;
	private Vector2 _axiscroll;
	private Vector2 _socketscroll;

	private string[] _axilabels;
	private ReorderableList _axilist;

	private List<int> _sockets;
	private ReorderableList _socketlist;

	private MeshPreview _preview;
	private PreviewVertexDisplay _selectdisplay;
	private PreviewMeshDisplay _meshdisplay;


	public SocketPanel(MeshPreview preview, string title, bool is_edge, string[] axilabels)
	{
		selectlist = new List<int>();
		selectset = new HashSet<int>();

		_title = title;
		_is_edge = is_edge;
		_preview = preview;
		_axilabels = axilabels;
		_sockets = new List<int>();
		_selectdisplay = new PreviewVertexDisplay();
		_meshdisplay = new PreviewMeshDisplay();
	}

	public override void Enable()
	{
		selectlist.Clear();
		selectset.Clear();

		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
		_axiscroll = Vector2.zero;
		_socketscroll = Vector2.zero;
		_selectdisplay.Enable();
		_meshdisplay.Enable();

		_axilist = new ReorderableList(_axilabels, typeof(string), false, false, false, false);
		_axilist.showDefaultBackground = false;
		_axilist.headerHeight = 0;
		_axilist.footerHeight = 0;
		_axilist.elementHeightCallback += SocketAxiHeight;
		_axilist.drawNoneElementCallback += DrawSocketAxiNone;
		_axilist.drawElementCallback += DrawSocketAxi;
		_axilist.drawElementBackgroundCallback += DrawSocketAxiBackground;

		_socketlist = new ReorderableList(_sockets, typeof(int), true, false, false, false);
		_socketlist.showDefaultBackground = false;
		_socketlist.headerHeight = 0;
		_socketlist.footerHeight = 0;
		_socketlist.elementHeight = VxlGUI.MED_BAR;
		_socketlist.drawNoneElementCallback += DrawSocketNone;
		_socketlist.drawElementCallback += DrawSocket;
		_socketlist.drawElementBackgroundCallback += DrawSocketBackground;
		_socketlist.onReorderCallbackWithDetails += ReorderSocket;
		_socketlist.onSelectCallback += SelectSocket;
	}
	public override void Disable()
	{
		selectlist.Clear();
		selectset.Clear();

		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
		target = null;
		_selectdisplay.Disable();
		_meshdisplay.Disable();
		_preview.invx = false;
		_preview.invy = false;

		_axilist = null;
		_socketlist = null;
		_sockets.Clear();
	}

	public override void DrawGUI(Rect rect)
	{
		UpdateAxiList();
		UpdateSocketList();
		//update layout rects
		UpdateRects(rect);
		//disable panel condition
		EditorGUI.BeginDisabledGroup(target == null || !target.IsValid());
		VxlGUI.DrawRect(_rect_axititle, "DarkGrey");
		VxlGUI.DrawRect(_rect_axiscroll, "DarkWhite");
		//axi socket title
		GUI.Label(_rect_axititle, _title + " Axi Sockets", "LeftLightHeader");
		//draw list of axi sockets
		_axiscroll = GUI.BeginScrollView(_rect_axiscroll, _axiscroll, _rect_axicontent);
		_axilist.DoList(_rect_axicontent);
		GUI.EndScrollView();

		EditorGUI.BeginDisabledGroup(_axilist.index < 0 || _axilist.index >= _axilist.count);

		//sockets
		VxlGUI.DrawRect(_rect_sockethead, "DarkGrey");
		VxlGUI.DrawRect(_rect_socketbuttonpanel, "DarkGrey");
		VxlGUI.DrawRect(_rect_socketscroll, "DarkWhite");
		//socket title
		string socket_title = "Socket: ";
		if (_socketlist.index < 0 || _socketlist.index >= _axilabels.Length) {
			socket_title += "None";
		}
		else {
			socket_title += _axilabels[_socketlist.index];
		}
		GUI.Label(_rect_sockettitle, socket_title, "LeftLightHeader");
		//socket list type selection
		EditorGUI.BeginChangeCheck();
		_invx = EditorGUI.Foldout(_rect_invx, _invx, "Invert X", true, "LightBoldFoldout");
		_invy = EditorGUI.Foldout(_rect_invy, _invy, "Invert Y", true, "LightBoldFoldout");
		if (EditorGUI.EndChangeCheck()) {
			selectlist.Clear();
			selectset.Clear();
			UpdateSocketList();
			_repaint_menu = true;
			_render_mesh = true;
		}
		//draw list scroll
		_socketscroll = GUI.BeginScrollView(_rect_socketscroll, _socketscroll, _rect_socketcontent);
		_socketlist.DoList(_rect_socketcontent);
		GUI.EndScrollView();

		//add button
		if (GUI.Button(_rect_addsocket, "Add", "LightButton")) {
			AddSocketElement();
		}
		//selection add button
		EditorGUI.BeginDisabledGroup(_preview.secondaryCount <= 0);
		if (GUI.Button(_rect_secondaddsocket, "Add S", "LightButton")) {
			AddSecondarySelection();
		}
		EditorGUI.EndDisabledGroup();

		//delete button
		EditorGUI.BeginDisabledGroup(selectlist.Count <= 0);
		if (GUI.Button(_rect_delsocket, "Delete", "LightButton")) {
			DeleteSelectedSocketVertex();
		}
		EditorGUI.EndDisabledGroup();
		EditorGUI.EndDisabledGroup();
		EditorGUI.EndDisabledGroup();

		_preview.invx = _meshdisplay.invertx;
		_preview.invy = _meshdisplay.inverty;
	}

	public override void DrawPreview(Rect rect)
	{
		//update preview displays
		_selectdisplay.selected = _preview.secondaryCount;
		_selectdisplay.superselected = _preview.secondarySuper;

		rect = VxlGUI.GetPaddedRect(rect, VxlGUI.MED_PAD);
		float width = Mathf.Min(200, rect.width / 3f);
		_selectdisplay.DrawGUI(VxlGUI.GetBelowLeftElement(rect, 0, width, 0, rect.height / 3f));
		_meshdisplay.DrawGUI(VxlGUI.GetAboveRightElement(rect, 0, width, 0, rect.height / 3f));
	}


	public bool inverseX {
		get {
			return _invx;
		}
	}
	public bool inverseY {
		get {
			return _invy;
		}
	}

	public int axiSocket {
		get {
			return _axilist.index;
		}
	}

	public override bool repaintMenu {
		get {
			return _repaint_menu || _meshdisplay.repaintMenu;
		}
		set {
			_repaint_menu = value;
			_meshdisplay.repaintMenu = value;
		}
	}
	public override bool updateMesh {
		get {
			return _update_mesh || _meshdisplay.updateMesh;
		}
		set {
			_update_mesh = value;
			_meshdisplay.updateMesh = value;
		}
	}
	public override bool renderMesh {
		get {
			return _render_mesh || _meshdisplay.renderMesh;
		}
		set {
			_render_mesh = value;
			_meshdisplay.renderMesh = value;
		}
	}

	private void UpdateAxiList()
	{
		if (target == null || !target.IsValid()) {
			selectlist.Clear();
			selectset.Clear();
			_axilist.index = -1;
			_sockets.Clear();
		}
	}

	private void UpdateSocketList()
	{
		_sockets.Clear();
		if (target == null || !target.IsValid()) {
			selectlist.Clear();
			selectset.Clear();
			return;
		}
		List<int> socket;
		if (_is_edge) {
			socket = target.GetEdgeSocketList(_invx, _invy, _axilist.index);
		}
		else {
			socket = target.GetFaceSocketList(_invx, _invy, _axilist.index);
		}
		if (socket == null) {
			selectlist.Clear();
			selectset.Clear();
			return;
		}
		for (int k = 0;k < socket.Count;k++) {
			_sockets.Add(socket[k]);
		}
	}

	private void UpdateRects(Rect rect)
	{
		Rect rect_col = VxlGUI.GetLeftColumn(rect, VxlGUI.SM_SPACE, 0.5f);
		//calculate axi rects
		_rect_axititle = VxlGUI.GetAboveElement(rect_col, 0, VxlGUI.MED_BAR);
		_rect_axiscroll = VxlGUI.GetSandwichedRectY(rect_col, VxlGUI.MED_BAR + VxlGUI.SM_SPACE, 0);
		_rect_axicontent = VxlGUI.GetScrollViewRect(_axilist, _rect_axiscroll.width, _rect_axiscroll.height);

		rect_col = VxlGUI.GetRightColumn(rect, VxlGUI.SM_SPACE, 0.5f);
		//calculate socket rects
		_rect_sockethead = VxlGUI.GetAboveElement(rect_col, 0, (2 * VxlGUI.MED_BAR) + VxlGUI.MED_SPACE);
		_rect_sockettitle = VxlGUI.GetAboveElement(_rect_sockethead, 0, VxlGUI.MED_BAR);
		_rect_invx = VxlGUI.GetBelowLeftElement(_rect_sockethead, 0, _rect_sockethead.width / 2f, 0, VxlGUI.MED_BAR);
		_rect_invy = VxlGUI.GetBelowRightElement(_rect_sockethead, 0, _rect_sockethead.width / 2f, 0, VxlGUI.MED_BAR);
		_rect_socketscroll = VxlGUI.GetSandwichedRectY(rect_col, _rect_sockethead.height + VxlGUI.SM_SPACE, VxlGUI.MED_BAR + VxlGUI.SM_SPACE);
		_rect_socketcontent = VxlGUI.GetScrollViewRect(_socketlist, _rect_socketscroll.width, _rect_socketscroll.height);
		_rect_socketbuttonpanel = VxlGUI.GetBelowElement(rect_col, 0, VxlGUI.MED_BAR);
		float button_width = Mathf.Min(60, _rect_socketbuttonpanel.width / 3f);
		_rect_addsocket = VxlGUI.GetRightElement(_rect_socketbuttonpanel, 0, button_width);
		_rect_delsocket = VxlGUI.GetLeftElement(_rect_socketbuttonpanel, 0, button_width);
		_rect_secondaddsocket = VxlGUI.GetRightElement(_rect_socketbuttonpanel, 1, button_width);
	}

	private void AddSocketElement()
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		List<int> socket;
		if (_is_edge) {
			socket = target.GetEdgeSocketList(_invx, _invy, _axilist.index);
		}
		else {
			socket = target.GetFaceSocketList(_invx, _invy, _axilist.index);
		}
		if (socket == null) {
			return;
		}
		Undo.RecordObject(target, "Add New Socket Vertex.");
		socket.Add(-1);
		_repaint_menu = true;
		_render_mesh = true;
		//dirty target object
		EditorUtility.SetDirty(target);
	}

	private void AddSecondarySelection()
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		if (_preview == null || _preview.secondaryCount <= 0) {
			return;
		}
		List<int> socket;
		if (_is_edge) {
			socket = target.GetEdgeSocketList(_invx, _invy, _axilist.index);
		}
		else {
			socket = target.GetFaceSocketList(_invx, _invy, _axilist.index);
		}
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

	private void DeleteSelectedSocketVertex()
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		List<int> socket;
		if (_is_edge) {
			socket = target.GetEdgeSocketList(_invx, _invy, _axilist.index);
		}
		else {
			socket = target.GetFaceSocketList(_invx, _invy, _axilist.index);
		}
		if (socket == null) {
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


	#region AxiSocket ReorderableList
	public float SocketAxiHeight(int index)
	{
		if (_axilabels.Length <= 0) {
			return VxlGUI.MED_BAR;
		}
		else {
			return VxlGUI.MED_BAR + (4 * VxlGUI.SM_BAR);
		}
	}
	public void DrawSocketAxiNone(Rect rect)
	{
		GUI.Label(rect, "No Axi Socket Found.", GUI.skin.GetStyle("LeftListLabel"));
	}
	public void DrawSocketAxi(Rect rect, int index, bool active, bool focus)
	{
		GUI.Label(VxlGUI.GetAboveElement(rect, 0, VxlGUI.MED_BAR), "Axi " + _axilabels[index], GUI.skin.GetStyle("LeftListLabel"));
		int normal_cnt = 0; int invertx_cnt = 0; int inverty_cnt = 0; int invertxy_cnt = 0;
		if (target != null && target.IsValid()) {
			if (_is_edge) {
				normal_cnt = target.GetEdgeSocketCountByIndex(false, false, index);
				invertx_cnt = target.GetEdgeSocketCountByIndex(true, false, index);
				inverty_cnt = target.GetEdgeSocketCountByIndex(false, true, index);
				invertxy_cnt = target.GetEdgeSocketCountByIndex(true, true, index);
			}
			else {
				normal_cnt = target.GetFaceSocketCountByIndex(false, false, index);
				invertx_cnt = target.GetFaceSocketCountByIndex(true, false, index);
				inverty_cnt = target.GetFaceSocketCountByIndex(false, true, index);
				invertxy_cnt = target.GetFaceSocketCountByIndex(true, true, index);
			}
			
		}
		GUI.Label(VxlGUI.GetAboveElement(rect, 0, VxlGUI.SM_BAR, VxlGUI.MED_BAR), normal_cnt.ToString() + " normal sockets", GUI.skin.GetStyle("RightListLabel"));
		GUI.Label(VxlGUI.GetAboveElement(rect, 1, VxlGUI.SM_BAR, VxlGUI.MED_BAR), invertx_cnt.ToString() + " invertx sockets", GUI.skin.GetStyle("RightListLabel"));
		GUI.Label(VxlGUI.GetAboveElement(rect, 2, VxlGUI.SM_BAR, VxlGUI.MED_BAR), inverty_cnt.ToString() + " inverty sockets", GUI.skin.GetStyle("RightListLabel"));
		GUI.Label(VxlGUI.GetAboveElement(rect, 3, VxlGUI.SM_BAR, VxlGUI.MED_BAR), invertxy_cnt.ToString() + " invertxy sockets", GUI.skin.GetStyle("RightListLabel"));
	}
	public void DrawSocketAxiBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_axilabels.Length > 0) && rect.Contains(Event.current.mousePosition);
		bool on = (_axilabels.Length > 0) && (index == _axilist.index);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}
	#endregion

	#region Socket ReorderableList
	public void DrawSocketNone(Rect rect)
	{
		GUI.Label(rect, "No Sockets Found", GUI.skin.GetStyle("RightListLabel"));
	}
	public void DrawSocket(Rect rect, int index, bool active, bool focus)
	{

		GUI.Label(VxlGUI.GetLeftColumn(rect, 0, 0.5f), "Socket " + index.ToString(), "LeftListLabel");
		EditorGUI.BeginChangeCheck();
		int socket = EditorGUI.IntField(VxlGUI.GetRightElement(rect, 0, VxlGUI.INTFIELD), _sockets[index]);
		if (EditorGUI.EndChangeCheck()) {
			if (target == null || !target.IsValid()) {
				return;
			}
			List<int> socketlist;
			if (_is_edge) {
				socketlist = target.GetEdgeSocketList(_invx, _invy, _axilist.index);
			}
			else {
				socketlist = target.GetFaceSocketList(_invx, _invy, _axilist.index);
			}
			if (socketlist == null || index < 0 || index >= socketlist.Count) {
				return;
			}
			Undo.RecordObject(target, "Updated Socket");
			socketlist[index] = socket;
			_repaint_menu = true;
			_render_mesh = true;
			_update_mesh = true;
			EditorUtility.SetDirty(target);
		}
	}
	public void DrawSocketBackground(Rect rect, int index, bool active, bool focus)
	{
		bool hover = (_sockets.Count > 0) && rect.Contains(Event.current.mousePosition);
		bool on = selectset.Contains(index);
		VxlGUI.DrawRect(rect, "SelectableGrey", hover, active, on, focus);
	}
	private void ReorderSocket(ReorderableList list, int old_index, int new_index)
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		if (old_index < 0 || new_index < 0 || old_index == new_index) {
			return;
		}
		List<int> socket;
		if (_is_edge) {
			socket = target.GetEdgeSocketList(_invx, _invy, _axilist.index);
		}
		else {
			socket = target.GetFaceSocketList(_invx, _invy, _axilist.index);
		}
		if (socket == null || old_index >= socket.Count || new_index >= socket.Count) {
			return;
		}
		Undo.RecordObject(target, "Reorder Socket List");
		int vertex = socket[old_index];
		socket.RemoveAt(old_index);
		socket.Insert(new_index, vertex);
		//maintain selection
		ReorderMaintainSelection(old_index, new_index, selectlist, selectset);
		//dirty target object
		_repaint_menu = true;
		EditorUtility.SetDirty(target);
	}
	private void SelectSocket(ReorderableList list)
	{
		if (target == null || !target.IsValid()) {
			selectset.Clear();
			selectlist.Clear();
			_render_mesh = true;
			_repaint_menu = true;
			return;
		}
		List<int> socket;
		if (_is_edge) {
			socket = target.GetEdgeSocketList(_invx, _invy, _axilist.index);
		}
		else {
			socket = target.GetFaceSocketList(_invx, _invy, _axilist.index);
		}
		int index = _socketlist.index;
		if (socket == null || index < 0 || index >= socket.Count) {
			selectset.Clear();
			selectlist.Clear();
			_render_mesh = true;
			_repaint_menu = true;
			return;
		}
		SelectListElement(index, selectlist, selectset);
		_render_mesh = true;
		_repaint_menu = true;
	}
	#endregion
}
