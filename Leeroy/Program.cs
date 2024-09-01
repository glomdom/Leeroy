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
using Leeroy.Corelib.Cryptography;

// testing

var result = SRP6.CalculateClientPublicKey(
    "A47DD4CD70DA1B0EF7E1FA8C02DE68AF0CEFCC77ACA287FBC3ADCDE0E7B78FE7".ToByteArray(),
    7,
    SRP6.LARGE_SAFE_PRIME_LE
);

Console.WriteLine(result.Reverse().ToArray().ToHexString(true));