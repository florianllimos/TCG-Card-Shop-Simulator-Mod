public class ConfirmGoNextDayScreen : UIScreenBase
{
	private bool m_CanOpenScreen = true;

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
		base.OnCloseScreen();
	}

	public void OnPressConfirmBtn()
	{
		CEventManager.AddListener<CEventPlayer_OnDayStarted>(OnDayStarted);
		m_CanOpenScreen = false;
		CSingleton<InteractionPlayerController>.Instance.ShowGoNextDayScreen();
		SoundManager.GenericConfirm();
		CloseScreen();
	}

	public void OnPressCancelBtn()
	{
		SoundManager.GenericCancel();
		CloseScreen();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
	}

	protected void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		CEventManager.RemoveListener<CEventPlayer_OnDayStarted>(OnDayStarted);
		m_CanOpenScreen = true;
	}

	public bool CanOpenScreen()
	{
		return m_CanOpenScreen;
	}
}
