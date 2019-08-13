using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vx
{
	public const float VXLSPACE = 0.1f;

	public readonly static int[] OPPAXI;
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
	public readonly static Vector3[] Hexagon;
	public readonly static Vector3[] MidHexagon;
	public readonly static Vector3[] HexagonUnit;
	public readonly static Vector3[] HexagonFrame;
	public readonly static Vector3[] HexagonSpace;

	public readonly static Quaternion[] AxiRot;

	
	private readonly static IJL Key_VxlX;
	private readonly static IJL Key_VxlY;
	private readonly static IJL Key_VxlZ;
	private readonly static IJL[] Key_AboveCorner;
	private readonly static IJL[] Key_BelowCorner;
	private readonly static IJL[] Key_AboveEdge;
	private readonly static IJL[] Key_MiddleEdge;
	private readonly static IJL[] Key_BelowEdge;
	private readonly static IJL[] Key_Face;
	
	private readonly static IJL[] AdjVoxelIJL;

	private readonly static IJL[] KeyTransform_Edge;
	private readonly static IJL[] KeyTransform_HexagonFaceCorner;
	private readonly static IJL[,] KeyTransform_RectFaceCorner;

	public readonly static byte[] InvAxis;

	static Vx()
	{
		OPPAXI = new int[] { 1, 0, 5, 6, 7, 2, 3, 4 };
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
		//XAxis = new Vector3(SQRT3, 0, 0);
		//YAxis = new Vector3(0, 1, 0);
		//ZAxis = new Vector3(SQRT3D2, 0, 1.5f);
		Hexagon = new Vector3[] {
			new Vector3(1,0f,SQRT3_R1),//new Vector3(SQRT3D2,0f,0.5f)
			new Vector3(1,0f,-SQRT3_R1),//new Vector3(SQRT3D2,0f,-0.5f)
			new Vector3(0f,0f,-SQRT3_R2),//new Vector3(0f,0f,-1f)
			new Vector3(-1,0f,-SQRT3_R1),//new Vector3(-SQRT3D2,0f,-0.5f)
			new Vector3(-1,0f,SQRT3_R1),//new Vector3(-SQRT3D2,0f,0.5f)
			new Vector3(0f,0f,SQRT3_R2)//new Vector3(0f,0f,1f)
		};
		MidHexagon = new Vector3[] {
			Vector3.Lerp(Hexagon[0], Hexagon[1], 0.5f),
			Vector3.Lerp(Hexagon[1], Hexagon[2], 0.5f),
			Vector3.Lerp(Hexagon[2], Hexagon[3], 0.5f),
			Vector3.Lerp(Hexagon[3], Hexagon[4], 0.5f),
			Vector3.Lerp(Hexagon[4], Hexagon[5], 0.5f),
			Vector3.Lerp(Hexagon[5], Hexagon[0], 0.5f)
		};
		HexagonFrame = new Vector3[] {
			new Vector3(1, 0.5f, SQRT3_R1),
			new Vector3(1, 0.5f, -SQRT3_R1),
			new Vector3(0, 0.5f, -SQRT3_R2),
			new Vector3(-1, 0.5f, -SQRT3_R1),
			new Vector3(-1, 0.5f, SQRT3_R1),
			new Vector3(0, 0.5f, SQRT3_R2)
		};
		HexagonSpace = new Vector3[] {
			VXLSPACE * Hexagon[0].normalized,
			VXLSPACE * Hexagon[1].normalized,
			VXLSPACE * Hexagon[2].normalized,
			VXLSPACE * Hexagon[3].normalized,
			VXLSPACE * Hexagon[4].normalized,
			VXLSPACE * Hexagon[5].normalized
		};
		HexagonUnit = new Vector3[] {
			Hexagon[0].normalized,//    D0
			MidHexagon[0].normalized,// M0
			Hexagon[1].normalized,//    D1
			MidHexagon[1].normalized,// M1
			Hexagon[2].normalized,//    D2
			MidHexagon[2].normalized,// M2
			Hexagon[3].normalized,//    D3
			MidHexagon[3].normalized,// M3
			Hexagon[4].normalized,//    D4
			MidHexagon[4].normalized,// M4
			Hexagon[5].normalized,//    D5
			MidHexagon[5].normalized//  M5
		};

		AxiRot = new Quaternion[] {
			Quaternion.identity,//       D0
			Quaternion.Euler(0,60,0),//  D1
			Quaternion.Euler(0,120,0),// D2
			Quaternion.Euler(0,180,0),// D3
			Quaternion.Euler(0,240,0),// D4
			Quaternion.Euler(0,300,0)//  D5
		};

		Key_VxlX = new IJL(0, 4, 0);
		Key_VxlY = new IJL(0, 0, 2);
		Key_VxlZ = new IJL(4, 2, 0);
		Key_AboveCorner = new IJL[] {
			new IJL(1, 2, 1),
			new IJL(-1, 2, 1),
			new IJL(-3, 0, 1),
			new IJL(-1, -2, 1),
			new IJL(1, -2, 1),
			new IJL(3, 0, 1)
		};
		Key_BelowCorner = new IJL[] {
			new IJL(1, 2, -1),
			new IJL(-1, 2, -1),
			new IJL(-3, 0, -1),
			new IJL(-1, -2, -1),
			new IJL(1, -2, -1),
			new IJL(3, 0, -1)
		};
		Key_AboveEdge = new IJL[] {
			new IJL(0, 2, 1),
			new IJL(-2, 1, 1),
			new IJL(-2, -1, 1),
			new IJL(0, -2, 1),
			new IJL(2, -1, 1),
			new IJL(2, 1, 1)
		};
		Key_MiddleEdge = new IJL[] {
			new IJL(1, 2, 0),
			new IJL(-1, 2, 0),
			new IJL(-3, 0, 0),
			new IJL(-1, -2, 0),
			new IJL(1, -2, 0),
			new IJL(3, 0, 0)
		};
		Key_BelowEdge = new IJL[] {
			new IJL(0, 2, -1),
			new IJL(-2, 1, -1),
			new IJL(-2, -1, -1),
			new IJL(0, -2, -1),
			new IJL(2, -1, -1),
			new IJL(2, 1, -1)
		};
		Key_Face = new IJL[] {
			new IJL(0, 0, 1),
			new IJL(0, 0, -1),
			new IJL(0, 2, 0),
			new IJL(-2, 1, 0),
			new IJL(-2, -1, 0),
			new IJL(0, -2, 0),
			new IJL(2, -1, 0),
			new IJL(2, 1, 0)
		};

		KeyTransform_Edge = new IJL[] {
			new IJL(0, 0, 1), //above
			new IJL(0, 0, -1), //below
			new IJL(-1, 0, 0), //d0
			new IJL(-1, -1, 0),//d1
			new IJL(1, -1, 0), //d2
			new IJL(1, 0, 0), //d3
			new IJL(1, 1, 0), //d4
			new IJL(-1, 1, 0), //d5
		};
		KeyTransform_HexagonFaceCorner = new IJL[] {
			new IJL(1, 2, 0),//d0
			new IJL(-1, 2, 0),//d1
			new IJL(-3, 0, 0),//d2
			new IJL(-1, -2, 0),//d3
			new IJL(1, -2, 0),//d4
			new IJL(3, 0, 0)//d5
		};

		KeyTransform_RectFaceCorner = new IJL[,] {
			{//d0
				Key_AboveCorner[0] - Key_Face[2],//c0
				Key_BelowCorner[0] - Key_Face[2],//c1
				Key_AboveCorner[1] - Key_Face[2],//c2
				Key_BelowCorner[1] - Key_Face[2],//c3
			},
			{//d1
				Key_AboveCorner[1] - Key_Face[3],//c0
				Key_BelowCorner[1] - Key_Face[3],//c1
				Key_AboveCorner[2] - Key_Face[3],//c2
				Key_BelowCorner[2] - Key_Face[3],//c3
			},
			{//d2
				Key_AboveCorner[2] - Key_Face[4],//c0
				Key_BelowCorner[2] - Key_Face[4],//c1
				Key_AboveCorner[3] - Key_Face[4],//c2
				Key_BelowCorner[3] - Key_Face[4],//c3
			},
			{//d3
				Key_AboveCorner[3] - Key_Face[5],//c0
				Key_BelowCorner[3] - Key_Face[5],//c1
				Key_AboveCorner[4] - Key_Face[5],//c2
				Key_BelowCorner[4] - Key_Face[5],//c3
			},
			{//d4
				Key_AboveCorner[4] - Key_Face[6],//c0
				Key_BelowCorner[4] - Key_Face[6],//c1
				Key_AboveCorner[5] - Key_Face[6],//c2
				Key_BelowCorner[5] - Key_Face[6],//c3
			},
			{//d5
				Key_AboveCorner[5] - Key_Face[7],//c0
				Key_BelowCorner[5] - Key_Face[7],//c1
				Key_AboveCorner[0] - Key_Face[7],//c2
				Key_BelowCorner[0] - Key_Face[7],//c3
			}
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

		InvAxis = new byte[256];
		for (int index = 0;index < 256; index++) {
			byte axis = (byte)index;
			byte invaxis = 0;
			//above
			if ((axis & 128) > 0) {
				invaxis += 64;
			}
			//below
			if ((axis & 64) > 0) {
				invaxis += 128;
			}
			//d0
			if ((axis & 32) > 0) {
				invaxis += 4;
			}
			//d1
			if ((axis & 16) > 0) {
				invaxis += 2;
			}
			//d2
			if ((axis & 8) > 0) {
				invaxis += 1;
			}
			//d3
			if ((axis & 4) > 0) {
				invaxis += 32;
			}
			//d4
			if ((axis & 2) > 0) {
				invaxis += 16;
			}
			//d5
			if ((axis & 1) > 0) {
				invaxis += 8;
			}
			InvAxis[axis] = invaxis;
		}
	}

	public static IJL VoxelKey(IJL ijl)
	{
		return (ijl.l * Key_VxlY) + (ijl.i * Key_VxlZ) + (ijl.j * Key_VxlX);
	}
	public static IJL IJLFromVoxelKey(IJL voxel_key)
	{
		return new IJL(voxel_key.i / 4, Mathf.FloorToInt((voxel_key.j / 4f) - (voxel_key.i / 8f)), voxel_key.l / 2);
	}
	//Corner Keys
	//normal
	public static IJL CornerKey_Above(IJL voxel_key, int d)
	{
		return Key_Standard(voxel_key, d, Key_AboveCorner);
	}
	public static IJL CornerKey_Below(IJL voxel_key, int d)
	{
		return Key_Standard(voxel_key, d, Key_BelowCorner);
	}
	//inverse
	public static IJL CornerInvKey_Above(IJL corner_key, int d)
	{
		return InverseKey_Standard(corner_key, d, Key_AboveCorner);
	}
	public static IJL CornerInvKey_Below(IJL corner_key, int d)
	{
		return InverseKey_Standard(corner_key, d, Key_BelowCorner);
	}

	//Edge Keys
	public static IJL EdgeKey_Above(IJL voxel_key, int d)
	{
		return Key_Standard(voxel_key, d, Key_AboveEdge);
	}
	public static IJL EdgeKey_Middle(IJL voxel_key, int d)
	{
		return Key_Standard(voxel_key, d, Key_MiddleEdge);
	}
	public static IJL EdgeKey_Below(IJL voxel_key, int d)
	{
		return Key_Standard(voxel_key, d, Key_BelowEdge);
	}
	//inverse
	public static IJL EdgeInvKey_Above(IJL edge_key, int d)
	{
		return InverseKey_Standard(edge_key, d, Key_AboveEdge);
	}
	public static IJL EdgeInvKey_Middle(IJL edge_key, int d)
	{
		return InverseKey_Standard(edge_key, d, Key_MiddleEdge);
	}
	public static IJL EdgeInvKey_Below(IJL edge_key, int d)
	{
		return InverseKey_Standard(edge_key, d, Key_BelowEdge);
	}

	//Face Keys
	public static IJL FaceKey(IJL voxel_key, int axi)
	{
		return Key_Axi(voxel_key, axi, Key_Face);
	}
	//inverse
	public static IJL FaceInvKey(IJL face_key, int axi)
	{
		return InverseKey_Axi(face_key, axi, Key_Face);
	}

	private static IJL Key_Standard(IJL voxel_key, int d, IJL[] arr)
	{
		if (d < 0 || d >= arr.Length) {
			throw new System.IndexOutOfRangeException();
		}
		return voxel_key + arr[d];
	}

	private static IJL InverseKey_Standard(IJL corner_key, int d, IJL[] arr)
	{
		if (d < 0 || d >= arr.Length) {
			throw new System.IndexOutOfRangeException();
		}
		return corner_key - arr[Vx.D[3,d]];
	}

	private static IJL Key_Axi(IJL voxel_key, int axi, IJL[] arr)
	{
		if (axi < 0 || axi >= 8) {
			throw new System.IndexOutOfRangeException();
		}
		return voxel_key + arr[axi];
	}
	private static IJL InverseKey_Axi(IJL component_key, int axi, IJL[] arr)
	{
		if (axi < 0 || axi >= 8) {
			throw new System.IndexOutOfRangeException();
		}
		return component_key - arr[Vx.OPPAXI[axi]];
	}

	//assuming corner_key is a valid key
	public static bool IsCornerTopHeavy(IJL corner_key)
	{
		IJL voxel_key = corner_key - Key_AboveCorner[1];
		float rem_j = Mathf.Abs(((2 * voxel_key.j) - voxel_key.i) / 8f) % 1;
		return (rem_j < 0.1f) || (rem_j > 0.9f);
	}

	public static bool IsEdgeTopHeavy(IJL edge_key)
	{
		IJL voxel_key = edge_key - Key_MiddleEdge[1];
		float rem_j = Mathf.Abs(((2 * voxel_key.j) - voxel_key.i) / 8f) % 1;
		return (rem_j < 0.1f) || (rem_j > 0.9f);
	}

	//calculate corner vertex from corner key
	public static Vector3 CornerVertex(IJL corner_key)
	{
		IJL voxel_key = corner_key;
		bool topheavy = IsCornerTopHeavy(corner_key);
		if (topheavy) {
			voxel_key -= Key_AboveCorner[1];
		}
		else {
			voxel_key -= Key_AboveCorner[0];
		}
		IJL ijl = IJLFromVoxelKey(voxel_key);
		Vector3 vertex = VoxelVertex(ijl);
		if (topheavy) {
			vertex += HexagonFrame[1];
		}
		else {
			vertex += HexagonFrame[0];
		}
		return vertex;
	}
	//assuming both corner_key and topheavy are accurate
	public static Vector3 CornerVertex(IJL corner_key, bool topheavy)
	{
		IJL voxel_key = corner_key;
		if (topheavy) {
			voxel_key -= Key_AboveCorner[1];
		}
		else {
			voxel_key -= Key_AboveCorner[0];
		}
		IJL ijl = IJLFromVoxelKey(voxel_key);
		Vector3 vertex = VoxelVertex(ijl);
		if (topheavy) {
			vertex += HexagonFrame[1];
		}
		else {
			vertex += HexagonFrame[0];
		}
		return vertex;
	}

	public static Vector3 FaceVertex(IJL face_key, int axi)
	{
		IJL voxel_ijl = IJLFromVoxelKey(FaceInvKey(face_key, axi));
		Vector3 vertex0 = VoxelVertex(voxel_ijl);
		voxel_ijl = IJLFromVoxelKey(FaceInvKey(face_key, Vx.OPPAXI[axi]));
		Vector3 vertex1 = VoxelVertex(voxel_ijl);
		return Vector3.Lerp(vertex0, vertex1, 0.5f);
	}

	public static Vector3 VoxelVertex(IJL voxel_ijl)
	{
		return (voxel_ijl.i * ZAxis) + (voxel_ijl.j * XAxis) + (voxel_ijl.l * YAxis);
	}

	public static IJL GetEdgeCorner(IJL edge_key, int axi)
	{
		if (axi < 0 || axi >= 8) {
			return IJL.zero;
		}
		return edge_key + KeyTransform_Edge[Vx.NROTAXI[2, axi]];
	}

	public static IJL GetFaceCorner(IJL face_key, int axi, int corner_index)
	{
		if (axi < 0 || axi >= 8 || corner_index < 0) {
			return IJL.zero;
		}
		if (axi < 2) {
			if (corner_index > 5) {
				return IJL.zero;
			}
			return face_key + KeyTransform_HexagonFaceCorner[corner_index];
		}
		else {
			if (corner_index > 3) {
				return IJL.zero;
			}
			return face_key + KeyTransform_RectFaceCorner[axi - 2, corner_index];
		}
	}



	//assuming edge_key is a valid Edge key
	public static void GetSegmentData(IJL edge_key, out IJL corner0_key, out IJL corner1_key, out int axi)
	{
		if ((edge_key.l % Key_VxlY.l) == 0) {
			corner0_key = edge_key + KeyTransform_Edge[1];
			corner1_key = edge_key + KeyTransform_Edge[0];
			axi = 0;
			return;
		}
		else if ((edge_key.i % Key_VxlZ.i) == 0) {
			corner0_key = edge_key + KeyTransform_Edge[5];
			corner1_key = edge_key + KeyTransform_Edge[2];
			axi = 4;
			return;
		}
		IJL voxel_key = edge_key - Key_AboveEdge[1];
		float rem_j = Mathf.Abs(((2 * voxel_key.j) - voxel_key.i) / 8f) % 1;
		if ((rem_j < 0.1f) || (rem_j > 0.9f)) {
			corner0_key = edge_key + KeyTransform_Edge[6];
			corner1_key = edge_key + KeyTransform_Edge[3];
			axi = 5;
			return;
		}
		else {
			corner0_key = edge_key + KeyTransform_Edge[7];
			corner1_key = edge_key + KeyTransform_Edge[4];
			axi = 6;
			return;
		}
	}

	public static void GetFacePacket(IJL face_key, out FacePacket packet)
	{
		if (Mathf.Abs(face_key.l) % 2 == 1) {
			packet = new FacePacket(
				0,
				face_key + Key_MiddleEdge[0],
				face_key + Key_MiddleEdge[1],
				face_key + Key_MiddleEdge[2],
				face_key + Key_MiddleEdge[3],
				face_key + Key_MiddleEdge[4],
				face_key + Key_MiddleEdge[5]
			);
			return;
		}
		else if ((face_key.i % Key_VxlZ.i) == 0) {
			packet = new FacePacket(
				2,
				face_key + KeyTransform_Edge[0] + KeyTransform_Edge[5],
				face_key + KeyTransform_Edge[1] + KeyTransform_Edge[5],
				face_key + KeyTransform_Edge[0] + KeyTransform_Edge[2],
				face_key + KeyTransform_Edge[1] + KeyTransform_Edge[2]
			);
			return;
		}

		IJL voxel_key = face_key - Key_Face[3];
		float rem_j = Mathf.Abs(((2 * voxel_key.j) - voxel_key.i) / 8f) % 1;
		if ((rem_j < 0.1f) || (rem_j > 0.9f)) {
			packet = new FacePacket(
				3,
				face_key + KeyTransform_Edge[0] + KeyTransform_Edge[6],
				face_key + KeyTransform_Edge[1] + KeyTransform_Edge[6],
				face_key + KeyTransform_Edge[0] + KeyTransform_Edge[3],
				face_key + KeyTransform_Edge[1] + KeyTransform_Edge[3]
			);
			return;
		}
		else {
			packet = new FacePacket(
				4,
				face_key + KeyTransform_Edge[0] + KeyTransform_Edge[7],
				face_key + KeyTransform_Edge[1] + KeyTransform_Edge[7],
				face_key + KeyTransform_Edge[0] + KeyTransform_Edge[4],
				face_key + KeyTransform_Edge[1] + KeyTransform_Edge[4]
			);
			return;
		}
	}
}