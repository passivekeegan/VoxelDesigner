public struct PatternMatch
{
	public readonly bool invx;
	public readonly bool invy;
	public readonly int shift;

	public PatternMatch(bool invx, bool invy, int shift)
	{
		this.invx = invx;
		this.invy = invy;
		this.shift = shift;
	}

	public static PatternMatch empty {
		get {
			return new PatternMatch(false, false, -1);
		}
	}
}

