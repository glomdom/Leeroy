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

using Leeroy.Corelib;

namespace Leeroy.Tests.Cryptography;

public class SRP6 {
    private const string TestDataRoot = "../../../Data";
    
    [SetUp]
    public void Setup() {}

    [Test]
    public void TestCalculatePasswordVerifier() {
        var data = File.ReadAllLines($"{TestDataRoot}/v_values.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var username = split[0];
            var password = split[1];
            var salt = split[2];
            
            var expected = split[3];
            var result = Corelib.Cryptography.SRP6.CalculatePasswordVerifier(username, password, salt.ToByteArray());
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestCalculateXWithSalt() {
        var data = File.ReadAllLines($"{TestDataRoot}/x_salt.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var salt = split[0];
            
            var expected = split[1];
            var result = Corelib.Cryptography.SRP6.CalculateX("USERNAME123", "PASSWORD123", salt.ToByteArray());
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestCalculateXWithValues() {
        var data = File.ReadAllLines($"{TestDataRoot}/x_values.txt");
        Assert.That(data, Is.Not.Empty);

        var salt = "CAC94AF32D817BA64B13F18FDEDEF92AD4ED7EF7AB0E19E9F2AE13C828AEAF57".ToByteArray();
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var username = split[0];
            var password = split[1];
            
            var expected = split[2];
            var result = Corelib.Cryptography.SRP6.CalculateX(username, password, salt);
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestCalculateServerPublicKeys() {
        var data = File.ReadAllLines($"{TestDataRoot}/b_values.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var verifier = split[0].ToByteArray();
            var private_key = split[1].ToByteArray();
            
            var expected = split[2];
            var result = Corelib.Cryptography.SRP6.CalculateServerPublicKey(verifier, private_key);
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestCalculateClientSessionValues() {
        var data = File.ReadAllLines($"{TestDataRoot}/client_s_values.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var clientPrivateKey = split[0].ToByteArray();
            var serverPublicKey = split[1].ToByteArray();
            var x = split[2].ToByteArray();
            var u = split[3].ToByteArray();
            
            var expected = split[4];
            var result = Corelib.Cryptography.SRP6.CalculateClientSessionValue(serverPublicKey, clientPrivateKey, x, u, Corelib.Cryptography.SRP6.LARGE_SAFE_PRIME_LE, 7);
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestCalculateServerSessionValues() {
        var data = File.ReadAllLines($"{TestDataRoot}/server_s_values.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var clientPublicKey = split[0].ToByteArray();
            var passwordVerifier = split[1].ToByteArray();
            var u = split[2].ToByteArray();
            var serverPrivateKey = split[3].ToByteArray();
            
            var expected = split[4];
            var result = Corelib.Cryptography.SRP6.CalculateServerSessionValue(clientPublicKey, passwordVerifier, u, serverPrivateKey);
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestCalculateScramblers() {
        var data = File.ReadAllLines($"{TestDataRoot}/u_values.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var clientPublicKey = split[0].ToByteArray();
            var serverPublicKey = split[1].ToByteArray();
            
            var expected = split[2];
            var result = Corelib.Cryptography.SRP6.CalculateScrambler(clientPublicKey, serverPublicKey);
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestSplitSessionValues() {
        var data = File.ReadAllLines($"{TestDataRoot}/split_s_values.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var sessionKey = split[0].ToByteArray();

            var expected = split[1];
            var result = Corelib.Cryptography.SRP6.SplitSessionValue(sessionKey);
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestSHAInterleaveSessionValues() {
        var data = File.ReadAllLines($"{TestDataRoot}/interleaved_values.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var sessionKey = split[0].ToByteArray();

            var expected = split[1];
            var result = Corelib.Cryptography.SRP6.SHAInterleave(sessionKey);
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestCalculateServerSessionKeys() {
        var data = File.ReadAllLines($"{TestDataRoot}/server_session_keys.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var clientPublicKey = split[0].ToByteArray();
            var passwordVerifier = split[1].ToByteArray();
            var serverPrivateKey = split[2].ToByteArray();
            
            var expected = split[3];
            var serverPublicKey = Corelib.Cryptography.SRP6.CalculateServerPublicKey(passwordVerifier, serverPrivateKey);
            var result = Corelib.Cryptography.SRP6.CalculateServerSessionKey(clientPublicKey, serverPublicKey, passwordVerifier, serverPrivateKey);
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
    
    [Test]
    public void TestCalculateClientSessionKeys() {
        var data = File.ReadAllLines($"{TestDataRoot}/client_session_keys.txt");
        Assert.That(data, Is.Not.Empty);
        
        foreach (var line in data) {
            var split = line.Split(" ");
            var username = split[0];
            var password = split[1];
            var serverPublicKey = split[2].ToByteArray();
            var clientPrivateKey = split[3].ToByteArray();
            var generator = byte.Parse(split[4]);
            var largeSafePrime = split[5].ToByteArray();
            var clientPublicKey = split[6].ToByteArray();
            var salt = split[7].ToByteArray();
            
            var expected = split[8];
            var result = Corelib.Cryptography.SRP6.CalculateClientSessionKey(
                username, password,
                serverPublicKey,
                clientPrivateKey,
                clientPublicKey, salt,
                largeSafePrime, generator
            );
            
            Assert.That(result.Reverse().ToArray().ToHexString(true), Is.EqualTo(expected));
        }
    }
}
