using System.Collections.Generic;
using UnityEngine;

public class InteractableBulkDonationBox : InteractableObject
{
	public List<Transform> m_CustomerStandLocList;

	public List<GameObject> m_CardStackGrpList;

	private bool m_IsUIOpen;

	public List<CompactCardDataAmount> m_CompactCardDataAmountList = new List<CompactCardDataAmount>();

	private int m_BoxTotalCardCountMax = 10000;

	protected override void Awake()
	{
		for (int i = 0; i < m_CardStackGrpList.Count; i++)
		{
			m_CardStackGrpList[i].gameObject.SetActive(value: false);
		}
		base.Awake();
		ShelfManager.InitBulkDonationBox(this);
	}

	public override void OnRaycasted()
	{
		if (!m_IsUIOpen)
		{
			base.OnRaycasted();
		}
	}

	public override void OnMouseButtonUp()
	{
		if (!m_IsUIOpen)
		{
			m_IsUIOpen = true;
			OnRaycastEnded();
			CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: true);
			CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
			CSingleton<InteractionPlayerController>.Instance.m_BulkDonationBoxUIScreen.OpenBulkDonationBoxScreen(this);
		}
	}

	public void UpdateFillPercent(float fillPercent)
	{
		int num = Mathf.RoundToInt((float)m_CardStackGrpList.Count * fillPercent);
		for (int i = 0; i < m_CardStackGrpList.Count; i++)
		{
			m_CardStackGrpList[i].gameObject.SetActive(value: false);
		}
		for (int num2 = num - 1; num2 >= 0; num2--)
		{
			m_CardStackGrpList[num2].gameObject.SetActive(value: true);
		}
	}

	private void EvaluateCardStackVisibility()
	{
		int num = 0;
		for (int i = 0; i < m_CompactCardDataAmountList.Count; i++)
		{
			int amount = m_CompactCardDataAmountList[i].amount;
			num += amount;
		}
		UpdateFillPercent((float)num / (float)m_BoxTotalCardCountMax);
	}

	public void OnCloseBulkDonationBoxUIScreen()
	{
		m_IsUIOpen = false;
		OnRaycasted();
	}

	public int RemoveRandomCardFromShelf()
	{
		int num = 0;
		int num2 = Random.Range(2, 20);
		int num3 = m_CompactCardDataAmountList.Count - 1;
		while (num3 >= 0 && num2 > 0)
		{
			int num4 = Mathf.Clamp(Random.Range(0, 5), 0, m_CompactCardDataAmountList[num3].amount);
			m_CompactCardDataAmountList[num3].amount -= num4;
			if (m_CompactCardDataAmountList[num3].amount <= 0)
			{
				m_CompactCardDataAmountList.RemoveAt(num3);
			}
			num2 -= num4;
			num += num4;
			num3--;
		}
		EvaluateCardStackVisibility();
		return num;
	}

	public override void OnDestroyed()
	{
		ShelfManager.RemoveBulkDonationBox(this);
		base.OnDestroyed();
	}

	public int GetBoxTotalCardCountMax()
	{
		return m_BoxTotalCardCountMax;
	}

	public List<CompactCardDataAmount> GetCompactCardDataAmountList()
	{
		return m_CompactCardDataAmountList;
	}

	public void SetCompactCardDataAmountList(List<CompactCardDataAmount> compactCardDataAmountList)
	{
		m_CompactCardDataAmountList = compactCardDataAmountList;
	}

	public int GetTotalCardAmount()
	{
		int num = 0;
		for (int i = 0; i < m_CompactCardDataAmountList.Count; i++)
		{
			num += m_CompactCardDataAmountList[i].amount;
		}
		return num;
	}

	public bool IsEditingBulkBox()
	{
		return m_IsUIOpen;
	}

	public void LoadData(BulkDonationBoxSaveData saveData)
	{
		m_CompactCardDataAmountList = saveData.compactCardDataAmountList;
		EvaluateCardStackVisibility();
	}
}
