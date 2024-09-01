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

using System.Reflection;
using Akka.Actor;
using Akka.IO;
using Leeroy.Corelib.Common;

namespace Leeroy.Corelib.Networking;

public class LoginPacketHandler : ReceiveActor {
    private readonly Dictionary<LoginPackets, Action<byte[]>> _loginPacketHandlers = [];

    protected LoginPacketHandler() {
        RegisterLoginPacketHandlers();
    }

    private void RegisterLoginPacketHandlers() {
        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(m => m.GetCustomAttributes(typeof(PacketHandlerAttribute), false).Length > 0);

        foreach (var method in methods) {
            var attribute = method.GetCustomAttribute<PacketHandlerAttribute>();
            
            if (attribute is not { Packet: LoginPackets loginPacketType })
                continue;

            var action = (Action<byte[]>)Delegate.CreateDelegate(typeof(Action<byte[]>), this, method);

            if (!_loginPacketHandlers.TryAdd(loginPacketType, action)) {
                Logger.Error("Duplicate login packet handler found for packet {0}", Logger.Args(loginPacketType));
            }
        }

        Receive<ByteString>(HandleLoginPacket);
    }

    private void HandleLoginPacket(ByteString data) {
        var opcode = (LoginPackets)data[0];

        if (_loginPacketHandlers.TryGetValue(opcode, out var handler)) {
            handler.Invoke(data.ToArray().Skip(1).ToArray());
        } else {
            Logger.Warning("Unhandled login packet {0}", Logger.Args(opcode));
        }
    }
}
