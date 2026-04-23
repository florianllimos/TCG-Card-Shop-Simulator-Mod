using TMPro;
using UnityEngine;

public class CardOpeningSequenceUI : MonoBehaviour
{
	public GameObject m_ScreenGrp;

	public GameObject m_CardValueTextGrp;

	public GameObject m_FoilRainbowGlowingBG;

	public TextMeshProUGUI m_CardValueText;

	public GameObject m_TotalCardValueTextGrp;

	public TextMeshProUGUI m_TotalCardValueText;

	private float m_CurrentTotalCardValueLerp;

	private float m_TargetTotalCardValueLerp;

	private float m_TotalValueLerpTimer;

	private bool m_IsShowingTotalValue;

	public void ShowSingleCardValue(float cardValue)
	{
		if (!m_ScreenGrp.activeSelf)
		{
			m_ScreenGrp.SetActive(value: true);
		}
		m_CardValueText.text = GameInstance.GetPriceString(cardValue);
		m_CardValueTextGrp.SetActive(value: true);
	}

	public void HideSingleCardValue()
	{
		m_CardValueTextGrp.SetActive(value: false);
		m_ScreenGrp.SetActive(value: false);
	}

	public void StartShowTotalValue(float totalValue, bool hasFoilCard)
	{
		m_IsShowingTotalValue = true;
		m_TotalValueLerpTimer = 0f;
		m_CurrentTotalCardValueLerp = 0f;
		m_TotalCardValueText.text = GameInstance.GetPriceString(0f);
		m_TargetTotalCardValueLerp = totalValue;
		m_TotalCardValueTextGrp.SetActive(value: true);
		m_FoilRainbowGlowingBG.SetActive(hasFoilCard);
		SoundManager.SetEnableSound_ExpIncrease(isEnable: true);
		m_ScreenGrp.SetActive(value: true);
	}

	public void HideTotalValue()
	{
		m_TotalValueLerpTimer = 0f;
		m_CurrentTotalCardValueLerp = 0f;
		m_IsShowingTotalValue = false;
		m_TotalCardValueTextGrp.SetActive(value: false);
		m_FoilRainbowGlowingBG.SetActive(value: false);
		SoundManager.SetEnableSound_ExpIncrease(isEnable: false);
		m_ScreenGrp.SetActive(value: false);
	}

	private void Update()
	{
		if (m_IsShowingTotalValue)
		{
			m_TotalValueLerpTimer += Time.deltaTime * 0.5f;
			m_CurrentTotalCardValueLerp = Mathf.Lerp(0f, m_TargetTotalCardValueLerp, m_TotalValueLerpTimer);
			m_TotalCardValueText.text = GameInstance.GetPriceString(m_CurrentTotalCardValueLerp);
			if (m_TotalValueLerpTimer >= 1f)
			{
				m_IsShowingTotalValue = false;
				SoundManager.SetEnableSound_ExpIncrease(isEnable: false);
			}
		}
	}
}
