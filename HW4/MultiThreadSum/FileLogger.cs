
namespace MultiThreadSum
{
    public class FileLogger : ILogger
    {
        private string _fileName = "intCalculator.log";

        public FileLogger()
        {
            File.Delete(_fileName);
        }

        public async Task Log(string message)
        {
            using (var stream = File.AppendText(_fileName))
            {
                stream.WriteLine(message);
                await stream.FlushAsync();
                stream.Close();
            }
        }
    }
}
