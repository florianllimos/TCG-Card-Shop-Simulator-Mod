using System.Collections.Generic;
using GA;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class PlatformPrefabLoader
{
	public static void ReplaceIfNeeded(string replaceeName)
	{
		GameObject gameObject = GameObject.Find(replaceeName);
		if (gameObject == null)
		{
			return;
		}
		PlatformPrefabSet prefabSet = PlatformManager.Instance.GetPrefabSet();
		if (prefabSet == null)
		{
			Debug.LogError("PlatformPrefabLoader: PrefabSet not assigned in PlatformManager.");
			return;
		}
		EPlatform platform = PlatformManager.GetPlatform();
		GameObject prefabForPlatform = prefabSet.GetPrefabForPlatform(replaceeName, platform);
		if (!(prefabForPlatform == null))
		{
			MergeVariant(gameObject, prefabForPlatform);
		}
	}

	public static void MergeVariant(GameObject targetRoot, GameObject variantPrefab)
	{
		GameObject gameObject = Object.Instantiate(variantPrefab);
		gameObject.name = variantPrefab.name;
		Dictionary<string, Transform> targetLookup = BuildRelativePathLookup(targetRoot.transform);
		HashSet<string> hashSet = new HashSet<string>();
		AddVariantPathsRecursive(gameObject.transform, "", hashSet);
		ApplyRecursiveByIndexAndPath(targetRoot.transform, gameObject.transform, targetLookup, "");
		DisableMissingTargetChildren(targetRoot.transform, hashSet, "");
		RectTransform component = targetRoot.GetComponent<RectTransform>();
		if (component != null)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(component);
		}
		Object.DestroyImmediate(gameObject);
	}

	private static Dictionary<string, Transform> BuildRelativePathLookup(Transform root)
	{
		Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
		AddChildrenToLookup(root, "", dictionary);
		return dictionary;
	}

	private static void AddChildrenToLookup(Transform current, string parentPath, Dictionary<string, Transform> dict)
	{
		foreach (Transform item in current)
		{
			string text = (string.IsNullOrEmpty(parentPath) ? item.name : (parentPath + "/" + item.name));
			dict[text] = item;
			AddChildrenToLookup(item, text, dict);
		}
	}

	private static void AddVariantPathsRecursive(Transform variantNode, string parentPath, HashSet<string> set)
	{
		foreach (Transform item in variantNode)
		{
			string text = (string.IsNullOrEmpty(parentPath) ? item.name : (parentPath + "/" + item.name));
			set.Add(text);
			AddVariantPathsRecursive(item, text, set);
		}
	}

	private static void ApplyRecursiveByIndexAndPath(Transform targetParent, Transform variantParent, Dictionary<string, Transform> targetLookup, string parentPath)
	{
		for (int i = 0; i < variantParent.childCount; i++)
		{
			Transform child = variantParent.GetChild(i);
			string text = (string.IsNullOrEmpty(parentPath) ? child.name : (parentPath + "/" + child.name));
			Transform value = null;
			if (i < targetParent.childCount)
			{
				Transform child2 = targetParent.GetChild(i);
				if (child2.name == child.name)
				{
					value = child2;
				}
			}
			if (value == null)
			{
				targetLookup.TryGetValue(text, out value);
			}
			if (IsTMPTextLeaf(child))
			{
				if (value == null)
				{
					GameObject gameObject = Object.Instantiate(child.gameObject, targetParent);
					gameObject.name = child.name;
					gameObject.SetActive(child.gameObject.activeSelf);
					continue;
				}
				Transform parent = value.parent;
				int siblingIndex = value.GetSiblingIndex();
				Object.DestroyImmediate(value.gameObject);
				GameObject gameObject2 = Object.Instantiate(child.gameObject, parent);
				gameObject2.name = child.name;
				gameObject2.transform.SetSiblingIndex(siblingIndex);
				gameObject2.SetActive(child.gameObject.activeSelf);
			}
			else if (value == null)
			{
				GameObject gameObject3 = Object.Instantiate(child.gameObject, targetParent);
				gameObject3.name = child.name;
				gameObject3.SetActive(child.gameObject.activeSelf);
				CopyUIComponents(gameObject3, child.gameObject);
				ApplyRecursiveByIndexAndPath(gameObject3.transform, child, targetLookup, text);
			}
			else
			{
				RectTransform rectTransform = value as RectTransform;
				RectTransform rectTransform2 = child as RectTransform;
				if (rectTransform != null && rectTransform2 != null)
				{
					CopyRectTransform(rectTransform2, rectTransform);
				}
				else
				{
					value.localPosition = child.localPosition;
					value.localRotation = child.localRotation;
					value.localScale = child.localScale;
				}
				value.gameObject.SetActive(child.gameObject.activeSelf);
				CopyUIComponents(value.gameObject, child.gameObject);
				ApplyRecursiveByIndexAndPath(value, child, targetLookup, text);
			}
		}
	}

	private static void DisableMissingTargetChildren(Transform targetParent, HashSet<string> variantPaths, string parentPath)
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform item in targetParent)
		{
			string text = (string.IsNullOrEmpty(parentPath) ? item.name : (parentPath + "/" + item.name));
			if (!variantPaths.Contains(text))
			{
				list.Add(item);
			}
			else
			{
				DisableMissingTargetChildren(item, variantPaths, text);
			}
		}
		foreach (Transform item2 in list)
		{
			item2.gameObject.SetActive(value: false);
		}
	}

	private static void CopyRectTransform(RectTransform from, RectTransform to)
	{
		to.anchorMin = from.anchorMin;
		to.anchorMax = from.anchorMax;
		to.pivot = from.pivot;
		to.anchoredPosition = from.anchoredPosition;
		to.sizeDelta = from.sizeDelta;
		to.localRotation = from.localRotation;
		to.localScale = from.localScale;
	}

	private static void CopyUIComponents(GameObject target, GameObject variant)
	{
		TextMeshProUGUI component = variant.GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI component2 = target.GetComponent<TextMeshProUGUI>();
		if (component != null && component2 != null)
		{
			component2.text = component.text;
			component2.alignment = component.alignment;
			component2.fontSize = component.fontSize;
			component2.textWrappingMode = component.textWrappingMode;
			component2.enableAutoSizing = component.enableAutoSizing;
			component2.fontStyle = component.fontStyle;
			component2.color = component.color;
			component2.ForceMeshUpdate();
		}
		Image component3 = variant.GetComponent<Image>();
		Image component4 = target.GetComponent<Image>();
		if (component3 != null && component4 != null)
		{
			component4.sprite = component3.sprite;
			component4.color = component3.color;
			component4.type = component3.type;
			component4.preserveAspect = component3.preserveAspect;
		}
		LayoutElement component5 = variant.GetComponent<LayoutElement>();
		LayoutElement layoutElement = target.GetComponent<LayoutElement>();
		if (component5 != null)
		{
			if (layoutElement == null)
			{
				layoutElement = target.AddComponent<LayoutElement>();
			}
			layoutElement.minWidth = component5.minWidth;
			layoutElement.minHeight = component5.minHeight;
			layoutElement.preferredWidth = component5.preferredWidth;
			layoutElement.preferredHeight = component5.preferredHeight;
			layoutElement.flexibleWidth = component5.flexibleWidth;
			layoutElement.flexibleHeight = component5.flexibleHeight;
		}
		ContentSizeFitter component6 = variant.GetComponent<ContentSizeFitter>();
		ContentSizeFitter component7 = target.GetComponent<ContentSizeFitter>();
		if (component6 != null && component7 != null)
		{
			component7.horizontalFit = component6.horizontalFit;
			component7.verticalFit = component6.verticalFit;
		}
		Localize component8 = variant.GetComponent<Localize>();
		Localize localize = target.GetComponent<Localize>();
		if (component8 != null)
		{
			if (localize == null)
			{
				localize = target.AddComponent<Localize>();
			}
			localize.Term = component8.Term;
			localize.SecondaryTerm = component8.SecondaryTerm;
			localize.TermPrefix = component8.TermPrefix;
			localize.TermSuffix = component8.TermSuffix;
			localize.IgnoreRTL = component8.IgnoreRTL;
			localize.AllowLocalizedParameters = component8.AllowLocalizedParameters;
			localize.mLocalizeTarget = component8.mLocalizeTarget;
		}
	}

	private static bool IsTMPTextLeaf(Transform t)
	{
		if (t.name == "Text")
		{
			return t.GetComponent<TextMeshProUGUI>() != null;
		}
		return false;
	}

	private static string GetTransformPath(Transform t)
	{
		if (t == null)
		{
			return "NULL";
		}
		Stack<string> stack = new Stack<string>();
		while (t != null)
		{
			stack.Push(t.name);
			t = t.parent;
		}
		return string.Join("/", stack);
	}
}
