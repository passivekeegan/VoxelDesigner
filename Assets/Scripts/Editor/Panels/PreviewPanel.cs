using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PreviewPanel : PanelGUI
{
	private const float MINZOOM = 0.1f;
	private const float MAXZOOM = 2f;
	private const float MAXHEIGHT = 2f;
	private const float ZOOMSPEED = 0.01f;
	private const float HORIZONTALSPEED = 1f;
	private const float VERTICALSPEED = 0.01f;

	public VoxelComponent target;

	private int _primary_index;
	private int _secondary_index;

	private float _hrot;
	private Vector3 _camfocus;
	
	private PreviewDrawMode _drawmode;
	private Texture2D _colourmap;
	private Material _previewmat;

	private PreviewRenderUtility _previewutility;
	private Texture _texture;

	private HashSet<int> _primary_vertex;
	private HashSet<int> _secondary_vertex;

	private PreviewObject _originobj;
	private PreviewObject _triangleobj;

	public PreviewPanel(Texture2D colourmap)
	{
		_drawmode = PreviewDrawMode.Vertex;
		_colourmap = colourmap;
		_primary_vertex = new HashSet<int>();
		_secondary_vertex = new HashSet<int>();

		_primary_index = -1;
		_secondary_index = -1;
		_previewmat = new Material(Shader.Find("Diffuse"));
		_previewmat.mainTexture = _colourmap;
	}

	public override void Enable()
	{
		InitializePreviewUtility();
		//initialize origin object
		_originobj = new PreviewObject("Origin");
		_originobj.UpdateMaterial(_previewmat);
		AddVertexMesh(Vector3.zero, VxlGUI.ORIGIN_UV, ref _originobj);
		_originobj.UploadMeshChanges();
		_previewutility.AddSingleGO(_originobj.obj);
		_originobj.obj.transform.position = Vector3.zero;
		_originobj.obj.transform.rotation = Quaternion.identity;
		_originobj.obj.transform.localScale = Vector3.one;
		//initialize triangle object
		_triangleobj = new PreviewObject("Triangles");
		_triangleobj.UpdateMaterial(_previewmat);
		_triangleobj.UploadMeshChanges();
		_previewutility.AddSingleGO(_triangleobj.obj);
		_triangleobj.obj.transform.position = Vector3.zero;
		_triangleobj.obj.transform.rotation = Quaternion.identity;
		_triangleobj.obj.transform.localScale = Vector3.one;
		//mark dirty for update and repaint
		_update_mesh = true;
		_render_mesh = true;
		_repaint_menu = true;
	}

	public override void Disable()
	{
		_primary_vertex.Clear();
		_secondary_vertex.Clear();
		_previewutility.Cleanup();
		_previewutility = null;
		_originobj.Dispose();
		_triangleobj.Dispose();
		_update_mesh = false;
		_render_mesh = false;
		_repaint_menu = false;
	}

	public override void DrawGUI(Rect rect)
	{
		if (_update_mesh) {
			//update mesh
			UpdateMesh();
			//clear update mesh flag
			_update_mesh = false;
		}
		if (_render_mesh) {
			//update render vertices
			UpdateRenderVertex();
			//update preview render texture
			_previewutility.BeginPreview(rect, GUI.skin.GetStyle("DarkGrey"));
			if (target != null && target.vertices != null && drawVertices) {
				List<VertexVector> vectors = target.vertices;
				for (int k = 0; k < vectors.Count; k++) {
					Vector3 vertex = Quaternion.Euler(0, _hrot, 0) * vectors[k].GenerateVertexVector(0);
					bool selected = _primary_vertex.Contains(k);
					bool groupselected = _secondary_vertex.Contains(k);
					if (selected) {
						_previewutility.DrawMesh(VxlGUI.SelectVertex, vertex, Quaternion.identity, _previewmat, 0);
					}
					else if (groupselected) {
						_previewutility.DrawMesh(VxlGUI.GroupSelectVertex, vertex, Quaternion.identity, _previewmat, 0);
					}
					else {
						_previewutility.DrawMesh(VxlGUI.NonSelectVertex, vertex, Quaternion.identity, _previewmat, 0);
					}
				}
			}
			_previewutility.Render();
			_texture = _previewutility.EndPreview();
			//clear render mesh flag
			_render_mesh = false;
		}
		//return if nothing to draw
		if (_texture == null) {
			return;
		}
		//draw preview render texture
		GUI.DrawTexture(rect, _texture);
	}

	private void UpdateRenderVertex()
	{
		_primary_vertex.Clear();
		_secondary_vertex.Clear();
		switch (_drawmode) {
			case PreviewDrawMode.Vertex:
				_primary_vertex.Add(_primary_index);
				_secondary_vertex.Add(_secondary_index);
				break;
			case PreviewDrawMode.Triangle:
				UpdateTriangleVertexSelect();
				break;
			case PreviewDrawMode.EdgeSocket:
				if (target != null) {
					UpdateSocketSelect(target.edgesockets);
				}
				break;
			case PreviewDrawMode.FaceSocket:
				if (target != null) {
					UpdateSocketSelect(target.facesockets);
				}
				break;
		}
	}
	private void UpdateMesh()
	{
		_triangleobj.Clear();
		switch (_drawmode) {
			case PreviewDrawMode.Triangle:
				UpdateTriangles(true, true);
				break;
			case PreviewDrawMode.Simple:
			case PreviewDrawMode.Vertex:
			case PreviewDrawMode.EdgeSocket:
			case PreviewDrawMode.FaceSocket:
				UpdateTriangles(true, false);
				break;
		}
		_triangleobj.UploadMeshChanges();
	}

	#region Getters and Setters
	private bool drawVertices {
		get {
			return _drawmode == PreviewDrawMode.Vertex ||
				_drawmode == PreviewDrawMode.Triangle ||
				_drawmode == PreviewDrawMode.EdgeSocket || 
				_drawmode == PreviewDrawMode.FaceSocket;
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
	public void SetPrimaryIndex(int index)
	{
		_primary_index = index;
	}
	public void SetSecondaryIndex(int index)
	{
		_secondary_index = index;
	}
	public void SetDrawMode(PreviewDrawMode mode)
	{
		_drawmode = mode;
	}
	#endregion

	#region Vertex and Triangle
	private void UpdateTriangles(bool backface, bool select)
	{
		if (target == null || target.vertices == null || target.triangles == null) {
			return;
		}
		List<VertexVector> vertices = target.vertices;
		List<Triangle> triangles = target.triangles;
		for (int k = 0; k < triangles.Count; k++) {
			Triangle tri = triangles[k];
			if (!tri.IsValid(vertices.Count, null, null)) {
				continue;
			}
			Vector3 vertex0 = vertices[tri.vertex0].GenerateVertexVector(0);
			Vector3 vertex1 = vertices[tri.vertex1].GenerateVertexVector(0);
			Vector3 vertex2 = vertices[tri.vertex2].GenerateVertexVector(0);
			Vector3 normal = Vector3.Cross((vertex1 - vertex0).normalized, (vertex2 - vertex0).normalized);
			Vector2 uv = VxlGUI.NORMAL_UV;
			if (select) {
				if (_primary_index == k) {
					uv = VxlGUI.SELECT_UV;
				}
				else {
					uv = VxlGUI.NONSELECT_UV;
				}
			}
			AddTriangleMesh(vertex0, vertex1, vertex2, normal, uv, backface, ref _triangleobj);
		}
	}
	private void UpdateTriangleVertexSelect()
	{
		if (target == null || target.vertices == null || target.triangles == null) {
			return;
		}
		List<Triangle> triangles = target.triangles;
		if (_primary_index < 0 || _primary_index >= triangles.Count) {
			return;
		}
		Triangle tri = triangles[_primary_index];
		if (!tri.IsValid(target.vertices.Count, null, null)) {
			return;
		}
		_primary_vertex.Add(tri.vertex0);
		_primary_vertex.Add(tri.vertex1);
		_primary_vertex.Add(tri.vertex2);
	}
	private void UpdateSocketSelect(List<List<int>> sockets)
	{
		if (sockets == null) {
			return;
		}
		if (_primary_index < 0 || _primary_index >= sockets.Count || sockets[_primary_index] == null) {
			return;
		}
		List<int> axi_socket = sockets[_primary_index];
		for (int k = 0; k < axi_socket.Count; k++) {
			int vertex_index = axi_socket[k];
			if (_secondary_index == k) {
				_primary_vertex.Add(vertex_index);
			}
			_secondary_vertex.Add(vertex_index);
		}
	}
	private void AddVertexMesh(Vector3 point, Vector2 uv, ref PreviewObject obj)
	{
		int vertex_index = obj.vertices.Count;
		Vector3[] vertices = VxlGUI.NormalVertex.vertices;
		for (int k = 0; k < vertices.Length; k++) {
			obj.vertices.Add(point + vertices[k]);
			obj.normals.Add(vertices[k].normalized);
			obj.uvs.Add(uv);
		}
		int[] triangles = VxlGUI.NormalVertex.triangles;
		for (int k = 0; k < triangles.Length; k++) {
			obj.triangles.Add(vertex_index + triangles[k]);
		}
	}
	private void AddTriangleMesh(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 normal, Vector2 uv, bool drawbackface, ref PreviewObject obj)
	{
		int vertex_index = obj.vertices.Count;
		//add forward triangle
		obj.vertices.Add(vertex0);
		obj.normals.Add(normal);
		obj.uvs.Add(uv);
		obj.triangles.Add(vertex_index);

		obj.vertices.Add(vertex1);
		obj.normals.Add(normal);
		obj.uvs.Add(uv);
		obj.triangles.Add(vertex_index + 1);

		obj.vertices.Add(vertex2);
		obj.normals.Add(normal);
		obj.uvs.Add(uv);
		obj.triangles.Add(vertex_index + 2);

		if (drawbackface) {
			//add backward triangle
			obj.vertices.Add(vertex0);
			obj.normals.Add(-normal);
			obj.uvs.Add(uv);
			obj.triangles.Add(vertex_index + 3);

			obj.vertices.Add(vertex2);
			obj.normals.Add(-normal);
			obj.uvs.Add(uv);
			obj.triangles.Add(vertex_index + 4);

			obj.vertices.Add(vertex1);
			obj.normals.Add(-normal);
			obj.uvs.Add(uv);
			obj.triangles.Add(vertex_index + 5);
		}
	}
	#endregion

	#region Preview
	private void InitializePreviewUtility()
	{
		_previewutility = new PreviewRenderUtility();
		_previewutility.camera.backgroundColor = new Color(50 / 255f, 50 / 255f, 50 / 255f);
		_previewutility.camera.nearClipPlane = 0.01f;
		_previewutility.camera.farClipPlane = 1000f;
		_previewutility.cameraFieldOfView = 60;
		_previewutility.ambientColor = new Color(0.4f, 0.4f, 0.4f);

		_camfocus = Vector3.zero;
		_hrot = 0f;
		UpdateCameraTransform(0.5f, 1f);
	}
	public void ZoomPreview(float delta)
	{
		Vector3 pos = _previewutility.camera.transform.position;
		float zoom_factor = Mathf.Clamp01(-((pos.z + MINZOOM) / (MAXZOOM - MINZOOM)));
		float lower_bias = MINZOOM / MAXZOOM;
		float zoom_influence = 1 - lower_bias;
		float height_factor = pos.y / ((lower_bias + (zoom_influence * zoom_factor)) * MAXHEIGHT);
		zoom_factor += (delta * ZOOMSPEED);
		UpdateCameraTransform(zoom_factor, height_factor);
	}
	public void RotatePreview(float delta)
	{
		_hrot = (_hrot + (delta * HORIZONTALSPEED)) % 360;
		if (_hrot < 0) {
			_hrot += 360;
		}
		Quaternion objrot = Quaternion.Euler(0f, _hrot, 0f);
		_triangleobj.obj.transform.rotation = objrot;
	}
	public void MoveVerticalPreview(float delta)
	{
		Vector3 pos = _previewutility.camera.transform.position;
		float zoom_factor = Mathf.Clamp01(-((pos.z + MINZOOM) / (MAXZOOM - MINZOOM)));
		float lower_bias = MINZOOM / MAXZOOM;
		float zoom_influence = 1 - lower_bias;
		float height_factor = (pos.y + (delta * VERTICALSPEED)) / ((lower_bias + (zoom_influence * zoom_factor)) * MAXHEIGHT);
		UpdateCameraTransform(zoom_factor, height_factor);
	}
	public int VertexCast(Vector2 viewpos)
	{
		Ray ray = _previewutility.camera.ViewportPointToRay(viewpos);
		if (ray.direction == Vector3.zero || target == null || target.vertices == null) {
			return -1;
		}
		Quaternion vertex_rot = Quaternion.Euler(0f, _hrot, 0f);

		float nearest_t = Mathf.Infinity;
		int nearest_sphere = -1;
		float near_clip = _previewutility.camera.nearClipPlane;
		float far_clip = _previewutility.camera.farClipPlane;
		List<VertexVector> vertices = target.vertices;
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertex_rot * vertices[k].GenerateVertexVector(0);
			Vector3 point_vector = ray.origin - vertex;
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

	private void UpdateCameraTransform(float zoom_factor, float height_factor)
	{
		zoom_factor = Mathf.Clamp01(zoom_factor);
		height_factor = Mathf.Clamp(height_factor, -1, 1);
		float lower_bias = MINZOOM / MAXZOOM;
		float zoom_influence = 1 - lower_bias;
		float y = ((zoom_factor * zoom_influence) + lower_bias) * height_factor * MAXHEIGHT;
		float z = Mathf.Lerp(MINZOOM, MAXZOOM, zoom_factor);
		_previewutility.camera.transform.position = new Vector3(0, y, -z);
		_previewutility.camera.transform.LookAt(_camfocus, Vector3.up);
	}
	#endregion
}

public struct PreviewObject
{
	public List<Vector3> vertices;
	public List<Vector3> normals;
	public List<Vector2> uvs;
	public List<int> triangles;
	public Mesh mesh;
	public MeshRenderer renderer;
	public MeshCollider collider;
	public GameObject obj;

	public PreviewObject(string name)
	{
		vertices = new List<Vector3>();
		normals = new List<Vector3>();
		uvs = new List<Vector2>();
		triangles = new List<int>();

		//create obj
		obj = new GameObject(name);

		//create mesh
		mesh = new Mesh();
		mesh.name = name;
		mesh.MarkDynamic();

		//create mesh renderer
		renderer = obj.AddComponent<MeshRenderer>();
		renderer.name = name;
		renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		renderer.receiveShadows = false;
		renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		//create mesh filter
		MeshFilter filter = obj.AddComponent<MeshFilter>();
		filter.sharedMesh = mesh;

		//create mesh collider
		collider = obj.AddComponent<MeshCollider>();
		collider.convex = false;
		collider.isTrigger = false;
		collider.cookingOptions = MeshColliderCookingOptions.WeldColocatedVertices | 
								  MeshColliderCookingOptions.EnableMeshCleaning | 
								  MeshColliderCookingOptions.CookForFasterSimulation;
		//collider.sharedMesh = mesh;
	}

	public void Clear()
	{
		vertices.Clear();
		normals.Clear();
		uvs.Clear();
		triangles.Clear();
		mesh.Clear();
	}

	public void UploadMeshChanges()
	{
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uvs);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();
		mesh.UploadMeshData(false);
	}

	public void UpdateMaterial(Material material)
	{
		if (renderer.sharedMaterial == material) {
			return;
		}
		renderer.sharedMaterial = material;
	}

	public void Dispose()
	{
		vertices = null;
		normals = null;
		uvs = null;
		triangles = null;
		GameObject.DestroyImmediate(mesh);
		mesh = null;
		GameObject.DestroyImmediate(renderer);
		renderer = null;
		GameObject.DestroyImmediate(collider);
		collider = null;
		GameObject.DestroyImmediate(obj);
		obj = null;
	}
}

public enum PreviewDrawMode
{
	None,
	Simple,
	Vertex,
	Triangle,
	EdgeSocket,
	FaceSocket,
	CornerPlug,
	EdgePlug
}
