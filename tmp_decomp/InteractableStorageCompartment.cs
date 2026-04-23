public class InteractableStorageCompartment : InteractableObject
{
	private WarehouseShelf m_WarehouseShelf;

	private ShelfCompartment m_ShelfCompartment;

	public void InitWarehouseShelf(WarehouseShelf warehouseShelf, ShelfCompartment shelfCompartment, int index)
	{
		m_WarehouseShelf = warehouseShelf;
		m_ShelfCompartment = shelfCompartment;
		m_ShelfCompartment.SetIndex(index);
		m_ShelfCompartment.SetWarehouseShelf(m_WarehouseShelf);
	}

	public override void OnMouseButtonUp()
	{
		if (m_ShelfCompartment.GetInteractablePackagingBoxList().Count > 0)
		{
			InteractablePackagingBox_Item lastInteractablePackagingBox = m_ShelfCompartment.GetLastInteractablePackagingBox();
			if (lastInteractablePackagingBox.CanPickup())
			{
				lastInteractablePackagingBox.StartHoldBox(isPlayer: true, CSingleton<InteractionPlayerController>.Instance.m_HoldItemPos);
				m_ShelfCompartment.RemoveBox(lastInteractablePackagingBox);
			}
		}
	}

	public void DisableAllItem()
	{
		for (int i = 0; i < m_ShelfCompartment.GetInteractablePackagingBoxList().Count; i++)
		{
			if ((bool)m_ShelfCompartment.GetInteractablePackagingBoxList()[i])
			{
				m_ShelfCompartment.GetInteractablePackagingBoxList()[i].m_ItemCompartment.DisableAllItem();
			}
			RestockManager.RemoveItemPackageBox(m_ShelfCompartment.GetInteractablePackagingBoxList()[i]);
		}
	}

	public ShelfCompartment GetShelfCompartment()
	{
		return m_ShelfCompartment;
	}
}
