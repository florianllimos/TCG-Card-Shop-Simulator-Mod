using UnityEngine;

public class PlayCard : MonoBehaviour
{
	public GameObject m_Deck;

	public GameObject m_CardGrp;

	public GameObject m_ParentRoot;

	public void Init()
	{
		base.gameObject.SetActive(value: false);
	}

	public void ParentCardGrpToObj(Transform inputTransform)
	{
		m_CardGrp.transform.position = inputTransform.position;
		m_CardGrp.transform.rotation = inputTransform.rotation;
		m_CardGrp.transform.parent = inputTransform;
	}

	public void SetDeckToPosition(Transform inputTransform)
	{
		m_Deck.transform.position = inputTransform.position;
		m_Deck.transform.rotation = inputTransform.rotation;
	}

	public void EndPlay()
	{
		base.gameObject.SetActive(value: false);
		m_CardGrp.transform.parent = m_ParentRoot.transform;
	}
}
