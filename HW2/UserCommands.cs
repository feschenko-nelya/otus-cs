using HW2.User;
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

	public UserCommands? GetByState(ToDoItemState commandState)
	{
		if (this.Count == 0)
		{
			return null;
		}

        UserCommands result = new();

        foreach (ToDoItem cmd in this)
        {
            if (cmd.State == commandState)
            {
                result.Add(cmd);
            }
        }

        return result;
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

        Add(new ToDoItem(name));

        return true;
	}
}
