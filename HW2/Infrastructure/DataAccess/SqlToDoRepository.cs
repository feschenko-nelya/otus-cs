
using Core.DataAccess;
using Core.Entity;
using HW2.Core.DataAccess;
using HW2.Core.DataAccess.Models;
using LinqToDB;

namespace HW2.Infrastructure.DataAccess
{
    internal class SqlToDoRepository : IToDoRepository
    {
        private readonly IDataContextFactory<ToDoDataContext> _toDofactory;

        public SqlToDoRepository(IDataContextFactory<ToDoDataContext> factory)
        {
            _toDofactory = factory;
        }
        public Task Add(ToDoItem item, CancellationToken cancelToken)
        {
            using (var dbContext = _toDofactory.CreateDataContext())
            {   
                dbContext.Insert(ModelMapper.MapToModel(item));
            }

            return Task.CompletedTask;
        }

        public Task<int> CountActive(Guid userId, CancellationToken cancelToken)
        {
            int count = 0;

            using (var dbContext = _toDofactory.CreateDataContext())
            {
                count = dbContext.GetTable<ToDoItemModel>()
                          .Where(item => item.UserId == userId)
                          .Count();
            }

            return Task.FromResult(count);
        }

        public Task Delete(Guid id, CancellationToken cancelToken)
        {
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                dbContext.GetTable<ToDoItemModel>().Where(item => item.Id == id).Delete();
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsByName(Guid userId, string name, CancellationToken cancelToken)
        {
            bool isExist = false;
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                isExist = dbContext.GetTable<ToDoItemModel>()
                          .FirstOrDefault(u => u.Name == name) != null;
            }

            return Task.FromResult(isExist);
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken cancelToken)
        {
            List<ToDoItem> items = new();

            using (var dbContext = _toDofactory.CreateDataContext())
            {
                await dbContext.GetTable<ToDoItemModel>()
                          .Where(item => item.UserId == userId && predicate(ModelMapper.MapFromModel(item)))
                          .ForEachAsync(item => items.Add(ModelMapper.MapFromModel(item)));
            }

            return items;
        }

        public Task<ToDoItem?> Get(Guid id, CancellationToken cancelToken)
        {
            ToDoItemModel? item = null;
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                item = dbContext.GetTable<ToDoItemModel>()
                    .LoadWith(im => im.User)
                    .FirstOrDefault(u => u.Id == id);
            }

            if (item == null)
                return Task.FromResult<ToDoItem?>(null);

            return Task.FromResult(ModelMapper.MapFromModel(item) ?? null);
        }

        public Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancelToken)
        {
            Func<ToDoItem, bool> predicate = (item) => item.State == ToDoItemState.Active;
            return Find(userId, predicate, cancelToken);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken cancelToken)
        {
            List<ToDoItem> items = new();

            using (var dbContext = _toDofactory.CreateDataContext())
            {
                await dbContext.GetTable<ToDoItemModel>()
                      .ForEachAsync(item => items.Add(ModelMapper.MapFromModel(item)));
            }

            return items;
        }

        public Task Update(ToDoItem item, CancellationToken cancelToken)
        {
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                dbContext.GetTable<ToDoItemModel>()
                    .Where(sqlItem => sqlItem.Id == item.Id)
                    .Set(sqlItem => sqlItem.Name, item.Name)
                    .Set(sqlItem => sqlItem.State, item.State)
                    .Set(sqlItem => sqlItem.Deadline, item.Deadline)
                    .Update();
                          
            }

            return Task.CompletedTask;
        }
    }
}
