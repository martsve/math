# Mathemathical Expression Evaluator

    Usage: math [OPTIONS] EXPRESSION

      -f FORMAT  Default is {0}. C# convention:
                 https://msdn.microsoft.com/en-us/library/0c899ak8(v=vs.110).aspx
                 Scientific: {0:0.0000E+00}
                 Decimal, optional decimals: {0:0.##########}
                 With text: "Ans={0:0.00000}"

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

    Operators:    ^,*,/,%,+,-

