﻿
public class TaskCountLimitException : Exception
{
    public TaskCountLimitException(int taskCountLimit)
           : base($"Превышено максимальное количество задач равное {taskCountLimit}")
    {
    }
}
public class TaskLengthLimitException : Exception
{
    public TaskLengthLimitException(int taskLength, int taskLengthLimit)
           : base($"Длина задачи ‘{taskLength}’ превышает максимально допустимое значение {taskLengthLimit}.")
    {
    }
}
public class DuplicateTaskException : Exception
{
    public DuplicateTaskException(string task)
           : base($"Задача ‘{task}’ уже существует.")
    {
    }
}
