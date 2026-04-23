using System;
using System.Collections.Generic;

[Serializable]
public class PlayTableSaveData
{
	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public bool isBoxed;

	public Vector3Serializer boxedPackagePos;

	public QuaternionSerializer boxedPackageRot;

	public EObjectType objectType;

	public bool hasStartPlay;

	public List<bool> isSeatOccupied;

	public List<bool> isCustomerSmelly;

	public int currentPlayerCount;

	public float currentPlayTime;

	public float currentPlayTimeMax;

	public List<float> playTableFee;
}
