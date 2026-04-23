using System;
using System.Collections.Generic;

[Serializable]
public class GradeCardSubmitSet
{
	public List<CardData> m_CardDataList;

	public int m_ServiceLevel;

	public int m_DayPassed;

	public float m_MinutePassed;
}
