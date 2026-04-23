using UnityEngine;

public class LerpSlider : MonoBehaviour
{
	private Vector3 m_StartPos;

	private bool m_IsLerping;

	private Vector3 m_LerpPos;

	private float m_LerpSpeed = 7f;

	private void Start()
	{
		m_StartPos = base.transform.localPosition;
	}

	private void Update()
	{
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, m_LerpPos, Time.deltaTime * m_LerpSpeed);
	}

	public void SetLerpPos(float posX)
	{
		m_LerpPos = new Vector3(posX, m_StartPos.y, 0f);
		m_IsLerping = true;
	}

	public void SetLerpPosY(float posY)
	{
		m_LerpPos = new Vector3(m_StartPos.x, posY, 0f);
		m_IsLerping = true;
	}

	public void StopLerp()
	{
		m_IsLerping = false;
	}
}
