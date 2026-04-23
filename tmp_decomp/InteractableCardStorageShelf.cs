using System.Collections.Generic;
using UnityEngine;

public class InteractableCardStorageShelf : InteractableObject
{
	public List<Transform> m_CustomerStandLocList;

	public GameObject m_NotForSaleMesh;

	private bool m_IsUIOpen;

	private List<CompactCardDataAmount> m_CompactCardDataAmountList = new List<CompactCardDataAmount>();

	private int m_BoxTotalCardCountMax = 10000;

	private bool m_CanWorkerTake = true;

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitCardStorageShelf(this);
	}

	public override void OnRaycasted()
	{
		if (!m_IsUIOpen)
		{
			base.OnRaycasted();
		}
	}

	public override void OnMouseButtonUp()
	{
		if (!m_IsUIOpen)
		{
			m_IsUIOpen = true;
			OnRaycastEnded();
			CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: true);
			CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
			CSingleton<InteractionPlayerController>.Instance.m_BulkDonationBoxUIScreen.OpenCardStorageShelfScreen(this);
		}
	}

	public override void OnRightMouseButtonUp()
	{
		CSingleton<InteractionPlayerController>.Instance.OpenCardStorageShelfSettingScreen(this);
	}

	public void OnCloseBulkDonationBoxUIScreen()
	{
		m_IsUIOpen = false;
		OnRaycasted();
	}

	public InteractableCard3d GetRandomCard()
	{
		if (m_IsUIOpen)
		{
			return null;
		}
		if (m_CompactCardDataAmountList.Count == 0)
		{
			return null;
		}
		CompactCardDataAmount compactCardDataAmount = null;
		int index = Random.Range(0, m_CompactCardDataAmountList.Count);
		if (m_CompactCardDataAmountList[index].gradedCardIndex > 0)
		{
			compactCardDataAmount = m_CompactCardDataAmountList[index];
			m_CompactCardDataAmountList.RemoveAt(index);
		}
		else
		{
			compactCardDataAmount = m_CompactCardDataAmountList[index];
			m_CompactCardDataAmountList[index].amount--;
			if (m_CompactCardDataAmountList[index].amount <= 0)
			{
				m_CompactCardDataAmountList.RemoveAt(index);
			}
		}
		CardData cardData = null;
		cardData = ((compactCardDataAmount.gradedCardIndex <= 0) ? CPlayerData.GetCardData(compactCardDataAmount.cardSaveIndex, compactCardDataAmount.expansionType, compactCardDataAmount.isDestiny) : CPlayerData.GetGradedCardData(compactCardDataAmount));
		Card3dUIGroup cardUI = CSingleton<Card3dUISpawner>.Instance.GetCardUI();
		InteractableCard3d component = ShelfManager.SpawnInteractableObject(EObjectType.Card3d).GetComponent<InteractableCard3d>();
		cardUI.m_IgnoreCulling = true;
		cardUI.m_CardUIAnimGrp.gameObject.SetActive(value: true);
		cardUI.SetSimplifyCardDistanceCull(isCull: false);
		cardUI.m_CardUI.SetFoilCullListVisibility(isActive: true);
		cardUI.m_CardUI.ResetFarDistanceCull();
		cardUI.m_CardUI.SetFoilMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilMaterialWorldView);
		cardUI.m_CardUI.SetFoilBlendedMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilBlendedMaterialWorldView);
		cardUI.m_CardUI.SetCardUI(cardData);
		cardUI.transform.position = base.transform.position;
		cardUI.transform.rotation = base.transform.rotation;
		cardUI.m_IgnoreCulling = false;
		component.transform.position = base.transform.position;
		component.transform.rotation = base.transform.rotation;
		component.SetCardUIFollow(cardUI);
		component.SetEnableCollision(isEnable: false);
		return component;
	}

	public int RemoveRandomCardFromShelf()
	{
		int num = 0;
		int num2 = Random.Range(2, 20);
		int num3 = m_CompactCardDataAmountList.Count - 1;
		while (num3 >= 0 && num2 > 0)
		{
			int num4 = Mathf.Clamp(Random.Range(0, 5), 0, m_CompactCardDataAmountList[num3].amount);
			m_CompactCardDataAmountList[num3].amount -= num4;
			if (m_CompactCardDataAmountList[num3].amount <= 0)
			{
				m_CompactCardDataAmountList.RemoveAt(num3);
			}
			num2 -= num4;
			num += num4;
			num3--;
		}
		return num;
	}

	public override void OnDestroyed()
	{
		ShelfManager.RemoveCardStorageShelf(this);
		base.OnDestroyed();
	}

	public int GetBoxTotalCardCountMax()
	{
		return m_BoxTotalCardCountMax;
	}

	public List<CompactCardDataAmount> GetCompactCardDataAmountList()
	{
		return m_CompactCardDataAmountList;
	}

	public void SetCompactCardDataAmountList(List<CompactCardDataAmount> compactCardDataAmountList)
	{
		m_CompactCardDataAmountList = compactCardDataAmountList;
	}

	public int GetTotalCardAmount()
	{
		int num = 0;
		for (int i = 0; i < m_CompactCardDataAmountList.Count; i++)
		{
			num += m_CompactCardDataAmountList[i].amount;
		}
		return num;
	}

	public bool IsEditingBulkBox()
	{
		return m_IsUIOpen;
	}

	public void SetCanWorkerTake(bool canWorkerTake)
	{
		m_CanWorkerTake = canWorkerTake;
	}

	public bool CanWorkerTake()
	{
		return m_CanWorkerTake;
	}

	public void OnCardStorageShelfSettingDone()
	{
		m_NotForSaleMesh.SetActive(!m_CanWorkerTake);
	}

	public void LoadData(CardStorageShelfSaveData saveData)
	{
		m_CompactCardDataAmountList = saveData.compactCardDataAmountList;
		m_CanWorkerTake = !saveData.isDisableWorkerTake;
		m_NotForSaleMesh.SetActive(!m_CanWorkerTake);
	}
}
