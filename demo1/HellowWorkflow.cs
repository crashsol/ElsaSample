using DemoActivities;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace demo1
{
   public class HellowWorkflow: IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder
                .StartWith<HelloWorld>()
                .Then<GoodByeWorld>();
        }
    }
}
