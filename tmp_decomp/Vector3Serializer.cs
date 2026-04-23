using System;
using UnityEngine;

[Serializable]
public struct Vector3Serializer
{
	public float x;

	public float y;

	public float z;

	public Vector3 Data => new Vector3(x, y, z);

	public void SetData(Vector3 v3)
	{
		x = v3.x;
		y = v3.y;
		z = v3.z;
	}
}
