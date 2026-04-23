using System.Collections.Generic;
using UnityEngine;

public class InteractablePlayTable : InteractableObject
{
	public List<Transform> m_StandLocList;

	public List<Transform> m_StandLocBList;

	public List<Transform> m_SitLocList;

	public List<GameObject> m_LinkerStartList;

	public List<GameObject> m_LinkerEndList;

	public List<TableGameItemSet> m_TableGameItemSetList;

	public int m_MaxSeatCount = 2;

	public bool m_HasStartPlay;

	private bool m_HasStartPlayerPlayCard;

	private bool m_IsPlayerOccupied;

	public List<bool> m_IsSeatBooked = new List<bool>();

	public List<bool> m_IsSeatOccupied = new List<bool>();

	public List<bool> m_IsQueueOccupied = new List<bool>();

	public List<bool> m_IsCustomerSmelly = new List<bool>();

	public List<bool> m_IsStandLocValid = new List<bool>();

	public List<bool> m_IsStandLocBValid = new List<bool>();

	private List<int> m_EmptySeatIndex = new List<int>();

	private List<int> m_OccupiedSeatIndex = new List<int>();

	public List<float> m_PlayTableFee = new List<float>();

	private List<Customer> m_OccupiedCustomer = new List<Customer>();

	private int m_Index;

	public int m_CurrentPlayerCount;

	public float m_CurrentPlayTime;

	public float m_CurrentPlayTimeMax;

	public bool IsLookingForPlayer(bool findTableWithPlayerWaiting)
	{
		if (m_HasStartPlay || m_HasStartPlayerPlayCard || m_IsPlayerOccupied)
		{
			return false;
		}
		if (!findTableWithPlayerWaiting && m_CurrentPlayerCount == 0 && HasSeatBooking() && HasEmptySeatBooking())
		{
			return true;
		}
		if (m_CurrentPlayerCount > 0 && m_CurrentPlayerCount < m_MaxSeatCount && HasEmptySeatBooking())
		{
			return true;
		}
		return false;
	}

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitPlayTable(this);
		for (int i = 0; i < m_MaxSeatCount; i++)
		{
			m_IsSeatBooked.Add(item: false);
			m_IsSeatOccupied.Add(item: false);
			m_IsQueueOccupied.Add(item: false);
			m_IsCustomerSmelly.Add(item: false);
			m_IsStandLocValid.Add(item: true);
			m_IsStandLocBValid.Add(item: true);
			m_OccupiedCustomer.Add(null);
			m_PlayTableFee.Add(0f);
		}
		for (int j = 0; j < m_OccupiedCustomer.Count; j++)
		{
			m_TableGameItemSetList[j].gameObject.SetActive(value: false);
		}
	}

	public override void StartMoveObject()
	{
		if (m_CurrentPlayerCount > 0)
		{
			StopTableGame();
		}
		else
		{
			base.StartMoveObject();
		}
	}

	protected override void OnPlacedMovedObject()
	{
		base.OnPlacedMovedObject();
		if (m_ObjectType == EObjectType.PlayTable || m_ObjectType == EObjectType.BlackPlayTable || m_ObjectType == EObjectType.WhitePlayTable)
		{
			TutorialManager.AddTaskValue(ETutorialTaskCondition.BuyPlayTable, 1f);
		}
		EvaluateValidStandLoc();
	}

	public override void OnRightMouseButtonUp()
	{
		base.OnRightMouseButtonUp();
	}

	public void StartPlayerCardGame()
	{
		m_HasStartPlayerPlayCard = true;
		for (int i = 0; i < m_OccupiedCustomer.Count; i++)
		{
			if ((bool)m_OccupiedCustomer[i])
			{
				m_OccupiedCustomer[i].PlayTableGameStarted();
			}
		}
	}

	public void ExitPlayerCardGame()
	{
		StopTableGame();
	}

	private void EvaluateValidStandLoc()
	{
		for (int i = 0; i < m_StandLocList.Count; i++)
		{
			int mask = LayerMask.GetMask("PlayTableStandLocBlockedArea", "Glass");
			Collider[] array = Physics.OverlapBox(m_StandLocList[i].position + Vector3.up, Vector3.one * 0.1f, m_StandLocList[i].rotation, mask);
			m_IsStandLocValid[i] = array.Length == 0;
		}
		for (int j = 0; j < m_StandLocBList.Count; j++)
		{
			int mask2 = LayerMask.GetMask("PlayTableStandLocBlockedArea", "Glass");
			Collider[] array2 = Physics.OverlapBox(m_StandLocBList[j].position + Vector3.up, Vector3.one * 0.1f, Quaternion.identity, mask2);
			m_IsStandLocBValid[j] = array2.Length == 0;
		}
		for (int k = 0; k < m_LinkerEndList.Count; k++)
		{
			if (m_LinkerEndList[k].transform.position.x > 3.75f)
			{
				m_LinkerEndList[k].SetActive(value: false);
				m_LinkerStartList[k].SetActive(value: false);
			}
			else
			{
				m_LinkerEndList[k].SetActive(value: true);
				m_LinkerStartList[k].SetActive(value: true);
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		if (m_HasStartPlay)
		{
			m_CurrentPlayTime += Time.deltaTime;
			if (m_CurrentPlayTime >= m_CurrentPlayTimeMax)
			{
				StopTableGame();
			}
		}
		else if (m_HasStartPlayerPlayCard)
		{
			m_CurrentPlayTime += Time.deltaTime;
		}
	}

	private void StopTableGame()
	{
		if (!m_HasStartPlay && !m_HasStartPlayerPlayCard)
		{
			m_CurrentPlayTime = 0f;
		}
		for (int i = 0; i < m_OccupiedCustomer.Count; i++)
		{
			if ((bool)m_OccupiedCustomer[i])
			{
				m_OccupiedCustomer[i].PlayTableGameEnded(m_CurrentPlayTime, m_PlayTableFee[i]);
				CSingleton<CustomerManager>.Instance.OnCustomerExitPlaytable();
			}
		}
		for (int j = 0; j < m_OccupiedCustomer.Count; j++)
		{
			m_TableGameItemSetList[j].gameObject.SetActive(value: false);
		}
		m_HasStartPlay = false;
		m_HasStartPlayerPlayCard = false;
		m_IsPlayerOccupied = false;
		m_CurrentPlayTime = 0f;
		m_CurrentPlayerCount = 0;
		for (int k = 0; k < m_IsSeatOccupied.Count; k++)
		{
			m_IsSeatOccupied[k] = false;
			m_IsSeatBooked[k] = false;
			m_IsCustomerSmelly[k] = false;
			m_IsQueueOccupied[k] = false;
			m_OccupiedCustomer[k] = null;
			m_PlayTableFee[k] = 0f;
		}
		m_EmptySeatIndex.Clear();
		m_OccupiedSeatIndex.Clear();
	}

	public void LoadData(PlayTableSaveData playTableSaveData)
	{
		m_HasStartPlay = playTableSaveData.hasStartPlay;
		m_IsSeatOccupied = playTableSaveData.isSeatOccupied;
		m_CurrentPlayerCount = playTableSaveData.currentPlayerCount;
		m_CurrentPlayTime = playTableSaveData.currentPlayTime;
		m_CurrentPlayTimeMax = playTableSaveData.currentPlayTimeMax;
		if (playTableSaveData.isCustomerSmelly != null)
		{
			m_IsCustomerSmelly = playTableSaveData.isCustomerSmelly;
		}
		if (playTableSaveData.playTableFee == null || playTableSaveData.playTableFee.Count < m_MaxSeatCount)
		{
			playTableSaveData.playTableFee = new List<float>();
			playTableSaveData.playTableFee.Clear();
			for (int i = 0; i < m_MaxSeatCount; i++)
			{
				playTableSaveData.playTableFee.Add(0f);
			}
		}
		m_PlayTableFee = playTableSaveData.playTableFee;
		int num = 0;
		for (int j = 0; j < m_IsSeatOccupied.Count; j++)
		{
			if (!m_IsSeatOccupied[j])
			{
				continue;
			}
			Customer newCustomer = CSingleton<CustomerManager>.Instance.GetNewCustomer(canSpawnSmelly: false);
			if ((bool)newCustomer)
			{
				newCustomer.InstantSnapToPlayTable(this, j, m_HasStartPlay);
				m_OccupiedCustomer[j] = newCustomer;
				if (m_IsCustomerSmelly[j])
				{
					newCustomer.SetSmelly();
				}
				num++;
			}
		}
		if (m_HasStartPlay)
		{
			for (int k = 0; k < m_OccupiedCustomer.Count; k++)
			{
				m_TableGameItemSetList[k].RandomizeSetup();
				m_TableGameItemSetList[k].gameObject.SetActive(value: true);
			}
		}
		else if (num >= m_MaxSeatCount)
		{
			m_CurrentPlayerCount = 0;
			for (int l = 0; l < m_MaxSeatCount; l++)
			{
				CustomerHasReached(m_OccupiedCustomer[l], l);
			}
		}
		EvaluateValidStandLoc();
	}

	public Transform GetStandLoc(int index, bool canReturnNull = false)
	{
		if (m_IsStandLocValid[index])
		{
			return m_StandLocList[index];
		}
		if (canReturnNull)
		{
			return null;
		}
		return GetStandLocB(index, canReturnNull: true);
	}

	public Transform GetStandLocB(int index, bool canReturnNull = false)
	{
		if (m_IsStandLocBValid[index])
		{
			return m_StandLocBList[index];
		}
		if (canReturnNull)
		{
			return null;
		}
		return GetStandLoc(index, canReturnNull: true);
	}

	public Transform GetSitLoc(int index)
	{
		return m_SitLocList[index];
	}

	public void CustomerBookSeatIndex(int bookedSeatIndex)
	{
	}

	public void CustomerUnbookSeatIndex(int bookedSeatIndex)
	{
		m_IsSeatBooked[bookedSeatIndex] = false;
	}

	public void CustomerBookQueueIndex(int bookedSeatIndex)
	{
		m_IsQueueOccupied[bookedSeatIndex] = true;
	}

	public void CustomerUnbookQueueIndex(int bookedSeatIndex)
	{
		m_IsQueueOccupied[bookedSeatIndex] = false;
	}

	public void CustomerHasReached(Customer customer, int seatIndex)
	{
		m_OccupiedCustomer[seatIndex] = customer;
		m_IsSeatBooked[seatIndex] = false;
		m_IsSeatOccupied[seatIndex] = true;
		m_PlayTableFee[seatIndex] = customer.GetCurrentPlayTableFee();
		m_IsCustomerSmelly[seatIndex] = customer.IsSmelly();
		m_CurrentPlayerCount++;
		if (m_CurrentPlayerCount >= m_MaxSeatCount)
		{
			CSingleton<CustomerManager>.Instance.OnCustomerSitDownAtPlaytable(isWaitingForAnotherPlayer: false, customer);
			m_HasStartPlay = true;
			m_CurrentPlayTime = 0f;
			m_CurrentPlayTimeMax = Random.Range(15, 180);
			for (int i = 0; i < m_OccupiedCustomer.Count; i++)
			{
				if ((bool)m_OccupiedCustomer[i])
				{
					m_OccupiedCustomer[i].PlayTableGameStarted();
				}
			}
			for (int j = 0; j < m_OccupiedCustomer.Count; j++)
			{
				m_TableGameItemSetList[j].RandomizeSetup();
				m_TableGameItemSetList[j].gameObject.SetActive(value: true);
			}
		}
		else
		{
			CSingleton<CustomerManager>.Instance.OnCustomerSitDownAtPlaytable(isWaitingForAnotherPlayer: true, customer);
		}
	}

	public bool HasSeatBooking()
	{
		for (int i = 0; i < m_IsSeatBooked.Count; i++)
		{
			if (m_IsSeatBooked[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool HasEmptySeatBooking()
	{
		for (int i = 0; i < m_IsSeatBooked.Count; i++)
		{
			if (!m_IsSeatBooked[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool HasEmptyQueue()
	{
		for (int i = 0; i < m_IsQueueOccupied.Count; i++)
		{
			if (!m_IsQueueOccupied[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSeatEmpty(int index)
	{
		return !m_IsSeatOccupied[index];
	}

	public bool IsQueueEmpty(int index)
	{
		return !m_IsQueueOccupied[index];
	}

	public int GetEmptySeatBookingIndex()
	{
		m_EmptySeatIndex.Clear();
		m_OccupiedSeatIndex.Clear();
		for (int i = 0; i < m_IsSeatBooked.Count; i++)
		{
			if (!m_IsSeatBooked[i] && !m_IsSeatOccupied[i])
			{
				m_EmptySeatIndex.Add(i);
			}
			else
			{
				m_OccupiedSeatIndex.Add(i);
			}
		}
		if (m_EmptySeatIndex.Count <= 0)
		{
			return -1;
		}
		return m_EmptySeatIndex[Random.Range(0, m_EmptySeatIndex.Count)];
	}

	public int GetEmptySeatIndex()
	{
		if (m_CurrentPlayerCount >= m_MaxSeatCount || m_HasStartPlayerPlayCard || m_IsPlayerOccupied)
		{
			return -1;
		}
		m_EmptySeatIndex.Clear();
		m_OccupiedSeatIndex.Clear();
		for (int i = 0; i < m_IsSeatOccupied.Count; i++)
		{
			if (!m_IsSeatOccupied[i])
			{
				m_EmptySeatIndex.Add(i);
			}
			else
			{
				m_OccupiedSeatIndex.Add(i);
			}
		}
		if (m_EmptySeatIndex.Count <= 0)
		{
			return -1;
		}
		return m_EmptySeatIndex[Random.Range(0, m_EmptySeatIndex.Count)];
	}

	public override void OnDestroyed()
	{
		ShelfManager.RemovePlayTable(this);
		base.OnDestroyed();
	}

	public void SetIndex(int index)
	{
		m_Index = index;
	}

	public int GetIndex()
	{
		return m_Index;
	}

	public bool GetHasStartPlay()
	{
		return m_HasStartPlay;
	}

	public bool GetHasStartPlayerPlayCard()
	{
		if (!m_HasStartPlayerPlayCard)
		{
			return m_IsPlayerOccupied;
		}
		return true;
	}

	public List<bool> GetIsSeatOccupied()
	{
		return m_IsSeatOccupied;
	}

	public List<Customer> GetOccupiedCustomerList()
	{
		return m_OccupiedCustomer;
	}

	public List<bool> GetIsCustomerSmelly()
	{
		if (m_IsCustomerSmelly != null && m_OccupiedCustomer != null)
		{
			for (int i = 0; i < m_OccupiedCustomer.Count; i++)
			{
				if ((bool)m_OccupiedCustomer[i] && i < m_IsCustomerSmelly.Count)
				{
					m_IsCustomerSmelly[i] = m_OccupiedCustomer[i].IsSmelly();
				}
			}
		}
		return m_IsCustomerSmelly;
	}

	public List<float> GetPlayTableFee()
	{
		return m_PlayTableFee;
	}

	public int GetCurrentPlayerCount()
	{
		return m_CurrentPlayerCount;
	}

	public float GetCurrentPlayTime()
	{
		return m_CurrentPlayTime;
	}

	public float GetCurrentPlayTimeMax()
	{
		return m_CurrentPlayTimeMax;
	}

	public void OnPressGoNextDay()
	{
		StopTableGame();
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.AddListener<CEventPlayer_OnDayEnded>(OnDayEnded);
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected new void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.RemoveListener<CEventPlayer_OnDayEnded>(OnDayEnded);
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		for (int i = 0; i < m_OccupiedCustomer.Count; i++)
		{
			if ((bool)m_OccupiedCustomer[i])
			{
				m_OccupiedCustomer[i].DeactivateCustomer();
			}
			m_OccupiedCustomer[i] = null;
		}
		StopTableGame();
	}

	protected void OnDayEnded(CEventPlayer_OnDayEnded evt)
	{
		m_CurrentPlayTimeMax -= Random.Range(0f, m_CurrentPlayTimeMax - m_CurrentPlayTime);
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		EvaluateValidStandLoc();
	}
}
