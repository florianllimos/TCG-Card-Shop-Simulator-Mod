using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CashCounterScreen : MonoBehaviour
{
	public FollowObject m_FollowObject;

	public GameObject m_ItemListGrp;

	public GameObject m_ScaledUpTotalTextGrp;

	public TextMeshProUGUI m_ScaledUpTotalText;

	public TextMeshProUGUI m_TotalItemListCostText;

	public Transform m_CheckoutBarSliderGrp;

	public List<UI_CheckoutItemBar> m_CheckoutItemBarList;

	private Dictionary<EItemType, int> m_ItemScannedListDict = new Dictionary<EItemType, int>();

	private List<EItemType> m_ItemTypeList = new List<EItemType>();

	public GameObject m_GivingChangeGrp;

	public Animation m_MoneyGuideFollowDrawerAnim;

	public TextMeshProUGUI m_CustomerGiveAmountText;

	public TextMeshProUGUI m_TotalItemCostText;

	public TextMeshProUGUI m_ChangeToGiveAmountText;

	public TextMeshProUGUI m_ChangeGivenAmountText;

	public List<TextMeshProUGUI> m_ChangeGuideTextList;

	public Color m_ChangeReadyColor;

	public Color m_ChangeNotReadyColor;

	private float m_MouseWheelScrollSpeed = 10f;

	private float m_LerpPosX;

	private float m_PosX;

	private float m_MinPosX;

	private float m_MaxPosX = 5000f;

	private double m_TotalItemCost;

	private int m_ActiveBarCount;

	private InteractableCashierCounter m_CashierCounter;

	private EMoneyCurrencyType m_CurrentMoneyCurrencyType;

	private void Awake()
	{
		ResetCounter();
	}

	public void Init(InteractableCashierCounter cashierCounter)
	{
		m_CashierCounter = cashierCounter;
	}

	private void Update()
	{
		m_PosX -= Input.mouseScrollDelta.y * m_MouseWheelScrollSpeed;
		m_PosX = Mathf.Clamp(m_PosX, m_MinPosX, m_MaxPosX);
		m_LerpPosX = Mathf.Lerp(m_LerpPosX, m_PosX, Time.deltaTime * CSingleton<TouchManager>.Instance.m_LerpSpeed);
		m_CheckoutBarSliderGrp.transform.localPosition = new Vector3(0f, m_LerpPosX, 0f);
	}

	public void OnItemScanned(double value, EItemType itemType, double totalItemCost)
	{
		if (itemType == EItemType.None)
		{
			return;
		}
		if (m_ItemScannedListDict.ContainsKey(itemType))
		{
			m_ItemScannedListDict[itemType]++;
			int index = m_ItemTypeList.IndexOf(itemType);
			m_CheckoutItemBarList[index].AddScannedItem(m_ItemScannedListDict[itemType]);
		}
		else
		{
			m_ItemScannedListDict.Add(itemType, 1);
			m_ItemTypeList.Add(itemType);
			for (int i = 0; i < m_CheckoutItemBarList.Count; i++)
			{
				if (!m_CheckoutItemBarList[i].gameObject.activeSelf)
				{
					string text = InventoryBase.GetItemData(itemType).GetName();
					m_CheckoutItemBarList[i].SetItemName(text, value);
					m_CheckoutItemBarList[i].gameObject.SetActive(value: true);
					m_ActiveBarCount++;
					m_MaxPosX = Mathf.Clamp((float)(m_ActiveBarCount - 8) * 7.5f, 0f, 240f);
					break;
				}
			}
		}
		m_TotalItemCost = totalItemCost;
		m_TotalItemListCostText.text = GameInstance.GetPriceString(m_TotalItemCost);
		m_ScaledUpTotalText.text = m_TotalItemListCostText.text;
	}

	public void OnCardScanned(double value, CardData cardData, double totalItemCost)
	{
		if (cardData == null || cardData.monsterType == EMonsterType.None)
		{
			return;
		}
		for (int i = 0; i < m_CheckoutItemBarList.Count; i++)
		{
			if (!m_CheckoutItemBarList[i].gameObject.activeSelf)
			{
				string text = InventoryBase.GetMonsterData(cardData.monsterType).GetName() + " - " + CPlayerData.GetFullCardTypeName(cardData, ignoreRarity: true);
				m_CheckoutItemBarList[i].SetItemName(text, value);
				m_CheckoutItemBarList[i].gameObject.SetActive(value: true);
				m_ActiveBarCount++;
				m_MaxPosX = Mathf.Clamp((float)(m_ActiveBarCount - 8) * 7.5f, 0f, 240f);
				m_ItemTypeList.Add(EItemType.None);
				break;
			}
		}
		m_TotalItemCost = totalItemCost;
		m_TotalItemListCostText.text = GameInstance.GetPriceString(m_TotalItemCost);
		m_ScaledUpTotalText.text = m_TotalItemListCostText.text;
	}

	public void OnStartGivingChange()
	{
		m_ItemListGrp.SetActive(value: false);
		m_GivingChangeGrp.SetActive(value: true);
		m_MoneyGuideFollowDrawerAnim.Play();
		if (m_CurrentMoneyCurrencyType != CSingleton<CGameManager>.Instance.m_CurrencyType)
		{
			m_CurrentMoneyCurrencyType = CSingleton<CGameManager>.Instance.m_CurrencyType;
			UpdateChangeGuideText();
		}
	}

	private void UpdateChangeGuideText()
	{
		for (int i = 0; i < m_ChangeGuideTextList.Count; i++)
		{
			m_ChangeGuideTextList[i].text = GameInstance.GetPriceString(m_CashierCounter.m_InteractableCounterMoneyChangeList[i].m_ValueDouble, useDashAsZero: false, useCurrencySymbol: true, useCentSymbol: true, "F0");
		}
	}

	public void UpdateMoneyChangeAmount(bool isChangeReady, double customerPaidAmount, double totalScannedItemCost, double currentMoneyChangeValue)
	{
		m_CustomerGiveAmountText.text = GameInstance.GetPriceString(customerPaidAmount);
		m_TotalItemCostText.text = GameInstance.GetPriceString(totalScannedItemCost);
		m_ChangeToGiveAmountText.text = GameInstance.GetPriceString(customerPaidAmount - totalScannedItemCost);
		m_ChangeGivenAmountText.text = GameInstance.GetPriceString(currentMoneyChangeValue);
		if (isChangeReady)
		{
			m_ChangeGivenAmountText.color = m_ChangeReadyColor;
		}
		else
		{
			m_ChangeGivenAmountText.color = m_ChangeNotReadyColor;
		}
	}

	public void ShowScaledUpTotalCost()
	{
		m_ScaledUpTotalTextGrp.SetActive(value: true);
	}

	public void ResetCounter()
	{
		m_ItemListGrp.SetActive(value: true);
		m_GivingChangeGrp.SetActive(value: false);
		m_ScaledUpTotalTextGrp.SetActive(value: false);
		m_TotalItemCost = 0.0;
		m_TotalItemListCostText.text = GameInstance.GetPriceString(0f);
		UpdateMoneyChangeAmount(isChangeReady: false, 0.0, 0.0, 0.0);
		m_ItemTypeList.Clear();
		m_ItemScannedListDict.Clear();
		for (int i = 0; i < m_CheckoutItemBarList.Count; i++)
		{
			m_CheckoutItemBarList[i].gameObject.SetActive(value: false);
		}
		m_ActiveBarCount = 0;
		m_MaxPosX = 0f;
		m_CheckoutBarSliderGrp.transform.localPosition = Vector3.zero;
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
		if (m_CurrentMoneyCurrencyType != CSingleton<CGameManager>.Instance.m_CurrencyType)
		{
			m_CurrentMoneyCurrencyType = CSingleton<CGameManager>.Instance.m_CurrencyType;
			StartCoroutine(DelayUpdateCurrency());
			m_TotalItemListCostText.text = GameInstance.GetPriceString(m_TotalItemCost);
			m_ScaledUpTotalText.text = m_TotalItemListCostText.text;
		}
	}

	private IEnumerator DelayUpdateCurrency()
	{
		yield return new WaitForSeconds(0.01f);
		UpdateChangeGuideText();
	}

	public Dictionary<EItemType, int> GetItemScannedListDict()
	{
		return m_ItemScannedListDict;
	}
}
