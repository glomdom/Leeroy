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

using System.Net;
using Akka.Actor;
using Akka.IO;
using Leeroy.Corelib.Common;
using Leeroy.Corelib.Networking;

namespace Leeroy.Corelib.Login;

public class LoginClient : LoginPacketHandler {
    private readonly IActorRef _connection;

    public LoginClient(EndPoint remoteAddress) {
        _connection = Sender;

        Receive<Tcp.Received>(received => {
            Self.Tell(received.Data);
        });
    }

    [PacketHandler(LoginPackets.CMD_AUTH_LOGON_CHALLENGE)]
    public void HandleAuthLogonChallenge(byte[] data) {
        Logger.Information("Received logon challenge packet");
    }
}
