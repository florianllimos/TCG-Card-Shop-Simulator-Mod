using System;
using System.Collections.Generic;

[Serializable]
public class AutoCleanserSaveData
{
	public bool isNeedRefill;

	public bool isTurnedOn;

	public int itemAmount;

	public List<float> contentFillList;

	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public bool isBoxed;

	public Vector3Serializer boxedPackagePos;

	public QuaternionSerializer boxedPackageRot;

	public EObjectType objectType;
}
