using UnityEngine;

namespace CC;

public class CC_UI_Util : MonoBehaviour
{
	private CharacterCustomization customizer;

	public void Initialize(CharacterCustomization customizerScript)
	{
		customizer = customizerScript;
		ICustomizerUI[] componentsInChildren = base.gameObject.GetComponentsInChildren<ICustomizerUI>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].InitializeUIElement(customizerScript, this);
		}
	}

	public void refreshUI()
	{
		ICustomizerUI[] componentsInChildren = base.gameObject.GetComponentsInChildren<ICustomizerUI>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RefreshUIElement();
		}
	}

	public void characterNext()
	{
		CC_UI_Manager.instance.characterNext();
	}

	public void characterPrev()
	{
		CC_UI_Manager.instance.characterPrev();
	}

	public void saveCharacter()
	{
		customizer.SaveToJSON();
	}

	public void loadCharacter()
	{
		customizer.LoadFromJSON();
		refreshUI();
	}

	public void setCharacterName(string newName)
	{
		customizer.setCharacterName(newName);
	}
}
