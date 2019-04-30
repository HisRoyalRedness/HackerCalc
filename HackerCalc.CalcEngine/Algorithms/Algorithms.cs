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
            if (index == 0)
                return 1;
            if (index == 1)
                return radicand;
            if (radicand < 0)
                throw new InvalidCalcOperationException("Roots can only be taken of positive numbers");
            if (radicand == 0)
                return 0;

            var x = (BigInteger)Math.Pow((double)radicand, 1.0 / index);
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
            => ((radicand / BigInteger.Pow(x, index - 1)) - x) / index;
    }
}
