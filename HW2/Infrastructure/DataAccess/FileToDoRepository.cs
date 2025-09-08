using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using Core.DataAccess;
using Core.Entity;
using HW2.Infrastructure;

namespace Infrastructure.DataAccess
{
    internal class FileToDoRepository : IToDoRepository
    {
        private string _repositoryDir = "items";
        private UserItemIndex _userItemIndexFile = new();

        public FileToDoRepository(string repositoryDir)
        {
            _repositoryDir = repositoryDir;

            FillIndexFile();
        }

        public async Task Add(ToDoItem item, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            await Task.Run(() =>
            {
                FileInfo itemFile = new(GetFileName(item));

                if (!Directory.Exists(itemFile.DirectoryName))
                {
                    Directory.CreateDirectory(itemFile.DirectoryName);
                }

                string json = JsonSerializer.Serialize(item);

                StreamWriter wstream = File.CreateText(itemFile.FullName);
                wstream.Write(json);
                wstream.Flush();
                wstream.Close();

                _userItemIndexFile.Add(item.UserId, item.Id);
            });
        }

        public async Task<int> CountActive(Guid userId, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            string userDirectory = Path.Combine(_repositoryDir, userId.GetHashCode().ToString());
            if (!Directory.Exists(userDirectory))
            {
                return 0;
            }

            int count = 0;

            await Task.Run(() =>
            {
                return Directory.EnumerateFiles(userDirectory).Count();
            });

            return count;
        }

        public async Task Delete(Guid id, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_repositoryDir))
            {
                return;
            }

            string itemFile = id.GetHashCode().ToString() + ".json";
            int userHashCode = -1;

            await Task.Run( () =>
            {
                var userDirs = Directory.GetDirectories(_repositoryDir);
                foreach (var userDir in userDirs)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    FileInfo fileInfo = new(Path.Combine(userDir, itemFile));

                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();

                        DirectoryInfo dirInfo = new(userDir);

                        if (!int.TryParse(dirInfo.Name, out userHashCode))
                            return;

                        return;
                    }
                }
            });

            if (userHashCode > 0)
            {
                await _userItemIndexFile.Delete(userHashCode, id.GetHashCode(), cancelToken);
            }
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            string userDirectory = Path.Combine(_repositoryDir, userId.GetHashCode().ToString());
            if (!Directory.Exists(userDirectory))
            {
                return false;
            }

            bool isNameExists = false;

            await Task.Run(() =>
            {
                var itemFiles = Directory.EnumerateFiles(userDirectory);
                foreach (var itemFile in itemFiles)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

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

            string userDirectory = Path.Combine(_repositoryDir, userId.GetHashCode().ToString());
            if (!Directory.Exists(userDirectory))
            {
                return [];
            }

            List<ToDoItem> items = new();

            await Task.Run(() =>
            {
                var itemFiles = Directory.EnumerateFiles(userDirectory);
                foreach (var itemFile in itemFiles)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    string itemJson = File.ReadAllText(itemFile);
                    ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                    if ((toDoItem != null) && predicate(toDoItem))
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

            string itemFileName = id.GetHashCode().ToString() + ".json";
            ToDoItem? resItem = null;
            await Task.Run(() =>
            {
                var userDirs = Directory.GetDirectories(_repositoryDir);
                foreach (var userDir in userDirs)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var itemFiles = Directory.GetFiles(userDir);
                    foreach (var itemFile in itemFiles)
                    {
                        if (cancelToken.IsCancellationRequested)
                        {
                            return;
                        }

                        FileInfo fileInfo = new(itemFile);
                        if (fileInfo.Name == itemFileName)
                        {
                            string itemJson = File.ReadAllText(itemFile);
                            ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                            if (toDoItem?.Id == id)
                            {
                                resItem = toDoItem;
                                break;
                            }
                        }
                    }

                    if (resItem != null)
                    {
                        break;
                    }
                }
            });

            return resItem;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(_repositoryDir))
            {
                throw new DirectoryNotFoundException();
            }

            string userDirectory = Path.Combine(_repositoryDir, userId.GetHashCode().ToString());
            if (!Directory.Exists(userDirectory))
            {
                return [];
            }

            List<ToDoItem> items = new();

            await Task.Run(() =>
            {
                var itemFiles = Directory.EnumerateFiles(userDirectory);
                foreach (var itemFile in itemFiles)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    string itemJson = File.ReadAllText(itemFile);
                    ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                    if ((toDoItem != null) && (toDoItem.State == ToDoItemState.Active))
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

            string userDirectory = Path.Combine(_repositoryDir, userId.GetHashCode().ToString());
            if (!Directory.Exists(userDirectory))
            {
                return [];
            }

            List<ToDoItem> items = new();

            await Task.Run(() =>
            {
                var itemFiles = Directory.EnumerateFiles(userDirectory);
                foreach (var itemFile in itemFiles)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    string itemJson = File.ReadAllText(itemFile);
                    ToDoItem? toDoItem = JsonSerializer.Deserialize<ToDoItem>(itemJson);

                    if (toDoItem != null)
                    {
                        items.Add(toDoItem);
                    }
                }
            });

            return items;
        }

        public async Task Update(ToDoItem item, CancellationToken cancelToken)
        {
            await Add(item, cancelToken);
        }

        private void FillIndexFile()
        {
            if (_userItemIndexFile.IsExist())
                return;

            var userDirs = Directory.GetDirectories(_repositoryDir);
            if (userDirs.Length == 0)
            {
                return;
            }

            foreach (var userDir in userDirs)
            {
                DirectoryInfo dirInfo = new(userDir);

                int userHashCode = -1;
                if (!int.TryParse(dirInfo.Name, out userHashCode))
                    continue;

                var itemFiles = Directory.GetFiles(userDir);
                foreach (var itemFile in itemFiles)
                {
                    FileInfo itemFileInfo = new(itemFile);

                    int itemHashCode = -1;
                    if (!int.TryParse(Path.GetFileNameWithoutExtension(itemFileInfo.Name), out itemHashCode))
                        continue;

                    _userItemIndexFile.Add(userHashCode, itemHashCode);
                }
            }
        }

        private string GetFileName(ToDoItem item)
        {
            return Path.Combine(_repositoryDir, item.UserId.GetHashCode().ToString(), item.Id.GetHashCode().ToString() + ".json");
        }
    }
}
