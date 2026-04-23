using UnityEngine;

public class InteractableScanItem : InteractableObject
{
	public Item m_Item;

	private Customer m_CurrentCustomer;

	private Transform m_ScannedItemLerpPos;

	public override void OnMouseButtonUp()
	{
		m_Item.m_Collider.enabled = false;
		m_Item.m_Rigidbody.isKinematic = true;
		m_CurrentCustomer.OnItemScanned(m_Item);
		m_CurrentCustomer = null;
		SetHideItemAfterFinishLerp();
		LerpToTransform(m_ScannedItemLerpPos, m_ScannedItemLerpPos);
	}

	public void RegisterScanItem(Customer customer, Transform scannedItemLerpPos)
	{
		m_CurrentCustomer = customer;
		m_ScannedItemLerpPos = scannedItemLerpPos;
	}

	public bool IsNotScanned()
	{
		if ((bool)m_CurrentCustomer)
		{
			return true;
		}
		return false;
	}
}
