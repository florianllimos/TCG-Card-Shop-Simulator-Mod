public class InteractableShopBillboard : InteractableObject
{
	public ShopRenamer m_ShopRenamer;

	public override void OnMouseButtonUp()
	{
		if (CPlayerData.m_TutorialIndex != 0)
		{
			SoundManager.GenericConfirm();
			m_ShopRenamer.ShowRenameShopScreen(showIntro: false);
		}
	}
}
