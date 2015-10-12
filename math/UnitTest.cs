using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathEvaluation;
using System.Diagnostics;

namespace UTest
{
    class MathEvaluationTest
    {
        class Test
        {
            public static void Main()
            {
                Debug.Assert("1+2+3+4+5+6+7+8+9".Evaulate() == 45);
                Debug.Assert("1-6+8-4-3-3--2".Evaulate() == -5);

                Debug.Assert(aprox("3*7*23*1.1123".Evaulate(), 537.2409));
                Debug.Assert(aprox("23/56+12/7-12/-3".Evaulate(), 6.125));

                Debug.Assert("-1+123^3*2".Evaulate() == 3721733);

                Debug.Assert("63*9+123325%3".Evaulate() == 568);
                Debug.Assert("10*4!/3".Evaulate() == 80);

                Console.ReadKey();
            }

            public static bool aprox(double a, double b, double rem = 0.00001)
            {
                return (Math.Abs(a - b) < rem);
            }

        }
    }
}
