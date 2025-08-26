using System.Text.Json;
using Core.DataAccess;
using Core.Entity;
using Telegram.Bot.Types;

namespace HW2.Infrastructure.DataAccess
{
    internal class FileUserRepository : IUserRepository
    {
        private string _repositoryDir = "users";

        public FileUserRepository(string repositoryDir)
        {
            _repositoryDir = repositoryDir;
        }
        public async Task Add(ToDoUser user, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                Directory.CreateDirectory(_repositoryDir);
            }

            await Task.Run(() =>
            {
                string json = JsonSerializer.Serialize(user);

                StreamWriter wstream = File.CreateText(GetFileName(user.UserId));
                wstream.Write(json);
                wstream.Flush();
            });
        }

        public Task<ToDoUser?> GetUser(Guid userId, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return Task.FromResult<ToDoUser?>(null);
            }

            string userFileName = GetFileName(userId);
            if (!File.Exists(userFileName))
            {
                return Task.FromResult<ToDoUser?>(null);
            }

            StreamReader rstream = File.OpenText(GetFileName(userId));
            string userJson = rstream.ReadToEnd();
            ToDoUser? toDoUser = JsonSerializer.Deserialize<ToDoUser>(userJson);

            return Task.FromResult(toDoUser);
        }

        public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return null;
            }

            ToDoUser? resToDoUser = null;

            await Task.Run(() =>
            {
                var usersFiles = Directory.EnumerateFiles(_repositoryDir);
                foreach (var userFile in usersFiles)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    StreamReader rstream = File.OpenText(userFile);
                    string userJson = rstream.ReadToEnd();

                    ToDoUser? toDoUser = JsonSerializer.Deserialize<ToDoUser>(userJson);

                    if (toDoUser?.TelegramUserId == telegramUserId)
                    {
                        resToDoUser = toDoUser;
                        break;
                    }
                }
            });

            return resToDoUser;
        }

        private string GetFileName(Guid id)
        {
            return Path.Combine(_repositoryDir, id.ToString(), ".json");
        }
    }
}
