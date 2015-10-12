using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using MathEvaluation;

namespace SimpleMath
{
    class Program
    {
        enum Option {
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


        static void Main(string[] args) 
        {
            Option perform = Option.ShowHelp;

            // check pipe
            String pipedText = "";
            bool isKeyAvailable;
            bool piped = false;
            try { isKeyAvailable = System.Console.KeyAvailable; }
            catch { pipedText = System.Console.In.ReadToEnd(); piped = true; }

            string expr = "";
            string inpfile = "";
            string format = "{0}";

            bool showAll = true;
            string expr2 = "";
            bool showSteps = false;

            // check input arguments
            for (int i = 0; i < args.Length; i++)
            {

                if (args[i] == "-help" || args[i] == "--help" || args[i] == "-?") 
                    perform = Option.ShowHelp;

                else if (args[i] == "-f")
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
                else if (args[i] == "-e")
                {
                    if (args.Length < i + 1) Error("Insufficient args for -e");
                    expr2 = args[i + 1];
                    showAll = false;
                    i++;
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
             With text: "+"\"Ans={0:0.00000}\""+ @"

  -o FILE    Use FILE as input to EXPRESSION
  -x         Only write last line
  -d         Show evaluation steps

  -e EXPRESISION
             Calculate this expression after all other are calculated. 
             Only this line is written.

Calcualte the math expression given in EXPRESSION. 

If FILE or pipe is given, EXPRESSION is evaulated for each given line.
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


            List<string> evalLines = new List<string>();
            if (piped)
            {
                evalLines = pipedText.Split('\n').ToList();
            }
            else
            {
                evalLines.Add("0");
            }

            double last = 0;
            int count = 0;
            List<double> results = new List<double>();

            MathEvalWrapper eval = new MathEvalWrapper(showSteps);

            foreach (string input in evalLines)
            {
                string line = input.Replace("\r", "").Replace(",", " ").Replace("\t", " ").ToLower().Trim();
                line = System.Text.RegularExpressions.Regex.Replace(line, @"\s+", " ");

                if (line.StartsWith("#")) continue;
                if (line.Length == 0) continue;

                count++;

                eval.SetExpression(expr, line);
                double res = eval.GetResult();
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





        class MathEvalWrapper {

            MathEval engine = new MathEval();

            string expression;
            bool evaluated = false;
            double value = 0;
            int line = 0;
            List<double> results = new List<double>();
            bool useLineNumber = true;

            public void LockLineNumber()
            {
                useLineNumber = false;
            }

            public void SetExpression(string input, string instr)
            {
                string[] args = instr.Split(' ');

                engine.AddReplacement("L", instr);
                engine.SetArguments(args);

                SetExpression(input);
            }

            public void SetExpression(string input) {
                evaluated = false;
                if (useLineNumber)
                    line += 1;

                string arr = string.Join(",", results.ToArray());
                if (arr.Length == 0)
                    arr = "0";

                engine.AddReplacement("0", value);
                engine.AddReplacement("N", line);
                engine.AddReplacement("A", arr);

                expression = input;
            }

            public MathEvalWrapper()
            {

            }


            public MathEvalWrapper(bool showSteps) : this()
            {
                engine.debug = showSteps;
            }

            public MathEvalWrapper(string input, bool showSteps)
                : this(showSteps)
            {
                SetExpression(input);
            }

            public double GetResult() {
                if (!evaluated)
                {
                    Evaluate();
                }
                return value;
            }

            public double Evaluate()
            {
                try
                {
                    value = engine.EvaluateExpression(expression);
                    results.Add(value);
                    return value;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Unable to evaluate expression:\n" + expression.Trim() + "\n");

                    if (!engine.debug)
                    {
                        try
                        {
                            engine.debug = true;
                            value = engine.EvaluateExpression(expression);
                        }
                        catch
                        {
                        }
                    }

                    Error(ex.ToString());
                    return 0;
                }

            }
            
        }


    }
}

