using System;
using System.Collections.Generic;

[Serializable]
public class WorkerSaveData
{
	public EWorkerTask primaryTask;

	public EWorkerTask secondaryTask;

	public EWorkerTask workerTask;

	public EWorkerState currentState;

	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public bool isFillShelfWithoutLabel;

	public bool isRoundUpPrice;

	public bool isRoundUpCardPrice;

	public bool isGoingHome;

	public bool isBonusBoosted;

	public int bonusBoostedCount;

	public float setPriceMultiplier;

	public float setCardPriceMultiplier;

	public List<bool> cardPackItemTypeEnabledList;

	public List<int> expList;
}
