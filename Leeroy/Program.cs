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

var result = SRP6.CalculateServerProof(
    "BFD1AC65C8DAAAD88BF9DFF9AF8D1DCDF11DFD0C7E398EDCDF5DBBD08EFB39D3".ToByteArray(),
    "7EBBC190D9AB2DC0CD891372CB30DF1ED35CDA1E".ToByteArray(),
    "4876E68F9FCCB6CA9BC9C9BCEBDB36F2358B6EAD0F17881D811891A9888E8E5B10E1162CE8B58293".ToByteArray()
);

Console.WriteLine(result.Reverse().ToArray().ToHexString(true));