using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class WorkerManager : CSingleton<WorkerManager>
{
	private bool m_IsPausing;

	protected bool m_FinishLoading;

	public List<WorkerData> m_WorkerDataList;

	public Worker m_WorkerPrefab;

	public Worker m_WorkerFemalePrefab;

	public Transform m_WorkerParentGrp;

	public InteractableTrashBin m_TrashBin;

	public List<Transform> m_WorkerRestPointList;

	public List<string> m_TaskNameList;

	private bool m_IsDayEnded;

	public int m_TotalCurrentWorkerCount;

	private int m_WorkerCountMax = 8;

	private int m_SpawnedWorkerCount;

	public float m_TotalSalaryCost;

	public List<float> m_TotalSalaryCostList = new List<float>();

	private List<Worker> m_WorkerList = new List<Worker>();

	public List<WorkerSaveData> m_WorkerSaveDataList = new List<WorkerSaveData>();

	private void Start()
	{
		m_WorkerList = new List<Worker>();
		for (int i = 0; i < m_WorkerParentGrp.childCount; i++)
		{
			m_WorkerList.Add(m_WorkerParentGrp.GetChild(i).gameObject.GetComponent<Worker>());
		}
		for (int j = 0; j < m_WorkerList.Count; j++)
		{
			m_WorkerList[j].gameObject.SetActive(value: false);
			m_WorkerList[j].InitializeCharacter();
			m_WorkerList[j].SetOutOfScreen();
		}
	}

	private void Init()
	{
		if (!m_FinishLoading)
		{
			m_FinishLoading = true;
			m_WorkerSaveDataList = CPlayerData.m_WorkerSaveDataList;
			StartCoroutine(DelayLoadWorker());
		}
	}

	public void ActivateWorker(int index, bool resetTask)
	{
		if (!LightManager.GetHasDayEnded())
		{
			UpdateWorkerCount(1);
			m_WorkerList[index].ActivateWorker(resetTask);
			m_WorkerList[index].gameObject.SetActive(value: true);
			EvaluateSalaryCost();
		}
	}

	private void EvaluateSalaryCost()
	{
		m_TotalSalaryCost = 0f;
		for (int i = 0; i < m_WorkerList.Count; i++)
		{
			if (CPlayerData.GetIsWorkerHired(i))
			{
				m_TotalSalaryCost += m_WorkerDataList[i].costPerDay;
			}
		}
	}

	public float GetTotalSalaryCost()
	{
		EvaluateSalaryCost();
		return m_TotalSalaryCost;
	}

	public void UpdateWorkerCount(int addAmount)
	{
		m_TotalCurrentWorkerCount += addAmount;
	}

	private void AddWorkerPrefab()
	{
		Worker worker = null;
		if (Random.Range(0, 3) == 0)
		{
			worker = Object.Instantiate(m_WorkerFemalePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, m_WorkerParentGrp);
			worker.name = "FemaleWorker" + m_SpawnedWorkerCount;
		}
		else
		{
			worker = Object.Instantiate(m_WorkerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, m_WorkerParentGrp);
			worker.name = "Worker" + m_SpawnedWorkerCount;
		}
		worker.gameObject.SetActive(value: false);
		m_WorkerList.Add(worker);
		m_SpawnedWorkerCount++;
	}

	public static Transform GetWorkerRestPoint(int index)
	{
		return CSingleton<WorkerManager>.Instance.m_WorkerRestPointList[index];
	}

	public static string GetTaskName(EWorkerTask task)
	{
		if ((int)task >= CSingleton<WorkerManager>.Instance.m_TaskNameList.Count)
		{
			return "-";
		}
		return LocalizationManager.GetTranslation(CSingleton<WorkerManager>.Instance.m_TaskNameList[(int)task]);
	}

	public static WorkerData GetWorkerData(int index)
	{
		return CSingleton<WorkerManager>.Instance.m_WorkerDataList[index];
	}

	public static List<Worker> GetWorkerList()
	{
		return CSingleton<WorkerManager>.Instance.m_WorkerList;
	}

	public static int GetActiveWorkerCount()
	{
		int num = 0;
		for (int i = 0; i < CSingleton<WorkerManager>.Instance.m_WorkerList.Count; i++)
		{
			if (CSingleton<WorkerManager>.Instance.m_WorkerList[i].m_IsActive)
			{
				num++;
			}
		}
		return num;
	}

	public static void OnPressGoNextDay()
	{
		for (int num = CSingleton<WorkerManager>.Instance.m_WorkerList.Count - 1; num >= 0; num--)
		{
			CSingleton<WorkerManager>.Instance.m_WorkerList[num].ForceGoHome();
		}
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.AddListener<CEventPlayer_OnDayEnded>(OnDayEnded);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.RemoveListener<CEventPlayer_OnDayEnded>(OnDayEnded);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		Init();
	}

	protected void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		m_IsDayEnded = false;
		for (int i = 0; i < m_WorkerList.Count; i++)
		{
			m_WorkerList[i].DeactivateWorker();
		}
		for (int j = 0; j < m_WorkerList.Count; j++)
		{
			if (CPlayerData.GetIsWorkerHired(j))
			{
				ActivateWorker(j, resetTask: false);
				m_WorkerList[j].SetExtraSpeedMultiplier(Random.Range(1f + GetWorkerData(j).arriveEarlySpeedMin, 5f + GetWorkerData(j).arriveEarlySpeedMax));
			}
		}
		AchievementManager.OnStaffHired(GetActiveWorkerCount());
		StartCoroutine(DelayResetCustomerExtraSpeed());
	}

	private IEnumerator DelayResetCustomerExtraSpeed()
	{
		yield return new WaitForSeconds(1.65f);
		for (int i = 0; i < m_WorkerList.Count; i++)
		{
			m_WorkerList[i].ResetExtraSpeedMultiplier();
		}
	}

	protected void OnDayEnded(CEventPlayer_OnDayEnded evt)
	{
		m_IsDayEnded = true;
		for (int i = 0; i < m_WorkerList.Count; i++)
		{
			if (CPlayerData.GetIsWorkerHired(i))
			{
				m_WorkerList[i].OnDayEnded();
			}
		}
	}

	private IEnumerator DelayLoadWorker()
	{
		List<Worker> loadWorkerList = new List<Worker>();
		yield return new WaitForSeconds(0.02f);
		for (int i = 0; i < m_WorkerList.Count; i++)
		{
			if (!m_WorkerList[i].IsActive() && CPlayerData.GetIsWorkerHired(i))
			{
				if (!LightManager.GetHasDayEnded())
				{
					m_WorkerSaveDataList[i].isGoingHome = false;
				}
				else if (LightManager.GetHasDayEnded() && !m_WorkerSaveDataList[i].isGoingHome)
				{
					m_WorkerSaveDataList[i].isGoingHome = true;
					m_WorkerSaveDataList[i].primaryTask = m_WorkerSaveDataList[i].workerTask;
				}
				if (LightManager.GetHasDayEnded() && m_WorkerSaveDataList[i].primaryTask == EWorkerTask.GoBackHome)
				{
					m_WorkerSaveDataList[i].isGoingHome = true;
					m_WorkerSaveDataList[i].primaryTask = EWorkerTask.Rest;
					m_WorkerSaveDataList[i].workerTask = EWorkerTask.Rest;
				}
				if (!m_WorkerSaveDataList[i].isGoingHome)
				{
					UpdateWorkerCount(1);
					m_WorkerList[i].ActivateWorker(resetTask: true);
				}
				if (i < m_WorkerSaveDataList.Count)
				{
					m_WorkerList[i].LoadWorkerSaveData(m_WorkerSaveDataList[i]);
				}
				if (!m_WorkerSaveDataList[i].isGoingHome)
				{
					m_WorkerList[i].gameObject.SetActive(value: true);
					loadWorkerList.Add(m_WorkerList[i]);
				}
				if (m_WorkerSaveDataList[i].workerTask == EWorkerTask.GoBackHome && !m_WorkerSaveDataList[i].isGoingHome)
				{
					m_WorkerList[i].SetLastTask(EWorkerTask.Rest);
				}
			}
		}
	}

	public void SaveWorkerData()
	{
		m_WorkerSaveDataList.Clear();
		for (int i = 0; i < m_WorkerList.Count; i++)
		{
			WorkerSaveData workerSaveData = m_WorkerList[i].GetWorkerSaveData();
			m_WorkerSaveDataList.Add(workerSaveData);
		}
		CPlayerData.m_WorkerSaveDataList = m_WorkerSaveDataList;
	}
}
