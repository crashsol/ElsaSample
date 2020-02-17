using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using System;


namespace DemoActivities
{   
    public class GoodByeWorld: Activity
    {
        protected override ActivityExecutionResult OnExecute(WorkflowExecutionContext context)
        {
            Console.WriteLine("Goodbye cruel world...");
            return Done();
        }
    }
}
