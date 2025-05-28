using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysisClient
{
    class Program
    {
        private const string ServerIp = "127.0.0.1";
        private const int ServerPort = 5000;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Введите путь к текстовому файлу:");
            string filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден.");
                return;
            }

            var client = new FileAnalysisClient(ServerIp, ServerPort);
            await client.SendFileAndGetAnalysis(filePath);
        }
    }

    public class FileAnalysisClient
    {
        private readonly string _serverIp;
        private readonly int _serverPort;

        public FileAnalysisClient(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
        }

        public async Task SendFileAndGetAnalysis(string filePath)
        {
            using (TcpClient tcpClient = new TcpClient(_serverIp, _serverPort))
            {
                NetworkStream stream = tcpClient.GetStream();
                byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                await stream.WriteAsync(fileBytes, 0, fileBytes.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("Результаты анализа:");
                Console.WriteLine(response);
            }
        }
    }
}
