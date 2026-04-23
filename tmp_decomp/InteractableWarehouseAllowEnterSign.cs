using System.Collections;
using UnityEngine;

public class InteractableWarehouseAllowEnterSign : InteractableObject
{
	public Animation m_Anim;

	public GameObject m_OpenShopMesh;

	public GameObject m_CloseShopMesh;

	private bool m_IsSwapping;

	private bool m_IsDayEnded;

	public override void OnMouseButtonUp()
	{
		if (!m_IsSwapping)
		{
			CPlayerData.m_IsWarehouseDoorClosed = !CPlayerData.m_IsWarehouseDoorClosed;
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
		m_OpenShopMesh.SetActive(CPlayerData.m_IsWarehouseDoorClosed);
		m_CloseShopMesh.SetActive(!CPlayerData.m_IsWarehouseDoorClosed);
		CSingleton<UnlockRoomManager>.Instance.EvaluateWarehouseRoomOpenClose();
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected new void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		EvaluateSignOpenCloseMesh();
	}
}
