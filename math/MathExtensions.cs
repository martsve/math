using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace math
{public static class MathExtensions
    {
        /// <summary>
        ///  Math Expression Evaluator
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static double Evaulate(this string expr)
        {
            var engine = new MathEval(expr);
            return engine.Evaluate();
        }

        /// <summary>
        /// Math Expression Evaluator
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static double Evaulate(this string expr, Dictionary<string, double> replacements)
        {
            var engine = new MathEval(expr);
            engine.AddReplacement(replacements);
            return engine.Evaluate();
        }
    }
}
