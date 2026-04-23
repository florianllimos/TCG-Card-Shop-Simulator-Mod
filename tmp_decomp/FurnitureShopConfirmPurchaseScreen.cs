using TMPro;
using UnityEngine.UI;

public class FurnitureShopConfirmPurchaseScreen : UIScreenBase
{
	public Image m_Image;

	public TextMeshProUGUI m_NameText;

	public TextMeshProUGUI m_DescriptionText;

	public TextMeshProUGUI m_PriceText;

	private FurnitureShopUIScreen m_FurnitureShopUIScreen;

	private int m_Index;

	private float m_TotalCost;

	public void UpdateData(FurnitureShopUIScreen furnitureShopUIScreen, int index)
	{
		m_FurnitureShopUIScreen = furnitureShopUIScreen;
		m_Index = index;
		FurniturePurchaseData furniturePurchaseData = InventoryBase.GetFurniturePurchaseData(index);
		m_TotalCost = furniturePurchaseData.price;
		m_Image.sprite = furniturePurchaseData.icon;
		m_NameText.text = furniturePurchaseData.GetName();
		m_DescriptionText.text = furniturePurchaseData.GetDescription();
		m_PriceText.text = GameInstance.GetPriceString(m_TotalCost);
	}

	public void OnPressConfirmCheckout()
	{
		if (m_TotalCost <= 0f)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.NothingInCart);
		}
		else if (CPlayerData.m_CoinAmountDouble >= (double)m_TotalCost)
		{
			m_FurnitureShopUIScreen.EvaluateCartCheckout(m_TotalCost, m_Index);
			CloseScreen();
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
		}
	}
}
