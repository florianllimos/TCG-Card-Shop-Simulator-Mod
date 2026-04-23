using System;
using System.Collections.Generic;

namespace GA;

[Serializable]
public class PlayerPrefsData
{
	public Dictionary<string, string> strings = new Dictionary<string, string>();

	public Dictionary<string, int> ints = new Dictionary<string, int>();

	public Dictionary<string, float> floats = new Dictionary<string, float>();
}
