using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PriceTag : MonoBehaviour
{
	public Transform m_UIGrp;

	public TextMeshProUGUI m_PriceText;

	public TextMeshProUGUI m_AmountText;

	public Image m_Icon;

	public Image m_BrightnessImage;

	public Image m_BarcodeImage;

	private EItemType m_ItemType = EItemType.None;

	private EObjectType m_ObjectType = EObjectType.None;

	public bool m_IsPriceZero;

	public bool m_IgnoreCull;

	public GameObject m_PriceZeroWarningIndicator;

	private CardData m_CardData;

	private float m_CurrentPrice;

	private InteractablePriceTag m_InteractablePriceTag;

	private InteractableCardPriceTag m_InteractableCardPriceTag;

	public void Init(InteractablePriceTag interactablePriceTag)
	{
		m_InteractablePriceTag = interactablePriceTag;
		if ((bool)m_BarcodeImage)
		{
			m_BarcodeImage.enabled = CPlayerData.m_IsScannerRestockUnlocked;
		}
	}

	public void InitCard(InteractableCardPriceTag interactableCardPriceTag)
	{
		m_InteractableCardPriceTag = interactableCardPriceTag;
	}

	public void SetItemImage(EItemType itemType)
	{
		m_ItemType = itemType;
		if (itemType == EItemType.None)
		{
			m_Icon.sprite = null;
		}
		else
		{
			m_Icon.sprite = InventoryBase.GetItemData(itemType).icon;
		}
	}

	public void SetObjectImage(EObjectType objectType)
	{
		m_ObjectType = objectType;
		if (objectType == EObjectType.None)
		{
			m_Icon.sprite = null;
		}
		else
		{
			m_Icon.sprite = InventoryBase.GetFurniturePurchaseData(objectType).icon;
		}
	}

	public void SetAmountText(int amount)
	{
		m_AmountText.text = amount.ToString();
	}

	public void SetPriceText(float price)
	{
		m_CurrentPrice = price;
		m_PriceText.text = GameInstance.GetPriceString(price, useDashAsZero: true);
		if (price <= 0f)
		{
			m_IsPriceZero = true;
			if ((bool)m_PriceZeroWarningIndicator)
			{
				m_PriceZeroWarningIndicator.SetActive(value: true);
			}
		}
		else
		{
			m_IsPriceZero = false;
			if ((bool)m_PriceZeroWarningIndicator)
			{
				m_PriceZeroWarningIndicator.SetActive(value: false);
			}
		}
	}

	public void RefreshPriceText()
	{
		SetPriceText(m_CurrentPrice);
	}

	public void SetBrightness(float brightness)
	{
		Color color = m_BrightnessImage.color;
		color.a = (1f - brightness) * 0.975f;
		m_BrightnessImage.color = color;
	}

	public EItemType GetItemType()
	{
		return m_ItemType;
	}

	public void SetCardData(CardData cardData)
	{
		m_CardData = cardData;
	}

	public CardData GetCardData()
	{
		return m_CardData;
	}

	private void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_ItemPriceChanged>(CPlayer_OnItemPriceChanged);
			CEventManager.AddListener<CEventPlayer_CardPriceChanged>(CPlayer_OnCardPriceChanged);
			CEventManager.AddListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			if ((bool)m_BarcodeImage)
			{
				CEventManager.AddListener<CEventPlayer_ScannerRestockUnlocked>(OnScannerRestockUnlocked);
			}
		}
	}

	private void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_ItemPriceChanged>(CPlayer_OnItemPriceChanged);
			CEventManager.RemoveListener<CEventPlayer_CardPriceChanged>(CPlayer_OnCardPriceChanged);
			CEventManager.RemoveListener<CEventPlayer_OnDayStarted>(OnDayStarted);
		}
	}

	private void CPlayer_OnItemPriceChanged(CEventPlayer_ItemPriceChanged evt)
	{
		if (m_ItemType == evt.m_ItemType)
		{
			SetPriceText(evt.m_Price);
			SetPriceChecked(isPriceSet: true);
		}
	}

	private void CPlayer_OnCardPriceChanged(CEventPlayer_CardPriceChanged evt)
	{
		if (m_CardData == null || m_CardData.monsterType == EMonsterType.None || CPlayerData.GetCardSaveIndex(m_CardData) != CPlayerData.GetCardSaveIndex(evt.m_CardData) || evt.m_CardData.expansionType != m_CardData.expansionType || evt.m_CardData.cardGrade != m_CardData.cardGrade)
		{
			return;
		}
		if (evt.m_CardData.expansionType == ECardExpansionType.Ghost)
		{
			if (m_CardData.isDestiny == evt.m_CardData.isDestiny)
			{
				SetPriceText(evt.m_Price);
				SetPriceChecked(isPriceSet: true);
			}
		}
		else
		{
			SetPriceText(evt.m_Price);
			SetPriceChecked(isPriceSet: true);
		}
	}

	private void SetPriceChecked(bool isPriceSet)
	{
		if ((bool)m_InteractableCardPriceTag)
		{
			m_InteractableCardPriceTag.SetPriceChecked(isPriceSet);
		}
		else if ((bool)m_InteractablePriceTag)
		{
			m_InteractablePriceTag.SetPriceChecked(isPriceSet);
		}
	}

	private void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		SetPriceChecked(isPriceSet: false);
	}

	private void OnScannerRestockUnlocked(CEventPlayer_ScannerRestockUnlocked evt)
	{
		if ((bool)m_BarcodeImage)
		{
			m_BarcodeImage.enabled = CPlayerData.m_IsScannerRestockUnlocked;
		}
		CEventManager.RemoveListener<CEventPlayer_ScannerRestockUnlocked>(OnScannerRestockUnlocked);
	}
}
