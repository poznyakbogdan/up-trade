using System.Numerics;
using Nethereum.Web3;

namespace Implementations;

public static class BigIntegerExtensions
{
    public static decimal ToDecimal(this BigInteger bigInt, int decimals = 18) =>
         Web3.Convert.FromWei(bigInt, decimals);
}