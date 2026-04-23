using UnityEngine;

public class TouchDragUI : MonoBehaviour
{
	public GameObject m_StartDragIconGrp;

	public GameObject m_CurrentDragIconImage;

	public GameObject m_CurrentDragIconGrp;

	public GameObject m_LineRendererIcon;

	private void Awake()
	{
		m_StartDragIconGrp.SetActive(value: false);
		m_CurrentDragIconImage.SetActive(value: false);
		m_LineRendererIcon.SetActive(value: false);
	}

	private void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_TouchReleased>(CPlayer_OnTouchReleased);
			CEventManager.AddListener<CEventPlayer_TouchScreen>(CPlayer_OnTouchScreen);
		}
	}

	private void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_TouchReleased>(CPlayer_OnTouchReleased);
			CEventManager.RemoveListener<CEventPlayer_TouchScreen>(CPlayer_OnTouchScreen);
		}
	}

	private void CPlayer_OnTouchReleased(CEventPlayer_TouchReleased evt)
	{
		m_StartDragIconGrp.SetActive(value: false);
		m_CurrentDragIconImage.SetActive(value: false);
		m_LineRendererIcon.SetActive(value: false);
	}

	private void CPlayer_OnTouchScreen(CEventPlayer_TouchScreen evt)
	{
		m_StartDragIconGrp.SetActive(value: true);
		m_CurrentDragIconImage.SetActive(value: true);
		m_LineRendererIcon.SetActive(value: true);
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = 2f;
		m_StartDragIconGrp.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
	}
}
