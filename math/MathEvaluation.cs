using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MathEvaluation
{
    public static class MathExtensions
    {
        /// <summary>
        ///  Math Expression Evaluator
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static double Evaulate(this string expr)
        {
            MathEval engine = new MathEval(expr);
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
            MathEval engine = new MathEval(expr);
            engine.AddReplacement(replacements);
            return engine.Evaluate();
        }
    }

    /// <summary>
    ///  Delegate for mathematical functions
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    delegate double MathFunction(List<double> values);
    delegate double ObjectFunction(List<object> values);

  
    /// <summary>
    ///  Invalid operators in a Mathematical Expressions
    /// </summary>
    class MathOperatorException : Exception
    {
        public MathOperatorException()
        {
        }
        public MathOperatorException(string message)
            : base(message)
        {
        }
        public MathOperatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    ///  Invalid input in a Mathematical Expressions
    /// </summary>
    class MathEvaluationException : Exception
    {
        public MathEvaluationException()
        {
        }
        public MathEvaluationException(string message)
            : base(message)
        {
        }
        public MathEvaluationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Invalid Parenthesis usaged in a Mathematical Expressions
    /// </summary>
    class ParenthesisException : Exception
    {
        public ParenthesisException()
        {
        }
        public ParenthesisException(string message)
            : base(message)
        {
        }
        public ParenthesisException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    ///  Error thrown by a Math function with invalid input
    /// </summary>
    class MathOperationArgumentException : Exception
    {
        public MathOperationArgumentException()
        {
        }
        public MathOperationArgumentException(string message)
            : base(message)
        {
        }
        public MathOperationArgumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception because of non-existent functions in a Mathematical Expressions
    /// </summary>
    class MissingMathOperationException : Exception
    {
        public MissingMathOperationException()
        {
        }
        public MissingMathOperationException(string message)
            : base(message)
        {
        }
        public MissingMathOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    ///  Mathematical Expressions tokens
    /// </summary>
    class Token
    {
        public string Value { get; set; }
        public TokenType Type { get; set; }

        public Token(string value, TokenType type)
        {
            Value = value;
            Type = type;
        }
    }

    /// <summary>
    ///  Token types in Mathematical Expressions
    /// </summary>
    enum TokenType
    {
        Open,
        Close,
        ListSep,
        Operator,
        Number,
        Function,
        Args,
        String
    }

    enum Associative
    {
        LEFT,
        RIGHT
    }


    /// <summary>
    ///  Libraray of default functions for Mathematical Expressions
    /// </summary>
    class MathFunctionLibrary
    {

        public class Function
        {
            MathFunction mfunc;
            ObjectFunction ofunc;
            Type t;
            public Function(object o)
            {
                if (o is MathFunction)
                {
                    mfunc = (MathFunction)o;
                    t = typeof(MathFunction);
                }
                else
                {
                    ofunc = (ObjectFunction)o;
                    t = typeof(ObjectFunction);
                }
            }
            public double Invoke(List<object> inp)
            {
                if (t == typeof(MathFunction))
                {
                    return mfunc.Invoke(inp.Select(x=>(double)x).ToList());
                }
                else
                {
                    return ofunc.Invoke(inp);
                }
            }
        }

        public Dictionary<string, Function> functions = new Dictionary<string, Function>();

        public MathFunctionLibrary()
        {
            LoadStandardFunctions();
        }

        public void Add(string name, MathFunction func)
        {
            Function f = new Function(func);
            functions.Add(name, f);
        }
        public void Add(string name, ObjectFunction func)
        {
            Function f = new Function(func);
            functions.Add(name, f);
        }

        void LoadStandardFunctions()
        {
            Add("cos", (x) => Math.Cos(x[0]));
            Add("acos", (x) => Math.Acos(x[0]));
            Add("cosh", (x) => Math.Cosh(x[0]));

            Add("sin", (x) => Math.Sin(x[0]));
            Add("asin", (x) => Math.Asin(x[0]));
            Add("sinh", (x) => Math.Sinh(x[0]));

            Add("tan", (x) => Math.Tan(x[0]));
            Add("atan", (x) => Math.Atan(x[0]));
            Add("tanh", (x) => Math.Tanh(x[0]));

            Add("log", (x) => Math.Log10(x[0]));
            Add("ln", (x) => Math.Log(x[0]));
            Add("exp", (x) => Math.Exp(x[0]));

            Add("gamma", (x) => gamma(x[0]));
            Add("fact", (x) => fact(x[0]));

            Add("abs", (x) => Math.Abs(x[0]));
            Add("floor", (x) => Math.Floor(x[0]));
            Add("ceil", (x) => Math.Ceiling(x[0]));
            Add("sqrt", (x) => Math.Sqrt(x[0]));

            Add("round", (x) => Math.Round(x[0], (int)x[1]));

            Add("max", (x) => x.Max());
            Add("min", (x) => x.Max());
            Add("avrg", (x) => x.Average());
            Add("sum", (x) => x.Sum());
            Add("amax", new MathFunction(Amax));
            Add("amin", new MathFunction(Amin));
            Add("stdev", new MathFunction(StDev));
            Add("kurt", new MathFunction(Kurtosis));
            Add("skew", new MathFunction(Skew));

            Add("chr", (List<object> x) => (int)(x[0].ToString()[0]));
            Add("len", (List<object> x) => x[0].ToString().Length);
            Add("equal", (List<object> x) => x[0].ToString() == x[1].ToString() ? 1 : 0);
            Add("count", (List<object> x) => ((x[0].ToString().Length - x[0].ToString().Replace(x[1].ToString(),"").Length) / x[1].ToString().Length));
        }

        double Amax(List<double> values)
        {
            int maxElem = 0;
            for (int i = 0; i < values.Count(); i++)
                if (Math.Abs(values.ElementAt(i)) > Math.Abs(values.ElementAt(maxElem))) maxElem = i;
            return values.ElementAt(maxElem);
        }
        double Amin(List<double> values)
        {
            int minElem = 0;
            for (int i = 0; i < values.Count(); i++)
                if (Math.Abs(values.ElementAt(i)) < Math.Abs(values.ElementAt(minElem))) minElem = i;
            return values.ElementAt(minElem);
        }
        double StDev(List<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
        double Skew(List<double> values)
        {
            double avg = values.Average();
            double stdev = Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));

            int N = values.Count();
            double dInterim = 0;
            double dCount = (double)N;
            double dMultiplier = (dCount) / ((dCount - 1) * (dCount - 2));

            for (int i = 1; i <= N; i++)
                dInterim = dInterim + Math.Pow(((values.ElementAt(i - 1) - avg) / stdev), 3);

            double skewness = dMultiplier * dInterim;

            return (skewness);
        }
        double Kurtosis(List<double> values)
        {
            double avg = values.Average();
            double stdev = Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));

            int N = values.Count();
            double dInterim = 0;
            double dCount = (double)N;
            double dMultiplier = ((dCount) * (dCount + 1)) / ((dCount - 1) * (dCount - 2) * (dCount - 3));
            double dSubtractor = 3 * (Math.Pow(dCount - 1, 2)) / ((dCount - 2) * (dCount - 3));

            for (int i = 1; i <= N; i++)
                dInterim = dInterim + Math.Pow(((values.ElementAt(i - 1) - avg) / stdev), 4);

            return (dMultiplier * dInterim - dSubtractor);
        }

        double fact(double v) { return gamma(v + 1); }
        double gamma(double v)
        {
            double[] coef = new double[6] { 76.18009172947146, -86.50532032941677, 24.01409824083091,
            -1.231739572450155, 0.1208650973866179E-2, -0.5395239384953E-5 };
            double LogSqrtTwoPi = 0.91893853320467274178, denom = v + 1, y = v + 5.5, series = 1.000000000190015;
            for (int i = 0; i < 6; ++i)
            {
                series += coef[i] / denom;
                denom += 1.0;
            }
            return Math.Exp((LogSqrtTwoPi + (v + 0.5) * Math.Log(y) - y + Math.Log(series / v)));
        }

        const double pi = Math.PI;
    }

    /// <summary>
    ///  Libraray of default functions for Mathematical Expressions
    /// </summary>
    class MathOperatorLibrary
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

        public Dictionary<char, Operator> operators = new Dictionary<char, Operator>();

        public void AddOperator(char name, MathFunction func, int parameters = 2, int precedence = 2, Associative assoc = Associative.LEFT)
        {
            Operator op = new Operator(name, func, parameters, precedence, assoc);
            operators.Add(name, op);
        }

        public MathOperatorLibrary()
        {
            LoadStandardOperators();
        }

        void LoadStandardOperators()
        {
            AddOperator('+', (x) => x[0] + x[1], 2, 2);
            AddOperator('-', (x) => x[0] - x[1], 2, 2);
            AddOperator('*', (x) => x[0] * x[1], 2, 3);
            AddOperator('/', (x) => x[0] / x[1], 2, 3);
            AddOperator('%', (x) => x[0] % x[1], 2, 3);
            AddOperator('^', (x) => Math.Pow(x[0], x[1]), 2, 4, Associative.LEFT);
            AddOperator('!', (x) => fact(x[0]), 1, 10);

            AddOperator('>', (x) => x[0] > x[1] ? 1 : 0, 2, 1);
            AddOperator('<', (x) => x[0] < x[1] ? 1 : 0, 2, 1);
            AddOperator('=', (x) => x[0] == x[1] ? 1 : 0, 2, 1);

            AddOperator('&', (x) => x[0] != 0 && x[1] != 0 ? 1 : 0, 2, 1);
            AddOperator('|', (x) => x[0] != 0 || x[1] != 0 ? 1 : 0, 2, 1);
        }

        double fact(double y)
        {
            double val = Math.Floor(y);
            while (y > 1)
            {
                val = val * (y - 1);
                y--;
            }
            return val;
        }

    }

    /// <summary>
    ///  Evaluates Mathematical Expressions
    /// </summary>
    class MathEval
    {
        public bool debug {
            get;set;
        }

        bool evaluated = false;
        string expression = "";

        double result;
        /// <summary>
        /// Returns the result of evaluating the stored expression
        /// </summary>
        public double Result
        {
            get
            {
                if (!evaluated)
                    Evaluate();
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="debug"></param>
        public MathEval(bool debug = false)
        {
            this.debug = debug;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="expr">Mathematical expression to use</param>
        /// <param name="debug"></param>
        public MathEval(string expr, bool debug = false)
        {
            this.debug = debug;
            expression = expr;
        }

        /// <summary>
        ///  Sets the expression the engine will evaluate 
        /// </summary>
        /// <param name="expr">Mathematical expression to use</param>
        public void SetExpression(string expr)
        {
            expression = expr;
            evaluated = false;
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
            if (debug) Console.WriteLine("Input:   {0}", expression);

            string expr = GetReplacedString(expression);

            if (debug) Console.WriteLine("Replace: {0}", expression);

            List<Token> tokens = GetTokens(expr);

            if (tokens.Count == 0) throw new MathEvaluationException("No valid input given");

            List<Token> RPN = ShuntingYardAlgorithm(tokens);

            double val = PostfixAlgorithm(RPN);

            if (debug) Console.WriteLine("{0,-10}{1,-10}{2,-20}\n", "Result", "", val);

            result = val;

            return val;
        }

        /// <summary>
        ///  Adds a math function to the evaluation engine.
        /// </summary>
        /// <param name="op">Function Name. Case insensitiv</param>
        /// <param name="func">Function that returns a double and take a List&lt;double&gt; as input</param>
        public void AddFunction(string op, MathFunction func)
        {
            MathFunctions.Add(op.ToLower(), func);
        }

        /// <summary>
        ///  Removes a math function from the evaluation engine.
        /// </summary>
        /// <param name="op">Function Name. Case insensitiv</param>
        public void RemoveFunction(string op)
        {
            MathFunctions.functions.Remove(op.ToLower());
        }


        /// <summary>
        ///  Adds a math operator to the evaluation engine.
        /// </summary>
        /// <param name="op">Operator Name. Case insensitiv</param>
        /// <param name="func">Function that returns a double and take a List&lt;double&gt; as input</param>
        /// <param name="func">Precedence (2 = +-, 3 = */%, 4 = ^)</param>
        /// <param name="func">Associative, Left or Right</param>
        public void AddOperator(char op, MathFunction func, int Parameters = 2, int Precedence = 2, Associative associative = Associative.LEFT)
        {
            if (TokenTypes().ContainsKey(op))
            {
                throw new MathOperatorException("Unable to add operators named '(', ')' or ','");
            }
            MathOperators.AddOperator(char.ToLower(op), func, Parameters, Precedence, associative);
        }

        /// <summary>
        ///  Removes a math operator from the evaluation engine.
        /// </summary>
        /// <param name="op">Operator Name. Case insensitiv</param>
        public void RemoveOperator(char op)
        {
            MathOperators.operators.Remove(char.ToLower(op));
        }


        public List<string> GetOperators()
        {
            return (from op in MathOperators.operators
                   select op.Key.ToString()).ToList();
        }
        public List<string> GetFunctions()
        {
            return (from func in MathFunctions.functions
                    select func.Key).ToList();
        }

        Dictionary<string, string> Replacements = new Dictionary<string, string>();
        List<string> Arguments = new List<string>();

        public void SetArguments(string[] args)
        {
            Arguments = args.ToList();
        }
        public void SetArguments(List<string> args)
        {
            Arguments = args.ToList();
        }
        public void SetArguments(List<double> args)
        {
            Arguments = args.Select(x=>x.ToString()).ToList();
        }
        public void SetArguments(double[] args)
        {
            Arguments = args.Select(x => x.ToString()).ToList();
        }

        public void AddArgument(double arg)
        {
            Arguments.Add(arg.ToString());
        }
        public void AddArgument(string arg)
        {
            Arguments.Add(arg);
        }
        public void ClearArguments(string arg)
        {
            Arguments = new List<string>();
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
             Replacements[key] = value;
        }
        string GetReplacedString(string expr)
        {
            var sortedList = from pair in Replacements
                             orderby pair.Key.Length descending
                             select pair;

            foreach (var item in sortedList)
                expr = expr.Replace("$" + item.Key, item.Value);

            int k = Arguments.Count;

            for (int i = k; i > 0; i--)
                expr = expr.Replace("$" + i, Arguments[i - 1]);
            for (int i = 1; i <= k; i++)
                expr = expr.Replace("$-" + i, Arguments[k - i]);

            return expr;
        }

        MathFunctionLibrary MathFunctions = new MathFunctionLibrary();
        MathOperatorLibrary MathOperators = new MathOperatorLibrary();

        double UseFunction(string op, List<object> values)
        {
            values.Reverse();

            if (MathFunctions.functions.ContainsKey(op))
            {
                return MathFunctions.functions[op].Invoke(values);
            }
            else
                throw new MissingMathOperationException(op);
        }

        double UseOperator(string op, List<double> values)
        {
            values.Reverse();
            if (MathOperators.operators.ContainsKey(op[0]))
            {
                double val = MathOperators.operators[op[0]].Function.Invoke(values);
                return val;
            }
            else
                throw new MissingMathOperationException(op);
        }


        int GetPrecedence(Token op)
        {
            if (MathOperators.operators.ContainsKey(op.Value[0]))
                return MathOperators.operators[op.Value[0]].Precevendce;
            else
                throw new MissingMathOperationException("Invalid operator: " + op.Type + "/"+op.Value);
        }

        Associative GetAssociative(Token op)
        {
            if (MathOperators.operators.ContainsKey(op.Value[0]))
                return MathOperators.operators[op.Value[0]].Assoc;
            else
                throw new MissingMathOperationException("Invalid operator: " + op.Type + "/" + op.Value);
        }


        /// <summary>
        ///  Function to insert two parathesis' into a math equation to turn -func(..) into (-1*func(..))
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        string InsertBracket(string outer, string p1, string p2)
        {
            string find = p1 + "-" + p2;
            int pos = outer.IndexOf(find);
            string replace = p1 + "(-1*" + p2;
            outer = outer.Remove(pos, find.Length).Insert(pos, replace);
            int from = pos + replace.Length - 1;
            int lvl = 0;
            for (int i = from; i < outer.Length; i++)
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


        Dictionary<char, TokenType> TokenTypes()
        {
            Dictionary<char, TokenType> specials = new Dictionary<char, TokenType>();
            specials.Add('(', TokenType.Open);
            specials.Add(')', TokenType.Close);
            specials.Add(',', TokenType.ListSep);
            return specials;
        }

        List<Token> GetTokens(string expr)
        {
            List<Token> tokens = new List<Token>();

            expr = Regex.Replace(expr, @"^-([a-zA-Z(])", "-1*$1");
            expr = Regex.Replace(expr, @"\(-([a-zA-Z(])", "(-1*$1");
            expr = Regex.Replace(expr, @"\,-([a-zA-Z(])", ",-1*$1");

            // fix for oper
            Regex reg = new Regex(@"([\^\*\/\+\-\%])-([a-zA-Z]+\(|\()");
            while (reg.IsMatch(expr))
            {
                var cm = reg.Match(expr);
                if (cm.Success)
                {
                    expr = InsertBracket(expr, cm.Groups[1].Value, cm.Groups[2].Value);
                }
            }


            if (debug) Console.WriteLine("Fixed:   {0}", expr);

            Dictionary<char, TokenType> specials = TokenTypes();

            Match m;

            while (expr.Length > 0)
            {
                char c = expr[0];

                // custom operators with 1 parameter might fuck this up when followed by + or -
                bool doubleOp = true;
                if (tokens.Count > 0 && tokens[tokens.Count - 1].Type == TokenType.Operator)
                    if (MathOperators.operators.ContainsKey(tokens[tokens.Count - 1].Value[0]))
                        if (MathOperators.operators[tokens[tokens.Count - 1].Value[0]].Parameters == 1)
                            doubleOp = false;

                if (tokens.Count == 0 ||
                    (tokens[tokens.Count - 1].Type == TokenType.Operator && doubleOp) ||
                    tokens[tokens.Count - 1].Type == TokenType.ListSep ||
                    tokens[tokens.Count - 1].Type == TokenType.Open)
                {

                    m = Regex.Match(expr, @"^[-+]{0,1}([0-9.]+([eE][\-\+]{0,1}[0-9.]+|))");
                    if (m.Success)
                    {
                        string func = m.Groups[0].Value;
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

                if (MathOperators.operators.ContainsKey(c))
                {
                    tokens.Add(new Token(c.ToString(), TokenType.Operator));
                    expr = expr.Substring(1);
                    continue;
                }


                m = Regex.Match(expr, "^\"([^\"]+)\"");
                if (m.Success)
                {
                    string func = m.Groups[1].Value;
                    tokens.Add(new Token(func, TokenType.String));
                    expr = expr.Substring(func.Length+2);
                    continue;
                }

                m = Regex.Match(expr, @"^([a-zA-Z]+)");
                if (m.Success)
                {
                    string func = m.Groups[0].Value;
                    tokens.Add(new Token(func, TokenType.Function));
                    expr = expr.Substring(func.Length);
                    continue;
                }

                m = Regex.Match(expr, @"^([0-9.]+([eE][\-\+]{0,1}[0-9.]+|))");
                if (m.Success)
                {
                    string func = m.Groups[0].Value;
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


        /// <summary>
        /// https://en.wikipedia.org/wiki/Reverse_Polish_notation
        /// </summary>
        /// <param name="RPN"></param>
        /// <returns></returns>
        double PostfixAlgorithm(List<Token> RPN)
        {

            Stack<Object> stack = new Stack<Object>();

            if (debug)
            {
                Console.Write("Postfix:  ");
                foreach (Token t in RPN)
                {
                    if (t.Type == TokenType.Args)
                        Console.Write("{0}>", t.Value);
                    else
                        Console.Write("{0} ", t.Value);
                }
                Console.WriteLine();
                if (debug) Console.WriteLine("\nInput     Operation    Stack");
            }

            int Nargs = 0;

            // While there are input tokens left
            //   Read the next token from input.

            foreach (Token o in RPN)
            {
                if (debug)
                {
                    Console.Write("{0,-10}", o.Value);
                    Console.Write("{0,-10}", o.Type.ToString());
                    foreach (object v in stack)
                    {
                        Console.Write("{0,-10}\n{1,-20}", v, "");
                    }
                    Console.WriteLine();
                }

                // If the token is a value
                if (o.Type == TokenType.Number)
                {
                    // Push it onto the stack.
                    try
                    {
                        double num = double.Parse(o.Value, System.Globalization.CultureInfo.InvariantCulture);
                        stack.Push(num);
                    }
                    catch
                    {
                        throw new MathEvaluationException("Incorrect formated number: " + o.Value);
                    }
                }
                // If the token is a value
                if (o.Type == TokenType.String)
                {
                    // Push it onto the stack.
                    stack.Push(o.Value);
                }
                // Otherwise, the token is an operator (operator here includes both operators and functions).
                else if (o.Type == TokenType.Function || o.Type == TokenType.Operator)
                {
                    // It is known a priori that the operator takes n arguments.
                    // If there are fewer than n values on the stack

                    int N = 2;

                    if (o.Type == TokenType.Operator && MathOperators.operators.ContainsKey(o.Value[0]))
                    {
                        N = MathOperators.operators[o.Value[0]].Parameters;
                    }

                    if (o.Type == TokenType.Function)
                    {
                        N = Nargs;
                    }

                    if (stack.Count < N)
                    {
                        // (Error) The user has not input sufficient values in the expression.
                        throw new MathEvaluationException("The user has not input sufficient values in the expression");
                    }

                    string opr = o.Value.ToLower();

                    // Else, Pop the top n values from the stack.
                    List<object> v = new List<object>();
                    for (int i = 0; i < N; i++)
                    {
                        Object ov = stack.Pop();
                        v.Add(ov);
                    }
                    double result;

                    // Evaluate the operator, with the values as arguments.
                    if (o.Type == TokenType.Operator) 
                        result = UseOperator(opr, v.Select(x=>(double)x).ToList());
                    else 
                        result = UseFunction(opr, v);

                    // Push the returned results, if any, back onto the stack.
                    stack.Push(result);
                }

                else if (o.Type == TokenType.Args)
                {
                    Nargs = int.Parse(o.Value);
                }

            }

            // If there is only one value in the stack
            if (stack.Count == 1)
            {
                // That value is the result of the calculation.
                Object o = stack.Peek();
                if (o is double)
                    return (double)stack.Peek();
                else
                    throw new Exception("Invalid return type: string="+o.ToString());
            }
            // Otherwise, there are more values in the stack
            else
            {
                // (Error) The user input has too many values.
                throw new MathEvaluationException("The user input has too many values");
            }

        }


        /// <summary>
        ///  https://en.wikipedia.org/wiki/Shunting-yard_algorithm
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        List<Token> ShuntingYardAlgorithm(List<Token> tokens)
        {
            List<Token> queue = new List<Token>();
            Stack<Token> stack = new Stack<Token>();

            Dictionary<int, int> lvlargs = new Dictionary<int, int>();
            int lvl = 0;

            // While there are tokens to be read:
            for (int i = 0; i < tokens.Count; i++)
            {
                // Read a token.
                Token o1 = tokens[i];

                // If the token is a number, then add it to the output queue.
                if (o1.Type == TokenType.Number)
                    queue.Add(tokens[i]);

                // If the token is a number, then add it to the output queue.
                if (o1.Type == TokenType.String)
                    queue.Add(tokens[i]);

                // If the token is a function token, then push it onto the stack.
                else if (o1.Type == TokenType.Function)
                    stack.Push(tokens[i]);

                // If the token is a function argument separator (e.g., a comma):
                else if (o1.Type == TokenType.ListSep)
                {
                    lvlargs[lvl]++;

                    // Until the token at the top of the stack is a left parenthesis, 
                    while (stack.Count > 0 && stack.Peek().Type != TokenType.Open)
                    {
                        //// pop operators off the stack onto the output queue.
                        queue.Add(stack.Pop());
                    }
                    // If no left parentheses are encountered, either the separator was misplaced or parentheses were mismatched.
                    if (stack.Peek().Type != TokenType.Open)
                        throw new ParenthesisException("Mismatches parantheses");
                }

                // If the token is an operator, o1, then:
                else if (o1.Type == TokenType.Operator)
                {
                    // while there is an operator token, o2, at the top of the operator stack
                    while (stack.Count > 0 && stack.Peek().Type == TokenType.Operator)
                    {
                        Token o2 = stack.Peek();

                        // and either o1 is left-associative and its precedence is less than or equal to that of o2, or
                        // o1 is right associative, and has precedence less than that of o2,
                        if ((GetAssociative(o1) == Associative.LEFT && GetPrecedence(o1) <= GetPrecedence(o2))
                            || (GetAssociative(o1) == Associative.RIGHT && GetPrecedence(o1) < GetPrecedence(o2)))
                        {
                            // then pop o2 off the operator stack, onto the output queue;
                            stack.Pop();
                            queue.Add(o2);
                        }
                        else
                            break;
                    }

                    // push o1 onto the operator stack.
                    stack.Push(o1);
                }

                // If the token is a left parenthesis (i.e. "("), then push it onto the stack.
                else if (o1.Type == TokenType.Open)
                {
                    lvl++;
                    lvlargs[lvl] = 1;

                    stack.Push(o1);
                }


                // If the token is a right parenthesis (i.e. ")"):
                else if (o1.Type == TokenType.Close)
                {

                    // Until the token at the top of the stack is a left parenthesis, pop operators off the stack onto the output queue.
                    while (stack.Count > 0 && stack.Peek().Type != TokenType.Open)
                    {
                        queue.Add(stack.Pop());
                    }

                    // If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                    if (stack.Peek().Type != TokenType.Open)
                        throw new ParenthesisException("Mismatches parantheses");

                    // Pop the left parenthesis from the stack, but not onto the output queue.
                    Token o2 = stack.Pop();


                    // If the token at the top of the stack is a function token, pop it onto the output queue.
                    if (stack.Count > 0 && stack.Peek().Type == TokenType.Function)
                    {
                        int Nargs = lvlargs[lvl];
                        queue.Add(new Token(Nargs.ToString(), TokenType.Args));

                        queue.Add(stack.Pop());
                    }

                    lvl--;
                }

            }

            // When there are no more tokens to read:
            // While there are still operator tokens in the stack:
            while (stack.Count > 0)
            {
                // If the operator token on the top of the stack is a parenthesis, then there are mismatched parentheses.
                if (stack.Peek().Type == TokenType.Open)
                {
                    throw new ParenthesisException("Mismatches parantheses");
                }

                // Pop the operator onto the output queue.
                queue.Add(stack.Pop());

            }

            return queue;
        }

    }

}