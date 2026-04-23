using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[AddComponentMenu("Skybox Blender/Skybox Blender")]
public class SkyboxBlender : MonoBehaviour
{
	[Tooltip("The materials you want to blend to linearly.")]
	public Material[] skyboxMaterials;

	[Tooltip("Checking this will instantly make the first material your current skybox.")]
	public bool makeFirstMaterialSkybox;

	[Min(0f)]
	[Tooltip("The speed of the blending between the skyboxes.")]
	public float blendSpeed = 0.5f;

	[Min(0f)]
	[Tooltip("The time to wait before blending the next skybox material.")]
	public float timeToWait;

	[Tooltip("If enabled, will loop the materials list. When the blender reaches the last skybox in the list, it'll blend back to the first one.")]
	public bool loop = true;

	[Tooltip("If enabled, the lighting of the world will be updated to that of the skyboxes blending.")]
	public bool updateLighting;

	[Tooltip("If enabled, the reflections of the world will be updated to that of the skyboxes blending.")]
	public bool updateReflections;

	[Range(1f, 30f)]
	[Tooltip("Set how many frames need to pass during blend before updating the reflections & lighting each time. Updating these take a toll on performance so the higher this number is, the more performant your game will be (during blend) but the less accurate the lighting & reflections update will be. The less this number is, the slower the game will be but the accuracy increases. By average the best performance/accuracy results is setting it between 5-10.")]
	public int updateEveryFrames = 5;

	[Tooltip("Keep rotating the skybox infinetly while blending.")]
	public bool keepRotating;

	[Tooltip("if you would prefer a certain degree to rotate the skybox to during blending - 360 is a full turn.")]
	public float rotateToAngle = 180f;

	[Min(0f)]
	[Tooltip("The speed of the skybox rotation.")]
	public float rotationSpeed;

	[Tooltip("If enabled, the rotation will stop when the blend finishes. If disabled, even after blending the skybox will continue rotating. TAKE NOTE: if loop is enabled in blend options. This will not take effect.")]
	public bool stopRotationOnBlendFinish;

	private Material defaultSkyboxMaterial;

	private Material skyboxBlenderMaterial;

	private Texture currentTexture;

	private float totalBlendValue = 1f;

	public float blendValue;

	private float defaultBlend;

	private float defaultRotation;

	private float rotationSpeedValue;

	private int index;

	private int indexToBlend;

	private int usedBlend;

	private int LightAndReflectionFrames;

	private bool linearBlending;

	private bool currentSkyboxNotFirstMaterialBlending;

	private bool comingFromLoop;

	private bool rotateSkybox;

	private bool oneTickBlend;

	private bool stillRunning;

	private bool singleBlend;

	private bool stopped;

	private bool blendByIndex;

	private bool stopRotation;

	private bool blendFinished;

	private bool blendingCurrentSkyToListNotSingleBlend;

	private bool isLinearBlend;

	private bool isSetReflectionProbeOnStart;

	private ReflectionProbe reflectionProbe;

	private Cubemap cubemap;

	public int CurrentIndex => index;

	private void Awake()
	{
		skyboxBlenderMaterial = Resources.Load("Material & Shader/Skybox Blend Material", typeof(Material)) as Material;
		if ((bool)skyboxBlenderMaterial)
		{
			defaultBlend = skyboxBlenderMaterial.GetFloat("_BlendCubemaps");
			defaultRotation = skyboxBlenderMaterial.GetFloat("_Rotation");
			defaultSkyboxMaterial = skyboxBlenderMaterial;
			InspectorAndAwakeChanges();
		}
		else
		{
			Debug.LogWarning("Can't find Skybox Blend Material in resources. Please re-import!");
		}
		if (updateLighting || updateReflections)
		{
			SetReflectionProbe();
			UpdateLightingAndReflections(forceUpdate: true);
		}
	}

	private void OnValidate()
	{
		if (skyboxBlenderMaterial == null)
		{
			skyboxBlenderMaterial = Resources.Load("Material & Shader/Skybox Blend Material", typeof(Material)) as Material;
		}
		InspectorAndAwakeChanges();
	}

	private void OnApplicationQuit()
	{
		skyboxBlenderMaterial.SetFloat("_BlendCubemaps", defaultBlend);
		skyboxBlenderMaterial.SetFloat("_Rotation", defaultRotation);
		if (currentTexture != null)
		{
			skyboxBlenderMaterial.SetTexture("_Tex", currentTexture);
		}
		RenderSettings.skybox = defaultSkyboxMaterial;
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			if (!(RenderSettings.skybox == null) && RenderSettings.skybox.HasProperty("_Tex"))
			{
				skyboxBlenderMaterial.SetTexture("_Tex", RenderSettings.skybox.GetTexture("_Tex"));
				skyboxBlenderMaterial.SetColor("_Tint", RenderSettings.skybox.GetColor("_Tint"));
			}
		}
		else
		{
			if (updateReflections && !isSetReflectionProbeOnStart && !SetReflectionProbeTexture())
			{
				return;
			}
			if (linearBlending && !stopped)
			{
				usedBlend = 1;
				blendValue += Time.deltaTime * blendSpeed;
				skyboxBlenderMaterial.SetFloat("_BlendCubemaps", blendValue);
				UpdateLightingAndReflections();
				if (skyboxBlenderMaterial.GetFloat("_BlendCubemaps") >= totalBlendValue)
				{
					blendFinished = true;
					linearBlending = false;
					blendValue = 0f;
					StopAllCoroutines();
					skyboxBlenderMaterial.SetFloat("_BlendCubemaps", 0f);
					SetSkyBoxes(firstTex: true, index, secondTex: false, 0, apply: true);
					UpdateLightingAndReflections(forceUpdate: true);
					if (comingFromLoop)
					{
						index = 0;
					}
					if (index + 1 < skyboxMaterials.Length)
					{
						if (!comingFromLoop)
						{
							index++;
						}
						comingFromLoop = false;
						SetSkyBoxes(firstTex: true, index);
						if (index + 1 < skyboxMaterials.Length)
						{
							SetSkyBoxes(firstTex: false, 0, secondTex: true, index + 1);
						}
						if (index - (skyboxMaterials.Length - 1) > 0)
						{
							if (!oneTickBlend)
							{
								linearBlending = true;
							}
						}
						else if (!oneTickBlend)
						{
							StartCoroutine(WaitBeforeBlending());
						}
					}
					if (index >= skyboxMaterials.Length - 1)
					{
						if (loop)
						{
							if (oneTickBlend)
							{
								stillRunning = false;
								return;
							}
							SetSkyBoxes(firstTex: true, index, secondTex: true, 0, apply: true);
							comingFromLoop = true;
							StartCoroutine(WaitBeforeBlending());
						}
						else
						{
							stillRunning = false;
							if (stopRotationOnBlendFinish)
							{
								StopRotation();
							}
						}
					}
				}
				else
				{
					blendFinished = false;
				}
			}
			if (singleBlend && !stopped)
			{
				blendValue += Time.deltaTime * blendSpeed;
				skyboxBlenderMaterial.SetFloat("_BlendCubemaps", blendValue);
				UpdateLightingAndReflections();
				if (skyboxBlenderMaterial.GetFloat("_BlendCubemaps") >= totalBlendValue)
				{
					blendFinished = true;
					singleBlend = false;
					blendValue = 0f;
					StopAllCoroutines();
					if (blendByIndex)
					{
						index = indexToBlend;
						blendByIndex = false;
					}
					else if (index + 1 < skyboxMaterials.Length)
					{
						index++;
					}
					else
					{
						index = 0;
					}
					skyboxBlenderMaterial.SetFloat("_BlendCubemaps", 0f);
					SetSkyBoxes(firstTex: true, index, secondTex: false, 0, apply: true);
					UpdateLightingAndReflections(forceUpdate: true);
					stillRunning = false;
					if (stopRotationOnBlendFinish)
					{
						StopRotation();
					}
				}
				else
				{
					blendFinished = false;
				}
			}
			if (currentSkyboxNotFirstMaterialBlending && !stopped)
			{
				usedBlend = 2;
				blendValue += Time.deltaTime * blendSpeed;
				skyboxBlenderMaterial.SetFloat("_BlendCubemaps", blendValue);
				UpdateLightingAndReflections();
				if (skyboxBlenderMaterial.GetFloat("_BlendCubemaps") >= totalBlendValue)
				{
					blendFinished = true;
					currentSkyboxNotFirstMaterialBlending = false;
					blendValue = 0f;
					StopAllCoroutines();
					int secondTexIndex = 1;
					skyboxBlenderMaterial.SetFloat("_BlendCubemaps", 0f);
					if (skyboxMaterials.Length == 1)
					{
						secondTexIndex = 0;
					}
					SetSkyBoxes(firstTex: true, 0, secondTex: true, secondTexIndex, apply: true);
					UpdateLightingAndReflections(forceUpdate: true);
					if (oneTickBlend)
					{
						stillRunning = false;
					}
					else
					{
						StartCoroutine(WaitBeforeBlending());
					}
					if (stopRotationOnBlendFinish && !blendingCurrentSkyToListNotSingleBlend)
					{
						StopRotation();
					}
					blendingCurrentSkyToListNotSingleBlend = false;
				}
				else
				{
					blendFinished = false;
				}
			}
			rotationSpeedValue += Time.deltaTime * rotationSpeed;
			if (keepRotating)
			{
				skyboxBlenderMaterial.SetFloat("_Rotation", rotationSpeedValue);
				return;
			}
			if (skyboxBlenderMaterial.GetFloat("_Rotation") < rotateToAngle)
			{
				skyboxBlenderMaterial.SetFloat("_Rotation", rotationSpeedValue);
				return;
			}
			rotateSkybox = false;
			skyboxBlenderMaterial.SetFloat("_Rotation", rotateToAngle);
		}
	}

	private void SetSkyBoxes(bool firstTex = false, int firstTexIndex = 0, bool secondTex = false, int secondTexIndex = 0, bool apply = false)
	{
		if (firstTex)
		{
			skyboxBlenderMaterial.SetTexture("_Tex", skyboxMaterials[firstTexIndex].GetTexture("_Tex"));
			skyboxBlenderMaterial.SetColor("_Tint", skyboxMaterials[firstTexIndex].GetColor("_Tint"));
		}
		if (secondTex)
		{
			skyboxBlenderMaterial.SetTexture("_Tex2", skyboxMaterials[secondTexIndex].GetTexture("_Tex"));
			skyboxBlenderMaterial.SetColor("_Tint2", skyboxMaterials[secondTexIndex].GetColor("_Tint"));
		}
		if (apply)
		{
			RenderSettings.skybox = skyboxBlenderMaterial;
		}
	}

	private void PrepareMaterialForBlend(int skyboxIndex)
	{
		skyboxBlenderMaterial.SetTexture("_Tex", RenderSettings.skybox.GetTexture("_Tex"));
		skyboxBlenderMaterial.SetTexture("_Tex2", skyboxMaterials[skyboxIndex].GetTexture("_Tex"));
		skyboxBlenderMaterial.SetColor("_Tint", RenderSettings.skybox.GetColor("_Tint"));
		skyboxBlenderMaterial.SetColor("_Tint2", skyboxMaterials[skyboxIndex].GetColor("_Tint"));
	}

	private IEnumerator WaitBeforeBlending()
	{
		isLinearBlend = true;
		yield return new WaitForSeconds(timeToWait);
		linearBlending = true;
	}

	private void InspectorAndAwakeChanges()
	{
		if (makeFirstMaterialSkybox)
		{
			if (skyboxMaterials.Length >= 1)
			{
				if (skyboxMaterials[0] != null)
				{
					skyboxBlenderMaterial.SetTexture("_Tex", skyboxMaterials[0].GetTexture("_Tex"));
					skyboxBlenderMaterial.SetColor("_Tint", skyboxMaterials[0].GetColor("_Tint"));
					RenderSettings.skybox = skyboxBlenderMaterial;
				}
			}
			else
			{
				Debug.LogWarning("You need to set a material first to make it the skybox");
			}
		}
		if (skyboxMaterials != null && skyboxMaterials.Length > 1 && skyboxMaterials[1] != null)
		{
			skyboxBlenderMaterial.SetTexture("_Tex2", skyboxMaterials[1].GetTexture("_Tex"));
			skyboxBlenderMaterial.SetColor("_Tint2", skyboxMaterials[1].GetColor("_Tint"));
		}
	}

	private void SetReflectionProbe()
	{
		reflectionProbe = GetComponent<ReflectionProbe>();
		if (reflectionProbe == null)
		{
			reflectionProbe = base.gameObject.AddComponent<ReflectionProbe>();
		}
		reflectionProbe.cullingMask = 0;
		reflectionProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
		reflectionProbe.mode = ReflectionProbeMode.Realtime;
		reflectionProbe.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;
		if (updateReflections)
		{
			RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
			cubemap = new Cubemap(reflectionProbe.resolution, reflectionProbe.hdr ? TextureFormat.RGBAHalf : TextureFormat.RGBA32, mipChain: true);
		}
	}

	public void UpdateLightingAndReflections(bool forceUpdate = false)
	{
		if (!updateReflections && !updateLighting)
		{
			LightAndReflectionFrames = 0;
			return;
		}
		if (!forceUpdate && LightAndReflectionFrames < updateEveryFrames)
		{
			LightAndReflectionFrames++;
			return;
		}
		if (updateLighting && blendValue > 0.02f && blendValue < 0.98f)
		{
			DynamicGI.UpdateEnvironment();
		}
		if (updateReflections)
		{
			LightAndReflectionFrames = 0;
			reflectionProbe.RenderProbe();
			if (reflectionProbe.texture != null)
			{
				Graphics.CopyTexture(reflectionProbe.texture, cubemap);
				RenderSettings.customReflection = cubemap;
			}
		}
	}

	private bool SetReflectionProbeTexture()
	{
		if (!isSetReflectionProbeOnStart)
		{
			SetReflectionProbe();
			UpdateLightingAndReflections(forceUpdate: true);
			if (reflectionProbe.texture != null)
			{
				isSetReflectionProbeOnStart = true;
			}
		}
		return isSetReflectionProbeOnStart;
	}

	public void Blend(bool singlePassBlend = false, bool rotate = true)
	{
		if ((currentSkyboxNotFirstMaterialBlending && !stopped) || (isLinearBlend && !stopped))
		{
			return;
		}
		if (rotate)
		{
			rotateSkybox = true;
			stopRotation = false;
		}
		if ((stopped || stillRunning) && !singlePassBlend)
		{
			stopped = false;
			if (blendFinished && (usedBlend == 1 || usedBlend == 2) && !stillRunning)
			{
				StartCoroutine(WaitBeforeBlending());
				return;
			}
		}
		stopped = false;
		blendByIndex = false;
		StopAllCoroutines();
		currentTexture = RenderSettings.skybox.GetTexture("_Tex");
		if (blendValue > 0f)
		{
			oneTickBlend = singlePassBlend;
			return;
		}
		if (singlePassBlend)
		{
			if (index == 0 && currentTexture != skyboxMaterials[0].GetTexture("_Tex"))
			{
				PrepareMaterialForBlend(0);
				currentSkyboxNotFirstMaterialBlending = true;
			}
			else
			{
				int num = index;
				if (!stopped)
				{
					num = ((index < skyboxMaterials.Length - 1) ? (num + 1) : 0);
				}
				PrepareMaterialForBlend(num);
				singleBlend = true;
			}
			RenderSettings.skybox = skyboxBlenderMaterial;
			stillRunning = true;
		}
		else
		{
			if (skyboxMaterials.Length == 1)
			{
				if (currentTexture != skyboxMaterials[0].GetTexture("_Tex"))
				{
					PrepareMaterialForBlend(0);
					RenderSettings.skybox = skyboxBlenderMaterial;
				}
			}
			else if (index == 0 && skyboxMaterials[0] != null)
			{
				if (currentTexture == skyboxMaterials[0].GetTexture("_Tex"))
				{
					SetSkyBoxes(firstTex: true, 0, secondTex: false, 0, apply: true);
				}
				else
				{
					SetSkyBoxes(firstTex: false, 0, secondTex: true, 0, apply: true);
					currentSkyboxNotFirstMaterialBlending = true;
					blendingCurrentSkyToListNotSingleBlend = true;
				}
			}
			if (index >= skyboxMaterials.Length - 1)
			{
				comingFromLoop = true;
			}
			if (rotate)
			{
				rotateSkybox = true;
				stopRotation = false;
			}
			if (!currentSkyboxNotFirstMaterialBlending)
			{
				linearBlending = true;
				stillRunning = true;
				if (rotate)
				{
					rotateSkybox = true;
				}
			}
			isLinearBlend = true;
		}
		oneTickBlend = singlePassBlend;
	}

	public void Blend(int skyboxIndex, bool rotate = true)
	{
		stopped = false;
		if (stillRunning || index == skyboxIndex)
		{
			return;
		}
		if (skyboxIndex > skyboxMaterials.Length - 1)
		{
			Debug.Log("The passed index is bigger than the length of the skybox materials list.");
			return;
		}
		if (skyboxIndex < 0)
		{
			skyboxIndex = skyboxMaterials.Length - 1;
		}
		if (skyboxMaterials[skyboxIndex] == null)
		{
			Debug.Log("There is no material in the list with the passed index.");
			return;
		}
		StopAllCoroutines();
		currentTexture = RenderSettings.skybox.GetTexture("_Tex");
		blendByIndex = true;
		indexToBlend = skyboxIndex;
		PrepareMaterialForBlend(skyboxIndex);
		singleBlend = true;
		RenderSettings.skybox = skyboxBlenderMaterial;
		if (rotate)
		{
			rotateSkybox = true;
			stopRotation = false;
		}
		stillRunning = true;
		oneTickBlend = true;
	}

	public void Cancel()
	{
		StopAllCoroutines();
		linearBlending = false;
		singleBlend = false;
		currentSkyboxNotFirstMaterialBlending = false;
		blendingCurrentSkyToListNotSingleBlend = false;
		oneTickBlend = false;
		stopped = false;
		blendValue = 0f;
		stillRunning = false;
		isLinearBlend = false;
		comingFromLoop = false;
		skyboxBlenderMaterial.SetFloat("_BlendCubemaps", 0f);
		SetSkyBoxes(firstTex: true, index, secondTex: false, 0, apply: true);
		UpdateLightingAndReflections(forceUpdate: true);
	}

	public void Stop(bool stopRot = true)
	{
		stopped = true;
		StopAllCoroutines();
		if (stopRot && rotateSkybox)
		{
			stopRotation = true;
		}
	}

	public void Resume(bool resumeRot = true)
	{
		stopped = false;
		if (resumeRot)
		{
			stopRotation = false;
		}
		if ((usedBlend == 1 || usedBlend == 2) && blendFinished)
		{
			StartCoroutine(WaitBeforeBlending());
		}
	}

	public bool IsBlending()
	{
		if (stopped)
		{
			return false;
		}
		if (linearBlending || singleBlend || currentSkyboxNotFirstMaterialBlending)
		{
			return true;
		}
		return false;
	}

	public void Rotate()
	{
		skyboxBlenderMaterial.SetTexture("_Tex", RenderSettings.skybox.GetTexture("_Tex"));
		skyboxBlenderMaterial.SetColor("_Tint", RenderSettings.skybox.GetColor("_Tint"));
		RenderSettings.skybox = skyboxBlenderMaterial;
		rotateSkybox = true;
		stopRotation = false;
	}

	public void StopRotation()
	{
		rotateSkybox = false;
		stopRotation = false;
	}
}
