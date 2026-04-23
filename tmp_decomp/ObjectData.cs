using System;
using I2.Loc;

[Serializable]
public class ObjectData
{
	public string name;

	public EObjectType objectType;

	public InteractableObject spawnPrefab;

	public float decoBonus;

	public string GetName()
	{
		return LocalizationManager.GetTranslation(name);
	}
}
