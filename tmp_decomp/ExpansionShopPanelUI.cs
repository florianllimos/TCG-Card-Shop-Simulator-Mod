using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

public class ExpansionShopPanelUI : UIElementBase
{
	public TextMeshProUGUI m_NameText;

	public TextMeshProUGUI m_PriceText;

	public TextMeshProUGUI m_LevelRequirementText;

	public GameObject m_LockPurchaseBtn;

	public GameObject m_PurchasedBtn;

	public GameObject m_PrologueUIGrp;

	public List<GameObject> m_ShopB_EnableList;

	private ExpansionShopUIScreen m_ExpansionShopUIScreen;

	private bool m_UnlockPreviousFirst;

	private bool m_IsShopB;

	private int m_Index;

	private int m_LevelRequired;

	private float m_UpgradeCost;

	private string m_LevelRequirementString = "";

	public void Init(ExpansionShopUIScreen expansionShopUIScreen, int index, bool isShopB)
	{
		m_ExpansionShopUIScreen = expansionShopUIScreen;
		m_Index = index;
		m_IsShopB = isShopB;
		for (int i = 0; i < m_ShopB_EnableList.Count; i++)
		{
			m_ShopB_EnableList[i].SetActive(m_IsShopB);
		}
		if (!isShopB && CSingleton<CGameManager>.Instance.m_IsPrologue && index > 3)
		{
			m_UIGrp.SetActive(value: false);
			m_PrologueUIGrp.SetActive(value: true);
		}
		else if (isShopB && CSingleton<CGameManager>.Instance.m_IsPrologue && index > -1)
		{
			m_UIGrp.SetActive(value: false);
			m_PrologueUIGrp.SetActive(value: true);
		}
		int num = Mathf.Clamp(index * 2 + 1, 2, 60);
		int num2 = index / 4 * 2;
		num += num2;
		if (isShopB)
		{
			num = 20 + index * 5 + index / 4 * 10;
		}
		if (m_LevelRequirementString == "")
		{
			m_LevelRequirementString = m_LevelRequirementText.text;
		}
		bool flag = false;
		m_UnlockPreviousFirst = false;
		int num3 = CPlayerData.m_UnlockRoomCount;
		if (m_IsShopB)
		{
			num3 = CPlayerData.m_UnlockWarehouseRoomCount;
		}
		if (num3 > m_Index)
		{
			m_PurchasedBtn.gameObject.SetActive(value: true);
			m_LockPurchaseBtn.gameObject.SetActive(value: false);
			flag = true;
		}
		else if (num3 == index)
		{
			m_PurchasedBtn.gameObject.SetActive(value: false);
			m_LockPurchaseBtn.gameObject.SetActive(value: false);
		}
		else
		{
			m_PurchasedBtn.gameObject.SetActive(value: false);
			m_LockPurchaseBtn.gameObject.SetActive(value: true);
			m_UnlockPreviousFirst = true;
		}
		m_LevelRequired = num;
		if (CPlayerData.m_ShopLevel + 1 >= m_LevelRequired || flag)
		{
			m_LevelRequirementText.gameObject.SetActive(value: false);
			if (!m_UnlockPreviousFirst)
			{
				m_LockPurchaseBtn.gameObject.SetActive(value: false);
			}
		}
		else
		{
			m_LevelRequirementText.text = LocalizationManager.GetTranslation(m_LevelRequirementString).Replace("XXX", m_LevelRequired.ToString());
			m_LevelRequirementText.gameObject.SetActive(value: true);
			m_LockPurchaseBtn.gameObject.SetActive(value: true);
		}
		m_NameText.text = LocalizationManager.GetTranslation("Shop Expansion") + " " + (index + 1);
		m_UpgradeCost = CPlayerData.GetUnlockShopRoomCost(index);
		if (isShopB)
		{
			m_UpgradeCost = CPlayerData.GetUnlockWarehouseRoomCost(index);
		}
		m_PriceText.text = GameInstance.GetPriceString(m_UpgradeCost);
	}

	public override void OnPressButton()
	{
		int num = CPlayerData.m_UnlockRoomCount;
		if (m_IsShopB)
		{
			num = CPlayerData.m_UnlockWarehouseRoomCount;
		}
		if (num > m_Index)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AlreadyPurchased);
		}
		else if (m_UnlockPreviousFirst)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.UnlockPreviousRoomExpansionFirst);
		}
		else if (CPlayerData.m_ShopLevel + 1 < m_LevelRequired)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShopLevelNotEnough);
		}
		else if (CPlayerData.m_CoinAmountDouble >= (double)m_UpgradeCost)
		{
			m_ExpansionShopUIScreen.EvaluateCartCheckout(m_UpgradeCost, m_Index, m_IsShopB);
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
		}
	}
}
