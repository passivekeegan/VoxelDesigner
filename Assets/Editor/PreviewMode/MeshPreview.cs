using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshPreview
{
	private const float ZOOMSPEED = 0.01f;
	private const float PANSPEED = 0.01f;
	private const float ROTATESPEED = 0.1f;
	private const float POSITIONBOUNDS = 4f;
	private const float INIT_VERTICALROTATE = -30f;
	private const float INIT_HORIZONTALROTATE = 0;

	public bool repaint;
	public bool vertexselect;
	public bool invertx, inverty;
	public Quaternion rotation;
	public Vector3 scale;
	public Material material;

	private bool _updaterender;
	private bool _updatemesh;
	private float _hrot, _vrot;
	private List<Vector3> _vertex;
	private List<Vector3> _normals;
	private List<Vector2> _uvs;
	private List<int> _triangles;
	private Mesh _mesh;
	private Texture _render;
	private MaterialPropertyBlock _previewblock;
	private PreviewRenderUtility _previewutility;
	private List<PreviewGUI> _prevguis;

	public MeshPreview()
	{
		rotation = Quaternion.identity;
		scale = Vector3.one;

	   _previewblock = new MaterialPropertyBlock();
		_vertex = new List<Vector3>();
		_normals = new List<Vector3>();
		_uvs = new List<Vector2>();
		_triangles = new List<int>();
		_mesh = new Mesh();
		_mesh.MarkDynamic();
		_mesh.UploadMeshData(false);
		_prevguis = new List<PreviewGUI>();
	}

	public void Enable()
	{
		_previewblock.Clear();
		_vertex.Clear();
		_normals.Clear();
		_uvs.Clear();
		_triangles.Clear();
		_mesh.Clear();
		_mesh.UploadMeshData(false);
		for (int k = 0; k < _prevguis.Count; k++) {
			_prevguis[k].Enable();
		}
		InitializePreviewUtility();
	}

	public void Disable()
	{
		for (int k = 0; k < _prevguis.Count; k++) {
			_prevguis[k].Disable();
		}
		_previewutility.Cleanup();
		_previewutility = null;
		_previewblock.Clear();
		_vertex.Clear();
		_normals.Clear();
		_uvs.Clear();
		_triangles.Clear();
		_mesh.Clear();
		_mesh.UploadMeshData(false);
	}

	public void DrawGUI(Rect rect)
	{
		if (_updatemesh) {
			UpdateMesh();
			_updatemesh = false;
			_updaterender = true;
		}
		if (_updaterender) {
			UpdateRender(rect);
			_updaterender = false;
			repaint = true;
		}
		//draw preview render texture
		if (_render != null) {
			GUI.DrawTexture(rect, _render);
		}
		//draw preview guis
		Rect display_rect = EVx.GetPaddedRect(rect, EVx.MED_PAD);
		for (int k = 0;k < _prevguis.Count;k++) {
			_prevguis[k].DrawGUI(display_rect);
			_updatemesh = _updatemesh || _prevguis[k].updatemesh;
			_prevguis[k].updatemesh = false;
			_updaterender = _updaterender || _prevguis[k].updaterender;
			_prevguis[k].updaterender = false;
		}
	}

	public void AddPreviewGUI(PreviewGUI prevgui)
	{
		if (prevgui == null) {
			return;
		}
		if (_prevguis.Contains(prevgui)) {
			return;
		}
		_prevguis.Add(prevgui);
	}

	public void RemovePreviewGUI(PreviewGUI prevgui)
	{
		if (prevgui == null) {
			return;
		}
		int index = _prevguis.IndexOf(prevgui);
		if (index < 0) {
			return;
		}
		_prevguis.RemoveAt(index);
	}

	

	public void DirtyRender()
	{
		_updaterender = true;
	}

	private void UpdateRender(Rect rect)
	{
		//update preview render texture
		_previewutility.BeginPreview(rect, "LightBlack");
		//draw preview objects
		for (int k = 0; k < _prevguis.Count; k++) {
			_prevguis[k].DrawDisplayObjects(_previewutility);
		}
		//draw mesh
		if (_mesh.triangles.Length > 0 && material != null) {
			_previewutility.DrawMesh(_mesh, Matrix4x4.TRS(Vector3.zero, rotation, scale), material, 0);
		}
		//render preview scene
		_previewutility.Render();
		_render = _previewutility.EndPreview();
	}

	public void ProcessInput(Rect rect)
	{
		if (!rect.Contains(Event.current.mousePosition)) {
			return;
		}
		
		//process input for preview guis
		for (int k = 0;k < _prevguis.Count;k++) {
			_prevguis[k].ProcessInput(_previewutility, rect);
		}
		//process preview navigation input
		switch (Event.current.type) {
			case EventType.MouseDrag:
				if (Event.current.button == 1) {
					RotateCamera(Event.current.delta);
				}
				if (Event.current.button == 2) {
					PanCamera(Event.current.delta);
				}
				break;
			case EventType.ScrollWheel:
				ZoomCamera(Event.current.delta.y);
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

	public void ClearMesh()
	{
		_vertex.Clear();
		_normals.Clear();
		_uvs.Clear();
		_triangles.Clear();
		_updatemesh = true;
	}

	private void UpdateMesh()
	{
		_mesh.Clear();
		_mesh.SetVertices(_vertex);
		_mesh.SetNormals(_normals);
		_mesh.SetUVs(0, _uvs);
		_mesh.SetTriangles(_triangles, 0);
		_mesh.RecalculateBounds();
		_mesh.RecalculateTangents();
		_mesh.UploadMeshData(false);
	}

	public void UpdateMeshComponents(List<Vector3> vertex, List<Vector3> normals, List<int> triangles)
	{
		int oldvertex_cnt = _vertex.Count;
		int oldtri_cnt = _triangles.Count;
		_vertex.Clear();
		_normals.Clear();
		_uvs.Clear();
		_triangles.Clear();
		if (vertex == null || normals == null || triangles == null) {
			_updatemesh = _updatemesh || (_triangles.Count != oldtri_cnt) || (_vertex.Count != oldvertex_cnt);
			return;
		}
		int vertex_cnt = vertex.Count;
		int tri_cnt = triangles.Count;
		if (normals.Count != vertex_cnt || tri_cnt % 3 != 0) {
			_updatemesh = _updatemesh || (_triangles.Count != oldtri_cnt) || (_vertex.Count != oldvertex_cnt);
			return;
		}
		for (int k = 0; k < vertex_cnt; k++) {
			_vertex.Add(vertex[k]);
			_normals.Add(normals[k]);
			_uvs.Add(Vector2.zero);
		}
		for (int k = 0; k < tri_cnt; k++) {
			_triangles.Add(triangles[k]);
		}
		_updatemesh = true;
	}

	public void UpdateMeshComponents(VoxelDesign design, HashSet<int> primaryset, HashSet<int> primarysuper)
	{
		_updatemesh = true;
		_vertex.Clear();
		_normals.Clear();
		_uvs.Clear();
		_triangles.Clear();
		if (design == null) {
			return;
		}
		List<Point> points = design.vertices;
		List<Triangle> triangles = design.triangles;
		for (int k = 0;k < triangles.Count;k++) {
			Triangle tri = triangles[k];
			if (!tri.IsValid(points.Count)) {
				continue;
			}
			int vertex_index = _vertex.Count;
			Point point0 = points[tri.vertex0];
			Point point1 = points[tri.vertex1];
			Point point2 = points[tri.vertex2];
			Vector3 normal = Triangle.Normal(point0.vertex, point1.vertex, point2.vertex);
			_vertex.Add(point0.vertex);
			_normals.Add(normal);
			_triangles.Add(vertex_index);
			_vertex.Add(point1.vertex);
			_normals.Add(normal);
			_triangles.Add(vertex_index + 1);
			_vertex.Add(point2.vertex);
			_normals.Add(normal);
			_triangles.Add(vertex_index + 2);
			if (primarysuper != null &&
				primarysuper.Contains(tri.vertex0) && 
				primarysuper.Contains(tri.vertex1) && 
				primarysuper.Contains(tri.vertex2)
			) {
				_uvs.Add(new Vector2(0.25f, 0));
				_uvs.Add(new Vector2(0.25f, 0));
				_uvs.Add(new Vector2(0.25f, 0));
			}
			else if (
				primaryset != null && 
				primaryset.Contains(tri.vertex0) && 
				primaryset.Contains(tri.vertex1) && 
				primaryset.Contains(tri.vertex2)
			) {
				_uvs.Add(new Vector2(0.15f, 0));
				_uvs.Add(new Vector2(0.15f, 0));
				_uvs.Add(new Vector2(0.15f, 0));
			}
			else {
				_uvs.Add(Vector2.zero);
				_uvs.Add(Vector2.zero);
				_uvs.Add(Vector2.zero);
			}
		}
	}

	#region Preview
	public void PanCamera(Vector2 delta)
	{
		Vector3 forward = _previewutility.camera.transform.forward.normalized;
		Vector3 xaxis = Vector3.Cross(Vector3.up, forward);
		Vector3 yaxis = Vector3.Cross(forward, xaxis);
		MovePosition((-delta.x * PANSPEED * xaxis) + (delta.y * PANSPEED * yaxis));
		_updaterender = true;
	}
	public void ZoomCamera(float delta)
	{
		MovePosition(delta * -ZOOMSPEED * _previewutility.camera.transform.forward);
		_updaterender = true;
	}
	public void RotateCamera(Vector2 delta)
	{
		_hrot = (_hrot + (delta.x * ROTATESPEED)) % 360;
		if (_hrot < 0) {
			_hrot += 360;
		}
		_vrot = Mathf.Clamp(_vrot + (-delta.y * ROTATESPEED), -89.9f, 89.9f);
		_previewutility.camera.transform.rotation = Quaternion.Euler(-_vrot, _hrot, 0);
		_updaterender = true;
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
	#endregion
}
