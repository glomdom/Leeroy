using Leeroy.Corelib;
using Leeroy.Corelib.Cryptography;

// testing

var line = "mk4XsOLEilUi2cPI KoMdFPLfrXOHaYB5 7A9821D032C3F0823CE3778008EE189C34BF0EA89C2C62369D4A45884F6544AF C0E46A6968DBB41E886FF09334CF094BA25C84E68F6C51BAB5437565004390B5 157 DEB12A41D54A702E9496871BE93BE13196F741EBF89AF5B8311FA4DC30C0530F 505726F23D5B432B62AD261C7783D561D33C3AE831ABF1965302CE502C181E8D 51F625B1EC39D23DAF2E6B21ED546C8D11FB349BB21E7F355DDA5A16DD603E28 B1FD090488558C5A759A2E1923BABFAC46EE208470FBC92E89C64F67E58150455C10AE98BC59B6A0";
var split = line.Split(" ");
var username = split[0];
var password = split[1];
var serverPublicKey = split[2].ToByteArray();
var clientPrivateKey = split[3].ToByteArray();
var generator = byte.Parse(split[4]);
var largeSafePrime = split[5].ToByteArray();
var clientPublicKey = split[6].ToByteArray();
var salt = split[7].ToByteArray();
var expected = split[8].ToByteArray();

var result = SRP6.CalculateClientSessionKey(
    username, password,
    serverPublicKey,
    clientPrivateKey,
    clientPublicKey, salt,
    largeSafePrime, generator
);

Console.WriteLine(expected.Reverse().ToArray().ToHexString(true));
Console.WriteLine(result.Reverse().ToArray().ToHexString(true));