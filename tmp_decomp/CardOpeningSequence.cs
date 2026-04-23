using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOpeningSequence : CSingleton<CardOpeningSequence>
{
	public SkinnedMeshRenderer m_CardPackMesh;

	public Animator m_CardPackAnimator;

	public Transform m_StartLerpTransform;

	public GameObject m_CardOpeningUIGroup;

	public GameObject m_NewCardIcon;

	public GameObject m_HighValueCardIcon;

	public Animation m_CardOpeningRotateToFrontAnim;

	public List<Card3dUIGroup> m_Card3dUIList;

	public List<Animation> m_CardAnimList;

	public List<Transform> m_ShowAllCardPosList;

	public CardOpeningSequenceUI m_CardOpeningSequenceUI;

	public ParticleSystem m_OpenPackVFX;

	private bool m_IsReadyingToOpen;

	private bool m_IsReadyToOpen;

	private bool m_IsCanceling;

	private bool m_IsAutoFire;

	private bool m_IsAutoFireKeydown;

	public int m_StateIndex;

	private int m_TempIndex;

	private float m_StateTimer;

	private float m_MultiplierStateTimer = 1f;

	private float m_Slider;

	private float m_LerpPosTimer;

	private float m_LerpPosSpeed = 3f;

	private float m_AutoFireTimer;

	private float m_TotalCardValue;

	private Vector3 m_TargetMoveObjectPosition;

	private Transform m_TargetLerpTransform;

	private Item m_CurrentItem;

	private List<CardData> m_RolledCardDataList = new List<CardData>();

	private List<CardData> m_SecondaryRolledCardDataList = new List<CardData>();

	private List<CardData> m_ManualOpenCardDataList = new List<CardData>();

	private List<CardData> m_CardDataPool = new List<CardData>();

	private List<CardData> m_CardDataPool2 = new List<CardData>();

	private bool m_HasFoilCard;

	private ECollectionPackType m_CollectionPackType = ECollectionPackType.None;

	private List<bool> m_IsNewlList = new List<bool>();

	private List<float> m_CardValueList = new List<float>();

	private bool m_IsScreenActive;

	private bool m_IsGetHighValueCard;

	private int m_CurrentOpenedCardIndex;

	private int m_TotalExpGained;

	private int m_GCCollectCount;

	private float m_HighValueCardThreshold = 10f;

	private void Start()
	{
		m_CardPackAnimator.gameObject.SetActive(value: false);
		m_CardPackAnimator.speed = 0f;
		m_StateIndex = 0;
		m_CardOpeningUIGroup.SetActive(value: false);
		m_NewCardIcon.SetActive(value: false);
		m_HighValueCardIcon.SetActive(value: false);
		m_CardOpeningSequenceUI.m_CardValueTextGrp.SetActive(value: false);
		m_CardOpeningSequenceUI.m_TotalCardValueTextGrp.SetActive(value: false);
		m_CardOpeningSequenceUI.m_FoilRainbowGlowingBG.SetActive(value: false);
		for (int i = 0; i < 10; i++)
		{
			m_CardDataPool.Add(new CardData());
			m_CardDataPool2.Add(new CardData());
		}
	}

	private void InitOpenSequence()
	{
		m_CardPackAnimator.speed = 0f;
		m_CardPackAnimator.gameObject.SetActive(value: true);
		m_CardPackAnimator.Play("PackOpenAnim", -1, 0f);
		m_CardOpeningRotateToFrontAnim.Play("CardOpenSeq0_Idle");
		m_CardOpeningUIGroup.SetActive(value: true);
		m_NewCardIcon.SetActive(value: false);
		m_HighValueCardIcon.SetActive(value: false);
		InteractionPlayerController.RemoveToolTip(EGameAction.CancelOpenPack);
		InteractionPlayerController.RemoveToolTip(EGameAction.OpenPack);
		InteractionPlayerController.AddToolTip(EGameAction.OpenPack, isHold: true);
		m_MultiplierStateTimer = 1f + 2.5f * CSingleton<CGameManager>.Instance.m_OpenPackSpeedSlider;
		m_HighValueCardThreshold = 10 + CPlayerData.m_ShopLevel / 5 * 2;
	}

	public void ReadyingCardPack(Item item)
	{
		if (!m_IsReadyingToOpen)
		{
			m_IsScreenActive = true;
			CSingleton<InteractionPlayerController>.Instance.EnterLockMoveMode();
			CSingleton<InteractionPlayerController>.Instance.OnEnterOpenPackState();
			m_IsReadyingToOpen = true;
			m_IsReadyToOpen = false;
			m_LerpPosTimer = 0f;
			m_CurrentItem = item;
			m_CardPackAnimator.transform.position = m_StartLerpTransform.position;
			m_CardPackAnimator.transform.rotation = m_StartLerpTransform.rotation;
			m_CardPackAnimator.transform.localScale = m_StartLerpTransform.localScale;
			m_CardPackMesh.material = m_CurrentItem.m_Mesh.sharedMaterial;
			m_CardPackAnimator.gameObject.SetActive(value: true);
			m_CurrentItem.gameObject.SetActive(value: false);
			m_CardOpeningUIGroup.SetActive(value: false);
			m_CardPackAnimator.Play("PackOpenAnim", -1, 0f);
			CSingleton<InteractionPlayerController>.Instance.m_BlackBGWorldUIFade.SetFadeIn(3f);
			TutorialManager.SetGameUIVisible(isVisible: false);
			CenterDot.SetVisibility(isVisible: false);
			GameUIScreen.HideEnterGoNextDayIndicatorVisible();
			InteractionPlayerController.TempHideToolTip();
			InteractionPlayerController.AddToolTip(EGameAction.OpenPack, isHold: true);
			InteractionPlayerController.AddToolTip(EGameAction.CancelOpenPack);
			InteractionPlayerController.SetAllHoldItemVisibility(isVisible: false);
			CSingleton<InteractionPlayerController>.Instance.m_CameraFOVController.StartLerpToFOV(40f);
			SoundManager.GenericPop();
		}
	}

	private void Update()
	{
		m_IsAutoFire = false;
		if (!m_IsScreenActive)
		{
			return;
		}
		if (InputManager.GetKeyDownAction(EGameAction.OpenPack))
		{
			m_IsAutoFireKeydown = true;
		}
		if (InputManager.GetKeyUpAction(EGameAction.OpenPack))
		{
			m_IsAutoFireKeydown = false;
		}
		if (m_IsAutoFireKeydown)
		{
			m_AutoFireTimer += Time.deltaTime;
			if (m_AutoFireTimer >= 0.05f)
			{
				m_AutoFireTimer = 0f;
				m_IsAutoFire = true;
			}
		}
		else if (m_AutoFireTimer > 0f)
		{
			m_AutoFireTimer = 0f;
			m_IsAutoFire = true;
		}
		if (m_IsReadyingToOpen)
		{
			if (!m_IsReadyToOpen)
			{
				if (m_IsCanceling)
				{
					m_LerpPosTimer -= Time.deltaTime * m_LerpPosSpeed;
					if (m_LerpPosTimer < 0f)
					{
						m_LerpPosTimer = 0f;
						m_IsReadyToOpen = false;
						m_IsReadyingToOpen = false;
						m_IsCanceling = false;
						m_IsScreenActive = false;
						m_CardPackAnimator.gameObject.SetActive(value: false);
						CSingleton<InteractionPlayerController>.Instance.ExitLockMoveMode();
						CSingleton<InteractionPlayerController>.Instance.OnExitOpenPackState();
						InteractionPlayerController.RestoreHiddenToolTip();
						m_CurrentItem.gameObject.SetActive(value: true);
						InteractionPlayerController.SetAllHoldItemVisibility(isVisible: true);
						m_CurrentItem = null;
						TutorialManager.SetGameUIVisible(isVisible: true);
						CenterDot.SetVisibility(isVisible: true);
						GameUIScreen.ResetEnterGoNextDayIndicatorVisible();
					}
				}
				else
				{
					m_LerpPosTimer += Time.deltaTime * m_LerpPosSpeed;
					if (m_LerpPosTimer > 1f)
					{
						m_LerpPosTimer = 1f;
						m_IsReadyToOpen = true;
					}
				}
				m_CardPackAnimator.transform.localPosition = Vector3.Lerp(m_StartLerpTransform.localPosition, Vector3.zero, m_LerpPosTimer);
				m_CardPackAnimator.transform.localRotation = Quaternion.Lerp(m_StartLerpTransform.localRotation, Quaternion.identity, m_LerpPosTimer);
				m_CardPackAnimator.transform.localScale = Vector3.Lerp(m_StartLerpTransform.localScale, Vector3.one, m_LerpPosTimer);
			}
			else if (m_IsAutoFire)
			{
				m_IsReadyingToOpen = false;
				OpenScreen(InventoryBase.ItemTypeToCollectionPackType(m_CurrentItem.GetItemType()), isMultiPack: false);
			}
			else if (InputManager.GetKeyDownAction(EGameAction.CancelOpenPack) && !m_IsCanceling)
			{
				CSingleton<InteractionPlayerController>.Instance.AddHoldItemToFront(m_CurrentItem);
				m_IsCanceling = true;
				m_IsReadyToOpen = false;
				CSingleton<InteractionPlayerController>.Instance.m_BlackBGWorldUIFade.SetFadeOut(3f);
				InteractionPlayerController.RestoreHiddenToolTip();
				CSingleton<InteractionPlayerController>.Instance.m_CameraFOVController.StopLerpFOV();
				SoundManager.GenericPop(1f, 0.9f);
			}
		}
		else
		{
			if (!m_IsScreenActive)
			{
				return;
			}
			if (m_StateIndex == 0)
			{
				InitOpenSequence();
				m_StateIndex++;
			}
			else if (m_StateIndex == 1)
			{
				m_StateTimer += Time.deltaTime * m_MultiplierStateTimer;
				if (m_StateTimer > 0.05f)
				{
					m_StateTimer = 0f;
					if (m_TempIndex < m_Card3dUIList.Count)
					{
						m_Card3dUIList[m_TempIndex].gameObject.SetActive(value: true);
						m_TempIndex++;
					}
				}
				if (m_IsAutoFire || m_IsAutoFireKeydown || CSingleton<CGameManager>.Instance.m_OpenPacAutoNextCard)
				{
					m_Slider += 0.0065f * m_MultiplierStateTimer;
					m_CardPackAnimator.Play("PackOpenAnim", -1, m_Slider);
					if (m_Slider >= 0.3f)
					{
						m_OpenPackVFX.Play();
						SoundManager.PlayAudio("SFX_OpenPack", 0.6f);
						SoundManager.PlayAudio("SFX_BoxOpen", 0.5f);
						m_StateIndex++;
					}
				}
			}
			else if (m_StateIndex == 2)
			{
				m_Slider += Time.deltaTime * 1f * m_MultiplierStateTimer;
				m_CardPackAnimator.Play("PackOpenAnim", -1, m_Slider);
				m_StateTimer += Time.deltaTime;
				if (m_StateTimer > 0.05f)
				{
					m_StateTimer = 0f;
					if (m_TempIndex < m_Card3dUIList.Count)
					{
						m_Card3dUIList[m_TempIndex].gameObject.SetActive(value: true);
						m_TempIndex++;
					}
				}
				if (m_Slider >= 1f)
				{
					InteractionPlayerController.RemoveToolTip(EGameAction.OpenPack);
					m_TempIndex = 0;
					m_StateTimer = 0f;
					m_Slider = 0f;
					m_StateIndex++;
					for (int i = 0; i < m_Card3dUIList.Count; i++)
					{
						m_Card3dUIList[i].gameObject.SetActive(value: true);
					}
				}
			}
			else if (m_StateIndex == 3)
			{
				m_Slider += Time.deltaTime * 1f * m_MultiplierStateTimer;
				if (m_Slider >= 0.15f)
				{
					m_Slider = 0f;
					m_StateIndex++;
					m_CardOpeningRotateToFrontAnim.Play("CardOpenSeq1_RotateToFront");
				}
				else if (m_IsAutoFire || CSingleton<CGameManager>.Instance.m_OpenPacAutoNextCard)
				{
					float num = 0.002f * (float)m_CurrentOpenedCardIndex;
					float num2 = 0.001f * (float)m_CurrentOpenedCardIndex;
					SoundManager.PlayAudio("SFX_CardReveal1", 0.6f + num2, 1f + num);
					m_CardOpeningRotateToFrontAnim.Play("CardOpenSeq1_RotateToFront");
					m_Slider = 0f;
					m_StateTimer = 0f;
					m_StateIndex++;
				}
			}
			else if (m_StateIndex == 4)
			{
				m_Slider += Time.deltaTime * 1f * m_MultiplierStateTimer;
				if (!m_CardOpeningSequenceUI.m_CardValueTextGrp.activeSelf && m_CurrentOpenedCardIndex < 6 && m_Slider >= 0.45f && !m_IsNewlList[m_CurrentOpenedCardIndex] && m_CardValueList[m_CurrentOpenedCardIndex] < m_HighValueCardThreshold)
				{
					m_TotalCardValue += m_CardValueList[m_CurrentOpenedCardIndex];
					m_CardOpeningSequenceUI.ShowSingleCardValue(m_CardValueList[m_CurrentOpenedCardIndex]);
				}
				if (m_Slider >= 0.8f)
				{
					m_Slider = 0f;
					if (m_CardValueList[m_CurrentOpenedCardIndex] >= m_HighValueCardThreshold)
					{
						SoundManager.PlayAudio("SFX_FinalizeCard", 0.6f, 1.2f);
						m_CardAnimList[m_CurrentOpenedCardIndex].Play("OpenCardNewCard");
						m_HighValueCardIcon.SetActive(value: true);
						StartCoroutine(DelayToState(5, 0.9f));
						m_TotalCardValue += m_CardValueList[m_CurrentOpenedCardIndex];
						m_CardOpeningSequenceUI.ShowSingleCardValue(m_CardValueList[m_CurrentOpenedCardIndex]);
						m_IsGetHighValueCard = true;
					}
					else if (m_IsNewlList[m_CurrentOpenedCardIndex])
					{
						SoundManager.PlayAudio("SFX_CardReveal0", 0.6f);
						m_CardAnimList[m_CurrentOpenedCardIndex].Play("OpenCardNewCard");
						m_NewCardIcon.SetActive(value: true);
						StartCoroutine(DelayToState(5, 0.9f));
						m_TotalCardValue += m_CardValueList[m_CurrentOpenedCardIndex];
						m_CardOpeningSequenceUI.ShowSingleCardValue(m_CardValueList[m_CurrentOpenedCardIndex]);
					}
					else
					{
						m_StateIndex++;
					}
				}
			}
			else if (m_StateIndex == 5)
			{
				if (m_IsAutoFire || (!m_IsGetHighValueCard && CSingleton<CGameManager>.Instance.m_OpenPacAutoNextCard))
				{
					int num3 = UnityEngine.Random.Range(0, 3);
					float num4 = 0.002f * (float)m_CurrentOpenedCardIndex;
					float num5 = 0.001f * (float)m_CurrentOpenedCardIndex;
					switch (num3)
					{
					case 0:
						SoundManager.PlayAudio("SFX_CardReveal1", 0.6f + num5, 1f + num4);
						break;
					case 1:
						SoundManager.PlayAudio("SFX_CardReveal2", 0.6f + num5, 1f + num4);
						break;
					default:
						SoundManager.PlayAudio("SFX_CardReveal3", 0.6f + num5, 1f + num4);
						break;
					}
					if (m_CurrentOpenedCardIndex >= 7)
					{
						m_StateIndex = 7;
					}
					else
					{
						m_StateIndex++;
						m_NewCardIcon.SetActive(value: false);
						m_HighValueCardIcon.SetActive(value: false);
						m_CardAnimList[m_CurrentOpenedCardIndex].Play("OpenCardSlideExit");
						m_CardAnimList[m_CurrentOpenedCardIndex]["OpenCardSlideExit"].speed = 1f * m_MultiplierStateTimer;
						m_CardOpeningSequenceUI.HideSingleCardValue();
					}
					m_IsGetHighValueCard = false;
				}
			}
			else if (m_StateIndex == 6)
			{
				m_Slider += Time.deltaTime * 1f * m_MultiplierStateTimer;
				if (!m_CardOpeningSequenceUI.m_CardValueTextGrp.activeSelf && m_CurrentOpenedCardIndex < 6 && m_Slider >= 0.3f && !m_IsNewlList[m_CurrentOpenedCardIndex + 1] && m_CardValueList[m_CurrentOpenedCardIndex + 1] < m_HighValueCardThreshold)
				{
					m_TotalCardValue += m_CardValueList[m_CurrentOpenedCardIndex + 1];
					m_CardOpeningSequenceUI.ShowSingleCardValue(m_CardValueList[m_CurrentOpenedCardIndex + 1]);
				}
				if (!(m_Slider >= 0.5f))
				{
					return;
				}
				m_Slider = 0f;
				if (m_Card3dUIList.Count > m_CurrentOpenedCardIndex)
				{
					m_CardAnimList[m_CurrentOpenedCardIndex].transform.localPosition = Vector3.zero;
					m_Card3dUIList[m_CurrentOpenedCardIndex].gameObject.SetActive(value: false);
				}
				m_CurrentOpenedCardIndex++;
				if (m_CurrentOpenedCardIndex >= 7)
				{
					m_IsGetHighValueCard = false;
					m_StateIndex = 7;
					return;
				}
				if (m_Card3dUIList.Count > m_CurrentOpenedCardIndex + 1)
				{
					m_Card3dUIList[m_CurrentOpenedCardIndex + 1].gameObject.SetActive(value: true);
				}
				if (m_CardValueList[m_CurrentOpenedCardIndex] >= m_HighValueCardThreshold)
				{
					SoundManager.PlayAudio("SFX_FinalizeCard", 0.6f, 1.2f);
					m_CardAnimList[m_CurrentOpenedCardIndex].Play("OpenCardNewCard");
					m_HighValueCardIcon.SetActive(value: true);
					StartCoroutine(DelayToState(5, 0.9f));
					m_TotalCardValue += m_CardValueList[m_CurrentOpenedCardIndex];
					m_CardOpeningSequenceUI.ShowSingleCardValue(m_CardValueList[m_CurrentOpenedCardIndex]);
					m_IsGetHighValueCard = true;
				}
				else if (m_IsNewlList[m_CurrentOpenedCardIndex])
				{
					SoundManager.PlayAudio("SFX_CardReveal0", 0.6f);
					m_CardAnimList[m_CurrentOpenedCardIndex].Play("OpenCardNewCard");
					m_NewCardIcon.SetActive(value: true);
					StartCoroutine(DelayToState(5, 0.9f));
					m_TotalCardValue += m_CardValueList[m_CurrentOpenedCardIndex];
					m_CardOpeningSequenceUI.ShowSingleCardValue(m_CardValueList[m_CurrentOpenedCardIndex]);
				}
				else
				{
					m_StateIndex = 5;
				}
			}
			else if (m_StateIndex == 7)
			{
				if (m_StateTimer == 0f && m_Slider == 0f)
				{
					SoundManager.PlayAudio("SFX_PercStarJingle3", 0.6f);
					SoundManager.PlayAudio("SFX_Gift", 0.6f);
				}
				m_Slider += Time.deltaTime;
				if (m_Slider >= 0.05f)
				{
					m_Slider = 0f;
					m_CardAnimList[(int)m_StateTimer].transform.position = m_ShowAllCardPosList[(int)m_StateTimer].position;
					m_CardAnimList[(int)m_StateTimer].transform.rotation = m_ShowAllCardPosList[(int)m_StateTimer].rotation;
					m_Card3dUIList[(int)m_StateTimer].gameObject.SetActive(value: true);
					m_CardAnimList[(int)m_StateTimer].Play("OpenCardFinalReveal");
					m_StateTimer += 1f;
					if (m_StateTimer >= (float)m_Card3dUIList.Count)
					{
						m_StateTimer = 0f;
						m_StateIndex++;
						m_CardOpeningSequenceUI.StartShowTotalValue(m_TotalCardValue, m_HasFoilCard);
					}
				}
			}
			else if (m_StateIndex == 8)
			{
				m_StateTimer += Time.deltaTime;
				if (m_StateTimer >= 0.02f)
				{
					m_Slider = 0f;
					if ((int)m_StateTimer < m_Card3dUIList.Count)
					{
						m_Card3dUIList[(int)m_StateTimer].m_NewCardIndicator.gameObject.SetActive(m_ManualOpenCardDataList[(int)m_StateTimer].isNew);
					}
					m_StateTimer += 1f;
					if (m_StateTimer >= (float)m_Card3dUIList.Count)
					{
						m_StateIndex++;
					}
				}
			}
			else if (m_StateIndex == 9)
			{
				m_Slider += Time.deltaTime;
				if (m_Slider >= 1f)
				{
					m_Slider = 0f;
					m_StateIndex++;
				}
			}
			else if (m_StateIndex == 10)
			{
				if (m_IsAutoFire)
				{
					m_StateIndex++;
				}
			}
			else if (m_StateIndex == 11)
			{
				m_StateTimer += Time.deltaTime * 1f;
				if (!(m_StateTimer >= 0.01f))
				{
					return;
				}
				m_Slider = 0f;
				m_IsScreenActive = false;
				m_IsReadyToOpen = false;
				m_CardPackAnimator.gameObject.SetActive(value: false);
				m_CardOpeningUIGroup.SetActive(value: false);
				m_CardOpeningSequenceUI.HideTotalValue();
				CSingleton<InteractionPlayerController>.Instance.ExitLockMoveMode();
				CSingleton<InteractionPlayerController>.Instance.OnExitOpenPackState();
				if ((bool)m_CurrentItem)
				{
					m_CurrentItem.DisableItem();
				}
				m_CurrentItem = null;
				int num6 = 0;
				m_TotalCardValue = 0f;
				m_TotalExpGained = 0;
				bool isGet = false;
				bool isGet2 = false;
				for (int j = 0; j < m_ManualOpenCardDataList.Count; j++)
				{
					int num7 = (int)(m_ManualOpenCardDataList[j].GetCardBorderType() + 1) * Mathf.CeilToInt((float)(m_ManualOpenCardDataList[j].GetCardBorderType() + 1) / 2f);
					if (m_ManualOpenCardDataList[j].isFoil)
					{
						num7 *= 8;
					}
					m_TotalExpGained += num7;
					if (m_ManualOpenCardDataList[j].GetCardBorderType() == ECardBorderType.FullArt && m_ManualOpenCardDataList[j].isFoil)
					{
						isGet = true;
						if (m_ManualOpenCardDataList[j].expansionType == ECardExpansionType.Ghost)
						{
							isGet2 = true;
						}
					}
					if (m_ManualOpenCardDataList[j].isNew)
					{
						num6++;
					}
				}
				if (m_TotalExpGained > 0)
				{
					CEventManager.QueueEvent(new CEventPlayer_AddShopExp(m_TotalExpGained));
				}
				for (int k = 0; k < m_CardAnimList.Count; k++)
				{
					m_CardAnimList[k].transform.localPosition = Vector3.zero;
					m_CardAnimList[k].transform.localRotation = Quaternion.identity;
					m_Card3dUIList[k].m_NewCardIndicator.gameObject.SetActive(value: false);
					m_CardAnimList[k].Play("OpenCardDefaultPos");
				}
				if (CSingleton<InteractionPlayerController>.Instance.GetHoldItemCount() <= 0)
				{
					TutorialManager.SetGameUIVisible(isVisible: true);
					CenterDot.SetVisibility(isVisible: true);
					GameUIScreen.ResetEnterGoNextDayIndicatorVisible();
					CSingleton<InteractionPlayerController>.Instance.m_BlackBGWorldUIFade.SetFadeOut(3f);
					CSingleton<InteractionPlayerController>.Instance.m_CameraFOVController.StopLerpFOV();
					m_IsAutoFireKeydown = false;
					m_AutoFireTimer = 0f;
				}
				CSingleton<CustomerManager>.Instance.PlayerFinishOpenCardPack();
				CSingleton<InteractionPlayerController>.Instance.EvaluateOpenCardPack();
				TutorialManager.AddTaskValue(ETutorialTaskCondition.OpenPack, 1f);
				CPlayerData.m_GameReportDataCollect.cardPackOpened++;
				CPlayerData.m_GameReportDataCollectPermanent.cardPackOpened++;
				AchievementManager.OnCardPackOpened(CPlayerData.m_GameReportDataCollectPermanent.cardPackOpened);
				AchievementManager.OnGetFullArtFoil(isGet);
				AchievementManager.OnGetFullArtGhostFoil(isGet2);
				if (num6 > 0)
				{
					AchievementManager.OnCheckAlbumCardCount(CPlayerData.GetTotalCardCollectedAmount());
				}
				m_GCCollectCount += 2;
			}
			else if (m_StateIndex == 12)
			{
				m_IsScreenActive = false;
			}
			else if (m_StateIndex == 101)
			{
				_ = m_StateTimer;
				_ = 0f;
				m_StateTimer += Time.deltaTime;
				if (m_StateTimer >= 0.05f)
				{
					int num8 = UnityEngine.Random.Range(0, 3);
					float num9 = 0.002f * (float)m_CurrentOpenedCardIndex;
					float num10 = 0.001f * (float)m_CurrentOpenedCardIndex;
					switch (num8)
					{
					case 0:
						SoundManager.PlayAudio("SFX_CardReveal1", 0.6f + num10, 1f + num9);
						break;
					case 1:
						SoundManager.PlayAudio("SFX_CardReveal2", 0.6f + num10, 1f + num9);
						break;
					default:
						SoundManager.PlayAudio("SFX_CardReveal3", 0.6f + num10, 1f + num9);
						break;
					}
					m_CurrentOpenedCardIndex++;
				}
			}
			else
			{
				_ = m_StateIndex;
				_ = -1;
			}
		}
	}

	public void OnPressFinishGetCard()
	{
		SoundManager.GenericConfirm();
		m_StateIndex = 11;
	}

	private IEnumerator DelayToState(int stateIndex, float delayTime)
	{
		m_StateIndex = -1;
		yield return new WaitForSeconds(delayTime);
		m_StateIndex = stateIndex;
	}

	public void OpenScreen(ECollectionPackType collectionPackType, bool isMultiPack, bool isPremiumPack = false)
	{
		m_IsScreenActive = true;
		m_CollectionPackType = collectionPackType;
		m_IsNewlList.Clear();
		m_TotalExpGained = 0;
		m_TotalCardValue = 0f;
		m_HasFoilCard = false;
		if (isMultiPack)
		{
			m_ManualOpenCardDataList.Clear();
			m_RolledCardDataList.Clear();
			m_CardValueList.Clear();
			m_SecondaryRolledCardDataList.Clear();
			m_StateIndex = -1;
			int godPackRollIndex = GetGodPackRollIndex();
			GetPackContent(clearList: false, godPackRollIndex);
			godPackRollIndex = GetGodPackRollIndex();
			GetPackContent(clearList: false, godPackRollIndex);
			godPackRollIndex = GetGodPackRollIndex();
			GetPackContent(clearList: false, godPackRollIndex);
			godPackRollIndex = GetGodPackRollIndex();
			GetPackContent(clearList: false, godPackRollIndex);
			godPackRollIndex = GetGodPackRollIndex();
			GetPackContent(clearList: false, godPackRollIndex);
		}
		else
		{
			int godPackRollIndex2 = GetGodPackRollIndex();
			m_StateIndex = 0;
			GetPackContent(clearList: true, godPackRollIndex2);
			m_ManualOpenCardDataList.Clear();
			m_CardValueList.Clear();
			for (int i = 0; i < m_RolledCardDataList.Count; i++)
			{
				CardData cardData = new CardData();
				cardData.CopyData(m_RolledCardDataList[i]);
				m_ManualOpenCardDataList.Add(cardData);
				m_CardValueList.Add(CPlayerData.GetCardMarketPrice(m_RolledCardDataList[i]));
			}
			ECardExpansionType cardExpansionType = InventoryBase.GetCardExpansionType(collectionPackType);
			if (cardExpansionType == ECardExpansionType.Tetramon || cardExpansionType == ECardExpansionType.Destiny)
			{
				int num = UnityEngine.Random.Range(0, 10000);
				if (cardExpansionType == ECardExpansionType.Tetramon)
				{
					num = UnityEngine.Random.Range(0, 20000);
				}
				if (godPackRollIndex2 == 11 || godPackRollIndex2 == 12)
				{
					num = 0;
				}
				if (num < 20 && CPlayerData.m_ShopLevel > 1)
				{
					GetPackContent(clearList: true, godPackRollIndex2, isSecondaryRolledData: true, ECollectionPackType.GhostPack);
					if (m_SecondaryRolledCardDataList.Count > 0)
					{
						if (godPackRollIndex2 == 11 || godPackRollIndex2 == 12)
						{
							for (int j = 0; j < m_SecondaryRolledCardDataList.Count; j++)
							{
								m_ManualOpenCardDataList[j].CopyData(m_SecondaryRolledCardDataList[j]);
								m_CardValueList[j] = CPlayerData.GetCardMarketPrice(m_SecondaryRolledCardDataList[j]);
							}
						}
						else
						{
							int index = UnityEngine.Random.Range(0, m_SecondaryRolledCardDataList.Count);
							m_ManualOpenCardDataList[m_ManualOpenCardDataList.Count - 1].CopyData(m_SecondaryRolledCardDataList[index]);
							m_CardValueList[m_ManualOpenCardDataList.Count - 1] = CPlayerData.GetCardMarketPrice(m_SecondaryRolledCardDataList[index]);
						}
					}
				}
			}
		}
		m_CurrentOpenedCardIndex = 0;
		for (int k = 0; k < m_Card3dUIList.Count; k++)
		{
			m_Card3dUIList[k].gameObject.SetActive(value: false);
		}
		for (int l = 0; l < m_ManualOpenCardDataList.Count; l++)
		{
			CPlayerData.AddCard(m_ManualOpenCardDataList[l], 1);
			m_Card3dUIList[l].m_CardUI.SetCardUI(m_ManualOpenCardDataList[l]);
			if (m_ManualOpenCardDataList[l].monsterType > EMonsterType.None && CPlayerData.GetCardAmount(m_ManualOpenCardDataList[l]) == 1)
			{
				m_ManualOpenCardDataList[l].isNew = true;
				if (CSingleton<CGameManager>.Instance.m_OpenPackShowNewCard)
				{
					m_IsNewlList.Add(item: true);
				}
				else
				{
					m_IsNewlList.Add(item: false);
				}
			}
			else
			{
				m_ManualOpenCardDataList[l].isNew = false;
				m_IsNewlList.Add(item: false);
			}
		}
		CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
		if (!isMultiPack)
		{
			return;
		}
		m_StateTimer = 0f;
		m_StateTimer = 0f;
		m_StateIndex = 101;
		for (int m = 0; m < m_ManualOpenCardDataList.Count; m++)
		{
			m_Card3dUIList[m].m_CardUI.SetCardUI(m_ManualOpenCardDataList[m]);
			if (m_ManualOpenCardDataList[m].monsterType > EMonsterType.None && CPlayerData.GetCardAmount(m_ManualOpenCardDataList[m]) == 1)
			{
				m_ManualOpenCardDataList[m].isNew = true;
				if (CSingleton<CGameManager>.Instance.m_OpenPackShowNewCard)
				{
					m_IsNewlList.Add(item: true);
				}
				else
				{
					m_IsNewlList.Add(item: false);
				}
			}
			else
			{
				m_ManualOpenCardDataList[m].isNew = false;
				m_IsNewlList.Add(item: false);
			}
		}
		CEventManager.QueueEvent(new CEventPlayer_SetCanvasGroupVisibility(isVisible: false));
	}

	private int GetGodPackRollIndex()
	{
		if (CPlayerData.m_ShopLevel <= 1)
		{
			return 0;
		}
		int result = 0;
		int num = UnityEngine.Random.Range(0, 100000);
		switch (num)
		{
		case 1000:
			result = 12;
			break;
		case 1001:
		case 1002:
		case 1003:
		case 1004:
		case 1005:
		case 1006:
		case 1007:
		case 1008:
		case 1009:
		case 1010:
		case 1011:
		case 1012:
		case 1013:
		case 1014:
		case 1015:
		case 1016:
		case 1017:
		case 1018:
		case 1019:
		case 1020:
		case 1021:
		case 1022:
		case 1023:
		case 1024:
		case 1025:
		case 1026:
		case 1027:
		case 1028:
		case 1029:
		case 1030:
		case 1031:
		case 1032:
		case 1033:
		case 1034:
		case 1035:
		case 1036:
		case 1037:
		case 1038:
		case 1039:
		case 1040:
		case 1041:
		case 1042:
		case 1043:
		case 1044:
		case 1045:
		case 1046:
		case 1047:
		case 1048:
		case 1049:
		case 1050:
		case 1051:
			result = 11;
			break;
		default:
			if (num >= 1052 && num <= 1060)
			{
				result = 10;
			}
			else if (num >= 1061 && num <= 1081)
			{
				result = 9;
			}
			else if (num >= 1082 && num <= 1130)
			{
				result = 8;
			}
			else if (num >= 1130 && num <= 1230)
			{
				result = 7;
			}
			else if (num >= 1240 && num <= 1360)
			{
				result = 6;
			}
			else if (num >= 1370 && num <= 1570)
			{
				result = 13;
			}
			else if (num >= 1580 && num <= 1650)
			{
				result = 5;
			}
			else if (num >= 1660 && num <= 1860)
			{
				result = 4;
			}
			else if (num >= 1870 && num <= 2120)
			{
				result = 3;
			}
			else if (num >= 2130 && num <= 2440)
			{
				result = 2;
			}
			else if (num >= 2450 && num <= 2950)
			{
				result = 1;
			}
			break;
		}
		return result;
	}

	private void GetPackContent(bool clearList, int godPackRollIndex, bool isSecondaryRolledData = false, ECollectionPackType overrideCollectionPackType = ECollectionPackType.None)
	{
		int num = 0;
		if (clearList)
		{
			if (isSecondaryRolledData)
			{
				m_SecondaryRolledCardDataList.Clear();
			}
			else
			{
				m_RolledCardDataList.Clear();
			}
		}
		List<EMonsterType> list = new List<EMonsterType>();
		List<EMonsterType> list2 = new List<EMonsterType>();
		List<EMonsterType> list3 = new List<EMonsterType>();
		List<EMonsterType> list4 = new List<EMonsterType>();
		List<EMonsterType> list5 = new List<EMonsterType>();
		ECardExpansionType cardExpansionType = InventoryBase.GetCardExpansionType(m_CollectionPackType);
		if (isSecondaryRolledData)
		{
			cardExpansionType = InventoryBase.GetCardExpansionType(overrideCollectionPackType);
		}
		bool openPackCanUseRarity = InventoryBase.GetCardUISetting(cardExpansionType).openPackCanUseRarity;
		bool openPackCanHaveDuplicate = InventoryBase.GetCardUISetting(cardExpansionType).openPackCanHaveDuplicate;
		for (int i = 0; i < InventoryBase.GetShownMonsterList(cardExpansionType).Count; i++)
		{
			EMonsterType monsterType = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).MonsterType;
			ERarity rarity = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).Rarity;
			list.Add(monsterType);
			switch (rarity)
			{
			case ERarity.Legendary:
				list5.Add(monsterType);
				break;
			case ERarity.Epic:
				list4.Add(monsterType);
				break;
			case ERarity.Rare:
				list3.Add(monsterType);
				break;
			default:
				list2.Add(monsterType);
				break;
			}
		}
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 1;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		float num10 = 10f;
		float num11 = 2f;
		float num12 = 0.1f;
		float num13 = 5f;
		ECardBorderType borderType = ECardBorderType.Base;
		float num14 = 20f;
		float num15 = 8f;
		float num16 = 4f;
		float num17 = 1f;
		float num18 = 0.25f;
		ERarity eRarity = ERarity.Common;
		int num19 = 7;
		if (m_CollectionPackType == ECollectionPackType.RareCardPack || m_CollectionPackType == ECollectionPackType.DestinyRareCardPack)
		{
			num6 = 0;
			num7 = 2;
			num7 = 7;
			num10 += 45f;
			num11 += 2f;
			num12 += 1f;
		}
		else if (m_CollectionPackType == ECollectionPackType.EpicCardPack || m_CollectionPackType == ECollectionPackType.DestinyEpicCardPack)
		{
			num6 = 0;
			num7 = 1;
			num8 = 2;
			num8 = 7;
			num9 = 0;
			num10 += 65f;
			num11 += 45f;
			num12 += 3f;
		}
		else if (m_CollectionPackType == ECollectionPackType.LegendaryCardPack || m_CollectionPackType == ECollectionPackType.DestinyLegendaryCardPack)
		{
			num6 = 0;
			num7 = 0;
			num8 = 1;
			num9 = 2;
			num9 = 7;
			num10 += 65f;
			num11 += 55f;
			num12 += 35f;
		}
		else if (m_CollectionPackType == ECollectionPackType.BasicCardPack || m_CollectionPackType == ECollectionPackType.DestinyBasicCardPack)
		{
			num6 = 7;
		}
		if (godPackRollIndex > 0)
		{
			num13 = 0f;
			num14 = 0f;
			num15 = 0f;
			num16 = 0f;
			num17 = 0f;
			num18 = 0f;
		}
		switch (godPackRollIndex)
		{
		case 1:
			num14 = 100f;
			break;
		case 2:
			num15 = 100f;
			break;
		case 3:
			num16 = 100f;
			break;
		case 4:
			num17 = 100f;
			break;
		case 5:
			num18 = 100f;
			break;
		case 6:
			num13 = 10000f;
			num14 = 100f;
			break;
		case 7:
			num13 = 10000f;
			num15 = 100f;
			break;
		case 8:
			num13 = 10000f;
			num16 = 100f;
			break;
		case 9:
			num13 = 10000f;
			num17 = 100f;
			break;
		case 10:
			num13 = 10000f;
			num18 = 100f;
			break;
		case 12:
			num13 = 10000f;
			break;
		case 13:
			num13 = 10000f;
			break;
		}
		for (int j = 0; j < num19; j++)
		{
			if (list.Count <= 0)
			{
				break;
			}
			int num20 = UnityEngine.Random.Range(0, list.Count);
			if (num9 - num5 > 0 && list5.Count > 0)
			{
				eRarity = ERarity.Legendary;
				num5++;
			}
			else if (num8 - num4 > 0 && list4.Count > 0)
			{
				eRarity = ERarity.Epic;
				num4++;
			}
			else if (num7 - num3 > 0 && list3.Count > 0)
			{
				eRarity = ERarity.Rare;
				num3++;
			}
			else if (num6 - num2 > 0 && list2.Count > 0)
			{
				eRarity = ERarity.Common;
				num2++;
			}
			else
			{
				int num21 = UnityEngine.Random.Range(0, 10000);
				int num22 = 0;
				int num23 = 4 - num3;
				int num24 = 4 - num4;
				int num25 = 4 - num5;
				bool flag = false;
				if (!flag && num12 > 0f && list5.Count > 0 && num25 > 0)
				{
					num22 = Mathf.RoundToInt(num12 * 100f);
					if (num21 < num22)
					{
						flag = true;
						eRarity = ERarity.Legendary;
						num5++;
					}
				}
				if (!flag && num11 > 0f && list4.Count > 0 && num24 > 0)
				{
					num22 = Mathf.RoundToInt(num11 * 100f);
					if (num21 < num22)
					{
						flag = true;
						eRarity = ERarity.Epic;
						num4++;
					}
				}
				if (!flag && num10 > 0f && list3.Count > 0 && num23 > 0)
				{
					num22 = Mathf.RoundToInt(num10 * 100f);
					if (num21 < num22)
					{
						flag = true;
						eRarity = ERarity.Rare;
						num3++;
					}
				}
				if (!flag)
				{
					flag = true;
					eRarity = ERarity.Common;
					num2++;
				}
			}
			if (openPackCanUseRarity)
			{
				switch (eRarity)
				{
				case ERarity.Legendary:
					num20 = UnityEngine.Random.Range(0, list5.Count);
					num = (int)list5[num20];
					if (!openPackCanHaveDuplicate)
					{
						list5.RemoveAt(num20);
					}
					break;
				case ERarity.Epic:
					num20 = UnityEngine.Random.Range(0, list4.Count);
					num = (int)list4[num20];
					if (!openPackCanHaveDuplicate)
					{
						list4.RemoveAt(num20);
					}
					break;
				case ERarity.Rare:
					num20 = UnityEngine.Random.Range(0, list3.Count);
					num = (int)list3[num20];
					if (!openPackCanHaveDuplicate)
					{
						list3.RemoveAt(num20);
					}
					break;
				default:
					num20 = UnityEngine.Random.Range(0, list2.Count);
					num = (int)list2[num20];
					if (!openPackCanHaveDuplicate)
					{
						list2.RemoveAt(num20);
					}
					break;
				}
			}
			else
			{
				num20 = UnityEngine.Random.Range(0, list.Count);
				num = (int)list[num20];
				if (!openPackCanHaveDuplicate)
				{
					list.RemoveAt(num20);
				}
			}
			CardData cardData = m_CardDataPool[j];
			if (isSecondaryRolledData)
			{
				cardData = m_CardDataPool2[j];
			}
			cardData.monsterType = (EMonsterType)num;
			if (UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(num13 * 100f))
			{
				cardData.isFoil = true;
				m_HasFoilCard = true;
			}
			else
			{
				cardData.isFoil = false;
			}
			if (CPlayerData.m_TutorialIndex < 10 && CPlayerData.m_GameReportDataCollectPermanent.cardPackOpened == 0 && !m_HasFoilCard && j == num19 - 1)
			{
				cardData.isFoil = true;
				m_HasFoilCard = true;
			}
			bool flag2 = false;
			if (UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(num18 * 100f))
			{
				borderType = ECardBorderType.FullArt;
				flag2 = true;
			}
			if (!flag2 && UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(num17 * 100f))
			{
				borderType = ECardBorderType.EX;
				flag2 = true;
			}
			if (!flag2 && UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(num16 * 100f))
			{
				borderType = ECardBorderType.Gold;
				flag2 = true;
			}
			if (!flag2 && UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(num15 * 100f))
			{
				borderType = ECardBorderType.Silver;
				flag2 = true;
			}
			if (!flag2 && UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(num14 * 100f))
			{
				borderType = ECardBorderType.FirstEdition;
				flag2 = true;
			}
			if (!flag2 || cardExpansionType == ECardExpansionType.Ghost)
			{
				borderType = ECardBorderType.Base;
			}
			cardData.borderType = borderType;
			cardData.expansionType = cardExpansionType;
			if (cardData.expansionType == ECardExpansionType.Tetramon)
			{
				cardData.isDestiny = false;
			}
			else if (cardData.expansionType == ECardExpansionType.Destiny)
			{
				cardData.isDestiny = true;
			}
			else if (cardData.expansionType == ECardExpansionType.Ghost)
			{
				int num26 = UnityEngine.Random.Range(0, 100);
				cardData.isDestiny = num26 < 50;
			}
			else
			{
				cardData.isDestiny = false;
			}
			if (isSecondaryRolledData)
			{
				m_SecondaryRolledCardDataList.Add(cardData);
			}
			else
			{
				m_RolledCardDataList.Add(cardData);
			}
		}
		list.Clear();
		list2.Clear();
		list3.Clear();
		list4.Clear();
		list5.Clear();
		m_GCCollectCount++;
		if (m_GCCollectCount >= 100)
		{
			m_GCCollectCount = 0;
			GC.Collect();
		}
	}

	public bool IsActive()
	{
		return m_IsScreenActive;
	}

	public List<CardData> GetPackContentCardDataList(ECollectionPackType collectionPackType, bool isPremiumPack = false)
	{
		m_CollectionPackType = collectionPackType;
		int godPackRollIndex = GetGodPackRollIndex();
		GetPackContent(clearList: true, godPackRollIndex);
		ECardExpansionType cardExpansionType = InventoryBase.GetCardExpansionType(collectionPackType);
		if (cardExpansionType == ECardExpansionType.Tetramon || cardExpansionType == ECardExpansionType.Destiny)
		{
			int num = UnityEngine.Random.Range(0, 10000);
			if (cardExpansionType == ECardExpansionType.Tetramon)
			{
				num = UnityEngine.Random.Range(0, 20000);
			}
			if (godPackRollIndex == 11 || godPackRollIndex == 12)
			{
				num = 0;
			}
			if (num < 20 && CPlayerData.m_ShopLevel > 1)
			{
				GetPackContent(clearList: true, godPackRollIndex, isSecondaryRolledData: true, ECollectionPackType.GhostPack);
				if (m_SecondaryRolledCardDataList.Count > 0)
				{
					if (godPackRollIndex == 11 || godPackRollIndex == 12)
					{
						for (int i = 0; i < m_SecondaryRolledCardDataList.Count; i++)
						{
							m_RolledCardDataList[i] = m_SecondaryRolledCardDataList[i];
						}
					}
					else
					{
						int index = UnityEngine.Random.Range(0, m_SecondaryRolledCardDataList.Count);
						m_RolledCardDataList[m_RolledCardDataList.Count - 1] = m_SecondaryRolledCardDataList[index];
					}
				}
			}
		}
		List<CardData> list = new List<CardData>();
		for (int j = 0; j < m_RolledCardDataList.Count; j++)
		{
			list.Add(m_RolledCardDataList[j]);
		}
		m_RolledCardDataList.Clear();
		return list;
	}
}
