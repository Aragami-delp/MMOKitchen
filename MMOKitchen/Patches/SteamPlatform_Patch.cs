using HarmonyLib;
using UnityEngine;
using Kitchen.Layouts.Modules;
using Kitchen;
using System.Reflection;
using Unity.Entities;
using KitchenLib.Utils;
using System.IO;
using Steamworks;
using Kitchen.NetworkSupport;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Kitchen.Transports;
using Steamworks.Data;
namespace MMOKitchen
{
    [HarmonyPatch(typeof(SteamPlatform), "CreateNewLobby")]
    public class SteamPlatform_Patch
    {
        public static bool Prefix(SteamPlatform __instance, Action<bool, Lobby> callback)
        {
            if (__instance.IsReady)
            {
                SteamMatchmaking.CreateLobbyAsync(12).ContinueWith(delegate(Task<Lobby?> task)
                {
                    if (task.IsCompleted && task.Result != null)
                    {
                        Lobby valueOrDefault = task.Result.GetValueOrDefault();
                        __instance.CurrentInviteLobby = valueOrDefault;
                        //Reflection Stuff
                        MethodInfo performSetPermissions = AccessTools.Method(typeof(SteamPlatform), "PerformSetPermissions");
                        performSetPermissions.Invoke(__instance, new object[] { __instance.Permissions });
						callback(true, valueOrDefault);
                    }else
                    {
                        callback(false, default(Lobby));
                    }
                });
            }
            return false;
        }
    }
}