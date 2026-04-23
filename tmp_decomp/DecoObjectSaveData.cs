using System;

[Serializable]
public class DecoObjectSaveData
{
	public bool isVerticalSnapToWarehouseWall;

	public int verticalSnapWallIndex;

	public bool isMovingObject;

	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public EDecoObject decoObjectType;
}
