using HarmonyLib;
using Il2Cpp;

namespace YuchiGames.PrimitierDesktop.Patches
{
    [HarmonyPatch(typeof(HeightCalibrator), nameof(HeightCalibrator.Initialize))]
    static class HeightCalibrator_InitializePatch
    {
        static void Postfix(HeightCalibrator __instance)
        {
            //HeightCalibrator heightCalibrator = GameObject.FindObjectOfType<HeightCalibrator>();
            __instance.Calibrate();
        }
    }

    [HarmonyPatch(typeof(HeightCalibrator), nameof(HeightCalibrator.Calibrate))]
    static class HeightCalibrator_CalibratePatch
    {
        static void Postfix(HeightCalibrator __instance)
        {
            //HeightCalibrator heightCalibrator = GameObject.FindObjectOfType<HeightCalibrator>();
            __instance.ShowTitleMenu();
        }
    }
}