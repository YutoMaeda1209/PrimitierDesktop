using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace YuchiGames.PrimitierDesktop.Patches
{
    [HarmonyPatch(typeof(HeightCalibrator), nameof(HeightCalibrator.Initialize))]
    public class HeightCalibrator_Initialize
    {
        public static void Postfix()
        {
            HeightCalibrator heightCalibrator = GameObject.FindObjectOfType<HeightCalibrator>();
            heightCalibrator.Calibrate();
        }
    }

    [HarmonyPatch(typeof(HeightCalibrator), nameof(HeightCalibrator.Calibrate))]
    public class HeightCalibrator_Calibrate
    {
        public static void Postfix()
        {
            HeightCalibrator heightCalibrator = GameObject.FindObjectOfType<HeightCalibrator>();
            heightCalibrator.ShowTitleMenu();
        }
    }
}