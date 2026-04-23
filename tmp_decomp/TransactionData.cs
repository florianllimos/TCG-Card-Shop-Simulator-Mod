using System;

[Serializable]
public class TransactionData
{
	public int day;

	public int hourMinute;

	public int index;

	public int amount;

	public ETransactionType transactionType;

	public float moneyChangeAmount;

	public CardData cardData;

	public void SetData(float mMoneyChangeAmount, ETransactionType mTransactionType, int mIndex, int mAmount = 0, CardData mCardData = null)
	{
		day = CPlayerData.m_CurrentDay;
		hourMinute = LightManager.GetTimeHour() * 60 + LightManager.GetTimeMinute();
		transactionType = mTransactionType;
		index = mIndex;
		amount = mAmount;
		moneyChangeAmount = mMoneyChangeAmount;
		cardData = mCardData;
	}
}
