﻿using Comfort.Common;
using EFT;
using SIT.Core.Coop;
using SIT.Tarkov.Core.PlayerPatches.Health;
using System;
using System.Linq;
using System.Reflection;

namespace SIT.Tarkov.Core
{
    public class OfflineSaveProfile : ModulePatch
    {


        static OfflineSaveProfile()
        {
            // compile-time check
            //_ = nameof(ClientMetrics.Metrics);

            //_ = nameof(TarkovApplication);
            //_ = nameof(EFT.RaidSettings);

            //_defaultJsonConverters = Traverse.Create(converterClass).Field<JsonConverter[]>("Converters").Value;
        }

        protected override MethodBase GetTargetMethod()
        {
            foreach (var method in PatchConstants.GetAllMethodsForType(typeof(TarkovApplication)))
            {
                if (method.Name.StartsWith("method") &&
                    method.GetParameters().Length >= 3 &&
                    method.GetParameters()[0].Name == "profileId" &&
                    method.GetParameters()[1].Name == "savageProfile" &&
                    method.GetParameters()[2].Name == "location" &&
                    method.GetParameters().Any(x => x.Name == "result") &&
                    method.GetParameters()[method.GetParameters().Length - 1].Name == "timeHasComeScreenController"
                    )
                {
                    //Logger.Log(BepInEx.Logging.LogLevel.Info, method.Name);
                    return method;
                }
            }
            Logger.Log(BepInEx.Logging.LogLevel.Error, "OfflineSaveProfile::Method is not found!");

            return null;
        }

        [PatchPrefix]
        public static bool PatchPrefix(
            //ref EFT.RaidSettings ____raidSettings
            //, ref Result<EFT.ExitStatus, TimeSpan, object> result
            //)
            //    [PatchPostfix]
            //public static void PatchPostfix(ref ESideType ___esideType_0, ref object result)
            //{
            //    Logger.LogInfo("OfflineSaveProfile::PatchPrefix");
            // isLocal = false;
            //____raidSettings.RaidMode = ERaidMode.Online;

            //var session = ClientAccesor.GetClientApp().GetClientBackEndSession();
            //var isPlayerScav = false;
            //var profile = session.Profile;

            //if (____raidSettings.Side == ESideType.Savage)
            //{
            //    profile = session.ProfileOfPet;
            //    isPlayerScav = true;
            //}

            //var currentHealth = HealthListener.Instance.CurrentHealth;

            //var beUrl = SIT.Tarkov.Core.PatchConstants.GetBackendUrl();
            //var sessionId = SIT.Tarkov.Core.PatchConstants.GetPHPSESSID();

            //SaveProfileProgress(beUrl
            //    , sessionId
            //    , result.Value0
            //    , profile
            //    , currentHealth
            //    , isPlayerScav);

            //return true;

            string profileId, RaidSettings ____raidSettings, TarkovApplication __instance, Result<ExitStatus, TimeSpan, object> result)
        {
            // Get scav or pmc profile based on IsScav value
            var profile = (____raidSettings.IsScav)
                ? __instance.GetClientBackEndSession().ProfileOfPet
                : __instance.GetClientBackEndSession().Profile;

            //SaveProfileRequest request = new SaveProfileRequest
            //{
            //    Exit = result.Value0.ToString().ToLowerInvariant(),
            //    Profile = profile,
            //    Health = Utils.Healing.HealthListener.Instance.CurrentHealth,
            //    IsPlayerScav = ____raidSettings.IsScav
            //};

            //RequestHandler.PutJson("/raid/profile/save", request.ToJson(_defaultJsonConverters.AddItem(new NotesJsonConverter()).ToArray()));
            //var beUrl = SIT.Tarkov.Core.PatchConstants.GetBackendUrl();
            var currentHealth = HealthListener.Instance.CurrentHealth;
            SaveProfileProgress(result.Value0, profile, currentHealth, ____raidSettings.IsScav);


            var coopGC = CoopGameComponent.GetCoopGameComponent();
            if (coopGC != null)
            {
                GameWorld.Destroy(coopGC);
            }

            return true;
        }

        public static void SaveProfileProgress(EFT.ExitStatus exitStatus, EFT.Profile profileData, PlayerHealth currentHealth, bool isPlayerScav)
        {
            SaveProfileRequest request = new SaveProfileRequest
            {
                exit = exitStatus.ToString().ToLower(),
                profile = profileData,
                health = currentHealth,
                //health = profileData.Health,
                isPlayerScav = isPlayerScav
            };

            var convertedJson = request.SITToJson();
            Request.Instance.PostJson("/raid/profile/save", convertedJson);

        }

        public class SaveProfileRequest
        {
            public string exit { get; set; }
            public EFT.Profile profile { get; set; }
            public bool isPlayerScav { get; set; }
            //public PlayerHealth health { get; set; }
            public object health { get; set; }
        }
    }
}
