using System;

[Serializable]
public class CashierCounterSaveData
{
	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public bool isBoxed;

	public Vector3Serializer boxedPackagePos;

	public QuaternionSerializer boxedPackageRot;

	public EObjectType objectType;

	public bool isDisableCheckout;

	public bool isDisableTrading;
}
