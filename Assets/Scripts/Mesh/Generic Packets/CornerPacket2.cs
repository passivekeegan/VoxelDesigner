public struct CornerPacket2
{
	public bool valid;
	public CornerPacket cp0;
	public CornerPacket cp1;

	public CornerPacket2(bool valid, CornerPacket cp0, CornerPacket cp1)
	{
		this.valid = valid && cp0.isValid && cp1.isValid;
		this.cp0 = cp0;
		this.cp1 = cp1;
	}

	public CornerPacket this[int index] {
		get {
			switch(index) {
				case 0:
					return cp0;
				case 1:
					return cp1;
				default:
					return CornerPacket.empty;
			}
		}
	}

	public static CornerPacket2 empty {
		get {
			return new CornerPacket2(false, CornerPacket.empty, CornerPacket.empty);
		}
	}
}
