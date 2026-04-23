using UnityEngine;

public class InteractableCustomerCash : InteractableObject
{
	public bool m_IsCard;

	public GameObject m_CashModel;

	public GameObject m_CardModel;

	public GameObject m_CashOutlineModel;

	public GameObject m_CardOutlineModel;

	public MeshRenderer m_CashMeshRender;

	private Customer m_CurrentCustomer;

	private EMoneyCurrencyType m_CurrentCurrencyType;

	public void Init(Customer customer)
	{
		m_CurrentCustomer = customer;
	}

	public void SetIsCard(bool isCard)
	{
		m_IsCard = isCard;
		m_CashModel.SetActive(!m_IsCard);
		m_CardModel.SetActive(m_IsCard);
		m_CashOutlineModel.SetActive(!m_IsCard);
		m_CardOutlineModel.SetActive(m_IsCard);
		if (!m_IsCard && m_CurrentCurrencyType != CSingleton<CGameManager>.Instance.m_CurrencyType)
		{
			m_CurrentCurrencyType = CSingleton<CGameManager>.Instance.m_CurrencyType;
			m_CashMeshRender.material = InventoryBase.GetCurrencyMaterial(m_CurrentCurrencyType);
		}
	}

	public override void OnMouseButtonUp()
	{
		m_CurrentCustomer.OnCashTaken(m_IsCard);
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected new virtual void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected void OnMoneyCurrencyUpdated(CEventPlayer_OnMoneyCurrencyUpdated evt)
	{
		if (!m_IsCard)
		{
			SetIsCard(isCard: false);
		}
	}
}
