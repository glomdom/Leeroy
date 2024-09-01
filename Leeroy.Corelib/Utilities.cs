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
