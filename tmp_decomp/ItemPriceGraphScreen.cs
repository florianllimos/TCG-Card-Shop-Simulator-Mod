using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPriceGraphScreen : UIScreenBase
{
	public UIScreenBase m_ChangeGradeOptionUIScreen;

	public GameObject m_ItemGrp;

	public GameObject m_CardGrp;

	public GameObject m_ValueGrp;

	public Transform m_ValueAnimGrp;

	public Image m_ItemImage;

	public TextMeshProUGUI m_ItemName;

	public TextMeshProUGUI m_ValueText;

	public CardUI m_CardUI;

	public TextMeshProUGUI m_CardName;

	public TextMeshProUGUI m_CardGradeName;

	public Transform m_GraphStartXY;

	public Transform m_GraphEndXY;

	public float m_LineGraphLerpSpeed = 60f;

	public float m_LineGraphDivide = 8f;

	public RectTransform m_GraphStartCalculatePos;

	public RectTransform m_GraphEndCalculatePos;

	public Color m_PositiveColor;

	public Color m_NegativeColor;

	public Color m_NeutralColor;

	public List<ItemPriceLineGrp> m_ItemPriceLineGrpList;

	public List<Transform> m_DotSpriteList = new List<Transform>();

	public List<TextMeshProUGUI> m_PriceTextList;

	public List<TextMeshProUGUI> m_DayTextList;

	public List<TextMeshProUGUI> m_QuickViewGradedCardPriceTextList;

	public List<ControllerButton> m_ChangeGradeBtnList;

	private bool m_CanRunUpdate;

	private float m_GraphDimensionX;

	private float m_GraphDimensionY;

	private int m_DistributionCountX = 30;

	private int m_DistributionCountY = 10;

	public int m_CurrentScaleLineIndex;

	private int m_LastIndex;

	private float m_LowestValue;

	private float m_HighestValue;

	private float m_Distribution;

	private CompactCardDataAmount m_CurrentCompactCardData;

	private List<float> m_GraphValueList = new List<float>();

	private List<float> m_ValuePercentList = new List<float>();

	public void ShowItemPriceChart(EItemType itemType)
	{
		m_CanRunUpdate = true;
		m_CurrentScaleLineIndex = 0;
		List<float> floatDataList = CPlayerData.m_ItemPricePercentPastChangeList[(int)itemType].floatDataList;
		List<float> list = new List<float>();
		for (int i = 0; i < floatDataList.Count; i++)
		{
			list.Add(CPlayerData.GetItemMarketPriceCustomPercent(itemType, floatDataList[i]));
		}
		EvaluatePriceChart(list);
		ItemData itemData = InventoryBase.GetItemData(itemType);
		m_ItemImage.sprite = itemData.icon;
		m_ItemName.text = itemData.GetName();
		m_ValueText.text = GameInstance.GetPriceString(list[list.Count - 1]);
		m_ItemGrp.SetActive(value: true);
		m_CardGrp.SetActive(value: false);
		m_ValueGrp.SetActive(value: false);
		if (itemData.isHideItemUntilUnlocked)
		{
			int restockDataIndex = InventoryBase.GetRestockDataIndex(itemType);
			bool flag = false;
			if (restockDataIndex > 0)
			{
				flag = CPlayerData.GetIsItemLicenseUnlocked(restockDataIndex);
			}
			if (!flag)
			{
				m_ItemImage.sprite = InventoryBase.GetQuestionMarkSprite();
				m_ItemName.text = "???";
			}
		}
	}

	public void ShowCardPriceChart(int cardIndex, ECardExpansionType expansionType, bool isDestiny, int cardGrade)
	{
		m_CanRunUpdate = true;
		m_CurrentScaleLineIndex = 0;
		List<float> pastCardPricePercentChange = CPlayerData.GetPastCardPricePercentChange(cardIndex, expansionType, isDestiny);
		List<float> list = new List<float>();
		for (int i = 0; i < pastCardPricePercentChange.Count; i++)
		{
			list.Add(CPlayerData.GetCardMarketPriceCustomPercent(cardIndex, expansionType, isDestiny, pastCardPricePercentChange[i], cardGrade));
		}
		m_ValueText.text = GameInstance.GetPriceString(list[list.Count - 1]);
		EvaluatePriceChart(list);
		CardData cardData = new CardData();
		cardData.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(cardIndex, expansionType);
		cardData.isFoil = cardIndex % CPlayerData.GetCardAmountPerMonsterType(expansionType) >= CPlayerData.GetCardAmountPerMonsterType(expansionType, includeFoilCount: false);
		cardData.borderType = (ECardBorderType)(cardIndex % CPlayerData.GetCardAmountPerMonsterType(expansionType, includeFoilCount: false));
		cardData.isDestiny = isDestiny;
		cardData.expansionType = expansionType;
		cardData.cardGrade = cardGrade;
		m_CardName.text = InventoryBase.GetMonsterData(cardData.monsterType).GetName() + " - " + CPlayerData.GetFullCardTypeName(cardData);
		if (cardGrade == 0)
		{
			m_CardGradeName.text = LocalizationManager.GetTranslation("Ungraded");
		}
		else
		{
			m_CardGradeName.text = "TCG " + cardGrade;
		}
		m_CurrentCompactCardData = new CompactCardDataAmount();
		m_CurrentCompactCardData.cardSaveIndex = cardIndex;
		m_CurrentCompactCardData.expansionType = expansionType;
		m_CurrentCompactCardData.isDestiny = isDestiny;
		m_CurrentCompactCardData.amount = cardGrade;
		m_CardUI.SetCardUI(cardData);
		m_ItemGrp.SetActive(value: false);
		m_CardGrp.SetActive(value: true);
		m_ValueGrp.SetActive(value: false);
		m_QuickViewGradedCardPriceTextList[0].text = GameInstance.GetPriceString(CPlayerData.GetCardMarketPrice(cardIndex, expansionType, isDestiny, 0));
		m_QuickViewGradedCardPriceTextList[1].text = GameInstance.GetPriceString(CPlayerData.GetCardMarketPrice(cardIndex, expansionType, isDestiny, 7));
		m_QuickViewGradedCardPriceTextList[2].text = GameInstance.GetPriceString(CPlayerData.GetCardMarketPrice(cardIndex, expansionType, isDestiny, 8));
		m_QuickViewGradedCardPriceTextList[3].text = GameInstance.GetPriceString(CPlayerData.GetCardMarketPrice(cardIndex, expansionType, isDestiny, 9));
		m_QuickViewGradedCardPriceTextList[4].text = GameInstance.GetPriceString(CPlayerData.GetCardMarketPrice(cardIndex, expansionType, isDestiny, 10));
	}

	private void EvaluatePriceChart(List<float> dataList)
	{
		m_GraphDimensionX = m_GraphEndXY.transform.position.x - m_GraphStartXY.transform.position.x;
		m_GraphDimensionY = m_GraphEndXY.transform.position.y - m_GraphStartXY.transform.position.y;
		m_HighestValue = 0f;
		m_LowestValue = 1000000f;
		for (int i = 0; i < dataList.Count; i++)
		{
			if (dataList[i] > m_HighestValue)
			{
				m_HighestValue = dataList[i];
			}
			if (dataList[i] < m_LowestValue)
			{
				m_LowestValue = dataList[i];
			}
		}
		float min = 0.01f;
		if (m_HighestValue > 100000f)
		{
			min = 5000f;
		}
		else if (m_HighestValue > 50000f)
		{
			min = 2000f;
		}
		else if (m_HighestValue > 20000f)
		{
			min = 1000f;
		}
		else if (m_HighestValue > 18000f)
		{
			min = 900f;
		}
		else if (m_HighestValue > 16000f)
		{
			min = 800f;
		}
		else if (m_HighestValue > 14000f)
		{
			min = 700f;
		}
		else if (m_HighestValue > 12000f)
		{
			min = 600f;
		}
		else if (m_HighestValue > 10000f)
		{
			min = 500f;
		}
		else if (m_HighestValue > 8000f)
		{
			min = 400f;
		}
		else if (m_HighestValue > 6000f)
		{
			min = 300f;
		}
		else if (m_HighestValue > 4000f)
		{
			min = 200f;
		}
		else if (m_HighestValue > 2000f)
		{
			min = 100f;
		}
		else if (m_HighestValue > 1000f)
		{
			min = 50f;
		}
		else if (m_HighestValue > 500f)
		{
			min = 25f;
		}
		else if (m_HighestValue > 100f)
		{
			min = 5f;
		}
		else if (m_HighestValue > 80f)
		{
			min = 4f;
		}
		else if (m_HighestValue > 60f)
		{
			min = 3f;
		}
		else if (m_HighestValue > 50f)
		{
			min = 2.5f;
		}
		else if (m_HighestValue > 40f)
		{
			min = 2f;
		}
		else if (m_HighestValue > 30f)
		{
			min = 1.5f;
		}
		else if (m_HighestValue > 20f)
		{
			min = 1f;
		}
		else if (m_HighestValue > 10f)
		{
			min = 0.5f;
		}
		else if (m_HighestValue > 5f)
		{
			min = 0.2f;
		}
		else if (m_HighestValue > 2f)
		{
			min = 0.1f;
		}
		else if (m_HighestValue > 1f)
		{
			min = 0.05f;
		}
		else if (m_HighestValue > 0.5f)
		{
			min = 0.02f;
		}
		m_Distribution = Mathf.Clamp((m_HighestValue - m_LowestValue) / (float)m_DistributionCountY, min, 1000f);
		m_HighestValue = m_LowestValue + m_Distribution * (float)m_DistributionCountY;
		if (m_LowestValue > 0.02f)
		{
			m_LowestValue -= m_Distribution * 2f;
		}
		m_GraphValueList.Clear();
		for (int j = 0; j < m_DistributionCountY + 2; j++)
		{
			m_GraphValueList.Add(m_LowestValue + m_Distribution * (float)j);
			m_PriceTextList[m_PriceTextList.Count - j - 1].text = GameInstance.GetPriceString(m_LowestValue + m_Distribution * (float)j);
		}
		m_ValuePercentList.Clear();
		for (int k = 0; k < dataList.Count; k++)
		{
			float item = (dataList[k] - m_LowestValue) / (m_HighestValue - m_LowestValue);
			m_ValuePercentList.Add(item);
		}
		for (int l = 0; l < m_DotSpriteList.Count; l++)
		{
			m_DotSpriteList[l].gameObject.SetActive(value: false);
		}
		float num = m_GraphDimensionX / (float)m_DistributionCountX;
		Vector3 position = m_GraphStartXY.position;
		for (int m = 0; m < dataList.Count; m++)
		{
			position = m_GraphStartXY.position;
			position.x += num * (float)m;
			position.y += Mathf.Lerp(0f, m_GraphDimensionY, m_ValuePercentList[m]);
			m_DotSpriteList[m].position = position;
			m_LastIndex = m;
		}
		m_ValueGrp.transform.position = m_DotSpriteList[m_LastIndex].position;
		if (dataList.Count > 0)
		{
			m_DotSpriteList[0].gameObject.SetActive(value: true);
		}
		for (int n = 0; n < m_ItemPriceLineGrpList.Count - 1; n++)
		{
			m_ItemPriceLineGrpList[n].SetActive(isActive: false);
		}
		if (dataList.Count > 1)
		{
			for (int num2 = 0; num2 < dataList.Count - 1; num2++)
			{
				m_ItemPriceLineGrpList[num2].transform.position = m_DotSpriteList[num2].position;
				m_ItemPriceLineGrpList[num2].transform.LookAt(m_DotSpriteList[num2 + 1], Vector3.up);
				Vector3 localScale = m_ItemPriceLineGrpList[num2].transform.localScale;
				m_GraphStartCalculatePos.transform.position = m_DotSpriteList[num2 + 1].position;
				m_GraphEndCalculatePos.transform.position = m_DotSpriteList[num2].position;
				float num3 = Vector2.Distance(m_GraphStartCalculatePos.anchoredPosition, m_GraphEndCalculatePos.anchoredPosition);
				localScale.z = num3 / m_LineGraphDivide * (2f - base.transform.localScale.x);
				m_ItemPriceLineGrpList[num2].transform.localScale = localScale;
				float num4 = dataList[num2 + 1] - dataList[num2];
				if (num4 > 0.005f)
				{
					m_ItemPriceLineGrpList[num2].SetColor(m_PositiveColor);
				}
				else if (num4 < -0.005f)
				{
					m_ItemPriceLineGrpList[num2].SetColor(m_NegativeColor);
				}
				else
				{
					m_ItemPriceLineGrpList[num2].SetColor(m_NeutralColor);
				}
				m_ItemPriceLineGrpList[num2].SetScaleLerp(0f);
				m_ItemPriceLineGrpList[num2].SetActive(isActive: true);
			}
		}
		int num5 = CPlayerData.m_CurrentDay + 1;
		Mathf.Clamp(dataList.Count / 2, 1, 15);
		for (int num6 = 0; num6 < m_DayTextList.Count; num6++)
		{
			m_DayTextList[num6].enabled = false;
		}
		int num7 = 2;
		if (dataList.Count < 3)
		{
			num7 = 1;
		}
		for (int num8 = 0; num8 < dataList.Count; num8++)
		{
			Vector3 position2 = m_DayTextList[num8].transform.position;
			position2.x = m_DotSpriteList[num8].transform.position.x;
			m_DayTextList[num8].transform.position = position2;
			m_DayTextList[num8].text = (num5 - (dataList.Count - num8 - 1)).ToString();
			m_DayTextList[num8].enabled = num8 % num7 == 0 || num8 == 0 || num8 == dataList.Count - 1;
		}
	}

	protected override void RunUpdate()
	{
		if (!m_CanRunUpdate)
		{
			return;
		}
		for (int i = m_CurrentScaleLineIndex; i < m_ItemPriceLineGrpList.Count; i++)
		{
			if (!m_ItemPriceLineGrpList[i].m_IsActive)
			{
				continue;
			}
			if (m_ItemPriceLineGrpList[i].m_Lerp >= 1f)
			{
				m_DotSpriteList[i + 1].gameObject.SetActive(value: true);
				if (i + 1 >= m_LastIndex)
				{
					Vector3 localPosition = m_ValueAnimGrp.localPosition;
					if (m_LastIndex < 4)
					{
						localPosition.x = 75f;
					}
					else if (m_LastIndex >= 4 && m_LastIndex < 8)
					{
						localPosition.x = 45f;
					}
					else if (m_LastIndex >= 8 && m_LastIndex < 20)
					{
						localPosition.x = 0f;
					}
					else if (m_LastIndex >= 20 && m_LastIndex < 24)
					{
						localPosition.x = -45f;
					}
					else
					{
						localPosition.x = -75f;
					}
					m_ValueAnimGrp.localPosition = localPosition;
					m_ValueGrp.SetActive(value: true);
					m_CanRunUpdate = false;
				}
				continue;
			}
			m_ItemPriceLineGrpList[i].AddScaleLerp(Time.fixedDeltaTime * m_LineGraphLerpSpeed);
			m_CurrentScaleLineIndex = i;
			break;
		}
	}

	protected override void OnOpenScreen()
	{
		base.OnOpenScreen();
	}

	protected override void OnCloseScreen()
	{
		base.OnCloseScreen();
	}

	public void ShowChangeGradeScreen()
	{
		for (int i = 0; i < m_ChangeGradeBtnList.Count; i++)
		{
			m_ChangeGradeBtnList[i].SetBGHighlightVisibility(isVisible: false);
		}
		m_ChangeGradeBtnList[m_CurrentCompactCardData.amount].SetBGHighlightVisibility(isVisible: true);
		OpenChildScreen(m_ChangeGradeOptionUIScreen);
	}

	public void CloseChangeGradeScreen()
	{
		m_ChangeGradeOptionUIScreen.CloseScreen();
	}

	public void OnPressChangeCardGradeButton(int grade)
	{
		CPlayerData.m_LastSelectedPriceGraphCardGrade = grade;
		m_ChangeGradeOptionUIScreen.CloseScreen();
		m_ScreenGroup.SetActive(value: false);
		m_CardGrp.SetActive(value: false);
		ShowCardPriceChart(m_CurrentCompactCardData.cardSaveIndex, m_CurrentCompactCardData.expansionType, m_CurrentCompactCardData.isDestiny, grade);
		m_ScreenGroup.SetActive(value: true);
	}
}
