using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysisServer
{
    class Program
    {
        private const int Port = 5000;

        static async Task Main(string[] args)
        {
            var server = new FileAnalysisServer(Port);
            await server.Start();
        }
    }

    public class FileAnalysisServer
    {
        private readonly int _port;

        public FileAnalysisServer(int port)
        {
            _port = port;
        }

        public async Task Start()
        {
            TcpListener server = new TcpListener(IPAddress.Any, _port);
            server.Start();
            Console.WriteLine("Сервер запущен...");

            while (true)
            {
                var client = await server.AcceptTcpClientAsync();
                await Task.Run(() => HandleClient(client));
            }
        }

        private static async Task HandleClient(TcpClient client)
        {
            using (client)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                string fileName = $"file_{Guid.NewGuid()}.txt";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                await File.WriteAllBytesAsync(filePath, buffer[..bytesRead]);

                var analyzer = new FileAnalyzer();
                var analysisResult = analyzer.AnalyzeFile(filePath);

                await File.AppendAllTextAsync("analysis_result.txt", analysisResult + Environment.NewLine);
                byte[] response = Encoding.UTF8.GetBytes(analysisResult);
                await stream.WriteAsync(response, 0, response.Length);
            }
        }
    }

    public class FileAnalyzer
    {
        public string AnalyzeFile(string filePath)
        {
            var content = File.ReadAllText(filePath);
            int lineCount = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length;
            int wordCount = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            int charCount = content.Length;

            return $"Имя файла: {Path.GetFileName(filePath)}\nСтрок: {lineCount}, Слов: {wordCount}, Символов: {charCount}";
        }
    }
}


