public struct KeyPacket
{
	public int socketlevel;
	public int socketaxi;
	public IJL key;

	public KeyPacket(int socketlevel, int socketaxi, IJL key)
	{
		this.socketlevel = socketlevel;
		this.socketaxi = socketaxi;
		this.key = key;
	}

	public static KeyPacket empty {
		get {
			return new KeyPacket(0, 0, IJL.zero);
		}
	}
}

