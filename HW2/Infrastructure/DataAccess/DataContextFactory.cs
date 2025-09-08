
using HW2.Core.DataAccess;

namespace HW2.Infrastructure.DataAccess
{
    internal class DataContextFactory : IDataContextFactory<ToDoDataContext>
    {
        public ToDoDataContext CreateDataContext()
        {
            string? pgPswd = Environment.GetEnvironmentVariable("PG_PSWD");

            return new ToDoDataContext($"User ID=postgres;Password={pgPswd};Host=localhost;Port=5432;Database=ToDoList;");
        }
    }
}
