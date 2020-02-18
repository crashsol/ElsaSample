using System;
using System.Collections.Generic;
using System.Text;
using Elsa.Services;
using System.Linq;

namespace demo4.Activities
{
    public class Sum: ArithmeticOperation
    {
        public Sum(IWorkflowExpressionEvaluator evaluator) : base(evaluator)
        {
        }

        protected override double Calculate(params double[] values) => values.Sum();
    }
}
