using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshPreview : PanelGUI
{
	private const float ZOOMSPEED = 0.01f;
	private const float PANSPEED = 0.01f;
	private const float ROTATESPEED = 0.1f;
	private const float POSITIONBOUNDS = 4f;
	private const float INIT_VERTICALROTATE = -30f;
	private const float INIT_HORIZONTALROTATE = 0;

	public Color origin_colour;
	public Color aboveaxi_colour;
	public Color belowaxi_colour;
	public Color d0axi_colour;
	public Color d1axi_colour;
	public Color d2axi_colour;
	public Color d3axi_colour;
	public Color d4axi_colour;
	public Color d5axi_colour;
	public Color frame_colour;

	public Color colour_superprimary = new Color(0.01228193f, 0.8679245f, 0.5222418f);
	public Color colour_supersecondary = new Color(0.5638335f, 0.252314f, 0.8490566f);
	public Color colour_selectprimary = new Color(1, 0.9937412f, 0.1098039f);
	public Color colour_selectsecondary = new Color(0.1098039f, 0.566f, 1);
	public Color colour_normal = new Color(1, 1, 1);

	public VoxelComponent target;
	public Mesh vertex_mesh;
	public Material vertex_mat;
	public Material mesh_mat;

	private VertexMode _vertexmode;
	private List<int> _primarylist;
	private HashSet<int> _primarytset;
	private List<int> _secondarylist;
	private HashSet<int> _secondarytset;

	private float _hrot;
	private float _vrot;
	
	private Texture _render;
	private MaterialPropertyBlock _previewblock;
	private PreviewRenderUtility _previewutility;

	private List<Vector3> _meshvertex;
	private List<Vector3> _meshnormals;
	private List<Vector2> _meshuvs;
	private List<int> _meshtriangles;
	private Mesh _mesh;
	protected PreviewAxiDisplay _display;

	public MeshPreview()
	{
		_previewblock = new MaterialPropertyBlock();
		_meshvertex = new List<Vector3>();
		_meshnormals = new List<Vector3>();
		_meshuvs = new List<Vector2>();
		_meshtriangles = new List<int>();
		_mesh = new Mesh();
		_mesh.MarkDynamic();
		_mesh.UploadMeshData(false);
		_vertexmode = VertexMode.None;
		_primarylist = new List<int>();
		_secondarylist = new List<int>();
		_primarytset = new HashSet<int>();
		_secondarytset = new HashSet<int>();
		_display = new PreviewAxiDisplay();
	}


	public override void Enable()
	{
		_vertexmode = VertexMode.None;
		InitializePreviewUtility();
		_previewblock.Clear();
		_meshvertex.Clear();
		_meshnormals.Clear();
		_meshuvs.Clear();
		_meshtriangles.Clear();
		_mesh.Clear();
		_mesh.UploadMeshData(false);
		_primarylist.Clear();
		_secondarylist.Clear();
		_primarytset.Clear();
		_secondarytset.Clear();
		_display.Enable();
		//mark dirty for update and repaint
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
	}

	public override void Disable()
	{
		_vertexmode = VertexMode.None;
		_previewutility.Cleanup();
		_previewutility = null;
		_previewblock.Clear();
		_meshvertex.Clear();
		_meshnormals.Clear();
		_meshuvs.Clear();
		_meshtriangles.Clear();
		_mesh.Clear();
		_mesh.UploadMeshData(false);
		_primarylist.Clear();
		_secondarylist.Clear();
		_primarytset.Clear();
		_secondarytset.Clear();
		_display.Disable();
		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
	}

	public override void DrawGUI(Rect rect) {}

	public override void DrawPreview(Rect rect)
	{
		//process preview controls
		ProcessPreviewInput(rect);
		//update display flags
		UpdateDisplayFlags();
		if (_update_mesh) {
			//update mesh
			UpdateMesh();
			//clear update mesh flag
			_update_mesh = false;
		}
		if (_render_mesh) {
			UpdateDisplay();
			//update preview render texture
			_previewutility.BeginPreview(rect, GUI.skin.GetStyle("DarkGrey"));
			//draw axi stuff
			_display.DrawDisplayObjects(_previewutility);
			//draw mesh
			if (_mesh.triangles.Length > 0 && mesh_mat != null) {
				_previewutility.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, mesh_mat, 0);
			}
			//draw vertices
			DrawVertex();
			//render preview scene
			_previewutility.Render();
			_render = _previewutility.EndPreview();
			//clear render mesh flag
			_render_mesh = false;
		}
		//return if nothing to draw
		if (_render == null) {
			return;
		}
		//draw preview render texture
		GUI.DrawTexture(rect, _render);
		rect = VxlGUI.GetPaddedRect(rect, VxlGUI.MED_PAD);
		_display.DrawGUI(new Rect(rect.x, rect.y, 0.5f * rect.width, 0.5f * rect.height));
	}

	private void UpdateDisplay()
	{
		float length = _previewutility.camera.transform.position.magnitude;
		_display.distance_factor = Mathf.Max(0.001f, Mathf.Clamp01(length / POSITIONBOUNDS));
	}

	private void UpdateDisplayFlags()
	{
		_update_mesh = _update_mesh || _display.updateMesh;
		_render_mesh = _render_mesh || _display.renderMesh;
		_repaint_menu = _repaint_menu || _display.repaintMenu;
		_display.updateMesh = false;
		_display.renderMesh = false;
		_display.repaintMenu = false;
	}

	#region Properties
	public bool setVoxelFrame {
		set {
			_display.enable_voxelframe = value;
		}
	}
	public bool setVoxelFlip {
		set {
			_display.enable_voxelflip = value;
		}
	}

	public Vector3 setFramePos {
		set {
			_display.voxelpos = value;
		}
	}

	public Vector3 setFrameAltPos {
		set {
			_display.altvoxelpos = value;
		}
	}

	public int secondaryCount {
		get {
			return _secondarylist.Count;
		}
	}
	public int secondarySuper {
		get {
			if (_secondarylist.Count <= 0) {
				return -1;
			}
			return _secondarylist[_secondarylist.Count - 1];
		}
	}
	public VertexMode vertexMode {
		set {
			if (!System.Enum.IsDefined(typeof(VertexMode), value)) {
				return;
			}
			if (_vertexmode != value) {
				_vertexmode = value;
				_primarylist.Clear();
				_primarytset.Clear();
				_secondarylist.Clear();
				_secondarytset.Clear();
			}
		}
	}
	public Material axiMaterial {
		set {
			_display.material = value;
		}
	}
	public Mesh originMesh {
		set {
			_display.origin_mesh = value;
		}
	}
	public Mesh axiMesh {
		set {
			_display.axis_mesh = value;
		}
	}
	public Mesh voxelMesh {
		set {
			_display.voxel_mesh = value;
		}
	}
	#endregion

	#region Update Selection
	public void UpdatePrimarySelection(List<int> selectlist)
	{
		if (_vertexmode != VertexMode.PrimarySelect && 
			_vertexmode != VertexMode.PrimarySecondarySelet && 
			_vertexmode != VertexMode.PrimarySelectTriangle) {
			return;
		}
		_primarytset.Clear();
		_primarylist.Clear();
		if (selectlist == null) {
			return;
		}
		for (int k = 0;k < selectlist.Count;k++) {
			if (_primarytset.Add(selectlist[k])) {
				_primarylist.Add(selectlist[k]);
			}
		}
	}
	public void UpdatePrimarySocketSelection(int socket_index, SocketType type, List<int> selectlist)
	{
		if (_vertexmode != VertexMode.PrimarySelect &&
			_vertexmode != VertexMode.PrimarySecondarySelet) {
			return;
		}
		_primarytset.Clear();
		_primarylist.Clear();
		if (target == null || !target.IsValid()) {
			return;
		}
		List<int> socket = target.GetSocket(type, socket_index);
		if (selectlist == null || socket == null) {
			return;
		}
		for (int k = 0; k < selectlist.Count; k++) {
			int select_index = selectlist[k];
			if (select_index < 0 || select_index >= socket.Count) {
				continue;
			}
			int vertex_index = socket[select_index];
			if (_primarytset.Add(vertex_index)) {
				_primarylist.Add(vertex_index);
			}
		}
	}
	public void AppendSecondarySelection(List<int> vertexlist)
	{
		if (vertexlist == null || target == null || !target.IsValid()) {
			return;
		}
		if (_secondarylist.Count <= 0) {
			return;
		}
		int vertex_cnt = target.vertices.Count;
		for (int k = 0; k < _secondarylist.Count; k++) {
			int vertex_index = _secondarylist[k];
			if (vertex_index < 0 || vertex_index >= vertex_cnt) {
				continue;
			}
			vertexlist.Add(vertex_index);
		}
	}

	public Triangle GetSelectedTriangle()
	{
		if (_secondarylist.Count != 3) {
			return Triangle.empty;
		}
		int index = _secondarylist.Count - 3;
		return new Triangle((ushort)_secondarylist[index], (ushort)_secondarylist[index + 1], (ushort)_secondarylist[index + 2]);

	}
	#endregion

	#region Mesh
	private void DrawVertex()
	{
		if (target == null || !target.IsValid()) {
			return;
		}
		if (vertex_mesh == null || vertex_mat == null) {
			return;
		}
		if (_vertexmode == VertexMode.None) {
			return;
		}
		List<Vector3> vectors = target.vertices;
		if (_vertexmode == VertexMode.PrimarySelectTriangle) {
			int super_vertex0 = -1; int super_vertex1 = -1; int super_vertex2 = -1;
			HashSet<int> trivertex = new HashSet<int>();
			List<Triangle> triangles = target.triangles;
			for (int k = 0;k < _primarylist.Count;k++) {
				int tri_index = _primarylist[k];
				if (tri_index < 0 || tri_index >= triangles.Count) {
					continue;
				}
				Triangle tri = triangles[tri_index];
				if (!tri.IsValid(vectors.Count, null, null)) {
					continue;
				}
				trivertex.Add(tri.vertex0);
				trivertex.Add(tri.vertex1);
				trivertex.Add(tri.vertex2);
				if (k == _primarylist.Count - 1) {
					super_vertex0 = tri.vertex0;
					super_vertex1 = tri.vertex1;
					super_vertex2 = tri.vertex2;
				}
			}
			for (int k = 0; k < vectors.Count; k++) {
				bool primary_superselect = false;
				bool primary_select = trivertex.Contains(k);
				if (primary_select) {
					primary_superselect = (super_vertex0 == k) || (super_vertex1 == k) || (super_vertex2 == k);
				}
				bool secondary_superselect = false;
				bool secondary_select = _secondarytset.Contains(k);
				if (secondary_select) {
					secondary_superselect = _secondarylist[_secondarylist.Count - 1] == k;
				}
				SetMaterialBlock(primary_select, primary_superselect, secondary_select, secondary_superselect);
				_previewutility.DrawMesh(vertex_mesh, vectors[k], Quaternion.identity, vertex_mat, 0, _previewblock);
			}
		}
		else {
			for (int k = 0; k < vectors.Count; k++) {
				bool primary_superselect = false;
				bool primary_select = _primarytset.Contains(k);
				if (primary_select) {
					primary_superselect = _primarylist[_primarylist.Count - 1] == k;
				}
				bool secondary_superselect = false;
				bool secondary_select = _secondarytset.Contains(k);
				if (secondary_select) {
					secondary_superselect = _secondarylist[_secondarylist.Count - 1] == k;
				}

				SetMaterialBlock(primary_select, primary_superselect, secondary_select, secondary_superselect);
				_previewutility.DrawMesh(vertex_mesh, vectors[k], Quaternion.identity, vertex_mat, 0, _previewblock);
			}
		}
	}

	private void SetMaterialBlock(bool primary_select, bool primary_superselect, bool secondary_select, bool secondary_superselect)
	{
		if (primary_superselect) {
			_previewblock.SetColor("_Primary", colour_superprimary);
			if (secondary_superselect) {
				_previewblock.SetColor("_Secondary", colour_supersecondary);
			}
			else if (secondary_select) {
				_previewblock.SetColor("_Secondary", colour_selectsecondary);
			}
			else {
				_previewblock.SetColor("_Secondary", colour_superprimary);
			}
		}
		else if (primary_select) {
			_previewblock.SetColor("_Primary", colour_selectprimary);
			if (secondary_superselect) {
				_previewblock.SetColor("_Secondary", colour_supersecondary);
			}
			else if (secondary_select) {
				_previewblock.SetColor("_Secondary", colour_selectsecondary);
			}
			else {
				_previewblock.SetColor("_Secondary", colour_selectprimary);
			}
		}
		else {
			if (secondary_superselect) {
				_previewblock.SetColor("_Primary", colour_supersecondary);
				_previewblock.SetColor("_Secondary", colour_supersecondary);
			}
			else if (secondary_select) {
				_previewblock.SetColor("_Primary", colour_selectsecondary);
				_previewblock.SetColor("_Secondary", colour_selectsecondary);
			}
			else {
				_previewblock.SetColor("_Primary", colour_normal);
				_previewblock.SetColor("_Secondary", colour_normal);
			}
		}
		
	}

	private void UpdateMesh()
	{
		_meshvertex.Clear();
		_meshnormals.Clear();
		_meshuvs.Clear();
		_meshtriangles.Clear();
		_mesh.Clear();
		if (target == null || !target.IsValid()) {
			UploadMeshChanges();
			return;
		}
		List<Vector3> vertices = target.vertices;
		List<Triangle> triangles = target.triangles;
		for (int k = 0;k < triangles.Count;k++) {
			Triangle tri = triangles[k];
			if (!tri.IsValid(vertices.Count, null, null)) {
				continue;
			}
			Vector2 uv = GetTriangleUV(k);
			Vector3 vertex0 = vertices[tri.vertex0];
			Vector3 vertex1 = vertices[tri.vertex1];
			Vector3 vertex2 = vertices[tri.vertex2];
			AddTriangle(vertex0, vertex1, vertex2, uv);
		}
		UploadMeshChanges();
	}

	private Vector2 GetTriangleUV(int tri_index)
	{
		if (_vertexmode != VertexMode.PrimarySelectTriangle) {
			return Vector2.zero;
		}
		if (_primarytset.Contains(tri_index)) {
			if (_primarylist[_primarylist.Count - 1] == tri_index) {
				return new Vector2(0.25f, 0);
				
			}
			else {
				return new Vector2(0.15f, 0);
			}
		}
		return Vector2.zero;
	}

	private void AddTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector2 uv)
	{
		int vertex_index = _meshvertex.Count;
		Vector3 normal = Vector3.Cross((vertex1 - vertex0).normalized, (vertex2 - vertex0).normalized);
		//vertex 0
		_meshvertex.Add(vertex0);
		_meshnormals.Add(normal);
		_meshuvs.Add(uv);
		//vertex 1
		_meshvertex.Add(vertex1);
		_meshnormals.Add(normal);
		_meshuvs.Add(uv);
		//vertex 2
		_meshvertex.Add(vertex2);
		_meshnormals.Add(normal);
		_meshuvs.Add(uv);
		//triangles
		_meshtriangles.Add(vertex_index);
		_meshtriangles.Add(vertex_index + 1);
		_meshtriangles.Add(vertex_index + 2);
	}

	private void UploadMeshChanges()
	{
		_mesh.SetVertices(_meshvertex);
		_mesh.SetUVs(0, _meshuvs);
		_mesh.SetTriangles(_meshtriangles, 0);
		_mesh.RecalculateBounds();
		_mesh.RecalculateTangents();
		_mesh.UploadMeshData(false);
	}
	#endregion

	#region Preview
	public void PanCamera(Vector2 delta)
	{
		Vector3 forward = _previewutility.camera.transform.forward.normalized;
		Vector3 xaxis = Vector3.Cross(Vector3.up, forward);
		Vector3 yaxis = Vector3.Cross(forward, xaxis);
		MovePosition((-delta.x * PANSPEED * xaxis) + (delta.y * PANSPEED * yaxis));
		_render_mesh = true;
	}
	public void ZoomCamera(float delta)
	{
		MovePosition(delta * -ZOOMSPEED * _previewutility.camera.transform.forward);
		_render_mesh = true;
	}
	public void RotateCamera(Vector2 delta)
	{
		_hrot = (_hrot + (delta.x * ROTATESPEED)) % 360;
		if (_hrot < 0) {
			_hrot += 360;
		}
		_vrot = Mathf.Clamp(_vrot + (-delta.y * ROTATESPEED), -89.9f, 89.9f);
		_previewutility.camera.transform.rotation = Quaternion.Euler(-_vrot, _hrot, 0);
		_render_mesh = true;
	}

	public void SelectVertex(Vector2 viewpos, bool shift)
	{
		if (_vertexmode != VertexMode.PrimarySecondarySelet && _vertexmode != VertexMode.PrimarySelectTriangle) {
			return;
		}
		if (target == null || !target.IsValid()) {
			_secondarytset.Clear();
			_secondarylist.Clear();
			_repaint_menu = true;
			_render_mesh = true;
			return;
		}
		if (!shift) {
			_secondarytset.Clear();
			_secondarylist.Clear();
		}
		int vertex = VertexCast(viewpos);
		if (vertex < 0 || vertex >= target.vertices.Count) {
			_render_mesh = true;
			_repaint_menu = true;
			return;
		}
		if (_secondarytset.Add(vertex)) {
			_secondarylist.Add(vertex);
			if (_vertexmode == VertexMode.PrimarySelectTriangle) {
				while (_secondarylist.Count > 3) {
					_secondarytset.Remove(_secondarylist[0]);
					_secondarylist.RemoveAt(0);
				}
			}
		}
		else {
			_secondarytset.Remove(vertex);
			_secondarylist.Remove(vertex);
		}
		_render_mesh = true;
		_repaint_menu = true;
	}

	private void ProcessPreviewInput(Rect rect)
	{
		if (!rect.Contains(Event.current.mousePosition)) {
			return;
		}
		switch (Event.current.type) {
			case EventType.MouseDrag:
				if (Event.current.button == 1) {
					RotateCamera(Event.current.delta);
					_repaint_menu = true;
					_render_mesh = true;
				}
				if (Event.current.button == 2) {
					PanCamera(Event.current.delta);
					_repaint_menu = true;
					_render_mesh = true;
				}
				break;
			case EventType.ScrollWheel:
				ZoomCamera(Event.current.delta.y);
				_repaint_menu = true;
				_render_mesh = true;
				break;
			case EventType.MouseDown:
				if (Event.current.button == 0) {
					float x = Mathf.Clamp01((Event.current.mousePosition.x - rect.xMin) / rect.width);
					float y = 1 - Mathf.Clamp01((Event.current.mousePosition.y - rect.yMin) / rect.height);
					Vector2 viewport_point = new Vector2(x, y);
					SelectVertex(viewport_point, Event.current.shift);
				}
				break;
			case EventType.KeyDown:
				switch (Event.current.keyCode) {
					case KeyCode.KeypadPeriod:
						//align view to origin
						break;
					case KeyCode.Keypad0:
						//lookat origin and set rotation to d0
						break;
					case KeyCode.Keypad1:
						//lookat origin and set rotation to d1
						break;
					case KeyCode.Keypad2:
						//lookat origin and set rotation to d2
						break;
					case KeyCode.Keypad3:
						//lookat origin and set rotation to d3
						break;
					case KeyCode.Keypad4:
						//lookat origin and set rotation to d4
						break;
					case KeyCode.Keypad5:
						//lookat origin and set rotation to d5
						break;
					case KeyCode.Keypad9:
						//flatten y position and rotation
						break;
				}
				break;
		}
	}
	private void InitializePreviewUtility()
	{
		_previewutility = new PreviewRenderUtility();
		_previewutility.camera.backgroundColor = new Color(50 / 255f, 50 / 255f, 50 / 255f);
		_previewutility.camera.nearClipPlane = 0.01f;
		_previewutility.camera.farClipPlane = 1000f;
		_previewutility.cameraFieldOfView = 60;
		_previewutility.ambientColor = new Color(0.4f, 0.4f, 0.4f);

		_hrot = INIT_HORIZONTALROTATE;
		_vrot = INIT_VERTICALROTATE;
		Quaternion rotate = Quaternion.Euler(-_vrot, _hrot, 0);
		Vector3 campos = 3 * (rotate * Vector3.back);
		_previewutility.camera.transform.position = Vector3.zero;
		_previewutility.camera.transform.rotation = rotate;
		MovePosition(campos);
	}
	private void MovePosition(Vector3 move)
	{
		Vector3 pos = _previewutility.camera.transform.position;
		float t = Mathf.Infinity;
		if ((move.y > 0 && pos.y < POSITIONBOUNDS) || (move.y < 0 && pos.y > -POSITIONBOUNDS)) {
			float ydif = POSITIONBOUNDS;
			if (move.y > 0) {
				ydif -= (pos.y + move.y);
			}
			else {
				ydif += (pos.y + move.y);
			}
			if (ydif >= 0) {
				t = Mathf.Min(1, t);
			}
			else {
				t = Mathf.Min(t, Mathf.Clamp01(Mathf.Abs(ydif / move.y)));
			}
		}
		if ((move.x > 0 && pos.x < POSITIONBOUNDS) || (move.x < 0 && pos.x > -POSITIONBOUNDS)) {
			float xdif = POSITIONBOUNDS;
			if (move.x > 0) {
				xdif -= (pos.x + move.x);
			}
			else {
				xdif += (pos.x + move.x);
			}
			if (xdif >= 0) {
				t = Mathf.Min(1, t);
			}
			else {
				t = Mathf.Min(t, Mathf.Clamp01(Mathf.Abs(xdif / move.x)));
			}
		}
		if ((move.z > 0 && pos.z < POSITIONBOUNDS) || (move.z < 0 && pos.z > -POSITIONBOUNDS)) {
			float zdif = POSITIONBOUNDS;
			if (move.z > 0) {
				zdif -= (pos.z + move.z);
			}
			else {
				zdif += (pos.z + move.z);
			}
			if (zdif >= 0) {
				t = Mathf.Min(1, t);
			}
			else {
				t = Mathf.Min(t, Mathf.Clamp01(Mathf.Abs(zdif / move.z)));
			}
		}
		if (t == Mathf.Infinity || t < 0) {
			t = 0;
		}
		_previewutility.camera.transform.position += t * move;
	}
	private int VertexCast(Vector2 viewpos)
	{
		Ray ray = _previewutility.camera.ViewportPointToRay(viewpos);
		if (ray.direction == Vector3.zero || target == null || target.vertices == null) {
			return -1;
		}
		float nearest_t = Mathf.Infinity;
		int nearest_sphere = -1;
		float near_clip = _previewutility.camera.nearClipPlane;
		float far_clip = _previewutility.camera.farClipPlane;
		List<Vector3> vertices = target.vertices;
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 point_vector = ray.origin - vertices[k];
			float proj_pv = Vector3.Dot(ray.direction, point_vector);
			float sqr_magn = Vector3.SqrMagnitude(point_vector - (proj_pv * ray.direction));
			//check if ray intersects point
			if (sqr_magn > VxlGUI.POINT_RADIUSSQR) {
				continue;
			}
			//get t of impact and check if in range
			float t = Mathf.Abs(proj_pv) - Mathf.Sqrt(Mathf.Max(0, VxlGUI.POINT_RADIUSSQR - sqr_magn));
			if (t < near_clip || t > far_clip) {
				continue;
			}
			if (t >= nearest_t) {
				continue;
			}
			nearest_t = t;
			nearest_sphere = k;
		}
		return nearest_sphere;
	}
	#endregion
}


public enum VertexMode
{
	None,
	Draw,
	PrimarySelect,
	PrimarySelectTriangle,
	PrimarySecondarySelet
}
