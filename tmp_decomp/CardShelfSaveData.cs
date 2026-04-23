using System;
using System.Collections.Generic;

[Serializable]
public class CardShelfSaveData
{
	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public bool isBoxed;

	public Vector3Serializer boxedPackagePos;

	public QuaternionSerializer boxedPackageRot;

	public EObjectType objectType;

	public List<CardData> cardDataList;

	public bool isVerticalSnapToWarehouseWall;

	public int verticalSnapWallIndex;
}
