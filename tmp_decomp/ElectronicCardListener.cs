using System.Collections;
using UnityEngine;

public class ElectronicCardListener : MonoBehaviour
{
	private Card3dUIGroup m_Card3dUI;

	private bool m_IsVisible;

	private Coroutine m_ShowCardCoroutine;

	protected void LateUpdate()
	{
		if ((bool)m_Card3dUI)
		{
			m_Card3dUI.transform.position = base.transform.position;
			m_Card3dUI.transform.rotation = base.transform.rotation;
		}
	}

	public void UpdateCardUI(CardData cardData)
	{
		if (m_Card3dUI == null)
		{
			Card3dUIGroup cardUI = CSingleton<Card3dUISpawner>.Instance.GetCardUI();
			cardUI.m_IgnoreCulling = true;
			cardUI.SetSimplifyCardDistanceCull(isCull: false);
			cardUI.m_CardUI.SetFoilCullListVisibility(isActive: true);
			cardUI.m_CardUI.ResetFarDistanceCull();
			cardUI.m_CardUIAnimGrp.gameObject.SetActive(value: true);
			cardUI.transform.position = base.transform.position;
			cardUI.transform.rotation = base.transform.rotation;
			cardUI.SetLocalScale(base.transform.localScale);
			cardUI.SetVisibility(isVisible: false);
			m_Card3dUI = cardUI;
		}
		if (m_ShowCardCoroutine != null)
		{
			StopCoroutine(m_ShowCardCoroutine);
		}
		if (cardData == null)
		{
			SoundManager.PlayAudio("SFX_MenuCloseB", 0.1f, 1.5f);
			m_Card3dUI.SetVisibility(isVisible: false);
			m_IsVisible = false;
		}
		else
		{
			m_Card3dUI.m_CardUI.SetCardUI(cardData);
			m_Card3dUI.m_GradedCardGrp.SetActive(value: false);
			m_ShowCardCoroutine = StartCoroutine(DelayShowCard());
			m_IsVisible = true;
		}
	}

	public void ShowCardInstantly()
	{
		if (m_ShowCardCoroutine != null)
		{
			StopCoroutine(m_ShowCardCoroutine);
		}
		m_Card3dUI.SetVisibility(isVisible: true);
		m_IsVisible = true;
	}

	private IEnumerator DelayShowCard()
	{
		yield return new WaitForSeconds(0.35f);
		SoundManager.PlayAudio("SFX_MenuCloseB", 0.1f, 0.5f);
		m_Card3dUI.SetVisibility(isVisible: true);
		yield return new WaitForSeconds(0.025f);
		m_Card3dUI.SetVisibility(isVisible: false);
		yield return new WaitForSeconds(0.035f);
		m_Card3dUI.SetVisibility(isVisible: true);
		yield return new WaitForSeconds(0.05f);
		m_Card3dUI.SetVisibility(isVisible: false);
		yield return new WaitForSeconds(0.1f);
		m_Card3dUI.SetVisibility(isVisible: true);
	}

	public void OnDestroyed()
	{
		if ((bool)m_Card3dUI)
		{
			m_Card3dUI.SetLocalScale(Vector3.one);
			m_Card3dUI.DisableCard();
		}
		m_Card3dUI = null;
	}

	public void OnPlacedMovedObject()
	{
		if ((bool)m_Card3dUI && m_IsVisible)
		{
			m_Card3dUI.SetVisibility(isVisible: true);
		}
	}

	public void OnStartMoveObject()
	{
		if ((bool)m_Card3dUI && m_IsVisible)
		{
			m_Card3dUI.SetVisibility(isVisible: true);
		}
	}

	public void BoxUpObject()
	{
		if ((bool)m_Card3dUI && m_IsVisible)
		{
			m_Card3dUI.SetVisibility(isVisible: false);
		}
	}
}
