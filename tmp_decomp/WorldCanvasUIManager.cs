using UnityEngine;

public class WorldCanvasUIManager : CSingleton<WorldCanvasUIManager>
{
	public static WorldCanvasUIManager m_Instance;

	public UI_CashCounterScreen m_UICashCounterScreenPrefab;

	public UI_CreditCardScreen m_UICreditCardScreenPrefab;

	public Transform m_UIParentGrp_Cash;

	public Transform m_UIParentGrp_Credit;

	private void Awake()
	{
	}

	public static UI_CashCounterScreen SpawnCashCounterScreenUI(Transform followTarget)
	{
		UI_CashCounterScreen uI_CashCounterScreen = Object.Instantiate(CSingleton<WorldCanvasUIManager>.Instance.m_UICashCounterScreenPrefab, followTarget.position, followTarget.rotation, CSingleton<WorldCanvasUIManager>.Instance.m_UIParentGrp_Cash);
		uI_CashCounterScreen.m_FollowObject.SetFollowTarget(followTarget);
		uI_CashCounterScreen.m_FollowObject.enabled = true;
		return uI_CashCounterScreen;
	}

	public static UI_CreditCardScreen SpawnCreditCardScreenUI(Transform followTarget)
	{
		UI_CreditCardScreen uI_CreditCardScreen = Object.Instantiate(CSingleton<WorldCanvasUIManager>.Instance.m_UICreditCardScreenPrefab, followTarget.position, followTarget.rotation, CSingleton<WorldCanvasUIManager>.Instance.m_UIParentGrp_Credit);
		uI_CreditCardScreen.m_FollowObject.SetFollowTarget(followTarget);
		uI_CreditCardScreen.m_FollowObject.enabled = true;
		return uI_CreditCardScreen;
	}
}
