using System;

[Serializable]
public class PackageBoxItemaveData
{
	public ItemTypeAmount itemTypeAmount;

	public bool isBigBox;

	public bool isStored;

	public int storedWarehouseShelfIndex;

	public int storageCompartmentIndex;

	public Vector3Serializer pos;

	public QuaternionSerializer rot;
}
