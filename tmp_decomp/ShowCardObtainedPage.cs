using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowCardObtainedPage : UIScreenBase
{
	public ItemPriceGraphScreen m_ItemPriceGraphScreen;

	public List<CardAmountUIGroup> m_CardAmountUIGroupList;

	public GameObject m_ValueGrp;

	public TextMeshProUGUI m_ValueText;

	public TextMeshProUGUI m_PageText;

	public TextMeshProUGUI m_CurrentPageValueText;

	public TextMeshProUGUI m_TotalValueText;

	public Button m_NextButton;

	public Button m_PreviousButton;

	private int m_CurrentPageIndex;

	private int m_MaxPageIndex;

	private int m_TotalExpGained;

	private float m_CurrentPageValue;

	private float m_TotalValue;

	private float m_HighValueCardThreshold = 10f;

	private List<bool> m_IsNewCardList = new List<bool>();

	private List<bool> m_IsHighValueCardList = new List<bool>();

	private List<CompactCardDataAmount> m_CompactCardDataAmountList = new List<CompactCardDataAmount>();

	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < m_CardAmountUIGroupList.Count; i++)
		{
			m_CardAmountUIGroupList[i].InitShowCardObtainedPage(this, i);
		}
	}

	public void ShowCardObtained(List<CompactCardDataAmount> compactCardDataAmountList)
	{
		m_HighValueCardThreshold = 10 + CPlayerData.m_ShopLevel / 5 * 2;
		if (compactCardDataAmountList.Count <= 0)
		{
			Debug.Log("compactCardDataAmountList 0 list");
			return;
		}
		m_CompactCardDataAmountList.Clear();
		m_IsNewCardList.Clear();
		m_IsHighValueCardList.Clear();
		for (int i = 0; i < compactCardDataAmountList.Count; i++)
		{
			m_CompactCardDataAmountList.Add(compactCardDataAmountList[i]);
		}
		m_CurrentPageIndex = 0;
		m_MaxPageIndex = (m_CompactCardDataAmountList.Count - 1) / m_CardAmountUIGroupList.Count;
		m_TotalValue = 0f;
		m_TotalExpGained = 0;
		bool isGet = false;
		bool isGet2 = false;
		for (int j = 0; j < m_CompactCardDataAmountList.Count; j++)
		{
			CardData cardData = CPlayerData.GetCardData(m_CompactCardDataAmountList[j].cardSaveIndex, m_CompactCardDataAmountList[j].expansionType, m_CompactCardDataAmountList[j].isDestiny);
			m_IsNewCardList.Add(CPlayerData.GetCardAmount(cardData) == 0);
			m_IsHighValueCardList.Add(CPlayerData.GetCardMarketPrice(cardData) >= m_HighValueCardThreshold);
			m_TotalValue += CPlayerData.GetCardMarketPrice(cardData) * (float)m_CompactCardDataAmountList[j].amount;
			CPlayerData.AddCard(cardData, m_CompactCardDataAmountList[j].amount);
			int num = (int)(cardData.GetCardBorderType() + 1) * Mathf.CeilToInt((float)(cardData.borderType + 1) / 2f);
			if (cardData.isFoil)
			{
				num *= 8;
			}
			m_TotalExpGained += num / 2 * m_CompactCardDataAmountList[j].amount;
			if (cardData.GetCardBorderType() == ECardBorderType.FullArt && cardData.isFoil)
			{
				isGet = true;
				if (cardData.expansionType == ECardExpansionType.Ghost)
				{
					isGet2 = true;
				}
			}
		}
		m_TotalValueText.text = GameInstance.GetPriceString(m_TotalValue);
		EvaluateCardUI();
		OpenScreen();
		CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
		AchievementManager.OnGetFullArtFoil(isGet);
		AchievementManager.OnGetFullArtGhostFoil(isGet2);
		AchievementManager.OnCheckAlbumCardCount(CPlayerData.GetTotalCardCollectedAmount());
	}

	public void OnPressCardUI(int index)
	{
		int index2 = index + m_CurrentPageIndex * m_CardAmountUIGroupList.Count;
		m_ItemPriceGraphScreen.ShowCardPriceChart(m_CompactCardDataAmountList[index2].cardSaveIndex, m_CompactCardDataAmountList[index2].expansionType, m_CompactCardDataAmountList[index2].isDestiny, 0);
		m_ItemPriceGraphScreen.OpenScreen();
		SoundManager.GenericMenuOpen();
	}

	private void EvaluateCardUI()
	{
		m_CurrentPageValue = 0f;
		for (int i = 0; i < m_CardAmountUIGroupList.Count; i++)
		{
			m_CardAmountUIGroupList[i].SetActive(isActive: false);
			m_CardAmountUIGroupList[i].m_IsNewIcon.SetActive(value: false);
			m_CardAmountUIGroupList[i].m_IsHighValueIcon.SetActive(value: false);
		}
		int num = 0;
		for (int j = m_CurrentPageIndex * m_CardAmountUIGroupList.Count; j < m_CompactCardDataAmountList.Count; j++)
		{
			if (num >= m_CardAmountUIGroupList.Count)
			{
				break;
			}
			CardData cardData = CPlayerData.GetCardData(m_CompactCardDataAmountList[j].cardSaveIndex, m_CompactCardDataAmountList[j].expansionType, m_CompactCardDataAmountList[j].isDestiny);
			int amount = m_CompactCardDataAmountList[j].amount;
			m_CardAmountUIGroupList[num].m_CardUI.SetCardUI(cardData);
			m_CardAmountUIGroupList[num].m_AmountText.text = "X " + amount;
			m_CardAmountUIGroupList[num].SetActive(isActive: true);
			if (m_IsHighValueCardList[j])
			{
				m_CardAmountUIGroupList[num].m_IsHighValueIcon.SetActive(value: true);
			}
			else
			{
				m_CardAmountUIGroupList[num].m_IsHighValueIcon.SetActive(value: false);
				m_CardAmountUIGroupList[num].m_IsNewIcon.SetActive(m_IsNewCardList[j]);
			}
			m_CurrentPageValue += CPlayerData.GetCardMarketPrice(cardData) * (float)amount;
			num++;
		}
		m_CurrentPageValueText.text = GameInstance.GetPriceString(m_CurrentPageValue);
		m_PageText.text = m_CurrentPageIndex + 1 + " / " + (m_MaxPageIndex + 1);
		m_NextButton.interactable = m_CurrentPageIndex < m_MaxPageIndex;
		m_PreviousButton.interactable = m_CurrentPageIndex > 0;
	}

	protected override void OnOpenScreen()
	{
		m_ValueGrp.SetActive(value: false);
		SoundManager.GenericMenuOpen();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: true);
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		base.OnOpenScreen();
	}

	protected override void OnCloseScreen()
	{
		if (m_TotalExpGained > 0)
		{
			CEventManager.QueueEvent(new CEventPlayer_AddShopExp(m_TotalExpGained));
		}
		SoundManager.GenericMenuClose();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		base.OnCloseScreen();
		SoundManager.PlayAudio("SFX_AlbumOpen", 0.6f);
	}

	public void OnPressConfirmBtn()
	{
		SoundManager.GenericConfirm();
		CloseScreen();
	}

	public void OnPressCancelBtn()
	{
		SoundManager.GenericCancel();
		CloseScreen();
	}

	public void OnPressNextCardPage()
	{
		if (m_CurrentPageIndex < m_MaxPageIndex)
		{
			SoundManager.GenericConfirm();
			m_CurrentPageIndex++;
			EvaluateCardUI();
		}
	}

	public void OnPressPreviousCardPage()
	{
		if (m_CurrentPageIndex > 0)
		{
			SoundManager.GenericConfirm();
			m_CurrentPageIndex--;
			EvaluateCardUI();
		}
	}

	public void OnHoverEnterCardUI(int index)
	{
		m_ValueGrp.SetActive(value: true);
		m_ValueGrp.transform.position = m_CardAmountUIGroupList[index].transform.position;
		int index2 = index + m_CurrentPageIndex * m_CardAmountUIGroupList.Count;
		m_ValueText.text = GameInstance.GetPriceString(CPlayerData.GetCardMarketPrice(m_CompactCardDataAmountList[index2].cardSaveIndex, m_CompactCardDataAmountList[index2].expansionType, m_CompactCardDataAmountList[index2].isDestiny, 0));
	}

	public void OnHoverExitCardUI()
	{
		m_ValueGrp.SetActive(value: false);
	}
}
