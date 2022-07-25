using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using ProjectM;
using VRising.ProjectTemplate.Config;
using VRising.ProjectTemplate.Hooks;
using Wetstone.API;
using Wetstone.Hooks;

namespace VRising.ProjectTemplate
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    public class Plugin : BasePlugin, IRunOnInitialized
    {
        internal static ManualLogSource Logger { get; private set; }
        internal static ConfigFile Configuration { get; private set; }
        private static Harmony _harmonyInstance;

        public override void Load()
        {
            Logger = Log;
            Configuration = Config;
            PluginConfig.Initialize();

            _harmonyInstance = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmonyInstance.PatchAll(Assembly.GetAssembly(typeof(Plugin)));

            if (VWorld.IsServer)
            {
                ServerEvents.OnServerStartupStateChanged += ServerEvents_OnServerStartupStateChanged;
                Chat.OnChatMessage += Chat_OnChatMessage; ;
            }

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
        }

        public void OnGameInitialized()
        {
            Logger.LogInfo($"Game initialized!");
        }

        public override bool Unload()
        {
            if (VWorld.IsServer)
            {
                ServerEvents.OnServerStartupStateChanged -= ServerEvents_OnServerStartupStateChanged;
                Chat.OnChatMessage -= Chat_OnChatMessage; ;
            }

            _harmonyInstance?.UnpatchSelf();
            PluginConfig.Destroy();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is unloaded!");
            return true;
        }
        
        
        private static void Chat_OnChatMessage(VChatEvent e)
        {
            if (e.Message == "!hello" && PluginConfig.RespondToHello.Value)
            {
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, e.User, $"Hello from {PluginInfo.PLUGIN_NAME}");
            }
        }

        private static void ServerEvents_OnServerStartupStateChanged(ProjectM.LoadPersistenceSystemV2 sender, ProjectM.ServerStartupState.State serverStartupState)
        {
            if (serverStartupState == ServerStartupState.State.SuccessfulStartup)
            {
                Logger.LogInfo($"Server is ready.");
            }
        }
    }
}
