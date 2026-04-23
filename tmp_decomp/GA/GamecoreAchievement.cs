namespace GA;

internal class GamecoreAchievement
{
	public string Id;

	public double Value;

	public GamecoreAchievement(string id, double value)
	{
		Id = id;
		Value = value;
	}

	public override int GetHashCode()
	{
		return (17 * 23 + ((Id != null) ? Id.GetHashCode() : 0)) * 23 + Value.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj is GamecoreAchievement other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(GamecoreAchievement other)
	{
		if (Id == other.Id)
		{
			return Value.Equals(other.Value);
		}
		return false;
	}

	public override string ToString()
	{
		return $"'{Id}'({Value})";
	}
}
