using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenSharer : MonoBehaviour
{
    public const string FILE_NAME = "share_screenshot.png";
    public const string FOLDER_NAME = "screenshot";

    public UIManager uiManager;

    public void StartSharing()
    {
        //some animations and stuff
        //soundService.PlayCameraShutter();
        Sounds.instance.PlayCapture();
        VibrationManager.instance.VibrateSuccess();
        StartCoroutine(Share());
    }

    private IEnumerator Share()
    {
        uiManager.endPanel.SetActive(false);
        uiManager.TopBar.SetActive(true);
        AdManager.instance.HideBanner();

        yield return StartCoroutine(Capture());

        uiManager.endPanel.SetActive(true);
        uiManager.TopBar.SetActive(false);
        AdManager.instance.ShowBannerIfEnabled(uiManager.levelManger.levelNum);

        ShareToSocial();

    }
    private IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();

        string screenshotName = FILE_NAME;
        string screenShotPath = Path.Combine(Application.persistentDataPath, FOLDER_NAME);
        screenShotPath = Path.Combine(screenShotPath, screenshotName);

        if (File.Exists(screenShotPath)) File.Delete(screenShotPath);
        else Directory.CreateDirectory(Path.GetDirectoryName(screenShotPath));

        int resWidth = Screen.width, resHeight = Screen.height;

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 0);
        rt.depth = 24;
        Camera.main.targetTexture = rt;

        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();

        File.WriteAllBytes(screenShotPath, bytes);
    }

    private void ShareToSocial()
    {
#if !UNITY_EDITOR
        new NativeShare ().AddFile (GetPath ()).SetSubject ("").SetText ("").Share ();
#endif
    }

    private static string GetPath()
    {
        string screenshotName = FILE_NAME;
        string screenShotPath = Path.Combine(Application.persistentDataPath, FOLDER_NAME);
        return Path.Combine(screenShotPath, screenshotName);
    }

}
