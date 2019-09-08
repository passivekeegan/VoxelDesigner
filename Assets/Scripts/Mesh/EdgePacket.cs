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