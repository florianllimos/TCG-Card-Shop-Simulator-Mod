using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScannerRestockScreen : UIScreenBase
{
	public GameObject m_UnlockedGrp;

	public GameObject m_LockedGrp;

	public Animation m_UnlockScreenAnim;

	public List<GameObject> m_LockedDisableList;

	public List<RestockCheckoutItemBar> m_RestockCheckoutItemBarUIList;

	public TextMeshProUGUI m_PageText;

	public TextMeshProUGUI m_SubtotalText;

	public TextMeshProUGUI m_DeliveryFeeText;

	public TextMeshProUGUI m_TotalPriceText;

	public TextMeshProUGUI m_UnlockCostText;

	public Button m_NextPageBtn;

	public Button m_PreviousPageBtn;

	private int m_PageIndex;

	private int m_PageMaxIndex;

	private int m_MaxUICountPerPage = 8;

	private float m_UnlockCost = 12000f;

	private float m_TotalCost;

	private float m_DeliveryFee;

	private List<int> m_RestockIndexList = new List<int>();

	private List<int> m_RestockBoxCountList = new List<int>();

	private List<EItemType> m_RestockItemTypeList = new List<EItemType>();

	private List<int> m_RestockTotalItemCountList = new List<int>();

	protected override void Awake()
	{
		base.Awake();
		for (int i = 0; i < m_RestockCheckoutItemBarUIList.Count; i++)
		{
			m_RestockCheckoutItemBarUIList[i].InitScannerRestock(this);
		}
		m_UnlockedGrp.SetActive(value: true);
		m_LockedGrp.SetActive(value: false);
	}

	protected override void OnOpenScreen()
	{
		base.OnOpenScreen();
		if (!CPlayerData.m_IsScannerRestockUnlocked)
		{
			m_UnlockCostText.text = GameInstance.GetPriceString(m_UnlockCost);
			m_UnlockedGrp.SetActive(value: false);
			m_LockedGrp.SetActive(value: true);
			for (int i = 0; i < m_LockedDisableList.Count; i++)
			{
				m_LockedDisableList[i].SetActive(value: false);
			}
			return;
		}
		m_UnlockedGrp.SetActive(value: true);
		m_LockedGrp.SetActive(value: false);
		EvaluatePanelUIPage(m_PageIndex);
		UpdateTotalCostAndBoxCount();
		if (m_RestockIndexList.Count <= 0)
		{
			CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
			CSingleton<InteractionPlayerController>.Instance.ExitLockMoveMode();
			CSingleton<InteractionPlayerController>.Instance.OnEnterScannerRestockMode();
			GameUIScreen.SetGameUIVisible(isVisible: true);
		}
	}

	protected override void OnCloseScreen()
	{
		base.OnCloseScreen();
	}

	public void OnPressUnlockButton()
	{
		if (CPlayerData.m_IsScannerRestockUnlocked)
		{
			return;
		}
		if (CPlayerData.m_CoinAmountDouble >= (double)m_UnlockCost)
		{
			for (int i = 0; i < m_LockedDisableList.Count; i++)
			{
				m_LockedDisableList[i].SetActive(value: true);
			}
			EvaluatePanelUIPage(m_PageIndex);
			UpdateTotalCostAndBoxCount();
			CPlayerData.m_IsScannerRestockUnlocked = true;
			CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(m_UnlockCost));
			CPlayerData.m_GameReportDataCollect.upgradeCost -= m_UnlockCost;
			CPlayerData.m_GameReportDataCollectPermanent.upgradeCost -= m_UnlockCost;
			PriceChangeManager.AddTransaction(0f - m_UnlockCost, ETransactionType.PayScannerUnlock, 0);
			SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
			m_UnlockScreenAnim.Play();
			CEventManager.QueueEvent(new CEventPlayer_ScannerRestockUnlocked());
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			SoundManager.GenericCancel();
		}
	}

	public void OnPressStartScanButton()
	{
		if (CPlayerData.m_IsScannerRestockUnlocked)
		{
			CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
			CSingleton<InteractionPlayerController>.Instance.ExitLockMoveMode();
			CSingleton<InteractionPlayerController>.Instance.OnEnterScannerRestockMode();
			GameUIScreen.SetGameUIVisible(isVisible: true);
		}
	}

	public void OnPressNextPage()
	{
		if (CPlayerData.m_IsScannerRestockUnlocked && m_PageIndex < m_PageMaxIndex)
		{
			m_PageIndex++;
			EvaluatePanelUIPage(m_PageIndex);
		}
	}

	public void OnPressPreviousPage()
	{
		if (CPlayerData.m_IsScannerRestockUnlocked && m_PageIndex > 0)
		{
			m_PageIndex--;
			EvaluatePanelUIPage(m_PageIndex);
		}
	}

	private void EvaluatePanelUIPage(int pageIndex)
	{
		m_PageMaxIndex = (m_RestockIndexList.Count - 1) / m_MaxUICountPerPage;
		m_PageText.text = m_PageIndex + 1 + " / " + (m_PageMaxIndex + 1);
		if (m_PageIndex == 0)
		{
			m_PreviousPageBtn.interactable = false;
			m_NextPageBtn.interactable = m_PageIndex < m_PageMaxIndex;
		}
		else if (m_PageIndex >= m_PageMaxIndex)
		{
			m_PreviousPageBtn.interactable = true;
			m_NextPageBtn.interactable = false;
		}
		else
		{
			m_PreviousPageBtn.interactable = true;
			m_NextPageBtn.interactable = true;
		}
		for (int i = 0; i < m_RestockCheckoutItemBarUIList.Count; i++)
		{
			int num = m_PageIndex * m_MaxUICountPerPage + i;
			if (num >= m_RestockIndexList.Count)
			{
				m_RestockCheckoutItemBarUIList[i].SetActive(isActive: false);
				continue;
			}
			m_RestockCheckoutItemBarUIList[i].UpdateData(m_RestockIndexList[num], m_RestockBoxCountList[num]);
			m_RestockCheckoutItemBarUIList[i].SetActive(isActive: true);
		}
	}

	private void HighlightUIBar(int itemListIndex, bool isRemoving)
	{
		if (!isRemoving)
		{
			int index = itemListIndex % m_MaxUICountPerPage;
			m_RestockCheckoutItemBarUIList[index].HighlightUIBar();
		}
	}

	private void UpdateTotalCostAndBoxCount()
	{
		int num = 0;
		float num2 = 0f;
		m_RestockItemTypeList.Clear();
		m_RestockTotalItemCountList.Clear();
		for (int i = 0; i < m_RestockBoxCountList.Count; i++)
		{
			RestockData restockData = InventoryBase.GetRestockData(m_RestockIndexList[i]);
			InventoryBase.GetItemData(restockData.itemType);
			int maxItemCountInBox = RestockManager.GetMaxItemCountInBox(restockData.itemType, restockData.isBigBox);
			int num3 = m_RestockBoxCountList[i] * maxItemCountInBox;
			float itemCost = CPlayerData.GetItemCost(restockData.itemType);
			num2 += itemCost * (float)num3;
			num += m_RestockBoxCountList[i];
			m_RestockItemTypeList.Add(restockData.itemType);
			m_RestockTotalItemCountList.Add(num3);
		}
		m_DeliveryFee = Mathf.Clamp(10 * num / 5, 5, 1000);
		if (num == 0)
		{
			m_DeliveryFee = 0f;
		}
		m_TotalCost = num2 + m_DeliveryFee;
		m_SubtotalText.text = GameInstance.GetPriceString(num2);
		m_DeliveryFeeText.text = GameInstance.GetPriceString(m_DeliveryFee);
		m_TotalPriceText.text = GameInstance.GetPriceString(m_TotalCost);
	}

	public void OnPressCheckoutButton()
	{
		if (m_RestockIndexList.Count <= 0)
		{
			return;
		}
		if (CPlayerData.m_CoinAmountDouble >= (double)m_TotalCost)
		{
			EvaluateCartCheckout(m_TotalCost);
			for (int i = 0; i < m_RestockBoxCountList.Count; i++)
			{
				RestockData restockData = InventoryBase.GetRestockData(m_RestockIndexList[i]);
				InventoryBase.GetItemData(restockData.itemType);
				int maxItemCountInBox = RestockManager.GetMaxItemCountInBox(restockData.itemType, restockData.isBigBox);
				int num = m_RestockBoxCountList[i] * maxItemCountInBox;
				PriceChangeManager.AddTransaction(0f - CPlayerData.GetItemCost(restockData.itemType) * (float)num, ETransactionType.Restock, (int)restockData.itemType, num);
			}
			PriceChangeManager.AddTransaction(0f - m_DeliveryFee, ETransactionType.RestockDeliveryFee, 0);
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			SoundManager.GenericCancel();
		}
	}

	public void AddToCartForCheckout(int index, int boxCount)
	{
		if (index >= 0)
		{
			int num = -1;
			if (m_RestockIndexList.Contains(index))
			{
				num = m_RestockIndexList.IndexOf(index);
				m_RestockBoxCountList[num] += boxCount;
			}
			else
			{
				m_RestockIndexList.Add(index);
				m_RestockBoxCountList.Add(boxCount);
				num = m_RestockIndexList.Count - 1;
			}
			m_PageMaxIndex = (m_RestockIndexList.Count - 1) / m_MaxUICountPerPage;
			m_PageIndex = num / m_MaxUICountPerPage;
			EvaluatePanelUIPage(m_PageIndex);
			HighlightUIBar(num, isRemoving: false);
			UpdateTotalCostAndBoxCount();
			SoundManager.PlayAudio("SFX_CheckoutScan", 0.05f, 1.1f);
		}
	}

	public void RemoveFromCartForCheckout(int index, int boxCount)
	{
		if (index < 0)
		{
			return;
		}
		bool isRemoving = false;
		int num = -1;
		if (m_RestockIndexList.Contains(index))
		{
			num = m_RestockIndexList.IndexOf(index);
			m_RestockBoxCountList[num] -= boxCount;
			if (m_RestockBoxCountList[num] <= 0)
			{
				m_RestockIndexList.RemoveAt(num);
				m_RestockBoxCountList.RemoveAt(num);
				isRemoving = true;
			}
			m_PageMaxIndex = (m_RestockIndexList.Count - 1) / m_MaxUICountPerPage;
			m_PageIndex = num / m_MaxUICountPerPage;
			if (m_PageIndex > m_PageMaxIndex)
			{
				m_PageIndex = m_PageMaxIndex;
			}
			EvaluatePanelUIPage(m_PageIndex);
			HighlightUIBar(num, isRemoving);
			UpdateTotalCostAndBoxCount();
			SoundManager.PlayAudio("SFX_CheckoutScan", 0.03f, 0.75f);
		}
	}

	public void EvaluateCartCheckout(float totalCost)
	{
		CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(totalCost));
		CPlayerData.m_GameReportDataCollect.supplyCost -= totalCost;
		CPlayerData.m_GameReportDataCollectPermanent.supplyCost -= totalCost;
		int num = 0;
		for (int i = 0; i < m_RestockIndexList.Count; i++)
		{
			RestockManager.SpawnPackageBoxItemMultipleFrame(m_RestockIndexList[i], m_RestockBoxCountList[i]);
			num += m_RestockBoxCountList[i];
		}
		m_RestockIndexList.Clear();
		m_RestockBoxCountList.Clear();
		m_PageIndex = 0;
		EvaluatePanelUIPage(m_PageIndex);
		UpdateTotalCostAndBoxCount();
		StartCoroutine(DelaySaveShelfData());
		CEventManager.QueueEvent(new CEventPlayer_AddShopExp(num * 5));
		TutorialManager.AddTaskValue(ETutorialTaskCondition.RestockItem, num);
		SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
	}

	private IEnumerator DelaySaveShelfData()
	{
		yield return new WaitForSeconds(1f);
		CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
	}

	public int GetItemCountInCart(EItemType itemType)
	{
		int num = 0;
		for (int i = 0; i < m_RestockItemTypeList.Count; i++)
		{
			if (m_RestockItemTypeList[i] == itemType)
			{
				num += m_RestockTotalItemCountList[i];
			}
		}
		return num;
	}
}
