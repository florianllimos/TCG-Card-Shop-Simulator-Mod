using UnityEngine;

public class InteractablePriceTag : InteractableObject
{
	private ShelfCompartment m_ShelfCompartment;

	public UI_PriceTag m_PriceTagUI;

	public bool m_IsWarehouseTag;

	private bool m_IsPriceSet;

	public void Init(ShelfCompartment shelfCompartment)
	{
		m_ShelfCompartment = shelfCompartment;
	}

	protected override void ShowToolTip()
	{
		if (m_IsWarehouseTag)
		{
			InteractionPlayerController.AddToolTip(EGameAction.RemoveLabel);
		}
		else
		{
			for (int i = 0; i < m_GameActionInputDisplayList.Count; i++)
			{
				if (m_GameActionInputDisplayList[i] == EGameAction.RemoveLabel)
				{
					if (m_ShelfCompartment.GetItemCount() == 0)
					{
						InteractionPlayerController.AddToolTip(m_GameActionInputDisplayList[i]);
					}
				}
				else
				{
					InteractionPlayerController.AddToolTip(m_GameActionInputDisplayList[i]);
				}
			}
		}
		for (int j = 0; j < m_ControllerOnlyGameActionInputDisplayList.Count; j++)
		{
			InteractionPlayerController.RemoveToolTip(m_ControllerOnlyGameActionInputDisplayList[j]);
		}
		if (CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			for (int k = 0; k < m_ControllerOnlyGameActionInputDisplayList.Count; k++)
			{
				InteractionPlayerController.AddToolTip(m_ControllerOnlyGameActionInputDisplayList[k]);
			}
		}
	}

	public void SetPriceTagUI(UI_PriceTag PriceTagUI)
	{
		m_PriceTagUI = PriceTagUI;
		if ((bool)m_PriceTagUI)
		{
			m_PriceTagUI.Init(this);
		}
	}

	public UI_PriceTag GetPriceTagUI()
	{
		return m_PriceTagUI;
	}

	public override void OnMouseButtonUp()
	{
		if (!m_IsWarehouseTag)
		{
			m_ShelfCompartment.OnStartSetPrice();
			CSingleton<SetItemPriceScreen>.Instance.OpenScreen(m_PriceTagUI.GetItemType(), m_ShelfCompartment);
		}
	}

	public override void OnRightMouseButtonUp()
	{
		if ((bool)m_ShelfCompartment)
		{
			m_ShelfCompartment.RemoveLabel(playSound: true);
		}
	}

	public void SetVisibility(bool isVisible)
	{
		base.gameObject.SetActive(isVisible);
		m_PriceTagUI.gameObject.SetActive(isVisible);
	}

	public void SetItemImage(EItemType itemType)
	{
		m_PriceTagUI.SetItemImage(itemType);
	}

	public void SetAmountText(int amount)
	{
		m_PriceTagUI.SetAmountText(amount);
	}

	public void SetPriceText(float price)
	{
		m_PriceTagUI.SetPriceText(price);
	}

	public void RefreshPriceText()
	{
		m_PriceTagUI.RefreshPriceText();
	}

	public void SetPriceTagScale(float scale)
	{
		m_PriceTagUI.transform.localScale = Vector3.one * scale;
	}

	public void SetIgnoreCull(bool ignoreCull)
	{
		m_PriceTagUI.m_IgnoreCull = ignoreCull;
		if (ignoreCull)
		{
			m_PriceTagUI.m_UIGrp.gameObject.SetActive(value: true);
		}
	}

	public void SetPriceChecked(bool isPriceSet)
	{
		m_IsPriceSet = isPriceSet;
	}

	public bool GetIsPriceSet()
	{
		return m_IsPriceSet;
	}

	public Shelf GetShelf()
	{
		if ((bool)m_ShelfCompartment && !m_IsWarehouseTag)
		{
			return m_ShelfCompartment.GetShelf();
		}
		return null;
	}

	public WarehouseShelf GetWarehouseShelf()
	{
		if ((bool)m_ShelfCompartment && m_IsWarehouseTag)
		{
			return m_ShelfCompartment.GetWarehouseShelf();
		}
		return null;
	}

	public bool IsBigBox()
	{
		if ((bool)m_ShelfCompartment && m_IsWarehouseTag)
		{
			return m_ShelfCompartment.CheckBoxType(isBigBox: true);
		}
		return false;
	}

	public EItemType GetCompartmentItemType()
	{
		if ((bool)m_ShelfCompartment)
		{
			return m_ShelfCompartment.GetItemType();
		}
		return EItemType.None;
	}
}
