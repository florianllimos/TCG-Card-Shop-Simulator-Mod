using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopDecoPanelUI : UIElementBase
{
	public Image m_Image;

	public TextMeshProUGUI m_NameText;

	public TextMeshProUGUI m_NameCenteredText;

	public TextMeshProUGUI m_OwnedText;

	public TextMeshProUGUI m_PriceText;

	public GameObject m_BuyBtnGrp;

	public GameObject m_BoughtBtnGrp;

	public GameObject m_PlaceBtnGrp;

	public GameObject m_EquippedBtnGrp;

	public GameObject m_EquippedBtnGrpB;

	public GameObject m_ChangeBtnGrp;

	public GameObject m_ChangeBtnGrpB;

	public GameObject m_ChangeBtnGrpB_Locked;

	public GameObject m_LockedBG;

	public Animation m_BuyBtnAnim;

	public Animation m_BoughtBtnAnim;

	public Animation m_EquippedBtnAnim;

	public Animation m_EquippedBtnAnimB;

	private bool m_IsShopWallFloorCeiling;

	private bool m_IsDecoUnlocked;

	private float m_Price = 100000f;

	private int m_OwnedDecoCount;

	private EDecoObject m_ItemType;

	private ShopDecoData m_ShopDecoData;

	private DecoPurchaseData m_DecoPurchaseData;

	private int m_ShopDecoIndex;

	private PlaceDecoUIScreen m_PlaceDecoUIScreen;

	private ShopBuyDecoUIScreen m_ShopBuyDecoUIScreen;

	public void Init(PlaceDecoUIScreen placeDecoUIScreen)
	{
		m_PlaceDecoUIScreen = placeDecoUIScreen;
	}

	public void InitShop(ShopBuyDecoUIScreen shopBuyDecoUIScreen)
	{
		m_ShopBuyDecoUIScreen = shopBuyDecoUIScreen;
	}

	public void InitShopWallFloorCeiling(ShopDecoData decoData, int decoIndex)
	{
		m_IsShopWallFloorCeiling = true;
		m_UIGrp.SetActive(value: true);
		m_ShopDecoData = decoData;
		m_ShopDecoIndex = decoIndex;
		m_NameText.text = m_ShopDecoData.GetName();
		m_NameCenteredText.text = m_NameText.text;
		m_Price = m_ShopDecoData.price;
		m_PriceText.text = GameInstance.GetPriceString(m_ShopDecoData.price);
		m_Image.sprite = m_ShopDecoData.icon;
		m_OwnedText.gameObject.SetActive(value: false);
		m_NameText.gameObject.SetActive(value: false);
		m_NameCenteredText.gameObject.SetActive(value: true);
		m_BuyBtnGrp.SetActive(value: false);
		m_BoughtBtnGrp.SetActive(value: false);
		m_PlaceBtnGrp.SetActive(value: false);
		m_EquippedBtnGrp.SetActive(value: false);
		m_EquippedBtnGrpB.SetActive(value: false);
		m_ChangeBtnGrp.SetActive(value: true);
		m_ChangeBtnGrpB.SetActive(CPlayerData.m_IsWarehouseRoomUnlocked);
		m_ChangeBtnGrpB_Locked.SetActive(!CPlayerData.m_IsWarehouseRoomUnlocked);
	}

	public void InitDecoObject(DecoPurchaseData decoPurchaseData, EDecoObject decoObject, int decoIndex)
	{
		m_IsShopWallFloorCeiling = false;
		m_UIGrp.SetActive(value: true);
		m_ItemType = decoObject;
		m_DecoPurchaseData = decoPurchaseData;
		m_ShopDecoIndex = decoIndex;
		m_NameText.text = m_DecoPurchaseData.GetName();
		m_Price = m_DecoPurchaseData.price;
		m_PriceText.text = GameInstance.GetPriceString(m_DecoPurchaseData.price);
		m_Image.sprite = m_DecoPurchaseData.icon;
		m_OwnedText.gameObject.SetActive(value: true);
		m_NameText.gameObject.SetActive(value: true);
		m_NameCenteredText.gameObject.SetActive(value: false);
		m_BuyBtnGrp.SetActive(value: false);
		m_BoughtBtnGrp.SetActive(value: false);
		m_PlaceBtnGrp.SetActive(value: true);
		m_EquippedBtnGrp.SetActive(value: false);
		m_EquippedBtnGrpB.SetActive(value: false);
		m_ChangeBtnGrp.SetActive(value: false);
		m_ChangeBtnGrpB.SetActive(value: false);
		m_ChangeBtnGrpB_Locked.SetActive(value: false);
		m_LockedBG.SetActive(value: false);
	}

	public void ShowPurchaseDecoButtonUI()
	{
		m_BuyBtnGrp.SetActive(value: true);
		m_BoughtBtnGrp.SetActive(value: false);
		m_PlaceBtnGrp.SetActive(value: false);
		m_EquippedBtnGrp.SetActive(value: false);
		m_EquippedBtnGrpB.SetActive(value: false);
		m_ChangeBtnGrp.SetActive(value: false);
		m_ChangeBtnGrpB.SetActive(value: false);
		m_ChangeBtnGrpB_Locked.SetActive(value: false);
		m_LockedBG.SetActive(value: false);
	}

	public override void OnPressButton()
	{
		SoundManager.GenericConfirm();
	}

	public void OnPressSwitchShopLotADeco()
	{
		SoundManager.GenericConfirm();
		m_PlaceDecoUIScreen.OnPressSwitchShopDeco(m_ShopDecoIndex, isShopLotB: false);
		m_EquippedBtnAnim.Play();
	}

	public void OnPressSwitchShopLotBDeco()
	{
		SoundManager.GenericConfirm();
		m_PlaceDecoUIScreen.OnPressSwitchShopDeco(m_ShopDecoIndex, isShopLotB: true);
		m_EquippedBtnAnimB.Play();
	}

	public void OnPressBuyDeco()
	{
		if (m_IsShopWallFloorCeiling)
		{
			m_ShopBuyDecoUIScreen.OnPressBuyShopDeco(m_ShopDecoIndex, m_Price);
			if (CPlayerData.m_CoinAmountDouble >= (double)m_Price)
			{
				m_BoughtBtnAnim.Play();
			}
		}
		else
		{
			m_ShopBuyDecoUIScreen.OnPressBuyShopDecoItem(m_ItemType, m_Price);
			if (CPlayerData.m_CoinAmountDouble >= (double)m_Price)
			{
				m_BuyBtnAnim.Play();
			}
		}
	}

	public void EvaluateShopDecoEquippedUIState(int shopDecoIndex, bool isShopLotB)
	{
		if (!m_IsDecoUnlocked)
		{
			return;
		}
		if (isShopLotB)
		{
			if (CPlayerData.m_IsWarehouseRoomUnlocked)
			{
				if (shopDecoIndex == m_ShopDecoIndex)
				{
					m_EquippedBtnGrpB.SetActive(value: true);
					m_ChangeBtnGrpB.SetActive(value: false);
					m_ChangeBtnGrpB_Locked.SetActive(value: false);
				}
				else
				{
					m_EquippedBtnGrpB.SetActive(value: false);
					m_ChangeBtnGrpB.SetActive(value: true);
				}
			}
			else
			{
				m_ChangeBtnGrpB.SetActive(value: false);
				m_ChangeBtnGrpB_Locked.SetActive(value: true);
			}
		}
		else if (shopDecoIndex == m_ShopDecoIndex)
		{
			m_EquippedBtnGrp.SetActive(value: true);
			m_ChangeBtnGrp.SetActive(value: false);
		}
		else
		{
			m_EquippedBtnGrp.SetActive(value: false);
			m_ChangeBtnGrp.SetActive(value: true);
		}
	}

	public void EvaluateShopDecoUnlockedState(int categoryIndex, int shopDecoIndex)
	{
		m_IsDecoUnlocked = false;
		switch (categoryIndex)
		{
		case 0:
			m_IsDecoUnlocked = CPlayerData.IsDecoWallUnlocked(shopDecoIndex);
			break;
		case 1:
			m_IsDecoUnlocked = CPlayerData.IsDecoFloorUnlocked(shopDecoIndex);
			break;
		case 2:
			m_IsDecoUnlocked = CPlayerData.IsDecoCeilingUnlocked(shopDecoIndex);
			break;
		}
		m_LockedBG.SetActive(!m_IsDecoUnlocked);
		if (!m_IsDecoUnlocked)
		{
			m_ChangeBtnGrp.SetActive(value: false);
			m_ChangeBtnGrpB.SetActive(value: false);
			m_ChangeBtnGrpB_Locked.SetActive(value: false);
		}
	}

	public void EvaluateShopDecoBoughtUIState(int categoryIndex, int shopDecoIndex)
	{
		m_IsDecoUnlocked = false;
		switch (categoryIndex)
		{
		case 0:
			m_IsDecoUnlocked = CPlayerData.IsDecoWallUnlocked(shopDecoIndex);
			break;
		case 1:
			m_IsDecoUnlocked = CPlayerData.IsDecoFloorUnlocked(shopDecoIndex);
			break;
		case 2:
			m_IsDecoUnlocked = CPlayerData.IsDecoCeilingUnlocked(shopDecoIndex);
			break;
		}
		if (m_IsDecoUnlocked)
		{
			m_BoughtBtnGrp.SetActive(value: true);
			m_BuyBtnGrp.SetActive(value: false);
		}
		else
		{
			m_BoughtBtnGrp.SetActive(value: false);
			m_BuyBtnGrp.SetActive(value: true);
		}
	}

	public void EvaluateOwnedDecoItemCount()
	{
		m_OwnedDecoCount = CPlayerData.GetDecoItemInventoryCount(m_ItemType);
		m_OwnedText.text = LocalizationManager.GetTranslation("Owned") + " : " + m_OwnedDecoCount;
	}

	public void OnPressPlaceDeco()
	{
		SoundManager.GenericConfirm();
		m_PlaceDecoUIScreen.StartPlaceDecoItem(m_ItemType);
	}

	public void OnPressAlreadyPurchasedButton()
	{
		NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AlreadyPurchased);
	}
}
