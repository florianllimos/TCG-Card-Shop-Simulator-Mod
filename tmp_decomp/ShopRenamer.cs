using System.Collections;
using TMPro;
using UnityEngine;

public class ShopRenamer : MonoBehaviour
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension_IntroScreen;

	public ControllerScreenUIExtension m_ControllerScreenUIExtension_NameScreen;

	public ControllerScreenUIExtension m_ControllerScreenUIExtension_ConfirmNameScreen;

	public Transform m_FollowTarget;

	public GameObject m_IntroScreen;

	public GameObject m_NameScreen;

	public GameObject m_ConfirmNameScreen;

	public TMP_InputField m_SetNameInput;

	public TextMeshProUGUI m_SetNameInputDisplay;

	public TextMeshProUGUI m_SetName;

	public TextMeshProUGUI m_ShopName;

	private bool m_IsInArea;

	private bool m_IsTutorial;

	public void SetIsTutorial()
	{
		m_IsTutorial = true;
		m_ShopName.text = "";
		CPlayerData.PlayerName = "My Card Shop";
		GameUIScreen.SetGameUIVisible(isVisible: false);
		InteractionPlayerController.AddToolTip(EGameAction.MoveForward);
		InteractionPlayerController.AddToolTip(EGameAction.MoveLeft);
		InteractionPlayerController.AddToolTip(EGameAction.MoveBackward);
		InteractionPlayerController.AddToolTip(EGameAction.MoveRight);
		InteractionPlayerController.AddToolTip(EGameAction.Jump);
		InteractionPlayerController.AddToolTip(EGameAction.Sprint);
		InteractionPlayerController.AddToolTip(EGameAction.Crouch);
	}

	private IEnumerator DelayShowToolTip()
	{
		yield return new WaitForSeconds(0.1f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!m_IsInArea && m_IsTutorial)
		{
			m_IsInArea = true;
			ShowRenameShopScreen(showIntro: true);
			CSingleton<TutorialManager>.Instance.m_TutorialTargetIndicator.SetActive(value: false);
			InteractionPlayerController.RemoveToolTip(EGameAction.MoveForward);
			InteractionPlayerController.RemoveToolTip(EGameAction.MoveLeft);
			InteractionPlayerController.RemoveToolTip(EGameAction.MoveBackward);
			InteractionPlayerController.RemoveToolTip(EGameAction.MoveRight);
			InteractionPlayerController.RemoveToolTip(EGameAction.Jump);
			InteractionPlayerController.RemoveToolTip(EGameAction.Sprint);
			InteractionPlayerController.RemoveToolTip(EGameAction.Crouch);
			ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension_IntroScreen);
		}
	}

	public void ShowRenameShopScreen(bool showIntro)
	{
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		CSingleton<InteractionPlayerController>.Instance.ForceLookAt(m_FollowTarget, 3f);
		InteractionPlayerController.SetPlayerPos(m_FollowTarget.position);
		CSingleton<InteractionPlayerController>.Instance.EnterLockMoveMode();
		GameUIScreen.SetGameUIVisible(isVisible: false);
		if (showIntro)
		{
			m_IntroScreen.SetActive(value: true);
			return;
		}
		InteractionPlayerController.RemoveToolTip(EGameAction.Interact);
		m_IsTutorial = false;
		OnPressGoNextButton();
		m_SetNameInput.text = CPlayerData.PlayerName;
	}

	public void OnPressGoNextButton()
	{
		m_IntroScreen.SetActive(value: false);
		m_NameScreen.SetActive(value: true);
		m_SetName.text = CPlayerData.PlayerName;
		m_SetNameInputDisplay.text = CPlayerData.PlayerName;
		m_ShopName.text = CPlayerData.PlayerName;
		m_SetNameInputDisplay.gameObject.SetActive(!m_IsTutorial);
		ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension_IntroScreen);
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension_NameScreen);
	}

	public void OnPressRenameButton()
	{
		GameUIScreen.SetGameUIVisible(isVisible: false);
		m_SetName.text = CPlayerData.PlayerName;
		m_SetNameInputDisplay.text = CPlayerData.PlayerName;
		m_ShopName.text = CPlayerData.PlayerName;
		m_NameScreen.SetActive(value: true);
		m_ConfirmNameScreen.SetActive(value: false);
		m_SetNameInputDisplay.gameObject.SetActive(!m_IsTutorial);
		ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension_ConfirmNameScreen);
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension_NameScreen);
	}

	public void OnPressSetNameButton()
	{
		if (!string.IsNullOrWhiteSpace(CPlayerData.PlayerName))
		{
			m_NameScreen.SetActive(value: false);
			m_ConfirmNameScreen.SetActive(value: true);
			ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension_NameScreen);
			ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension_ConfirmNameScreen);
		}
	}

	public void OnInputChanged(string text)
	{
		m_SetNameInputDisplay.text = text;
		m_ShopName.text = text;
		m_SetNameInputDisplay.gameObject.SetActive(value: true);
		m_SetName.gameObject.SetActive(value: false);
		CPlayerData.PlayerName = text;
	}

	public void OnInputTextSelected(string text)
	{
		m_SetNameInputDisplay.gameObject.SetActive(value: true);
		m_SetNameInput.text = text;
		m_SetName.gameObject.SetActive(value: false);
	}

	public void OnInputTextUpdated(string text)
	{
		m_SetNameInput.text = text;
		m_SetNameInputDisplay.text = text;
		m_SetName.text = text;
		m_ShopName.text = text;
		m_SetNameInputDisplay.gameObject.SetActive(value: false);
		m_SetName.gameObject.SetActive(value: true);
		CPlayerData.PlayerName = text;
	}

	public void OnPressConfirmShopName()
	{
		m_IntroScreen.SetActive(value: false);
		m_NameScreen.SetActive(value: false);
		m_ConfirmNameScreen.SetActive(value: false);
		base.gameObject.SetActive(value: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		CSingleton<InteractionPlayerController>.Instance.ExitLockMoveMode();
		GameUIScreen.SetGameUIVisible(isVisible: true);
		ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension_ConfirmNameScreen);
		if (m_IsTutorial)
		{
			m_IsTutorial = false;
			CPlayerData.m_TutorialIndex = 1;
			CSingleton<TutorialManager>.Instance.EvaluateTaskVisibility();
			CSingleton<TutorialManager>.Instance.m_TutorialTargetIndicator.SetActive(value: false);
		}
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		if (CPlayerData.m_TutorialIndex != 0)
		{
			m_ShopName.text = CPlayerData.PlayerName;
			base.gameObject.SetActive(value: false);
		}
	}
}
