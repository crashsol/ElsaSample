using Elsa.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace demo4.Activities
{
    public class Multiply : ArithmeticOperation
    {
        public Multiply(IWorkflowExpressionEvaluator evaluator) : base(evaluator)
        {
        }

        protected override double Calculate(params double[] values)
        {
            return values.Aggregate((left, right) => left * right);
        }
    }
}
