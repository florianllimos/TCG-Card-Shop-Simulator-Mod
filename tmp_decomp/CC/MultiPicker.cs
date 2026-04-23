using System;
using System.Collections.Generic;

namespace CC;

[Serializable]
public class MultiPicker
{
	public CC_Property Property;

	public List<string> Objects = new List<string>();
}
