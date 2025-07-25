using UnityEngine.XR;

public static class VRHeadsetDetector
{
    public enum VRHeadset
    {
        Unknown,
        Oculus,
        HTC_Vive,
        None
    }

    public static VRHeadset GetConnectedHeadset()
    {
        string vrDevice = XRSettings.loadedDeviceName;

        return vrDevice switch
        {
            _ when vrDevice.Contains("Oculus") => VRHeadset.Oculus,
            _ when vrDevice.Contains("OpenXR Display") => VRHeadset.HTC_Vive,
            null or "" => VRHeadset.None,
            _ => VRHeadset.Unknown
        };
    }
}
