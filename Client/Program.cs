using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
        using (TcpClient client = new TcpClient(ServerIp, ServerPort))
        {
            NetworkStream stream = client.GetStream();
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