using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : CSingleton<CustomerManager>
{
	private bool m_IsPausing;

	protected bool m_FinishLoading;

	public bool m_IsPlayerTrading;

	public StockItemData_ScriptableObject m_StockItemData_SO;

	public static CustomerManager m_Instance;

	public CustomerTradeCardScreen m_CustomerTradeCardScreen;

	public Customer m_CustomerPrefab;

	public Customer m_CustomerFemalePrefab;

	public Transform m_CustomerParentGrp;

	public Transform m_ShopEntranceLocationPoint;

	public List<Transform> m_ShopEntranceLocationPointList;

	public List<Transform> m_ShopLocationPointList;

	public List<Transform> m_CustomerExitPointList;

	public List<Transform> m_ShopWindowOutsidePointList;

	private List<Customer> m_CustomerList = new List<Customer>();

	private List<Customer> m_SmellyCustomerList = new List<Customer>();

	public List<EItemType> m_TargetBuyItemList = new List<EItemType>();

	private float m_Timer;

	public float m_TimePerCustomer = 10f;

	public int m_TotalCurrentCustomerCount;

	public int m_TotalPlaytableCustomerCount;

	public int m_CustomerCountMax = 25;

	private int m_CustomerCountMaxBase = 1;

	private int m_CustomerCountMaxAddPerRoom = 1;

	private int m_SpawnedCustomerCount;

	private int m_CustomerExactChangeChance;

	private float m_CustomerMaxMoney = 100f;

	private bool m_IsDayEnded;

	private List<CustomerSaveData> m_CustomerSaveDataList = new List<CustomerSaveData>();

	private List<int> m_CustomerMaleModelIndexList = new List<int>();

	private List<int> m_CustomerFemaleModelIndexList = new List<int>();

	private int m_MaxMaleModelIndex = 35;

	private int m_MaxFemaleModelIndex = 15;

	private void Start()
	{
		m_CustomerList = new List<Customer>();
		for (int i = 0; i < m_CustomerParentGrp.childCount; i++)
		{
			m_CustomerList.Add(m_CustomerParentGrp.GetChild(i).gameObject.GetComponent<Customer>());
		}
		for (int j = 0; j < m_CustomerList.Count; j++)
		{
			m_CustomerList[j].gameObject.SetActive(value: false);
			m_CustomerList[j].RandomizeCharacterMesh();
			m_CustomerList[j].SetOutOfScreen();
		}
		EvaluateMaxCustomerCount();
	}

	public int GetCustomerModelIndex(bool isMale)
	{
		if (isMale)
		{
			if (m_CustomerMaleModelIndexList.Count <= 0)
			{
				for (int i = 0; i < m_MaxMaleModelIndex + 1; i++)
				{
					m_CustomerMaleModelIndexList.Add(i);
				}
			}
			int index = Random.Range(0, m_CustomerMaleModelIndexList.Count);
			int result = m_CustomerMaleModelIndexList[index];
			m_CustomerMaleModelIndexList.RemoveAt(index);
			return result;
		}
		if (m_CustomerFemaleModelIndexList.Count <= 0)
		{
			for (int j = 0; j < m_MaxFemaleModelIndex + 1; j++)
			{
				m_CustomerFemaleModelIndexList.Add(j);
			}
		}
		int index2 = Random.Range(0, m_CustomerFemaleModelIndexList.Count);
		int result2 = m_CustomerFemaleModelIndexList[index2];
		m_CustomerFemaleModelIndexList.RemoveAt(index2);
		return result2;
	}

	public int GetCustomerBuyItemChance(float currentPrice, float marketPrice)
	{
		int num = Mathf.RoundToInt((currentPrice - marketPrice) / marketPrice * 100f);
		num = Mathf.RoundToInt((float)num / Mathf.Lerp(1f, 10f, Mathf.Clamp((2f - marketPrice) / 2f, 0f, 1f)));
		int num2 = 0;
		if (num <= -20)
		{
			return 100;
		}
		if (num > -20 && num <= -10)
		{
			return Mathf.RoundToInt(Mathf.Lerp(95f, 100f, (num - -10) / -10));
		}
		if (num > -10 && num <= 0)
		{
			return Mathf.RoundToInt(Mathf.Lerp(90f, 95f, num / -10));
		}
		if (num > 0 && num <= 10)
		{
			return Mathf.RoundToInt(Mathf.Lerp(75f, 90f, (num - 10) / -10));
		}
		if (num > 10 && num <= 20)
		{
			return Mathf.RoundToInt(Mathf.Lerp(60f, 75f, (num - 20) / -10));
		}
		if (num > 20 && num <= 30)
		{
			return Mathf.RoundToInt(Mathf.Lerp(45f, 60f, (num - 30) / -10));
		}
		if (num > 30 && num <= 40)
		{
			return Mathf.RoundToInt(Mathf.Lerp(15f, 45f, (num - 40) / -10));
		}
		if (num > 40 && num <= 50)
		{
			return Mathf.RoundToInt(Mathf.Lerp(5f, 15f, (num - 50) / -10));
		}
		if (num > 50 && num <= 60)
		{
			return Mathf.RoundToInt(Mathf.Lerp(1f, 5f, (num - 60) / -10));
		}
		return 0;
	}

	public float GetCustomerMaxMoney()
	{
		int num = Mathf.Clamp((CPlayerData.m_UnlockRoomCount + CPlayerData.m_UnlockWarehouseRoomCount) * 10 + CPlayerData.m_ShopLevel, 0, 450);
		int num2 = Random.Range(0, 500 - num);
		float num3 = 0f;
		if (num2 == 0)
		{
			num3 += (float)Random.Range(50000, 1000000);
		}
		int num4 = Random.Range(0, 1000);
		if (num4 < 3)
		{
			return Random.Range(300f, (m_CustomerMaxMoney + num3) * 4f);
		}
		if (num4 < 50)
		{
			return Random.Range(40f, (m_CustomerMaxMoney + num3) * 2f);
		}
		return Random.Range(20f, m_CustomerMaxMoney + num3);
	}

	private void EvaluateMaxCustomerCount()
	{
		float num = 0f;
		int num2 = CPlayerData.m_UnlockRoomCount;
		for (int i = 0; i < CPlayerData.m_UnlockRoomCount + 1; i++)
		{
			if (num2 >= i)
			{
				num2 -= i;
				num += 1.25f;
			}
		}
		int num3 = 0;
		int num4 = CPlayerData.m_ShopLevel;
		for (int j = 0; j < CPlayerData.m_ShopLevel + 1; j++)
		{
			if (num4 >= j)
			{
				num4 -= j;
				num3++;
			}
		}
		num3--;
		m_CustomerCountMax = Mathf.Clamp(m_CustomerCountMaxBase + Mathf.RoundToInt(num) + num3, 3, 30);
		float num5 = 0f;
		List<int> list = new List<int>();
		for (int k = 0; k < 129; k++)
		{
			list.Add(0);
		}
		for (int l = 0; l < CSingleton<ShelfManager>.Instance.m_DecoObjectList.Count; l++)
		{
			list[(int)CSingleton<ShelfManager>.Instance.m_DecoObjectList[l].m_DecoObjectType]++;
		}
		for (int m = 0; m < list.Count; m++)
		{
			if (list[m] != 0)
			{
				num5 += InventoryBase.GetItemDecoPurchaseData((EDecoObject)m).price / 20000f * (float)list[m];
			}
		}
		num5 += (float)Mathf.Clamp(CSingleton<ShelfManager>.Instance.m_DecoObjectList.Count / 20, 0, 5);
		List<int> list2 = new List<int>();
		for (int n = 0; n < 55; n++)
		{
			list2.Add(0);
		}
		for (int num6 = 0; num6 < ShelfManager.GetInteractableObjectList().Count; num6++)
		{
			list2[(int)ShelfManager.GetInteractableObjectList()[num6].m_ObjectType]++;
		}
		for (int num7 = 0; num7 < ShelfManager.GetShelfList().Count; num7++)
		{
			list2[(int)ShelfManager.GetShelfList()[num7].m_ObjectType]++;
		}
		for (int num8 = 0; num8 < CSingleton<ShelfManager>.Instance.m_PlayTableList.Count; num8++)
		{
			list2[(int)CSingleton<ShelfManager>.Instance.m_PlayTableList[num8].m_ObjectType]++;
		}
		for (int num9 = 0; num9 < list2.Count; num9++)
		{
			if (list2[num9] != 0)
			{
				num5 += InventoryBase.GetObjectData((EObjectType)num9).decoBonus * (float)list2[num9];
			}
		}
		float num10 = 0f;
		for (int num11 = 0; num11 < CSingleton<ShelfManager>.Instance.m_CardShelfList.Count; num11++)
		{
			if (!CSingleton<ShelfManager>.Instance.m_CardShelfList[num11].m_ItemNotForSale)
			{
				continue;
			}
			float decoBonus = InventoryBase.GetObjectData(CSingleton<ShelfManager>.Instance.m_CardShelfList[num11].m_ObjectType).decoBonus;
			if (decoBonus == 0f)
			{
				continue;
			}
			for (int num12 = 0; num12 < CSingleton<ShelfManager>.Instance.m_CardShelfList[num11].GetCardCompartmentList().Count; num12++)
			{
				for (int num13 = 0; num13 < CSingleton<ShelfManager>.Instance.m_CardShelfList[num11].GetCardCompartmentList()[num12].m_StoredCardList.Count; num13++)
				{
					num10 += CPlayerData.GetCardMarketPrice(CSingleton<ShelfManager>.Instance.m_CardShelfList[num11].GetCardCompartmentList()[num12].m_StoredCardList[num13].m_Card3dUI.m_CardUI.GetCardData());
				}
			}
			num5 += num10 / 50000f * decoBonus;
		}
		if (num5 > 20f)
		{
			num5 = 20f;
		}
		m_CustomerCountMax += Mathf.RoundToInt(num5);
		m_TimePerCustomer = Mathf.Clamp(8f - (float)CPlayerData.m_UnlockRoomCount * 0.05f - (float)CPlayerData.m_ShopLevel * 0.05f, 4f, 10f);
		int num14 = 0;
		if (CPlayerData.m_IsWarehouseRoomUnlocked)
		{
			num14 = 500;
		}
		m_CustomerMaxMoney = Mathf.Clamp(100 + CPlayerData.m_UnlockRoomCount * 250 + num14 + CPlayerData.m_UnlockWarehouseRoomCount * 400 + CPlayerData.m_ShopLevel * 100, 100, 30000);
	}

	private void Init()
	{
		if (!m_FinishLoading)
		{
			m_FinishLoading = true;
			EvaluateMaxCustomerCount();
			m_CustomerSaveDataList = CPlayerData.m_CustomerSaveDataList;
			if (m_CustomerSaveDataList.Count > 0)
			{
				StartCoroutine(DelayLoadCustomer());
			}
			else
			{
				StartCoroutine(DelaySpawnGameStartCustomer());
			}
		}
	}

	private IEnumerator DelaySpawnGameStartCustomer()
	{
		yield return new WaitForSeconds(0.01f);
		SpawnGameStartCustomer();
	}

	private void SpawnGameStartCustomer()
	{
		if (LightManager.GetHasDayEnded())
		{
			return;
		}
		for (int i = 0; i < m_CustomerList.Count; i++)
		{
			if (m_CustomerList[i].IsActive())
			{
				m_CustomerList[i].DeactivateCustomer();
			}
		}
		m_TotalCurrentCustomerCount = 0;
		m_TotalPlaytableCustomerCount = 0;
		for (int j = 0; j < m_CustomerCountMax / 2 + 1; j++)
		{
			CustomerEnterShop();
		}
		bool flag = true;
		for (int k = 0; k < m_CustomerList.Count; k++)
		{
			if (m_CustomerList[k].IsActive())
			{
				m_CustomerList[k].SetExtraSpeedMultiplier(Random.Range(5, 200));
				if (flag)
				{
					m_CustomerList[k].SetExtraSpeedMultiplier(Random.Range(200, 400));
					flag = false;
				}
			}
		}
		StartCoroutine(DelayResetCustomerExtraSpeed());
	}

	private IEnumerator DelayResetCustomerExtraSpeed()
	{
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < m_CustomerList.Count; i++)
		{
			m_CustomerList[i].ResetExtraSpeedMultiplier();
		}
	}

	private void Update()
	{
		if (!m_IsPausing && !m_IsDayEnded)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer >= m_TimePerCustomer)
			{
				m_Timer = 0f;
				CustomerEnterShop();
			}
		}
	}

	private void CustomerEnterShop()
	{
		GetUnableFindQueueCustomerCount();
		_ = 3;
		if (m_TotalCurrentCustomerCount - m_TotalPlaytableCustomerCount >= m_CustomerCountMax)
		{
			return;
		}
		if (Random.Range(0, 2) == 0)
		{
			List<Customer> list = new List<Customer>();
			for (int i = 0; i < m_CustomerList.Count; i++)
			{
				if (!m_CustomerList[i].IsActive())
				{
					list.Add(m_CustomerList[i]);
				}
			}
			if (list.Count > 0)
			{
				UpdateCustomerCount(1);
				int index = Random.Range(0, list.Count);
				list[index].ActivateCustomer();
				list[index].gameObject.SetActive(value: true);
				return;
			}
		}
		for (int j = 0; j < m_CustomerList.Count; j++)
		{
			if (m_CustomerList[j].IsActive())
			{
				continue;
			}
			bool flag = false;
			if (!flag)
			{
				flag = true;
				UpdateCustomerCount(1);
				m_CustomerList[j].ActivateCustomer();
				m_CustomerList[j].gameObject.SetActive(value: true);
			}
			if (flag)
			{
				if (m_TotalCurrentCustomerCount >= m_CustomerList.Count)
				{
					AddCustomerPrefab();
				}
				break;
			}
		}
	}

	public Customer GetNewCustomer(bool canSpawnSmelly = true)
	{
		for (int i = 0; i < m_CustomerList.Count; i++)
		{
			if (!m_CustomerList[i].IsActive())
			{
				UpdateCustomerCount(1);
				m_CustomerList[i].ActivateCustomer(canSpawnSmelly);
				m_CustomerList[i].gameObject.SetActive(value: true);
				return m_CustomerList[i];
			}
		}
		return null;
	}

	private int GetUnableFindQueueCustomerCount()
	{
		int num = 0;
		for (int i = 0; i < m_CustomerList.Count; i++)
		{
			if (m_CustomerList[i].gameObject.activeSelf && m_CustomerList[i].IsUnableToFindQueue())
			{
				num++;
			}
		}
		return num;
	}

	private IEnumerator SpawnCustomer()
	{
		int i = 0;
		while (i < 20)
		{
			yield return new WaitForSeconds(0.01f * (float)i);
			CustomerEnterShop();
			int num = i + 1;
			i = num;
		}
	}

	public void UpdateCustomerCount(int addAmount)
	{
		m_TotalCurrentCustomerCount += addAmount;
		if (m_TotalCurrentCustomerCount < m_CustomerCountMax)
		{
			_ = 0;
		}
	}

	private void AddCustomerPrefab()
	{
		Customer customer = null;
		if (Random.Range(0, 3) == 0)
		{
			customer = Object.Instantiate(m_CustomerFemalePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, m_CustomerParentGrp);
			customer.name = "FemaleCustomer" + m_SpawnedCustomerCount;
		}
		else
		{
			customer = Object.Instantiate(m_CustomerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, m_CustomerParentGrp);
			customer.name = "Customer" + m_SpawnedCustomerCount;
		}
		customer.gameObject.SetActive(value: false);
		m_CustomerList.Add(customer);
		m_SpawnedCustomerCount++;
	}

	public void CustomerBuy(Customer customer, int shelfArrayIndex, EItemType itemType, int amount)
	{
	}

	public CustomerProfile GenerateRandomCustomerProfile()
	{
		return new CustomerProfile();
	}

	public static Transform GetRandomShopLocationPoint()
	{
		return CSingleton<CustomerManager>.Instance.m_ShopLocationPointList[Random.Range(0, CSingleton<CustomerManager>.Instance.m_ShopLocationPointList.Count)];
	}

	public static Transform GetRandomExitPoint()
	{
		return CSingleton<CustomerManager>.Instance.m_CustomerExitPointList[Random.Range(0, CSingleton<CustomerManager>.Instance.m_CustomerExitPointList.Count)];
	}

	public static Transform GetRandomShopWindowOutsidePoint()
	{
		return CSingleton<CustomerManager>.Instance.m_ShopWindowOutsidePointList[Random.Range(0, CSingleton<CustomerManager>.Instance.m_ShopWindowOutsidePointList.Count)];
	}

	public static bool CheckIsInsideShop(Vector3 customerPos)
	{
		if ((customerPos - CSingleton<CustomerManager>.Instance.m_ShopEntranceLocationPoint.position).magnitude < 0.2f)
		{
			return true;
		}
		for (int i = 0; i < CSingleton<CustomerManager>.Instance.m_ShopEntranceLocationPointList.Count; i++)
		{
			if ((customerPos - CSingleton<CustomerManager>.Instance.m_ShopEntranceLocationPointList[i].position).magnitude < 0.2f)
			{
				return true;
			}
		}
		return false;
	}

	public void AddToSmellyCustomerList(Customer customer)
	{
		if (!m_SmellyCustomerList.Contains(customer))
		{
			m_SmellyCustomerList.Add(customer);
		}
	}

	public void RemoveFromSmellyCustomerList(Customer customer)
	{
		if (m_SmellyCustomerList.Contains(customer))
		{
			m_SmellyCustomerList.Remove(customer);
		}
	}

	public List<Customer> GetCustomerList()
	{
		return m_CustomerList;
	}

	public List<Customer> GetSmellyCustomerList()
	{
		return m_SmellyCustomerList;
	}

	public int GetSmellyCustomerInsideShopCount()
	{
		int num = 0;
		for (int i = 0; i < m_SmellyCustomerList.Count; i++)
		{
			if (m_SmellyCustomerList[i].IsActive() && m_SmellyCustomerList[i].IsInsideShop() && m_SmellyCustomerList[i].IsSmelly())
			{
				num++;
			}
		}
		return num;
	}

	public bool IsWithinSmellyCustomerRange(Vector3 currentPos)
	{
		for (int i = 0; i < m_SmellyCustomerList.Count; i++)
		{
			if (m_SmellyCustomerList[i].IsActive() && m_SmellyCustomerList[i].IsInsideShop() && m_SmellyCustomerList[i].IsSmelly() && (m_SmellyCustomerList[i].transform.position - currentPos).magnitude < 3f)
			{
				return true;
			}
		}
		return false;
	}

	public void PlayerFinishOpenCardPack()
	{
		for (int i = 0; i < m_CustomerList.Count; i++)
		{
			if (m_CustomerList[i].IsActive() && m_CustomerList[i].IsInsideShop() && (m_CustomerList[i].transform.position - CSingleton<InteractionPlayerController>.Instance.transform.position).magnitude < 3f)
			{
				m_CustomerList[i].AddPlayerOpenPackNearby();
			}
		}
	}

	public bool HasCustomerInShop()
	{
		for (int i = 0; i < m_CustomerList.Count; i++)
		{
			if (m_CustomerList[i].IsActive() && m_CustomerList[i].IsInsideShop() && !m_CustomerList[i].HasCheckedOut())
			{
				return true;
			}
		}
		return false;
	}

	public void AddCustomerExactChangeChance()
	{
		m_CustomerExactChangeChance += Random.Range(5, 15);
	}

	public int GetCustomerExactChangeChance()
	{
		return m_CustomerExactChangeChance;
	}

	public void ResetCustomerExactChangeChance()
	{
		m_CustomerExactChangeChance = 0;
	}

	public void OnCustomerSitDownAtPlaytable(bool isWaitingForAnotherPlayer, Customer customer)
	{
		m_TotalPlaytableCustomerCount++;
		if (!isWaitingForAnotherPlayer)
		{
			return;
		}
		List<Customer> list = new List<Customer>();
		List<float> list2 = new List<float>();
		for (int i = 0; i < m_CustomerList.Count; i++)
		{
			bool flag = false;
			float magnitude = (m_CustomerList[i].transform.position - customer.transform.position).magnitude;
			if (list.Count > 0)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (magnitude < list2[j])
					{
						list.Insert(j, m_CustomerList[i]);
						list2.Insert(j, magnitude);
						break;
					}
				}
			}
			if (!flag)
			{
				list.Add(m_CustomerList[i]);
				list2.Add(magnitude);
			}
		}
		for (int k = 0; k < list.Count && !list[k].FindPlaytableWithWaitingCustomer(); k++)
		{
		}
	}

	public void OnCustomerExitPlaytable()
	{
		m_TotalPlaytableCustomerCount--;
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.AddListener<CEventPlayer_OnDayEnded>(OnDayEnded);
			CEventManager.AddListener<CEventPlayer_ShopLeveledUp>(OnShopLeveledUp);
			CEventManager.AddListener<CEventPlayer_NewRoomUnlocked>(OnNewRoomUnlocked);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.RemoveListener<CEventPlayer_OnDayEnded>(OnDayEnded);
			CEventManager.RemoveListener<CEventPlayer_ShopLeveledUp>(OnShopLeveledUp);
			CEventManager.RemoveListener<CEventPlayer_NewRoomUnlocked>(OnNewRoomUnlocked);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		Init();
		m_TargetBuyItemList = CPlayerData.m_TargetBuyItemList;
		if (CPlayerData.m_TargetBuyItemList.Count == 0)
		{
			EvaluateTargetBuyItemList();
		}
	}

	protected void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		m_IsDayEnded = false;
		StartCoroutine(DelaySpawnGameStartCustomer());
		m_SmellyCustomerList.Clear();
		EvaluateTargetBuyItemList();
	}

	private void EvaluateTargetBuyItemList()
	{
		if (CPlayerData.m_CurrentDay % 7 != 0 && m_TargetBuyItemList.Count != 0)
		{
			return;
		}
		m_TargetBuyItemList.Clear();
		int num = 2 + (CPlayerData.m_ShopLevel + 1) / 10;
		if (num > 8)
		{
			num = 8;
		}
		List<EItemType> itemTypeListOnShelf = ShelfManager.GetItemTypeListOnShelf();
		for (int i = 0; i < itemTypeListOnShelf.Count; i++)
		{
			if (itemTypeListOnShelf.Count <= 0)
			{
				break;
			}
			int index = Random.Range(0, itemTypeListOnShelf.Count);
			m_TargetBuyItemList.Add(itemTypeListOnShelf[index]);
			itemTypeListOnShelf.RemoveAt(index);
			if (m_TargetBuyItemList.Count >= num)
			{
				break;
			}
		}
		List<EItemType> unlockableItemTypeAtShopLevel = InventoryBase.GetUnlockableItemTypeAtShopLevel(CPlayerData.m_ShopLevel + 1);
		for (int j = 0; j < unlockableItemTypeAtShopLevel.Count; j++)
		{
			int index2 = Random.Range(0, unlockableItemTypeAtShopLevel.Count);
			m_TargetBuyItemList.Add(unlockableItemTypeAtShopLevel[index2]);
			unlockableItemTypeAtShopLevel.RemoveAt(index2);
			if (m_TargetBuyItemList.Count >= num + num / 2)
			{
				break;
			}
		}
		CPlayerData.m_TargetBuyItemList = m_TargetBuyItemList;
	}

	protected void OnDayEnded(CEventPlayer_OnDayEnded evt)
	{
		m_IsDayEnded = true;
		EvaluateMaxCustomerCount();
	}

	protected void OnShopLeveledUp(CEventPlayer_ShopLeveledUp evt)
	{
		EvaluateMaxCustomerCount();
	}

	protected void OnNewRoomUnlocked(CEventPlayer_NewRoomUnlocked evt)
	{
		EvaluateMaxCustomerCount();
	}

	private IEnumerator DelayLoadCustomer()
	{
		List<Customer> loadCustomerList = new List<Customer>();
		yield return new WaitForSeconds(0.02f);
		for (int i = 0; i < m_CustomerSaveDataList.Count; i++)
		{
			for (int j = 0; j < m_CustomerList.Count; j++)
			{
				if (!m_CustomerList[j].IsActive())
				{
					UpdateCustomerCount(1);
					m_CustomerList[j].ActivateCustomer();
					m_CustomerList[j].LoadCustomerSaveData(m_CustomerSaveDataList[i]);
					m_CustomerList[j].gameObject.SetActive(value: true);
					loadCustomerList.Add(m_CustomerList[j]);
					break;
				}
			}
		}
	}

	public void SaveCustomerData()
	{
		m_CustomerSaveDataList.Clear();
		for (int i = 0; i < m_CustomerList.Count; i++)
		{
			if ((bool)m_CustomerList[i] && m_CustomerList[i].m_IsActive)
			{
				CustomerSaveData customerSaveData = m_CustomerList[i].GetCustomerSaveData();
				if (customerSaveData.currentState != ECustomerState.PlayingAtTable && customerSaveData.currentState != ECustomerState.ExitingShop)
				{
					m_CustomerSaveDataList.Add(customerSaveData);
				}
			}
		}
		CPlayerData.m_CustomerSaveDataList = m_CustomerSaveDataList;
	}
}
