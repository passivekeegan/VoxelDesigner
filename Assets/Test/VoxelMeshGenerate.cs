using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMeshGenerate : MonoBehaviour
{
	public bool flat_shaded = true;
	public int rand_seed = 0;
	public int height = 10;
	public float perlin_scale = 1f;
	public float heightmap_radius = 20f;
	[Range(1, 10)]
	public int chunk_radius = 4;
	public Material material;
	public MappingObject voxelstyle;
	public List<Vector3Int> customvoxels;

	private VoxelMeshData _data;
	private List<IJL> _chunklist;
	private HashSet<IJL> _sampleset;
	private Dictionary<IJL, List<IJL>> _chunkmap;
	private List<GameObject> _chunkobjs;
	private List<IJL> _chunksobj_ijls;
	private Dictionary<IJL, GameObject> _chunkobjmap;

	public void Start()
	{
		_data = new VoxelMeshData();
		_chunklist = new List<IJL>();
		_sampleset = new HashSet<IJL>();
		_chunkmap = new Dictionary<IJL, List<IJL>>();

		_chunkobjs = new List<GameObject>();
		_chunksobj_ijls = new List<IJL>();
		_chunkobjmap = new Dictionary<IJL, GameObject>();

		GenerateChunks();
	}

	public void GenerateChunks()
	{
		Random.InitState(rand_seed);
		//clear any existing chunk data
		ClearChunkData();
		//add sampled voxel ijl to chunk data
		AddHeightMapVoxelIJL();
		//add custom voxel ijl to chunk data
		AddCustomVoxels();
		//delete chunk objects
		int index = 0;
		while (index < _chunkobjs.Count) {
			IJL ijl = _chunksobj_ijls[index];
			if (_chunkmap.ContainsKey(ijl)) {
				index += 1;
			}
			else {
				Destroy(_chunkobjs[index]);
				_chunkobjs.RemoveAt(index);
				_chunksobj_ijls.RemoveAt(index);
				_chunkobjmap.Remove(ijl);
			}
		}
		//add and update chunk objects
		for (int k = 0; k < _chunklist.Count; k++) {
			IJL chunk = _chunklist[k];
			if (!_chunkobjmap.ContainsKey(chunk)) {
				//create new chunk object
				_chunksobj_ijls.Add(chunk);
				_chunkobjs.Add(null);
				InitializeChunkObject(_chunkobjs.Count - 1);
				_chunkobjmap.Add(chunk, _chunkobjs[_chunkobjs.Count - 1]);
			}
			UpdateChunkMesh(chunk);
		}
	}

	private void UpdateChunkMesh(IJL chunk)
	{
		if (!_chunkobjmap .ContainsKey(chunk) || !_chunkmap.ContainsKey(chunk)) {
			return;
		}
		_data.Clear();
		_data.flat_shaded = flat_shaded;
		_data.map = voxelstyle;
		int cnt = Vx.AdjBlockIJL.Length;
		IJL chunkvoxel = Vx.ChunkVoxelIJL(chunk, chunk_radius);
		List<IJL> voxels = _chunkmap[chunk];
		for (int k = 0;k < voxels.Count;k++) {
			_data.UpdateVoxelData(voxels[k], new Voxel(1), true);
			IJL voxelijl = chunkvoxel + voxels[k];
			for (int cell_index = 0;cell_index < cnt;cell_index++) {
				IJL adjvoxel = voxelijl + Vx.AdjBlockIJL[cell_index];
				if (Vx.ChunkIJL(adjvoxel, chunk_radius) == chunk) {
					continue;
				}
				if (!_sampleset.Contains(adjvoxel)) {
					continue;
				}
				_data.UpdateVoxelData(voxels[k] + Vx.AdjBlockIJL[cell_index], new Voxel(1), false);
			}
		}
		_data.UpdateDirtyComponents();
		_data.GenerateMesh();

		MeshFilter filter = _chunkobjmap[chunk].GetComponent<MeshFilter>();
		if (filter.mesh == null) {
			filter.mesh = new Mesh();
		}
		filter.mesh.SetVertices(_data.vertices);
		filter.mesh.SetNormals(_data.normals);
		filter.mesh.SetTriangles(_data.triangles, 0);
		filter.mesh.RecalculateBounds();
		filter.mesh.RecalculateTangents();
		filter.mesh.UploadMeshData(false);
	}

	private void InitializeChunkObject(int index)
	{
		if (index < 0 || index >= _chunkobjs.Count || _chunkobjs[index] != null) {
			return;
		}
		IJL chunk = _chunksobj_ijls[index];
		_chunkobjs[index] = new GameObject("Chunk " + chunk.ToString());
		_chunkobjs[index].transform.localPosition = VMD.VoxelVertex(Vx.ChunkVoxelIJL(chunk, chunk_radius));

		_chunkobjs[index].AddComponent<MeshFilter>();
		MeshRenderer renderer = _chunkobjs[index].AddComponent<MeshRenderer>();
		renderer.sharedMaterial = material;
	}

	private void ClearChunkData()
	{
		_chunklist.Clear();
		_sampleset.Clear();
		_chunkmap.Clear();
	}

	private void AddHeightMapVoxelIJL()
	{
		float sqdist = heightmap_radius * heightmap_radius;
		int max = Mathf.CeilToInt(heightmap_radius);
		for (int i = -max; i <= max; i++) {
			for (int j = -max; j <= max; j++) {
				//distance validity check
				Vector3 v = VMD.VoxelVertex(new IJL(i, j, 0));
				if (VMD.VoxelVertex(new IJL(i, j, 0)).sqrMagnitude > sqdist) {
					continue;
				}
				float h = height * Mathf.PerlinNoise(perlin_scale * (j + 0.1f), perlin_scale * (i + 0.1f));
				int max_l = Mathf.CeilToInt(h);
				for (int l = 0; l < max_l; l++) {
					if (l > h) {
						continue;
					}
					AddVoxelIJL(new IJL(i, j, l));
				}
			}
		}
	}

	private void AddCustomVoxels()
	{
		if (customvoxels == null) {
			return;
		}
		for (int k = 0; k < customvoxels.Count; k++) {
			AddVoxelIJL(new	IJL(customvoxels[k]));
		}
	}

	private void AddVoxelIJL(IJL ijl)
	{
		if (!_sampleset.Add(ijl)) {
			return;
		}
		IJL chunk = Vx.ChunkIJL(ijl, chunk_radius);
		if (!_chunkmap.ContainsKey(chunk)) {
			_chunkmap.Add(chunk, new List<IJL>());
			_chunklist.Add(chunk);
		}
		_chunkmap[chunk].Add(ijl - Vx.ChunkVoxelIJL(chunk, chunk_radius));
	}

}
