using System;

namespace HW3
{
    public static class ProgramInfo
    {
        public static readonly Version version = new Version(1, 0);
        public static readonly DateTime creationDate = new DateTime(2025, 3, 27);

        public enum State
        {
            None,
            Started,
            Finished
        }

        public static State state = State.None;
        public static string? userName = string.Empty;
        public static CommandContainer mainCommands = new();
        public static CommandContainer userCommands = new();
    }
}
