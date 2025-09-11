
using Core.DataAccess;
using Core.Entity;
using HW2.Core.DataAccess;
using HW2.Core.DataAccess.Models;
using LinqToDB;

namespace HW2.Infrastructure.DataAccess
{
    internal class SqlUserRepository : IUserRepository
    {
        private readonly IDataContextFactory<ToDoDataContext> _toDofactory;

        public SqlUserRepository(IDataContextFactory<ToDoDataContext> factory)
        {
            _toDofactory = factory;
        }
        public async Task Add(ToDoUser user, CancellationToken cancelToken)
        {
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                await dbContext.InsertAsync(ModelMapper.MapToModel(user), token: cancelToken);
            }
        }

        public Task<ToDoUser?> GetUser(Guid userId, CancellationToken cancelToken)
        {
            ToDoUserModel? user = null;
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                user = dbContext.GetTable<ToDoUserModel>()
                       .FirstOrDefault(u => u.Id == userId);
            }

            if (user == null)
                return Task.FromResult<ToDoUser?>(null);

            return Task.FromResult(ModelMapper.MapFromModel(user) ?? null);
        }

        public Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken cancelToken)
        {
            ToDoUserModel? user = null;
            using (var dbContext = _toDofactory.CreateDataContext())
            {
                user = dbContext.GetTable<ToDoUserModel>()
                       .FirstOrDefault(u => u.TelegramId == telegramUserId);
            }

            if (user == null)
                return Task.FromResult<ToDoUser?>(null);

            return Task.FromResult(ModelMapper.MapFromModel(user) ?? null);
        }
    }
}
