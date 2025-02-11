namespace ConsoleApp
{
    interface IMyApp
    {
        Task Run();
    }
    public class MyApp : IMyApp
    {
        public Task Run()
        {
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("RRRRRRRRRRRRUNNNNNNN !!!!!!!!! ****NOT ASYNC***");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("done");

            throw new Exception("hihi");

            return Task.CompletedTask;
        }
    }

    public class MyAppAsync : IMyApp
    {
        public async Task Run()
        {
            await Task.Delay(1000);
            Console.WriteLine("RRRRRRRRRRRRUNNNNNNN !!!!!!!!! ****ASYNC***");
            await Task.Delay(1000);
            Console.WriteLine("done");

            throw new Exception("hihi");
        }
    }
}
