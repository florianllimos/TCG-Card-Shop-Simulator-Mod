using TMPro;
using UnityEngine;

public class PricePopupUI : MonoBehaviour
{
	public Transform m_Transform;

	public Transform m_FollowTransform;

	public TextMeshProUGUI m_Text;

	public float m_OffsetUp;

	public void Init()
	{
		m_Transform = base.transform;
	}

	public void SetFollowTransform(Transform follow, float offsetUp)
	{
		m_FollowTransform = follow;
		m_OffsetUp = offsetUp;
	}
}
