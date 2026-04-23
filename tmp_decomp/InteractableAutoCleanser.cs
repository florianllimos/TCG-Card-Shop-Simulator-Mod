using System.Collections.Generic;
using UnityEngine;

public class InteractableAutoCleanser : InteractableObject
{
	public List<Transform> m_PosList;

	public Transform m_StandLoc;

	public ParticleSystem m_DeodorantSprayVFX;

	public int m_Potency = 5;

	public int m_DispenseMethod;

	public float m_ItemContentDepleteAmountPerSpray = 0.05f;

	public float m_CleanserRange = 2.5f;

	public float m_CooldownTime = 3f;

	public float m_UIPosYOffset = 1f;

	private bool m_IsTurnedOn;

	private bool m_IsSprayOnCooldown = true;

	private bool m_IsNeedRefill = true;

	private int m_ItemAmount;

	private float m_Timer;

	public List<Item> m_StoredItemList;

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitAutoCleanser(this);
	}

	public int GetCurrentSlotAvailable()
	{
		return m_PosList.Count - m_ItemAmount;
	}

	public override void OnMouseButtonUp()
	{
		base.OnMouseButtonUp();
		m_IsTurnedOn = !m_IsTurnedOn;
		if (!m_IsTurnedOn)
		{
			m_IsSprayOnCooldown = true;
			m_Timer = 0f;
		}
		else if (m_IsTurnedOn && m_IsNeedRefill)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AutoCleanserRefill);
		}
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.6f, 0.5f);
		if (m_IsTurnedOn)
		{
			InteractionPlayerController.AddToolTip(EGameAction.TurnOff);
			InteractionPlayerController.RemoveToolTip(EGameAction.TurnOn);
		}
		else
		{
			InteractionPlayerController.AddToolTip(EGameAction.TurnOn);
			InteractionPlayerController.RemoveToolTip(EGameAction.TurnOff);
		}
	}

	public override void OnRightMouseButtonUp()
	{
		base.OnRightMouseButtonUp();
	}

	protected override void Update()
	{
		base.Update();
		if (m_IsBoxedUp || !m_IsTurnedOn || m_IsNeedRefill)
		{
			return;
		}
		if (m_DispenseMethod == 1)
		{
			if (!m_IsSprayOnCooldown)
			{
				for (int i = 0; i < CSingleton<CustomerManager>.Instance.GetSmellyCustomerList().Count; i++)
				{
					if (CSingleton<CustomerManager>.Instance.GetSmellyCustomerList()[i].IsInsideShop() && (base.transform.position - CSingleton<CustomerManager>.Instance.GetSmellyCustomerList()[i].transform.position).magnitude <= m_CleanserRange)
					{
						m_Timer = 0f;
						m_IsSprayOnCooldown = true;
						Spray();
					}
				}
			}
			else
			{
				m_Timer += Time.deltaTime;
				if (m_Timer >= m_CooldownTime)
				{
					m_Timer = m_CooldownTime;
					m_IsSprayOnCooldown = false;
				}
			}
		}
		else if (m_DispenseMethod == 0)
		{
			m_Timer += Time.deltaTime;
			if (m_CooldownTime > 0f && m_Timer >= m_CooldownTime)
			{
				m_Timer = 0f;
				Spray();
			}
		}
	}

	public override void OnRaycasted()
	{
		base.OnRaycasted();
		m_IsRaycasted = true;
		AutoCleanserStatusUI.SetTargetCleanser(this, m_UIPosYOffset);
	}

	public override void OnRaycastEnded()
	{
		base.OnRaycastEnded();
		m_IsRaycasted = false;
		AutoCleanserStatusUI.SetTargetCleanser(null);
	}

	private bool HasEnoughSprayMeter(bool depleteItemContent)
	{
		if (m_ItemAmount <= 0)
		{
			return false;
		}
		for (int i = 0; i < m_StoredItemList.Count; i++)
		{
			if (m_StoredItemList[i].GetContentFill() <= 0f)
			{
				m_StoredItemList[i].DisableItem();
				RemoveItem(m_StoredItemList[i]);
			}
		}
		int num = -1;
		float num2 = 1000f;
		for (int j = 0; j < m_StoredItemList.Count; j++)
		{
			if (m_StoredItemList[j].GetContentFill() > 0f && m_StoredItemList[j].GetContentFill() < num2)
			{
				num = j;
				num2 = m_StoredItemList[j].GetContentFill();
			}
		}
		if (num != -1)
		{
			if (depleteItemContent)
			{
				m_StoredItemList[num].DepleteContent(m_ItemContentDepleteAmountPerSpray);
				if (m_StoredItemList[num].GetContentFill() <= 0f)
				{
					m_StoredItemList[num].DisableItem();
					RemoveItem(m_StoredItemList[num]);
				}
			}
			return true;
		}
		return false;
	}

	private void Spray()
	{
		if (m_IsNeedRefill)
		{
			return;
		}
		if (HasEnoughSprayMeter(depleteItemContent: true))
		{
			m_DeodorantSprayVFX.Play();
			for (int i = 0; i < CSingleton<CustomerManager>.Instance.GetCustomerList().Count; i++)
			{
				CSingleton<CustomerManager>.Instance.GetCustomerList()[i].DeodorantSprayCheck(base.transform.position, m_CleanserRange, m_Potency);
			}
		}
		else
		{
			m_IsNeedRefill = true;
			m_IsSprayOnCooldown = true;
			m_Timer = 0f;
			Debug.Log("Ran out of spray content, need refill");
		}
	}

	public void DispenseItemFromBox(InteractablePackagingBox_Item itemBox, bool isPlayer)
	{
		if ((bool)itemBox && itemBox.IsBoxOpened())
		{
			if (itemBox.m_ItemCompartment.GetItemType() == EItemType.Deodorant)
			{
				if (itemBox.m_ItemCompartment.GetItemCount() > 0)
				{
					if (HasEnoughSlot())
					{
						Item firstItem = itemBox.m_ItemCompartment.GetFirstItem();
						AddItem(firstItem, addToFront: true);
						itemBox.m_ItemCompartment.RemoveItem(firstItem);
						if (isPlayer)
						{
							SoundManager.GenericPop();
						}
					}
					else if (isPlayer)
					{
						NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AutoCleanserNoSlot);
					}
				}
				else if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxNoItem);
				}
			}
			else if (isPlayer)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AutoCleanserOnlyAllowCleanser);
			}
		}
		else if ((bool)itemBox && !itemBox.IsBoxOpened())
		{
			itemBox.OnPressOpenBox();
		}
	}

	public void AddItem(Item item, bool addToFront)
	{
		if (item.GetContentFill() > 0f)
		{
			m_IsNeedRefill = false;
		}
		m_ItemAmount++;
		if (m_IsRaycasted)
		{
			CSingleton<AutoCleanserStatusUI>.Instance.UpdateItemSlotAmountFilled(m_ItemAmount);
		}
		if (addToFront)
		{
			m_StoredItemList.Insert(0, item);
		}
		else
		{
			m_StoredItemList.Add(item);
		}
		item.LerpToTransform(m_PosList[m_ItemAmount - 1], m_PosList[m_ItemAmount - 1]);
	}

	public void RemoveItem(Item item)
	{
		m_ItemAmount--;
		m_StoredItemList.Remove(item);
		if (!HasEnoughSprayMeter(depleteItemContent: false))
		{
			m_IsNeedRefill = true;
			m_IsSprayOnCooldown = true;
			m_Timer = 0f;
		}
		if (m_IsRaycasted)
		{
			CSingleton<AutoCleanserStatusUI>.Instance.UpdateItemSlotAmountFilled(m_ItemAmount);
		}
		for (int i = 0; i < m_StoredItemList.Count; i++)
		{
			m_StoredItemList[i].transform.position = m_PosList[i].position;
		}
	}

	public Item TakeItemToHand(bool getLastItem = true)
	{
		if (m_ItemAmount <= 0)
		{
			return null;
		}
		Item item = GetLastItem();
		if (!getLastItem)
		{
			item = GetFirstItem();
		}
		m_ItemAmount--;
		m_StoredItemList.Remove(item);
		if (m_IsRaycasted)
		{
			CSingleton<AutoCleanserStatusUI>.Instance.UpdateItemSlotAmountFilled(m_ItemAmount);
		}
		if (!HasEnoughSprayMeter(depleteItemContent: false))
		{
			m_IsNeedRefill = true;
			m_IsSprayOnCooldown = true;
			m_Timer = 0f;
		}
		return item;
	}

	protected override void OnStartMoveObject()
	{
		base.OnStartMoveObject();
		AutoCleanserStatusUI.SetTargetCleanser(this, m_UIPosYOffset);
	}

	protected override void OnPlacedMovedObject()
	{
		base.OnPlacedMovedObject();
		AutoCleanserStatusUI.SetTargetCleanser(null);
	}

	public override void OnDestroyed()
	{
		ShelfManager.RemoveAutoCleanser(this);
		base.OnDestroyed();
	}

	public int GetItemCount()
	{
		return m_ItemAmount;
	}

	public bool HasEnoughSlot()
	{
		if (m_ItemAmount < m_PosList.Count)
		{
			return true;
		}
		return false;
	}

	public Transform GetEmptySlotTransform()
	{
		return m_PosList[m_ItemAmount];
	}

	public Transform GetLastEmptySlotTransform()
	{
		return m_PosList[m_PosList.Count - m_ItemAmount - 1];
	}

	public Item GetFirstItem()
	{
		return m_StoredItemList[0];
	}

	public Item GetLastItem()
	{
		if (m_StoredItemList.Count <= 0)
		{
			return null;
		}
		return m_StoredItemList[m_ItemAmount - 1];
	}

	public float GetTimer()
	{
		return m_Timer;
	}

	public bool IsTurnedOn()
	{
		return m_IsTurnedOn;
	}

	public bool IsNeedRefill()
	{
		return m_IsNeedRefill;
	}

	public List<Item> GetStoredItemList()
	{
		return m_StoredItemList;
	}

	public void LoadData(AutoCleanserSaveData saveData)
	{
		m_IsNeedRefill = saveData.isNeedRefill;
		m_IsTurnedOn = saveData.isTurnedOn;
		m_IsSprayOnCooldown = true;
		m_Timer = 0f;
		for (int i = 0; i < saveData.itemAmount; i++)
		{
			Item item = null;
			ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(EItemType.Deodorant);
			item = ItemSpawnManager.GetItem(m_PosList[i]);
			item.SetMesh(itemMeshData.mesh, itemMeshData.material, EItemType.Deodorant, itemMeshData.meshSecondary, itemMeshData.materialSecondary);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
			item.SetContentFill(saveData.contentFillList[i]);
			item.gameObject.SetActive(value: true);
			AddItem(item, addToFront: true);
		}
		m_ItemAmount = saveData.itemAmount;
		for (int j = 0; j < m_StoredItemList.Count; j++)
		{
			if (m_StoredItemList[j].GetContentFill() <= 0f)
			{
				m_StoredItemList[j].DisableItem();
				RemoveItem(m_StoredItemList[j]);
			}
		}
	}
}
