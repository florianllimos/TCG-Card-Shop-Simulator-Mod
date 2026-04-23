using System.Collections.Generic;
using UnityEngine;

public class InputTooltipListDisplay : MonoBehaviour
{
	public List<InputTooltipUI> m_InputTooltipUIPool;

	public List<InputTooltipUI> m_ActiveInputTooltipUIList;

	public InputTooltipUI m_PhoneTooltipUI;

	public InputTooltipUI m_AlbumTooltipUI;

	public InputTooltipUI m_DecoInventoryTooltipUI;

	public Transform m_PoolParent;

	public Transform m_ActiveLayoutParent;

	private List<EGameAction> m_ActiveActionList = new List<EGameAction>();

	private List<bool> m_ActiveIsHoldList = new List<bool>();

	private List<bool> m_ActiveSingleKeyOnlyList = new List<bool>();

	private void Awake()
	{
		ClearTooltip();
	}

	private void Init()
	{
		UpdatePhoneAndAlbumTooltip();
		m_PhoneTooltipUI.SetActive(isActive: true);
		m_AlbumTooltipUI.SetActive(isActive: true);
		m_DecoInventoryTooltipUI.SetActive(isActive: true);
	}

	public void UpdatePhoneAndAlbumTooltip()
	{
		UpdateTooltip(m_PhoneTooltipUI, EGameAction.OpenPhone, isHold: false, singleKeyOnly: true);
		UpdateTooltip(m_AlbumTooltipUI, EGameAction.OpenCardAlbum, isHold: false, singleKeyOnly: true);
		UpdateTooltip(m_DecoInventoryTooltipUI, EGameAction.Decorate, isHold: false, singleKeyOnly: true);
	}

	public void RefreshActiveTooltip()
	{
		if (m_ActiveActionList.Count <= 0)
		{
			return;
		}
		List<EGameAction> list = new List<EGameAction>();
		List<bool> list2 = new List<bool>();
		List<bool> list3 = new List<bool>();
		for (int i = 0; i < m_ActiveActionList.Count; i++)
		{
			list.Add(m_ActiveActionList[i]);
			list2.Add(m_ActiveIsHoldList[i]);
			list3.Add(m_ActiveSingleKeyOnlyList[i]);
		}
		ClearTooltip();
		EGameAction eGameAction = EGameAction.None;
		for (int j = 0; j < list.Count; j++)
		{
			if (eGameAction != list[j])
			{
				eGameAction = list[j];
				ShowTooltip(list[j], list2[j], list3[j]);
			}
		}
	}

	public void SetCurrentGameState(EGameState state)
	{
		ClearTooltip();
		m_PhoneTooltipUI.SetActive(isActive: false);
		m_AlbumTooltipUI.SetActive(isActive: false);
		m_DecoInventoryTooltipUI.SetActive(isActive: false);
		if (!CSingleton<CGameManager>.Instance.m_EnableTooltip)
		{
			m_ActiveLayoutParent.gameObject.SetActive(value: false);
			return;
		}
		switch (state)
		{
		case EGameState.DefaultState:
			m_PhoneTooltipUI.SetActive(isActive: true);
			m_AlbumTooltipUI.SetActive(isActive: true);
			m_DecoInventoryTooltipUI.SetActive(isActive: true);
			break;
		case EGameState.CashCounterState:
			ShowTooltip(EGameAction.ExitCounter);
			ShowTooltip(EGameAction.ScanCounter, isHold: true);
			break;
		case EGameState.HoldingBoxState:
			ShowTooltip(EGameAction.PlaceBox);
			ShowTooltip(EGameAction.Throw);
			break;
		case EGameState.HoldingCardState:
			ShowTooltip(EGameAction.OpenCardAlbum);
			break;
		case EGameState.HoldingItemState:
			if (CSingleton<InteractionPlayerController>.Instance.CanOpenPack())
			{
				ShowTooltip(EGameAction.InitiateOpenPack);
			}
			else if (CSingleton<InteractionPlayerController>.Instance.CanOpenCardBox())
			{
				ShowTooltip(EGameAction.OpenCardBox);
			}
			else if (CSingleton<InteractionPlayerController>.Instance.IsHoldingSpray())
			{
				ShowTooltip(EGameAction.InitiateSpray);
			}
			break;
		case EGameState.HoldSprayState:
			ShowTooltip(EGameAction.Spray);
			ShowTooltip(EGameAction.CancelSpray);
			break;
		case EGameState.MovingObjectState:
			ShowTooltip(EGameAction.PlaceMoveObject);
			ShowTooltip(EGameAction.DisableSnap);
			ShowTooltip(EGameAction.Rotate);
			ShowTooltip(EGameAction.RotateB);
			ShowTooltip(EGameAction.BoxUpShelf);
			break;
		case EGameState.MovingBoxState:
			ShowTooltip(EGameAction.PlaceMoveObject);
			ShowTooltip(EGameAction.Rotate);
			ShowTooltip(EGameAction.RotateB);
			break;
		case EGameState.ViewAlbumState:
			ShowTooltip(EGameAction.CloseCardAlbum);
			ShowTooltip(EGameAction.ViewAlbumCard, isHold: false, singleKeyOnly: true);
			ShowTooltip(EGameAction.TakeCard, isHold: false, singleKeyOnly: true);
			ShowTooltip(EGameAction.SortAlbum);
			ShowTooltip(EGameAction.FlipNextPage);
			ShowTooltip(EGameAction.FlipPreviousPage);
			ShowTooltip(EGameAction.FlipNextPage10);
			ShowTooltip(EGameAction.FlipPreviousPage10);
			if (CSingleton<InputManager>.Instance.m_IsControllerActive)
			{
				ShowTooltip(EGameAction.QuickSelect);
			}
			break;
		case EGameState.PhoneState:
			ShowTooltip(EGameAction.ClosePhone);
			break;
		}
		m_ActiveLayoutParent.gameObject.SetActive(value: true);
	}

	public void ClearTooltip()
	{
		m_ActiveActionList.Clear();
		m_ActiveIsHoldList.Clear();
		m_ActiveSingleKeyOnlyList.Clear();
		m_ActiveInputTooltipUIList.Clear();
		for (int i = 0; i < m_InputTooltipUIPool.Count; i++)
		{
			m_InputTooltipUIPool[i].SetActive(isActive: false);
			m_InputTooltipUIPool[i].m_Transform.SetParent(m_PoolParent, worldPositionStays: true);
		}
	}

	public void ShowTooltip(EGameAction action, bool isHold = false, bool singleKeyOnly = false)
	{
		string actionName = InputManager.GetActionName(action);
		if (CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			List<EGamepadControlBtn> actionBindedGamepadBtnList = InputManager.GetActionBindedGamepadBtnList(action);
			for (int i = 0; i < actionBindedGamepadBtnList.Count; i++)
			{
				for (int j = 0; j < m_InputTooltipUIPool.Count; j++)
				{
					if (!m_InputTooltipUIPool[j].m_IsActive)
					{
						m_InputTooltipUIPool[j].m_Transform.SetParent(m_ActiveLayoutParent, worldPositionStays: true);
						m_InputTooltipUIPool[j].SetGamepadInputTooltip(action, actionBindedGamepadBtnList[i], actionName, isHold);
						m_InputTooltipUIPool[j].SetActive(isActive: true);
						m_ActiveInputTooltipUIList.Add(m_InputTooltipUIPool[j]);
						m_ActiveActionList.Add(action);
						m_ActiveIsHoldList.Add(isHold);
						m_ActiveSingleKeyOnlyList.Add(singleKeyOnly);
						break;
					}
				}
				if (singleKeyOnly)
				{
					break;
				}
			}
			return;
		}
		List<KeyCode> actionBindedKeyList = InputManager.GetActionBindedKeyList(action);
		for (int k = 0; k < actionBindedKeyList.Count; k++)
		{
			for (int l = 0; l < m_InputTooltipUIPool.Count; l++)
			{
				if (!m_InputTooltipUIPool[l].m_IsActive)
				{
					m_InputTooltipUIPool[l].m_Transform.SetParent(m_ActiveLayoutParent, worldPositionStays: true);
					m_InputTooltipUIPool[l].SetInputTooltip(action, actionBindedKeyList[k], actionName, isHold);
					m_InputTooltipUIPool[l].SetActive(isActive: true);
					m_ActiveInputTooltipUIList.Add(m_InputTooltipUIPool[l]);
					m_ActiveActionList.Add(action);
					m_ActiveIsHoldList.Add(isHold);
					m_ActiveSingleKeyOnlyList.Add(singleKeyOnly);
					break;
				}
			}
			if (singleKeyOnly)
			{
				break;
			}
		}
	}

	public void RemoveTooltip(EGameAction action)
	{
		InputManager.GetActionBindedKeyList(action);
		InputManager.GetActionName(action);
		for (int num = m_ActiveInputTooltipUIList.Count - 1; num >= 0; num--)
		{
			if (m_ActiveInputTooltipUIList[num].m_CurrentGameAction == action)
			{
				m_ActiveInputTooltipUIList[num].m_Transform.SetParent(m_PoolParent, worldPositionStays: true);
				m_ActiveInputTooltipUIList[num].SetActive(isActive: false);
				m_ActiveInputTooltipUIList.RemoveAt(num);
				m_ActiveActionList.RemoveAt(num);
				m_ActiveIsHoldList.RemoveAt(num);
				m_ActiveSingleKeyOnlyList.RemoveAt(num);
			}
		}
	}

	public void UpdateTooltip(InputTooltipUI tooltipUI, EGameAction action, bool isHold = false, bool singleKeyOnly = false)
	{
		string actionName = InputManager.GetActionName(action);
		if (CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			List<EGamepadControlBtn> actionBindedGamepadBtnList = InputManager.GetActionBindedGamepadBtnList(action);
			int num = 0;
			if (num < actionBindedGamepadBtnList.Count)
			{
				tooltipUI.SetGamepadInputTooltip(action, actionBindedGamepadBtnList[num], actionName, isHold);
			}
		}
		else
		{
			List<KeyCode> actionBindedKeyList = InputManager.GetActionBindedKeyList(action);
			int num2 = 0;
			if (num2 < actionBindedKeyList.Count)
			{
				tooltipUI.SetInputTooltip(action, actionBindedKeyList[num2], actionName, isHold);
			}
		}
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_OnLanguageChanged>(OnLanguageChanged);
			CEventManager.AddListener<CEventPlayer_OnKeybindChanged>(OnKeybindChanged);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_OnLanguageChanged>(OnLanguageChanged);
			CEventManager.RemoveListener<CEventPlayer_OnKeybindChanged>(OnKeybindChanged);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		Init();
	}

	protected void OnLanguageChanged(CEventPlayer_OnLanguageChanged evt)
	{
		UpdatePhoneAndAlbumTooltip();
		RefreshActiveTooltip();
	}

	protected void OnKeybindChanged(CEventPlayer_OnKeybindChanged evt)
	{
		UpdatePhoneAndAlbumTooltip();
		RefreshActiveTooltip();
	}
}
