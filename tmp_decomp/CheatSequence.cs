using System.Collections.Generic;

public class CheatSequence
{
	public List<EGameBaseKey> m_Keys = new List<EGameBaseKey>();

	private int m_Matched;

	public CheatSequence(List<EGameBaseKey> keys)
	{
		m_Keys.AddRange(keys);
	}

	public bool Update(EGameBaseKey key)
	{
		if (m_Keys[m_Matched] == key)
		{
			m_Matched++;
			if (m_Matched == m_Keys.Count)
			{
				m_Matched = 0;
				return true;
			}
		}
		else
		{
			m_Matched = 0;
		}
		return false;
	}
}
