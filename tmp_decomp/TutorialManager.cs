using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : CSingleton<TutorialManager>
{
	public ShopRenamer m_ShopRenamer;

	public GameObject m_TutorialTargetIndicator;

	public List<TutorialSubGroup> m_TutorialSubGroupList;

	public int m_TempShowTutorialIndex;

	public List<TutorialData> m_TutorialDataList = new List<TutorialData>();

	public CanvasGroup m_CanvasGrp;

	private bool m_IsShowingCanvasGrpAlpha;

	private bool m_IsHidingCanvasGrpAlpha;

	private bool m_FinishedTutorial;

	private float m_CanvasGrpAlphaLerpTimer;

	private void Awake()
	{
		for (int i = 0; i < CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList.Count; i++)
		{
			CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList[i].CloseScreen();
		}
	}

	private void Update()
	{
		if (m_FinishedTutorial)
		{
			return;
		}
		m_TempShowTutorialIndex = CPlayerData.m_TutorialIndex;
		m_TutorialDataList = CPlayerData.m_TutorialDataList;
		if (m_IsShowingCanvasGrpAlpha)
		{
			m_CanvasGrpAlphaLerpTimer += Time.deltaTime * 2f;
			m_CanvasGrp.alpha = Mathf.Lerp(0f, 1f, m_CanvasGrpAlphaLerpTimer);
			if (m_CanvasGrpAlphaLerpTimer >= 1f)
			{
				m_IsShowingCanvasGrpAlpha = false;
				m_CanvasGrpAlphaLerpTimer = 1f;
			}
		}
		else if (m_IsHidingCanvasGrpAlpha)
		{
			m_CanvasGrpAlphaLerpTimer -= Time.deltaTime * 2f;
			m_CanvasGrp.alpha = Mathf.Lerp(0f, 1f, m_CanvasGrpAlphaLerpTimer);
			if (m_CanvasGrpAlphaLerpTimer <= 0f)
			{
				m_IsShowingCanvasGrpAlpha = false;
				m_CanvasGrpAlphaLerpTimer = 0f;
			}
		}
	}

	public static void AddTaskValue(ETutorialTaskCondition tutorialTaskCondition, float valueAdd)
	{
		if (CSingleton<TutorialManager>.Instance.m_FinishedTutorial)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < CPlayerData.m_TutorialDataList.Count; i++)
		{
			if (CPlayerData.m_TutorialDataList[i].tutorialTaskCondition == tutorialTaskCondition)
			{
				flag = true;
				CPlayerData.m_TutorialDataList[i].value += valueAdd;
			}
		}
		if (!flag)
		{
			TutorialData tutorialData = new TutorialData();
			tutorialData.tutorialTaskCondition = tutorialTaskCondition;
			tutorialData.value = valueAdd;
			CPlayerData.m_TutorialDataList.Add(tutorialData);
		}
		for (int j = 0; j < CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList.Count; j++)
		{
			CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList[j].AddTaskValue(valueAdd, tutorialTaskCondition);
		}
		CSingleton<TutorialManager>.Instance.EvaluateTaskVisibility();
	}

	public void EvaluateTaskVisibility()
	{
		bool flag = false;
		for (int i = 0; i < CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList.Count; i++)
		{
			if (CPlayerData.m_TutorialIndex == 0 || flag || CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList[i].IsTaskFinish())
			{
				CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList[i].CloseScreen();
				continue;
			}
			flag = true;
			CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList[i].OpenScreen();
			CPlayerData.m_TutorialIndex = i + 1;
		}
		if (!flag)
		{
			m_FinishedTutorial = true;
		}
	}

	public static void SetGameUIVisible(bool isVisible)
	{
		if (isVisible)
		{
			if (CSingleton<TutorialManager>.Instance.m_CanvasGrp.alpha != 1f)
			{
				CSingleton<TutorialManager>.Instance.m_IsShowingCanvasGrpAlpha = true;
				CSingleton<TutorialManager>.Instance.m_IsHidingCanvasGrpAlpha = false;
			}
		}
		else if (CSingleton<TutorialManager>.Instance.m_CanvasGrp.alpha != 0f)
		{
			CSingleton<TutorialManager>.Instance.m_IsShowingCanvasGrpAlpha = false;
			CSingleton<TutorialManager>.Instance.m_IsHidingCanvasGrpAlpha = true;
		}
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		if (CPlayerData.m_TutorialIndex == 0)
		{
			m_TutorialTargetIndicator.gameObject.SetActive(value: true);
			m_ShopRenamer.SetIsTutorial();
			return;
		}
		if (CPlayerData.GetIsItemLicenseUnlocked(2) || CPlayerData.GetIsItemLicenseUnlocked(3))
		{
			bool flag = false;
			for (int i = 0; i < CPlayerData.m_TutorialDataList.Count; i++)
			{
				if (CPlayerData.m_TutorialDataList[i].tutorialTaskCondition == ETutorialTaskCondition.UnlockBasicCardBox)
				{
					flag = true;
					CPlayerData.m_TutorialDataList[i].value = 1f;
					break;
				}
			}
			if (!flag)
			{
				TutorialData tutorialData = new TutorialData();
				tutorialData.tutorialTaskCondition = ETutorialTaskCondition.UnlockBasicCardBox;
				tutorialData.value = 1f;
				CPlayerData.m_TutorialDataList.Add(tutorialData);
			}
		}
		m_TutorialTargetIndicator.gameObject.SetActive(value: false);
		for (int j = 0; j < CPlayerData.m_TutorialDataList.Count; j++)
		{
			for (int k = 0; k < CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList.Count; k++)
			{
				CSingleton<TutorialManager>.Instance.m_TutorialSubGroupList[k].AddTaskValue(CPlayerData.m_TutorialDataList[j].value, CPlayerData.m_TutorialDataList[j].tutorialTaskCondition);
			}
		}
		EvaluateTaskVisibility();
	}
}
