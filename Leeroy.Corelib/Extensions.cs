using System.Numerics;

namespace Leeroy.Corelib;

public static class Extensions {
    public static byte[] ToByteArray(this string hex, bool padding = true) {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("String cannot have an odd number of digits", nameof(hex));

        var bytes = Enumerable.Range(0, hex.Length / 2)
            .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
            .ToArray();

        if (padding && bytes.Length % 2 != 0)
            bytes = new byte[] { 0 }.Concat(bytes).ToArray();

        Array.Reverse(bytes);

        return bytes;
    }

    public static string ToHexString(this byte[] bytes, bool uppercase = false) {
        return string.Create(bytes.Length * 2, (bytes, uppercase), (span, state) => {
            var (byteArray, useUppercase) = state;
            var format = useUppercase
                ? "X2"
                : "x2";

            for (var i = 0; i < byteArray.Length; i++) {
                var hex = byteArray[i].ToString(format);
                span[i * 2] = hex[0];
                span[i * 2 + 1] = hex[1];
            }
        });
    }

    public static byte[] ToPaddedArray(this BigInteger b, int length) => Utilities.TrimArray(b.ToByteArray(), length);

    public static BigInteger ModPow(this BigInteger value, BigInteger exponent, BigInteger modulus) {
        var val = BigInteger.ModPow(value, exponent, modulus);

        if (val < 0) {
            val += modulus;
        }

        return val;
    }

    public static BigInteger ToBigInteger(this ReadOnlySpan<byte> bytes) => new BigInteger(bytes, true);
    public static BigInteger ToBigInteger(this byte[] bytes) => new BigInteger(bytes, true);
    public static BigInteger ToBigInteger(this byte byt) => new BigInteger(byt);
}
