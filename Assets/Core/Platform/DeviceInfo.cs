using UnityEngine;
using UnityEngine.XR;

namespace VRTraining.Core.Platform
{
    public static class DeviceInfo
    {
        public static bool IsVr => XRSettings.isDeviceActive;

        public static string PlatformName => IsVr ? "VR" : "PC";

        public static string DeviceName
        {
            get
            {
                if (!IsVr)
                {
                    return SystemInfo.deviceName;
                }

                var head = InputDevices.GetDeviceAtXRNode(XRNode.Head);

                return head.isValid && !string.IsNullOrWhiteSpace(head.name) ? head.name : XRSettings.loadedDeviceName;
            }
        }
    }
}