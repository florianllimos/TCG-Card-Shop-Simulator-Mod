using System;
using System.Collections.Generic;

[Serializable]
public class DeckCompactCardDataList
{
	public string deckName;

	public int deckBoxIndex;

	public int playmatIndex;

	public List<CompactCardDataAmount> compactCardDataAmountList = new List<CompactCardDataAmount>();

	public int GetTotalCardCount()
	{
		int num = 0;
		for (int i = 0; i < compactCardDataAmountList.Count; i++)
		{
			num += compactCardDataAmountList[i].amount;
		}
		return num;
	}
}
