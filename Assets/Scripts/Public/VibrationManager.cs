using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        //MMVibrationManager.iOSInitializeHaptics();
    }

    public void VibrateHeavy()//Stars in Eureka
    {
        if (Manager.instance.isHapticEnabled)
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }

    public void VibrateTouch()
    {
        if (Manager.instance.isHapticEnabled)
            MMVibrationManager.Haptic(HapticTypes.Selection, true);
    }

    public void VibrateSuccess()
    {
        if (Manager.instance.isHapticEnabled)
            MMVibrationManager.Haptic(HapticTypes.Success, true);
    }

    public void VibrateFailure()
    {
        if (Manager.instance.isHapticEnabled)
            MMVibrationManager.Haptic(HapticTypes.Failure, true);
    }

    public void VibrateWarning()
    {
        if (Manager.instance.isHapticEnabled)
        {
            MMVibrationManager.Haptic(HapticTypes.Warning, true);
        }
    }

    public void VibrateHold()
    {
        if (Manager.instance.isHapticEnabled)
        {
            MMVibrationManager.Haptic(HapticTypes.MediumImpact, true);
        }
    }
}
