using System;
using UnityEngine;

[Serializable]
public class CounterMoneyChangeCurrencyData
{
	public string name;

	public EMoneyCurrencyType currencyType;

	public bool isCoin;

	public float value;

	public Material changeMaterial;
}
