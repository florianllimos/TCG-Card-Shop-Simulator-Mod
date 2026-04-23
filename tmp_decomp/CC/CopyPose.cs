using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CC;

public class CopyPose : MonoBehaviour
{
	private Transform sourceRoot;

	private Transform targetRoot;

	private List<Transform> sourceBones = new List<Transform>();

	private List<Transform> targetBones = new List<Transform>();

	private void Start()
	{
		sourceRoot = base.transform.parent.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
		targetRoot = getRootBone(base.transform.GetComponentInChildren<SkinnedMeshRenderer>().rootBone);
		Transform[] componentsInChildren = targetRoot.GetComponentsInChildren<Transform>();
		Transform[] componentsInChildren2 = sourceRoot.GetComponentsInChildren<Transform>();
		foreach (Transform sourceBone in componentsInChildren2)
		{
			Transform transform = componentsInChildren.FirstOrDefault((Transform t) => t.name == sourceBone.name);
			if (transform != null)
			{
				sourceBones.Add(sourceBone);
				targetBones.Add(transform);
			}
		}
	}

	private Transform getRootBone(Transform bone)
	{
		if (bone.parent == null || bone.parent == base.gameObject.transform)
		{
			return bone;
		}
		return getRootBone(bone.parent);
	}

	private void LateUpdate()
	{
		for (int i = 0; i < sourceBones.Count; i++)
		{
			targetBones[i].localPosition = sourceBones[i].localPosition;
			targetBones[i].localRotation = sourceBones[i].localRotation;
			targetBones[i].localScale = sourceBones[i].localScale;
		}
	}
}
