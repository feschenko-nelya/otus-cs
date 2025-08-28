
using System.Text.Json;
using Telegram.Bot.Types;

namespace HW2.Infrastructure
{
    internal class UserItemIndex
    {
        private static string _fileName = "index.json";
        private FileInfo _file = new(_fileName);

        public UserItemIndex()
        { 
        }

        public void Add(Guid userId, Guid itemId)
        {
            Add(userId.GetHashCode(), itemId.GetHashCode());
        }

        public void Add(int userIdHashCode, int itemIdHashCode)
        {
            using (var streamW = _file.AppendText())
            {
                var jsonIndex = new { UserId = userIdHashCode, ItemId = itemIdHashCode };
                var jsonText = JsonSerializer.Serialize(jsonIndex);

                streamW.WriteLine(jsonText);
                streamW.Flush();
                streamW.Close();
            }
        }

        public async Task Delete(Guid userId, Guid itemId, CancellationToken cancelToken)
        {
            await Delete(userId.GetHashCode(), itemId.GetHashCode(), cancelToken);
        }

        public async Task Delete(int userIdHashCode, int itemIdHashCode, CancellationToken cancelToken)
        {
            if (!_file.Exists || (new FileInfo(_file.FullName).Length == 0))
                return;

            var shadowFile = GetShadowFile();
            if (shadowFile == null)
                return;

            await Task.Run(() =>
            {
                var indexStream = _file.OpenText();
                var shadowStream = shadowFile.AppendText();

                string? line = null;
                while ((line = indexStream.ReadLine()) != null)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        try
                        {
                            indexStream.Close();
                            shadowStream.Close();
                            shadowFile.Delete();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        return;
                    }
                    var jsonDoc = JsonDocument.Parse(line);
                    var jsonRoot = jsonDoc.RootElement;

                    if (userIdHashCode == jsonRoot.GetProperty("UserId").GetInt32()
                        && itemIdHashCode == jsonRoot.GetProperty("ItemId").GetInt32())
                    {
                        continue;
                    }

                    shadowStream.WriteLine(line);
                }

                indexStream.Close();

                shadowStream.Flush();
                shadowStream.Close();

                shadowFile.Replace(_file.FullName, null);
            });
        }

        public bool IsExist()
        {
            return _file.Exists;
        }

        private FileInfo? GetShadowFile()
        {
            var shadowFile = new FileInfo("_" + _fileName);

            if (shadowFile.Exists)
            {
                try
                {
                    shadowFile.Delete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            return shadowFile;
        }
    }
}
