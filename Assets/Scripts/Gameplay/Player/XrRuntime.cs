using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace VRTraining.Gameplay.Player
{
    public static class XrRuntime
    {
        public static bool IsRunning()
        {
            var settings = XRGeneralSettings.Instance;
            if (settings != null && settings.Manager != null && settings.Manager.activeLoader != null)
            {
                return true;
            }

            // На случай загрузчика поднятого вне XR Management
            return XRSettings.isDeviceActive;
        }
    }
}
