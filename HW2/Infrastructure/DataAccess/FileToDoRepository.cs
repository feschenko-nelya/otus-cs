using System.Text.Json;
using Core.DataAccess;
using Core.Entity;

namespace Infrastructure.DataAccess
{
    internal class FileToDoRepository : IToDoRepository
    {
        private string _repositoryDir = "items";
        private SemaphoreSlim _semaphore = new SemaphoreSlim(3);

        public FileToDoRepository(string repositoryDir)
        {
            _repositoryDir = repositoryDir;
        }

        public async Task Add(ToDoItem item, CancellationToken cancelToken)
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
                string json = JsonSerializer.Serialize(item);

                StreamWriter wstream = File.CreateText(GetFileName(item.Id));
                wstream.Write(json);
                wstream.Flush();
                wstream.Close();
            });
        }

        public async Task<int> CountActive(Guid userId, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return 0;
            }

            int count = 0;

            await Task.Run(() =>
            {
                var itemFiles = Directory.EnumerateFiles(_repositoryDir);
                foreach (var itemFile in itemFiles)
                {
                    string itemJson = File.ReadAllText(itemFile);
                    ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                    if (toDoItem?.UserId == userId && toDoItem?.State == ToDoItemState.Active)
                    {
                        count++;
                    }
                }
            });

            return count;
        }

        public Task Delete(Guid id, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return Task.CompletedTask;
            }

            string fileName = GetFileName(id);

            if (File.Exists(fileName))
                File.Delete(fileName);

            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return false;
            }

            bool isNameExists = false;

            await Task.Run(() =>
            {
                var itemFiles = Directory.EnumerateFiles(_repositoryDir);
                foreach (var itemFile in itemFiles)
                {
                    string itemJson = File.ReadAllText(itemFile);
                    ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                    if (toDoItem?.UserId == userId && toDoItem?.Name == name)
                    {
                        isNameExists = true;
                        break;
                    }
                }
            });

            return isNameExists;
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return [];
            }

            List<ToDoItem> items = new();

            await Task.Run(() =>
            {
                var itemFiles = Directory.EnumerateFiles(_repositoryDir);
                foreach (var itemFile in itemFiles)
                {
                    string itemJson = File.ReadAllText(itemFile);
                    ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                    if (toDoItem?.UserId == userId && predicate(toDoItem))
                    {
                        items.Add(toDoItem);
                    }
                }
            });

            return items;
        }

        public async Task<ToDoItem?> Get(Guid id, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return null;
            }

            ToDoItem? item = null;

            await Task.Run(() =>
            {
                var itemFiles = Directory.EnumerateFiles(_repositoryDir);
                foreach (var itemFile in itemFiles)
                {
                    string itemJson = File.ReadAllText(itemFile);
                    ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                    if (toDoItem?.Id == id)
                    {
                        item = toDoItem;
                        break;
                    }
                }
            });

            return item;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return [];
            }

            List<ToDoItem> items = new();

            await Task.Run(() =>
            {
                var itemFiles = Directory.EnumerateFiles(_repositoryDir);
                foreach (var itemFile in itemFiles)
                {
                    string itemJson = File.ReadAllText(itemFile);
                    ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                    if (toDoItem?.UserId == userId && toDoItem.State == ToDoItemState.Active)
                    {
                        items.Add(toDoItem);
                    }
                }
            });

            return items;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return [];
            }

            List<ToDoItem> items = new();

            await Task.Run(() =>
            {
                _semaphore.Wait();
                try
                {
                    var itemFiles = Directory.EnumerateFiles(_repositoryDir);
                    foreach (var itemFile in itemFiles)
                    {
                        string itemJson = File.ReadAllText(itemFile);

                        ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                        if (toDoItem?.UserId == userId)
                        {
                            items.Add(toDoItem);
                        }
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            return items;
        }

        public async Task Update(ToDoItem item, CancellationToken cancelToken)
        {
            await Add(item, cancelToken);
        }

        private string GetFileName(Guid id)
        {
            return Path.Combine(_repositoryDir, id.GetHashCode().ToString() + ".json");
        }
    }
}
