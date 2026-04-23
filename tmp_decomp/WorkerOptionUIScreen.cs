using TMPro;
using UnityEngine;

public class WorkerOptionUIScreen : MonoBehaviour
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public GameObject m_ScreenGrp;

	public TextMeshProUGUI m_OptionText;

	private Worker m_Worker;

	private int m_TaskIndex;

	public void OpenScreen(Worker worker, int taskIndex)
	{
		m_Worker = worker;
		m_TaskIndex = taskIndex;
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

	public void OnPressRestockShelfWithNoLabel(bool isFillShelfWithoutLabel)
	{
		m_Worker.SetRestockShelfWithNoLabel(isFillShelfWithoutLabel);
		if (m_Worker.GetIsSetTaskSettingPrimarySecondary())
		{
			m_Worker.SetTask((EWorkerTask)m_TaskIndex);
			m_Worker.SetLastTask((EWorkerTask)m_TaskIndex);
		}
		else
		{
			m_Worker.SetSecondaryTask((EWorkerTask)m_TaskIndex);
		}
		SoundManager.GenericConfirm();
		m_Worker.OnPressStopInteract();
		CloseScreen();
	}
}
