using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 7)]
public class SkillList_ScriptableObject : ScriptableObject
{
	public List<Color> m_ElementColorList;
}
