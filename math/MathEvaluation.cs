using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace math
{
    /// <summary>
    ///  Delegate for mathematical functions
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public delegate double MathFunction(List<double> values);
    
    /// <summary>
    ///  Evaluates Mathematical Expressions
    /// </summary>
    public class MathEval
    {
        public List<string> History { get; set; } = new List<string>();

        public MathFunctionLibrary MathFunctions { get; set; } = new MathFunctionLibrary();
        public MathOperatorLibrary MathOperators { get; set; } = new MathOperatorLibrary();

        private bool _evaluated { get; set; }

        private string _expression { get; set; } = "";

        private double _result { get; set; }

        private readonly Dictionary<string, string> _replacements = new Dictionary<string, string>();

        /// <summary>
        /// Returns the result of evaluating the stored expression
        /// </summary>
        public double Result
        {
            get
            {
                if (!_evaluated)
                    Evaluate();
                return _result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MathEval()
        {
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="expr">Mathematical expression to use</param>
        public MathEval(string expr)
        {
            _expression = expr;
        }

        /// <summary>
        ///  Sets the expression the engine will evaluate 
        /// </summary>
        /// <param name="expr">Mathematical expression to use</param>
        public void SetExpression(string expr)
        {
            _expression = expr;
            _evaluated = false;
        }

        /// <summary>
        ///  Evaluates the given expression and returns the final value
        /// </summary>
        /// <param name="expr">Mathematical expression to use</param>
        /// <returns></returns>
        public double EvaluateExpression(string expr)
        {
            SetExpression(expr);
            return Evaluate();
        }

        /// <summary>
        ///  Evaluates the previously stored expression
        /// </summary>
        /// <returns></returns>
        public double Evaluate()
        {
            History.Add($"Input:   {_expression}");

            var expr = GetReplacedString(_expression);

            History.Add($"Replace: {_expression}");

            var parser = new MathParser(MathOperators, MathFunctions);

            var tokens = parser.GetTokens(expr);

            if (tokens.Count == 0) throw new MathEvaluationException("No valid input given");

            var rpn = parser.ShuntingYardAlgorithm(tokens);
            var val = parser.PostfixAlgorithm(rpn, out var history);

            foreach (var line in history)
            {
                History.Add(line);
            }

            History.Add($"{"Result",-10}{"",-10}{val,-20}\n");

            _result = val;

            return val;
        }

        public void SetArguments(IEnumerable<string> args)
        {
            var arguments = args.ToList();

            var k = arguments.Count;

            for (var i = k; i > 0; i--)
                AddReplacement($"{i}", arguments[i - 1]);
            for (var i = 1; i <= k; i++)
                AddReplacement($"-{i}", arguments[k - 1]);
        }

        public void AddReplacement(string key, object value)
        {
            _replacements[key] = value.ToString();
        }

        private string GetReplacedString(string expr)
        {
            var sortedList = from pair in _replacements
                             orderby pair.Key.Length descending
                             select pair;

            foreach (var item in sortedList)
                expr = expr.Replace("$" + item.Key, item.Value);

            return expr;
        }
    }
}