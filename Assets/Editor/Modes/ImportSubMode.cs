using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportSubMode : SubModeScreen
{
	private Rect _rect_title;
	private Rect _rect_rotationlabel;
	private Rect _rect_rotation;
	private Rect _rect_scalelabel;
	private Rect _rect_scale;
	private Rect _rect_meshlabel;
	private Rect _rect_mesh;
	private Rect _rect_import;
	private Vector3 _rotation;
	private Vector3 _scale;
	private Mesh _mesh;

	public ImportSubMode(string title, MeshPreview preview)
	{
		_map = null;
		_design = null;
		_title = title;
		_preview = preview;
		_scale = Vector3.one;
	}

	public override void Enable(VoxelMapping map, VoxelDesign design)
	{
		_map = map;
		_design = design;
		_preview.rotation = Quaternion.Euler(_rotation);
		_preview.scale = _scale;
		UpdateMesh();
	}

	public override void Disable()
	{
		_map = null;
		_design = null;
		_preview.rotation = Quaternion.identity;
		_preview.scale = Vector3.one;
		_preview.ClearMesh();
	}

	private void UpdateMesh()
	{
		_preview.ClearMesh();
		List<Vector3> vertices = null;
		List<Vector3> normals = null;
		List<int> triangles = null;
		if (_mesh != null) {
			vertices = new List<Vector3>();
			normals = new List<Vector3>();
			triangles = new List<int>();
			_mesh.GetVertices(vertices);
			_mesh.GetNormals(normals);
			_mesh.GetTriangles(triangles, 0);
		}
		_preview.UpdateMeshComponents(vertices, normals, triangles);
	}

	public override void DrawGUI(Rect rect)
	{
		//update layout rects
		UpdateLayoutRects(rect);
		//draw title
		GUI.Label(_rect_title, _title);
		//draw rotation in euler angles
		EditorGUI.BeginChangeCheck();
		GUI.Label(_rect_rotationlabel, "Rotation", "LeftListElement");
		_rotation = EVx.DrawVector3Field(_rect_rotation, _rotation, "VectorLabel", "VectorDarkField10");
		_rotation.x = _rotation.x % 360;
		_rotation.y = _rotation.y % 360;
		_rotation.z = _rotation.z % 360;
		if (EditorGUI.EndChangeCheck()) {
			_preview.rotation = Quaternion.Euler(_rotation);
			_preview.DirtyRender();
		}
		//draw scale vector
		GUI.Label(_rect_scalelabel, "Scale", "LeftListElement");
		EditorGUI.BeginChangeCheck();
		_scale = EVx.DrawVector3Field(_rect_scale, _scale, "VectorLabel", "VectorDarkField10");
		if (EditorGUI.EndChangeCheck()) {
			_preview.scale = _scale;
			_preview.DirtyRender();
		}
		//draw mesh field
		GUI.Label(_rect_meshlabel, "Mesh", "LeftListElement");
		//draw object import field
		string name = "None";
		if (_mesh != null) {
			name = _mesh.name;
		}
		int picker_id = 948347;
		GUIContent content = new GUIContent(name, EditorGUIUtility.ObjectContent(null, typeof(Mesh)).image);
		if (GUI.Button(_rect_mesh, content, "ObjectField")) {
			EditorGUIUtility.ShowObjectPicker<Mesh>(_mesh, false, "", picker_id);
		}
		if (Event.current.commandName == "ObjectSelectorUpdated") {
			if (EditorGUIUtility.GetObjectPickerControlID() == picker_id) {
				_mesh = (Mesh)EditorGUIUtility.GetObjectPickerObject();
				UpdateMesh();
			}
		}
		//draw import button
		EditorGUI.BeginDisabledGroup(_mesh == null);
		if (GUI.Button(_rect_import, "Import", "DarkField10")) {
			ImportVertexAndTriangle();
		}
		EditorGUI.EndDisabledGroup();
	}
	private void UpdateLayoutRects(Rect rect)
	{
		float label_width = Mathf.Min(rect.width - 80, 0.5f * rect.width);
		_rect_title = EVx.GetAboveElement(rect, 0, EVx.LRG_LINE);
		//scale row
		Rect row_rect = EVx.GetAboveElement(rect, 0, EVx.MED_LINE, EVx.SM_SPACE, EVx.LRG_LINE);
		_rect_scalelabel = EVx.GetLeftElement(row_rect, 0, label_width);
		_rect_scale = EVx.GetRightElement(row_rect, 0, row_rect.width - label_width);
		//rotation row
		row_rect = EVx.GetAboveElement(rect, 1, EVx.MED_LINE, EVx.SM_SPACE, EVx.LRG_LINE);
		_rect_rotationlabel = EVx.GetLeftElement(row_rect, 0, label_width);
		_rect_rotation = EVx.GetRightElement(row_rect, 0, row_rect.width - label_width);
		//mesh row
		row_rect = EVx.GetAboveElement(rect, 2, EVx.MED_LINE, EVx.SM_SPACE, EVx.LRG_LINE);
		_rect_meshlabel = EVx.GetLeftElement(row_rect, 0, label_width);
		_rect_mesh = EVx.GetRightElement(row_rect, 0, row_rect.width - label_width);
		//import button
		row_rect = EVx.GetAboveElement(rect, 3, EVx.MED_LINE, EVx.SM_SPACE, EVx.LRG_LINE);
		_rect_import = EVx.GetRightElement(row_rect, 0, 80);
	}

	private void ImportVertexAndTriangle()
	{
		if (_map == null || !_map.isValid || _design == null || !_design.isValid) {
			return;
		}
		Vector3[] mesh_vertices = _mesh.vertices;
		int[] mesh_triangles = _mesh.triangles;
		if (mesh_vertices == null || mesh_triangles == null || mesh_vertices.Length <= 0) {
			return;
		}
		List<Point> vertices = _design.vertices;
		List<Triangle> triangles = _design.triangles;
		if (vertices == null || triangles == null) {
			return;
		}
		//maximum mesh vertex count was exceeded
		if (mesh_vertices.Length + vertices.Count > ushort.MaxValue + 1) {
			return;
		}
		Undo.RecordObject(_map, "Import Vertices and Triangles");
		//add mesh vertices
		int vertex_index = vertices.Count;
		Quaternion rotation = Quaternion.Euler(_rotation);
		for (int k = 0; k < mesh_vertices.Length; k++) {
			Vector3 vertex = rotation * Vector3.Scale(_scale, mesh_vertices[k]);
			vertices.Add(new Point(false, vertex));
		}
		//add mesh triangles
		int tri_count = mesh_triangles.Length / 3;
		for (int k = 0; k < tri_count; k++) {
			int index = 3 * k;
			triangles.Add(new Triangle(
				(vertex_index + mesh_triangles[index]),
				(vertex_index + mesh_triangles[index + 1]),
				(vertex_index + mesh_triangles[index + 2])
			));
		}
		//dirty target object
		EditorUtility.SetDirty(_map);
	}
}
