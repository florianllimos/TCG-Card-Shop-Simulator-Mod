using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionBinderFlipAnimCtrl : MonoBehaviour
{
	public Animator m_BookAnim;

	public Animator m_BinderThicknessAnim;

	public List<BinderPageGrp> m_BinderPageGrpList;

	public Animation m_ShowHideAnim;

	public Transform m_InteractableCardFollowTransform;

	public List<InteractableCard3d> m_InteractableCard3dList;

	public List<InteractableCard3d> m_ControllerSelectionCard3dList;

	public CollectionBinderUI m_CollectionBinderUI;

	public CardOpeningSequenceUI m_CardOpeningSequenceUI;

	public MaterialFadeInOut m_BlackMaterialBGFade;

	public CardPricePopupUI m_CardPricePopupUI;

	public SkinnedMeshRenderer m_AlbumMesh;

	public Material m_NormalAlbumMat;

	public Material m_GradedCardAlbumMat;

	private bool m_OpenBinder;

	private bool m_CloseBinder;

	private bool m_GoNext;

	private bool m_GoPrevious;

	private bool m_GoNext10;

	private bool m_GoPrevious10;

	private bool m_IsGradedCardAlbum;

	private bool m_CanFlip = true;

	private bool m_IsBookOpen;

	private bool m_CanUpdateSort = true;

	private bool m_CanUpdateControllerCardIndex = true;

	private int m_Index = 1;

	private int m_MaxIndex = 1;

	private int m_CurrentRaycastedCardIndex;

	private int m_CurrentControllerCardIndex;

	private ECardExpansionType m_ExpansionType;

	private Transform m_TargetFollowPos;

	private Vector3 m_TargetLerpPos;

	private Vector3 m_OffsetPos;

	private Quaternion m_StartRot;

	private Quaternion m_TargetLerpRot;

	private float m_LerpTimer;

	private bool m_IsInViewCardAlbumMode;

	private bool m_IsLerpingPos;

	private bool m_IsHidingAlbum;

	private bool m_IsSorting;

	private Coroutine m_CanFlipCoroutine;

	private List<int> m_SortedIndexList = new List<int>();

	private List<int> m_SortTempList = new List<int>();

	private List<int> m_SortGradedCardPrice = new List<int>();

	private List<CardData> m_SortGradedCardDataList = new List<CardData>();

	private ECollectionSortingType m_SortingType;

	public bool m_IsHoldingCardCloseUp;

	public bool m_IsExitingCardCloseUp;

	public InteractableCard3d m_CurrentRaycastedInteractableCard3d;

	public InteractableCard3d m_CurrentViewInteractableCard3d;

	public InteractableCard3d m_CurrentSpawnedInteractableCard3d;

	private void Awake()
	{
		m_ShowHideAnim.gameObject.SetActive(value: false);
		m_BinderPageGrpList[0].m_Anim.SetTrigger("CloseBinder");
		m_BinderPageGrpList[1].m_Anim.SetTrigger("CloseBinder");
		m_BinderPageGrpList[2].m_Anim.SetTrigger("CloseBinder");
		m_BinderPageGrpList[1].SetVisibility(isVisible: false);
		m_BinderPageGrpList[2].SetVisibility(isVisible: false);
		m_Index = 1;
		if (m_CanFlipCoroutine != null)
		{
			StopCoroutine(m_CanFlipCoroutine);
		}
		m_CanFlipCoroutine = StartCoroutine(DelayResetCanFlipBook(CSingleton<InteractionPlayerController>.Instance.m_HideCardAlbumTime));
		base.transform.position = Vector3.one * -100000f;
	}

	public void OnMouseButtonUp()
	{
		if (!m_CollectionBinderUI.m_SortAlbumScreen.activeSelf && !InteractionPlayerController.GetCurrentHoldCard())
		{
			if (!m_IsHoldingCardCloseUp && !m_IsExitingCardCloseUp)
			{
				EnterViewUpCloseState();
			}
			else if (m_IsHoldingCardCloseUp && !m_IsExitingCardCloseUp)
			{
				ExitViewUpCloseState();
			}
		}
	}

	public void OnRightMouseButtonUp()
	{
		if ((m_IsHoldingCardCloseUp || m_IsExitingCardCloseUp) && m_IsHoldingCardCloseUp)
		{
			_ = m_IsExitingCardCloseUp;
		}
		if (CSingleton<InteractionPlayerController>.Instance.IsSelectingCardForEditDeckScreen() && (bool)m_CurrentRaycastedInteractableCard3d)
		{
			if (!m_IsGradedCardAlbum)
			{
				CardData cardData = m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
				CSingleton<InteractionPlayerController>.Instance.UpdateSelectedCardData(cardData);
				CPlayerData.ReduceCard(cardData, 1);
				CSingleton<InteractionPlayerController>.Instance.ExitViewCardAlbumMode();
			}
		}
		else if (CSingleton<InteractionPlayerController>.Instance.IsSelectingCardForBulkDonationBoxScreen() && (bool)m_CurrentRaycastedInteractableCard3d)
		{
			if (m_IsGradedCardAlbum)
			{
				if (CSingleton<InteractionPlayerController>.Instance.CanSelectGradedCardForBulkDonationBoxScreen())
				{
					CardData cardData2 = m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
					CSingleton<InteractionPlayerController>.Instance.UpdateSelectedCardData(cardData2);
					CPlayerData.RemoveGradedCard(cardData2);
					CSingleton<InteractionPlayerController>.Instance.ExitViewCardAlbumMode();
					m_CanUpdateSort = true;
				}
			}
			else
			{
				CardData cardData3 = m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
				CSingleton<InteractionPlayerController>.Instance.UpdateSelectedCardData(cardData3);
				CPlayerData.ReduceCard(cardData3, 1);
				CSingleton<InteractionPlayerController>.Instance.ExitViewCardAlbumMode();
			}
		}
		else if (CSingleton<InteractionPlayerController>.Instance.IsSelectingCardForGradingScreen() && (bool)m_CurrentRaycastedInteractableCard3d)
		{
			CardData cardData4 = m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
			CSingleton<InteractionPlayerController>.Instance.UpdateSelectedCardData(cardData4);
			if (m_IsGradedCardAlbum)
			{
				CPlayerData.RemoveGradedCard(cardData4);
				m_CanUpdateSort = true;
			}
			else
			{
				CPlayerData.ReduceCard(cardData4, 1);
			}
			CSingleton<InteractionPlayerController>.Instance.ExitViewCardAlbumMode();
		}
		else
		{
			if (!m_CurrentRaycastedInteractableCard3d)
			{
				return;
			}
			if (!InteractionPlayerController.HasEnoughSlotToHoldCard())
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.HandFull);
				return;
			}
			Card3dUIGroup cardUI = CSingleton<Card3dUISpawner>.Instance.GetCardUI();
			m_CurrentSpawnedInteractableCard3d = ShelfManager.SpawnInteractableObject(EObjectType.Card3d).GetComponent<InteractableCard3d>();
			cardUI.m_IgnoreCulling = true;
			cardUI.m_CardUIAnimGrp.gameObject.SetActive(value: true);
			cardUI.SetSimplifyCardDistanceCull(isCull: false);
			cardUI.m_CardUI.SetFoilCullListVisibility(isActive: true);
			cardUI.m_CardUI.ResetFarDistanceCull();
			cardUI.m_CardUI.SetFoilMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilMaterialWorldView);
			cardUI.m_CardUI.SetFoilBlendedMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilBlendedMaterialWorldView);
			cardUI.m_CardUI.SetCardUI(m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData());
			cardUI.transform.position = m_CurrentRaycastedInteractableCard3d.transform.position;
			cardUI.transform.rotation = m_CurrentRaycastedInteractableCard3d.transform.rotation;
			m_CurrentSpawnedInteractableCard3d.transform.position = m_CurrentRaycastedInteractableCard3d.transform.position;
			m_CurrentSpawnedInteractableCard3d.transform.rotation = m_CurrentRaycastedInteractableCard3d.transform.rotation;
			m_CurrentSpawnedInteractableCard3d.SetCardUIFollow(cardUI);
			m_CurrentSpawnedInteractableCard3d.SetEnableCollision(isEnable: false);
			CardData cardData5 = m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
			if (m_IsGradedCardAlbum)
			{
				CPlayerData.RemoveGradedCard(cardData5);
				m_CanUpdateSort = true;
			}
			else
			{
				CPlayerData.ReduceCard(cardData5, 1);
			}
			int num = CPlayerData.GetCardAmount(cardData5);
			m_BinderPageGrpList[0].SetSingleCard(m_CurrentRaycastedCardIndex, cardData5, num, m_SortingType);
			if (m_SortingType == ECollectionSortingType.DuplicatePrice)
			{
				num--;
			}
			if (m_IsGradedCardAlbum)
			{
				num = 0;
			}
			if (num <= 0)
			{
				m_CurrentRaycastedInteractableCard3d.m_Card3dUI.gameObject.SetActive(value: false);
				m_CurrentRaycastedInteractableCard3d.gameObject.SetActive(value: false);
				m_CurrentRaycastedInteractableCard3d.OnRaycastEnded();
				m_CurrentRaycastedInteractableCard3d = null;
			}
			InteractionPlayerController.AddHoldCard(m_CurrentSpawnedInteractableCard3d);
			InteractionPlayerController.RemoveToolTip(EGameAction.ViewAlbumCard);
		}
	}

	private void EnterViewUpCloseState()
	{
		if (!m_IsHoldingCardCloseUp && (bool)m_CurrentRaycastedInteractableCard3d)
		{
			CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
			m_CanUpdateControllerCardIndex = true;
			CenterDot.SetVisibility(isVisible: false);
			m_IsHoldingCardCloseUp = true;
			InteractionPlayerController.TempHideToolTip();
			InteractionPlayerController.AddToolTip(EGameAction.CloseCardAlbum);
			InteractionPlayerController.AddToolTip(EGameAction.PutCard);
			if (!m_CurrentSpawnedInteractableCard3d)
			{
				Card3dUIGroup cardUI = CSingleton<Card3dUISpawner>.Instance.GetCardUI();
				m_CurrentSpawnedInteractableCard3d = ShelfManager.SpawnInteractableObject(EObjectType.Card3d).GetComponent<InteractableCard3d>();
				cardUI.m_IgnoreCulling = true;
				cardUI.m_CardUIAnimGrp.gameObject.SetActive(value: true);
				cardUI.SetSimplifyCardDistanceCull(isCull: false);
				cardUI.m_CardUI.SetFoilCullListVisibility(isActive: true);
				cardUI.m_CardUI.ResetFarDistanceCull();
				cardUI.m_CardUI.SetFoilMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilMaterialTangentView);
				cardUI.m_CardUI.SetFoilBlendedMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilBlendedMaterialTangentView);
				cardUI.m_CardUI.SetCardUI(m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData());
				cardUI.transform.position = m_CurrentRaycastedInteractableCard3d.transform.position;
				cardUI.transform.rotation = m_CurrentRaycastedInteractableCard3d.transform.rotation;
				m_CurrentSpawnedInteractableCard3d.transform.position = m_CurrentRaycastedInteractableCard3d.transform.position;
				m_CurrentSpawnedInteractableCard3d.transform.rotation = m_CurrentRaycastedInteractableCard3d.transform.rotation;
				m_CurrentSpawnedInteractableCard3d.SetCardUIFollow(cardUI);
				m_CurrentSpawnedInteractableCard3d.LerpToTransform(CSingleton<InteractionPlayerController>.Instance.m_HoldCardCloseUpPos, CSingleton<InteractionPlayerController>.Instance.m_HoldCardCloseUpPos);
				m_CurrentSpawnedInteractableCard3d.SetEnableCollision(isEnable: false);
				m_CurrentSpawnedInteractableCard3d.m_CollectionBinderFlipAnimCtrl = this;
			}
			else if ((bool)m_CurrentSpawnedInteractableCard3d)
			{
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_IgnoreCulling = true;
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.SetSimplifyCardDistanceCull(isCull: false);
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUIAnimGrp.gameObject.SetActive(value: true);
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.SetFoilCullListVisibility(isActive: true);
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.ResetFarDistanceCull();
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.SetFoilMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilMaterialTangentView);
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.SetFoilBlendedMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilBlendedMaterialTangentView);
				m_CurrentSpawnedInteractableCard3d.StopLerpToTransform();
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.SetCardUI(m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData());
				m_CurrentSpawnedInteractableCard3d.transform.position = m_CurrentRaycastedInteractableCard3d.transform.position;
				m_CurrentSpawnedInteractableCard3d.transform.rotation = m_CurrentRaycastedInteractableCard3d.transform.rotation;
				m_CurrentSpawnedInteractableCard3d.gameObject.SetActive(value: true);
				m_CurrentSpawnedInteractableCard3d.LerpToTransform(CSingleton<InteractionPlayerController>.Instance.m_HoldCardCloseUpPos, CSingleton<InteractionPlayerController>.Instance.m_HoldCardCloseUpPos);
				m_CurrentSpawnedInteractableCard3d.SetEnableCollision(isEnable: false);
				m_CurrentSpawnedInteractableCard3d.m_CollectionBinderFlipAnimCtrl = this;
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.transform.position = m_CurrentSpawnedInteractableCard3d.transform.position;
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.transform.rotation = m_CurrentSpawnedInteractableCard3d.transform.rotation;
				m_CurrentSpawnedInteractableCard3d.m_Card3dUI.gameObject.SetActive(value: true);
			}
			m_CurrentViewInteractableCard3d = m_CurrentRaycastedInteractableCard3d;
			CardData cardData = m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
			if (CPlayerData.GetCardAmount(cardData) <= 1 || m_IsGradedCardAlbum)
			{
				m_CurrentViewInteractableCard3d.m_Card3dUI.gameObject.SetActive(value: false);
				m_CurrentViewInteractableCard3d.OnRaycastEnded();
			}
			if ((bool)m_CurrentRaycastedInteractableCard3d)
			{
				m_CurrentRaycastedInteractableCard3d.OnRaycastEnded();
				m_CurrentRaycastedInteractableCard3d = null;
			}
			m_CollectionBinderUI.CloseScreen();
			float cardMarketPrice = CPlayerData.GetCardMarketPrice(m_CurrentViewInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData());
			m_CardOpeningSequenceUI.ShowSingleCardValue(cardMarketPrice);
			m_CollectionBinderUI.m_CardNameText.text = InventoryBase.GetMonsterData(cardData.monsterType).GetName();
			m_CollectionBinderUI.m_CardFullRarityNameText.text = CPlayerData.GetFullCardTypeName(cardData);
			if (cardData.isFoil)
			{
				m_CollectionBinderUI.m_CardNameFoilAnim.Play("FoilText");
				m_CollectionBinderUI.m_CardFullRarityNameFoilAnim.Play("FoilText");
			}
			else
			{
				m_CollectionBinderUI.m_CardNameFoilAnim.Play("NoFoilText");
				m_CollectionBinderUI.m_CardFullRarityNameFoilAnim.Play("NoFoilText");
			}
			m_CollectionBinderUI.m_CardNameText.gameObject.SetActive(value: true);
			m_CollectionBinderUI.m_CardFullRarityNameText.gameObject.SetActive(value: true);
			m_BlackMaterialBGFade.SetFadeIn(3f, 0.2f);
			SoundManager.GenericPop();
			if (CSingleton<InteractionPlayerController>.Instance.IsPuttingCardOnDisplay())
			{
				CSingleton<InteractionPlayerController>.Instance.ExitViewCardAlbumMode();
			}
		}
	}

	private void ExitViewUpCloseState()
	{
		if (m_IsHoldingCardCloseUp && !m_IsExitingCardCloseUp)
		{
			CenterDot.SetVisibility(isVisible: true);
			InteractionPlayerController.RestoreHiddenToolTip();
			m_IsExitingCardCloseUp = true;
			m_InteractableCardFollowTransform.position = m_CurrentViewInteractableCard3d.m_Card3dUI.m_CardFrontMeshPos.transform.position;
			m_InteractableCardFollowTransform.rotation = m_CurrentViewInteractableCard3d.m_Card3dUI.m_CardFrontMeshPos.transform.rotation;
			m_CurrentSpawnedInteractableCard3d.SetTargetRotation(Quaternion.identity);
			Vector3 one = Vector3.one;
			if (m_IsGradedCardAlbum)
			{
				one *= 0.75f;
			}
			m_CurrentSpawnedInteractableCard3d.SetTargetLocalScale(one);
			m_CurrentSpawnedInteractableCard3d.SetTargetLocalPos(Vector3.zero);
			m_CurrentSpawnedInteractableCard3d.SetHideItemAfterFinishLerp();
			m_CurrentSpawnedInteractableCard3d.LerpToTransform(m_InteractableCardFollowTransform, m_InteractableCardFollowTransform);
			m_CollectionBinderUI.OpenScreen();
			m_CardOpeningSequenceUI.HideSingleCardValue();
			m_CollectionBinderUI.m_CardNameText.gameObject.SetActive(value: false);
			m_CollectionBinderUI.m_CardFullRarityNameText.gameObject.SetActive(value: false);
			m_BlackMaterialBGFade.SetFadeOut(12f);
			SoundManager.GenericPop(1f, 0.9f);
		}
	}

	public void OnCardFinishLerpHide()
	{
		m_IsHoldingCardCloseUp = false;
		m_IsExitingCardCloseUp = false;
		m_CurrentViewInteractableCard3d.m_Card3dUI.gameObject.SetActive(value: true);
	}

	public void OnCardRaycasted(InteractableCard3d interactableCard)
	{
		if (!m_IsHoldingCardCloseUp)
		{
			m_CurrentRaycastedInteractableCard3d = interactableCard;
			float cardMarketPrice = CPlayerData.GetCardMarketPrice(m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData());
			m_CardPricePopupUI.ShowCardPricePopup(cardMarketPrice, m_CurrentRaycastedInteractableCard3d.transform.position);
			m_CurrentRaycastedCardIndex = m_InteractableCard3dList.IndexOf(m_CurrentRaycastedInteractableCard3d);
			if (m_CanUpdateControllerCardIndex)
			{
				m_CurrentControllerCardIndex = m_ControllerSelectionCard3dList.IndexOf(m_CurrentRaycastedInteractableCard3d);
			}
		}
	}

	public void OnCardRaycastEnded()
	{
		m_CurrentRaycastedInteractableCard3d = null;
		m_CardPricePopupUI.HideCardPricePopup();
	}

	public void StartShowCardAlbum(Transform targetFollowPos)
	{
		if (!m_IsSorting && (bool)InteractionPlayerController.GetCurrentHoldCard())
		{
			m_IsHoldingCardCloseUp = false;
			InteractionPlayerController.RemoveAllCard();
			m_CanUpdateSort = true;
		}
		m_OpenBinder = true;
		m_IsInViewCardAlbumMode = true;
		m_IsLerpingPos = true;
		m_CanUpdateControllerCardIndex = true;
		m_CurrentControllerCardIndex = -1;
		StartCoroutine(DelayStopLerpPos());
		m_TargetFollowPos = targetFollowPos;
		m_TargetLerpPos = m_TargetFollowPos.position;
		m_TargetLerpRot = m_TargetFollowPos.rotation;
		base.transform.position = m_TargetFollowPos.position;
		base.transform.rotation = m_TargetFollowPos.rotation;
		m_ShowHideAnim.Play("ShowCardAlbum");
		base.gameObject.SetActive(value: true);
		if (m_CanFlipCoroutine != null)
		{
			StopCoroutine(m_CanFlipCoroutine);
		}
		m_CanFlipCoroutine = StartCoroutine(DelayResetCanFlipBook(CSingleton<InteractionPlayerController>.Instance.m_HideCardAlbumTime));
		SoundManager.PlayAudio("SFX_AlbumOpen", 0.6f);
		SoundManager.PlayAudio("SFX_WhipSoft", 0.6f);
		m_BinderPageGrpList[0].SetGradedCardMode(m_IsGradedCardAlbum);
		m_BinderPageGrpList[1].SetGradedCardMode(m_IsGradedCardAlbum);
		m_BinderPageGrpList[2].SetGradedCardMode(m_IsGradedCardAlbum);
		if (m_IsGradedCardAlbum)
		{
			m_AlbumMesh.material = m_GradedCardAlbumMat;
		}
		else
		{
			m_AlbumMesh.material = m_NormalAlbumMat;
		}
	}

	public void HideCardAlbum()
	{
		if (CSingleton<InteractionPlayerController>.Instance.IsSelectingCardForEditDeckScreen() || CSingleton<InteractionPlayerController>.Instance.IsSelectingCardForBulkDonationBoxScreen() || CSingleton<InteractionPlayerController>.Instance.IsSelectingCardForGradingScreen())
		{
			ExitViewUpCloseState();
			m_IsHoldingCardCloseUp = false;
		}
		if (m_IsHoldingCardCloseUp && (bool)m_CurrentSpawnedInteractableCard3d && InteractionPlayerController.HasEnoughSlotToHoldCard())
		{
			CenterDot.SetVisibility(isVisible: true);
			m_IsHoldingCardCloseUp = false;
			m_IsExitingCardCloseUp = false;
			m_CurrentSpawnedInteractableCard3d.m_CollectionBinderFlipAnimCtrl = null;
			m_CurrentSpawnedInteractableCard3d.SetTargetRotation(Quaternion.identity);
			m_CurrentSpawnedInteractableCard3d.SetTargetLocalScale(Vector3.one);
			m_CurrentSpawnedInteractableCard3d.SetTargetLocalPos(Vector3.zero);
			CardData cardData = m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
			if (m_IsGradedCardAlbum)
			{
				CPlayerData.RemoveGradedCard(cardData);
				m_CanUpdateSort = true;
			}
			else
			{
				CPlayerData.ReduceCard(cardData, 1);
			}
			InteractionPlayerController.AddHoldCard(m_CurrentSpawnedInteractableCard3d);
			CSingleton<InteractionPlayerController>.Instance.EnterHoldCardMode();
			m_CurrentSpawnedInteractableCard3d = null;
		}
		else if (!m_IsSorting)
		{
			CSingleton<InteractionPlayerController>.Instance.SetIsPuttingCardOnDisplay(isPuttingCardOnDisplay: false);
			if ((bool)InteractionPlayerController.GetCurrentHoldCard())
			{
				m_CurrentSpawnedInteractableCard3d = null;
				CSingleton<InteractionPlayerController>.Instance.EnterHoldCardMode();
			}
		}
		if ((bool)m_CurrentRaycastedInteractableCard3d)
		{
			m_CurrentRaycastedInteractableCard3d.OnRaycastEnded();
			m_CurrentRaycastedInteractableCard3d = null;
		}
		if (m_CollectionBinderUI.m_SortAlbumScreen.activeSelf)
		{
			m_CollectionBinderUI.CloseSortAlbumScreen();
		}
		m_OffsetPos = m_TargetFollowPos.position - base.transform.position;
		m_StartRot = base.transform.rotation;
		m_LerpTimer = 0f;
		m_IsLerpingPos = true;
		m_IsHidingAlbum = true;
		m_CloseBinder = true;
		m_ShowHideAnim.Play("HideCardAlbum");
		StartCoroutine(DelayStopLerpPos());
		StartCoroutine(DelayHideCardAlbum());
		if (m_CanFlipCoroutine != null)
		{
			StopCoroutine(m_CanFlipCoroutine);
		}
		m_CanFlipCoroutine = StartCoroutine(DelayResetCanFlipBook(CSingleton<InteractionPlayerController>.Instance.m_HideCardAlbumTime + 0.1f));
		m_CollectionBinderUI.CloseScreen();
		m_CardOpeningSequenceUI.HideSingleCardValue();
		m_CollectionBinderUI.m_CardNameText.gameObject.SetActive(value: false);
		m_CollectionBinderUI.m_CardFullRarityNameText.gameObject.SetActive(value: false);
		m_BlackMaterialBGFade.SetFadeOut(12f);
		SoundManager.PlayAudio("SFX_AlbumOpen", 0.6f);
		CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
		m_CanUpdateControllerCardIndex = true;
	}

	public void FlipNextPage()
	{
		if (!m_IsHoldingCardCloseUp)
		{
			m_GoNext = true;
		}
	}

	public void FlipNext10Page()
	{
		if (!m_IsHoldingCardCloseUp)
		{
			m_GoNext10 = true;
		}
	}

	public void FlipPreviousPage()
	{
		if (!m_IsHoldingCardCloseUp)
		{
			m_GoPrevious = true;
		}
	}

	public void FlipPrevious10Page()
	{
		if (!m_IsHoldingCardCloseUp)
		{
			m_GoPrevious10 = true;
		}
	}

	private IEnumerator DelayStopLerpPos()
	{
		yield return new WaitForSeconds(CSingleton<InteractionPlayerController>.Instance.m_HideCardAlbumTime - 0.1f);
		m_IsLerpingPos = false;
	}

	private IEnumerator DelayHideCardAlbum()
	{
		yield return new WaitForSeconds(CSingleton<InteractionPlayerController>.Instance.m_HideCardAlbumTime);
		m_IsInViewCardAlbumMode = false;
		base.transform.position = Vector3.one * -100000f;
		m_IsHidingAlbum = false;
		m_ShowHideAnim.gameObject.SetActive(value: false);
	}

	private IEnumerator DelaySetBinderPageVisibility(bool isVisible)
	{
		yield return new WaitForSeconds(0.5f);
		m_BinderPageGrpList[1].SetVisibility(isVisible);
		m_BinderPageGrpList[2].SetVisibility(isVisible);
	}

	private IEnumerator DelayResetCanFlipBook(float delayTime)
	{
		HideCurrentInteractableCard3dList();
		m_CanFlip = false;
		yield return new WaitForSeconds(delayTime);
		m_CanFlip = true;
		yield return new WaitForSeconds(0.1f);
		UpdateCurrentInteractableCard3dList();
	}

	private void HideCurrentInteractableCard3dList()
	{
		for (int i = 0; i < m_InteractableCard3dList.Count; i++)
		{
			m_InteractableCard3dList[i].gameObject.SetActive(value: false);
		}
	}

	private void UpdateCurrentInteractableCard3dList()
	{
		for (int i = 0; i < m_InteractableCard3dList.Count; i++)
		{
			m_InteractableCard3dList[i].m_Card3dUI = m_BinderPageGrpList[0].m_CardList[i];
			m_InteractableCard3dList[i].transform.position = m_BinderPageGrpList[0].m_CardList[i].transform.position;
			m_InteractableCard3dList[i].transform.rotation = m_BinderPageGrpList[0].m_CardList[i].transform.rotation;
			m_InteractableCard3dList[i].gameObject.SetActive(m_BinderPageGrpList[0].m_CardList[i].gameObject.activeSelf);
		}
		if (m_IsGradedCardAlbum)
		{
			for (int j = 0; j < m_InteractableCard3dList.Count; j++)
			{
				Vector3 localPosition = m_InteractableCard3dList[j].transform.localPosition;
				localPosition.z -= 0.01f;
				Vector3 one = Vector3.one;
				one.x = 0.93f;
				one.y = 1.1f;
				m_InteractableCard3dList[j].transform.localPosition = localPosition;
				m_InteractableCard3dList[j].transform.localScale = one;
			}
		}
		else
		{
			for (int k = 0; k < m_InteractableCard3dList.Count; k++)
			{
				m_InteractableCard3dList[k].transform.localScale = Vector3.one;
			}
		}
	}

	private void LateUpdate()
	{
		if (m_IsLerpingPos)
		{
			m_TargetLerpPos = m_TargetFollowPos.position;
			m_TargetLerpRot = m_TargetFollowPos.rotation;
			if (!m_IsHidingAlbum)
			{
				CSingleton<InteractionPlayerController>.Instance.m_CameraController.EnterViewCardAlbumMode();
			}
		}
		if (m_IsInViewCardAlbumMode)
		{
			if (m_IsHidingAlbum)
			{
				m_LerpTimer += Time.deltaTime / CSingleton<InteractionPlayerController>.Instance.m_HideCardAlbumTime;
				base.transform.position = m_TargetFollowPos.position - m_OffsetPos;
				base.transform.rotation = Quaternion.Lerp(m_StartRot, m_TargetFollowPos.rotation, m_LerpTimer);
			}
			else
			{
				base.transform.position = Vector3.Lerp(base.transform.position, m_TargetLerpPos, Time.deltaTime * 5f);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, m_TargetLerpRot, Time.deltaTime * 5f);
			}
		}
	}

	private void Update()
	{
		if (m_IsBookOpen && m_CanFlip && !m_CollectionBinderUI.IsSortAlbumScreenOpen() && !m_IsHoldingCardCloseUp && !CSingleton<CGameManager>.Instance.m_DisableController)
		{
			if (!CSingleton<InputManager>.Instance.m_IsControllerActive || InputManager.IsMovingRightThumbstick())
			{
				if (!CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled)
				{
					CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
					m_CanUpdateControllerCardIndex = true;
				}
			}
			else
			{
				if (InputManager.GetLeftAnalogDown(0, positiveValue: true) || InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_R))
				{
					CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
					m_CurrentControllerCardIndex++;
					if (m_CurrentControllerCardIndex >= m_ControllerSelectionCard3dList.Count)
					{
						m_CurrentControllerCardIndex = 0;
					}
					CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(m_ControllerSelectionCard3dList[m_CurrentControllerCardIndex].transform, 8f);
					m_CanUpdateControllerCardIndex = m_ControllerSelectionCard3dList[m_CurrentControllerCardIndex].gameObject.activeSelf;
				}
				else
				{
					InputManager.GetLeftAnalogUp(0, positiveValue: true);
				}
				if (InputManager.GetLeftAnalogDown(0, positiveValue: false) || InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_L))
				{
					CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
					if (m_CurrentControllerCardIndex == -1)
					{
						m_CurrentControllerCardIndex = 0;
					}
					else
					{
						m_CurrentControllerCardIndex--;
					}
					if (m_CurrentControllerCardIndex < 0)
					{
						m_CurrentControllerCardIndex = m_ControllerSelectionCard3dList.Count - 1;
					}
					CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(m_ControllerSelectionCard3dList[m_CurrentControllerCardIndex].transform, 8f);
					m_CanUpdateControllerCardIndex = m_ControllerSelectionCard3dList[m_CurrentControllerCardIndex].gameObject.activeSelf;
				}
				else
				{
					InputManager.GetLeftAnalogUp(0, positiveValue: false);
				}
				if (InputManager.GetLeftAnalogDown(1, positiveValue: false) || InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Down))
				{
					CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
					if (m_CurrentControllerCardIndex == -1)
					{
						m_CurrentControllerCardIndex = 0;
					}
					else if (m_CurrentControllerCardIndex >= m_ControllerSelectionCard3dList.Count / 2)
					{
						m_CurrentControllerCardIndex -= m_ControllerSelectionCard3dList.Count / 2;
					}
					else
					{
						m_CurrentControllerCardIndex += m_ControllerSelectionCard3dList.Count / 2;
					}
					CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(m_ControllerSelectionCard3dList[m_CurrentControllerCardIndex].transform, 8f);
					m_CanUpdateControllerCardIndex = m_ControllerSelectionCard3dList[m_CurrentControllerCardIndex].gameObject.activeSelf;
				}
				else
				{
					InputManager.GetLeftAnalogUp(1, positiveValue: false);
				}
				if (InputManager.GetLeftAnalogDown(1, positiveValue: true) || InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Up))
				{
					CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
					if (m_CurrentControllerCardIndex == -1)
					{
						m_CurrentControllerCardIndex = 0;
					}
					else if (m_CurrentControllerCardIndex >= m_ControllerSelectionCard3dList.Count / 2)
					{
						m_CurrentControllerCardIndex -= m_ControllerSelectionCard3dList.Count / 2;
					}
					else
					{
						m_CurrentControllerCardIndex += m_ControllerSelectionCard3dList.Count / 2;
					}
					CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(m_ControllerSelectionCard3dList[m_CurrentControllerCardIndex].transform, 8f);
					m_CanUpdateControllerCardIndex = m_ControllerSelectionCard3dList[m_CurrentControllerCardIndex].gameObject.activeSelf;
				}
				else
				{
					InputManager.GetLeftAnalogUp(1, positiveValue: true);
				}
			}
		}
		if (m_IsBookOpen && m_IsHoldingCardCloseUp && !m_IsExitingCardCloseUp)
		{
			float x = Mathf.Clamp(CSingleton<InteractionPlayerController>.Instance.m_CameraController.GetViewCardDeltaAngleX() * 1.5f, -15f, 15f);
			float num = Mathf.Clamp(CSingleton<InteractionPlayerController>.Instance.m_CameraController.GetViewCardDeltaAngleY(), -35f, 35f);
			Quaternion localRotation = m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_ScaleGrp.transform.localRotation;
			Vector3 eulerAngles = localRotation.eulerAngles;
			eulerAngles.x = x;
			eulerAngles.y = 0f - num;
			eulerAngles.z = 0f;
			localRotation.eulerAngles = eulerAngles;
			m_CurrentSpawnedInteractableCard3d.SetTargetRotation(localRotation);
			Vector3 one = Vector3.one;
			Vector3 zero = Vector3.zero;
			if (m_IsGradedCardAlbum)
			{
				one *= 0.66f;
				zero.y = -60f;
			}
			m_CurrentSpawnedInteractableCard3d.SetTargetLocalScale(one);
			m_CurrentSpawnedInteractableCard3d.SetTargetLocalPos(zero);
		}
		if (m_OpenBinder && !m_IsBookOpen)
		{
			m_ShowHideAnim.gameObject.SetActive(value: true);
			m_OpenBinder = false;
			m_IsBookOpen = true;
			m_BookAnim.SetTrigger("OpenBinder");
			m_BinderThicknessAnim.SetTrigger("OpenBinder");
			m_BinderPageGrpList[0].m_Anim.SetTrigger("OpenBinder");
			m_BinderPageGrpList[1].m_Anim.SetTrigger("SetHideNextIdle");
			m_BinderPageGrpList[2].m_Anim.SetTrigger("SetHidePreviousIdle");
			StartCoroutine(DelaySetBinderPageVisibility(isVisible: true));
			if (m_CanFlipCoroutine != null)
			{
				StopCoroutine(m_CanFlipCoroutine);
			}
			m_CanFlipCoroutine = StartCoroutine(DelayResetCanFlipBook(CSingleton<InteractionPlayerController>.Instance.m_HideCardAlbumTime + 0.3f));
			if (m_IsGradedCardAlbum)
			{
				if (CPlayerData.m_GradedCardInventoryList.Count == 0)
				{
					m_MaxIndex = 1;
				}
				else
				{
					m_MaxIndex = Mathf.CeilToInt((float)CPlayerData.m_GradedCardInventoryList.Count / 12f);
				}
				m_CollectionBinderUI.SetMaxPage(m_MaxIndex);
				m_CollectionBinderUI.SetCurrentPage(m_Index);
			}
			else
			{
				int num2 = InventoryBase.GetShownMonsterList(m_ExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType);
				if (m_ExpansionType == ECardExpansionType.Ghost)
				{
					num2 *= 2;
				}
				m_MaxIndex = Mathf.CeilToInt((float)num2 / 12f);
				m_CollectionBinderUI.SetMaxPage(m_MaxIndex);
				m_CollectionBinderUI.SetCurrentPage(m_Index);
				m_CollectionBinderUI.SetMaxCardCollectCount(num2);
			}
			if (m_IsGradedCardAlbum)
			{
				m_CollectionBinderUI.SetCardCollected(CPlayerData.m_GradedCardInventoryList.Count, ECardExpansionType.None);
				float num3 = 0f;
				for (int i = 0; i < CPlayerData.m_GradedCardInventoryList.Count; i++)
				{
					num3 += CPlayerData.GetCardMarketPrice(CPlayerData.GetGradedCardData(CPlayerData.m_GradedCardInventoryList[i]));
				}
				m_CollectionBinderUI.SetTotalValue(num3);
			}
			else if (m_ExpansionType == ECardExpansionType.Ghost)
			{
				m_CollectionBinderUI.SetCardCollected(CPlayerData.GetCardCollectedAmount(m_ExpansionType, isDimensionCard: false) + CPlayerData.GetCardCollectedAmount(m_ExpansionType, isDimensionCard: true), m_ExpansionType);
				m_CollectionBinderUI.SetTotalValue(CPlayerData.GetCardAlbumTotalValue(m_ExpansionType, isDimensionCard: false) + CPlayerData.GetCardAlbumTotalValue(m_ExpansionType, isDimensionCard: true));
			}
			else
			{
				m_CollectionBinderUI.SetCardCollected(CPlayerData.GetCardCollectedAmount(m_ExpansionType, isDimensionCard: false), m_ExpansionType);
				m_CollectionBinderUI.SetTotalValue(CPlayerData.GetCardAlbumTotalValue(m_ExpansionType, isDimensionCard: false));
			}
			m_CollectionBinderUI.OpenScreen();
			if (m_IsGradedCardAlbum)
			{
				m_SortingType = (ECollectionSortingType)CPlayerData.m_GradedCollectionSortingMethodIndex;
			}
			else
			{
				m_SortingType = (ECollectionSortingType)CPlayerData.m_CollectionSortingMethodIndexList[(int)m_ExpansionType];
			}
			if (m_SortingType < ECollectionSortingType.Default || m_SortingType >= ECollectionSortingType.MAX)
			{
				m_SortingType = ECollectionSortingType.Price;
				CPlayerData.m_CollectionSortingMethodIndexList[(int)m_ExpansionType] = (int)m_SortingType;
			}
			OnSortingMethodUpdated(backToFirstPage: false);
			if (!CPlayerData.m_HasGetGhostCard && (CPlayerData.GetCardCollectedAmount(ECardExpansionType.Ghost, isDimensionCard: false) > 0 || CPlayerData.GetCardCollectedAmount(ECardExpansionType.Ghost, isDimensionCard: true) > 0))
			{
				m_CollectionBinderUI.OpenGhostCardTutorialScreen();
			}
		}
		if (m_IsBookOpen)
		{
			if (m_CloseBinder)
			{
				m_IsBookOpen = false;
				m_BookAnim.Play("CollectionBookClose");
				m_BinderThicknessAnim.Play("CollectionBookClose");
				m_BinderPageGrpList[0].m_Anim.Play("BinderClose");
				m_BinderPageGrpList[1].m_Anim.SetTrigger("SetHideNextIdle");
				m_BinderPageGrpList[2].m_Anim.SetTrigger("SetHidePreviousIdle");
				m_BinderPageGrpList[1].SetVisibility(isVisible: false);
				m_BinderPageGrpList[2].SetVisibility(isVisible: false);
				m_CloseBinder = false;
				if (m_CanFlipCoroutine != null)
				{
					StopCoroutine(m_CanFlipCoroutine);
				}
				m_CanFlipCoroutine = StartCoroutine(DelayResetCanFlipBook(0.5f));
				SoundManager.PlayAudio("SFX_AlbumFlip", 0.6f);
			}
			if (!m_CanFlip)
			{
				m_GoNext = false;
				m_GoPrevious = false;
				m_GoNext10 = false;
				m_GoPrevious10 = false;
				return;
			}
			if (m_GoNext && m_Index < m_MaxIndex)
			{
				m_BinderPageGrpList[0].m_Anim.SetTrigger("GoNextPage");
				m_BinderPageGrpList[1].m_Anim.SetTrigger("GoNextPage");
				m_BinderPageGrpList[2].m_Anim.SetTrigger("SetHideNextIdle");
				BinderPageGrp item = m_BinderPageGrpList[0];
				m_BinderPageGrpList.RemoveAt(0);
				m_BinderPageGrpList.Add(item);
				m_GoNext = false;
				m_Index++;
				if (m_CanFlipCoroutine != null)
				{
					StopCoroutine(m_CanFlipCoroutine);
				}
				m_CanFlipCoroutine = StartCoroutine(DelayResetCanFlipBook(0.55f));
				m_CollectionBinderUI.SetCurrentPage(m_Index);
				SoundManager.PlayAudio("SFX_AlbumFlip", 0.6f);
				if (m_Index < m_MaxIndex)
				{
					if (m_IsGradedCardAlbum)
					{
						UpdateBinderGradedCardUI(1, m_Index + 1);
					}
					else
					{
						UpdateBinderAllCardUI(1, m_Index + 1);
					}
				}
			}
			if (m_GoPrevious && m_Index > 1)
			{
				m_BinderPageGrpList[2].m_Anim.SetTrigger("GoPreviousPage");
				m_BinderPageGrpList[1].m_Anim.SetTrigger("SetHidePreviousIdle");
				m_BinderPageGrpList[0].m_Anim.SetTrigger("GoPreviousPage");
				BinderPageGrp item2 = m_BinderPageGrpList[2];
				m_BinderPageGrpList.RemoveAt(2);
				m_BinderPageGrpList.Insert(0, item2);
				m_GoPrevious = false;
				m_Index--;
				if (m_CanFlipCoroutine != null)
				{
					StopCoroutine(m_CanFlipCoroutine);
				}
				m_CanFlipCoroutine = StartCoroutine(DelayResetCanFlipBook(0.55f));
				m_CollectionBinderUI.SetCurrentPage(m_Index);
				SoundManager.PlayAudio("SFX_AlbumFlip", 0.6f);
				if (m_Index > 1)
				{
					if (m_IsGradedCardAlbum)
					{
						UpdateBinderGradedCardUI(2, m_Index - 1);
					}
					else
					{
						UpdateBinderAllCardUI(2, m_Index - 1);
					}
				}
			}
			if (m_GoNext10 && m_Index < m_MaxIndex)
			{
				m_BinderPageGrpList[0].m_Anim.SetTrigger("GoNextPage");
				m_BinderPageGrpList[1].m_Anim.SetTrigger("GoNextPage");
				m_BinderPageGrpList[2].m_Anim.SetTrigger("SetHideNextIdle");
				BinderPageGrp item3 = m_BinderPageGrpList[0];
				m_BinderPageGrpList.RemoveAt(0);
				m_BinderPageGrpList.Add(item3);
				m_GoNext10 = false;
				m_Index += 10;
				if (m_Index > m_MaxIndex)
				{
					m_Index = m_MaxIndex;
				}
				if (m_CanFlipCoroutine != null)
				{
					StopCoroutine(m_CanFlipCoroutine);
				}
				m_CanFlipCoroutine = StartCoroutine(DelayResetCanFlipBook(0.55f));
				if (m_IsGradedCardAlbum)
				{
					UpdateBinderGradedCardUI(0, m_Index);
				}
				else
				{
					UpdateBinderAllCardUI(0, m_Index);
				}
				StartCoroutine(DelaySetBinderPageCardIndex(2, m_Index - 1));
				m_CollectionBinderUI.SetCurrentPage(m_Index);
				SoundManager.PlayAudio("SFX_AlbumFlip", 0.6f);
				if (m_Index < m_MaxIndex)
				{
					StartCoroutine(DelaySetBinderPageCardIndex(1, m_Index + 1));
				}
			}
			if (m_GoPrevious10 && m_Index > 1)
			{
				m_BinderPageGrpList[2].m_Anim.SetTrigger("GoPreviousPage");
				m_BinderPageGrpList[1].m_Anim.SetTrigger("SetHidePreviousIdle");
				m_BinderPageGrpList[0].m_Anim.SetTrigger("GoPreviousPage");
				BinderPageGrp item4 = m_BinderPageGrpList[2];
				m_BinderPageGrpList.RemoveAt(2);
				m_BinderPageGrpList.Insert(0, item4);
				m_GoPrevious10 = false;
				m_Index -= 10;
				if (m_Index < 1)
				{
					m_Index = 1;
				}
				if (m_CanFlipCoroutine != null)
				{
					StopCoroutine(m_CanFlipCoroutine);
				}
				m_CanFlipCoroutine = StartCoroutine(DelayResetCanFlipBook(0.55f));
				if (m_IsGradedCardAlbum)
				{
					UpdateBinderGradedCardUI(0, m_Index);
				}
				else
				{
					UpdateBinderAllCardUI(0, m_Index);
				}
				StartCoroutine(DelaySetBinderPageCardIndex(1, m_Index + 1));
				m_CollectionBinderUI.SetCurrentPage(m_Index);
				if (m_Index > 1)
				{
					StartCoroutine(DelaySetBinderPageCardIndex(2, m_Index - 1));
				}
				SoundManager.PlayAudio("SFX_AlbumFlip", 0.6f);
			}
		}
		m_OpenBinder = false;
		m_CloseBinder = false;
		m_GoNext = false;
		m_GoPrevious = false;
		m_GoNext10 = false;
		m_GoPrevious10 = false;
	}

	private IEnumerator DelaySetBinderPageCardIndex(int binderIndex, int pageIndex)
	{
		yield return new WaitForSeconds(0.5f);
		if (m_IsGradedCardAlbum)
		{
			UpdateBinderGradedCardUI(binderIndex, pageIndex);
		}
		else
		{
			UpdateBinderAllCardUI(binderIndex, pageIndex);
		}
	}

	public void OpenSortAlbumScreen()
	{
		if (m_CanFlip && !m_IsHoldingCardCloseUp && !m_IsExitingCardCloseUp)
		{
			CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
			m_CanUpdateControllerCardIndex = true;
			m_CollectionBinderUI.OpenSortAlbumScreen((int)m_SortingType, (int)m_ExpansionType, m_IsGradedCardAlbum);
		}
	}

	public void OpenExpansionSelectScreen()
	{
		if (m_CanFlip && !m_IsHoldingCardCloseUp && !m_IsExitingCardCloseUp)
		{
			m_CollectionBinderUI.OpenExpansionSelectScreen((int)m_ExpansionType, m_IsGradedCardAlbum);
			CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
			m_CanUpdateControllerCardIndex = true;
		}
	}

	public void OnPressSwitchSortingMethod(int sortingMethodIndex)
	{
		m_CanUpdateSort = true;
		SoundManager.GenericConfirm();
		m_SortingType = (ECollectionSortingType)sortingMethodIndex;
		if (m_IsGradedCardAlbum)
		{
			CPlayerData.m_GradedCollectionSortingMethodIndex = (int)m_SortingType;
		}
		else
		{
			CPlayerData.m_CollectionSortingMethodIndexList[(int)m_ExpansionType] = (int)m_SortingType;
		}
		StartCoroutine(DelayAlbumSort());
	}

	public void OnPressSwitchExpansion(int expansionIndex)
	{
		m_CanUpdateSort = true;
		SoundManager.GenericConfirm();
		m_IsGradedCardAlbum = false;
		m_ExpansionType = (ECardExpansionType)expansionIndex;
		m_SortingType = (ECollectionSortingType)CPlayerData.m_CollectionSortingMethodIndexList[(int)m_ExpansionType];
		StartCoroutine(DelayAlbumSort());
	}

	public void OnPressSwitchToGradedAlbum()
	{
		m_CanUpdateSort = true;
		SoundManager.GenericConfirm();
		m_IsGradedCardAlbum = true;
		m_SortingType = (ECollectionSortingType)CPlayerData.m_GradedCollectionSortingMethodIndex;
		StartCoroutine(DelayAlbumSort());
	}

	private IEnumerator DelayAlbumSort()
	{
		m_IsSorting = true;
		HideCardAlbum();
		yield return new WaitForSeconds(0.6f);
		OnSortingMethodUpdated(backToFirstPage: true);
		yield return new WaitForSeconds(0.05f);
		StartShowCardAlbum(m_TargetFollowPos);
		m_IsSorting = false;
	}

	private void SortByDefault()
	{
		m_SortedIndexList.Clear();
		m_SortTempList.Clear();
		int num = 0;
		if (m_IsGradedCardAlbum)
		{
			for (int i = 0; i < CPlayerData.m_GradedCardInventoryList.Count; i++)
			{
				m_SortedIndexList.Add(num);
				num++;
			}
			return;
		}
		for (int j = 0; j < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; j++)
		{
			for (int k = 0; k < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); k++)
			{
				m_SortedIndexList.Add(num);
				num++;
			}
		}
		if (m_ExpansionType != ECardExpansionType.Ghost)
		{
			return;
		}
		for (int l = 0; l < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; l++)
		{
			for (int m = 0; m < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); m++)
			{
				m_SortedIndexList.Add(num);
				num++;
			}
		}
	}

	private void SortByCardAmount()
	{
		m_SortedIndexList.Clear();
		m_SortTempList.Clear();
		int num = 0;
		if (m_IsGradedCardAlbum)
		{
			m_SortGradedCardDataList.Clear();
			m_SortGradedCardPrice.Clear();
			for (int i = 0; i < CPlayerData.m_GradedCardInventoryList.Count; i++)
			{
				m_SortGradedCardDataList.Add(CPlayerData.GetGradedCardData(CPlayerData.m_GradedCardInventoryList[i]));
				m_SortGradedCardPrice.Add(Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(m_SortGradedCardDataList[i]) * 100f));
			}
			for (int j = 0; j < CPlayerData.m_GradedCardInventoryList.Count; j++)
			{
				int index = m_SortedIndexList.Count;
				for (int k = 0; k < m_SortedIndexList.Count; k++)
				{
					int amount = CPlayerData.m_GradedCardInventoryList[j].amount;
					int amount2 = CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[k]].amount;
					if (amount > amount2)
					{
						index = k;
						break;
					}
					if (m_SortGradedCardPrice[j] > m_SortGradedCardPrice[m_SortedIndexList[k]] && amount >= amount2)
					{
						index = k;
						break;
					}
				}
				m_SortedIndexList.Insert(index, num);
				num++;
			}
			return;
		}
		for (int l = 0; l < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; l++)
		{
			for (int m = 0; m < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); m++)
			{
				int cardAmountByIndex = CPlayerData.GetCardAmountByIndex(num, m_ExpansionType, isDimensionCard: false);
				int index2 = m_SortTempList.Count;
				for (int n = 0; n < m_SortTempList.Count; n++)
				{
					if (cardAmountByIndex > m_SortTempList[n])
					{
						index2 = n;
						break;
					}
				}
				m_SortTempList.Insert(index2, cardAmountByIndex);
				m_SortedIndexList.Insert(index2, num);
				num++;
			}
		}
		if (m_ExpansionType != ECardExpansionType.Ghost)
		{
			return;
		}
		int num2 = 0;
		for (int num3 = 0; num3 < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; num3++)
		{
			for (int num4 = 0; num4 < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); num4++)
			{
				int cardAmountByIndex2 = CPlayerData.GetCardAmountByIndex(num2, m_ExpansionType, isDimensionCard: true);
				int index3 = m_SortTempList.Count;
				for (int num5 = 0; num5 < m_SortTempList.Count; num5++)
				{
					if (cardAmountByIndex2 > m_SortTempList[num5])
					{
						index3 = num5;
						break;
					}
				}
				m_SortTempList.Insert(index3, cardAmountByIndex2);
				m_SortedIndexList.Insert(index3, num);
				num++;
				num2++;
			}
		}
	}

	private void SortByPriceAmount()
	{
		m_SortedIndexList.Clear();
		m_SortTempList.Clear();
		int num = 0;
		if (m_IsGradedCardAlbum)
		{
			m_SortGradedCardDataList.Clear();
			m_SortGradedCardPrice.Clear();
			for (int i = 0; i < CPlayerData.m_GradedCardInventoryList.Count; i++)
			{
				m_SortGradedCardDataList.Add(CPlayerData.GetGradedCardData(CPlayerData.m_GradedCardInventoryList[i]));
				m_SortGradedCardPrice.Add(Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(m_SortGradedCardDataList[i]) * 100f));
			}
			for (int j = 0; j < CPlayerData.m_GradedCardInventoryList.Count; j++)
			{
				int index = m_SortedIndexList.Count;
				for (int k = 0; k < m_SortedIndexList.Count; k++)
				{
					if (m_SortGradedCardPrice[j] > m_SortGradedCardPrice[m_SortedIndexList[k]])
					{
						index = k;
						break;
					}
				}
				m_SortedIndexList.Insert(index, num);
				num++;
			}
			return;
		}
		for (int l = 0; l < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; l++)
		{
			for (int m = 0; m < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); m++)
			{
				CardData cardData = new CardData();
				cardData.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num, m_ExpansionType);
				cardData.borderType = (ECardBorderType)(num % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false));
				cardData.isFoil = num % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType) >= CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false);
				cardData.isDestiny = false;
				cardData.expansionType = m_ExpansionType;
				int cardAmount = CPlayerData.GetCardAmount(cardData);
				int num2 = 0;
				if (cardAmount > 0)
				{
					num2 = Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(cardData) * 100f);
				}
				int index2 = m_SortTempList.Count;
				for (int n = 0; n < m_SortTempList.Count; n++)
				{
					if (num2 > m_SortTempList[n])
					{
						index2 = n;
						break;
					}
				}
				m_SortTempList.Insert(index2, num2);
				m_SortedIndexList.Insert(index2, num);
				num++;
			}
		}
		if (m_ExpansionType != ECardExpansionType.Ghost)
		{
			return;
		}
		int num3 = 0;
		for (int num4 = 0; num4 < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; num4++)
		{
			for (int num5 = 0; num5 < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); num5++)
			{
				CardData cardData2 = new CardData();
				cardData2.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num3, m_ExpansionType);
				cardData2.borderType = (ECardBorderType)(num3 % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false));
				cardData2.isFoil = num3 % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType) >= CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false);
				cardData2.isDestiny = true;
				cardData2.expansionType = m_ExpansionType;
				int cardAmount2 = CPlayerData.GetCardAmount(cardData2);
				int num6 = 0;
				if (cardAmount2 > 0)
				{
					num6 = Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(cardData2) * 100f);
				}
				int index3 = m_SortTempList.Count;
				for (int num7 = 0; num7 < m_SortTempList.Count; num7++)
				{
					if (num6 > m_SortTempList[num7])
					{
						index3 = num7;
						break;
					}
				}
				m_SortTempList.Insert(index3, num6);
				m_SortedIndexList.Insert(index3, num);
				num++;
				num3++;
			}
		}
	}

	private void SortByDuplicatePriceAmount()
	{
		m_SortedIndexList.Clear();
		m_SortTempList.Clear();
		int num = 0;
		if (m_IsGradedCardAlbum)
		{
			for (int i = 0; i < CPlayerData.m_GradedCardInventoryList.Count; i++)
			{
				int index = m_SortedIndexList.Count;
				for (int j = 0; j < m_SortedIndexList.Count; j++)
				{
					if (CPlayerData.m_GradedCardInventoryList[i].expansionType == CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[j]].expansionType && CPlayerData.m_GradedCardInventoryList[i].isDestiny == CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[j]].isDestiny)
					{
						if (CPlayerData.m_GradedCardInventoryList[i].cardSaveIndex < CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[j]].cardSaveIndex)
						{
							index = j;
							break;
						}
						if (CPlayerData.m_GradedCardInventoryList[i].cardSaveIndex == CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[j]].cardSaveIndex && CPlayerData.m_GradedCardInventoryList[i].amount < CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[j]].amount)
						{
							index = j;
							break;
						}
					}
				}
				m_SortedIndexList.Insert(index, num);
				num++;
			}
			return;
		}
		for (int k = 0; k < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; k++)
		{
			for (int l = 0; l < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); l++)
			{
				CardData cardData = new CardData();
				cardData.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num, m_ExpansionType);
				cardData.borderType = (ECardBorderType)(num % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false));
				cardData.isFoil = num % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType) >= CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false);
				cardData.isDestiny = false;
				cardData.expansionType = m_ExpansionType;
				int cardAmount = CPlayerData.GetCardAmount(cardData);
				int num2 = 0;
				if (cardAmount > 1)
				{
					num2 = Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(cardData) * 100000f);
				}
				else if (cardAmount == 1)
				{
					num2 = Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(cardData) * 1f);
				}
				int index2 = m_SortTempList.Count;
				for (int m = 0; m < m_SortTempList.Count; m++)
				{
					if (num2 > m_SortTempList[m])
					{
						index2 = m;
						break;
					}
				}
				m_SortTempList.Insert(index2, num2);
				m_SortedIndexList.Insert(index2, num);
				num++;
			}
		}
		if (m_ExpansionType != ECardExpansionType.Ghost)
		{
			return;
		}
		int num3 = 0;
		for (int n = 0; n < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; n++)
		{
			for (int num4 = 0; num4 < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); num4++)
			{
				CardData cardData2 = new CardData();
				cardData2.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num3, m_ExpansionType);
				cardData2.borderType = (ECardBorderType)(num3 % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false));
				cardData2.isFoil = num3 % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType) >= CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false);
				cardData2.isDestiny = true;
				cardData2.expansionType = m_ExpansionType;
				int cardAmount2 = CPlayerData.GetCardAmount(cardData2);
				int num5 = 0;
				if (cardAmount2 > 1)
				{
					num5 = Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(cardData2) * 100000f);
				}
				else if (cardAmount2 == 1)
				{
					num5 = Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(cardData2) * 1f);
				}
				int index3 = m_SortTempList.Count;
				for (int num6 = 0; num6 < m_SortTempList.Count; num6++)
				{
					if (num5 > m_SortTempList[num6])
					{
						index3 = num6;
						break;
					}
				}
				m_SortTempList.Insert(index3, num5);
				m_SortedIndexList.Insert(index3, num);
				num++;
				num3++;
			}
		}
	}

	private void SortByTotalValueAmount()
	{
		m_SortedIndexList.Clear();
		m_SortTempList.Clear();
		int num = 0;
		if (m_IsGradedCardAlbum)
		{
			m_SortGradedCardDataList.Clear();
			m_SortGradedCardPrice.Clear();
			for (int i = 0; i < CPlayerData.m_GradedCardInventoryList.Count; i++)
			{
				m_SortGradedCardDataList.Add(CPlayerData.GetGradedCardData(CPlayerData.m_GradedCardInventoryList[i]));
				m_SortGradedCardPrice.Add(Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(m_SortGradedCardDataList[i]) * 100f));
			}
			for (int j = 0; j < CPlayerData.m_GradedCardInventoryList.Count; j++)
			{
				int index = m_SortedIndexList.Count;
				for (int k = 0; k < m_SortedIndexList.Count; k++)
				{
					if (m_SortGradedCardPrice[j] > m_SortGradedCardPrice[m_SortedIndexList[k]])
					{
						index = k;
						break;
					}
				}
				m_SortedIndexList.Insert(index, num);
				num++;
			}
			return;
		}
		for (int l = 0; l < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; l++)
		{
			for (int m = 0; m < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); m++)
			{
				CardData cardData = new CardData();
				cardData.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num, m_ExpansionType);
				cardData.borderType = (ECardBorderType)(num % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false));
				cardData.isFoil = num % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType) >= CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false);
				cardData.isDestiny = false;
				cardData.expansionType = m_ExpansionType;
				int cardAmount = CPlayerData.GetCardAmount(cardData);
				int num2 = 0;
				if (cardAmount > 0)
				{
					num2 = cardAmount * Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(cardData) * 100f);
				}
				int index2 = m_SortTempList.Count;
				for (int n = 0; n < m_SortTempList.Count; n++)
				{
					if (num2 > m_SortTempList[n])
					{
						index2 = n;
						break;
					}
				}
				m_SortTempList.Insert(index2, num2);
				m_SortedIndexList.Insert(index2, num);
				num++;
			}
		}
		if (m_ExpansionType != ECardExpansionType.Ghost)
		{
			return;
		}
		int num3 = 0;
		for (int num4 = 0; num4 < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; num4++)
		{
			for (int num5 = 0; num5 < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); num5++)
			{
				CardData cardData2 = new CardData();
				cardData2.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num3, m_ExpansionType);
				cardData2.borderType = (ECardBorderType)(num3 % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false));
				cardData2.isFoil = num3 % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType) >= CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false);
				cardData2.isDestiny = true;
				cardData2.expansionType = m_ExpansionType;
				int cardAmount2 = CPlayerData.GetCardAmount(cardData2);
				int num6 = 0;
				if (cardAmount2 > 0)
				{
					num6 = cardAmount2 * Mathf.RoundToInt(CPlayerData.GetCardMarketPrice(cardData2) * 100f);
				}
				int index3 = m_SortTempList.Count;
				for (int num7 = 0; num7 < m_SortTempList.Count; num7++)
				{
					if (num6 > m_SortTempList[num7])
					{
						index3 = num7;
						break;
					}
				}
				m_SortTempList.Insert(index3, num6);
				m_SortedIndexList.Insert(index3, num);
				num++;
				num3++;
			}
		}
	}

	private void SortByCardType(bool isCompactView)
	{
		m_SortedIndexList.Clear();
		m_SortTempList.Clear();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (m_IsGradedCardAlbum)
		{
			for (int i = 0; i < CPlayerData.m_GradedCardInventoryList.Count; i++)
			{
				int index = m_SortedIndexList.Count;
				for (int j = 0; j < m_SortedIndexList.Count; j++)
				{
					num2 = (CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_GradedCardInventoryList[i].cardSaveIndex) * 20 + CPlayerData.m_GradedCardInventoryList[i].amount;
					num3 = (CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[j]].cardSaveIndex) * 20 + CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[j]].amount;
					num2 += (int)(7 - CPlayerData.m_GradedCardInventoryList[i].expansionType) * CPlayerData.GetCardCollectionDataCount() * 100;
					num3 += (int)(7 - CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[j]].expansionType) * CPlayerData.GetCardCollectionDataCount() * 100;
					if (!CPlayerData.m_GradedCardInventoryList[i].isDestiny)
					{
						num2 += CPlayerData.GetCardCollectionDataCount();
					}
					if (!CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[j]].isDestiny)
					{
						num3 += CPlayerData.GetCardCollectionDataCount();
					}
					if (num2 > num3)
					{
						index = j;
						break;
					}
				}
				m_SortedIndexList.Insert(index, num);
				num++;
			}
			return;
		}
		for (int k = 0; k < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); k++)
		{
			for (int l = 0; l < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; l++)
			{
				num = l * CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType) + k;
				int cardAmountByIndex = CPlayerData.GetCardAmountByIndex(num, m_ExpansionType, isDimensionCard: false);
				if (isCompactView && cardAmountByIndex == 0)
				{
					m_SortTempList.Add(num);
				}
				else
				{
					m_SortedIndexList.Add(num);
				}
			}
		}
		if (m_ExpansionType == ECardExpansionType.Ghost)
		{
			int num4 = 0;
			for (int m = 0; m < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); m++)
			{
				for (int n = 0; n < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; n++)
				{
					num = n * CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType) + m + InventoryBase.GetShownMonsterList(m_ExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType);
					int cardAmountByIndex2 = CPlayerData.GetCardAmountByIndex(num4, m_ExpansionType, isDimensionCard: true);
					if (isCompactView && cardAmountByIndex2 == 0)
					{
						m_SortTempList.Add(num);
					}
					else
					{
						m_SortedIndexList.Add(num);
					}
					num4++;
				}
			}
		}
		for (int num5 = 0; num5 < m_SortTempList.Count; num5++)
		{
			m_SortedIndexList.Add(m_SortTempList[num5]);
		}
	}

	private void SortByCardRarity(bool isCompactView)
	{
		m_SortedIndexList.Clear();
		m_SortTempList.Clear();
		int num = 0;
		if (m_IsGradedCardAlbum)
		{
			m_SortGradedCardDataList.Clear();
			m_SortGradedCardPrice.Clear();
			for (int i = 0; i < CPlayerData.m_GradedCardInventoryList.Count; i++)
			{
				m_SortGradedCardDataList.Add(CPlayerData.GetGradedCardData(CPlayerData.m_GradedCardInventoryList[i]));
			}
			for (int j = 0; j < CPlayerData.m_GradedCardInventoryList.Count; j++)
			{
				int index = m_SortedIndexList.Count;
				for (int k = 0; k < m_SortedIndexList.Count; k++)
				{
					if (CPlayerData.m_GradedCardInventoryList[j].expansionType == CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[k]].expansionType && CPlayerData.m_GradedCardInventoryList[j].isDestiny == CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[k]].isDestiny)
					{
						ERarity rarity = InventoryBase.GetMonsterData(m_SortGradedCardDataList[j].monsterType).Rarity;
						ERarity rarity2 = InventoryBase.GetMonsterData(m_SortGradedCardDataList[m_SortedIndexList[k]].monsterType).Rarity;
						if (rarity < rarity2)
						{
							index = k;
							break;
						}
						if (rarity == rarity2 && CPlayerData.m_GradedCardInventoryList[j].cardSaveIndex < CPlayerData.m_GradedCardInventoryList[m_SortedIndexList[k]].cardSaveIndex)
						{
							index = k;
							break;
						}
					}
				}
				m_SortedIndexList.Insert(index, num);
				num++;
			}
			return;
		}
		for (int l = 0; l < 4; l++)
		{
			num = 0;
			for (int m = 0; m < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; m++)
			{
				for (int n = 0; n < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); n++)
				{
					if (InventoryBase.GetMonsterData(CPlayerData.GetMonsterTypeFromCardSaveIndex(num, m_ExpansionType)).Rarity == (ERarity)l)
					{
						int cardAmountByIndex = CPlayerData.GetCardAmountByIndex(num, m_ExpansionType, isDimensionCard: false);
						if (isCompactView && cardAmountByIndex == 0)
						{
							m_SortTempList.Add(num);
						}
						else
						{
							m_SortedIndexList.Add(num);
						}
					}
					num++;
				}
			}
		}
		if (m_ExpansionType == ECardExpansionType.Ghost)
		{
			for (int num2 = 0; num2 < 4; num2++)
			{
				num = 0;
				for (int num3 = 0; num3 < InventoryBase.GetShownMonsterList(m_ExpansionType).Count; num3++)
				{
					for (int num4 = 0; num4 < CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType); num4++)
					{
						if (InventoryBase.GetMonsterData(CPlayerData.GetMonsterTypeFromCardSaveIndex(num, m_ExpansionType)).Rarity == (ERarity)num2)
						{
							int cardAmountByIndex2 = CPlayerData.GetCardAmountByIndex(num, m_ExpansionType, isDimensionCard: true);
							if (isCompactView && cardAmountByIndex2 == 0)
							{
								m_SortTempList.Add(num + InventoryBase.GetShownMonsterList(m_ExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType));
							}
							else
							{
								m_SortedIndexList.Add(num + InventoryBase.GetShownMonsterList(m_ExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType));
							}
						}
						num++;
					}
				}
			}
		}
		for (int num5 = 0; num5 < m_SortTempList.Count; num5++)
		{
			m_SortedIndexList.Add(m_SortTempList[num5]);
		}
	}

	private void OnSortingMethodUpdated(bool backToFirstPage)
	{
		m_BinderPageGrpList[0].SetGradedCardMode(m_IsGradedCardAlbum);
		m_BinderPageGrpList[1].SetGradedCardMode(m_IsGradedCardAlbum);
		m_BinderPageGrpList[2].SetGradedCardMode(m_IsGradedCardAlbum);
		if (backToFirstPage)
		{
			m_Index = 1;
		}
		if (m_CanUpdateSort)
		{
			m_CanUpdateSort = false;
			switch (m_SortingType)
			{
			case ECollectionSortingType.Default:
				SortByDefault();
				break;
			case ECollectionSortingType.Amount:
				SortByCardAmount();
				break;
			case ECollectionSortingType.Price:
				SortByPriceAmount();
				break;
			case ECollectionSortingType.Type:
				SortByCardType(isCompactView: false);
				break;
			case ECollectionSortingType.Rarity:
				SortByCardRarity(isCompactView: false);
				break;
			case ECollectionSortingType.DuplicatePrice:
				SortByDuplicatePriceAmount();
				break;
			case ECollectionSortingType.TotalValue:
				SortByTotalValueAmount();
				break;
			}
		}
		if (m_IsGradedCardAlbum)
		{
			UpdateBinderGradedCardUI(0, m_Index);
			UpdateBinderGradedCardUI(1, m_Index + 1);
			UpdateBinderGradedCardUI(2, m_Index - 1);
		}
		else
		{
			UpdateBinderAllCardUI(0, m_Index);
			UpdateBinderAllCardUI(1, m_Index + 1);
			UpdateBinderAllCardUI(2, m_Index - 1);
		}
	}

	private void UpdateBinderAllCardUI(int binderIndex, int pageIndex)
	{
		for (int i = 0; i < m_BinderPageGrpList[binderIndex].m_CardList.Count; i++)
		{
			m_BinderPageGrpList[binderIndex].SetSingleCard(i, null, 0, m_SortingType);
		}
		if (pageIndex <= 0 || pageIndex > m_MaxIndex)
		{
			return;
		}
		for (int j = 0; j < m_BinderPageGrpList[binderIndex].m_CardList.Count; j++)
		{
			int num = (pageIndex - 1) * 12 + j;
			if (num >= m_SortedIndexList.Count)
			{
				m_BinderPageGrpList[binderIndex].SetSingleCard(j, null, 0, m_SortingType);
				continue;
			}
			int num2 = m_SortedIndexList[num];
			bool isDestiny = false;
			if (m_ExpansionType == ECardExpansionType.Ghost && num2 >= InventoryBase.GetShownMonsterList(m_ExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType))
			{
				isDestiny = true;
				num2 -= InventoryBase.GetShownMonsterList(m_ExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType);
			}
			CardData cardData = new CardData();
			cardData.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num2, m_ExpansionType);
			cardData.isFoil = num2 % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType) >= CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false);
			cardData.borderType = (ECardBorderType)(num2 % CPlayerData.GetCardAmountPerMonsterType(m_ExpansionType, includeFoilCount: false));
			cardData.isDestiny = isDestiny;
			cardData.expansionType = m_ExpansionType;
			int cardAmountByIndex = CPlayerData.GetCardAmountByIndex(num2, cardData.expansionType, cardData.isDestiny);
			m_BinderPageGrpList[binderIndex].SetSingleCard(j, cardData, cardAmountByIndex, m_SortingType);
		}
	}

	private void UpdateBinderGradedCardUI(int binderIndex, int pageIndex)
	{
		for (int i = 0; i < m_BinderPageGrpList[binderIndex].m_CardList.Count; i++)
		{
			m_BinderPageGrpList[binderIndex].SetSingleCard(i, null, 0, m_SortingType);
		}
		if (pageIndex <= 0 || pageIndex > m_MaxIndex)
		{
			return;
		}
		for (int j = 0; j < m_BinderPageGrpList[binderIndex].m_CardList.Count; j++)
		{
			int num = (pageIndex - 1) * 12 + j;
			if (num >= m_SortedIndexList.Count)
			{
				m_BinderPageGrpList[binderIndex].SetSingleCard(j, null, 0, m_SortingType);
				continue;
			}
			int num2 = m_SortedIndexList[num];
			if (num2 >= CPlayerData.m_GradedCardInventoryList.Count)
			{
				m_BinderPageGrpList[binderIndex].SetSingleCard(j, null, 0, m_SortingType);
				continue;
			}
			CardData gradedCardData = CPlayerData.GetGradedCardData(CPlayerData.m_GradedCardInventoryList[num2]);
			m_BinderPageGrpList[binderIndex].SetSingleCard(j, gradedCardData, 1, m_SortingType);
		}
	}

	public void SetCanUpdateSort(bool canSort)
	{
		m_CanUpdateSort = canSort;
	}
}
