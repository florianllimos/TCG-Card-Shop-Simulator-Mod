using System;
using System.Collections.Generic;

[Serializable]
public class ShelfSaveData
{
	public List<ItemTypeAmount> itemTypeAmountList;

	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public bool isBoxed;

	public Vector3Serializer boxedPackagePos;

	public QuaternionSerializer boxedPackageRot;

	public EObjectType objectType;
}
