using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureShopPanelUI : UIElementBase
{
	public Image m_Image;

	public TextMeshProUGUI m_NameText;

	public TextMeshProUGUI m_PriceText;

	public TextMeshProUGUI m_LevelRequirementText;

	public GameObject m_CompanyTitle;

	public GameObject m_LockPurchaseBtn;

	public GameObject m_PrologueUIGrp;

	private FurnitureShopUIScreen m_FurnitureShopUIScreen;

	private int m_Index;

	private int m_LevelRequired;

	private string m_LevelRequirementString = "";

	public void Init(FurnitureShopUIScreen furnitureShopUIScreen, int index)
	{
		m_FurnitureShopUIScreen = furnitureShopUIScreen;
		m_Index = index;
		FurniturePurchaseData furniturePurchaseData = InventoryBase.GetFurniturePurchaseData(index);
		if (CSingleton<CGameManager>.Instance.m_IsPrologue && index > 5)
		{
			m_UIGrp.SetActive(value: false);
			m_PrologueUIGrp.SetActive(value: true);
		}
		if (m_LevelRequirementString == "")
		{
			m_LevelRequirementString = m_LevelRequirementText.text;
		}
		m_LevelRequired = furniturePurchaseData.levelRequirement;
		if (CPlayerData.m_ShopLevel + 1 >= m_LevelRequired)
		{
			m_LevelRequirementText.gameObject.SetActive(value: false);
			m_CompanyTitle.gameObject.SetActive(value: true);
			m_LockPurchaseBtn.gameObject.SetActive(value: false);
		}
		else
		{
			m_LevelRequirementText.text = LocalizationManager.GetTranslation(m_LevelRequirementString).Replace("XXX", m_LevelRequired.ToString());
			m_LevelRequirementText.gameObject.SetActive(value: true);
			m_CompanyTitle.gameObject.SetActive(value: false);
			m_LockPurchaseBtn.gameObject.SetActive(value: true);
		}
		m_Image.sprite = furniturePurchaseData.icon;
		m_NameText.text = furniturePurchaseData.GetName();
		m_PriceText.text = GameInstance.GetPriceString(furniturePurchaseData.price);
	}

	public override void OnPressButton()
	{
		if (CPlayerData.m_ShopLevel + 1 >= m_LevelRequired)
		{
			m_FurnitureShopUIScreen.OnPressPanelUIButton(m_Index);
		}
	}
}
