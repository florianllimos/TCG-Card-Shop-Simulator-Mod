using I2.Loc;
using TMPro;

public class ConfirmSellShelfScreen : UIScreenBase
{
	public TextMeshProUGUI m_DisplayText;

	public string m_Text = "Are you sure about selling XXX for YYY?";

	protected override void OnOpenScreen()
	{
		SoundManager.GenericMenuOpen();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: true);
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		base.OnOpenScreen();
	}

	protected override void OnCloseScreen()
	{
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		SoundManager.GenericMenuClose();
		base.OnCloseScreen();
	}

	public void SetFurnitureData(FurniturePurchaseData data)
	{
		string text = LocalizationManager.GetTranslation(m_Text).Replace("XXX", data.GetName());
		text = text.Replace("YYY", GameInstance.GetPriceString(data.price / 2f));
		m_DisplayText.text = text;
	}

	public void OnPressConfirmBtn()
	{
		SoundManager.GenericConfirm();
		CSingleton<InteractionPlayerController>.Instance.ConfirmSellFurniture();
		CloseScreen();
	}

	public void OnPressCancelBtn()
	{
		SoundManager.GenericCancel();
		CloseScreen();
	}
}
