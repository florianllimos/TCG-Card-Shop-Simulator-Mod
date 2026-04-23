using UnityEngine;

namespace SkyboxBlenderSpace;

public class SpacebarClick2 : MonoBehaviour
{
	public SkyboxBlender skyboxScript;

	private bool isStopped;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			skyboxScript.Blend(singlePassBlend: true);
			isStopped = false;
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (isStopped)
			{
				skyboxScript.Blend(singlePassBlend: true);
				isStopped = false;
			}
			else
			{
				skyboxScript.Stop();
				isStopped = true;
			}
		}
	}
}
