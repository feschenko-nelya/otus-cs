
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

        public async Task Add(ToDoItem item, CancellationToken cancelToken)
        {
            using (var dbContext = _toDofactory.CreateDataContext())
            {   
                await dbContext.InsertAsync(ModelMapper.MapToModel(item), token: cancelToken);
            }
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

        public async Task Delete(Guid id, CancellationToken cancelToken)
        {
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                await dbContext.GetTable<ToDoItemModel>()
                      .Where(item => item.Id == id)
                      .DeleteAsync(token: cancelToken);
            }
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
                List<ToDoItemModel> modelItems = await dbContext.GetTable<ToDoItemModel>()
                                                      .Where(item => item.UserId == userId && predicate(ModelMapper.MapFromModel(item)))
                                                      .LoadWith(im => im.User)
                                                      .LoadWith(im => im.List)
                                                      .ToListAsync(token: cancelToken);

                await Task.Run(() =>
                {
                    foreach (var modelItem in modelItems)
                        items.Add(ModelMapper.MapFromModel(modelItem));
                });
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
                    .LoadWith(im => im.List)
                    .FirstOrDefault(u => u.Id == id);
            }

            if (item == null)
                return Task.FromResult<ToDoItem?>(null);

            return Task.FromResult(ModelMapper.MapFromModel(item) ?? null);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancelToken)
        {
            Func<ToDoItem, bool> predicate = (item) => item.State == ToDoItemState.Active;
            return await Find(userId, predicate, cancelToken);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken cancelToken)
        {
            Func<ToDoItem, bool> predicate = (item) => true;
            return await Find(userId, predicate, cancelToken);
        }

        public async Task Update(ToDoItem item, CancellationToken cancelToken)
        {
            ToDoItem? prevItem = await Get(item.Id, cancelToken);
            if (prevItem == null) return;

            using (var dbContext = _toDofactory.CreateDataContext())
            {
                await dbContext.GetTable<ToDoItemModel>()
                      .Where(sqlItem => sqlItem.Id == item.Id)
                      .Set(sqlItem => sqlItem.Name, item.Name)
                      .Set(sqlItem => sqlItem.State, item.State)
                      .Set(sqlItem => sqlItem.Deadline, item.Deadline)
                      .UpdateAsync(token: cancelToken);
                
                if (item.State != prevItem.State)
                {
                    await dbContext.GetTable<ToDoItemModel>()
                      .Where(sqlItem => sqlItem.Id == item.Id)
                      .Set(sqlItem => sqlItem.StateChangedAt, DateTime.UtcNow)
                      .UpdateAsync(token: cancelToken);
                }
            }
        }
    }
}
