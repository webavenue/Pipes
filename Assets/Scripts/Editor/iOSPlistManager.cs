
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_IOS
using UnityEngine.iOS;
using UnityEditor.iOS.Xcode;
using UnityEditor;
using UnityEditor.Callbacks;
#endif

public class iOSPlistManager : MonoBehaviour
{
#if UNITY_IOS
    [PostProcessBuild]
    static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Read plist
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Update value
            PlistElementDict rootDict = plist.root;

            rootDict.values.Remove("UIApplicationExitsOnSuspend");

            PlistElementDict theTransportDict = rootDict.CreateDict("NSAppTransportSecurity");
            theTransportDict.SetBoolean("NSAllowsArbitraryLoads", true);
            rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-4062760191449557~1954897738");
            rootDict.SetString("gad_preferred_webview", "wkwebview");

            rootDict.SetString("NSLocationWhenInUseUsageDescription", "Pipes needs your location for analytics and advertising purposes");
            rootDict.SetString("NSCalendarsUsageDescription", "Pipes needs your calendar to provide personalised advertising experience tailored to you");

            //PlistElementArray queriesSchema = rootDict.CreateArray("LSApplicationQueriesSchemes");
            //queriesSchema.AddString("fb");
            //queriesSchema.AddString("instagram");
            //queriesSchema.AddString("tumblr");
            //queriesSchema.AddString("twitter");

            //rootDict.SetString("NSCalendarsUsageDescription", "Adding events");
            //rootDict.SetString("NSPhotoLibraryUsageDescription", "Taking selfies");
            //rootDict.SetString("NSCameraUsageDescription", "Taking selfies");
            //rootDict.SetString("NSMotionUsageDescription", "Interactive ad controls");

            // Write plist
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
#endif
}