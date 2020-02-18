using System;
using System.Threading.Tasks;
using Elsa;
using Elsa.Activities.Console.Activities;
using Elsa.Activities.Console.Extensions;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Services;
using Microsoft.Extensions.DependencyInjection;

namespace demo3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddElsa()
                .AddConsoleActivities()
                .BuildServiceProvider();

            //Define a workflow as data so we can store it in( file ,database,)
            var workflowDefinition = new WorkflowDefinitionVersion
            {
                Activities = new[]
                {
                    new ActivityDefinition<WriteLine>("activity-1",new { TextExpression = new LiteralExpression("Hello World!")}),
                    new ActivityDefinition<WriteLine>("activity-2", new { TextExpression = new LiteralExpression("Goodbye cruel world...")})
                },
                Connections = new[]
                {
                    new ConnectionDefinition("activity-1","activity-2",OutcomeNames.Done)
                }

            };

            var invoker = services.GetService<IWorkflowInvoker>();
            await invoker.StartAsync(workflowDefinition);
            Console.ReadLine();
            

        
        }
    }
}
