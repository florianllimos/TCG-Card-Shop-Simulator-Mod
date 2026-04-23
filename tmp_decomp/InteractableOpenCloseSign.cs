using System.Collections;
using UnityEngine;

public class InteractableOpenCloseSign : InteractableObject
{
	public Animation m_Anim;

	public GameObject m_OpenShopMesh;

	public GameObject m_CloseShopMesh;

	private bool m_IsSwapping;

	private bool m_IsDayEnded;

	public override void OnMouseButtonUp()
	{
		if (CPlayerData.m_TutorialIndex < 5 && CPlayerData.m_ShopLevel < 1)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CannotOpenShopYet);
		}
		else if (!m_IsSwapping)
		{
			CPlayerData.m_IsShopOpen = !CPlayerData.m_IsShopOpen;
			if (!CPlayerData.m_IsShopOnceOpen && CPlayerData.m_IsShopOpen)
			{
				CPlayerData.m_IsShopOnceOpen = true;
				TutorialManager.AddTaskValue(ETutorialTaskCondition.OpenShop, 1f);
			}
			m_IsSwapping = true;
			m_Anim.Play();
			StartCoroutine(DelaySwapMesh());
			SoundManager.GenericPop();
		}
	}

	private IEnumerator DelaySwapMesh()
	{
		yield return new WaitForSeconds(0.6f);
		SoundManager.PlayAudio("SFX_WhipSoft", 0.4f, 1.2f);
		EvaluateSignOpenCloseMesh();
		yield return new WaitForSeconds(0.7f);
		m_IsSwapping = false;
	}

	private void EvaluateSignOpenCloseMesh()
	{
		m_OpenShopMesh.SetActive(CPlayerData.m_IsShopOpen);
		m_CloseShopMesh.SetActive(!CPlayerData.m_IsShopOpen);
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

	protected new void OnDisable()
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
		EvaluateSignOpenCloseMesh();
	}

	protected void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		m_IsDayEnded = false;
		CPlayerData.m_IsShopOpen = false;
		CPlayerData.m_IsShopOnceOpen = false;
		EvaluateSignOpenCloseMesh();
	}

	protected void OnDayEnded(CEventPlayer_OnDayEnded evt)
	{
		m_IsDayEnded = true;
	}
}
