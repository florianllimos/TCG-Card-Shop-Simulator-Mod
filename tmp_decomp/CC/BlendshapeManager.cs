using System.Collections.Generic;
using UnityEngine;

namespace CC;

public class BlendshapeManager : MonoBehaviour
{
	private List<string> blendshapeNames = new List<string>();

	private SkinnedMeshRenderer mesh;

	public void parseBlendshapes()
	{
		mesh = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		if (mesh.sharedMesh != null)
		{
			for (int i = 0; i < mesh.sharedMesh.blendShapeCount; i++)
			{
				blendshapeNames.Add(mesh.sharedMesh.GetBlendShapeName(i));
			}
		}
	}

	public void setBlendshape(string name, float value)
	{
		int num = blendshapeNames.FindIndex((string t) => t == name);
		if (num != -1)
		{
			mesh.SetBlendShapeWeight(num, value * 100f);
		}
	}
}
