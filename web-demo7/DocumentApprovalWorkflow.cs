using Elsa;
using Elsa.Activities;
using Elsa.Activities.ControlFlow.Activities;
using Elsa.Activities.Email.Activities;
using Elsa.Activities.Http.Activities;
using Elsa.Activities.Timers.Activities;
using Elsa.Activities.Workflows.Activities;
using Elsa.Expressions;
using Elsa.Scripting.JavaScript;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace web_demo7
{
    public class DocumentApprovalWorkflow : IWorkflow
    {


        public void Build(IWorkflowBuilder builder)
        {
            builder.StartWith<ReceiveHttpRequest>(x =>
            {
                x.Method = HttpMethod.Post.Method;
                x.Path = new Uri("/document", UriKind.Relative);
                x.ReadContent = true;
            }).WithName("WaitForDocument") //导出的数据前缀

            .Then<SetVariable>(x =>
            {
                //获取上一步导出的内容 复制给全局变量 Document
                x.VariableName = "Document";
                x.ValueExpression = new JavaScriptExpression<object>("WaitForDocument.Content");
            })
            //发送邮件
            .Then<SendEmail>(x =>
            {
                //从全局Document 中读取接受 接受邮件人员信息，并发送 approval 邮件
                x.From = new LiteralExpression("approval@acme.com");
                x.To = new JavaScriptExpression<string>("Document.Author.Email");
                x.Subject =
                    new JavaScriptExpression<string>("`Document received from ${Document.Author.Name}`");
                x.Body = new JavaScriptExpression<string>(
                    "`Document from ${Document.Author.Name} received for review. " +
                    "<a href=\"${signalUrl('Approve')}\">Approve</a> or <a href=\"${signalUrl('Reject')}\">Reject</a>`"
                );
            })
            .Then<WriteHttpResponse>(x =>
            {
                //返回请求消息
                x.Content = new LiteralExpression("<h1>Request for Approval Sent</h1><p>Your document has been received and will be reviewed shortly.</p>");
                x.ContentType = "text/html";
                x.StatusCode = HttpStatusCode.OK;
                x.ResponseHeaders = new LiteralExpression("X-Powered-By=Elsa Workflows");
            })
            .Then<SetVariable>(x =>
            {
                //设置全局 认证信息 为false
                x.VariableName = "Approved";
                x.ValueExpression = new LiteralExpression<bool>("false");
            })
            // 构建三个分支选择，提供给用户选择，并一直等待用户确认
            .Then<Fork>(x =>
                {
                    x.Branches = new[] { "Approve", "Reject", "Remind" };
                },
                fork =>
                {
                    //确认分支，收到Approve Signal 后 继续执行 “Join”流程
                    fork
                      .When("Approve")
                      .Then<Signaled>(x => x.Signal = new LiteralExpression("Approve"))
                      .Then("Join");
                    //拒绝分支，收到Approve Signal 后 继续执行 “Join”流程
                    fork.When("Reject")
                           .Then<Signaled>(x => x.Signal = new LiteralExpression("Reject"))
                           .Then("Join");

                    //收到消息后，Timer模块进行每10秒进行一次消息提醒，直到用户 进行了选择
                    fork.When("Remind")
                    .Then<TimerEvent>(x => x.TimeoutExpression = new LiteralExpression<TimeSpan>("00:00:10"))
                    .WithName("RemindTimer")
                    .Then<IfElse>(
                        // IfElse 条件
                        x => x.ConditionExpression = new JavaScriptExpression<bool>("!!Approved"),
                        ifElse =>
                        {
                            ifElse.When(OutcomeNames.False)
                            .Then<SendEmail>(x =>
                            {
                                x.From = new LiteralExpression("reminder@acme.com");
                                x.To = new LiteralExpression("approval@acme.com");
                                x.Subject =
                                    new JavaScriptExpression<string>(
                                        "`${Document.Author.Name} is awaiting for your review!`"
                                    );
                                x.Body = new JavaScriptExpression<string>(
                                    "`Don't forget to review document ${Document.Id}.<br/>" +
                                    "<a href=\"${signalUrl('Approve')}\">Approve</a> or <a href=\"${signalUrl('Reject')}\">Reject</a>`"
                                );
                            }).Then("RemindTimer");
                        }

                    )
                    //等待用户进行选择
                    .Then<Join>(x => x.Mode = Join.JoinMode.WaitAny).WithName("Join")
                    //将用户确认结果 保存到 Approved
                    .Then<SetVariable>(x =>
                    {
                        x.VariableName = "Approved";
                        x.ValueExpression = new JavaScriptExpression<object>("input('Signal') === 'Approve'");
                    })
                    //判断用户确认结果True or False
                    .Then<IfElse>(
                        x => x.ConditionExpression = new JavaScriptExpression<bool>("!!Approved"),
                    ifElse =>
                    {
                        //如果为真
                        ifElse
                            .When(OutcomeNames.True)
                            .Then<SendEmail>(
                                x =>
                                {
                                    x.From = new LiteralExpression("approval@acme.com");
                                    x.To = new JavaScriptExpression<string>("Document.Author.Email");
                                    x.Subject =
                                        new JavaScriptExpression<string>("`Document ${Document.Id} approved!`");
                                    x.Body = new JavaScriptExpression<string>(
                                        "`Great job ${Document.Author.Name}, that document is perfect! Keep it up.`"
                                    );
                                }
                            );

                        //如果为假
                        ifElse
                            .When(OutcomeNames.False)
                            .Then<SendEmail>(
                                x =>
                                {
                                    x.From = new LiteralExpression("approval@acme.com");
                                    x.To = new JavaScriptExpression<string>("Document.Author.Email");
                                    x.Subject =
                                        new JavaScriptExpression<string>("`Document ${Document.Id} rejected`");
                                    x.Body = new JavaScriptExpression<string>(
                                        "`Sorry ${Document.Author.Name}, that document isn't good enough. Please try again.`"
                                    );
                                }
                            );
                    }
                );

                }
            );





        }
    }
}
