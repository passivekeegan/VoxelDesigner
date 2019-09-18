public struct CornerPacket6
{
	public bool valid;
	public CornerPacket cp0;
	public CornerPacket cp1;
	public CornerPacket cp2;
	public CornerPacket cp3;
	public CornerPacket cp4;
	public CornerPacket cp5;

	public CornerPacket6(bool valid, 
		CornerPacket cp0, CornerPacket cp1, 
		CornerPacket cp2, CornerPacket cp3, 
		CornerPacket cp4, CornerPacket cp5)
	{
		this.valid = valid && cp0.isValid && cp1.isValid && 
			cp2.isValid && cp3.isValid && 
			cp4.isValid && cp5.isValid;
		this.cp0 = cp0;
		this.cp1 = cp1;
		this.cp2 = cp2;
		this.cp3 = cp3;
		this.cp4 = cp4;
		this.cp5 = cp5;
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
				case 4:
					return cp4;
				case 5:
					return cp5;
				default:
					return CornerPacket.empty;
			}
		}
	}

	public static CornerPacket6 empty {
		get {
			return new CornerPacket6(false, 
				CornerPacket.empty, 
				CornerPacket.empty, 
				CornerPacket.empty, 
				CornerPacket.empty, 
				CornerPacket.empty, 
				CornerPacket.empty);
		}
	}
}