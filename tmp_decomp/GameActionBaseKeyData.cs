using System;
using I2.Loc;

[Serializable]
public class GameActionBaseKeyData
{
	public string actionName;

	public EGameAction gameAction;

	public EGameBaseKey baseKey;

	public string GetName()
	{
		return LocalizationManager.GetTranslation(actionName);
	}
}
