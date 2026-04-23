using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
	protected int m_eventManagerId;

	protected CheatSequence m_IncreaseMoney;

	protected CheatSequence m_IncreaseShopLevel;

	protected void ApplyCheat(CCheatEvent evt)
	{
		if (evt != null && CSingleton<CEventManager>.InstanceExist())
		{
			if (evt.m_cheatType == CCheatEvent.ECheatType.IncreaseMoney)
			{
				CEventManager.QueueEvent(new CEventPlayer_AddCoin(9999f));
			}
			else if (evt.m_cheatType == CCheatEvent.ECheatType.IncreaseShopLevel)
			{
				CEventManager.QueueEvent(new CEventPlayer_AddShopExp(CPlayerData.GetExpRequiredToLevelUp()));
			}
		}
	}

	private void Awake()
	{
		m_IncreaseMoney = new CheatSequence(new List<EGameBaseKey>
		{
			EGameBaseKey.JStick_L1,
			EGameBaseKey.JStick_DPad_L,
			EGameBaseKey.JStick_DPad_R
		});
		m_IncreaseShopLevel = new CheatSequence(new List<EGameBaseKey>
		{
			EGameBaseKey.JStick_L1,
			EGameBaseKey.JStick_DPad_R,
			EGameBaseKey.JStick_DPad_L
		});
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		Object.Destroy(this);
	}

	private void Update()
	{
		bool flag = CSingleton<CEventManager>.InstanceExist();
		bool num = CSingleton<InputManager>.InstanceExist();
		if (flag)
		{
			if (m_eventManagerId == 0)
			{
				CEventManager.AddListener<CCheatEvent>(ApplyCheat);
			}
			m_eventManagerId = CSingleton<CEventManager>.InstanceID();
		}
		else
		{
			if (m_eventManagerId != 0)
			{
				CEventManager.RemoveListener<CCheatEvent>(ApplyCheat);
			}
			m_eventManagerId = 0;
		}
		if (!(num && flag))
		{
			return;
		}
		for (int i = 27; i < 39; i++)
		{
			if (InputManager.GetKeyDown((EGameBaseKey)i))
			{
				if (m_IncreaseMoney.Update((EGameBaseKey)i))
				{
					CEventManager.QueueEvent(new CCheatEvent(CCheatEvent.ECheatType.IncreaseMoney));
				}
				if (m_IncreaseShopLevel.Update((EGameBaseKey)i))
				{
					CEventManager.QueueEvent(new CCheatEvent(CCheatEvent.ECheatType.IncreaseShopLevel));
				}
			}
		}
	}
}
