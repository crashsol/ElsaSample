using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Elsa.Services;
using DemoActivities;

namespace demo1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = new ServiceCollection().AddElsa()
                .AddActivity<HelloWorld>()
                .AddActivity<GoodByeWorld>()
                .BuildServiceProvider();

            var invoker = service.GetService<IWorkflowInvoker>();
            await invoker.StartAsync<HellowWorkflow>();
            Console.ReadLine();


        }
    }
}
