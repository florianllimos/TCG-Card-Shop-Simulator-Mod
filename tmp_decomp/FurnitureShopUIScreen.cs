using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureShopUIScreen : GenericSliderScreen
{
	public List<FurnitureShopPanelUI> m_FurnitureShopPanelUIList;

	public FurnitureShopConfirmPurchaseScreen m_ConfirmPurchaseScreen;

	protected override void Init()
	{
		for (int i = 0; i < m_FurnitureShopPanelUIList.Count; i++)
		{
			m_FurnitureShopPanelUIList[i].SetActive(isActive: false);
		}
		int count = CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_FurniturePurchaseDataList.Count;
		for (int j = 0; j < count; j++)
		{
			m_FurnitureShopPanelUIList[j].Init(this, j);
			m_FurnitureShopPanelUIList[j].SetActive(isActive: true);
			m_ScrollEndPosParent = m_FurnitureShopPanelUIList[j].gameObject;
		}
		base.Init();
	}

	protected override void OnOpenScreen()
	{
		Init();
		base.OnOpenScreen();
	}

	public void OnPressPanelUIButton(int index)
	{
		m_ConfirmPurchaseScreen.UpdateData(this, index);
		OpenChildScreen(m_ConfirmPurchaseScreen);
	}

	private void SpawnPackageItemBox(EObjectType objectType)
	{
		Transform randomPackageSpawnPos = RestockManager.GetRandomPackageSpawnPos();
		ShelfManager.SpawnInteractableObjectInPackageBox(objectType, randomPackageSpawnPos.position, randomPackageSpawnPos.rotation);
	}

	public void EvaluateCartCheckout(float totalCost, int index)
	{
		CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(totalCost));
		FurniturePurchaseData furniturePurchaseData = InventoryBase.GetFurniturePurchaseData(index);
		SpawnPackageItemBox(furniturePurchaseData.objectType);
		PriceChangeManager.AddTransaction(0f - totalCost, ETransactionType.BuyFurniture, (int)furniturePurchaseData.objectType);
		StartCoroutine(DelaySaveShelfData());
		CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Mathf.Clamp(Mathf.RoundToInt(totalCost / 100f), 5, 100)));
		CPlayerData.m_GameReportDataCollect.upgradeCost -= totalCost;
		CPlayerData.m_GameReportDataCollectPermanent.upgradeCost -= totalCost;
		SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
	}

	private IEnumerator DelaySaveShelfData()
	{
		yield return new WaitForSeconds(1f);
		CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
	}
}
