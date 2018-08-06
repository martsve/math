using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace math
{

    /// <summary>
    ///  Libraray of default functions for Mathematical Expressions
    /// </summary>
    public class MathOperatorLibrary
    {

        public class Operator
        {
            public char Name;
            public MathFunction Function;
            public int Precevendce;
            public Associative Assoc;
            public int Parameters;
            public Operator(char name, MathFunction func, int param, int precedence, Associative assoc)
            {
                Name = name;
                Function = func;
                Precevendce = precedence;
                Assoc = assoc;
                Parameters = param;
            }
        }

        public Dictionary<char, Operator> Operators = new Dictionary<char, Operator>();

        public void AddOperator(char name, MathFunction func, int parameters = 2, int precedence = 2, Associative assoc = Associative.Left)
        {
            var op = new Operator(name, func, parameters, precedence, assoc);
            Operators.Add(name, op);
        }

        public MathOperatorLibrary()
        {
            LoadStandardOperators();
        }

        private void LoadStandardOperators()
        {
            AddOperator('+', (x) => x[0] + x[1], 2, 2);
            AddOperator('-', (x) => x[0] - x[1], 2, 2);
            AddOperator('*', (x) => x[0] * x[1], 2, 3);
            AddOperator('/', (x) => x[0] / x[1], 2, 3);
            AddOperator('%', (x) => x[0] % x[1], 2, 3);
            AddOperator('^', (x) => Math.Pow(x[0], x[1]), 2, 4, Associative.Left);
            AddOperator('!', (x) => Fact(x[0]), 1, 10);

            AddOperator('>', (x) => x[0] > x[1] ? 1 : 0, 2, 1);
            AddOperator('<', (x) => x[0] < x[1] ? 1 : 0, 2, 1);
            AddOperator('=', (x) => x[0] == x[1] ? 1 : 0, 2, 1);

            AddOperator('&', (x) => x[0] != 0 && x[1] != 0 ? 1 : 0, 2, 1);
            AddOperator('|', (x) => x[0] != 0 || x[1] != 0 ? 1 : 0, 2, 1);
        }

        private double Fact(double y)
        {
            var val = Math.Floor(y);
            while (y > 1)
            {
                val = val * (y - 1);
                y--;
            }
            return val;
        }
    }
}
