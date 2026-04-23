using UnityEngine;

public class AspectRatioPositionOffset : MonoBehaviour
{
	public Vector3 m_OffsetAmount;

	public Vector3 m_OffsetLimitMin;

	public Vector3 m_OffsetLimitMax;

	private Vector3 m_OriginalPos;

	private float m_CompareRatio = -1f;

	private float m_BaseRatio = -1f;

	private void Start()
	{
		m_OriginalPos = base.transform.localPosition;
		m_BaseRatio = 1.7777778f;
	}

	private void Update()
	{
		float num = (float)Screen.width / (float)Screen.height;
		if (m_CompareRatio != num)
		{
			Vector3 localPosition = m_OriginalPos + m_OffsetAmount * (m_BaseRatio - num);
			localPosition.x = Mathf.Clamp(localPosition.x, m_OffsetLimitMin.x, m_OffsetLimitMax.x);
			localPosition.y = Mathf.Clamp(localPosition.y, m_OffsetLimitMin.y, m_OffsetLimitMax.y);
			localPosition.z = Mathf.Clamp(localPosition.z, m_OffsetLimitMin.z, m_OffsetLimitMax.z);
			base.transform.localPosition = localPosition;
			m_CompareRatio = num;
		}
	}
}
