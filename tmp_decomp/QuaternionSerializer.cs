using System;
using UnityEngine;

[Serializable]
public struct QuaternionSerializer
{
	public float x;

	public float y;

	public float z;

	public float w;

	public Quaternion Data => new Quaternion(x, y, z, w);

	public void SetData(Quaternion q)
	{
		x = q.x;
		y = q.y;
		z = q.z;
		w = q.w;
	}
}
