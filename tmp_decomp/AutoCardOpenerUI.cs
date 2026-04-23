using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoCardOpenerUI : MonoBehaviour
{
	public GameObject m_ScreenGrp;

	public GameObject m_IdleGrp;

	public GameObject m_ProcessingGrp;

	public GameObject m_DoneGrp;

	public TextMeshProUGUI m_PackCountText;

	public TextMeshProUGUI m_ProcessingTimeLeft;

	public Image m_IdleFillBar;

	public Image m_ProcessingFillBar;

	private Transform m_FollowTargetMachine;

	private InteractableAutoPackOpener m_CurrentAutoPackOpener;

	private void Start()
	{
	}

	public void SetUIState(int index)
	{
		switch (index)
		{
		case 2:
			m_IdleGrp.SetActive(value: false);
			m_ProcessingGrp.SetActive(value: false);
			m_DoneGrp.SetActive(value: true);
			break;
		case 1:
			m_IdleGrp.SetActive(value: false);
			m_ProcessingGrp.SetActive(value: true);
			m_DoneGrp.SetActive(value: false);
			break;
		default:
			m_IdleGrp.SetActive(value: true);
			m_ProcessingGrp.SetActive(value: false);
			m_DoneGrp.SetActive(value: false);
			break;
		}
	}

	public void UpdatePackCountText(int currentCount, int maxCount)
	{
		if (maxCount <= 0)
		{
			m_IdleFillBar.fillAmount = 0f;
		}
		else
		{
			m_IdleFillBar.fillAmount = (float)currentCount / (float)maxCount;
		}
		m_PackCountText.text = currentCount + " / " + maxCount;
	}

	public void UpdateProcessingFillBar(float fillAmount)
	{
		m_ProcessingFillBar.fillAmount = fillAmount;
	}

	public void UpdateProcessingTimeLeftText(float timeLeft)
	{
		m_ProcessingTimeLeft.text = Mathf.FloorToInt(timeLeft / 60f) + "H " + Mathf.RoundToInt(timeLeft) % 60 + "M";
	}

	public void SetVisibility(bool isVisible)
	{
		m_ScreenGrp.gameObject.SetActive(isVisible);
	}
}
