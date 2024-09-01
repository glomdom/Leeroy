using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Akka.Actor;
using Akka.Configuration;
using Leeroy.Corelib.Common;
using Serilog;

namespace Leeroy;

internal static class AkkaConfig {
    [UnconditionalSuppressMessage("SingleFile", "IL3000:Avoid accessing Assembly file path when publishing as a single file", Justification = "<Solved>")]
    private static readonly string ConfigLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "conf/akka.conf");

    internal static bool CreateActorSystem(string name, out ActorSystem system) {
        system = null!;

        if (!GetAkkaConfiguration(out var config)) {
            return false;
        }

        system = ActorSystem.Create(name, config);
        
        return true;
    }

    private static bool GetAkkaConfiguration(out Config config) {
        Logger.Information("Searching for {0} config file at {1}", Logger.Args("Akka.NET", ConfigLocation));
        config = default!;

        try {
            if (!File.Exists(ConfigLocation)) {
                Logger.Error("{0} not found.", Logger.Args(ConfigLocation));
                
                return false;
            }

            var configContents = File.ReadAllText(ConfigLocation);
            config = ConfigurationFactory.ParseString(configContents);

            return true;
        } catch (Exception e) {
            Logger.Error(e.Message);

            return false;
        }
    }
}
