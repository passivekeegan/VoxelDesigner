public struct KeyPacket4
{
	public bool valid;
	public KeyPacket pk0;
	public KeyPacket pk1;
	public KeyPacket pk2;
	public KeyPacket pk3;

	public KeyPacket4(bool valid, KeyPacket pk0, KeyPacket pk1, KeyPacket pk2, KeyPacket pk3)
	{
		this.valid = valid;
		this.pk0 = pk0;
		this.pk1 = pk1;
		this.pk2 = pk2;
		this.pk3 = pk3;
	}

	public KeyPacket this[int index] {
		get {
			switch (index) {
				case 0:
					return pk0;
				case 1:
					return pk1;
				case 2:
					return pk2;
				case 3:
					return pk3;
				default:
					return KeyPacket.empty;
			}
		}
	}

	public static KeyPacket4 empty {
		get {
			return new KeyPacket4(false, KeyPacket.empty, KeyPacket.empty, KeyPacket.empty, KeyPacket.empty);
		}
	}
}