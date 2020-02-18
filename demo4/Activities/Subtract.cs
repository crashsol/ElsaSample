using Elsa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace demo4.Activities
{
    public class Subtract : ArithmeticOperation
    {
        public Subtract(IWorkflowExpressionEvaluator evaluator) : base(evaluator)
        {
        }

        protected override double Calculate(params double[] values)
        {
            return values.Aggregate((left, right) => left - right);
        }
    }
}
