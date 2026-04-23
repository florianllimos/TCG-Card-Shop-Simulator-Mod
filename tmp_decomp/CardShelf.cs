using System.Collections.Generic;
using UnityEngine;

public class CardShelf : InteractableObject
{
	public bool m_ItemNotForSale;

	public List<Transform> m_CardShelfCompartmentGrpList;

	public ElectronicCardListener m_ElectronicCardListener;

	private List<InteractableCardCompartment> m_CardCompartmentList = new List<InteractableCardCompartment>();

	private List<UI_PriceTag> m_UIPriceTagList = new List<UI_PriceTag>();

	private int m_Index;

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitCardShelf(this);
	}

	public override void Init()
	{
		if (m_HasInit)
		{
			return;
		}
		base.Init();
		for (int i = 0; i < m_CardShelfCompartmentGrpList.Count; i++)
		{
			for (int j = 0; j < m_CardShelfCompartmentGrpList[i].childCount; j++)
			{
				m_CardCompartmentList.Add(m_CardShelfCompartmentGrpList[i].GetChild(j).GetComponent<InteractableCardCompartment>());
			}
		}
		m_Shelf_WorldUIGrp = PriceTagUISpawner.SpawnShelfWorldUIGrp(base.transform);
		for (int k = 0; k < m_CardCompartmentList.Count; k++)
		{
			m_CardCompartmentList[k].m_ItemNotForSale = m_ItemNotForSale;
			m_CardCompartmentList[k].InitCardShelf(this);
			m_GamepadQuickSelectTransformList.Add(m_CardCompartmentList[k].m_GamepadQuickSelectAimLoc.transform);
			if (!m_ItemNotForSale)
			{
				if (m_CardCompartmentList[k].m_InteractablePriceTagList.Count > 0)
				{
					m_GamepadQuickSelectTransformList.Add(m_CardCompartmentList[k].m_InteractablePriceTagList[0].transform);
					m_GamepadQuickSelectPriceTagTransformList.Add(m_CardCompartmentList[k].m_InteractablePriceTagList[0].transform);
				}
				for (int l = 0; l < m_CardCompartmentList[k].m_InteractablePriceTagList.Count; l++)
				{
					UI_PriceTag uI_PriceTag = PriceTagUISpawner.SpawnPriceTagCardWorldUIGrp(m_Shelf_WorldUIGrp, m_CardCompartmentList[k].m_InteractablePriceTagList[l].transform);
					m_UIPriceTagList.Add(uI_PriceTag);
					m_CardCompartmentList[k].m_InteractablePriceTagList[l].SetPriceTagUI(uI_PriceTag);
					m_CardCompartmentList[k].m_InteractablePriceTagList[l].SetVisibility(isVisible: false);
				}
			}
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (m_IsMovingObject && (bool)m_Shelf_WorldUIGrp)
		{
			m_Shelf_WorldUIGrp.transform.position = base.transform.position;
			m_Shelf_WorldUIGrp.transform.rotation = base.transform.rotation;
		}
	}

	public void LoadCardCompartment(List<CardData> cardDataList)
	{
		if (cardDataList == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < m_CardCompartmentList.Count && i < cardDataList.Count; i++)
		{
			if (cardDataList[i] != null && cardDataList[i].monsterType != EMonsterType.None && (cardDataList[i].monsterType < EMonsterType.MAX || cardDataList[i].monsterType >= EMonsterType.Alpha))
			{
				Card3dUIGroup cardUI = CSingleton<Card3dUISpawner>.Instance.GetCardUI();
				InteractableCard3d component = ShelfManager.SpawnInteractableObject(EObjectType.Card3d).GetComponent<InteractableCard3d>();
				cardUI.m_IgnoreCulling = true;
				cardUI.m_CardUI.SetFoilCullListVisibility(isActive: true);
				cardUI.SetSimplifyCardDistanceCull(isCull: false);
				cardUI.m_CardUI.ResetFarDistanceCull();
				cardUI.m_CardUI.SetCardUI(cardDataList[i]);
				cardUI.transform.position = component.transform.position;
				cardUI.transform.rotation = component.transform.rotation;
				component.SetCardUIFollow(cardUI);
				component.SetEnableCollision(isEnable: false);
				m_CardCompartmentList[i].SetCardOnShelf(component);
				cardUI.m_IgnoreCulling = false;
				flag = true;
			}
		}
		if ((bool)m_ElectronicCardListener && flag)
		{
			m_ElectronicCardListener.ShowCardInstantly();
		}
	}

	protected override void OnPlacedMovedObject()
	{
		base.OnPlacedMovedObject();
		if (m_ObjectType == EObjectType.CardShelf)
		{
			TutorialManager.AddTaskValue(ETutorialTaskCondition.BuyCardShelf, 1f);
		}
		if ((bool)m_ElectronicCardListener)
		{
			m_ElectronicCardListener.OnPlacedMovedObject();
		}
	}

	protected override void OnStartMoveObject()
	{
		base.OnStartMoveObject();
		for (int i = 0; i < m_CardCompartmentList.Count; i++)
		{
			if (m_CardCompartmentList[i].m_StoredCardList.Count > 0)
			{
				m_CardCompartmentList[i].SetCardVisibility(isVisible: true);
			}
		}
		if ((bool)m_ElectronicCardListener)
		{
			m_ElectronicCardListener.OnStartMoveObject();
		}
	}

	public override void BoxUpObject(bool holdBox)
	{
		base.BoxUpObject(holdBox);
		for (int i = 0; i < m_CardCompartmentList.Count; i++)
		{
			if (m_CardCompartmentList[i].m_StoredCardList.Count > 0)
			{
				m_CardCompartmentList[i].SetCardVisibility(isVisible: false);
			}
		}
		if ((bool)m_ElectronicCardListener)
		{
			m_ElectronicCardListener.BoxUpObject();
		}
	}

	public override void OnDestroyed()
	{
		ShelfManager.RemoveCardShelf(this);
		for (int i = 0; i < m_CardCompartmentList.Count; i++)
		{
			if ((bool)m_CardCompartmentList[i])
			{
				m_CardCompartmentList[i].DisableAllCard();
			}
		}
		base.OnDestroyed();
	}

	public InteractableCardCompartment GetCustomerTargetCardCompartment()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < m_CardCompartmentList.Count; i++)
		{
			if (m_CardCompartmentList[i].m_StoredCardList.Count > 0)
			{
				list.Add(i);
			}
		}
		if (list.Count > 0)
		{
			int index = list[Random.Range(0, list.Count)];
			return m_CardCompartmentList[index];
		}
		return m_CardCompartmentList[Random.Range(0, m_CardCompartmentList.Count)];
	}

	public InteractableCardCompartment GetCardCompartment(int index)
	{
		return m_CardCompartmentList[index];
	}

	public List<InteractableCardCompartment> GetCardCompartmentList()
	{
		return m_CardCompartmentList;
	}

	public void SetIndex(int index)
	{
		m_Index = index;
	}

	public int GetIndex()
	{
		return m_Index;
	}

	public bool HasCardOnShelf()
	{
		for (int i = 0; i < m_CardCompartmentList.Count; i++)
		{
			if (m_CardCompartmentList[i].m_StoredCardList.Count > 0)
			{
				return true;
			}
		}
		return false;
	}
}
