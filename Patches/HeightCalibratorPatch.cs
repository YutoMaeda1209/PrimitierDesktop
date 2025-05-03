using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace YuchiGames.PrimitierDesktop.Patches
{
    [HarmonyPatch(typeof(HeightCalibrator), nameof(HeightCalibrator.Initialize))]
    static class HeightCalibrator_InitializePatch
    {
        static void Postfix(HeightCalibrator __instance)
        {
            __instance.Calibrate();
        }
    }

    [HarmonyPatch(typeof(HeightCalibrator), nameof(HeightCalibrator.Calibrate))]
    static class HeightCalibrator_CalibratePatch
    {
        static void Postfix(HeightCalibrator __instance)
        {
            __instance.ShowTitleMenu();
        }
    }

    [HarmonyPatch(typeof(HeightCalibrator), nameof(HeightCalibrator.ShowTitleMenu))]
    static class HeightCalibrator_ShowTitleMenuPatch
    {
        static void Postfix()
        {
            Program.FootCollider = GameObject.Find("/Player/XR Origin").GetComponent<PlayerMovement>().footCol;
        }
    }
}