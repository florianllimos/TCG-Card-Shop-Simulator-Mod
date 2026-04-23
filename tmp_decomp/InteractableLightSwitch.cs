using UnityEngine;

public class InteractableLightSwitch : InteractableObject
{
	public GameObject m_SwitchOnModel;

	public GameObject m_SwitchOffModel;

	public override void OnMouseButtonUp()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.6f, 0.5f);
		CSingleton<LightManager>.Instance.ToggleShopLight();
		m_SwitchOnModel.SetActive(LightManager.IsShopLightOn());
		m_SwitchOffModel.SetActive(!LightManager.IsShopLightOn());
	}
}
