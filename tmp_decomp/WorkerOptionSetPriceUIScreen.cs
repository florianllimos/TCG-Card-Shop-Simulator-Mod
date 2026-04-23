using TMPro;
using UnityEngine;

public class WorkerOptionSetPriceUIScreen : MonoBehaviour
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public GameObject m_ScreenGrp;

	public GameObject m_RoundUpPriceTickImage;

	public TextMeshProUGUI m_OptionText;

	public TextMeshProUGUI m_MultiplierPercentText;

	private Worker m_Worker;

	private bool m_IsRoundUpPrice = true;

	private float m_SetPricePercentMultiplier = 1.1f;

	public Color m_PositiveColor;

	public Color m_NegativeColor;

	private EWorkerTask m_TaskToSet = EWorkerTask.SetPrice;

	public void OpenScreen(Worker worker, EWorkerTask task)
	{
		m_Worker = worker;
		m_TaskToSet = task;
		if (m_TaskToSet == EWorkerTask.SetPrice)
		{
			m_IsRoundUpPrice = m_Worker.GetIsRoundUpPrice();
			m_SetPricePercentMultiplier = m_Worker.GetPriceMultiplier();
		}
		else if (m_TaskToSet == EWorkerTask.RestockCardDisplay)
		{
			m_IsRoundUpPrice = m_Worker.GetIsRoundUpCardPrice();
			m_SetPricePercentMultiplier = m_Worker.GetCardPriceMultiplier();
		}
		EvaluateUI();
		m_ScreenGrp.SetActive(value: true);
		SoundManager.GenericMenuOpen();
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension);
	}

	public void CloseScreen()
	{
		m_ScreenGrp.SetActive(value: false);
		SoundManager.GenericMenuClose();
		ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
	}

	public void OnPressToggleRoundUpPrice()
	{
		m_IsRoundUpPrice = !m_IsRoundUpPrice;
		EvaluateUI();
		SoundManager.GenericConfirm();
	}

	public void OnPressAddPercent()
	{
		m_SetPricePercentMultiplier += 0.05f;
		EvaluateUI();
		SoundManager.GenericConfirm();
	}

	public void OnPressReducePercent()
	{
		m_SetPricePercentMultiplier -= 0.05f;
		if (m_SetPricePercentMultiplier < 0f)
		{
			m_SetPricePercentMultiplier = 0f;
		}
		EvaluateUI();
		SoundManager.GenericConfirm();
	}

	private void EvaluateUI()
	{
		m_RoundUpPriceTickImage.SetActive(m_IsRoundUpPrice);
		if (m_SetPricePercentMultiplier >= 0.999f)
		{
			m_MultiplierPercentText.text = "+" + Mathf.RoundToInt((m_SetPricePercentMultiplier - 1f) * 100f) + "%";
			m_MultiplierPercentText.color = m_PositiveColor;
		}
		else if (m_SetPricePercentMultiplier < 1f)
		{
			m_MultiplierPercentText.text = Mathf.RoundToInt((m_SetPricePercentMultiplier - 1f) * 100f) + "%";
			m_MultiplierPercentText.color = m_NegativeColor;
		}
	}

	public void OnPressConfirm()
	{
		if (m_TaskToSet == EWorkerTask.SetPrice)
		{
			m_Worker.UpdateSetPriceOption(m_IsRoundUpPrice, m_SetPricePercentMultiplier);
		}
		else if (m_TaskToSet == EWorkerTask.RestockCardDisplay)
		{
			m_Worker.UpdateSetCardPriceOption(m_IsRoundUpPrice, m_SetPricePercentMultiplier);
		}
		if (m_Worker.GetIsSetTaskSettingPrimarySecondary())
		{
			m_Worker.SetTask(m_TaskToSet);
			m_Worker.SetLastTask(m_TaskToSet);
		}
		else
		{
			m_Worker.SetSecondaryTask(m_TaskToSet);
		}
		SoundManager.GenericConfirm();
		m_Worker.OnPressStopInteract();
		CloseScreen();
	}
}
