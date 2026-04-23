using System;
using System.Collections.Generic;

[Serializable]
public class AutoPackOpenerSaveData
{
	public bool isProcessing;

	public int packOpenedCount;

	public List<EItemType> itemTypeList;

	public List<CompactCardDataAmount> compactCardDataAmountList;

	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public bool isBoxed;

	public Vector3Serializer boxedPackagePos;

	public QuaternionSerializer boxedPackageRot;

	public EObjectType objectType;
}
