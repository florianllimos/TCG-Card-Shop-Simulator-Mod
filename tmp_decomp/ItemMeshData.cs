using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemMeshData
{
	public string name;

	public Mesh mesh;

	public Material material;

	public Mesh meshSecondary;

	public Material materialSecondary;

	public List<Material> materialList;
}
