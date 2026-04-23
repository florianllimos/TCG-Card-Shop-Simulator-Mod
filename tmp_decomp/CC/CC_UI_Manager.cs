using System.Collections.Generic;
using UnityEngine;

namespace CC;

public class CC_UI_Manager : MonoBehaviour
{
	public static CC_UI_Manager instance;

	[Tooltip("The parent object of your customizable characters")]
	public GameObject CharacterParent;

	public List<AudioClip> UISounds = new List<AudioClip>();

	private int characterIndex;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Start()
	{
		SetActiveCharacter(0);
	}

	public void playUIAudio(int Index)
	{
		AudioSource component = base.gameObject.GetComponent<AudioSource>();
		if ((bool)component && UISounds.Count > Index)
		{
			component.clip = UISounds[Index];
		}
		component.Play();
	}

	public void SetActiveCharacter(int i)
	{
		characterIndex = i;
		for (int j = 0; j < CharacterParent.transform.childCount; j++)
		{
			CharacterParent.transform.GetChild(j).gameObject.SetActive(i == j);
			CharacterParent.transform.GetChild(j).GetComponent<CharacterCustomization>().UI.SetActive(i == j);
		}
	}

	public void characterNext()
	{
		SetActiveCharacter((characterIndex != CharacterParent.transform.childCount - 1) ? (characterIndex + 1) : 0);
	}

	public void characterPrev()
	{
		SetActiveCharacter((characterIndex == 0) ? (CharacterParent.transform.childCount - 1) : (characterIndex - 1));
	}
}
