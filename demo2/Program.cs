using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Elsa.Services;
using Elsa.Activities.Console.Extensions;
using Elsa.Activities.Console.Activities;
using Elsa.Expressions;

namespace demo2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddElsa()
                .AddConsoleActivities()
                .BuildServiceProvider();
            //定义工作流程
            var workflowBuilderFactory = services.GetRequiredService<Func<IWorkflowBuilder>>();

            var workBuilder = workflowBuilderFactory();
            var workFlowDefinition = workBuilder
                .StartWith<WriteLine>(x => x.TextExpression = new LiteralExpression("Start on"))
                .Then<WriteLine>(x => x.TextExpression = new LiteralExpression("End"))
                .Build();
            //开始流程
            var invoker = services.GetService<IWorkflowInvoker>();
            await invoker.StartAsync(workFlowDefinition);
            Console.ReadLine();
        }
    }
}
