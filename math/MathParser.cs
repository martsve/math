using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace math
{
    class MathParser
    {
        private readonly MathFunctionLibrary _mathFunctions;
        private readonly MathOperatorLibrary _mathOperators;

        public MathParser(MathOperatorLibrary mathOperators, MathFunctionLibrary mathFunctions)
        {
            _mathOperators = mathOperators;
            _mathFunctions = mathFunctions;
        }

        public bool Debug { get; set; }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Reverse_Polish_notation
        /// </summary>
        /// <param name="rpn"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        public double PostfixAlgorithm(List<Token> rpn, out List<string> history)
        {
            var stack = new Stack<Object>();
            history = new List<string>();
        
            var postfix = "Postfix: ";
            foreach (var t in rpn)
            {
                postfix += t.Type == TokenType.Args ? $"{t.Value}>" : $"{t.Value} ";
            }

            history.Add(postfix);
            history.Add("");
            history.Add("\nInput     Operation    Stack");
       
            var nargs = 0;

            // While there are input tokens left
            //   Read the next token from input.

            foreach (var o in rpn)
            {
                var text = $"{o.Value,-10}";
                text += $"{o.Type,-10}";
                foreach (var v in stack)
                {
                    text += $"{v,-10}\n{"",-20}";
                }
                history.Add(text);
                history.Add("");

                // If the token is a value
                if (o.Type == TokenType.Number)
                {
                    // Push it onto the stack.
                    try
                    {
                        var num = double.Parse(o.Value, System.Globalization.CultureInfo.InvariantCulture);
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

                    var n = 2;

                    if (o.Type == TokenType.Operator && _mathOperators.Operators.ContainsKey(o.Value[0]))
                    {
                        n = _mathOperators.Operators[o.Value[0]].Parameters;
                    }

                    if (o.Type == TokenType.Function)
                    {
                        n = nargs;
                    }

                    if (stack.Count < n)
                    {
                        // (Error) The user has not input sufficient values in the expression.
                        throw new MathEvaluationException("The user has not input sufficient values in the expression");
                    }

                    var opr = o.Value.ToLower();

                    // Else, Pop the top n values from the stack.
                    var v = new List<object>();
                    for (var i = 0; i < n; i++)
                    {
                        var ov = stack.Pop();
                        v.Add(ov);
                    }
                    double result;

                    // Evaluate the operator, with the values as arguments.
                    if (o.Type == TokenType.Operator)
                        result = UseOperator(opr, v.Select(x => (double)x).ToList());
                    else
                        result = UseFunction(opr, v);

                    // Push the returned results, if any, back onto the stack.
                    stack.Push(result);
                }

                else if (o.Type == TokenType.Args)
                {
                    nargs = int.Parse(o.Value);
                }

            }

            // If there is only one value in the stack
            if (stack.Count == 1)
            {
                // That value is the result of the calculation.
                var o = stack.Peek();
                if (o is double)
                    return (double)stack.Peek();
                throw new Exception("Invalid return type: string=" + o);
            }
            // Otherwise, there are more values in the stack

            // (Error) The user input has too many values.
            throw new MathEvaluationException("The user input has too many values");
        }

        /// <summary>
        ///  https://en.wikipedia.org/wiki/Shunting-yard_algorithm
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public List<Token> ShuntingYardAlgorithm(List<Token> tokens)
        {
            var queue = new List<Token>();
            var stack = new Stack<Token>();

            var lvlargs = new Dictionary<int, int>();
            var lvl = 0;

            // While there are tokens to be read:
            for (var i = 0; i < tokens.Count; i++)
            {
                // Read a token.
                var o1 = tokens[i];

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
                        var o2 = stack.Peek();

                        // and either o1 is left-associative and its precedence is less than or equal to that of o2, or
                        // o1 is right associative, and has precedence less than that of o2,
                        if ((GetAssociative(o1) == Associative.Left && GetPrecedence(o1) <= GetPrecedence(o2))
                            || (GetAssociative(o1) == Associative.Right && GetPrecedence(o1) < GetPrecedence(o2)))
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
                    var o2 = stack.Pop();


                    // If the token at the top of the stack is a function token, pop it onto the output queue.
                    if (stack.Count > 0 && stack.Peek().Type == TokenType.Function)
                    {
                        var nargs = lvlargs[lvl];
                        queue.Add(new Token(nargs.ToString(), TokenType.Args));

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
        
        private double UseFunction(string op, List<object> values)
        {
            values.Reverse();

            if (_mathFunctions.Functions.ContainsKey(op))
            {
                return _mathFunctions.Functions[op].Invoke(values);
            }

            throw new MissingMathOperationException(op);
        }

        private double UseOperator(string op, List<double> values)
        {
            values.Reverse();
            if (_mathOperators.Operators.ContainsKey(op[0]))
            {
                var val = _mathOperators.Operators[op[0]].Function.Invoke(values);
                return val;
            }

            throw new MissingMathOperationException(op);
        }

        private int GetPrecedence(Token op)
        {
            if (_mathOperators.Operators.ContainsKey(op.Value[0]))
                return _mathOperators.Operators[op.Value[0]].Precevendce;
            throw new MissingMathOperationException("Invalid operator: " + op.Type + "/" + op.Value);
        }

        private Associative GetAssociative(Token op)
        {
            if (_mathOperators.Operators.ContainsKey(op.Value[0]))
                return _mathOperators.Operators[op.Value[0]].Assoc;
            throw new MissingMathOperationException("Invalid operator: " + op.Type + "/" + op.Value);
        }
    }
}
