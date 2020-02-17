using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoActivities
{
    public class HelloWorld : Activity
    {

        protected override ActivityExecutionResult OnExecute(WorkflowExecutionContext context)
        {
            Console.WriteLine("Hello world!");
            return Done();
        }
    }
}
