using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AxiSocket
{
	[SerializeField]
	public List<int> normal;
	[SerializeField]
	public List<int> invx;
	[SerializeField]
	public List<int> invy;
	[SerializeField]
	public List<int> invxy;

	public AxiSocket(List<int> normal, List<int> invx, List<int> invy, List<int> invxy)
	{
		this.normal = normal;
		this.invx = invx;
		this.invy = invy;
		this.invxy = invxy;
	}

	public List<int> this[bool invertx, bool inverty] {
		get {
			if (invertx && inverty) {
				return invxy;
			}
			else if (invertx) {
				return invx;
			}
			else if (inverty) {
				return invy;
			}
			else {
				return normal;
			}
		}
	}

	public bool IsValid()
	{
		return normal != null && invx != null && invy != null && invxy != null;
	}

	public static AxiSocket empty
	{
		get {
			return new AxiSocket(null, null, null, null);
		}
	}
}
