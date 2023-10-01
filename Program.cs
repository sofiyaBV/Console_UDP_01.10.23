// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace UdpConsoleApp
{
    class Program
    {
        static IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        static Dictionary<string, int> prices = new Dictionary<string, int>
        {
            { "процесор", 5000 },
            { "відеокарта", 8000 },
            { "оперативна_пам'ять", 2000 },
            { "материнська_плата", 3500 },
            { "жорсткий_диск", 1500 }
        };

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Запускаємо метод отримання повідомлень в окремому потоці
            Task.Run(ReceiveMessageAsync);

            // Запускаємо ввод та відправку повідомлень
            await SendMessageAsync();

            // Очікуємо, поки користувач натисне Enter перед завершенням програми
            Console.ReadLine();
        }

        static async Task SendMessageAsync()
        {
            using UdpClient sender = new UdpClient();

            Console.WriteLine("Введіть запит (наприклад, 'процесор') або 'exit' для виходу:");
            while (true)
            {
                string message = Console.ReadLine();

                if (message == "exit")
                {
                    break;
                }

                string response;
                if (prices.ContainsKey(message))
                {
                    response = $"Ціна на {message}: {prices[message]} грн";
                }
                else
                {
                    response = $"Комплектуюча '{message}' не знайдена";
                }

                byte[] data = Encoding.UTF8.GetBytes(response);
                await sender.SendAsync(data, data.Length, new IPEndPoint(localAddress, 12345));
            }
        }

        static async Task ReceiveMessageAsync()
        {
            using UdpClient receiver = new UdpClient(12345);
            while (true)
            {
                var result = await receiver.ReceiveAsync();
                var message = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine($"Отримано: {message}");
            }
        }
    }
}
