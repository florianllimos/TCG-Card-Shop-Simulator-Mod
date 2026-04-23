using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

public class ExpansionShopUIScreen : GenericSliderScreen
{
	public List<ExpansionShopPanelUI> m_ExpansionShopPanelUIList;

	public int m_ShopB_UnlockLevelRequired = 15;

	public float m_ShopB_UnlockPrice = 5000f;

	public TextMeshProUGUI m_ShopB_PriceText;

	public TextMeshProUGUI m_ShopB_LevelRequirementText;

	public GameObject m_ShopB_LockPurchaseBtn;

	public GameObject m_ShopB_UnlockGrp;

	public GameObject m_ShopB_UnlockGrpPrologue;

	public GameObject m_ShopA_HighlightBtn;

	public GameObject m_ShopB_HighlightBtn;

	private bool m_IsShopB;

	private string m_LevelRequirementString = "";

	protected override void Init()
	{
		EvaluateShopPanelUI();
		base.Init();
	}

	protected override void OnOpenScreen()
	{
		Init();
		base.OnOpenScreen();
	}

	private void EvaluateShopPanelUI()
	{
		for (int i = 0; i < m_ExpansionShopPanelUIList.Count; i++)
		{
			m_ExpansionShopPanelUIList[i].SetActive(isActive: false);
		}
		m_ShopA_HighlightBtn.SetActive(!m_IsShopB);
		m_ShopB_HighlightBtn.SetActive(m_IsShopB);
		if (m_IsShopB)
		{
			if (m_LevelRequirementString == "")
			{
				m_LevelRequirementString = m_ShopB_LevelRequirementText.text;
			}
			m_ShopB_PriceText.text = GameInstance.GetPriceString(m_ShopB_UnlockPrice);
			m_ShopB_LevelRequirementText.text = LocalizationManager.GetTranslation(m_LevelRequirementString).Replace("XXX", m_ShopB_UnlockLevelRequired.ToString());
			m_ShopB_UnlockGrp.SetActive(!CPlayerData.m_IsWarehouseRoomUnlocked);
			if (CSingleton<CGameManager>.Instance.m_IsPrologue)
			{
				m_ShopB_UnlockGrpPrologue.SetActive(value: true);
			}
			if (CPlayerData.m_ShopLevel + 1 >= m_ShopB_UnlockLevelRequired)
			{
				m_ShopB_LevelRequirementText.enabled = false;
				m_ShopB_LockPurchaseBtn.SetActive(value: false);
			}
			else
			{
				m_ShopB_LevelRequirementText.enabled = true;
				m_ShopB_LockPurchaseBtn.SetActive(value: true);
			}
			if (!CPlayerData.m_IsWarehouseRoomUnlocked)
			{
				return;
			}
		}
		else
		{
			m_ShopB_UnlockGrp.SetActive(value: false);
		}
		int num = 30;
		if (m_IsShopB)
		{
			num = 14;
		}
		for (int j = 0; j < num; j++)
		{
			m_ExpansionShopPanelUIList[j].Init(this, j, m_IsShopB);
			m_ExpansionShopPanelUIList[j].SetActive(isActive: true);
			m_ScrollEndPosParent = m_ExpansionShopPanelUIList[j].gameObject;
		}
	}

	public void OnPressPanelUIButton(int index)
	{
	}

	public void OnPressShopSubButton(bool isShopB)
	{
		m_IsShopB = isShopB;
		EvaluateShopPanelUI();
		StartCoroutine(EvaluateActiveRestockUIScroller());
		m_PosX = 0f;
		m_LerpPosX = 0f;
	}

	public void OnPressUnlockShopB()
	{
		if (!CSingleton<CGameManager>.Instance.m_IsPrologue)
		{
			if (CPlayerData.m_IsWarehouseRoomUnlocked)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AlreadyPurchased);
			}
			else if (CPlayerData.m_ShopLevel + 1 < m_ShopB_UnlockLevelRequired)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShopLevelNotEnough);
			}
			else if (CPlayerData.m_CoinAmountDouble >= (double)m_ShopB_UnlockPrice)
			{
				PriceChangeManager.AddTransaction(0f - m_ShopB_UnlockPrice, ETransactionType.ShopExpansion, 1, -1);
				CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(m_ShopB_UnlockPrice));
				CSingleton<UnlockRoomManager>.Instance.SetUnlockWarehouseRoom(isUnlocked: true);
				AchievementManager.OnShopLotBUnlocked();
				CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Mathf.Clamp(Mathf.RoundToInt(m_ShopB_UnlockPrice / 100f), 5, 100)));
				StartCoroutine(DelaySaveShelfData());
				m_ShopB_UnlockGrp.SetActive(value: false);
				OnPressShopSubButton(isShopB: true);
				CPlayerData.m_GameReportDataCollect.upgradeCost -= m_ShopB_UnlockPrice;
				CPlayerData.m_GameReportDataCollectPermanent.upgradeCost -= m_ShopB_UnlockPrice;
				SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			}
		}
	}

	public void EvaluateCartCheckout(float totalCost, int index, bool isShopB)
	{
		if (m_IsShopB == isShopB)
		{
			CPlayerData.GetUnlockShopRoomCost(CPlayerData.m_UnlockRoomCount);
			if (isShopB)
			{
				CPlayerData.GetUnlockWarehouseRoomCost(CPlayerData.m_UnlockWarehouseRoomCount);
			}
			if (isShopB)
			{
				PriceChangeManager.AddTransaction(0f - totalCost, ETransactionType.ShopExpansion, 0, index);
			}
			else
			{
				PriceChangeManager.AddTransaction(0f - totalCost, ETransactionType.ShopExpansion, 1, index);
			}
			CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(totalCost));
			if (isShopB)
			{
				CSingleton<UnlockRoomManager>.Instance.StartUnlockNextWarehouseRoom();
			}
			else
			{
				CSingleton<UnlockRoomManager>.Instance.StartUnlockNextRoom();
			}
			CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Mathf.Clamp(Mathf.RoundToInt(totalCost / 100f), 5, 100)));
			StartCoroutine(DelaySaveShelfData());
			for (int i = 0; i < m_ExpansionShopPanelUIList.Count; i++)
			{
				m_ExpansionShopPanelUIList[i].Init(this, i, m_IsShopB);
			}
			CPlayerData.m_GameReportDataCollect.upgradeCost -= totalCost;
			CPlayerData.m_GameReportDataCollectPermanent.upgradeCost -= totalCost;
			SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
		}
	}

	private IEnumerator DelaySaveShelfData()
	{
		yield return new WaitForSeconds(1f);
		CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
	}
}
