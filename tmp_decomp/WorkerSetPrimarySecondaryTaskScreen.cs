using UnityEngine;

public class WorkerSetPrimarySecondaryTaskScreen : MonoBehaviour
{
	public WorkerInteractUIScreen m_WorkerInteractUIScreen;

	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public GameObject m_ScreenGrp;

	private Worker m_Worker;

	private int m_TaskIndex;

	public void OpenScreen()
	{
		m_ScreenGrp.SetActive(value: true);
		SoundManager.GenericMenuOpen();
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension);
	}

	public void CloseScreen()
	{
		m_ScreenGrp.SetActive(value: false);
		ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
	}

	public void OnPressSetTaskPrimarySecondary(bool isPrimary)
	{
		m_WorkerInteractUIScreen.SetTaskAsPrimaryOrSecondary(isPrimary);
		CloseScreen();
	}
}
