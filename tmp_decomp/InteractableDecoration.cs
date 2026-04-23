public class InteractableDecoration : InteractableObject
{
	public bool m_SoftPlaceObjectSFX;

	protected override void PlayPlaceMoveObjectSFX()
	{
		if (m_SoftPlaceObjectSFX)
		{
			SoundManager.PlayAudio("SFX_Throw", 0.3f);
		}
		else
		{
			base.PlayPlaceMoveObjectSFX();
		}
	}
}
