using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace math
{

    /// <summary>
    ///  Invalid operators in a Mathematical Expressions
    /// </summary>
    internal class MathOperatorException : Exception
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
    internal class MathEvaluationException : Exception
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
    internal class ParenthesisException : Exception
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
    internal class MathOperationArgumentException : Exception
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
    internal class MissingMathOperationException : Exception
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

}
