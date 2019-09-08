public struct FacePacket
{
	public readonly bool hexagon;
	public readonly int axi;
	public readonly IJL ckey0;
	public readonly IJL ckey1;
	public readonly IJL ckey2;
	public readonly IJL ckey3;
	public readonly IJL ckey4;
	public readonly IJL ckey5;

	public FacePacket(int axi, IJL ckey0, IJL ckey1, IJL ckey2, IJL ckey3, IJL ckey4, IJL ckey5)
	{
		this.hexagon = true;
		this.axi = axi;
		this.ckey0 = ckey0;
		this.ckey1 = ckey1;
		this.ckey2 = ckey2;
		this.ckey3 = ckey3;
		this.ckey4 = ckey4;
		this.ckey5 = ckey5;
	}

	public FacePacket(int axi, IJL ckey0, IJL ckey1, IJL ckey2, IJL ckey3)
	{
		this.hexagon = false;
		this.axi = axi;
		this.ckey0 = ckey0;
		this.ckey1 = ckey1;
		this.ckey2 = ckey2;
		this.ckey3 = ckey3;
		this.ckey4 = IJL.zero;
		this.ckey5 = IJL.zero;
	}
}