using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MarketPrice
{
	public float generatedMarketPrice;

	public float pricePercentChangeList;

	public List<float> pastPricePercentChangeList;

	public float GetMarketPrice(int index, int cardGrade)
	{
		if (cardGrade > 0)
		{
			int index2 = (index * 10 + (cardGrade - 1)) % CPlayerData.m_GenGradedCardPriceMultiplierList.Count;
			float num = 0f;
			if (cardGrade >= 10)
			{
				num = CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * 12f;
			}
			else
			{
				switch (cardGrade)
				{
				case 9:
					num = CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * 8f;
					break;
				case 8:
					num = CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * 4f;
					break;
				case 7:
					num = CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * 2f;
					break;
				}
			}
			return (float)Mathf.RoundToInt(CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * generatedMarketPrice * (1f + pricePercentChangeList / 100f) * 100f) / 100f + num;
		}
		return (float)Mathf.RoundToInt(generatedMarketPrice * (1f + pricePercentChangeList / 100f) * 100f) / 100f;
	}

	public float GetMarketPriceCustomPercent(float percent, int index, int cardGrade)
	{
		if (cardGrade > 0)
		{
			int index2 = (index * 10 + (cardGrade - 1)) % CPlayerData.m_GenGradedCardPriceMultiplierList.Count;
			float num = 0f;
			if (cardGrade >= 10)
			{
				num = CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * 12f;
			}
			else
			{
				switch (cardGrade)
				{
				case 9:
					num = CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * 8f;
					break;
				case 8:
					num = CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * 4f;
					break;
				case 7:
					num = CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * 2f;
					break;
				}
			}
			return (float)Mathf.RoundToInt(CPlayerData.m_GenGradedCardPriceMultiplierList[index2] * generatedMarketPrice * (1f + percent / 100f) * 100f) / 100f + num;
		}
		return (float)Mathf.RoundToInt(generatedMarketPrice * (1f + percent / 100f) * 100f) / 100f;
	}
}
