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
	)
	{
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