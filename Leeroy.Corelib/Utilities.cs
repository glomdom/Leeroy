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

namespace Leeroy.Corelib;

public static class Utilities {
    public static T[] TrimArray<T>(T[] arr, int length) {
        var newArr = new T[length];

        Array.Copy(arr, newArr, Math.Min(arr.Length, length));
        
        return newArr;
    }

    public static byte[] ConcatArrays(ReadOnlySpan<byte> first, ReadOnlySpan<byte> second) {
        Span<byte> concatted = stackalloc byte[first.Length + second.Length];

        first.CopyTo(concatted);
        second.CopyTo(concatted[first.Length..]);

        return concatted.ToArray();
    }

    /// <summary>
    /// Uppercases a username or password. Does some checks.
    /// </summary>
    /// <exception cref="ArgumentException">Throwed when the username or password is more than 16 characters or empty.</exception>
    public static string NormalizeUsernameOrPassword(string str) {
        if (str.Length is > 16 or 0) {
            throw new ArgumentException("Provided username/password is more than 16 characters or empty.", nameof(str));
        }
        
        return str.ToUpper();
    }
}
