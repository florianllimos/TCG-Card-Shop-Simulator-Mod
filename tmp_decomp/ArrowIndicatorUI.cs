using UnityEngine;

public class ArrowIndicatorUI : MonoBehaviour
{
	public Transform m_ArrowTransformGrp;

	public void SetPosY(float posYOffset)
	{
		Vector3 localPosition = m_ArrowTransformGrp.transform.localPosition;
		localPosition.y = posYOffset;
		m_ArrowTransformGrp.transform.localPosition = localPosition;
	}

	public void SetScale(float scale)
	{
		m_ArrowTransformGrp.localScale = Vector3.one * scale;
	}
}
