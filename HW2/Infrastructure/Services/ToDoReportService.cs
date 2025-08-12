using Core.Entity;
using Core.DataAccess;

namespace Infrastructure.DataAccess
{
    internal class ToDoReportService : IToDoReportService
    {
        private readonly IToDoRepository _toDoRepository;
        public ToDoReportService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }
        public async Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStats(Guid userId, CancellationToken cancelToken)
        {
            var items = await _toDoRepository.GetAllByUserId(userId, cancelToken);

            return await Task<(int total, int completed, int active, DateTime generatedAt)>.Run(() =>
            {
                (int total, int completed, int active, DateTime generatedAt) result = (0, 0, 0, DateTime.Now);
                foreach (ToDoItem item in items)
                {
                    if (item.State == ToDoItemState.Completed)
                    {
                        result.completed += 1;
                    }
                    else if (item.State == ToDoItemState.Active)
                    {
                        result.active += 1;
                    }

                    result.total += 1;
                }

                return result;
            });
        }
    }
}
