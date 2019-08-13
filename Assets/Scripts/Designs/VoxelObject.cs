using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoxelObject : ScriptableObject
{
	public string objname;
	public abstract void Initialize();
	public abstract bool IsValid();
}
