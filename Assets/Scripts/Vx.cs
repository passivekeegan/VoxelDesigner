using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vx
{
	public readonly static int[,] D;
	public readonly static int[,] ND;
	public readonly static int[] YREFLAXI;
	public readonly static int[,] XREFLAXI;
	public readonly static int[,] ROTAXI;
	public readonly static int[,] NROTAXI;
	public readonly static Vector3[] Hexagon;
	public readonly static Vector3 ReflVector;
	public readonly static Quaternion[] AxiRot;

	public readonly static float SQRT3;
	public readonly static float SQRT3_R1;
	public readonly static float SQRT3_R2;

	public readonly static IJL[] KeyTransform_BlockFace;
	public readonly static IJL[,] KeyTransform_VoxelBlock;
	public readonly static IJL[] AdjVoxelIJL;
	public readonly static IJL[] AdjBlockIJL;


	public readonly static Vector3 XAxis;
	public readonly static Vector3 YAxis;
	public readonly static Vector3 ZAxis;
	public readonly static Vector3 Key_XAxis;
	public readonly static Vector3 Key_YAxis;
	public readonly static Vector3 Key_ZAxis;
	private readonly static IJL AxiKey_VoxelX;
	private readonly static IJL AxiKey_VoxelY;
	private readonly static IJL AxiKey_VoxelZ;

	static Vx()
	{
		SQRT3 = Mathf.Sqrt(3f);
		SQRT3_R1 = 1 / SQRT3;
		SQRT3_R2 = 2 / SQRT3;
		D = new int[,] {
			{ 0, 1, 2, 3, 4, 5 },//+0
			{ 1, 2, 3, 4, 5, 0 },//+1
			{ 2, 3, 4, 5, 0, 1 },//+2
			{ 3, 4, 5, 0, 1, 2 },//+3
			{ 4, 5, 0, 1, 2, 3 },//+4
			{ 5, 0, 1, 2, 3, 4 } //+5
		};
		ND = new int[,] {
			{ 0, 1, 2, 3, 4, 5 },//-0
			{ 5, 0, 1, 2, 3, 4 },//-1
			{ 4, 5, 0, 1, 2, 3 },//-2
			{ 3, 4, 5, 0, 1, 2 },//-3
			{ 2, 3, 4, 5, 0, 1 },//-4
			{ 1, 2, 3, 4, 5, 0 } //-5
		};
		YREFLAXI = new int[] { 1, 0, 2, 3, 4, 5, 6, 7 };
		XREFLAXI = new int[,] {
			{ 0, 1, 2, 3, 4, 5, 6, 7 },
			{ 0, 1, 2, 3, 4, 5, 6, 7 },
			{ 0, 1, 2, 7, 6, 5, 4, 3 },
			{ 0, 1, 4, 3, 2, 7, 6, 5 },
			{ 0, 1, 6, 5, 4, 3, 2, 7 },
			{ 0, 1, 2, 7, 6, 5, 4, 3 },
			{ 0, 1, 4, 3, 2, 7, 6, 5 },
			{ 0, 1, 6, 5, 4, 3, 2, 7 }
		};
		ROTAXI = new int[,] {
			{ 0, 1, 2, 3, 4, 5, 6, 7},
			{ 0, 1, 3, 4, 5, 6, 7, 2},
			{ 0, 1, 4, 5, 6, 7, 2, 3},
			{ 0, 1, 5, 6, 7, 2, 3, 4},
			{ 0, 1, 6, 7, 2, 3, 4, 5},
			{ 0, 1, 7, 2, 3, 4, 5, 6}
		};
		NROTAXI = new int[,] {
			{ 0, 1, 2, 3, 4, 5, 6, 7},
			{ 0, 1, 7, 2, 3, 4, 5, 6},
			{ 0, 1, 6, 7, 2, 3, 4, 5},
			{ 0, 1, 5, 6, 7, 2, 3, 4},
			{ 0, 1, 4, 5, 6, 7, 2, 3},
			{ 0, 1, 3, 4, 5, 6, 7, 2}
		};
		Hexagon = new Vector3[] {
			new Vector3(1,0f,SQRT3_R1),
			new Vector3(1,0f,-SQRT3_R1),
			new Vector3(0f,0f,-SQRT3_R2),
			new Vector3(-1,0f,-SQRT3_R1),
			new Vector3(-1,0f,SQRT3_R1),
			new Vector3(0f,0f,SQRT3_R2)
		};
		ReflVector = Hexagon[0].normalized;
		AxiRot = new Quaternion[] {
			Quaternion.identity,//       D0
			Quaternion.Euler(0,60,0),//  D1
			Quaternion.Euler(0,120,0),// D2
			Quaternion.Euler(0,180,0),// D3
			Quaternion.Euler(0,240,0),// D4
			Quaternion.Euler(0,300,0)//  D5
		};
		
		XAxis = new Vector3(2, 0, 0);
		YAxis = new Vector3(0, 1, 0);
		ZAxis = new Vector3(1, 0, SQRT3);
		Key_XAxis = new Vector3(0.5f, 0, 0);
		Key_YAxis = new Vector3(0, 0.5f, 0);
		Key_ZAxis = new Vector3(0, 0, SQRT3 / 6f);

		AxiKey_VoxelX = new IJL(0, 4, 0);
		AxiKey_VoxelY = new IJL(0, 0, 2);
		AxiKey_VoxelZ = new IJL(6, 2, 0);

		AdjVoxelIJL = new IJL[] {
			new IJL(0, 0, 1),
			new IJL(0, 0, -1),
			new IJL(0, 1, 0),
			new IJL(-1, 1, 0),
			new IJL(-1, 0, 0),
			new IJL(0, -1, 0),
			new IJL(1, -1, 0),
			new IJL(1, 0, 0)
		};
		AdjBlockIJL = new IJL[] {
			AdjVoxelIJL[0],
			AdjVoxelIJL[0] + AdjVoxelIJL[2],
			AdjVoxelIJL[0] + AdjVoxelIJL[3],
			AdjVoxelIJL[0] + AdjVoxelIJL[4],
			AdjVoxelIJL[0] + AdjVoxelIJL[5],
			AdjVoxelIJL[0] + AdjVoxelIJL[6],
			AdjVoxelIJL[0] + AdjVoxelIJL[7],
			AdjVoxelIJL[2],
			AdjVoxelIJL[3],
			AdjVoxelIJL[4],
			AdjVoxelIJL[5],
			AdjVoxelIJL[6],
			AdjVoxelIJL[7],
			AdjVoxelIJL[1],
			AdjVoxelIJL[1] + AdjVoxelIJL[2],
			AdjVoxelIJL[1] + AdjVoxelIJL[3],
			AdjVoxelIJL[1] + AdjVoxelIJL[4],
			AdjVoxelIJL[1] + AdjVoxelIJL[5],
			AdjVoxelIJL[1] + AdjVoxelIJL[6],
			AdjVoxelIJL[1] + AdjVoxelIJL[7]
		};

		KeyTransform_VoxelBlock = new IJL[,] {
			{
				new IJL(2, 2, 1),
				new IJL(2, 2, -1)
			},
			{
				new IJL(-2, 2, 1),
				new IJL(-2, 2, -1)
			},
			{
				new IJL(-4, 0, 1),
				new IJL(-4, 0, -1)
			},
			{
				new IJL(-2, -2, 1),
				new IJL(-2, -2, -1)
			},
			{
				new IJL(2, -2, 1),
				new IJL(2, -2, -1)
			},
			{
				new IJL(4, 0, 1),
				new IJL(4, 0, -1)
			}
		};
		KeyTransform_BlockFace = new IJL[] {
			new IJL(0,0, 1),
			new IJL(0, 0, -1),
			new IJL(2, 2, 0),
			new IJL(-2, 2, 0),
			new IJL(-4, 0, 0),
			new IJL(-2, -2, 0),
			new IJL(2, -2, 0),
			new IJL(4, 0, 0)
		};
	}
	public static IJL VoxelKey(IJL voxel_ijl)
	{
		return (voxel_ijl.j * AxiKey_VoxelX) + (voxel_ijl.i * AxiKey_VoxelZ) + (voxel_ijl.l * AxiKey_VoxelY);
	}
	public static IJL VoxelKeyIJL(IJL voxel_key)
	{
		return new IJL(voxel_key.i / 6, Mathf.FloorToInt((voxel_key.j / 4f) - (voxel_key.i / 12f)), voxel_key.l / 2);
	}
	public static Vector3 KeyVertex(IJL key)
	{
		return (key.i * Key_ZAxis) + (key.j * Key_XAxis) + (key.l * Key_YAxis);
	}
	public static Vector3 VoxelVertex(IJL voxel_ijl)
	{
		return (voxel_ijl.i * ZAxis) + (voxel_ijl.j * XAxis) + (voxel_ijl.l * YAxis);
	}
	public static IJL ChunkVoxelIJL(IJL chunk, int radius)
	{
		return ((2 * radius) - 1) * chunk;
	}
	public static IJL ChunkIJL(IJL voxelijl, int radius)
	{
		return voxelijl / radius;
	}
}
