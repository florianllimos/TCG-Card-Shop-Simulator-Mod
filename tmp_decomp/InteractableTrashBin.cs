public class InteractableTrashBin : InteractableObject
{
	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitTrashBin(this);
	}

	public void DiscardBox(InteractablePackagingBox packagingBox, bool isPlayer)
	{
		packagingBox.OnDestroyed();
		if (isPlayer)
		{
			SoundManager.PlayAudio("SFX_Dispose", 0.6f);
			CSingleton<InteractionPlayerController>.Instance.OnExitHoldBoxMode();
		}
	}
}
