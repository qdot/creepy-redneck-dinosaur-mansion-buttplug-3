using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using System;
using Buttplug.Client;
using System.Timers;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Fleece;


namespace CreepyRedneckDinosaurMansionButtplug3
{
    [BepInPlugin("com.nonpolynomial.cbrdm3", "Creepy Redneck Dinosaur Mansion Buttplug 3", "0.0.1.0")]
    [BepInProcess("CRDM3 Demo.exe")]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("com.nonpolynomial.cbrdm3");
        internal static ManualLogSource Log;
        internal static ButtplugClient client;
        internal static Timer timer;
        internal static bool quitting = false;
        internal static bool playerStarted = false;
        internal static bool playerInCombo = true;

        private void Awake()
        {
            harmony.PatchAll();
            Log = base.Logger;
            Log.LogInfo($"Plugin com.nonpolynomial.cbrdm3 is loaded!");
            client = new ButtplugClient("Creepy Redneck Dinosaur Mansion Buttplug 3");

            var task = Task.Run(async () => {
                while (true)
                {
                    if (quitting)
                    {
                        return;
                    }
                    if (!client.Connected)
                    {
                        Log.LogInfo("Trying to connect Buttplug Client");
                        try
                        {
                            await client.ConnectAsync(new ButtplugWebsocketConnector(new Uri("ws://127.0.0.1:12345")));
                            Log.LogInfo("Buttplug Client connected");
                        }
                        catch (Exception ex)
                        {
                            Log.LogError(ex);
                        }
                    }

                    // Wait for 5 seconds then try connecting again.
                    await Task.Delay(5000);
                }
            });

            Log.LogInfo("Should've run task now.");
            Application.quitting += OnApplicationQuit;
            
            timer = new Timer();
            timer.Interval = 2000;
            timer.Elapsed += OnTimerElapsed;
        }

        static void OnApplicationQuit()
        {
            Log.LogInfo("Application quitting, spinning down buttplug client");
            quitting = true;
            if (client != null)
            {
                Task.Run(async () =>
                {
                    if (client.Connected)
                    {
                        await client.DisconnectAsync();
                    }
                    client = null;
                });
            }
        }

        static void OnTimerElapsed(object o, EventArgs e)
        {
            Log.LogInfo("Stopping devices");
            Task.Run(async () => await client.StopAllDevicesAsync());
            timer.Stop();
        }

        [HarmonyPatch(typeof(UICutsceneEncounter), "Update")]
        [HarmonyPostfix]
        static private void PatchUpdateDialogueBox(ref UICutsceneEncounter __instance)
        {
            var t = Traverse.Create(__instance);
            var f = t.Field("text");
            var p = f.GetValue<TMP_Text>();
            if (p.text.Contains("dinosaur"))
            {
                p.text = p.text.Replace("dinosaur", "buttplug");
                Log.LogInfo(p.text);
                f.SetValue(p);
                Log.LogInfo("New: " + Traverse.Create(__instance).Field("text").GetValue<TMP_Text>().text);
            }
        }

        [HarmonyPatch(typeof(Match3Board), "StartPlayerState")]
        [HarmonyPostfix]
        static private void PatchStartPlayerState()
        {
            playerInCombo = true;
        }


        [HarmonyPatch(typeof(Match3Board), "CheckMatches")]
        [HarmonyPostfix]
        static private void PatchTakeTurn(ref Match3Board __instance, ref bool __result)
        {
            if (!__instance.IsPlayerTurn() || !playerInCombo)
            {
                return;
            }
                if (!__result)
                {
                    Log.LogInfo("Player combo ended.");
                    playerInCombo = false;
                    OnTimerElapsed(null, null);
                    timer.Stop();
                    return;
                }
                else
                {
                    Log.LogInfo($"Player had matches: {__result}");
                    Log.LogInfo(__instance.state);
                }

                if (!timer.Enabled)
                {
                    if (client.Devices != null)
                    {
                        foreach (var d in client.Devices)
                        {
                            if (d.VibrateAttributes.Count > 0)
                            {
                                d.VibrateAsync(1.0);
                            }
                        if (d.RotateAttributes.Count > 0)
                        {
                            d.RotateAsync(1.0, true);
                        }
                    }
                    }
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                    timer.Start();
                }
        }
    }
}
