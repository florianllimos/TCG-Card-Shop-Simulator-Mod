using System.Collections.Generic;
using UnityEngine;

public class InteractableWorkbench : InteractableObject
{
	public Transform m_LockPlayerPos;

	public Transform m_PlayerLookRot;

	public Transform m_SpawnItemPos;

	public List<Transform> m_PosList;

	public GameObject m_NavMeshCutWhenManned;

	public Animator m_JankBoxAnim;

	public SkinnedMeshRenderer m_JankBoxSkinMesh;

	public List<Animation> m_CardEnterBoxAnimList;

	public List<InteractableCard3d> m_InteractableCard3dList;

	public List<EItemType> m_ValidStoreItemTypeList;

	private bool m_IsPlayCardEnterAnim;

	private bool m_IsEditingDeck;

	private int m_CardEnterIndex;

	private float m_CardEnterTimer;

	private int m_ItemAmount;

	public List<Item> m_StoredItemList;

	private EItemType m_CurrentItemTypeSpawn = EItemType.None;

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitWorkbench(this);
		m_JankBoxAnim.gameObject.SetActive(value: false);
		for (int i = 0; i < m_CardEnterBoxAnimList.Count; i++)
		{
			Card3dUIGroup cardUI = CSingleton<Card3dUISpawner>.Instance.GetCardUI();
			cardUI.transform.position = m_InteractableCard3dList[i].transform.position;
			cardUI.transform.rotation = m_InteractableCard3dList[i].transform.rotation;
			m_InteractableCard3dList[i].SetCardUIFollow(cardUI);
			cardUI.SetVisibility(isVisible: false);
			m_CardEnterBoxAnimList[i].gameObject.SetActive(value: false);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!m_IsPlayCardEnterAnim)
		{
			return;
		}
		m_CardEnterTimer += Time.deltaTime;
		if (m_CardEnterTimer > 0.1f)
		{
			m_CardEnterBoxAnimList[m_CardEnterIndex].gameObject.SetActive(value: false);
			m_CardEnterBoxAnimList[m_CardEnterIndex].gameObject.SetActive(value: true);
			m_InteractableCard3dList[m_CardEnterIndex].m_Card3dUI.SetVisibility(isVisible: true);
			m_CardEnterBoxAnimList[m_CardEnterIndex].Play();
			m_CardEnterTimer = 0f;
			m_CardEnterIndex++;
			if (m_CardEnterIndex >= m_InteractableCard3dList.Count)
			{
				m_IsPlayCardEnterAnim = false;
				m_CardEnterIndex = 0;
			}
		}
	}

	public override void OnMouseButtonUp()
	{
		if (!m_IsEditingDeck)
		{
			CSingleton<InteractionPlayerController>.Instance.OnEnterWorkbenchMode();
			OnRaycastEnded();
			InteractionPlayerController.SetPlayerPos(m_LockPlayerPos.position);
			CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
			CSingleton<InteractionPlayerController>.Instance.ForceLookAt(m_PlayerLookRot, 3f);
			m_NavMeshCutWhenManned.SetActive(value: true);
			WorkbenchUIScreen.OpenScreen(this);
		}
	}

	public override void OnRightMouseButtonUp()
	{
		if (!m_IsEditingDeck)
		{
			base.OnRightMouseButtonUp();
		}
	}

	public override void OnPressEsc()
	{
		if (!m_IsEditingDeck)
		{
			CSingleton<InteractionPlayerController>.Instance.OnExitWorkbenchMode();
			m_NavMeshCutWhenManned.SetActive(value: false);
			m_JankBoxAnim.gameObject.SetActive(value: false);
			m_JankBoxAnim.SetBool("IsClosing", value: false);
			for (int i = 0; i < m_CardEnterBoxAnimList.Count; i++)
			{
				m_InteractableCard3dList[i].m_Card3dUI.SetVisibility(isVisible: false);
				m_CardEnterBoxAnimList[i].gameObject.SetActive(value: false);
			}
		}
	}

	public override void OnRaycasted()
	{
		if (!m_IsEditingDeck)
		{
			base.OnRaycasted();
		}
	}

	public void PlayBundlingCardBoxSequence(List<CardData> cardDataListShown, ECardExpansionType cardExpansionType, float totalPrice)
	{
		m_JankBoxAnim.gameObject.SetActive(value: true);
		m_JankBoxAnim.SetBool("IsClosing", value: true);
		m_CurrentItemTypeSpawn = GetBulkBoxItemType(cardExpansionType, totalPrice);
		ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(m_CurrentItemTypeSpawn);
		m_JankBoxSkinMesh.material = itemMeshData.material;
		for (int i = 0; i < cardDataListShown.Count; i++)
		{
			m_InteractableCard3dList[i].m_Card3dUI.m_CardUI.SetCardUI(cardDataListShown[i]);
		}
		m_IsPlayCardEnterAnim = true;
	}

	public void OnTaskCompleted(ECardExpansionType cardExpansionType)
	{
		m_JankBoxAnim.gameObject.SetActive(value: false);
		Item item = null;
		ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(m_CurrentItemTypeSpawn);
		item = ItemSpawnManager.GetItem(m_SpawnItemPos);
		item.SetMesh(itemMeshData.mesh, itemMeshData.material, m_CurrentItemTypeSpawn, itemMeshData.meshSecondary, itemMeshData.materialSecondary);
		item.transform.position = m_SpawnItemPos.position;
		item.transform.rotation = m_SpawnItemPos.rotation;
		item.gameObject.SetActive(value: true);
		CSingleton<InteractionPlayerController>.Instance.AddHoldItemToFront(item);
	}

	public EItemType GetBulkBoxItemType(ECardExpansionType cardExpansionType, float totalPrice)
	{
		EItemType eItemType = EItemType.BulkBox_TetramonBase;
		if (cardExpansionType == ECardExpansionType.Destiny)
		{
			if (totalPrice > 1200f)
			{
				return EItemType.BulkBox_TetramonDestinyGradeUG;
			}
			if (totalPrice > 400f)
			{
				return EItemType.BulkBox_TetramonDestinyGradePG;
			}
			if (totalPrice > 200f)
			{
				return EItemType.BulkBox_TetramonDestinyGradeMG;
			}
			if (totalPrice > 100f)
			{
				return EItemType.BulkBox_TetramonDestinyGradeHG;
			}
			return EItemType.BulkBox_TetramonDestiny;
		}
		if (totalPrice > 1200f)
		{
			return EItemType.BulkBox_TetramonBaseGradeUG;
		}
		if (totalPrice > 400f)
		{
			return EItemType.BulkBox_TetramonBaseGradePG;
		}
		if (totalPrice > 200f)
		{
			return EItemType.BulkBox_TetramonBaseGradeMG;
		}
		if (totalPrice > 100f)
		{
			return EItemType.BulkBox_TetramonBaseGradeHG;
		}
		return EItemType.BulkBox_TetramonBase;
	}

	public void DispenseItemFromBox(InteractablePackagingBox_Item itemBox, bool isPlayer)
	{
		if (m_IsEditingDeck)
		{
			return;
		}
		if ((bool)itemBox && itemBox.IsBoxOpened())
		{
			if (IsValidItemType(itemBox.m_ItemCompartment.GetItemType()))
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
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CanOnlyPutBulkBox);
			}
		}
		else if ((bool)itemBox && !itemBox.IsBoxOpened())
		{
			itemBox.OnPressOpenBox();
		}
	}

	public void AddItem(Item item, bool addToFront)
	{
		if (!m_IsEditingDeck)
		{
			m_ItemAmount++;
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
	}

	public void RemoveItem(Item item)
	{
		if (!m_IsEditingDeck)
		{
			m_ItemAmount--;
			m_StoredItemList.Remove(item);
		}
	}

	public Item TakeItemToHand(bool getLastItem = true)
	{
		if (m_IsEditingDeck)
		{
			return null;
		}
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
		return item;
	}

	public void RemoveItemFromShelf(bool isPlayer, InteractablePackagingBox_Item packageBox)
	{
		if ((m_IsEditingDeck && isPlayer) || !packageBox.IsBoxOpened())
		{
			return;
		}
		if (m_StoredItemList.Count <= 0)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfNoItem);
			return;
		}
		Item firstItem = GetFirstItem();
		if ((bool)firstItem)
		{
			if (firstItem.GetItemType() == packageBox.GetItemType() || packageBox.m_ItemCompartment.GetItemCount() == 0)
			{
				packageBox.SetItemType(firstItem.GetItemType());
				packageBox.m_ItemCompartment.CheckItemType(firstItem.GetItemType());
				if (packageBox.m_ItemCompartment.HasEnoughSlot())
				{
					Transform lastEmptySlotTransform = packageBox.m_ItemCompartment.GetLastEmptySlotTransform();
					Transform emptySlotParent = packageBox.m_ItemCompartment.GetEmptySlotParent();
					firstItem.LerpToTransform(lastEmptySlotTransform, emptySlotParent);
					firstItem = TakeItemToHand(getLastItem: false);
					packageBox.m_ItemCompartment.AddItem(firstItem, addToFront: true);
					packageBox.m_ItemCompartment.SetPriceTagVisibility(isVisible: false);
					if (isPlayer)
					{
						SoundManager.GenericPop(1f, 0.9f);
					}
				}
				else
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxNoSlot);
					Debug.Log("Box no slot");
				}
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxWrongItemType);
				Debug.Log("Item type not match");
			}
		}
		else
		{
			Debug.Log("No item on workbench");
		}
	}

	public bool HasEnoughSlot()
	{
		if (m_ItemAmount < m_PosList.Count)
		{
			return true;
		}
		return false;
	}

	public int GetItemCount()
	{
		return m_ItemAmount;
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

	public bool IsValidItemType(EItemType itemType)
	{
		if (m_ValidStoreItemTypeList.Contains(itemType))
		{
			return true;
		}
		return false;
	}

	public void SetIsEditingDeck(bool isEditingDeck)
	{
		m_IsEditingDeck = isEditingDeck;
	}

	public void LoadData(WorkbenchSaveData saveData)
	{
		for (int i = 0; i < saveData.itemTypeList.Count; i++)
		{
			Item item = null;
			ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(saveData.itemTypeList[i]);
			item = ItemSpawnManager.GetItem(m_PosList[i]);
			item.SetMesh(itemMeshData.mesh, itemMeshData.material, saveData.itemTypeList[i], itemMeshData.meshSecondary, itemMeshData.materialSecondary);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
			item.gameObject.SetActive(value: true);
			AddItem(item, addToFront: true);
		}
	}
}
