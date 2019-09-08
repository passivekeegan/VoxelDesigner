public struct RectPacket
{
	public readonly CornerPacket corner0;
	public readonly CornerPacket corner1;
	public readonly CornerPacket corner2;
	public readonly CornerPacket corner3;

	public RectPacket(
		CornerPacket corner0, CornerPacket corner1,
		CornerPacket corner2, CornerPacket corner3
	)
	{
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