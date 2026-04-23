using System;
using System.Collections.Generic;

namespace CC;

[Serializable]
public class CC_Apparel_Material_Collection
{
	public string Label;

	public List<CC_Apparel_Material_Definition> MaterialDefinitions = new List<CC_Apparel_Material_Definition>();
}
