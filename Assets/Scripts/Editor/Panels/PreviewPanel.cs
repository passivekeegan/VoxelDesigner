using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PreviewPanel : PanelGUI
{
	private const float MINZOOM = 0.1f;
	private const float MAXZOOM = 1f;
	private const float ZOOMSPEED = 0.1f;
	private const float HORIZONTALROTSPEED = 1f;

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

	private VertexArgs _args;
	private PreviewObject _originobj;
	private PreviewObject _vertexobj;
	private PreviewObject _triangleobj;


	public PreviewPanel(Texture2D colourmap)
	{
		_drawmode = PreviewDrawMode.Vertex;
		_colourmap = colourmap;
		_args = new VertexArgs()
		{
			bevel = 0.1f,
			space = 0.256f
		};

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
		AddVertexMesh(Vector3.zero, originUV, ref _originobj);
		_originobj.UploadMeshChanges();
		_previewutility.AddSingleGO(_originobj.obj);
		_originobj.obj.transform.position = Vector3.zero;
		_originobj.obj.transform.rotation = Quaternion.identity;
		_originobj.obj.transform.localScale = Vector3.one;
		//initialize vertex object
		_vertexobj = new PreviewObject("Vertices");
		_vertexobj.UpdateMaterial(_previewmat);
		_vertexobj.UploadMeshChanges();
		_previewutility.AddSingleGO(_vertexobj.obj);
		_vertexobj.obj.transform.position = Vector3.zero;
		_vertexobj.obj.transform.rotation = Quaternion.identity;
		_vertexobj.obj.transform.localScale = Vector3.one;
		//initialize triangle object
		_triangleobj = new PreviewObject("Triangles");
		_triangleobj.UpdateMaterial(_previewmat);
		_triangleobj.UploadMeshChanges();
		_previewutility.AddSingleGO(_triangleobj.obj);
		_triangleobj.obj.transform.position = Vector3.zero;
		_triangleobj.obj.transform.rotation = Quaternion.identity;
		_triangleobj.obj.transform.localScale = Vector3.one;
		//mark dirty for update and repaint
		update = true;
		repaint = true;
	}

	public override void Disable()
	{
		_previewutility.Cleanup();
		_previewutility = null;
		_originobj.Dispose();
		_vertexobj.Dispose();
		_triangleobj.Dispose();
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

	public override void DrawGUI(Rect rect)
	{

		if (update) {
			//update mesh
			UpdateMesh();
			//update preview render texture
			_previewutility.BeginPreview(rect, GUI.skin.GetStyle("DarkGrey"));
			_previewutility.Render();
			_texture = _previewutility.EndPreview();
			//clear update mesh flag
			update = false;
		}
		//return if nothing to draw
		if (_texture == null) {
			return;
		}
		//draw preview render texture
		GUI.DrawTexture(rect, _texture);
		//clear repaint flag
		repaint = false;
	}


	private void UpdateMesh()
	{
		_vertexobj.Clear();
		_triangleobj.Clear();
		switch (_drawmode) {
			case PreviewDrawMode.Simple:
				UpdateSimpleModeMesh();
				break;
			case PreviewDrawMode.Vertex:
				UpdateVertexModeMesh();
				break;
			case PreviewDrawMode.Triangle:
				UpdateTriangleModeMesh();
				break;
			case PreviewDrawMode.EdgeSocket:
				UpdateEdgeSocketModeMesh();
				break;
			case PreviewDrawMode.FaceSocket:
				UpdateFaceSocketModeMesh();
				break;
		}
		_vertexobj.UploadMeshChanges();
		_triangleobj.UploadMeshChanges();
	}

	private void UpdateSimpleModeMesh()
	{
		if (target == null || target.vertices == null || target.triangles == null) {
			return;
		}
		List<VertexVector> vertices = target.vertices;
		List<Triangle> triangles = target.triangles;
		//draw triangles
		Vector2 uv = normalUV;
		for (int k = 0; k < triangles.Count; k++) {
			Triangle tri = triangles[k];
			if (!tri.IsValid(vertices.Count, null, null)) {
				continue;
			}
			Vector3 vertex0 = vertices[tri.vertex0].GenerateVertexVector(0, ref _args);
			Vector3 vertex1 = vertices[tri.vertex1].GenerateVertexVector(0, ref _args);
			Vector3 vertex2 = vertices[tri.vertex2].GenerateVertexVector(0, ref _args);
			Vector3 normal = Vector3.Cross((vertex1 - vertex0).normalized, (vertex2 - vertex0).normalized);
			AddTriangleMesh(vertex0, vertex1, vertex2, normal, uv, true, ref _triangleobj);
		}
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			AddVertexMesh(vertex, uv, ref _vertexobj);
		}
	}

	private void UpdateVertexModeMesh()
	{
		if (target == null || target.vertices == null || target.triangles == null) {
			return;
		}
		List<VertexVector> vertices = target.vertices;
		List<Triangle> triangles = target.triangles;
		//draw triangles
		Vector2 uv = normalUV;
		for (int k = 0; k < triangles.Count; k++) {
			Triangle tri = triangles[k];
			if (!tri.IsValid(vertices.Count, null, null)) {
				continue;
			}
			Vector3 vertex0 = vertices[tri.vertex0].GenerateVertexVector(0, ref _args);
			Vector3 vertex1 = vertices[tri.vertex1].GenerateVertexVector(0, ref _args);
			Vector3 vertex2 = vertices[tri.vertex2].GenerateVertexVector(0, ref _args);
			Vector3 normal = Vector3.Cross((vertex1 - vertex0).normalized, (vertex2 - vertex0).normalized);
			AddTriangleMesh(vertex0, vertex1, vertex2, normal, uv, true, ref _triangleobj);
		}
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			if (_primary_index == k) {
				AddVertexMesh(vertex, selectedUV, ref _vertexobj);
			}
			else {
				AddVertexMesh(vertex, nonSelectedUV, ref _vertexobj);
			}
		}
	}

	private void UpdateTriangleModeMesh()
	{
		if (target == null || target.vertices == null ||target.triangles == null) {
			return;
		}
		List<VertexVector> vertices = target.vertices;
		List<Triangle> triangles = target.triangles;
		//draw triangles
		int select_vertex0 = -1;
		int select_vertex1 = -1;
		int select_vertex2 = -1;
		for (int k = 0;k < triangles.Count;k++) {
			Triangle tri = triangles[k];
			if (!tri.IsValid(vertices.Count, null, null)) {
				continue;
			}
			Vector3 vertex0 = vertices[tri.vertex0].GenerateVertexVector(0, ref _args);
			Vector3 vertex1 = vertices[tri.vertex1].GenerateVertexVector(0, ref _args);
			Vector3 vertex2 = vertices[tri.vertex2].GenerateVertexVector(0, ref _args);
			Vector3 normal = Vector3.Cross((vertex1 - vertex0).normalized, (vertex2 - vertex0).normalized);
			Vector2 uv;
			if (_primary_index == k) {
				uv = selectedUV;
				select_vertex0 = tri.vertex0;
				select_vertex1 = tri.vertex1;
				select_vertex2 = tri.vertex2;
			}
			else {
				uv = nonSelectedUV;
			}
			AddTriangleMesh(vertex0, vertex1, vertex2, normal, uv, true, ref _triangleobj);
		}
		//draw vertices
		for (int k = 0;k < vertices.Count;k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			if (select_vertex0 == k || select_vertex1 == k || select_vertex2 == k) {
				AddVertexMesh(vertex, selectedUV, ref _vertexobj);
			}
			else {
				AddVertexMesh(vertex, nonSelectedUV, ref _vertexobj);
			}
		}
	}

	private void UpdateEdgeSocketModeMesh()
	{
		if (target == null || target.vertices == null || target.triangles == null || target.edgesockets == null) {
			return;
		}
		List<VertexVector> vertices = target.vertices;
		List<Triangle> triangles = target.triangles;
		//draw triangles
		for (int k = 0; k < triangles.Count; k++) {
			Triangle tri = triangles[k];
			if (!tri.IsValid(vertices.Count, null, null)) {
				continue;
			}
			Vector3 vertex0 = vertices[tri.vertex0].GenerateVertexVector(0, ref _args);
			Vector3 vertex1 = vertices[tri.vertex1].GenerateVertexVector(0, ref _args);
			Vector3 vertex2 = vertices[tri.vertex2].GenerateVertexVector(0, ref _args);
			Vector3 normal = Vector3.Cross((vertex1 - vertex0).normalized, (vertex2 - vertex0).normalized);
			Vector2 uv = normalUV;
			AddTriangleMesh(vertex0, vertex1, vertex2, normal, uv, true, ref _triangleobj);
		}
		//edge sockets
		HashSet<int> selectedgroup = new HashSet<int>();
		List<List<int>> edgesockets = target.edgesockets;
		int selected_index = -1;
		if (_primary_index >= 0 && _primary_index < edgesockets.Count && edgesockets[_primary_index] != null) {
			List<int> socket = edgesockets[_primary_index];
			for (int k = 0;k < socket.Count;k++) {
				int vertex_index = socket[k];
				if (_secondary_index == k) {
					selected_index = vertex_index;
				}
				selectedgroup.Add(vertex_index);
			}
		}
		//vertex mesh
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			if (selectedgroup.Contains(k)) {
				if (selected_index == k) {
					AddVertexMesh(vertex, selectedUV, ref _vertexobj);
				}
				else {
					AddVertexMesh(vertex, selectedGroupUV, ref _vertexobj);
				}
			}
			else {
				AddVertexMesh(vertex, nonSelectedUV, ref _vertexobj);
			}
		}
	}

	private void UpdateFaceSocketModeMesh()
	{
		if (target == null || target.vertices == null || target.triangles == null || target.facesockets == null) {
			return;
		}
		List<VertexVector> vertices = target.vertices;
		List<Triangle> triangles = target.triangles;
		//draw triangles
		for (int k = 0; k < triangles.Count; k++) {
			Triangle tri = triangles[k];
			if (!tri.IsValid(vertices.Count, null, null)) {
				continue;
			}
			Vector3 vertex0 = vertices[tri.vertex0].GenerateVertexVector(0, ref _args);
			Vector3 vertex1 = vertices[tri.vertex1].GenerateVertexVector(0, ref _args);
			Vector3 vertex2 = vertices[tri.vertex2].GenerateVertexVector(0, ref _args);
			Vector3 normal = Vector3.Cross((vertex1 - vertex0).normalized, (vertex2 - vertex0).normalized);
			Vector2 uv = normalUV;
			AddTriangleMesh(vertex0, vertex1, vertex2, normal, uv, true, ref _triangleobj);
		}
		//edge sockets
		HashSet<int> selectedgroup = new HashSet<int>();
		List<List<int>> facesockets = target.facesockets;
		int selected_index = -1;
		if (_primary_index >= 0 && _primary_index < facesockets.Count && facesockets[_primary_index] != null) {
			List<int> socket = facesockets[_primary_index];
			for (int k = 0; k < socket.Count; k++) {
				int vertex_index = socket[k];
				if (_secondary_index == k) {
					selected_index = vertex_index;
				}
				selectedgroup.Add(vertex_index);
			}
		}
		//vertex mesh
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			if (selectedgroup.Contains(k)) {
				if (selected_index == k) {
					AddVertexMesh(vertex, selectedUV, ref _vertexobj);
				}
				else {
					AddVertexMesh(vertex, selectedGroupUV, ref _vertexobj);
				}
			}
			else {
				AddVertexMesh(vertex, nonSelectedUV, ref _vertexobj);
			}
		}
	}

	private void AddVertexMesh(Vector3 point, Vector2 uv, ref PreviewObject obj)
	{
		int vertex_index = obj.vertices.Count;
		Vector3[] vertices = VxlGUI.VertexMesh.vertices;
		for (int k = 0; k < vertices.Length; k++) {
			obj.vertices.Add(point + (0.01f * vertices[k]));
			obj.normals.Add(vertices[k].normalized);
			obj.uvs.Add(uv);
		}
		int[] triangles = VxlGUI.VertexMesh.triangles;
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

	private Vector2 normalUV {
		get {
			return new Vector2(0.125f, 0.125f);
		}
	}
	private Vector2 selectedUV {
		get {
			return new Vector2(0.875f, 0.125f);
		}
	}
	private Vector2 selectedGroupUV {
		get {
			return new Vector2(0.625f, 0.375f);
		}
	}
	private Vector2 nonSelectedUV {
		get {
			return new Vector2(0.875f, 0.625f);
		}
	}
	private Vector2 originUV {
		get {
			return new Vector2(0.125f, 0.875f);
		}
	}

	private void InitializePreviewUtility()
	{
		_previewutility = new PreviewRenderUtility();
		_previewutility.camera.backgroundColor = new Color(50 / 255f, 50 / 255f, 50 / 255f);
		_previewutility.camera.nearClipPlane = 0.001f;
		_previewutility.camera.farClipPlane = 1000f;
		_previewutility.cameraFieldOfView = 60;
		_previewutility.ambientColor = new Color(0.4f, 0.4f, 0.4f);

		_camfocus = Vector3.zero;
		_hrot = 0f;
		_previewutility.camera.transform.position = MAXZOOM * (new Vector3(0, 1, -2)).normalized;
		_previewutility.camera.transform.LookAt(_camfocus, Vector3.up);
	}


	public void ZoomPreview(float delta)
	{
		float zoomlevel = _previewutility.camera.transform.position.magnitude;
		zoomlevel = Mathf.Clamp(zoomlevel + (delta * ZOOMSPEED), MINZOOM, MAXZOOM);
		_previewutility.camera.transform.position = zoomlevel * (new Vector3(0, 1, -2)).normalized;
		_previewutility.camera.transform.LookAt(_camfocus, Vector3.up);
	}

	public void RotatePreview(Vector2 delta)
	{
		_hrot = (_hrot + (delta.x * HORIZONTALROTSPEED)) % 360;
		if (_hrot < 0) {
			_hrot += 360;
		}
		Quaternion objrot = Quaternion.Euler(0f, _hrot, 0f);
		_vertexobj.obj.transform.rotation = objrot;
		_triangleobj.obj.transform.rotation = objrot;
	}
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
		collider.sharedMesh = mesh;
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
