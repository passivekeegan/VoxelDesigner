using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomVoxelMesh : MonoBehaviour
{
	public bool flat_shade = true;
	public int rand_seed = 0;
	[Range(1, 1000)]
	public int voxel_samples = 10;
	[Range(4f, 100f)]
	public float range_radius = 10f;
	[Range(0f, 1f)]
	public float cutoff = 0.4f;
	public float bevel = 0.1f;
	public float space = 0.268f;
	public MeshFilter filter;
	public MappingObject map;
	public VoxelMeshData data;

	public void Start()
	{
		data = new VoxelMeshData();
		data.flat_shaded = flat_shade;
		data.bevel = bevel;
		data.space = space;
		data.map = map;

		Random.InitState(rand_seed);
		for (int k = 0; k < voxel_samples;k++) {
			Vector3 point = range_radius * Random.insideUnitSphere;
			if (Sample3DPerlin(point) < cutoff) {
				continue;
			}
			IJL ijl = new IJL(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y), Mathf.RoundToInt(point.z));
			data.UpdateVoxelData(ijl, new Voxel(1, VoxelAxi.AD0), true);
		}

		
		//data.UpdateVoxelData(new IJL(0, 1, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(-1, 2, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(-2, 2, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(0, 2, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(0, -2, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(0, -2, 1), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(-1, -2, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(-1, -2, 1), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(2, 0, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(2, 0, 1), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(2, 1, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(2, 0, 2), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(2, 1, 2), new Voxel(1, VoxelAxi.AD0), true);

		//data.UpdateVoxelData(new IJL(5, 0, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(5, 0, 1), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(5, 0, 2), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(6, 0, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(6, 0, 1), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(6, 0, 2), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(5, 1, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(5, 1, 2), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(2, -1, 7), new Voxel(1, VoxelAxi.AD0), true);


		//data.UpdateVoxelData(new IJL(5, -3, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(5, -4, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(6, -4, 0), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(6, -4, 1), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(5, -3, 2), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(5, -4, 2), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(6, -4, 2), new Voxel(1, VoxelAxi.AD0), true);


		//data.UpdateVoxelData(new IJL(6, -4, 5), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(7, -4, 6), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(6, -3, 6), new Voxel(1, VoxelAxi.AD0), true);

		//data.UpdateVoxelData(new IJL(-5, -3, 4), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(-5, -4, 4), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(-4, -4, 5), new Voxel(1, VoxelAxi.AD0), true);
		//data.UpdateVoxelData(new IJL(-5, -3, 5), new Voxel(1, VoxelAxi.AD0), true);


		data.UpdateDirtyComponents();

		data.GenerateMesh();

		filter.mesh = new Mesh();
		filter.mesh.SetVertices(data.vertices);
		filter.mesh.SetTriangles(data.triangles, 0);
		filter.mesh.RecalculateBounds();
		filter.mesh.RecalculateNormals();
		filter.mesh.RecalculateTangents();
		filter.mesh.UploadMeshData(false);
	}


	private float Sample3DPerlin(Vector3 p)
	{
		float perlin_sum = 0f;
		perlin_sum += Mathf.PerlinNoise(p.x, p.y);//xy
		perlin_sum += Mathf.PerlinNoise(p.y, p.x);//yx
		perlin_sum += Mathf.PerlinNoise(p.y, p.z);//yz
		perlin_sum += Mathf.PerlinNoise(p.z, p.y);//zy
		perlin_sum += Mathf.PerlinNoise(p.x, p.z);//xz
		perlin_sum += Mathf.PerlinNoise(p.z, p.x);//zx
		return perlin_sum / 6f;
	}
}
