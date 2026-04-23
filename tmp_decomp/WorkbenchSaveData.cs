using System;
using System.Collections.Generic;

[Serializable]
public class WorkbenchSaveData
{
	public List<EItemType> itemTypeList;

	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public bool isBoxed;

	public Vector3Serializer boxedPackagePos;

	public QuaternionSerializer boxedPackageRot;

	public EObjectType objectType;
}
