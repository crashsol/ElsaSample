using Elsa.Expressions;
using Elsa.Extensions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace demo4.Activities
{
    public abstract class ArithmeticOperation : Activity
    {
        private readonly IWorkflowExpressionEvaluator evaluator;
        public ArithmeticOperation(IWorkflowExpressionEvaluator evaluator)
        {
            this.evaluator = evaluator;
        }

        public WorkflowExpression<double[]> Values
        {
            get => GetState<WorkflowExpression<double[]>>();
            set => SetState(value);
        }


        public string ResultVariableName
        {
            get => GetState<string>();
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            var values = await evaluator.EvaluateAsync(Values, context, cancellationToken);
            var sum = Calculate(values);
            context.SetLastResult(sum);
            context.CurrentScope.SetVariable(ResultVariableName, sum);
            return Done();

        }

        protected abstract double Calculate(params double[] values);

    }
}
