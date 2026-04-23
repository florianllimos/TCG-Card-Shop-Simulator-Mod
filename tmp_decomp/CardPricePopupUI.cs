using TMPro;
using UnityEngine;

public class CardPricePopupUI : MonoBehaviour
{
	public Transform m_UIGrp;

	public TextMeshProUGUI m_PriceText;

	public Transform m_TargetLookAt;

	public bool m_IsActive;

	private void Start()
	{
		m_TargetLookAt = CSingleton<InteractionPlayerController>.Instance.m_Cam.transform;
		HideCardPricePopup();
	}

	private void Update()
	{
		if (m_IsActive)
		{
			m_UIGrp.LookAt(m_TargetLookAt.position);
		}
	}

	public void ShowCardPricePopup(float price, Vector3 targetPos)
	{
		m_UIGrp.gameObject.SetActive(value: false);
		m_UIGrp.LookAt(m_TargetLookAt.position);
		m_UIGrp.gameObject.SetActive(value: true);
		m_PriceText.text = GameInstance.GetPriceString(price);
		base.transform.position = targetPos;
		m_IsActive = true;
		SoundManager.GenericPop(0.7f, 0.8f);
	}

	public void HideCardPricePopup()
	{
		m_UIGrp.gameObject.SetActive(value: false);
		m_IsActive = false;
	}
}
