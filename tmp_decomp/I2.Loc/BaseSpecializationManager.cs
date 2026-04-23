using System;
using System.Collections.Generic;

namespace I2.Loc;

public class BaseSpecializationManager
{
	public string[] mSpecializations;

	public Dictionary<string, string> mSpecializationsFallbacks;

	public virtual void InitializeSpecializations()
	{
		mSpecializations = new string[15]
		{
			"Any", "PC", "Touch", "Controller", "VR", "XBox", "PS4", "OculusVR", "ViveVR", "GearVR",
			"Android", "IOS", "PS5", "NX", "NX2"
		};
		mSpecializationsFallbacks = new Dictionary<string, string>(StringComparer.Ordinal)
		{
			{ "XBox", "Controller" },
			{ "PS4", "Controller" },
			{ "OculusVR", "VR" },
			{ "ViveVR", "VR" },
			{ "GearVR", "VR" },
			{ "Android", "Touch" },
			{ "IOS", "Touch" },
			{ "PS5", "Controller" },
			{ "NX", "Controller" },
			{ "NX2", "Controller" }
		};
	}

	public virtual string GetCurrentSpecialization()
	{
		if (mSpecializations == null)
		{
			InitializeSpecializations();
		}
		return "PC";
	}

	public virtual string GetFallbackSpecialization(string specialization)
	{
		if (mSpecializationsFallbacks == null)
		{
			InitializeSpecializations();
		}
		if (mSpecializationsFallbacks.TryGetValue(specialization, out var value))
		{
			return value;
		}
		return "Any";
	}
}
