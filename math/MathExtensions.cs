using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace math
{
    public static class MathExtensions
    {
        /// <summary>
        ///  Math Expression Evaluator
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns></returns>
        public static double Calculate(this string expr)
        {
            return new MathEval(expr).Evaluate();
        }
    }
}
