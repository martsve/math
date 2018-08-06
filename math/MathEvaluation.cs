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

    public delegate double ObjectFunction(List<object> values);
    
    /// <summary>
    ///  Evaluates Mathematical Expressions
    /// </summary>
    public class MathEval
    {
        public bool Debug { get; set; }
        public List<string> History { get; set; } = new List<string>();

        private bool _evaluated = false;
        private string _expression = "";

        private double _result;
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
        /// <param name="debug"></param>
        public MathEval(bool debug = false)
        {
            Debug = debug;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="expr">Mathematical expression to use</param>
        /// <param name="debug"></param>
        public MathEval(string expr, bool debug = false)
        {
            Debug = debug;
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
            if (Debug) Console.WriteLine("Input:   {0}", _expression);

            var expr = GetReplacedString(_expression);

            if (Debug) Console.WriteLine("Replace: {0}", _expression);

            var tokens = GetTokens(expr);

            if (tokens.Count == 0) throw new MathEvaluationException("No valid input given");

            var parser = new MathParser(_mathOperators, _mathFunctions);
            var rpn = parser.ShuntingYardAlgorithm(tokens);
            var val = parser.PostfixAlgorithm(rpn, out var history);

            if (Debug)
            {
                foreach (var line in history)
                {
                    Console.WriteLine(line);
                }

                Console.WriteLine("{0,-10}{1,-10}{2,-20}\n", "Result", "", val);
            }

            _result = val;

            return val;
        }

        /// <summary>
        ///  Adds a math function to the evaluation engine.
        /// </summary>
        /// <param name="op">Function Name. Case insensitiv</param>
        /// <param name="func">Function that returns a double and take a List&lt;double&gt; as input</param>
        public void AddFunction(string op, MathFunction func)
        {
            _mathFunctions.Add(op.ToLower(), func);
        }

        /// <summary>
        ///  Removes a math function from the evaluation engine.
        /// </summary>
        /// <param name="op">Function Name. Case insensitiv</param>
        public void RemoveFunction(string op)
        {
            _mathFunctions.Functions.Remove(op.ToLower());
        }


        /// <summary>
        ///  Adds a math operator to the evaluation engine.
        /// </summary>
        /// <param name="op">Operator Name. Case insensitiv</param>
        /// <param name="func">Function that returns a double and take a List&lt;double&gt; as input</param>
        /// <param name="func">Precedence (2 = +-, 3 = */%, 4 = ^)</param>
        /// <param name="func">Associative, Left or Right</param>
        public void AddOperator(char op, MathFunction func, int parameters = 2, int precedence = 2, Associative associative = Associative.Left)
        {
            if (TokenTypes().ContainsKey(op))
            {
                throw new MathOperatorException("Unable to add operators named '(', ')' or ','");
            }
            _mathOperators.AddOperator(char.ToLower(op), func, parameters, precedence, associative);
        }

        /// <summary>
        ///  Removes a math operator from the evaluation engine.
        /// </summary>
        /// <param name="op">Operator Name. Case insensitiv</param>
        public void RemoveOperator(char op)
        {
            _mathOperators.Operators.Remove(char.ToLower(op));
        }
        
        public List<string> GetOperators()
        {
            return (from op in _mathOperators.Operators
                   select op.Key.ToString()).ToList();
        }

        public List<string> GetFunctions()
        {
            return (from func in _mathFunctions.Functions
                    select func.Key).ToList();
        }

        private readonly Dictionary<string, string> _replacements = new Dictionary<string, string>();

        private List<string> _arguments = new List<string>();

        public void SetArguments(IEnumerable<string> args)
        {
            _arguments = args.ToList();
        }
        
        public void AddArgument(double arg)
        {
            _arguments.Add(arg.ToString());
        }

        public void AddArgument(string arg)
        {
            _arguments.Add(arg);
        }

        public void ClearArguments(string arg)
        {
            _arguments = new List<string>();
        }

        public void AddReplacement(Dictionary<string, double> replacements)
        {
            foreach (var pair in replacements)
                AddReplacement(pair.Key, pair.Value);
        }

        public void AddReplacement(string key, double value)
        {
            AddReplacement(key, value.ToString());
        }

        public void AddReplacement(Dictionary<string, string> replacements)
        {
            foreach (var pair in replacements)
                AddReplacement(pair.Key, pair.Value);
        }

        public void AddReplacement(string key, string value)
        {
             _replacements[key] = value;
        }

        private string GetReplacedString(string expr)
        {
            var sortedList = from pair in _replacements
                             orderby pair.Key.Length descending
                             select pair;

            foreach (var item in sortedList)
                expr = expr.Replace("$" + item.Key, item.Value);

            var k = _arguments.Count;

            for (var i = k; i > 0; i--)
                expr = expr.Replace("$" + i, _arguments[i - 1]);
            for (var i = 1; i <= k; i++)
                expr = expr.Replace("$-" + i, _arguments[k - i]);

            return expr;
        }

        private readonly MathFunctionLibrary _mathFunctions = new MathFunctionLibrary();
        private readonly MathOperatorLibrary _mathOperators = new MathOperatorLibrary();

        /// <summary>
        ///  Function to insert two parathesis' into a math equation to turn -func(..) into (-1*func(..))
        /// </summary>
        private string InsertBracket(string outer, string p1, string p2)
        {
            var find = p1 + "-" + p2;
            var pos = outer.IndexOf(find);
            var replace = p1 + "(-1*" + p2;
            outer = outer.Remove(pos, find.Length).Insert(pos, replace);
            var from = pos + replace.Length - 1;
            var lvl = 0;
            for (var i = from; i < outer.Length; i++)
            {
                if (outer[i] == '(') lvl++;
                if (outer[i] == ')')
                {
                    lvl--;
                    if (lvl == 0)
                    {
                        outer = outer.Insert(i + 1, ")");
                        break;
                    }
                }
            }
            return outer;
        }


        private Dictionary<char, TokenType> TokenTypes()
        {
            var specials = new Dictionary<char, TokenType>
            {
                {'(', TokenType.Open},
                {')', TokenType.Close},
                {',', TokenType.ListSep}
            };
            return specials;
        }

        private List<Token> GetTokens(string expr)
        {
            var tokens = new List<Token>();
            
            expr = Regex.Replace(expr, @"^-([a-zA-Z(])", "-1*$1");
            expr = Regex.Replace(expr, @"\(-([a-zA-Z(])", "(-1*$1");
            expr = Regex.Replace(expr, @"\,-([a-zA-Z(])", ",-1*$1");

            // fix for oper
            var reg = new Regex(@"([^0-9)])-([a-zA-Z]+\(|\()");
            while (reg.IsMatch(expr))
            {
                var cm = reg.Match(expr);
                if (cm.Success)
                {
                    expr = InsertBracket(expr, cm.Groups[1].Value, cm.Groups[2].Value);
                }
            }

            if (Debug) Console.WriteLine("Fixed:   {0}", expr);

            var specials = TokenTypes();

            while (expr.Length > 0)
            {
                var c = expr[0];

                // custom operators with 1 parameter might fuck this up when followed by + or -
                var doubleOp = true;
                if (tokens.Count > 0 && tokens[tokens.Count - 1].Type == TokenType.Operator)
                    if (_mathOperators.Operators.ContainsKey(tokens[tokens.Count - 1].Value[0]))
                        if (_mathOperators.Operators[tokens[tokens.Count - 1].Value[0]].Parameters == 1)
                            doubleOp = false;

                Match m;
                if (tokens.Count == 0 ||
                    (tokens[tokens.Count - 1].Type == TokenType.Operator && doubleOp) ||
                    tokens[tokens.Count - 1].Type == TokenType.ListSep ||
                    tokens[tokens.Count - 1].Type == TokenType.Open)
                {

                    m = Regex.Match(expr, @"^[-+]{0,1}([0-9.]+([eE][\-\+]{0,1}[0-9.]+|))");
                    if (m.Success)
                    {
                        var func = m.Groups[0].Value;
                        tokens.Add(new Token(func, TokenType.Number));
                        expr = expr.Substring(func.Length);
                        continue;
                    }
                }

                if (specials.ContainsKey(c))
                {
                    tokens.Add(new Token(c.ToString(), specials[c]));
                    expr = expr.Substring(1);
                    continue;
                }

                if (_mathOperators.Operators.ContainsKey(c))
                {
                    tokens.Add(new Token(c.ToString(), TokenType.Operator));
                    expr = expr.Substring(1);
                    continue;
                }

                m = Regex.Match(expr, @"^([a-zA-Z]+)");
                if (m.Success)
                {
                    var func = m.Groups[0].Value;
                    tokens.Add(new Token(func, TokenType.Function));
                    expr = expr.Substring(func.Length);
                    continue;
                }

                m = Regex.Match(expr, @"^([0-9.]+([eE][\-\+]{0,1}[0-9.]+|))");
                if (m.Success)
                {
                    var func = m.Groups[0].Value;
                    tokens.Add(new Token(func, TokenType.Number));
                    expr = expr.Substring(func.Length);
                    continue;
                }

                if (expr[0] == ' ')
                    expr = expr.Substring(1);
                else 
                    throw new MathOperatorException("Invalid at position 1: " + expr);


            }

            return tokens;
        }
    }
}