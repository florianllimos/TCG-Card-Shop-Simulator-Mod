using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CC;

public class CharacterCustomization : MonoBehaviour
{
	public SkinnedMeshRenderer MainMesh;

	public GameObject UI;

	public string CharacterName;

	public bool Autoload;

	public List<scrObj_Hair> HairTables = new List<scrObj_Hair>();

	private List<GameObject> HairObjects = new List<GameObject>();

	public List<scrObj_Apparel> ApparelTables = new List<scrObj_Apparel>();

	private List<GameObject> ApparelObjects = new List<GameObject>();

	public scrObj_Presets Presets;

	public CC_CharacterData StoredCharacterData;

	private string SavePath;

	public bool m_HasInit;

	private void Start()
	{
		SavePath = Application.dataPath + "/CharacterCustomizer.json";
		if (Autoload)
		{
			Initialize();
		}
	}

	public void Initialize()
	{
		if (m_HasInit)
		{
			LoadFromJSON();
			return;
		}
		m_HasInit = true;
		SavePath = Application.dataPath + "/CharacterCustomizer.json";
		SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.AddComponent<BlendshapeManager>().parseBlendshapes();
		}
		for (int j = 0; j < HairTables.Count; j++)
		{
			GameObject gameObject = new GameObject();
			HairObjects.Add(gameObject);
			Object.Destroy(gameObject);
		}
		for (int k = 0; k < ApparelTables.Count; k++)
		{
			GameObject gameObject2 = new GameObject();
			ApparelObjects.Add(gameObject2);
			Object.Destroy(gameObject2);
		}
		LoadFromJSON();
		if (UI != null)
		{
			UI.GetComponent<CC_UI_Util>().Initialize(this);
		}
	}

	public void SaveToJSON()
	{
		if (File.Exists(SavePath))
		{
			if (CharacterName != "")
			{
				CC_SaveData cC_SaveData = JsonUtility.FromJson<CC_SaveData>(File.ReadAllText(SavePath));
				StoredCharacterData.CharacterPrefab = base.gameObject.name;
				int num = cC_SaveData.SavedCharacters.FindIndex((CC_CharacterData t) => t.CharacterName == CharacterName);
				if (num != -1)
				{
					cC_SaveData.SavedCharacters[num] = StoredCharacterData;
				}
				else
				{
					cC_SaveData.SavedCharacters.Add(StoredCharacterData);
				}
				string contents = JsonUtility.ToJson(cC_SaveData, prettyPrint: true);
				File.WriteAllText(SavePath, contents);
			}
		}
		else
		{
			createSaveFile();
			SaveToJSON();
		}
	}

	public void InstantiateCharacter(string name, Transform _transform)
	{
		if (File.Exists(SavePath))
		{
			CC_SaveData cC_SaveData = JsonUtility.FromJson<CC_SaveData>(File.ReadAllText(SavePath));
			int num = cC_SaveData.SavedCharacters.FindIndex((CC_CharacterData t) => t.CharacterName == name);
			if (num != -1)
			{
				GameObject obj = (GameObject)Object.Instantiate(Resources.Load(cC_SaveData.SavedCharacters[num].CharacterPrefab), _transform);
				obj.GetComponent<CharacterCustomization>().CharacterName = name;
				obj.GetComponent<CharacterCustomization>().Initialize();
			}
		}
	}

	public void LoadFromJSON()
	{
		if (!(CharacterName != ""))
		{
			return;
		}
		if (Presets.Presets.Find((CC_CharacterData t) => t.CharacterName == CharacterName) != null)
		{
			StoredCharacterData = JsonUtility.FromJson<CC_CharacterData>(JsonUtility.ToJson(Presets.Presets.Find((CC_CharacterData t) => t.CharacterName == CharacterName)));
		}
		else
		{
			StoredCharacterData = JsonUtility.FromJson<CC_CharacterData>(JsonUtility.ToJson(Presets.Presets[0]));
			StoredCharacterData.CharacterName = CharacterName;
		}
		while (StoredCharacterData.HairNames.Count < HairObjects.Count)
		{
			StoredCharacterData.HairNames.Add("");
		}
		while (StoredCharacterData.HairColor.Count < HairObjects.Count)
		{
			StoredCharacterData.HairColor.Add(new CC_Property());
		}
		while (StoredCharacterData.ApparelNames.Count < ApparelObjects.Count)
		{
			StoredCharacterData.ApparelNames.Add("");
		}
		while (StoredCharacterData.ApparelMaterials.Count < ApparelObjects.Count)
		{
			StoredCharacterData.ApparelMaterials.Add(0);
		}
		ApplyCharacterVars(StoredCharacterData);
	}

	public void ApplyCharacterVars(CC_CharacterData characterData)
	{
		for (int i = 0; i < characterData.Blendshapes.Count; i++)
		{
			setBlendshapeByName(characterData.Blendshapes[i].propertyName, characterData.Blendshapes[i].floatValue, save: false);
		}
		for (int j = 0; j < characterData.HairNames.Count; j++)
		{
			setHairByName(characterData.HairNames[j], j);
		}
		for (int k = 0; k < characterData.ApparelNames.Count; k++)
		{
			setApparelByName(characterData.ApparelNames[k], k, characterData.ApparelMaterials[k]);
		}
		foreach (CC_Property textureProperty in characterData.TextureProperties)
		{
			setTextureProperty(textureProperty);
		}
		foreach (CC_Property floatProperty in characterData.FloatProperties)
		{
			setFloatProperty(floatProperty);
		}
		foreach (CC_Property colorProperty in characterData.ColorProperties)
		{
			if (ColorUtility.TryParseHtmlString("#" + colorProperty.stringValue, out var color))
			{
				setColorProperty(colorProperty, color);
			}
		}
	}

	public void createSaveFile()
	{
		string contents = JsonUtility.ToJson(new CC_SaveData(), prettyPrint: true);
		File.WriteAllText(SavePath, contents);
	}

	public void setCharacterName(string newName)
	{
		CharacterName = newName;
		StoredCharacterData.CharacterName = newName;
	}

	public void setHair(int selection, int slot)
	{
		if (HairTables[slot].Hairstyles.Count <= selection)
		{
			return;
		}
		scrObj_Hair.Hairstyle hairstyle = HairTables[slot].Hairstyles[selection];
		if (HairObjects[slot] != null)
		{
			Object.Destroy(HairObjects[slot]);
		}
		if (HairTables[slot].Hairstyles[selection].Mesh != null)
		{
			HairObjects[slot] = Object.Instantiate(hairstyle.Mesh, base.gameObject.transform);
			GameObject gameObject = HairObjects[slot];
			SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				BlendshapeManager blendshapeManager = skinnedMeshRenderer.gameObject.AddComponent<BlendshapeManager>();
				blendshapeManager.parseBlendshapes();
				foreach (CC_Property blendshape in StoredCharacterData.Blendshapes)
				{
					blendshapeManager.setBlendshape(blendshape.propertyName, blendshape.floatValue);
				}
				skinnedMeshRenderer.gameObject.layer = LayerMask.NameToLayer("Customer");
			}
			if (hairstyle.AddCopyPoseScript)
			{
				gameObject.AddComponent<CopyPose>();
			}
			else
			{
				componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren)
				{
					Transform[] array = new Transform[skinnedMeshRenderer2.bones.Length];
					for (int j = 0; j < skinnedMeshRenderer2.bones.Length; j++)
					{
						if (!(skinnedMeshRenderer2.bones[j] == null))
						{
							array[j] = FindChildByName(skinnedMeshRenderer2.bones[j].name, MainMesh.rootBone);
						}
					}
					skinnedMeshRenderer2.bones = array;
					skinnedMeshRenderer2.gameObject.layer = LayerMask.NameToLayer("Customer");
				}
			}
		}
		setTextureProperty(new CC_Property
		{
			propertyName = HairTables[slot].ShadowMapProperty
		}, save: false, hairstyle.ShadowMap);
		CC_Property cC_Property = StoredCharacterData.HairColor[slot];
		if (ColorUtility.TryParseHtmlString("#" + cC_Property.stringValue, out var color))
		{
			setHairColor(cC_Property, color, slot);
		}
		StoredCharacterData.HairNames[slot] = hairstyle.Name;
	}

	public void setHairByName(string name, int slot)
	{
		int num = HairTables[slot].Hairstyles.FindIndex((scrObj_Hair.Hairstyle t) => t.Name == name);
		if (num != -1)
		{
			setHair(num, slot);
		}
	}

	public void setApparel(int selection, int slot, int materialSelection)
	{
		if (ApparelTables[slot].Items.Count <= selection)
		{
			return;
		}
		scrObj_Apparel.Apparel apparel = ApparelTables[slot].Items[selection];
		if (ApparelObjects[slot] != null)
		{
			Object.Destroy(ApparelObjects[slot]);
		}
		if (ApparelTables[slot].Items[selection].Mesh != null)
		{
			ApparelObjects[slot] = Object.Instantiate(apparel.Mesh, base.gameObject.transform);
			GameObject gameObject = ApparelObjects[slot];
			SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				BlendshapeManager blendshapeManager = skinnedMeshRenderer.gameObject.AddComponent<BlendshapeManager>();
				blendshapeManager.parseBlendshapes();
				foreach (CC_Property blendshape in StoredCharacterData.Blendshapes)
				{
					blendshapeManager.setBlendshape(blendshape.propertyName, blendshape.floatValue);
				}
				skinnedMeshRenderer.gameObject.layer = LayerMask.NameToLayer("Customer");
			}
			componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren)
			{
				for (int j = 0; j < skinnedMeshRenderer2.materials.Length; j++)
				{
					if (apparel.Materials.Count != 0 && apparel.Materials[0].MaterialDefinitions.Count - 1 >= j)
					{
						List<CC_Apparel_Material_Definition> materialDefinitions = apparel.Materials[materialSelection].MaterialDefinitions;
						skinnedMeshRenderer2.materials[j].SetColor("_Tint", materialDefinitions[j].MainTint);
						skinnedMeshRenderer2.materials[j].SetColor("_Tint_R", materialDefinitions[j].TintR);
						skinnedMeshRenderer2.materials[j].SetColor("_Tint_G", materialDefinitions[j].TintG);
						skinnedMeshRenderer2.materials[j].SetColor("_Tint_B", materialDefinitions[j].TintB);
						if ((bool)materialDefinitions[j].Print)
						{
							skinnedMeshRenderer2.materials[j].SetTexture("_Print", materialDefinitions[j].Print);
						}
						else
						{
							skinnedMeshRenderer2.materials[j].SetTexture("_Print", Resources.Load<Texture2D>("T_Transparent"));
						}
					}
				}
			}
			if (apparel.AddCopyPoseScript)
			{
				gameObject.AddComponent<CopyPose>();
			}
			else
			{
				componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer3 in componentsInChildren)
				{
					Transform[] array = new Transform[skinnedMeshRenderer3.bones.Length];
					for (int k = 0; k < skinnedMeshRenderer3.bones.Length; k++)
					{
						if (!(skinnedMeshRenderer3.bones[k] == null))
						{
							array[k] = FindChildByName(skinnedMeshRenderer3.bones[k].name, MainMesh.rootBone);
						}
					}
					skinnedMeshRenderer3.bones = array;
					skinnedMeshRenderer3.gameObject.layer = LayerMask.NameToLayer("Customer");
				}
			}
		}
		if (apparel.FootOffset.HeightOffset >= 0f)
		{
			TransformBone[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<TransformBone>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].SetOffset(apparel.FootOffset);
			}
		}
		setTextureProperty(new CC_Property
		{
			propertyName = ApparelTables[slot].MaskProperty
		}, save: false, apparel.Mask);
		StoredCharacterData.ApparelNames[slot] = apparel.Name;
		StoredCharacterData.ApparelMaterials[slot] = materialSelection;
	}

	public void setApparelByName(string name, int slot, int materialSelection)
	{
		int num = ApparelTables[slot].Items.FindIndex((scrObj_Apparel.Apparel t) => t.Name == name);
		if (num != -1)
		{
			setApparel(num, slot, materialSelection);
		}
	}

	private Transform FindChildByName(string name, Transform parentObj)
	{
		if (parentObj.name == name)
		{
			return parentObj.transform;
		}
		foreach (Transform item in parentObj)
		{
			Transform transform = FindChildByName(name, item);
			if ((bool)transform)
			{
				return transform;
			}
		}
		return null;
	}

	public void setBlendshapeByName(string name, float value, bool save = true)
	{
		if (name != "")
		{
			BlendshapeManager[] componentsInChildren = base.gameObject.GetComponentsInChildren<BlendshapeManager>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].setBlendshape(name, value);
			}
			if (save)
			{
				saveProperty(ref StoredCharacterData.Blendshapes, new CC_Property
				{
					propertyName = name,
					floatValue = value
				});
			}
		}
	}

	public List<Material> getRelevantMaterials(int materialIndex, string meshTag)
	{
		List<Material> list = new List<Material>();
		if (materialIndex != -1)
		{
			if (meshTag != "")
			{
				foreach (SkinnedMeshRenderer item3 in getMeshByTag(meshTag))
				{
					if (item3.materials.Length > materialIndex)
					{
						list.Add(item3.materials[materialIndex]);
					}
				}
			}
			else
			{
				SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
				{
					if (skinnedMeshRenderer.materials.Length > materialIndex)
					{
						list.Add(skinnedMeshRenderer.materials[materialIndex]);
					}
				}
			}
		}
		else if (meshTag != "")
		{
			foreach (SkinnedMeshRenderer item4 in getMeshByTag(meshTag))
			{
				Material[] materials = item4.materials;
				foreach (Material item in materials)
				{
					list.Add(item);
				}
			}
		}
		else
		{
			SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] materials = componentsInChildren[i].materials;
				foreach (Material item2 in materials)
				{
					list.Add(item2);
				}
			}
		}
		return list;
	}

	public List<SkinnedMeshRenderer> getMeshByTag(string tag)
	{
		List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>();
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.tag == tag)
			{
				SkinnedMeshRenderer[] componentsInChildren2 = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer item in componentsInChildren2)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public void setTextureProperty(CC_Property p, bool save = false, Texture2D t = null)
	{
		foreach (Material relevantMaterial in getRelevantMaterials(p.materialIndex, p.meshTag))
		{
			relevantMaterial.SetTexture(p.propertyName, (t != null) ? t : ((Texture2D)Resources.Load(p.stringValue)));
		}
		if (save)
		{
			saveProperty(ref StoredCharacterData.TextureProperties, p);
		}
	}

	public void setFloatProperty(CC_Property p, bool save = false)
	{
		foreach (Material relevantMaterial in getRelevantMaterials(p.materialIndex, p.meshTag))
		{
			relevantMaterial.SetFloat(p.propertyName, p.floatValue);
		}
		if (p.propertyName == "Height")
		{
			ScaleCharacter[] componentsInChildren = base.gameObject.GetComponentsInChildren<ScaleCharacter>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetHeight(p.floatValue);
			}
		}
		if (p.propertyName == "Width")
		{
			ScaleCharacter[] componentsInChildren = base.gameObject.GetComponentsInChildren<ScaleCharacter>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetWidth(p.floatValue);
			}
		}
		if (save)
		{
			saveProperty(ref StoredCharacterData.FloatProperties, p);
		}
	}

	public void setColorProperty(CC_Property p, Color c, bool save = false)
	{
		foreach (Material relevantMaterial in getRelevantMaterials(p.materialIndex, p.meshTag))
		{
			if (p.propertyName == "_Hair_Color")
			{
				relevantMaterial.SetColor("_BaseColor", c);
			}
			relevantMaterial.SetColor(p.propertyName, c);
		}
		p.stringValue = ColorUtility.ToHtmlStringRGBA(c);
		if (save)
		{
			saveProperty(ref StoredCharacterData.ColorProperties, p);
		}
	}

	public void setHairColor(CC_Property p, Color color, int slot, bool save = false)
	{
		setColorProperty(p, color);
		setColorProperty(new CC_Property
		{
			propertyName = HairTables[(slot != -1) ? slot : 0].TintProperty
		}, color);
		if (!save)
		{
			return;
		}
		if (slot == -1)
		{
			for (int i = 0; i < StoredCharacterData.HairColor.Count; i++)
			{
				StoredCharacterData.HairColor[i] = p;
			}
		}
		else
		{
			StoredCharacterData.HairColor[slot] = p;
		}
	}

	public void saveProperty(ref List<CC_Property> properties, CC_Property p)
	{
		int num = properties.FindIndex((CC_Property t) => t.materialIndex == p.materialIndex && t.propertyName == p.propertyName && t.meshTag == p.meshTag);
		if (num == -1)
		{
			properties.Add(p);
		}
		else
		{
			properties[num] = p;
		}
	}
}
