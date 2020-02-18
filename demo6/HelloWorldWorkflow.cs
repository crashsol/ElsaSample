using Elsa.Activities.Http.Activities;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace demo6
{
    /// <summary>
    /// 定义一个请求Activity
    /// </summary>
    public class HelloWorldWorkflow : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder.StartWith<ReceiveHttpRequest>(
                activity => activity.Path = new Uri("/hello-world", UriKind.Relative)
                ).Then<WriteHttpResponse>(

                   activity =>
                   {
                       activity.Content = new LiteralExpression("<h1>Elsa say Hi</h1>");
                       activity.ContentType = "text/html";
                       activity.StatusCode = System.Net.HttpStatusCode.OK;
                       activity.ResponseHeaders = new LiteralExpression("X-Powered-By=Elsa Workflows");
                   }
                );
        }
    }
}
