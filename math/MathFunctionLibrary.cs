using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace math
{

    /// <summary>
    ///  Library of default functions for Mathematical Expressions
    /// </summary>
    public class MathFunctionLibrary
    {
        public class Function
        {
            private readonly MathFunction _mfunc;
            private readonly ObjectFunction _ofunc;
            private readonly Type _t;
            public Function(object o)
            {
                if (o is MathFunction function)
                {
                    _mfunc = function;
                    _t = typeof(MathFunction);
                }
                else
                {
                    _ofunc = (ObjectFunction)o;
                    _t = typeof(ObjectFunction);
                }
            }
            public double Invoke(List<object> inp)
            {
                if (_t == typeof(MathFunction))
                {
                    return _mfunc.Invoke(inp.Select(x => (double)x).ToList());
                }

                return _ofunc.Invoke(inp);
            }
        }

        public Dictionary<string, Function> Functions = new Dictionary<string, Function>();

        public MathFunctionLibrary()
        {
            LoadStandardFunctions();
        }

        public void Add(string name, MathFunction func)
        {
            var f = new Function(func);
            Functions.Add(name, f);
        }
        public void Add(string name, ObjectFunction func)
        {
            var f = new Function(func);
            Functions.Add(name, f);
        }

        private void LoadStandardFunctions()
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
            Add("pi", (List<double> x) => Pi);

            Add("gamma", (x) => Gamma(x[0]));
            Add("fact", (x) => Fact(x[0]));

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
            Add("count", (List<object> x) => x.Count);
        }

        private double Amax(List<double> values)
        {
            var maxElem = 0;
            for (var i = 0; i < values.Count(); i++)
                if (Math.Abs(values.ElementAt(i)) > Math.Abs(values.ElementAt(maxElem))) maxElem = i;
            return values.ElementAt(maxElem);
        }

        private double Amin(List<double> values)
        {
            var minElem = 0;
            for (var i = 0; i < values.Count(); i++)
                if (Math.Abs(values.ElementAt(i)) < Math.Abs(values.ElementAt(minElem))) minElem = i;
            return values.ElementAt(minElem);
        }

        private double StDev(List<double> values)
        {
            var avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        private double Skew(List<double> values)
        {
            var avg = values.Average();
            var stdev = Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));

            var n = values.Count();
            double dInterim = 0;
            var dCount = (double)n;
            var dMultiplier = (dCount) / ((dCount - 1) * (dCount - 2));

            for (var i = 1; i <= n; i++)
                dInterim = dInterim + Math.Pow(((values.ElementAt(i - 1) - avg) / stdev), 3);

            var skewness = dMultiplier * dInterim;

            return (skewness);
        }

        private double Kurtosis(List<double> values)
        {
            var avg = values.Average();
            var stdev = Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));

            var n = values.Count();
            double dInterim = 0;
            var dCount = (double)n;
            var dMultiplier = ((dCount) * (dCount + 1)) / ((dCount - 1) * (dCount - 2) * (dCount - 3));
            var dSubtractor = 3 * (Math.Pow(dCount - 1, 2)) / ((dCount - 2) * (dCount - 3));

            for (var i = 1; i <= n; i++)
                dInterim = dInterim + Math.Pow(((values.ElementAt(i - 1) - avg) / stdev), 4);

            return (dMultiplier * dInterim - dSubtractor);
        }

        private double Fact(double v) { return Gamma(v + 1); }

        private double Gamma(double v)
        {
            var coef = new double[6] { 76.18009172947146, -86.50532032941677, 24.01409824083091,
            -1.231739572450155, 0.1208650973866179E-2, -0.5395239384953E-5 };
            double logSqrtTwoPi = 0.91893853320467274178, denom = v + 1, y = v + 5.5, series = 1.000000000190015;
            for (var i = 0; i < 6; ++i)
            {
                series += coef[i] / denom;
                denom += 1.0;
            }
            return Math.Exp((logSqrtTwoPi + (v + 0.5) * Math.Log(y) - y + Math.Log(series / v)));
        }

        private const double Pi = Math.PI;
    }
}
