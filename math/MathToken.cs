using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace math
{
    /// <summary>
    ///  Mathematical Expressions tokens
    /// </summary>
    public class Token
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
    public enum TokenType
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

    public enum Associative
    {
        Left,
        Right
    }

}
