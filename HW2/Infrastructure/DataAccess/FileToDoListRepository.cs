
using System.Text.Json;
using HW2.Core.DataAccess;
using HW2.Core.Entities;

namespace HW2.Infrastructure.DataAccess
{
    public class FileToDoListRepository : IToDoListRepository
    {
        private string _repositoryDir = "toDoLists";

        public FileToDoListRepository(string repositoryDir)
        {
            _repositoryDir = repositoryDir;
        }
        public async Task Add(ToDoList list, CancellationToken ct)
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
                string json = JsonSerializer.Serialize<ToDoList>(list);

                StreamWriter wstream = File.CreateText(GetFileName(list.Id));
                wstream.Write(json);
                wstream.Flush();
                wstream.Close();
            });
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return Task.CompletedTask;
            }

            string itemFile = id.GetHashCode().ToString() + ".json";
            FileInfo fileInfo = new(Path.Combine(_repositoryDir, itemFile));

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return Task.FromResult<bool>(true);
            }

            string[] files = Directory.GetFiles(_repositoryDir);
            foreach (var file in files)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var streamR = File.OpenText(file);

                var json = streamR.ReadToEnd();
                ToDoList? toDoList = JsonSerializer.Deserialize<ToDoList>(json);
                if (toDoList == null)
                    continue;

                if (toDoList.UserId == userId && toDoList.Name == name)
                {
                    return Task.FromResult<bool>(true);
                }
            }

            return Task.FromResult<bool>(false);
        }

        public Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return Task.FromResult<ToDoList?>(null);
            }

            string userFileName = GetFileName(id);
            if (!File.Exists(userFileName))
            {
                return Task.FromResult<ToDoList?>(null);
            }

            StreamReader rstream = File.OpenText(GetFileName(id));
            string userJson = rstream.ReadToEnd();
            ToDoList? toDoList = JsonSerializer.Deserialize<ToDoList>(userJson);

            return Task.FromResult(toDoList);
        }

        public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return [];
            }

            List<ToDoList> result = [];

            await Task.Run(() =>
            {
                string[] files = Directory.GetFiles(_repositoryDir);
                foreach (var file in files)
                {
                    if (ct.IsCancellationRequested)
                        break;

                    var streamR = File.OpenText(file);

                    var json = streamR.ReadToEnd();
                    ToDoList? toDoList = JsonSerializer.Deserialize<ToDoList>(json);
                    if (toDoList == null)
                        continue;

                    if (toDoList.UserId == userId)
                    {
                        result.Add(toDoList);
                    }
                }
            });


            return result;
        }

        private string GetFileName(Guid id)
        {
            return Path.Combine(_repositoryDir, id.GetHashCode().ToString() + ".json");
        }
    }
}
