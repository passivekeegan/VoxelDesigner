public struct KeyPacket2
{
	public bool valid;
	public KeyPacket pk0;
	public KeyPacket pk1;

	public KeyPacket2(bool valid, KeyPacket pk0, KeyPacket pk1)
	{
		this.valid = valid;
		this.pk0 = pk0;
		this.pk1 = pk1;
	}

	public KeyPacket this[int index] {
		get {
			switch (index) {
				case 0:
					return pk0;
				case 1:
					return pk1;
				default:
					return KeyPacket.empty;
			}
		}
	}

	public static KeyPacket2 empty {
		get {
			return new KeyPacket2(false, KeyPacket.empty, KeyPacket.empty);
		}
	}
}
