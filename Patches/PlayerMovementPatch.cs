using HarmonyLib;
using Il2Cpp;

namespace YuchiGames.PrimitierDesktop.Patches
{
    [HarmonyPatch(typeof(PlayerMovement), nameof(PlayerMovement.FixedUpdate))]
    static class PlayerMovement_FixedUpdatePatch
    {
        static bool Prefix()
        {
            return false;
        }
    }
}
