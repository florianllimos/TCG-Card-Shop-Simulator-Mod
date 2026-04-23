using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePackagingBox_Card : InteractablePackagingBox
{
	public Transform m_StoredCardPosListGrp;

	public List<Transform> m_StoredCardPosList;

	private List<InteractableCard3d> m_StoredCardList = new List<InteractableCard3d>();

	private List<CardData> m_StoredCardDataList = new List<CardData>();

	private bool m_IsBoxOpened;

	private bool m_IsOpeningBox;

	protected override void Awake()
	{
		base.Awake();
		RestockManager.InitCardPackageBox(this);
	}

	protected override void Update()
	{
		base.Update();
		if (m_IsOpeningBox)
		{
			base.transform.position += Vector3.up * Time.deltaTime * -0.25f;
		}
	}

	public void UpdateCardData(List<CardData> cardDataList)
	{
		m_StoredCardDataList = cardDataList;
		for (int num = m_StoredCardDataList.Count - 1; num >= 0; num--)
		{
			if (m_StoredCardDataList[num] == null)
			{
				m_StoredCardDataList.RemoveAt(num);
			}
			else if (m_StoredCardDataList[num].monsterType == EMonsterType.None)
			{
				m_StoredCardDataList.RemoveAt(num);
			}
		}
		for (int i = 0; i < m_StoredCardDataList.Count; i++)
		{
			Card3dUIGroup cardUI = CSingleton<Card3dUISpawner>.Instance.GetCardUI();
			InteractableCard3d component = ShelfManager.SpawnInteractableObject(EObjectType.Card3d).GetComponent<InteractableCard3d>();
			cardUI.m_IgnoreCulling = true;
			cardUI.SetSimplifyCardDistanceCull(isCull: false);
			cardUI.m_CardUI.SetFoilCullListVisibility(isActive: true);
			cardUI.m_CardUI.ResetFarDistanceCull();
			cardUI.m_CardUI.SetCardUI(cardDataList[i]);
			cardUI.transform.position = component.transform.position;
			cardUI.transform.rotation = component.transform.rotation;
			component.SetCardUIFollow(cardUI);
			component.SetEnableCollision(isEnable: false);
			cardUI.m_IgnoreCulling = false;
			component.LerpToTransform(m_StoredCardPosList[i], m_StoredCardPosListGrp);
			m_StoredCardList.Add(component);
		}
	}

	public override void StartHoldBox(bool isPlayer, Transform holdItemPos)
	{
		if (!m_IsBeingHold)
		{
			base.StartHoldBox(isPlayer, CSingleton<InteractionPlayerController>.Instance.m_HoldBigItemPosUnscaled);
			if (isPlayer)
			{
				InteractionPlayerController.RemoveToolTip(EGameAction.OpenBox);
				InteractionPlayerController.AddToolTip(EGameAction.OpenBox);
			}
		}
	}

	public override void ThrowBox(bool isPlayer)
	{
		base.ThrowBox(isPlayer);
	}

	public override void DropBox(bool isPlayer)
	{
		base.DropBox(isPlayer);
	}

	protected override void SpawnPriceTag()
	{
	}

	public override void OnPressOpenBox()
	{
		if (m_IsOpeningBox)
		{
			return;
		}
		m_IsOpeningBox = true;
		StartCoroutine(DelayResetOpenBox());
		base.OnPressOpenBox();
		m_BoxAnim.Play("Open");
		CSingleton<InteractionPlayerController>.Instance.OnExitHoldBoxMode();
		SoundManager.PlayAudio("SFX_BoxOpen", 0.5f);
		if (m_StoredCardList.Count <= 0)
		{
			return;
		}
		CSingleton<InteractionPlayerController>.Instance.EnterHoldCardMode();
		for (int i = 0; i < m_StoredCardList.Count; i++)
		{
			InteractionPlayerController.AddHoldCard(m_StoredCardList[i]);
			if (m_StoredCardList[i].m_Card3dUI.m_CardUI.GetCardData().cardGrade == 10)
			{
				CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained++;
			}
		}
		m_StoredCardList.Clear();
		m_StoredCardDataList.Clear();
		AchievementManager.OnCheckGemMintCardCount(CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained);
		AchievementManager.OnCheckCollectedGradedCardSet();
	}

	protected IEnumerator DelayResetOpenBox()
	{
		yield return new WaitForSeconds(0.85f);
		m_IsOpeningBox = false;
		base.gameObject.SetActive(value: false);
		OnDestroyed();
	}

	public void SetOpenCloseBox(bool isOpen)
	{
		m_IsBoxOpened = isOpen;
		_ = m_IsBoxOpened;
	}

	public void EmptyBoxShelf()
	{
	}

	public override void OnDestroyed()
	{
		for (int i = 0; i < m_StoredCardList.Count; i++)
		{
			if ((bool)m_StoredCardList[i])
			{
				m_StoredCardList[i].OnDestroyed();
			}
		}
		RestockManager.RemoveCardPackageBox(this);
		base.OnDestroyed();
	}

	protected override void SetPriceTagVisibility(bool isVisible)
	{
	}

	public List<CardData> GetCardDataList()
	{
		return m_StoredCardDataList;
	}
}
