using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using math;

namespace SimpleMath
{
    internal class Program
    {
        private enum Option {
            DoMath,
            ShowHelp,
        }

        public static void Error(string str)
        {
            Console.Error.WriteLine(str);
#if DEBUG
            Console.ReadKey();
#endif
            Environment.Exit(1);
        }


        private static void Main(string[] args) 
        {
            var perform = Option.ShowHelp;

            // check pipe
            var pipedText = "";
            bool isKeyAvailable;
            var piped = false;
            try { isKeyAvailable = Console.KeyAvailable; }
            catch { pipedText = Console.In.ReadToEnd(); piped = true; }

            var expr = "";
            var inpfile = "";
            var inpdata = "";
            var format = "{0}";

            var showAll = true;
            var expr2 = "";
            var showSteps = false;

            // check input arguments
            for (var i = 0; i < args.Length; i++)
            {

                if (args[i] == "-help" || args[i] == "--help" || args[i] == "-?")
                {
                    perform = Option.ShowHelp;
                    break;
                }

                if (args[i] == "-f")
                {
                    if (args.Length < i + 1) Error("Insufficient args -f");
                    format = args[i + 1];
                    i++;
                }
                else if (args[i] == "-o")
                {
                    if (args.Length < i + 1) Error("Insufficient args -o");
                    inpfile = args[i + 1];
                    i++;
                }
                else if (args[i] == "-i")
                {
                    if (args.Length < i + 1) Error("Insufficient args -i");
                    inpdata = args[i + 1];
                    i++;
                }
                else if (args[i] == "-e")
                {
                    if (args.Length < i + 1) Error("Insufficient args for -e");
                    expr2 = args[i + 1];
                    showAll = false;
                    i++;
                    perform = Option.DoMath;
                }
                else if (args[i] == "-x")
                {
                    showAll = false;
                }
                else if (args[i] == "-d")
                {
                    showSteps = true;
                }
                else
                {
                    perform = Option.DoMath;
                    expr += " " + args[i];
                }
            }

            // Print header
            if (perform == Option.ShowHelp)
            {
                Console.WriteLine(@"Usage: math [OPTIONS] EXPRESSION

  -f FORMAT  Default is {0}. C# convention: 
             https://msdn.microsoft.com/en-us/library/0c899ak8(v=vs.110).aspx
             Scientific: {0:0.0000E+00}
             Decimal, optional decimals: {0:0.##########}
             With text: " + "\"Ans={0:0.00000}\"" + @"

  -o FILE    Use FILE as input to EXPRESSION
  -i DATA    Use DATA as input to EXPRESSION (comma-seperated)
  -x         Only write last line
  -d         Show evaluation steps

  -e EXPRESISION
             Calculate this expression after all other are calculated. 
             Only this line is written.

Calcualte the math expression given in EXPRESSION. 

If FILE or pipe is given, EXPRESSION is evaulated for each given line.
If DATA is given, EXPRESSION is evalueted for each given line in DATA. Seperate lines with ','.
$1, $2,.. returns the delimited numbers from each given line.
$-1, $-2,.. returns the reverse delimited numbers from each given line.
$0 is equal to the previous evaluated line.
$N is equal to the line number.
$A is an array of all previous results.

Simple functions:
  log,ln,exp,gamma,fact,abs,floor,ceil,sqrt,
  cos,sin,tan,acos,asin,atan,cosh,sinh,tanh

Multiple argument functions:
  sum,max,min,amin,amax,stdev,skew,kurt,avrg,round
  count

Operators:    ^,*,/,+,-     standard math operators
                !,%         factorial and reminder
              <,>,=,&,|     comparison. 0 (false) / 1 or any number (true)

Version 1.0. Report bugs to <martsve@gmail.com>");
                Environment.Exit(0);
            }


            // open file
            if (inpfile != "")
            {
                try {
                    piped = true;
                    pipedText = File.ReadAllText(inpfile);
                }
                catch { Error("Unable to open file: " + inpfile); }
            }
            else if (inpdata != "")
            {
                try
                {
                    piped = true;
                    pipedText = inpdata.Replace(",", "\n");
                }
                catch { Error("Unable to open file: " + inpfile); }
            }

            var evalLines = new List<string>();
            if (piped)
            {
                evalLines = pipedText.Split('\n').ToList();
            }
            else
            {
                evalLines.Add("0");
            }

            double last = 0;
            var count = 0;
            var results = new List<double>();

            if (string.IsNullOrEmpty(expr))
            {
                expr = "$1";
            }

            var eval = new MathEvalWrapper();

            foreach (var input in evalLines)
            {
                var line = input.Replace("\r", "").Replace(",", " ").Replace("\t", " ").ToLower().Trim();
                line = Regex.Replace(line, @"\s+", " ");

                if (line.StartsWith("#")) continue;
                if (line.Length == 0) continue;

                count++;

                eval.SetExpression(expr, line);
                var res = eval.GetResult();

                if (showSteps)
                {
                    foreach (var entry in eval.History)
                    {
                        Console.WriteLine(entry);
                    }
                }

                results.Add(res);

                if (showAll)
                {
                    try
                    {
                        Console.WriteLine(format, res);
                    }
                    catch { Error("Invalid format specified with -f: " + format); }
                }

                last = res;
            }


            if (!showAll)
            {
                if (expr2 != "")
                {
                    eval.LockLineNumber();
                    eval.SetExpression(expr2);
                    last = eval.GetResult();
                }

                try
                {
                    Console.WriteLine(format, last);
                }
                catch { Error("Invalid format specified with -f: " + format); }
            }

#if DEBUG
            Console.ReadKey();
#endif
        }


        private class MathEvalWrapper {
            private readonly MathEval _engine = new MathEval();

            private string _expression;
            private bool _evaluated = false;
            private double _value = 0;
            private int _line = 0;
            private readonly List<double> _results = new List<double>();
            private bool _useLineNumber = true;

            public List<string> History => _engine.History;

            public void LockLineNumber()
            {
                _useLineNumber = false;
            }

            public void SetExpression(string input, string instr)
            {
                var args = instr.Split(' ');

                _engine.AddReplacement("L", instr);
                _engine.SetArguments(args);

                SetExpression(input);
            }

            public void SetExpression(string input) {
                _evaluated = false;
                if (_useLineNumber)
                    _line += 1;

                var arr = string.Join(",", _results.ToArray());
                if (arr.Length == 0)
                    arr = "0";

                _engine.AddReplacement("0", _value);
                _engine.AddReplacement("N", _line);
                _engine.AddReplacement("A", arr);

                _expression = input;
            }

            public MathEvalWrapper()
            {

            }           

            public MathEvalWrapper(string input)
            {
                SetExpression(input);
            }

            public double GetResult() {
                if (!_evaluated)
                {
                    Evaluate();
                }
                return _value;
            }

            public double Evaluate()
            {
                try
                {
                    _value = _engine.EvaluateExpression(_expression);
                    _results.Add(_value);
                    return _value;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Unable to evaluate expression:\n" + _expression.Trim() + "\n");

                    foreach (var line in _engine.History)
                    {
                        Console.WriteLine(line);
                    }
                    
                    Error(ex.ToString());

                    return 0;
                }

            }
            
        }


    }
}

