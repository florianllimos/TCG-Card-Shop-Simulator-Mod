using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RestockItemCheckoutScreen : GenericSliderScreen
{
	public List<RestockCheckoutItemBar> m_RestockCheckoutItemBarUIList;

	public TextMeshProUGUI m_SubtotalText;

	public TextMeshProUGUI m_DeliveryFeeText;

	public TextMeshProUGUI m_TotalPriceText;

	private RestockItemScreen m_RestockItemScreen;

	private float m_TotalCost;

	private float m_DeliveryFee;

	public List<float> m_TotalItemCostList = new List<float>();

	public void UpdateData(RestockItemScreen restockItemScreen, Dictionary<int, int> cartItemList, bool hasItemRemoved)
	{
		m_RestockItemScreen = restockItemScreen;
		for (int i = 0; i < m_RestockCheckoutItemBarUIList.Count; i++)
		{
			m_RestockCheckoutItemBarUIList[i].Init(m_RestockItemScreen);
			m_RestockCheckoutItemBarUIList[i].SetActive(isActive: false);
		}
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		int num = 0;
		foreach (KeyValuePair<int, int> cartItem in cartItemList)
		{
			list.Add(cartItem.Key);
			list2.Add(cartItem.Value);
			num += cartItem.Value;
		}
		float num2 = 0f;
		for (int j = 0; j < list.Count; j++)
		{
			m_RestockCheckoutItemBarUIList[j].UpdateData(list[j], list2[j]);
			m_RestockCheckoutItemBarUIList[j].SetActive(isActive: true);
			m_ScrollEndPosParent = m_RestockCheckoutItemBarUIList[j].gameObject;
			num2 += m_RestockCheckoutItemBarUIList[j].GetTotalPrice();
		}
		m_DeliveryFee = Mathf.Clamp(10 * num / 5, 5, 1000);
		if (num == 0)
		{
			m_DeliveryFee = 0f;
		}
		m_TotalCost = num2 + m_DeliveryFee;
		m_SubtotalText.text = GameInstance.GetPriceString(num2);
		m_DeliveryFeeText.text = GameInstance.GetPriceString(m_DeliveryFee);
		m_TotalPriceText.text = GameInstance.GetPriceString(m_TotalCost);
		m_RestockItemScreen.UpdateCartTotalCost(num2, num);
		if (hasItemRemoved && (bool)m_ControllerScreenUIExtension)
		{
			m_ControllerScreenUIExtension.EvaluateCtrlBtnActiveChanged();
		}
	}

	public void OnPressConfirmCheckout()
	{
		if (m_TotalCost <= 0f)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.NothingInCart);
		}
		else if (CPlayerData.m_CoinAmountDouble >= (double)m_TotalCost)
		{
			for (int i = 0; i < m_RestockCheckoutItemBarUIList.Count; i++)
			{
				if (m_RestockCheckoutItemBarUIList[i].IsActive())
				{
					EItemType itemType = m_RestockCheckoutItemBarUIList[i].GetItemType();
					int totalUnit = m_RestockCheckoutItemBarUIList[i].GetTotalUnit();
					float unitPrice = m_RestockCheckoutItemBarUIList[i].GetUnitPrice();
					CPlayerData.UpdateAverageItemCost(itemType, totalUnit, unitPrice);
					PriceChangeManager.AddTransaction(0f - (float)totalUnit * unitPrice, ETransactionType.Restock, (int)itemType, totalUnit);
				}
			}
			PriceChangeManager.AddTransaction(0f - m_DeliveryFee, ETransactionType.RestockDeliveryFee, 0);
			m_RestockItemScreen.EvaluateCartCheckout(m_TotalCost);
			CloseScreen();
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
		}
	}
}
