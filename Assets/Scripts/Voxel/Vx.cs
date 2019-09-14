using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vx
{
	public const float VXLSPACE = 0.1f;

	public readonly static int[] OPPAXI;
	public readonly static int[] YREFLAXI;
	public readonly static int[,] XREFLAXI;
	public readonly static int[,] ROTAXI;
	public readonly static int[,] NROTAXI;
	public readonly static int[,] D;
	public readonly static int[,] ND;

	public readonly static float SQRT3;
	public readonly static float SQRT3D2;
	public readonly static float SQRT3_R1;
	public readonly static float SQRT3_R2;

	public readonly static Vector3 XAxis;
	public readonly static Vector3 YAxis;
	public readonly static Vector3 ZAxis;
	public readonly static Vector3 Key_XAxis;
	public readonly static Vector3 Key_YAxis;
	public readonly static Vector3 Key_ZAxis;
	public readonly static Vector3 ReflVector;
	public readonly static Vector3[] Hexagon;
	public readonly static Vector3[] MidHexagon;
	public readonly static Vector3[] HexagonFrame;
	public readonly static Vector3[] HexagonSpace;
	public readonly static Vector3[] AdjHexagon;

	public readonly static Quaternion[] AxiRot;
	
	public readonly static IJL[] AdjVoxelIJL;
	public readonly static IJL[] AdjBlockIJL;

	static Vx()
	{
		OPPAXI = new int[] { 1, 0, 5, 6, 7, 2, 3, 4 };
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
		SQRT3 = Mathf.Sqrt(3f);
		SQRT3D2 = SQRT3 / 2f;
		SQRT3_R1 = 1 / SQRT3;
		SQRT3_R2 = 2 / SQRT3;
		XAxis = new Vector3(2, 0, 0);
		YAxis = new Vector3(0, 1, 0);
		ZAxis = new Vector3(1, 0, SQRT3);
		Key_XAxis = new Vector3(0.5f, 0, 0);
		Key_YAxis = new Vector3(0, 0.5f, 0);
		Key_ZAxis = new Vector3(0, 0, SQRT3 / 6f);
		Hexagon = new Vector3[] {
			new Vector3(1,0f,SQRT3_R1),
			new Vector3(1,0f,-SQRT3_R1),
			new Vector3(0f,0f,-SQRT3_R2),
			new Vector3(-1,0f,-SQRT3_R1),
			new Vector3(-1,0f,SQRT3_R1),
			new Vector3(0f,0f,SQRT3_R2)
		};
		ReflVector = Hexagon[0].normalized;
		HexagonFrame = new Vector3[] {
			new Vector3(1, 0.5f, SQRT3_R1),
			new Vector3(1, 0.5f, -SQRT3_R1),
			new Vector3(0, 0.5f, -SQRT3_R2),
			new Vector3(-1, 0.5f, -SQRT3_R1),
			new Vector3(-1, 0.5f, SQRT3_R1),
			new Vector3(0, 0.5f, SQRT3_R2)
		};
		AdjHexagon = new Vector3[]
		{
			new Vector3(0, 1, 0),
			new Vector3(0, -1, 0),
			new Vector3(2,0,0),
			new Vector3(1,0,-SQRT3),
			new Vector3(-1,0,-SQRT3),
			new Vector3(-2,0,0),
			new Vector3(-1,0,SQRT3),
			new Vector3(1,0,SQRT3)
		};

		AxiRot = new Quaternion[] {
			Quaternion.identity,//       D0
			Quaternion.Euler(0,60,0),//  D1
			Quaternion.Euler(0,120,0),// D2
			Quaternion.Euler(0,180,0),// D3
			Quaternion.Euler(0,240,0),// D4
			Quaternion.Euler(0,300,0)//  D5
		};
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