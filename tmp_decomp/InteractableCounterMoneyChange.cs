using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCounterMoneyChange : InteractableObject
{
	public List<CounterMoneyChangeCurrencyData> m_CounterMoneyChangeCurrencyDataList;

	public bool m_CanBeCoinOrBill;

	public bool m_IsCoin;

	public float m_Value = 1f;

	public double m_ValueDouble;

	public int m_Index;

	public Transform m_MoneyModel;

	public Transform m_MoneyModelInstance;

	public MeshRenderer m_MoneyMeshRenderer;

	public MeshRenderer m_MoneyInstanceMeshRenderer;

	public GameObject m_CoinMoneyMeshGrp;

	public List<MeshRenderer> m_CoinMoneyMeshRenderer;

	public MeshRenderer m_CoinMoneyInstanceMeshRenderer;

	public GameObject m_BillMoneyOutline;

	public GameObject m_CoinMoneyOutline;

	public InteractableCashierCounter m_CashierCounter;

	private int m_GivenAmount;

	private Vector3 m_PosYIndexOffset;

	private List<float> m_LerpTimerList = new List<float>();

	private List<bool> m_IsLerpingList = new List<bool>();

	private List<bool> m_IsLerpingBackList = new List<bool>();

	private List<Vector3> m_PosYOffsetList = new List<Vector3>();

	private List<Transform> m_MoneyModelInstanceList = new List<Transform>();

	private Vector3 m_PlaceMoneyLocation;

	public void UpdateCurrency()
	{
		CounterMoneyChangeCurrencyData counterMoneyChangeCurrencyData = null;
		for (int i = 0; i < m_CounterMoneyChangeCurrencyDataList.Count; i++)
		{
			if (m_CounterMoneyChangeCurrencyDataList[i].currencyType == CSingleton<CGameManager>.Instance.m_CurrencyType)
			{
				counterMoneyChangeCurrencyData = m_CounterMoneyChangeCurrencyDataList[i];
				break;
			}
		}
		m_IsCoin = counterMoneyChangeCurrencyData.isCoin;
		m_ValueDouble = (double)counterMoneyChangeCurrencyData.value / (double)GameInstance.GetCurrencyConversionRate();
		if (GameInstance.GetCurrencyConversionRate() > 1f)
		{
			m_ValueDouble = Math.Round(m_ValueDouble, 3, MidpointRounding.AwayFromZero);
		}
		else
		{
			m_ValueDouble = Math.Round(m_ValueDouble, 2, MidpointRounding.AwayFromZero);
		}
		if (m_IsCoin)
		{
			for (int j = 0; j < m_CoinMoneyMeshRenderer.Count; j++)
			{
				m_CoinMoneyMeshRenderer[j].material = InventoryBase.GetCurrencyMaterial(CSingleton<CGameManager>.Instance.m_CurrencyType);
			}
			m_CoinMoneyInstanceMeshRenderer.material = InventoryBase.GetCurrencyMaterial(CSingleton<CGameManager>.Instance.m_CurrencyType);
			if (m_CanBeCoinOrBill)
			{
				m_MoneyMeshRenderer.gameObject.SetActive(value: false);
				m_MoneyInstanceMeshRenderer.gameObject.SetActive(value: false);
				for (int k = 0; k < m_CoinMoneyMeshRenderer.Count; k++)
				{
					m_CoinMoneyMeshRenderer[k].gameObject.SetActive(value: true);
				}
				m_CoinMoneyInstanceMeshRenderer.gameObject.SetActive(value: true);
				m_CoinMoneyMeshGrp.SetActive(value: true);
				m_BillMoneyOutline.SetActive(value: false);
				m_CoinMoneyOutline.SetActive(value: true);
			}
		}
		else
		{
			m_MoneyMeshRenderer.material = InventoryBase.GetCurrencyMaterial(CSingleton<CGameManager>.Instance.m_CurrencyType);
			m_MoneyInstanceMeshRenderer.material = InventoryBase.GetCurrencyMaterial(CSingleton<CGameManager>.Instance.m_CurrencyType);
			if (m_CanBeCoinOrBill)
			{
				m_MoneyMeshRenderer.gameObject.SetActive(value: true);
				m_MoneyInstanceMeshRenderer.gameObject.SetActive(value: true);
				for (int l = 0; l < m_CoinMoneyMeshRenderer.Count; l++)
				{
					m_CoinMoneyMeshRenderer[l].gameObject.SetActive(value: false);
				}
				m_CoinMoneyInstanceMeshRenderer.gameObject.SetActive(value: false);
				m_CoinMoneyMeshGrp.SetActive(value: false);
				m_BillMoneyOutline.SetActive(value: true);
				m_CoinMoneyOutline.SetActive(value: false);
			}
		}
		if (m_IsCoin)
		{
			m_PosYIndexOffset = Vector3.zero;
		}
		else
		{
			m_PosYIndexOffset = Vector3.up * 0.0005f * m_Index * m_Index;
		}
	}

	protected override void Start()
	{
		base.Start();
		UpdateCurrency();
	}

	protected override void Update()
	{
		if (!m_CashierCounter.IsGivingChange())
		{
			return;
		}
		base.Update();
		for (int i = 0; i < m_IsLerpingList.Count; i++)
		{
			if (m_IsLerpingList[i])
			{
				m_LerpTimerList[i] += Time.deltaTime * 3f;
				m_MoneyModelInstanceList[i].position = Vector3.Lerp(base.transform.position, m_PlaceMoneyLocation + m_PosYOffsetList[i] + m_PosYIndexOffset, m_LerpTimerList[i]);
				if (m_LerpTimerList[i] >= 1f)
				{
					m_LerpTimerList[i] = 1f;
					m_IsLerpingList[i] = false;
				}
			}
			else if (m_IsLerpingBackList[i])
			{
				m_LerpTimerList[i] -= Time.deltaTime * 3f;
				m_MoneyModelInstanceList[i].position = Vector3.Lerp(base.transform.position, m_PlaceMoneyLocation + m_PosYOffsetList[i] + m_PosYIndexOffset, m_LerpTimerList[i]);
				if (m_LerpTimerList[i] <= 0f)
				{
					m_LerpTimerList[i] = 0f;
					m_IsLerpingBackList[i] = false;
					m_MoneyModelInstanceList[i].gameObject.SetActive(value: false);
				}
			}
		}
	}

	public override void OnMouseButtonUp()
	{
		if (m_GivenAmount >= 100 || !m_CashierCounter.IsGivingChange())
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < m_MoneyModelInstanceList.Count; i++)
		{
			if (!m_MoneyModelInstanceList[i].gameObject.activeSelf)
			{
				flag = true;
				m_IsLerpingList[i] = true;
				m_IsLerpingBackList[i] = false;
				m_PosYOffsetList[i] = m_CashierCounter.GetChangeMoneyPosYOffset(m_IsCoin, m_GivenAmount);
				m_MoneyModelInstanceList[i].gameObject.SetActive(value: true);
				break;
			}
		}
		if (!flag)
		{
			if (m_CanBeCoinOrBill)
			{
				if (m_IsCoin)
				{
					m_MoneyModelInstance = UnityEngine.Object.Instantiate(m_CoinMoneyInstanceMeshRenderer.transform, m_CoinMoneyInstanceMeshRenderer.transform.position, m_CoinMoneyInstanceMeshRenderer.transform.rotation, base.transform);
				}
				else
				{
					m_MoneyModelInstance = UnityEngine.Object.Instantiate(m_MoneyInstanceMeshRenderer.transform, m_MoneyInstanceMeshRenderer.transform.position, m_MoneyInstanceMeshRenderer.transform.rotation, base.transform);
				}
				m_MoneyModelInstance.transform.localScale = m_MoneyModel.localScale;
			}
			else
			{
				m_MoneyModelInstance = UnityEngine.Object.Instantiate(m_MoneyModel, m_MoneyModel.position, m_MoneyModel.rotation, base.transform);
			}
			m_LerpTimerList.Add(0f);
			m_IsLerpingList.Add(item: true);
			m_IsLerpingBackList.Add(item: false);
			m_PosYOffsetList.Add(m_CashierCounter.GetChangeMoneyPosYOffset(m_IsCoin, m_GivenAmount));
			m_MoneyModelInstanceList.Add(m_MoneyModelInstance);
		}
		Vector3 vector = m_CashierCounter.m_PlaceMoneyLocation.forward * m_Index * 0.04f;
		m_PlaceMoneyLocation = m_CashierCounter.m_PlaceMoneyLocation.position + vector;
		if (m_IsCoin)
		{
			m_PlaceMoneyLocation = m_CashierCounter.m_PlaceCoinLocation.position + vector;
		}
		m_CashierCounter.OnGiveChange(m_ValueDouble, isTakingBack: false);
		m_GivenAmount++;
	}

	public override void OnRightMouseButtonUp()
	{
		if (m_GivenAmount <= 0 || !m_CashierCounter.IsGivingChange())
		{
			return;
		}
		for (int num = m_MoneyModelInstanceList.Count - 1; num >= 0; num--)
		{
			if (m_MoneyModelInstanceList[num].gameObject.activeSelf && !m_IsLerpingBackList[num])
			{
				m_IsLerpingList[num] = false;
				m_IsLerpingBackList[num] = true;
				m_PosYOffsetList[num] = m_CashierCounter.GetChangeMoneyPosYOffset(m_IsCoin, m_GivenAmount);
				m_MoneyModelInstanceList[num].gameObject.SetActive(value: true);
				break;
			}
		}
		m_CashierCounter.OnGiveChange(m_ValueDouble, isTakingBack: true);
		m_GivenAmount--;
	}

	public void ResetAmountGiven()
	{
		m_GivenAmount = 0;
		for (int i = 0; i < m_MoneyModelInstanceList.Count; i++)
		{
			m_IsLerpingList[i] = false;
			m_IsLerpingBackList[i] = false;
			m_LerpTimerList[i] = 0f;
			m_MoneyModelInstanceList[i].transform.position = base.transform.position;
			m_MoneyModelInstanceList[i].gameObject.SetActive(value: false);
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected new virtual void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected void OnMoneyCurrencyUpdated(CEventPlayer_OnMoneyCurrencyUpdated evt)
	{
		UpdateCurrency();
		for (int num = m_MoneyModelInstanceList.Count - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(m_MoneyModelInstanceList[num].gameObject);
		}
		m_MoneyModelInstanceList.Clear();
		m_LerpTimerList.Clear();
		m_IsLerpingList.Clear();
		m_IsLerpingBackList.Clear();
		m_PosYOffsetList.Clear();
		m_GivenAmount = 0;
	}
}
