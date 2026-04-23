public class ConfirmTrashScreen : UIScreenBase
{
	protected override void OnOpenScreen()
	{
		SoundManager.GenericMenuOpen();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: true);
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		base.OnOpenScreen();
	}

	protected override void OnCloseScreen()
	{
		SoundManager.GenericMenuClose();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		base.OnCloseScreen();
	}

	public void OnPressConfirmBtn()
	{
		CSingleton<InteractionPlayerController>.Instance.ConfirmDiscardBox();
		SoundManager.GenericConfirm();
		CloseScreen();
	}

	public void OnPressCancelBtn()
	{
		SoundManager.GenericCancel();
		CloseScreen();
	}
}
