using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoCleanserStatusUI : CSingleton<AutoCleanserStatusUI>
{
	public GameObject m_ScreenGrp;

	public GameObject m_RangeGrp;

	public GameObject m_TurnOnText;

	public GameObject m_TurnOffText;

	public GameObject m_NeedRefillText;

	public Image m_TimerFillBar;

	public List<Image> m_FillBarList;

	public List<Image> m_BarNoItemList;

	public List<GameObject> m_FillBarGrpList;

	public Transform m_RangeImage;

	public Transform m_Cam;

	private int m_CurrentItemAmount;

	private float m_CooldownTime;

	private float m_PosYOffset;

	private Transform m_FollowTargetCleanser;

	private InteractableAutoCleanser m_CurrentAutoCleanser;

	private void Start()
	{
		m_TurnOnText.SetActive(value: false);
		m_TurnOffText.SetActive(value: false);
		m_NeedRefillText.SetActive(value: false);
		SetVisibility(isVisible: false);
		SetRangeVisibility(isVisible: false);
	}

	private void Update()
	{
		if (!m_FollowTargetCleanser)
		{
			return;
		}
		if (CSingleton<InteractionPlayerController>.Instance.m_CurrentGameState != EGameState.DefaultState && CSingleton<InteractionPlayerController>.Instance.m_CurrentGameState != EGameState.HoldingBoxState && CSingleton<InteractionPlayerController>.Instance.m_CurrentGameState != EGameState.HoldingItemState && CSingleton<InteractionPlayerController>.Instance.m_CurrentGameState != EGameState.MovingObjectState)
		{
			SetVisibility(isVisible: false);
		}
		base.transform.position = m_FollowTargetCleanser.transform.position + Vector3.up * m_PosYOffset;
		base.transform.LookAt(new Vector3(m_Cam.position.x, m_Cam.position.y, m_Cam.position.z));
		if ((bool)m_CurrentAutoCleanser)
		{
			m_TimerFillBar.fillAmount = m_CurrentAutoCleanser.GetTimer() / m_CooldownTime;
			if (m_CurrentAutoCleanser.IsTurnedOn() && !m_CurrentAutoCleanser.IsNeedRefill() && !m_TurnOnText.activeSelf)
			{
				m_TurnOnText.SetActive(value: true);
				m_TurnOffText.SetActive(value: false);
				m_NeedRefillText.SetActive(value: false);
			}
			else if (m_CurrentAutoCleanser.IsTurnedOn() && m_CurrentAutoCleanser.IsNeedRefill() && !m_NeedRefillText.activeSelf)
			{
				m_TurnOnText.SetActive(value: false);
				m_TurnOffText.SetActive(value: false);
				m_NeedRefillText.SetActive(value: true);
			}
			else if (!m_CurrentAutoCleanser.IsTurnedOn() && !m_TurnOffText.activeSelf)
			{
				m_TurnOnText.SetActive(value: false);
				m_TurnOffText.SetActive(value: true);
				m_NeedRefillText.SetActive(value: false);
			}
			for (int i = 0; i < m_CurrentAutoCleanser.GetStoredItemList().Count; i++)
			{
				m_FillBarList[i].fillAmount = m_CurrentAutoCleanser.GetStoredItemList()[i].GetContentFill();
			}
		}
	}

	public void UpdateAutoCleanserData(int maxSlot, int potency, int dispenseMethod, float cleanserRange, float cooldownTime)
	{
		m_CooldownTime = cooldownTime;
		m_RangeImage.localScale = Vector3.one * cleanserRange;
		for (int i = 0; i < m_FillBarGrpList.Count; i++)
		{
			m_FillBarGrpList[i].SetActive(value: false);
		}
		for (int j = 0; j < maxSlot; j++)
		{
			m_FillBarGrpList[j].SetActive(value: true);
		}
		UpdateItemSlotAmountFilled(m_CurrentItemAmount);
	}

	public void UpdateItemSlotAmountFilled(int itemAmount)
	{
		m_CurrentItemAmount = itemAmount;
		for (int i = 0; i < m_FillBarList.Count; i++)
		{
			m_FillBarList[i].enabled = false;
			m_BarNoItemList[i].enabled = true;
		}
		for (int j = 0; j < m_CurrentItemAmount; j++)
		{
			m_FillBarList[j].enabled = true;
			m_BarNoItemList[j].enabled = false;
		}
	}

	public static void SetTargetCleanser(InteractableAutoCleanser target, float posYOffset = 0f)
	{
		CSingleton<AutoCleanserStatusUI>.Instance.m_PosYOffset = posYOffset;
		if (CSingleton<AutoCleanserStatusUI>.Instance.m_CurrentAutoCleanser != target)
		{
			SetVisibility(isVisible: false);
			SetRangeVisibility(isVisible: false);
			CSingleton<AutoCleanserStatusUI>.Instance.m_CurrentAutoCleanser = target;
			if (target != null)
			{
				CSingleton<AutoCleanserStatusUI>.Instance.m_CurrentItemAmount = CSingleton<AutoCleanserStatusUI>.Instance.m_CurrentAutoCleanser.GetStoredItemList().Count;
				CSingleton<AutoCleanserStatusUI>.Instance.UpdateAutoCleanserData(CSingleton<AutoCleanserStatusUI>.Instance.m_CurrentAutoCleanser.m_PosList.Count, CSingleton<AutoCleanserStatusUI>.Instance.m_CurrentAutoCleanser.m_Potency, CSingleton<AutoCleanserStatusUI>.Instance.m_CurrentAutoCleanser.m_DispenseMethod, CSingleton<AutoCleanserStatusUI>.Instance.m_CurrentAutoCleanser.m_CleanserRange, CSingleton<AutoCleanserStatusUI>.Instance.m_CurrentAutoCleanser.m_CooldownTime);
				CSingleton<AutoCleanserStatusUI>.Instance.m_FollowTargetCleanser = target.transform;
				CSingleton<AutoCleanserStatusUI>.Instance.transform.position = CSingleton<AutoCleanserStatusUI>.Instance.m_FollowTargetCleanser.transform.position + Vector3.up * CSingleton<AutoCleanserStatusUI>.Instance.m_PosYOffset;
				SetVisibility(isVisible: true);
				SetRangeVisibility(isVisible: true);
			}
			else
			{
				CSingleton<AutoCleanserStatusUI>.Instance.m_FollowTargetCleanser = null;
			}
		}
	}

	public static void SetVisibility(bool isVisible)
	{
		CSingleton<AutoCleanserStatusUI>.Instance.m_ScreenGrp.gameObject.SetActive(isVisible);
	}

	public static void SetRangeVisibility(bool isVisible)
	{
		CSingleton<AutoCleanserStatusUI>.Instance.m_RangeGrp.gameObject.SetActive(isVisible);
	}
}
