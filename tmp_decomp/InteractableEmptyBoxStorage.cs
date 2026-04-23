using System.Collections.Generic;
using UnityEngine;

public class InteractableEmptyBoxStorage : InteractableObject
{
	public Transform m_EmptyBoxStack;

	public Transform m_EmptyBoxSpawnLoc;

	public List<Transform> m_CustomerStandLocList;

	private int m_StoredBoxCount;

	private int m_MaxStoredBoxCount = 200;

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitEmptyBoxStorage(this);
		EvaluateStoredBoxStackHeight();
	}

	public override void OnMouseButtonUp()
	{
		base.OnMouseButtonUp();
		TakeBox(isPlayer: true);
	}

	public void TakeBox(bool isPlayer)
	{
		if (m_StoredBoxCount <= 0)
		{
			return;
		}
		InteractablePackagingBox_Item interactablePackagingBox_Item = RestockManager.SpawnPackageBoxItem(EItemType.None, 0, isBigBox: true);
		interactablePackagingBox_Item.transform.position = m_EmptyBoxSpawnLoc.position;
		interactablePackagingBox_Item.transform.rotation = m_EmptyBoxSpawnLoc.rotation;
		if (interactablePackagingBox_Item.CanPickup())
		{
			interactablePackagingBox_Item.ForceSetOpenCloseInstant(isOpen: true);
			interactablePackagingBox_Item.SetOpenCloseBox(isOpen: false, isPlayer);
			interactablePackagingBox_Item.StartHoldBox(isPlayer, CSingleton<InteractionPlayerController>.Instance.m_HoldItemPos);
			m_StoredBoxCount--;
			EvaluateStoredBoxStackHeight();
			if (isPlayer)
			{
				InteractionPlayerController.AddToolTip(EGameAction.StoreBox);
			}
		}
	}

	public void StoreBox(InteractablePackagingBox_Item packagingBox, bool isPlayer)
	{
		if (packagingBox.IsTogglingOpenClose())
		{
			return;
		}
		if (packagingBox.m_ItemCompartment.GetItemCount() > 0)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CanOnlyStoreEmptyBox);
			return;
		}
		if (!packagingBox.m_IsBigBox)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CannotStoreWrongBoxSize);
			return;
		}
		if (m_StoredBoxCount >= m_MaxStoredBoxCount)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfNoSlot);
			return;
		}
		packagingBox.OnDestroyed();
		m_StoredBoxCount++;
		EvaluateStoredBoxStackHeight();
		if (isPlayer)
		{
			SoundManager.PlayAudio("SFX_Throw", 0.6f);
			CSingleton<InteractionPlayerController>.Instance.OnExitHoldBoxMode();
			m_IsRaycasted = false;
			OnRaycasted();
		}
	}

	private void EvaluateStoredBoxStackHeight()
	{
		if (m_StoredBoxCount <= 0)
		{
			m_EmptyBoxStack.gameObject.SetActive(value: false);
			return;
		}
		Vector3 one = Vector3.one;
		one.y = 1f / (float)m_MaxStoredBoxCount * (float)m_StoredBoxCount;
		m_EmptyBoxStack.localScale = one;
		m_EmptyBoxStack.gameObject.SetActive(value: true);
	}

	public int GetBoxStoredCount()
	{
		return m_StoredBoxCount;
	}

	public bool HasStorageSpace()
	{
		return m_StoredBoxCount < m_MaxStoredBoxCount;
	}

	public void LoadData(EmptyBoxStorageSaveData saveData)
	{
		m_StoredBoxCount = saveData.storedBoxCount;
		EvaluateStoredBoxStackHeight();
	}
}
