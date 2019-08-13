using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CornerPacket
{
	public readonly CornerDesign design;
	public readonly int socket_axi_index;
	public readonly int index;
	public readonly int axi;

	public CornerPacket(CornerDesign design, int index, int socket_axi_index, int axi)
	{
		this.design = design;
		this.socket_axi_index = socket_axi_index;
		this.index = index;
		this.axi = axi;
	}

	public static CornerPacket empty {
		get {
			return new CornerPacket(null, -1, -1, 0);
		}
	}
}

public struct EdgePacket
{
	public readonly CornerPacket corner0;
	public readonly CornerPacket corner1;

	public EdgePacket(CornerPacket corner0, CornerPacket corner1)
	{
		this.corner0 = corner0;
		this.corner1 = corner1;
	}

	public CornerPacket this[int index] {
		get {
			switch (index) {
				case 0:
					return corner0;
				case 1:
					return corner1;
				default:
					return CornerPacket.empty;
			}

		}
	}

	public static EdgePacket empty {
		get {
			return new EdgePacket(
				CornerPacket.empty,
				CornerPacket.empty
			);
		}
	}
}

public struct RectPacket
{
	public readonly CornerPacket corner0;
	public readonly CornerPacket corner1;
	public readonly CornerPacket corner2;
	public readonly CornerPacket corner3;

	public RectPacket(
		CornerPacket corner0, CornerPacket corner1, 
		CornerPacket corner2, CornerPacket corner3
	) {
		this.corner0 = corner0;
		this.corner1 = corner1;
		this.corner2 = corner2;
		this.corner3 = corner3;
	}

	public CornerPacket this[int index] {
		get {
			switch (index) {
				case 0:
					return corner0;
				case 1:
					return corner1;
				case 2:
					return corner2;
				case 3:
					return corner3;
				default:
					return CornerPacket.empty;
			}

		}
	}

	public static RectPacket empty {
		get {
			return new RectPacket(
				CornerPacket.empty,
				CornerPacket.empty,
				CornerPacket.empty,
				CornerPacket.empty
			);
		}
	}
}

public struct HexagonPacket
{
	public readonly CornerPacket corner0;
	public readonly CornerPacket corner1;
	public readonly CornerPacket corner2;
	public readonly CornerPacket corner3;
	public readonly CornerPacket corner4;
	public readonly CornerPacket corner5;

	public HexagonPacket(
		CornerPacket corner0, CornerPacket corner1, 
		CornerPacket corner2, CornerPacket corner3, 
		CornerPacket corner4, CornerPacket corner5
	) {
		this.corner0 = corner0;
		this.corner1 = corner1;
		this.corner2 = corner2;
		this.corner3 = corner3;
		this.corner4 = corner4;
		this.corner5 = corner5;
	}

	public CornerPacket this[int index] {
		get {
			switch (index) {
				case 0:
					return corner0;
				case 1:
					return corner1;
				case 2:
					return corner2;
				case 3:
					return corner3;
				case 4:
					return corner4;
				case 5:
					return corner5;
				default:
					return CornerPacket.empty;
			}

		}
	}

	public static HexagonPacket empty {
		get {
			return new HexagonPacket(
				CornerPacket.empty,
				CornerPacket.empty,
				CornerPacket.empty,
				CornerPacket.empty,
				CornerPacket.empty,
				CornerPacket.empty
			);
		}
	}
}