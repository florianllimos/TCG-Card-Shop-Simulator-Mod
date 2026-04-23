using UnityEngine;

public class SetCashierCounterSettingScreen : UIScreenBase
{
	public GameObject m_CanCheckoutTick;

	public GameObject m_CanTradeTick;

	private bool m_CanCheckout;

	private bool m_CanTrade;

	private InteractableCashierCounter m_CurrentCashierCounter;

	protected override void OnOpenScreen()
	{
		SoundManager.GenericMenuOpen();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: true);
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		base.OnOpenScreen();
	}

	protected override void OnCloseScreen()
	{
		m_CurrentCashierCounter.OnCashierSettingDone();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		SoundManager.GenericMenuClose();
		base.OnCloseScreen();
	}

	public void OnPressToggleCheckout()
	{
		m_CanCheckout = !m_CanCheckout;
		m_CanCheckoutTick.SetActive(m_CanCheckout);
		m_CurrentCashierCounter.SetCanCheckout(m_CanCheckout);
		SoundManager.GenericConfirm();
	}

	public void OnPressToggleTrade()
	{
		m_CanTrade = !m_CanTrade;
		m_CanTradeTick.SetActive(m_CanTrade);
		m_CurrentCashierCounter.SetCanTradeCard(m_CanTrade);
		SoundManager.GenericConfirm();
	}

	public void SetCashierCounter(InteractableCashierCounter cashCounter)
	{
		m_CurrentCashierCounter = cashCounter;
		m_CanCheckout = cashCounter.CanCheckout();
		m_CanTrade = cashCounter.CanTradeCard();
		m_CanCheckoutTick.SetActive(m_CanCheckout);
		m_CanTradeTick.SetActive(m_CanTrade);
	}
}
