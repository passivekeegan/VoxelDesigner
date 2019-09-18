using UnityEngine;

public static class VMD
{
	private readonly static IJL AxisKey_VoxelX;
	private readonly static IJL AxisKey_VoxelY;
	private readonly static IJL AxisKey_VoxelZ;

	public readonly static IJL[,] KeyTransform_VoxelCorner;
	public readonly static IJL[,] KeyTransform_CornerVoxel;
	public readonly static IJL[,] KeyTransform_VoxelLateral;
	public readonly static IJL[,] KeyTransform_LateralVoxel;
	public readonly static IJL[] KeyTransform_VoxelLongitude;
	public readonly static IJL[] KeyTransform_LongitudeVoxel;
	public readonly static IJL[] KeyTransform_VoxelRect;
	public readonly static IJL[] KeyTransform_RectVoxel;
	public readonly static IJL[] KeyTransform_VoxelHexagon;
	public readonly static IJL[] KeyTransform_HexagonVoxel;

	public readonly static IJL[] KeyTransform_CornerLateral;
	public readonly static IJL[,] KeyTransform_LateralCorner;
	public readonly static IJL[] KeyTransform_CornerLongitude;
	public readonly static IJL[,] KeyTransform_LongitudeCorner;
	public readonly static IJL[,] KeyTransform_CornerRect;
	public readonly static IJL[,] KeyTransform_RectCorner;
	public readonly static IJL[] KeyTransform_CornerHexagon;
	public readonly static IJL[] KeyTransform_HexagonCorner;

	public readonly static IJL[] KeyTransform_LateralRect;
	public readonly static IJL[] KeyTransform_RectLateral;
	public readonly static IJL[] KeyTransform_LateralHexagon;
	public readonly static IJL[] KeyTransform_HexagonLateral;
	public readonly static IJL[] KeyTransform_LongitudeRect;
	public readonly static IJL[,] KeyTransform_RectLongitude;

	static VMD()
	{
		AxisKey_VoxelX = new IJL(0, 4, 0);
		AxisKey_VoxelY = new IJL(0, 0, 2);
		AxisKey_VoxelZ = new IJL(6, 2, 0);

		//Corner Voxel Transforms
		KeyTransform_VoxelCorner = new IJL[,] {
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
		KeyTransform_CornerVoxel = KeyTransform_VoxelCorner;
		//Lateral Voxel Transforms
		KeyTransform_VoxelLateral = new IJL[,] {
			{
				new IJL(0, 2, 1),
				new IJL(0, 2, -1)
			},
			{
				new IJL(-3, 1, 1),
				new IJL(-3, 1, -1)
			},
			{
				new IJL(-3, -1, 1),
				new IJL(-3, -1, -1)
			},
			{
				new IJL(0, -2, 1),
				new IJL(0, -2, -1)
			},
			{
				new IJL(3, -1, 1),
				new IJL(3, -1, -1)
			},
			{
				new IJL(3, 1, 1),
				new IJL(3, 1, -1)
			}
		};
		KeyTransform_LateralVoxel = KeyTransform_VoxelLateral;
		//Longitude Voxel Transforms
		KeyTransform_VoxelLongitude = new IJL[] {
			new IJL(2, 2, 0),
			new IJL(-2, 2, 0),
			new IJL(-4, 0, 0),
			new IJL(-2, -2, 0),
			new IJL(2, -2, 0),
			new IJL(4, 0, 0)
		};
		KeyTransform_LongitudeVoxel = KeyTransform_VoxelLongitude;
		//Rect Voxel Transforms
		KeyTransform_VoxelRect = new IJL[] {
			new IJL(0, 2, 0),
			new IJL(-3, 1, 0),
			new IJL(-3, -1, 0),
			new IJL(0, -2, 0),
			new IJL(3, -1, 0),
			new IJL(3, 1, 0)
		};
		KeyTransform_RectVoxel = KeyTransform_VoxelRect;
		//Hexagon Voxel Transforms
		KeyTransform_VoxelHexagon = new IJL[] {
			new IJL(0, 0, 1),
			new IJL(0, 0, -1)
		};
		KeyTransform_HexagonVoxel = KeyTransform_VoxelHexagon;

		//Corner to Other Components
		KeyTransform_CornerLateral = new IJL[] {
			new IJL(1, 1, 0),
			new IJL(-1, 1, 0),
			new IJL(-2, 0, 0),
			new IJL(-1, -1, 0),
			new IJL(1, -1, 0),
			new IJL(2, 0, 0)
		};
		KeyTransform_CornerLongitude = new IJL[] {
			new IJL(0, 0, 1),
			new IJL(0, 0, -1)
		};
		KeyTransform_CornerRect = new IJL[,] {
			{
				new IJL(1, 1, 1),
				new IJL(1, 1, -1)
			},
			{
				new IJL(-1, 1, 1),
				new IJL(-1, 1, -1)
			},
			{
				new IJL(-2, 0, 1),
				new IJL(-2, 0, -1),
			},
			{
				new IJL(-1, -1, 1),
				new IJL(-1, -1, -1)
			},
			{
				new IJL(1, -1, 1),
				new IJL(1, -1, -1)
			},
			{
				new IJL(2, 0, 1),
				new IJL(2, 0, -1)
			}
		};
		KeyTransform_CornerHexagon = new IJL[] {
			new IJL(2, 2, 0),
			new IJL(-2, 2, 0),
			new IJL(-4, 0, 0),
			new IJL(-2, -2, 0),
			new IJL(2, -2, 0),
			new IJL(4, 0, 0)
		};
		//Lateral Edge to Other Components
		KeyTransform_LateralCorner = new IJL[,] {
			{
				KeyTransform_CornerLateral[5],
				KeyTransform_CornerLateral[2]
			},
			{
				KeyTransform_CornerLateral[0],
				KeyTransform_CornerLateral[3]
			},
			{
				KeyTransform_CornerLateral[1],
				KeyTransform_CornerLateral[4]
			},
			{
				KeyTransform_CornerLateral[2],
				KeyTransform_CornerLateral[5],
			},
			{
				KeyTransform_CornerLateral[3],
				KeyTransform_CornerLateral[0]
			},
			{
				KeyTransform_CornerLateral[4],
				KeyTransform_CornerLateral[1]
			}
		};
		KeyTransform_LateralRect = new IJL[] {
			new IJL(0, 0, 1),
			new IJL(0, 0, -1)
		};
		KeyTransform_LateralHexagon = new IJL[] {
			new IJL(0, 2, 0),
			new IJL(-3, 1, 0),
			new IJL(-3, -1, 0),
			new IJL(0, -2, 0),
			new IJL(3, -1, 0),
			new IJL(3, 1, 0)
		};
		//Longitude Edge to Other Components
		KeyTransform_LongitudeCorner = new IJL[,] {
			{
				KeyTransform_CornerLongitude[1],
				KeyTransform_CornerLongitude[0]
			},
			{
				KeyTransform_CornerLongitude[0],
				KeyTransform_CornerLongitude[1]
			}
		};
		KeyTransform_LongitudeRect = new IJL[] {
			new IJL(1, 1, 0),
			new IJL(-1, 1, 0),
			new IJL(-2, 0, 0),
			new IJL(-1, -1, 0),
			new IJL(1, -1, 0),
			new IJL(2, 0, 0)
		};
		//Rect Face to Other Components
		KeyTransform_RectCorner = new IJL[,] {
			{
				new IJL(2, 0, 1),
				new IJL(2, 0, -1),
				new IJL(-2, 0, -1),
				new IJL(-2, 0, 1)
			},
			{
				new IJL(1, 1, 1),
				new IJL(1, 1, -1),
				new IJL(-1, -1, -1),
				new IJL(-1, -1, 1)
			},
			{
				new IJL(-1, 1, 1),
				new IJL(-1, 1, -1),
				new IJL(1, -1, -1),
				new IJL(1, -1, 1)
			},
			{
				new IJL(-2, 0, 1),
				new IJL(-2, 0, -1),
				new IJL(2, 0, -1),
				new IJL(2, 0, 1)
			},
			{
				new IJL(-1, -1, 1),
				new IJL(-1, -1, -1),
				new IJL(1, 1, -1),
				new IJL(1, 1, 1)
			},
			{
				new IJL(1, -1, 1),
				new IJL(1, -1, -1),
				new IJL(-1, 1, -1),
				new IJL(-1, 1, 1)
			}
		};
		KeyTransform_RectLateral = new IJL[] {
			KeyTransform_LateralRect[0],
			KeyTransform_LateralRect[1]
		};
		KeyTransform_RectLongitude = new IJL[,]
		{
			{
				new IJL(2, 0, 0),
				new IJL(-2, 0, 0)
			},
			{
				new IJL(1, 1, 0),
				new IJL(-1, -1, 0)
			},
			{
				new IJL(-1, 1, 0),
				new IJL(1, -1, 0)
			},
			{
				new IJL(-2, 0, 0),
				new IJL(2, 0, 0)
			},
			{
				new IJL(-1, -1, 0),
				new IJL(1, 1, 0)
			},
			{
				new IJL(1, -1, 0),
				new IJL(-1, 1, 0)
			}
		};
		//Hexagon Face to Other Components
		KeyTransform_HexagonCorner = new IJL[] {
			KeyTransform_CornerHexagon[0],
			KeyTransform_CornerHexagon[1],
			KeyTransform_CornerHexagon[2],
			KeyTransform_CornerHexagon[3],
			KeyTransform_CornerHexagon[4],
			KeyTransform_CornerHexagon[5]
		};
		KeyTransform_HexagonLateral = new IJL[] {
			KeyTransform_LateralHexagon[0],
			KeyTransform_LateralHexagon[1],
			KeyTransform_LateralHexagon[2],
			KeyTransform_LateralHexagon[3],
			KeyTransform_LateralHexagon[4],
			KeyTransform_LateralHexagon[5]
		};
	}
	public static IJL VoxelKey(IJL voxel_ijl)
	{
		return  (voxel_ijl.j * AxisKey_VoxelX) + (voxel_ijl.i * AxisKey_VoxelZ) + (voxel_ijl.l * AxisKey_VoxelY);
	}

	public static IJL VoxelKeyIJL(IJL voxel_key)
	{
		return new IJL(voxel_key.i / 6, Mathf.FloorToInt((voxel_key.j / 4f) - (voxel_key.i / 12f)), voxel_key.l / 2);
	}

	public static Vector3 VoxelVertex(IJL voxel_ijl)
	{
		return (voxel_ijl.i * Vx.ZAxis) + (voxel_ijl.j * Vx.XAxis) + (voxel_ijl.l * Vx.YAxis);
	}

	public static Vector3 KeyVertex(IJL key)
	{
		return (key.i * Vx.Key_ZAxis) + (key.j * Vx.Key_XAxis) + (key.l * Vx.Key_YAxis);
	}

	public static bool IsKeyTopHeavy(IJL key)
	{
		IJ key_diff = new IJ(key.i + 2, key.j - 2);
		int rem_i = key_diff.i % 6;
		int rem_j =  ((3 * key_diff.j) - key_diff.i) % 12;
		return (rem_i == 0) && (rem_j == 0);
	}

	public static IJL GetVoxelIJL(Vector3 point)
	{
		Vector3 trfm_point = InverseTransform(point);
		int min_i = Mathf.FloorToInt(trfm_point.z);
		int max_i = Mathf.CeilToInt(trfm_point.z);
		int min_j = Mathf.FloorToInt(trfm_point.x);
		int max_j = Mathf.CeilToInt(trfm_point.x);
		int l = Mathf.FloorToInt(point.y);
		IJL ijl0 = new IJL(min_i, min_j, l);
		float d0 = SquaredDistanceXZ(point, VoxelVertex(ijl0));
		IJL ijl1 = new IJL(min_i, max_j, l);
		float d1 = SquaredDistanceXZ(point, VoxelVertex(ijl1));
		IJL ijl2 = new IJL(max_i, min_j, l);
		float d2 = SquaredDistanceXZ(point, VoxelVertex(ijl2));
		IJL ijl3 = new IJL(max_i, max_j, l);
		float d3 = SquaredDistanceXZ(point, VoxelVertex(ijl3));
		float min = Mathf.Min(d0, d1, d2, d3);
		if (d0 <= min) {
			return new IJL(min_i, min_j, l);
		}
		else if (d1 <= min) {
			return new IJL(min_i, max_j, l);
		}
		else if (d2 <= min) {
			return new IJL(max_i, min_j, l);
		}
		else {
			return new IJL(max_i, max_j, l);
		}
	}

	private static float SquaredDistanceXZ(Vector3 a, Vector3 b)
	{
		return Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z, 2);
	}
	private static Vector3 InverseTransform(Vector3 point)
	{
		Vector3 pos = point;
		pos.x = (point.x / 2f) + ((-Vx.SQRT3_R1 / 2f) * point.z);
		pos.z = (Vx.SQRT3_R1 * point.z);
		return pos;
	}

	public static int ClassifyLatticKeyAxi(IJL key)
	{
		int i = key.i;
		int j = key.j - 2;
		int rem_i = i % 6;
		int rem_j = ((3 * j) - i) % 12;
		if ((rem_i == 0) && (rem_j == 0)) {
			return 2;
		}
		i = key.i + 3;
		rem_i = i % 6;
		if (rem_i == 0) {
			j = key.j - 1;
			rem_j = ((3 * j) - i) % 12;
			if (rem_j == 0) {
				return 3;
			}
			j = key.j + 1;
			rem_j = ((3 * j) - i) % 12;
			if (rem_j == 0) {
				return 4;
			}
		}
		return -1;
	}

	#region KeyPacket Methods
	public static KeyPacket2 GetLateralCornerKeys(IJL edge_key, int axi)
	{
		if (axi < 2 || axi > 7) {
			return KeyPacket2.empty;
		}
		int d = axi - 2;
		int forward_axi = Vx.ROTAXI[2, axi];
		int backward_axi = Vx.NROTAXI[1, axi];
		return new KeyPacket2(true,
			new KeyPacket(0, forward_axi, edge_key + KeyTransform_LateralCorner[d, 0]),
			new KeyPacket(0, backward_axi, edge_key + KeyTransform_LateralCorner[d, 1])
		);
	}
	public static KeyPacket2 GetLongitudeCornerKeys(IJL edge_key, int axi)
	{
		if (axi < 0 || axi > 1) {
			return KeyPacket2.empty;
		}
		int forward_axi = axi;
		int backward_axi = Vx.OPPAXI[axi];
		return new KeyPacket2(true,
			new KeyPacket(0, forward_axi, edge_key + KeyTransform_LongitudeCorner[axi, 0]),
			new KeyPacket(0, backward_axi, edge_key + KeyTransform_LongitudeCorner[axi, 1])
		);
	}
	public static KeyPacket4 GetRectCornerKeys(IJL face_key, int axi)
	{
		if (axi < 2 || axi > 7) {
			return KeyPacket4.empty;
		}
		int d = axi - 2;
		int forward_axi = Vx.ROTAXI[2, axi];
		int backward_axi = Vx.NROTAXI[1, axi];
		return new KeyPacket4(true,
			new KeyPacket(-1, forward_axi, face_key + KeyTransform_RectCorner[d, 0]),
			new KeyPacket(1, forward_axi, face_key + KeyTransform_RectCorner[d, 1]),
			new KeyPacket(1, backward_axi, face_key + KeyTransform_RectCorner[d, 2]),
			new KeyPacket(-1, backward_axi, face_key + KeyTransform_RectCorner[d, 3])
		);
	}
	public static KeyPacket6 GetHexagonCornerKeys(IJL face_key, int axi)
	{
		if (axi < 0 || axi > 1) {
			return KeyPacket6.empty;
		}
		return new KeyPacket6(true,
			new KeyPacket(0, 5, face_key + KeyTransform_HexagonCorner[0]),
			new KeyPacket(0, 6, face_key + KeyTransform_HexagonCorner[1]),
			new KeyPacket(0, 7, face_key + KeyTransform_HexagonCorner[2]),
			new KeyPacket(0, 2, face_key + KeyTransform_HexagonCorner[3]),
			new KeyPacket(0, 3, face_key + KeyTransform_HexagonCorner[4]),
			new KeyPacket(0, 4, face_key + KeyTransform_HexagonCorner[5])
		);
	}
	#endregion
}
