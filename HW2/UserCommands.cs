using HW3;

public class UserCommands : CommandContainer
{
    public short MaxNumber { get; set; }
	public short MaxLength { get; set; }
    public UserCommands() : base()
	{
		MaxNumber = 0;
		MaxLength = 0;
	}

	public List<string>? GetNames()
	{
		if (this.Count == 0)
		{
			return null;
		}

		List<string> names = new();

        foreach (AbstractCommand cmd in this)
        {
            names.Add(cmd.GetCode());
        }

        return names;
	}

	public bool HasDuplicate(string name)
	{
		foreach (var cmd in this)
		{
			if (cmd.GetCode() == name)
			{
				return true;
			}
		}

		return false;
	}

	public bool TryAdd(string name)
	{
        if (MaxNumber <= 0)
        {
            throw new Exception("Не определено максимально доступное количество задач.");
        }

        if (Count == MaxNumber)
        {
            throw new TaskCountLimitException(MaxNumber);
        }

        if (MaxLength <= 0)
        {
            throw new Exception("Не определена максимальная длина задачи.");
        }

        if (name.Length > MaxLength)
        {
            throw new TaskLengthLimitException(name.Length, MaxLength);
        }

        if (HasDuplicate(name))
        {
            throw new DuplicateTaskException(name);
        }

        Add(new UserCommand(name));

        return true;
	}
}
