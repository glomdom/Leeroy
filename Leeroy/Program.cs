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

using Akka.Actor;
using Leeroy.Corelib.Common;

namespace Leeroy; 

internal static class Program {
    private static string RevisionName = "Azeroth";

    private static ActorSystem _actorSystem = null!;
    
    private static void Main() {
        PrintTitle();
        
        Logger.Information("Starting {0}..", Logger.Args("Akka.NET"));
        if (!AkkaConfig.CreateActorSystem("leeroy", out var system)) {
            Logger.Fatal("Failed to create {0} root system. Check above for error messages.", Logger.Args("Akka.NET"));
            Console.ReadKey();

            return;
        }
        
        // TODO: Start login server.

        _actorSystem = system;
        Logger.Information("{0} system created.", Logger.Args("Akka.NET"));
    }

    private static void PrintTitle() {
        Console.WriteLine( @"    _                                                                  ");
        Console.WriteLine($@"   | |     ___   ___  _ __  ___   _   _       Revision: {RevisionName} ");
        Console.WriteLine( @"   | |    / _ \ / _ \| '__|/ _ \ | | | |                               ");
        Console.WriteLine( @"   | |___|  __/|  __/| |  | (_) || |_| |      WoW: Clasic's best.      ");
        Console.WriteLine( @"   |_____|\___| \___||_|   \___/  \__, |                               ");
        Console.WriteLine( @" -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-  |___/ -=-                            ");
        Console.WriteLine();
    }
}