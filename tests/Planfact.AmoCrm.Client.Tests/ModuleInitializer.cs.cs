using System.Runtime.CompilerServices;

namespace Planfact.AmoCrm.Client.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.InitializePlugins();
    }
}
