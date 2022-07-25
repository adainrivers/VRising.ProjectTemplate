using BepInEx.Configuration;

namespace VRising.ProjectTemplate.Config
{
    internal class PluginConfig
    {
        internal static ConfigEntry<bool> RespondToHello => null;

        internal static void Initialize()
        {
            Plugin.Configuration.Bind("Main", "RespondToHello", true, "Determines whether the responds to !hello or not.");
        }

        public static void Destroy()
        {
            Plugin.Configuration.Clear();
        }
    }
}
