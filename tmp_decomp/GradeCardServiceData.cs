using System;
using I2.Loc;

[Serializable]
public class GradeCardServiceData
{
	public string m_ServiceName;

	public int m_ServiceDays;

	public float m_CostPerCard;

	public string GetServiceName()
	{
		return LocalizationManager.GetTranslation(m_ServiceName);
	}

	public string GetServiceDayString()
	{
		return LocalizationManager.GetTranslation("XXX Days").Replace("XXX", m_ServiceDays.ToString());
	}

	public string GetServiceDayLeftString(int dayPassed)
	{
		int num = m_ServiceDays - dayPassed;
		if (num > 1)
		{
			return LocalizationManager.GetTranslation("XXX Days").Replace("XXX", num.ToString());
		}
		return LocalizationManager.GetTranslation("XXX Day").Replace("XXX", num.ToString());
	}
}
