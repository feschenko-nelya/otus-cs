

using HW2.User;

namespace HW2.Bot_Item
{
    internal class ToDoReportService : IToDoReportService
    {
        private readonly IToDoRepository _toDoRepository;
        public ToDoReportService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }
        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            (int total, int completed, int active, DateTime generatedAt) result = (0, 0, 0, DateTime.Now);

            var items = _toDoRepository.GetAllByUserId(userId);

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
        }
    }
}
