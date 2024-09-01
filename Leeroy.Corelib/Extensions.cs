/*

    Copyright (C) 2024  glomdom

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

*/

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
