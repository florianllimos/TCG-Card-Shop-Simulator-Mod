using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCardCompartment : MonoBehaviour
{
	public bool m_ItemNotForSale;

	public bool m_HideAdapterMesh;

	public bool m_NoneGradedCardUseAltCardLocation;

	public Transform m_CustomerStandLoc;

	public Transform m_StoredItemListGrp;

	public Transform m_PutCardLocation;

	public Transform m_PutCardLocationAlt;

	public Transform m_GamepadQuickSelectAimLoc;

	public GameObject m_AdapterMesh;

	public List<InteractableCardPriceTag> m_InteractablePriceTagList;

	public List<InteractableCard3d> m_StoredCardList = new List<InteractableCard3d>();

	private int m_Index;

	private CardShelf m_CardShelf;

	private ShelfCompartment m_ShelfCompartment;

	private float m_CurrentPrice;

	private bool m_IsSettingPrice;

	private Coroutine m_ResetIsSettingPriceCoroutine;

	public void InitCardShelf(CardShelf cardShelf)
	{
		m_CardShelf = cardShelf;
		for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
		{
			m_InteractablePriceTagList[i].Init(this);
		}
		if (m_ItemNotForSale)
		{
			for (int j = 0; j < m_InteractablePriceTagList.Count; j++)
			{
				m_InteractablePriceTagList[j].gameObject.SetActive(value: false);
			}
		}
		if ((bool)m_AdapterMesh)
		{
			m_AdapterMesh.SetActive(!m_HideAdapterMesh);
		}
	}

	public void OnMouseButtonUp()
	{
		if (!InteractionPlayerController.GetCurrentHoldCard() || InteractionPlayerController.GetCurrentHoldCard().IsDisplayedOnShelf())
		{
			return;
		}
		if (m_StoredCardList.Count > 0)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.GenericNoSlot);
			return;
		}
		SetCardOnShelf(InteractionPlayerController.GetCurrentHoldCard());
		InteractionPlayerController.RemoveCurrentCard();
		for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
		{
			m_InteractablePriceTagList[i].SetPriceChecked(isPriceSet: false);
		}
		TutorialManager.AddTaskValue(ETutorialTaskCondition.PutCardOnShelf, 1f);
	}

	public void OnRightMouseButtonUp()
	{
		if (m_StoredCardList.Count > 0 && m_StoredCardList[0].IsDisplayedOnShelf())
		{
			if (InteractionPlayerController.HasEnoughSlotToHoldCard())
			{
				InteractionPlayerController.AddHoldCard(m_StoredCardList[0]);
				CSingleton<InteractionPlayerController>.Instance.EnterHoldCardMode();
				RemoveCardFromShelf(null, null);
				TutorialManager.AddTaskValue(ETutorialTaskCondition.PutCardOnShelf, -1f);
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.HandFull);
			}
		}
	}

	public void SetCardOnShelf(InteractableCard3d card)
	{
		card.m_Card3dUI.m_IgnoreCulling = false;
		card.SetIsDisplayedOnShelf(isDisplayedOnShelf: true);
		SetPriceTagCardData(card.m_Card3dUI.m_CardUI.GetCardData());
		SetPriceTagItemPriceText(card.m_Card3dUI.m_CardUI.GetCardData());
		card.SetTargetRotation(Quaternion.identity);
		if (m_NoneGradedCardUseAltCardLocation && card.m_Card3dUI.m_CardUI.GetCardData().cardGrade == 0)
		{
			card.LerpToTransform(m_PutCardLocationAlt, m_StoredItemListGrp);
		}
		else
		{
			card.LerpToTransform(m_PutCardLocation, m_StoredItemListGrp);
		}
		m_StoredCardList.Add(card);
		SetPriceTagVisibility(isVisible: true);
		if ((bool)m_CardShelf.m_ElectronicCardListener)
		{
			m_CardShelf.m_ElectronicCardListener.UpdateCardUI(card.m_Card3dUI.m_CardUI.GetCardData());
		}
		if (m_ResetIsSettingPriceCoroutine != null)
		{
			StopCoroutine(m_ResetIsSettingPriceCoroutine);
		}
		m_ResetIsSettingPriceCoroutine = StartCoroutine(DelayResetIsSettingPrice());
	}

	private IEnumerator DelayResetIsSettingPrice()
	{
		m_IsSettingPrice = true;
		yield return new WaitForSeconds(3f);
		m_IsSettingPrice = false;
	}

	public void RemoveCardFromShelf(Transform targetpos, Transform targetParent)
	{
		if (m_StoredCardList.Count > 0)
		{
			m_StoredCardList[0].SetIsDisplayedOnShelf(isDisplayedOnShelf: false);
			m_StoredCardList[0].SetTargetRotation(Quaternion.identity);
			if (targetpos != null)
			{
				m_StoredCardList[0].LerpToTransform(targetpos, targetParent);
			}
			m_StoredCardList.RemoveAt(0);
			SetPriceTagCardData(null);
			SetPriceTagVisibility(isVisible: false);
			if ((bool)m_CardShelf.m_ElectronicCardListener)
			{
				m_CardShelf.m_ElectronicCardListener.UpdateCardUI(null);
			}
		}
	}

	public void SetPriceTagVisibility(bool isVisible)
	{
		if (!m_ItemNotForSale)
		{
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].SetVisibility(isVisible);
			}
		}
	}

	public void SetPriceTagCardData(CardData cardData)
	{
		if (!m_ItemNotForSale)
		{
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].SetCardData(cardData);
			}
		}
	}

	public void SetPriceTagItemPriceText(CardData cardData)
	{
		if (!m_ItemNotForSale)
		{
			m_CurrentPrice = CPlayerData.GetCardPrice(cardData);
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].SetPriceText(m_CurrentPrice);
			}
		}
	}

	public void RefreshPriceTagItemPriceText()
	{
		if (!m_ItemNotForSale)
		{
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].RefreshPriceText();
			}
		}
	}

	public void SetCardVisibility(bool isVisible)
	{
		for (int i = 0; i < m_StoredCardList.Count; i++)
		{
			if ((bool)m_StoredCardList[i])
			{
				m_StoredCardList[i].m_Card3dUI.SetVisibility(isVisible);
			}
		}
	}

	public void DisableAllCard()
	{
		if ((bool)m_CardShelf.m_ElectronicCardListener)
		{
			m_CardShelf.m_ElectronicCardListener.OnDestroyed();
		}
		for (int i = 0; i < m_StoredCardList.Count; i++)
		{
			if ((bool)m_StoredCardList[i])
			{
				m_StoredCardList[i].OnDestroyed();
			}
		}
	}

	public CardShelf GetCardShelf()
	{
		return m_CardShelf;
	}

	public bool HasSetPrice()
	{
		for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
		{
			if (m_InteractablePriceTagList[i].GetIsPriceSet())
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSettingPrice()
	{
		return m_IsSettingPrice;
	}

	public void OnStartSetPrice()
	{
		if (m_ResetIsSettingPriceCoroutine != null)
		{
			StopCoroutine(m_ResetIsSettingPriceCoroutine);
		}
		m_IsSettingPrice = true;
	}

	public void OnFinishSetPrice()
	{
		m_IsSettingPrice = false;
	}
}
