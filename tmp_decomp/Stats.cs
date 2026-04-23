using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public struct Stats
{
	public int FireElement;

	public int EarthElement;

	public int WaterElement;

	public int WindElement;

	public int HP;

	public int Strength;

	public int Magic;

	public int Vitality;

	public int Spirit;

	public int Speed;

	public int HP_LevelAdd;

	public int Strength_LevelAdd;

	public int Magic_LevelAdd;

	public int Vitality_LevelAdd;

	public int Spirit_LevelAdd;

	public int Speed_LevelAdd;

	public int GetElementPower(int elementIndex)
	{
		return elementIndex switch
		{
			0 => FireElement, 
			1 => EarthElement, 
			2 => WaterElement, 
			3 => WindElement, 
			_ => 0, 
		};
	}

	public void DeepCopy(Stats stat)
	{
		HP = stat.HP;
		Strength = stat.Strength;
		Magic = stat.Magic;
		Vitality = stat.Vitality;
		Spirit = stat.Spirit;
		Speed = stat.Speed;
	}

	public void Difference(Stats stat)
	{
		HP -= stat.HP;
		Strength -= stat.Strength;
		Magic -= stat.Magic;
		Vitality -= stat.Vitality;
		Spirit -= stat.Spirit;
		Speed -= stat.Speed;
	}

	public int GetStatMultiplied(int statAmount)
	{
		return Mathf.CeilToInt((float)statAmount * 1.1f);
	}

	public void UpdateStatBasedOnLevel(int level)
	{
		if (level > 1)
		{
			for (int i = 0; i < level - 1; i++)
			{
				HP = GetStatMultiplied(HP) + Mathf.CeilToInt((float)HP * 0.01f);
				Strength = GetStatMultiplied(Strength);
				Magic = GetStatMultiplied(Magic);
				Vitality = GetStatMultiplied(Vitality);
				Spirit = GetStatMultiplied(Spirit);
				Speed = GetStatMultiplied(Speed);
			}
		}
	}

	public string GetStatName(int index)
	{
		string term = "";
		switch (index)
		{
		case 0:
			term = "HP";
			break;
		case 1:
			term = "Strength";
			break;
		case 2:
			term = "Magic";
			break;
		case 3:
			term = "Vitality";
			break;
		case 4:
			term = "Spirit";
			break;
		case 5:
			term = "Speed";
			break;
		}
		return LocalizationManager.GetTranslation(term);
	}

	public int GetStatAmount(int index)
	{
		int result = 0;
		switch (index)
		{
		case 0:
			result = HP;
			break;
		case 1:
			result = Strength;
			break;
		case 2:
			result = Magic;
			break;
		case 3:
			result = Vitality;
			break;
		case 4:
			result = Spirit;
			break;
		case 5:
			result = Speed;
			break;
		}
		return result;
	}
}
