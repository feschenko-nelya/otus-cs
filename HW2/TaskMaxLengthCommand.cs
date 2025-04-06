using System;
using HW3;
using HW4;

public class TaskMaxLengthCommand : AbstractCommand
{
	public TaskMaxLengthCommand()
	{
	}

    public override void Execute(string arg)
    {
        Console.WriteLine("Введите максимально допустимую длину задачи");
        GetNumberCommand getNumberCommand = new(1, 100);
        getNumberCommand.Execute();
        ProgramInfo.tasksMaxLength = getNumberCommand.Number;

        if (ProgramInfo.tasksMaxLength == 0)
        {
            ProgramInfo.state = ProgramInfo.State.Finished;
        }
    }

    public override string GetCode()
    {
        return "/taskmaxlength";
    }

    public override string GetInfo()
    {
        return "Максимально допустимая длина задачи";
    }
}
