using System.Collections.Generic;
using UnityEngine;

public class UnlockRoomManager : CSingleton<UnlockRoomManager>
{
	public List<GameObject> m_WarehouseRoomUnlockHideList;

	public List<GameObject> m_WarehouseRoomUnlockShowList;

	public List<GameObject> m_GlassDoorGrpList;

	public List<Animation> m_GlassDoorAnimList;

	public List<LockedRoomBlocker> m_LockedRoomBlockerList;

	public List<LockedRoomBlocker> m_LockedWarehouseRoomBlockerList;

	public GameObject m_WarehouseNoEntryGrp;

	public GameObject m_WarehouseNoEntrySign;

	public Material m_WallMat;

	public Material m_WallBarMat;

	private void Init()
	{
		for (int i = 0; i < m_GlassDoorGrpList.Count; i++)
		{
			m_GlassDoorGrpList[i].SetActive(value: false);
		}
		for (int j = 0; j < m_LockedRoomBlockerList.Count; j++)
		{
			m_LockedRoomBlockerList[j].m_MainRoomIndex = j;
		}
		for (int k = 0; k < m_LockedWarehouseRoomBlockerList.Count; k++)
		{
			m_LockedWarehouseRoomBlockerList[k].m_StoreRoomIndex = k;
			m_LockedWarehouseRoomBlockerList[k].UpdateShopLotBWallMaterial(m_WallMat, m_WallBarMat);
		}
		for (int l = 0; l < CPlayerData.m_UnlockRoomCount; l++)
		{
			SetRoomBlockerVisibility(l, isVisible: false);
			EvaluateGlassDoorVisibility(playAnim: false, l);
		}
		SetUnlockWarehouseRoom(CPlayerData.m_IsWarehouseRoomUnlocked);
		for (int m = 0; m < CPlayerData.m_UnlockWarehouseRoomCount; m++)
		{
			SetWarehouseRoomBlockerVisibility(m, isVisible: false);
		}
		EvaluateWarehouseRoomOpenClose();
	}

	public void SetUnlockWarehouseRoom(bool isUnlocked)
	{
		CPlayerData.m_IsWarehouseRoomUnlocked = isUnlocked;
		for (int i = 0; i < m_WarehouseRoomUnlockHideList.Count; i++)
		{
			m_WarehouseRoomUnlockHideList[i].SetActive(!isUnlocked);
		}
		for (int j = 0; j < m_WarehouseRoomUnlockShowList.Count; j++)
		{
			m_WarehouseRoomUnlockShowList[j].SetActive(isUnlocked);
		}
		EvaluateWarehouseRoomOpenClose();
	}

	public void StartUnlockNextRoom()
	{
		if (CPlayerData.m_UnlockRoomCount < m_LockedRoomBlockerList.Count)
		{
			m_LockedRoomBlockerList[CPlayerData.m_UnlockRoomCount].HideBlocker();
			EvaluateGlassDoorVisibility(playAnim: true, CPlayerData.m_UnlockRoomCount);
			CPlayerData.m_UnlockRoomCount++;
			CEventManager.QueueEvent(new CEventPlayer_NewRoomUnlocked());
			AchievementManager.OnUnlockShopExpansion(CPlayerData.m_UnlockRoomCount);
		}
	}

	private void SetRoomBlockerVisibility(int index, bool isVisible)
	{
		m_LockedRoomBlockerList[index].gameObject.SetActive(isVisible);
	}

	public void StartUnlockNextWarehouseRoom()
	{
		if (CPlayerData.m_UnlockWarehouseRoomCount < m_LockedWarehouseRoomBlockerList.Count)
		{
			m_LockedWarehouseRoomBlockerList[CPlayerData.m_UnlockWarehouseRoomCount].HideBlocker();
			CPlayerData.m_UnlockWarehouseRoomCount++;
			CEventManager.QueueEvent(new CEventPlayer_NewRoomUnlocked());
		}
	}

	private void SetWarehouseRoomBlockerVisibility(int index, bool isVisible)
	{
		m_LockedWarehouseRoomBlockerList[index].gameObject.SetActive(isVisible);
	}

	private void EvaluateGlassDoorVisibility(bool playAnim, int index)
	{
		if (index % 4 != 0)
		{
			return;
		}
		int num = index / 4;
		if (num < m_GlassDoorGrpList.Count)
		{
			if (playAnim)
			{
				m_GlassDoorGrpList[num].SetActive(value: true);
				m_GlassDoorAnimList[num].Play();
			}
			else
			{
				m_GlassDoorGrpList[num].SetActive(value: true);
			}
		}
	}

	public void EvaluateWarehouseRoomOpenClose()
	{
		m_WarehouseNoEntrySign.SetActive(CPlayerData.m_IsWarehouseRoomUnlocked);
		m_WarehouseNoEntryGrp.SetActive(CPlayerData.m_IsWarehouseDoorClosed && CPlayerData.m_IsWarehouseRoomUnlocked);
	}

	public void SetWallBarVisibility(bool isVisible, bool isShopLotB)
	{
		for (int i = 0; i < m_LockedRoomBlockerList.Count; i++)
		{
			m_LockedRoomBlockerList[i].SetWallBarVisibility(isVisible, isShopLotB);
		}
		for (int j = 0; j < m_LockedWarehouseRoomBlockerList.Count; j++)
		{
			m_LockedWarehouseRoomBlockerList[j].SetWallBarVisibility(isVisible, isShopLotB);
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
		Init();
	}
}
