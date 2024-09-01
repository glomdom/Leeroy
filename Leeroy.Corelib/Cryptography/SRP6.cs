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
using System.Security.Cryptography;
using System.Text;

namespace Leeroy.Corelib.Cryptography;

public static class SRP6 {
    public static ReadOnlySpan<byte> LARGE_SAFE_PRIME_LE => new byte[] {
        0xb7, 0x9b, 0x3e, 0x2a, 0x87, 0x82, 0x3c, 0xab,
        0x8f, 0x5e, 0xbf, 0xbf, 0x8e, 0xb1, 0x01, 0x08,
        0x53, 0x50, 0x06, 0x29, 0x8b, 0x5b, 0xad, 0xbd,
        0x5b, 0x53, 0xe1, 0x89, 0x5e, 0x64, 0x4b, 0x89,
    };

    public static ReadOnlySpan<byte> LARGE_SAFE_PRIME_BE => new byte[] {
        0x89, 0x4b, 0x64, 0x5e, 0x89, 0xe1, 0x53, 0x5b,
        0xbd, 0xad, 0x5b, 0x8b, 0x29, 0x06, 0x50, 0x53,
        0x08, 0x01, 0xb1, 0x8e, 0xbf, 0xbf, 0x5e, 0x8f,
        0xab, 0x3c, 0x82, 0x87, 0x2a, 0x3e, 0x9b, 0xb7,
    };

    public static ReadOnlySpan<byte> PRECALCULATED_XOR_HASH => new byte[] {
        0xdd, 0x7b, 0xb0, 0x3a, 0x38, 0xac, 0x73, 0x11, 0x3, 0x98,
        0x7c, 0x5a, 0x50, 0x6f, 0xca, 0x96, 0x6c, 0x7b, 0xc2, 0xa7,
    };

    public static readonly BigInteger Generator = new BigInteger(7);
    public static readonly BigInteger K = new BigInteger(3);
    public static readonly BigInteger LargeSafePrime = LARGE_SAFE_PRIME_LE.ToBigInteger();
    public static readonly BigInteger PrecalculatedXORhash = PRECALCULATED_XOR_HASH.ToBigInteger();

    private const byte KeyLength = 32;

    public static byte[] CalculatePasswordVerifier(string username, string password, ReadOnlySpan<byte> salt) {
        if (salt.IsEmpty) {
            throw new ArgumentException("salt must not be empty");
        }

        var x = CalculateX(username, password, salt);
        var data = BigInteger.ModPow(Generator, x.ToBigInteger(), LargeSafePrime);

        return data.ToPaddedArray(KeyLength);
    }

    public static byte[] CalculateX(string username, string password, ReadOnlySpan<byte> salt) {
        if (salt.IsEmpty) {
            throw new ArgumentException("salt must not be empty");
        }

        var normalizedUsername = Utilities.NormalizeUsernameOrPassword(username);
        var normalizedPassword = Utilities.NormalizeUsernameOrPassword(password);

        var interim = $"{normalizedUsername}:{normalizedPassword}";
        var interimHashed = SHA1.HashData(Encoding.UTF8.GetBytes(interim));
        var concatted = Utilities.ConcatArrays(salt, interimHashed);

        return SHA1.HashData(concatted);
    }

    public static byte[] CalculateServerPublicKey(ReadOnlySpan<byte> passwordVerifier, ReadOnlySpan<byte> serverPrivateKey) {
        var vBig = passwordVerifier.ToBigInteger();
        var bBig = serverPrivateKey.ToBigInteger();

        var interim = K * vBig + BigInteger.ModPow(Generator, bBig, LargeSafePrime);
        var result = interim % LargeSafePrime;

        return result.ToPaddedArray(KeyLength);
    }

    public static byte[] CalculateClientSessionValue(
        ReadOnlySpan<byte> clientPrivateKey,
        ReadOnlySpan<byte> serverPublicKey,
        ReadOnlySpan<byte> x,
        ReadOnlySpan<byte> scrambler,
        ReadOnlySpan<byte> largeSafePrime,
        byte               generator
    ) {
        var clientPrivateKeyInt = clientPrivateKey.ToBigInteger();
        var serverPublicKeyInt = serverPublicKey.ToBigInteger();
        var xInt = x.ToBigInteger();
        var uInt = scrambler.ToBigInteger();
        var generatorInt = generator.ToBigInteger();
        var largeSafePrimeInt = largeSafePrime.ToBigInteger();

        var S = (serverPublicKeyInt - K * generatorInt.ModPow(xInt, largeSafePrimeInt)).ModPow(clientPrivateKeyInt + uInt * xInt, largeSafePrimeInt);

        var sessionKey = S.ToPaddedArray(KeyLength);

        return sessionKey;
    }

    public static byte[] CalculateServerSessionValue(
        ReadOnlySpan<byte> clientPublicKey,
        ReadOnlySpan<byte> passwordVerifier,
        ReadOnlySpan<byte> scrambler,
        ReadOnlySpan<byte> serverPrivateKey
    ) {
        var clientPublicKeyInt = clientPublicKey.ToBigInteger();
        var passwordVerifierInt = passwordVerifier.ToBigInteger();
        var uInt = scrambler.ToBigInteger();
        var serverPrivateKeyInt = serverPrivateKey.ToBigInteger();

        var S = (clientPublicKeyInt * passwordVerifierInt.ModPow(uInt, LargeSafePrime)).ModPow(serverPrivateKeyInt, LargeSafePrime);
        var sessionKey = S.ToPaddedArray(KeyLength);

        return sessionKey;
    }

    public static byte[] CalculateScrambler(ReadOnlySpan<byte> clientPublicKey, ReadOnlySpan<byte> serverPublicKey) {
        var scrambler = SHA1.HashData(Utilities.ConcatArrays(clientPublicKey, serverPublicKey));

        return scrambler;
    }

    public static byte[] SplitSessionValue(byte[] sessionValue) {
        while (sessionValue[0] == 0) {
            sessionValue = sessionValue[2..];
        }

        return sessionValue;
    }

    public static byte[] SHAInterleave(byte[] sessionValue) {
        var splitSessionValue = SplitSessionValue(sessionValue);

        var E = splitSessionValue.Where((_, index) => index % 2 == 0).ToArray();
        var G = SHA1.HashData(E);

        var F = splitSessionValue.Where((_, index) => index % 2 != 0).ToArray();
        var H = SHA1.HashData(F);

        var interleaved = new byte[40];
        for (var i = 0; i < 20; i++) {
            interleaved[i * 2] = G[i];
            interleaved[i * 2 + 1] = H[i];
        }

        return interleaved;
    }

    public static byte[] CalculateServerSessionKey(
        ReadOnlySpan<byte> clientPublicKey,
        ReadOnlySpan<byte> serverPublicKey,
        ReadOnlySpan<byte> passwordVerifier,
        ReadOnlySpan<byte> serverPrivateKey
    ) {
        var u = CalculateScrambler(clientPublicKey, serverPublicKey);
        var S = CalculateServerSessionValue(clientPublicKey, passwordVerifier, u, serverPrivateKey);
        var sessionKey = SHAInterleave(S);

        return sessionKey;
    }

    public static byte[] CalculateClientSessionKey(
        string             username,
        string             password,
        ReadOnlySpan<byte> serverPublicKey,
        ReadOnlySpan<byte> clientPrivateKey,
        ReadOnlySpan<byte> clientPublicKey,
        ReadOnlySpan<byte> salt,
        ReadOnlySpan<byte> largeSafePrime,
        byte               generator
    ) {
        var x = CalculateX(username, password, salt);
        var scrambler = CalculateScrambler(clientPublicKey, serverPublicKey);
        var S = CalculateClientSessionValue(clientPrivateKey, serverPublicKey, x, scrambler, largeSafePrime, generator);
        var sessionKey = SHAInterleave(S);
        
        return sessionKey;
    }

    public static byte[] CalculateServerProof(
        ReadOnlySpan<byte> clientPublicKey,
        ReadOnlySpan<byte> clientProof,
        ReadOnlySpan<byte> sessionKey
    ) {
        var full = Utilities.ConcatArrays(clientPublicKey, clientProof, sessionKey);
        var proof = SHA1.HashData(full);

        return proof;
    }

    public static byte[] CalculateClientProof(
        ReadOnlySpan<byte> xorhash,
        string             username,
        ReadOnlySpan<byte> sessionKey,
        ReadOnlySpan<byte> clientPublicKey,
        ReadOnlySpan<byte> serverPublicKey,
        ReadOnlySpan<byte> salt
    ) {
        var normalizedUsername = Utilities.NormalizeUsernameOrPassword(username);
        var hashed_username = SHA1.HashData(Encoding.UTF8.GetBytes(normalizedUsername));
        var full = Utilities.ConcatArrays(xorhash, hashed_username, salt, clientPublicKey, serverPublicKey, sessionKey);
        var proof = SHA1.HashData(full);

        return proof;
    }
}
