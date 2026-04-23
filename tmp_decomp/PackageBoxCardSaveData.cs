using System;
using System.Collections.Generic;

[Serializable]
public class PackageBoxCardSaveData
{
	public List<CardData> cardDataList;

	public Vector3Serializer pos;

	public QuaternionSerializer rot;
}
