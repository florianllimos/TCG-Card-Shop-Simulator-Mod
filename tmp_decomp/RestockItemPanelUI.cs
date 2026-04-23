using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestockItemPanelUI : UIElementBase
{
	public Image m_ItemImage;

	public Image m_ItemImage2;

	public TextMeshProUGUI m_ItemNameText;

	public TextMeshProUGUI m_AmountText;

	public TextMeshProUGUI m_UnitPriceText;

	public TextMeshProUGUI m_TotalPriceText;

	public Animation m_PopupAnim;

	public Image m_ItemImageB;

	public Image m_ItemImage2B;

	public TextMeshProUGUI m_ItemNameTextB;

	public TextMeshProUGUI m_LicensePriceText;

	public TextMeshProUGUI m_LevelRequirementText;

	public GameObject m_LockPurchaseBtn;

	public GameObject m_LicenseUIGrp;

	public GameObject m_PrologueUIGrp;

	private string m_LevelRequirementString = "";

	private RestockItemScreen m_RestockItemScreen;

	private int m_Index;

	private int m_Amount;

	private int m_LevelRequired;

	private float m_UnitPrice;

	private float m_TotalPrice;

	private float m_LicensePrice;

	private EItemType m_ItemType = EItemType.None;

	public void Init(RestockItemScreen restockItemScreen, int index)
	{
		if (index < 0)
		{
			m_LicenseUIGrp.SetActive(value: false);
			m_UIGrp.SetActive(value: false);
			return;
		}
		m_RestockItemScreen = restockItemScreen;
		m_Index = index;
		RestockData restockData = InventoryBase.GetRestockData(index);
		if (CSingleton<CGameManager>.Instance.m_IsPrologue && !restockData.prologueShow)
		{
			m_LicenseUIGrp.SetActive(value: false);
			m_UIGrp.SetActive(value: false);
			m_PrologueUIGrp.SetActive(value: true);
		}
		else
		{
			m_PrologueUIGrp.SetActive(value: false);
		}
		m_Amount = restockData.amount;
		m_ItemType = restockData.itemType;
		ItemData itemData = InventoryBase.GetItemData(m_ItemType);
		m_ItemImage.sprite = itemData.icon;
		m_ItemImage2.sprite = m_ItemImage.sprite;
		if (restockData.isBigBox)
		{
			m_ItemImage.enabled = true;
			m_ItemImage2.enabled = true;
		}
		else
		{
			m_ItemImage.enabled = true;
			m_ItemImage2.enabled = false;
		}
		if (restockData.ignoreDoubleImage)
		{
			m_ItemImage2.enabled = false;
		}
		m_ItemImageB.sprite = m_ItemImage.sprite;
		m_ItemImage2B.sprite = m_ItemImage2.sprite;
		m_ItemImageB.enabled = m_ItemImage.enabled;
		m_ItemImage2B.enabled = m_ItemImage2.enabled;
		m_Amount = RestockManager.GetMaxItemCountInBox(m_ItemType, restockData.isBigBox);
		m_UnitPrice = CPlayerData.GetItemCost(m_ItemType);
		m_TotalPrice = m_UnitPrice * (float)m_Amount;
		m_LicensePrice = restockData.licensePrice;
		m_LevelRequired = restockData.licenseShopLevelRequired;
		m_ItemNameText.text = itemData.GetName() + " (" + m_Amount + ")";
		m_AmountText.text = "Qty : " + m_Amount;
		m_UnitPriceText.text = GameInstance.GetPriceString(m_UnitPrice);
		m_TotalPriceText.text = GameInstance.GetPriceString(m_TotalPrice);
		if (CPlayerData.GetIsItemLicenseUnlocked(m_Index))
		{
			m_LicenseUIGrp.SetActive(value: false);
			m_UIGrp.SetActive(value: true);
			return;
		}
		m_ItemNameTextB.text = m_ItemNameText.text + " " + LocalizationManager.GetTranslation("License");
		m_LicensePriceText.text = GameInstance.GetPriceString(m_LicensePrice);
		if (m_LevelRequirementString == "")
		{
			m_LevelRequirementString = m_LevelRequirementText.text;
		}
		if (CPlayerData.m_ShopLevel + 1 >= m_LevelRequired)
		{
			m_LevelRequirementText.gameObject.SetActive(value: false);
			m_LockPurchaseBtn.gameObject.SetActive(value: false);
		}
		else
		{
			m_LevelRequirementText.text = LocalizationManager.GetTranslation(m_LevelRequirementString).Replace("XXX", m_LevelRequired.ToString());
			m_LevelRequirementText.gameObject.SetActive(value: true);
			m_LockPurchaseBtn.gameObject.SetActive(value: true);
		}
		if (restockData.isHideItemUntilUnlocked)
		{
			m_ItemNameTextB.text = "??? " + LocalizationManager.GetTranslation("License");
			m_ItemImageB.sprite = InventoryBase.GetQuestionMarkSprite();
			m_ItemImageB.enabled = true;
			m_ItemImage2B.enabled = false;
		}
		m_LicenseUIGrp.SetActive(value: true);
		m_UIGrp.SetActive(value: false);
	}

	public float GetTotalPrice()
	{
		return m_TotalPrice;
	}

	public float GetIndex()
	{
		return m_Index;
	}

	public override void OnPressButton()
	{
		m_RestockItemScreen.OnPressAddToCartButton(m_Index);
	}

	public void OnPressPurchaseButton()
	{
		if (CPlayerData.m_ShopLevel + 1 >= m_LevelRequired)
		{
			if (CPlayerData.m_CoinAmountDouble >= (double)m_LicensePrice)
			{
				PriceChangeManager.AddTransaction(0f - m_LicensePrice, ETransactionType.PayRestockLicense, m_Index);
				CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(m_LicensePrice));
				CPlayerData.SetUnlockItemLicense(m_Index);
				m_LicenseUIGrp.SetActive(value: false);
				m_UIGrp.SetActive(value: true);
				m_PopupAnim.Play();
				CPlayerData.m_GameReportDataCollect.upgradeCost -= m_LicensePrice;
				CPlayerData.m_GameReportDataCollectPermanent.upgradeCost -= m_LicensePrice;
				AchievementManager.OnItemLicenseUnlocked(m_ItemType);
				GameInstance.m_IsItemLicenseUnlocked = true;
				SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
				if (m_ItemType == EItemType.BasicCardBox)
				{
					TutorialManager.AddTaskValue(ETutorialTaskCondition.UnlockBasicCardBox, 1f);
				}
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			}
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShopLevelNotEnough);
		}
	}
}
