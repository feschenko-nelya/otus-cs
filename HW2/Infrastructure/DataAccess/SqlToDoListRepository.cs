
using HW2.Core.DataAccess;
using HW2.Core.DataAccess.Models;
using HW2.Core.Entities;
using LinqToDB;

namespace HW2.Infrastructure.DataAccess
{
    internal class SqlToDoListRepository : IToDoListRepository
    {
        private readonly IDataContextFactory<ToDoDataContext> _toDofactory;

        public SqlToDoListRepository(IDataContextFactory<ToDoDataContext> factory)
        {
            _toDofactory = factory;
        }
        public Task Add(ToDoList list, CancellationToken ct)
        {
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                dbContext.Insert(ModelMapper.MapToModel(list));
            }

            return Task.CompletedTask;
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                dbContext.GetTable<ToDoListModel>().Where(list => list.Id == id).Delete();
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            bool isExist = false;
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                isExist = dbContext.GetTable<ToDoListModel>()
                          .FirstOrDefault(list => list.Name == name) != null;
            }

            return Task.FromResult(isExist);
        }

        public Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            ToDoListModel? list = null;
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                list = dbContext.GetTable<ToDoListModel>()
                          .FirstOrDefault(u => u.Id == id);
            }

            if (list == null)
                return Task.FromResult<ToDoList?>(null);

            return Task.FromResult(ModelMapper.MapFromModel(list) ?? null);
        }

        public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            List<ToDoList> lists = new();

            using (var dbContext = _toDofactory.CreateDataContext())
            {
                await dbContext.GetTable<ToDoListModel>()
                      .Where(list => list.UserId == userId)
                      .ForEachAsync(list => lists.Add(ModelMapper.MapFromModel(list)));
            }

            return lists;
        }
    }
}
