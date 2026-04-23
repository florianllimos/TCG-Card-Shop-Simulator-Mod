using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreen : CSingleton<LoadingScreen>
{
	public static LoadingScreen m_Instance;

	public GameObject m_ScreenGrp;

	public Animation m_ScreenAnim;

	public TextMeshProUGUI m_PercentText;

	private Coroutine m_DelayHide;

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		Object.DontDestroyOnLoad(this);
	}

	public static void OpenScreen()
	{
		CSingleton<LoadingScreen>.Instance.m_PercentText.text = "0%";
		CSingleton<LoadingScreen>.Instance.m_ScreenGrp.SetActive(value: true);
		CSingleton<LoadingScreen>.Instance.m_ScreenAnim.Play("Loading_FadeIn");
		GameInstance.m_HasFinishHideLoadingScreen = false;
	}

	public static void CloseScreen()
	{
		CSingleton<LoadingScreen>.Instance.HideLoadingScreen();
	}

	public static void SetPercentDone(int percent)
	{
		CSingleton<LoadingScreen>.Instance.m_PercentText.text = percent + "%";
	}

	private void HideLoadingScreen()
	{
		m_ScreenGrp.SetActive(value: true);
		m_PercentText.text = "100%";
	}

	public void FinishLoading()
	{
		if (m_DelayHide != null)
		{
			StopCoroutine(m_DelayHide);
		}
		m_DelayHide = StartCoroutine(DelayHide(0.1f));
	}

	private IEnumerator DelayHide(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		m_ScreenAnim.Play("Loading_FadeOut");
		yield return new WaitForSeconds(1f);
		CEventManager.QueueEvent(new CEventPlayer_FinishHideLoadingScreen());
		m_ScreenGrp.SetActive(value: false);
		GameInstance.m_HasFinishHideLoadingScreen = true;
	}
}
