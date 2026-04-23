using UnityEngine;

public class SetCardStorageShelfSettingScreen : UIScreenBase
{
	public GameObject m_CanWorkerTakeTick;

	private bool m_CanWorkerTake;

	private InteractableCardStorageShelf m_CurrentCardStorageShelf;

	protected override void OnOpenScreen()
	{
		SoundManager.GenericMenuOpen();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: true);
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		base.OnOpenScreen();
	}

	protected override void OnCloseScreen()
	{
		m_CurrentCardStorageShelf.OnCardStorageShelfSettingDone();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		SoundManager.GenericMenuClose();
		base.OnCloseScreen();
	}

	public void OnPressToggleCanWorkerTake()
	{
		m_CanWorkerTake = !m_CanWorkerTake;
		m_CanWorkerTakeTick.SetActive(m_CanWorkerTake);
		m_CurrentCardStorageShelf.SetCanWorkerTake(m_CanWorkerTake);
		SoundManager.GenericConfirm();
	}

	public void SetCardStorageShelf(InteractableCardStorageShelf cardStorageShelf)
	{
		m_CurrentCardStorageShelf = cardStorageShelf;
		m_CanWorkerTake = cardStorageShelf.CanWorkerTake();
		m_CanWorkerTakeTick.SetActive(m_CanWorkerTake);
	}
}
