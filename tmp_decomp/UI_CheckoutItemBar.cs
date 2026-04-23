using TMPro;
using UnityEngine;

public class UI_CheckoutItemBar : MonoBehaviour
{
	public TextMeshProUGUI m_NameText;

	public TextMeshProUGUI m_PriceText;

	public TextMeshProUGUI m_UnitText;

	public TextMeshProUGUI m_TotalText;

	private double m_UnitPrice;

	private double m_TotalPrice;

	public void SetItemName(string name, double value)
	{
		m_NameText.text = name;
		m_UnitPrice = value;
		m_TotalPrice = value;
		m_UnitText.text = 1.ToString();
		m_PriceText.text = GameInstance.GetPriceString(m_UnitPrice);
		m_TotalText.text = m_PriceText.text;
	}

	public void AddScannedItem(int itemCount)
	{
		m_UnitText.text = itemCount.ToString();
		m_TotalPrice = GameInstance.GetConvertedCurrencyPrice(m_UnitPrice) * (double)itemCount;
		m_TotalPrice /= GameInstance.GetCurrencyConversionRate();
		m_TotalText.text = GameInstance.GetPriceString(m_TotalPrice);
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected void OnMoneyCurrencyUpdated(CEventPlayer_OnMoneyCurrencyUpdated evt)
	{
		m_PriceText.text = GameInstance.GetPriceString(m_UnitPrice);
		m_TotalText.text = GameInstance.GetPriceString(m_TotalPrice);
	}
}
