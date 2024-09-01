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

public class LoginServer : ReceiveActor {
    private readonly IActorRef _tcpManager;

    public LoginServer() {
        _tcpManager = Context.System.Tcp();
        _tcpManager.Tell(new Tcp.Bind(Self, new IPEndPoint(IPAddress.Parse("0.0.0.0"), 3724)));

        Receive<Tcp.Connected>(connected => {
            Logger.Information("Received client with ip {0}", Logger.Args(connected.RemoteAddress));

            var handler = Context.ActorOf(Props.Create(() => new LoginClient(connected.RemoteAddress)));
            Sender.Tell(new Tcp.Register(handler));
        });
        
        Receive<Tcp.ConnectionClosed>(closed => {
            Logger.Information("Closed connection");
        });
    }
}
