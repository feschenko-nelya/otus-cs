using HW2.User;
using HW3;

public class ToDoItems : List<ToDoItem>
{
    public short MaxNumber { get; set; }
	public short MaxLength { get; set; }
    public ToDoItems() : base()
	{
		MaxNumber = 0;
		MaxLength = 0;
	}

    public ToDoItem? GetByGuid(Guid id)
    {
        foreach (ToDoItem item in this)
        {
            if (item.Id.Equals(id))
            {
                return item;
            }
        }

        return null;
    }

	public ToDoItems GetByState(ToDoItemState commandState)
	{
        ToDoItems result = new();

        if (this.Count == 0)
		{
			return result;
		}

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
