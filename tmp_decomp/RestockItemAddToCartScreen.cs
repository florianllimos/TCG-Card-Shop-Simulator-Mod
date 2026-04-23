using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestockItemAddToCartScreen : UIScreenBase
{
	public Image m_ItemImage;

	public Image m_ItemImage2;

	public TextMeshProUGUI m_ItemNameText;

	public TextMeshProUGUI m_AmountText;

	public TextMeshProUGUI m_UnitPriceText;

	public TextMeshProUGUI m_TotalPriceText;

	public TextMeshProUGUI m_PurchaseBoxCountText;

	public TextMeshProUGUI m_TotalCheckoutPriceText;

	public TMP_InputField m_SetAmountInput;

	public TextMeshProUGUI m_SetAmountInputDisplay;

	private RestockItemScreen m_RestockItemScreen;

	private RestockItemPanelUI m_RestockItemPanelUI;

	private int m_Index;

	private int m_PurchaseBoxCount = 1;

	private int m_PurchaseBoxCountLimit = 500;

	private float m_TotalCheckoutPrice;

	public void UpdateData(RestockItemScreen restockItemScreen, int index)
	{
		m_Index = index;
		m_RestockItemScreen = restockItemScreen;
		m_RestockItemPanelUI = m_RestockItemScreen.GetRestockItemPanelUI(index);
		m_ItemImage.sprite = m_RestockItemPanelUI.m_ItemImage.sprite;
		m_ItemImage2.sprite = m_RestockItemPanelUI.m_ItemImage2.sprite;
		m_ItemImage2.enabled = m_RestockItemPanelUI.m_ItemImage2.enabled;
		m_ItemNameText.text = m_RestockItemPanelUI.m_ItemNameText.text;
		m_AmountText.text = m_RestockItemPanelUI.m_AmountText.text;
		m_UnitPriceText.text = m_RestockItemPanelUI.m_UnitPriceText.text;
		m_TotalPriceText.text = m_RestockItemPanelUI.m_TotalPriceText.text;
		m_PurchaseBoxCount = 1;
		m_SetAmountInputDisplay.gameObject.SetActive(value: false);
		EvaluateTotalCheckoutPrice();
	}

	public void OnPressAddCount()
	{
		m_PurchaseBoxCount++;
		if (m_PurchaseBoxCount >= m_PurchaseBoxCountLimit)
		{
			m_PurchaseBoxCount = m_PurchaseBoxCountLimit;
		}
		EvaluateTotalCheckoutPrice();
	}

	public void OnPressMinusCount()
	{
		m_PurchaseBoxCount--;
		if (m_PurchaseBoxCount <= 0)
		{
			m_PurchaseBoxCount = 0;
		}
		EvaluateTotalCheckoutPrice();
	}

	private void EvaluateTotalCheckoutPrice()
	{
		m_PurchaseBoxCountText.text = m_PurchaseBoxCount.ToString();
		m_TotalCheckoutPrice = m_RestockItemPanelUI.GetTotalPrice() * (float)m_PurchaseBoxCount;
		m_TotalCheckoutPriceText.text = GameInstance.GetPriceString(m_TotalCheckoutPrice);
	}

	public void OnPressAddToCartForCheckout()
	{
		if (m_PurchaseBoxCount > 0)
		{
			if (m_RestockItemScreen.HasEnoughCartSlot())
			{
				m_RestockItemScreen.AddToCartForCheckout(m_Index, m_PurchaseBoxCount);
				CloseScreen();
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CartTooManyItem);
			}
		}
	}

	public void OnInputChanged(string text)
	{
		m_PurchaseBoxCount = Mathf.FloorToInt(GameInstance.GetInvariantCultureDecimal(text));
		if (m_PurchaseBoxCount <= 0)
		{
			m_PurchaseBoxCount = 0;
		}
		if (m_PurchaseBoxCount >= m_PurchaseBoxCountLimit)
		{
			m_PurchaseBoxCount = m_PurchaseBoxCountLimit;
		}
		m_SetAmountInputDisplay.text = m_PurchaseBoxCount.ToString();
		m_SetAmountInputDisplay.gameObject.SetActive(value: true);
		m_PurchaseBoxCountText.gameObject.SetActive(value: false);
	}

	public void OnInputTextSelected(string text)
	{
		m_SetAmountInputDisplay.gameObject.SetActive(value: true);
		m_SetAmountInput.text = m_PurchaseBoxCount.ToString();
		m_PurchaseBoxCountText.gameObject.SetActive(value: false);
	}

	public void OnInputTextUpdated(string text)
	{
		m_PurchaseBoxCount = Mathf.FloorToInt(GameInstance.GetInvariantCultureDecimal(text));
		if (m_PurchaseBoxCount <= 0)
		{
			m_PurchaseBoxCount = 0;
		}
		if (m_PurchaseBoxCount >= m_PurchaseBoxCountLimit)
		{
			m_PurchaseBoxCount = m_PurchaseBoxCountLimit;
		}
		m_SetAmountInputDisplay.text = m_PurchaseBoxCount.ToString();
		m_PurchaseBoxCountText.text = m_PurchaseBoxCount.ToString();
		m_SetAmountInputDisplay.gameObject.SetActive(value: false);
		m_PurchaseBoxCountText.gameObject.SetActive(value: true);
		EvaluateTotalCheckoutPrice();
	}
}
