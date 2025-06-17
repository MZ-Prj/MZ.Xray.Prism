namespace MZ.ReceiveTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var receiver = new SocketReceiver();
            Console.WriteLine("Start TCP Receive");
            await receiver.StartListening(5887);

            while (receiver.IsConnected)
            {
                try
                {
                    var fileModel = await receiver.ReceiveFileAsync();
                    receiver.SaveImage(fileModel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    break;
                }
            }

            Console.WriteLine("Exit");
            Console.ReadKey();
        }
    }
}
