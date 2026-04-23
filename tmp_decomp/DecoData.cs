using System;
using I2.Loc;

[Serializable]
public class DecoData
{
	public string name;

	public EDecoType decoType;

	public InteractableObject spawnPrefab;

	public string GetName()
	{
		return LocalizationManager.GetTranslation(name);
	}
}
