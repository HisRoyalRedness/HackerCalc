using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{

    public static partial class Algoritms
    {
        public static BigInteger IntegerNthRoot(BigInteger radicand, int index)
        {
            if (radicand == 0)
                return 0;
            if (radicand < 0)
                throw new InvalidCalcOperationException("Roots can only be taken of positive numbers");

            var bits = new BigInteger(BigInteger.Log10(radicand) / BigInteger.Log10(2) + 1);
            var x =bits / index;
            //var x = 100.0;
            BigInteger delta = 0;
            do
            {
                delta = Delta(radicand, index, x);
                x += delta;
            } while (delta != 0);

            var test = BigInteger.Pow(x, index);
            if (test == radicand)
                return x;

            if (test < radicand)
            {
                if (BigInteger.Pow(x + 1, index) > radicand)
                    return x;
            }
            else if (test > radicand)
            {
                if (BigInteger.Pow(x - 1, index) <= radicand)
                    return x - 1;
            }

            throw new ApplicationException($"Something went wrong. Radicand={radicand}, index={index}, x={x}");
        }

        static BigInteger Delta(BigInteger radicand, int index, BigInteger x)
        {
            var sub = BigInteger.Pow(x, index - 1);
            var frac = radicand / sub;
            var inner = frac - x;
            var final = inner / index;
            return final;
        }

        static double Delta(double radicand, int index, double x)
        {
            var sub = Math.Pow(x, index - 1);
            var frac = radicand / sub;
            var inner = frac - x;
            var final = Math.Abs(inner) / index;
            return final;
        }
        //=> BigInteger.Abs(radicand / BigInteger.Pow(x, index - 1) - x) / index;
    }
}
