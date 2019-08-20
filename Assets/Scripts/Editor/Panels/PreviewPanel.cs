using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PreviewPanel : PanelGUI
{
	public VoxelComponent target;



	private int _primary_index;
	private int _secondary_index;
	//general
	private bool _vxlflag_axi;
	private bool _vxlflag_frame;
	private bool _vxlflag_origin;

	//triangles
	private bool _triflag_facenormal;
	private bool _triflag_labels;


	private Texture2D _colourmap;
	private PreviewDrawMode _drawmode;
	private Material _previewmat;
	private PreviewRenderUtility _previewutility;
	private Texture _texture;

	private Mesh _previewmesh;
	private List<Vector3> _vertices;
	private List<Vector3> _normals;
	private List<Vector2> _uvs;
	private List<int> _triangles;

	private VertexArgs _args;
	private HashSet<int> _workset;

	public PreviewPanel(Texture2D colourmap)
	{
		_drawmode = PreviewDrawMode.Vertex;
		_colourmap = colourmap;
		_args = new VertexArgs()
		{
			bevel = 0.1f,
			space = 0.256f
		};
		_workset = new HashSet<int>();

		_primary_index = -1;
		_secondary_index = -1;

		_previewmat = new Material(Shader.Find("Diffuse"));
		_previewmat.mainTexture = _colourmap;

		_vertices = new List<Vector3>();
		_normals = new List<Vector3>();
		_uvs = new List<Vector2>();
		_triangles = new List<int>();

		_previewmesh = new Mesh();
		_previewmesh.MarkDynamic();
		
		UploadMeshChanges();
	}

	public override void Enable()
	{
		_previewmesh.Clear();
		_vertices.Clear();
		_normals.Clear();
		_uvs.Clear();
		_triangles.Clear();
		UploadMeshChanges();

		InitializePreviewUtility();
		update = true;
		repaint = true;
	}

	public override void Disable()
	{
		_previewmesh.Clear();
		_vertices.Clear();
		_normals.Clear();
		_uvs.Clear();
		_triangles.Clear();
		UploadMeshChanges();

		_previewutility.Cleanup();
		_previewutility = null;
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
			_previewutility.DrawMesh(_previewmesh, Vector3.zero, Quaternion.identity, _previewmat, 0);
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
		_vertices.Clear();
		_normals.Clear();
		_uvs.Clear();
		_triangles.Clear();
		_previewmesh.Clear();
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
		UploadMeshChanges();
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
			
			int vertex_index = _vertices.Count;
			//add forward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 1);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 2);

			//add backward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 3);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 4);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 5);
		}
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			AddVertexMesh(vertex, uv);
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
			int vertex_index = _vertices.Count;
			//add forward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 1);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 2);

			//add backward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 3);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 4);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 5);
		}
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			if (_primary_index == k) {
				AddVertexMesh(vertex, selectedUV);
			}
			else {
				AddVertexMesh(vertex, nonSelectedUV);
			}
		}
	}

	private void UpdateTriangleModeMesh()
	{
		if (target == null || target.vertices == null ||target.triangles == null) {
			return;
		}
		_workset.Clear();
		List<VertexVector> vertices = target.vertices;
		List<Triangle> triangles = target.triangles;
		//draw triangles
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
				//add to selected vertices
				_workset.Add(tri.vertex0);
				_workset.Add(tri.vertex1);
				_workset.Add(tri.vertex2);
			}
			else {
				uv = nonSelectedUV;
			}
			int vertex_index = _vertices.Count;
			//add forward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 1);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 2);

			//add backward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 3);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 4);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 5);
		}
		//draw vertices
		for (int k = 0;k < vertices.Count;k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			if (_workset.Contains(k)) {
				AddVertexMesh(vertex, selectedUV);
			}
			else {
				AddVertexMesh(vertex, nonSelectedUV);
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
			int vertex_index = _vertices.Count;
			//add forward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 1);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 2);

			//add backward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 3);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 4);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 5);
		}
		//edge sockets
		_workset.Clear();
		List<List<int>> edgesockets = target.edgesockets;
		int selected_index = -1;
		if (_primary_index >= 0 && _primary_index < edgesockets.Count && edgesockets[_primary_index] != null) {
			List<int> socket = edgesockets[_primary_index];
			for (int k = 0;k < socket.Count;k++) {
				int vertex_index = socket[k];
				if (_secondary_index == k) {
					selected_index = vertex_index;
				}
				_workset.Add(vertex_index);
			}
		}
		//vertex mesh
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			if (_workset.Contains(k)) {
				if (selected_index == k) {
					AddVertexMesh(vertex, selectedUV);
				}
				else {
					AddVertexMesh(vertex, selectedGroupUV);
				}
			}
			else {
				AddVertexMesh(vertex, nonSelectedUV);
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
			int vertex_index = _vertices.Count;
			//add forward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 1);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 2);

			//add backward triangle
			_vertices.Add(vertex0);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 3);

			_vertices.Add(vertex2);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 4);

			_vertices.Add(vertex1);
			_normals.Add(normal);
			_uvs.Add(uv);
			_triangles.Add(vertex_index + 5);
		}
		//edge sockets
		_workset.Clear();
		List<List<int>> facesockets = target.facesockets;
		int selected_index = -1;
		if (_primary_index >= 0 && _primary_index < facesockets.Count && facesockets[_primary_index] != null) {
			List<int> socket = facesockets[_primary_index];
			for (int k = 0; k < socket.Count; k++) {
				int vertex_index = socket[k];
				if (_secondary_index == k) {
					selected_index = vertex_index;
				}
				_workset.Add(vertex_index);
			}
		}
		//vertex mesh
		for (int k = 0; k < vertices.Count; k++) {
			Vector3 vertex = vertices[k].GenerateVertexVector(0, ref _args);
			if (_workset.Contains(k)) {
				if (selected_index == k) {
					AddVertexMesh(vertex, selectedUV);
				}
				else {
					AddVertexMesh(vertex, selectedGroupUV);
				}
			}
			else {
				AddVertexMesh(vertex, nonSelectedUV);
			}
		}
	}

	private void AddVertexMesh(Vector3 point, Vector2 uv)
	{
		int vertex_index = _vertices.Count;
		Vector3[] vertices = VxlGUI.VertexMesh.vertices;
		for (int k = 0; k < vertices.Length; k++) {
			_vertices.Add(point + (0.01f * vertices[k]));
			_normals.Add(vertices[k].normalized);
			_uvs.Add(uv);
		}
		int[] triangles = VxlGUI.VertexMesh.triangles;
		for (int k = 0; k < triangles.Length; k++) {
			_triangles.Add(vertex_index + triangles[k]);
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

	private void InitializePreviewUtility()
	{
		_previewutility = new PreviewRenderUtility();
		_previewutility.camera.backgroundColor = new Color(50 / 255f, 50 / 255f, 50 / 255f);
		_previewutility.camera.nearClipPlane = 0.001f;
		_previewutility.camera.farClipPlane = 1000f;
		_previewutility.cameraFieldOfView = 60;
		_previewutility.camera.transform.position = new Vector3(0, 0, -1);
		_previewutility.ambientColor = new Color(0.4f, 0.4f, 0.4f);
		//_previewutility.camera.transform = 
	}

	private void UploadMeshChanges()
	{
		_previewmesh.SetVertices(_vertices);
		_previewmesh.SetNormals(_normals);
		_previewmesh.SetUVs(0, _uvs);
		_previewmesh.SetTriangles(_triangles, 0);
		_previewmesh.RecalculateBounds();
		_previewmesh.RecalculateTangents();
		_previewmesh.UploadMeshData(false);
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
