public struct CornerPacket4
{
	public bool valid;
	public CornerPacket cp0;
	public CornerPacket cp1;
	public CornerPacket cp2;
	public CornerPacket cp3;

	public CornerPacket4(bool valid, CornerPacket cp0, CornerPacket cp1, CornerPacket cp2, CornerPacket cp3)
	{
		this.valid = valid && cp0.isValid && cp1.isValid && cp2.isValid && cp3.isValid;
		this.cp0 = cp0;
		this.cp1 = cp1;
		this.cp2 = cp2;
		this.cp3 = cp3;
	}

	public CornerPacket this[int index] {
		get {
			switch (index) {
				case 0:
					return cp0;
				case 1:
					return cp1;
				case 2:
					return cp2;
				case 3:
					return cp3;
				default:
					return CornerPacket.empty;
			}
		}
	}

	public static CornerPacket4 empty {
		get {
			return new CornerPacket4(false, CornerPacket.empty, CornerPacket.empty, CornerPacket.empty, CornerPacket.empty);
		}
	}
}
