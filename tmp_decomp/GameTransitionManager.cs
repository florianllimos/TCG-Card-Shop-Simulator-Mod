using System.Linq;
using GA;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class GameTransitionManager : MonoBehaviour
{
	private const float ERROR_INTERVAL = 5f;

	private bool waitingForPlatform = true;

	private float waitTimer;

	private bool splashStarted;

	private bool splashFinished;

	private float fadeDuration = 1f;

	private float splashVisibleDuration = 5f;

	private bool fadeOutStarted;

	private float fadeOutTimer;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Canvas splashCanvas;

	[SerializeField]
	private TextMeshProUGUI splashText;

	private float splashTimer;

	private void Update()
	{
		if (waitingForPlatform && PlatformManager.Instance.IsInitialized)
		{
			waitingForPlatform = false;
			StartSplash();
		}
		if (waitingForPlatform && !PlatformManager.Instance.IsInitialized)
		{
			waitTimer += Time.deltaTime;
			if (waitTimer >= 5f)
			{
				Debug.LogError("[GameTransitionManager] Platform initialization is taking too long...");
				waitTimer = 0f;
			}
		}
		if (splashStarted && !splashFinished)
		{
			RunSplash();
		}
		if (splashFinished)
		{
			SceneManager.LoadScene(1);
			base.enabled = false;
		}
	}

	private void StartSplash()
	{
		splashStarted = true;
		splashTimer = 0f;
		canvasGroup.alpha = 0f;
		splashText.gameObject.AddComponent<Localize>().Term = "EarlyAccessWarning";
		if (!PlatformManager.Instance.HasAnySaves)
		{
			LocalizationManager.CurrentLanguage = PlatformManager.Instance.GetLanguage();
		}
		Debug.Log("[GameTransitionManager] Showing Early Access splash for language " + LocalizationManager.CurrentLanguage);
	}

	private void RunSplash()
	{
		if (!fadeOutStarted && splashTimer >= fadeDuration && WasAnyKeyPressed())
		{
			canvasGroup.alpha = 0f;
			fadeOutStarted = true;
			fadeOutTimer = 0f;
		}
		if (fadeOutStarted)
		{
			fadeOutTimer += Time.deltaTime;
			float t = fadeOutTimer / fadeDuration;
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, t);
			if (canvasGroup.alpha <= 0.01f)
			{
				canvasGroup.alpha = 0f;
				splashFinished = true;
			}
			return;
		}
		splashTimer += Time.deltaTime;
		if (splashTimer <= fadeDuration)
		{
			canvasGroup.alpha = Mathf.Clamp01(splashTimer / fadeDuration);
		}
		else if (!(splashTimer <= fadeDuration + splashVisibleDuration))
		{
			float num = splashTimer - (fadeDuration + splashVisibleDuration);
			canvasGroup.alpha = 1f - Mathf.Clamp01(num / fadeDuration);
			if (canvasGroup.alpha <= 0.01f)
			{
				canvasGroup.alpha = 0f;
				splashFinished = true;
			}
		}
	}

	private bool WasAnyKeyPressed()
	{
		if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
		{
			return true;
		}
		if (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame))
		{
			return true;
		}
		foreach (Gamepad item in Gamepad.all)
		{
			if (item != null && item.allControls.Any((InputControl c) => c is ButtonControl buttonControl && buttonControl.wasPressedThisFrame))
			{
				return true;
			}
		}
		return false;
	}
}
