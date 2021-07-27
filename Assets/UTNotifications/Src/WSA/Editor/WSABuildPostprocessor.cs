#if UNITY_WSA || UNITY_METRO

using UnityEngine;
using UnityEditor;
#if !ENABLE_IL2CPP || ENABLE_WINMD_SUPPORT
using System;
using System.IO;
using System.Xml;
#endif

namespace UTNotifications
{
    class WSABuildPostprocessor
    {
    //public
        [UnityEditor.Callbacks.PostProcessBuildAttribute(0)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target == BuildTarget.WSAPlayer)
            {
#if ENABLE_IL2CPP && !ENABLE_WINMD_SUPPORT
                Debug.LogWarning("UTNotifications: Unfortunately IL2CPP Scripting Backend is not supported on Windows Store as Unity doesn't support .winmd libraries in that configuration. Dummy UTNotifications.Manager implementation is used.");
#else
                bool manifestPatched = false, vcxprojPatched = false;
                Patch(Path.Combine(pathToBuiltProject, PlayerSettings.productName), ref manifestPatched, ref vcxprojPatched);
                Patch(Path.Combine(pathToBuiltProject, PlayerSettings.productName + "/" + PlayerSettings.productName + ".Windows"), ref manifestPatched, ref vcxprojPatched);
                Patch(Path.Combine(pathToBuiltProject, PlayerSettings.productName + "/" + PlayerSettings.productName + ".WindowsPhone"), ref manifestPatched, ref vcxprojPatched);

                if (!manifestPatched)
                {
                    Debug.LogError("UTNotifications: Failed to patch the manifest file. Notifications functionality will likely fail to work.");
                }

                if (!vcxprojPatched)
                {
                    Debug.LogError("UTNotifications: Failed to patch the .vcxproj file. Push notifications functionality will likely fail to work, scheduled repeated notifications can work incorrectly.");
                }

                if (EditorUserBuildSettings.wsaUWPBuildType != WSAUWPBuildType.XAML)
                {
                    // See https://docs.unity3d.com/ScriptReference/WSA.Application-arguments.html
                    Debug.LogWarning("UTNotifications: XAML build type is required to handle clicked notifications in UWP/WSA builds.\nSee also: https://docs.unity3d.com/ScriptReference/WSA.Application-arguments.html");
                }
#endif
            }
        }

    //private
#if !ENABLE_IL2CPP || ENABLE_WINMD_SUPPORT
        private static void Patch(string versionPath, ref bool manifestPatched, ref bool vcxprojPatched)
        {
            manifestPatched |= PatchManifest(Path.Combine(versionPath, "Package.appxmanifest"));
            vcxprojPatched |= PatchVcxproj(Path.Combine(versionPath, PlayerSettings.productName + ".vcxproj"));
        }

        private static bool PatchManifest(string manifestFileName)
        {
            if (!File.Exists(manifestFileName))
            {
                return false;
            }

            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(manifestFileName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            XmlNode packageNode = XmlUtils.FindChildNode(xmlDocument, "Package");
            XmlNode applicationsNode = XmlUtils.FindChildNode(packageNode, "Applications");
            XmlNode applicationNode = XmlUtils.FindChildNode(applicationsNode, "Application");

            string ns;
            XmlNode previous;
            if (XmlUtils.FindElement(out previous, applicationNode, "uap:VisualElements") != null)
            {
                //Windows 10 Universal Build
                ns = "uap";
                PatchLockScreen(xmlDocument, packageNode, applicationNode, ns);
            }
            else if (XmlUtils.FindElement(out previous, applicationNode, "m2:VisualElements") != null)
            {
                //Windows manifest
                ns = "m2";
                PatchLockScreen(xmlDocument, packageNode, applicationNode, ns);
            }
            else if (XmlUtils.FindElement(out previous, applicationNode, "m3:VisualElements") != null)
            {
                //Windows Phone manifest
                ns = "m3";
            }
            else if (XmlUtils.FindElement(out previous, applicationNode, "VisualElements") != null)
            {
#if UNITY_METRO_8_0 || UNITY_WSA_8_0
                //Windows 8.0 manifest (Unity 4.x generated)
                ns = null;
                PatchLockScreen(xmlDocument, packageNode, applicationNode, ns);
#else
                //Windows Phone manifest (Unity 4.x generated)
                ns = null;
#endif
            }
            else
            {
                throw new Exception(manifestFileName + " doesn't contain VisualElements node");
            }

            PatchIdentity(xmlDocument, packageNode);
            PatchCapabilities(xmlDocument, packageNode, applicationNode, ns);
            PatchExtensions(xmlDocument, packageNode, applicationNode);

            xmlDocument.Save(manifestFileName);

            DeleteInvalidXmlns(manifestFileName);

            return true;
        }

        private static bool PatchVcxproj(string vcxprojFileName)
        {
            // Makes sure UTNotifications.winmd is referenced in the project
            // (See https://answers.unity.com/questions/1623791/uwp-il2cpp-and-windows-background-task-not-work.html)
            // <Project>
            //   <ItemGroup>
            //     <Reference Include="UTNotifications">
            //       <HintPath>{ProjectRoot}\Assets\Plugins\WSA\UTNotifications.winmd</HintPath>
            //       <IsWinMDFile>true</IsWinMDFile>
            //     </Reference>
            //   </ItemGroup>
            // </Project>

            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) != ScriptingImplementation.IL2CPP)
            {
                // No need to patch anything
                return true;
            }

            if (!File.Exists(vcxprojFileName))
            {
                return false;
            }

            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(vcxprojFileName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            XmlNode projectNode = XmlUtils.FindChildNode(xmlDocument, "Project");
            if (projectNode == null)
            {
                Debug.LogError("Failed to find <Project> node");
                return false;
            }

            // Check if it's already referenced
            {
                XmlNode it = projectNode.FirstChild;
                while (it != null)
                {
                    if (it.Name.Equals("ItemGroup"))
                    {
                        XmlElement referenceNode = XmlUtils.FindChildNode(it, "Reference") as XmlElement;
                        if (referenceNode != null && referenceNode.GetAttribute("Include") == "UTNotifications")
                        {
                            // Already patched
                            return true;
                        }
                    }
                    it = it.NextSibling;
                }
            }

            XmlNode itemGroup = xmlDocument.CreateElement("ItemGroup");
            XmlNode reference = XmlUtils.UpdateOrCreateElement(xmlDocument, itemGroup, "Reference", "Include", null, "UTNotifications");
            XmlUtils.UpdateOrCreateElement(xmlDocument, reference, "HintPath", null, null, Settings.WSAWinmdPluginPath);
            XmlUtils.UpdateOrCreateElement(xmlDocument, reference, "IsWinMDFile", null, null, "true");
            projectNode.AppendChild(itemGroup);

            xmlDocument.Save(vcxprojFileName);

            DeleteInvalidXmlns(vcxprojFileName);

            return true;
        }

        private static void DeleteIfExists(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        private static void PatchIdentity(XmlDocument xmlDocument, XmlNode packageNode)
        {
            if (Settings.Instance.PushNotificationsEnabledWindows)
            {
                XmlNode previous;
                XmlElement identityNode = XmlUtils.FindElement(out previous, packageNode, "Identity");
                string identityName = Settings.Instance.WindowsIdentityName;

                if (!string.IsNullOrEmpty(identityName) && identityName != PlayerSettings.WSA.applicationDescription)
                {
                    identityNode.SetAttribute("Name", identityName);
                }
                else
                {
                    identityName = identityNode.GetAttribute("Name");
                }

                if (string.IsNullOrEmpty(identityName) || identityName == PlayerSettings.WSA.applicationDescription)
                {
                    Debug.LogWarning("Please specify Windows Store Identity Name in the UTNotifications Settings!");
                }

                string publisher = identityNode.GetAttribute("Publisher").Replace("CN=", "");
                if (!Settings.Instance.WindowsCertificateIsCorrect(publisher))
                {
                    Debug.LogWarning(Settings.WRONG_CERTIFICATE_MESSAGE);
                }
            }
        }

        private static void PatchLockScreen(XmlDocument xmlDocument, XmlNode packageNode, XmlNode applicationNode, string ns)
        {
            //<Package>
            //  <Applications>
            //    <Application>
            //      <m2/uap:VisualElements ToastCapable="true">
            //        <m2/uap:LockScreen Notification="badge" BadgeLogo="Assets\MediumTile.png" />
            //      </m2/uap:VisualElements>
            //    </Application>
            //  </Applications>
            //</Package>

            XmlNode previous;
            XmlElement visualElementsNode = XmlUtils.FindElement(out previous, applicationNode, ns != null ? ns + ":VisualElements" : "VisualElements");
            XmlElement lockScreenNode = XmlUtils.UpdateOrCreateElement(xmlDocument, visualElementsNode, ns != null ? ns + ":LockScreen" : "LockScreen", null, null, null, null, ns != null ? packageNode.GetNamespaceOfPrefix(ns) : null);
            if (string.IsNullOrEmpty(lockScreenNode.GetAttribute("Notification")))
            {
                lockScreenNode.SetAttribute("Notification", "badge");
            }
            if (string.IsNullOrEmpty(lockScreenNode.GetAttribute("BadgeLogo")))
            {
                string badgeLogo = visualElementsNode.GetAttribute("Square150x150Logo");
                if (string.IsNullOrEmpty(badgeLogo))
                {
                    badgeLogo = visualElementsNode.GetAttribute("Logo");
                }
                lockScreenNode.SetAttribute("BadgeLogo", badgeLogo);
            }
        }

        private static void PatchCapabilities(XmlDocument xmlDocument, XmlNode packageNode, XmlNode applicationNode, string ns)
        {
            //<Package>
            //  <Applications>
            //    <Application>
            //      <m3:VisualElements ToastCapable="true">
            //      </m3:VisualElements>
            //    </Application>
            //  </Applications>
            //  <Capabilities>
            //    <Capability Name="internetClientServer" /> / <Capability Name="internetClient" />
            //  </Capabilities>
            //</Package>

            XmlNode previous;
            XmlElement visualElementsNode = XmlUtils.FindElement(out previous, applicationNode, ns == null ? "VisualElements" : ns + ":VisualElements");
            if (ns != "uap")
            {
                visualElementsNode.SetAttribute("ToastCapable", "true");
            }

            if (Settings.Instance.PushNotificationsEnabledWindows)
            {
                XmlElement capabilitiesNode = XmlUtils.UpdateOrCreateElement(xmlDocument, packageNode, "Capabilities");

#if UNITY_METRO_8_0 || UNITY_WSA_8_0
                string requiredCapability = "internetClient";
#else
                string requiredCapability = ((ns == null || ns == "m3") ? "internetClientServer" : "internetClient");
#endif
                XmlUtils.UpdateOrCreateElement(xmlDocument, capabilitiesNode, "Capability", "Name", null, requiredCapability);
            }
        }

        private static void PatchExtensions(XmlDocument xmlDocument, XmlNode packageNode, XmlNode applicationNode)
        {
            //<Package>
            //  <Applications>
            //    <Applications>
            //      <Extensions>
            //        <Extension Category="windows.backgroundTasks" EntryPoint="UTNotifications.WSA.BackgroundTask">
            //          <BackgroundTasks>
            //            <Task Type="systemEvent"/>
            //          </BackgroundTasks>
            //        </Extension>
            //        <Extension Category="windows.backgroundTasks" EntryPoint="UTNotifications.WSA.PushBackgroundTask">
            //          <BackgroundTasks>
            //            <Task Type="pushNotification"/>
            //          </BackgroundTasks>
            //        </Extension>
            //      </Extensions>
            //    </Application>
            //  </Applications>
            //</Package>

            //<Extensions>
            XmlElement extensionsNode = XmlUtils.UpdateOrCreateElement(xmlDocument, applicationNode, "Extensions");

            //<Extension Category="windows.backgroundTasks" EntryPoint="UTNotifications.WSA.BackgroundTask">
            //  <BackgroundTasks>
            //    <Task Type="systemEvent"/>
            //  </BackgroundTasks>
            //</Extension>
            {
                XmlElement extensionNode = XmlUtils.UpdateOrCreateElement(xmlDocument, extensionsNode, "Extension", "EntryPoint", null, "UTNotifications.WSA.BackgroundTask");
                extensionNode.SetAttribute("Category", "windows.backgroundTasks");
                XmlElement backgroundTasksNode = XmlUtils.UpdateOrCreateElement(xmlDocument, extensionNode, "BackgroundTasks");
                XmlUtils.UpdateOrCreateElement(xmlDocument, backgroundTasksNode, "Task", "Type", null, "systemEvent");
            }

            //<Extension Category="windows.backgroundTasks" EntryPoint="UTNotifications.WSA.PushBackgroundTask">
            //  <BackgroundTasks>
            //    <Task Type="pushNotification"/>
            //  </BackgroundTasks>
            //</Extension>
            if (Settings.Instance.PushNotificationsEnabledWindows)
            {
                XmlElement extensionNode = XmlUtils.UpdateOrCreateElement(xmlDocument, extensionsNode, "Extension", "EntryPoint", null, "UTNotifications.WSA.PushBackgroundTask");
                extensionNode.SetAttribute("Category", "windows.backgroundTasks");
                XmlElement backgroundTasksNode = XmlUtils.UpdateOrCreateElement(xmlDocument, extensionNode, "BackgroundTasks");
                XmlUtils.UpdateOrCreateElement(xmlDocument, backgroundTasksNode, "Task", "Type", null, "pushNotification");
            }
            else
            {
                XmlUtils.RemoveElement(xmlDocument, extensionsNode, "Extension", "EntryPoint", null, "UTNotifications.WSA.PushBackgroundTask");
            }
        }

        private static void DeleteInvalidXmlns(string manifestFileName)
        {
            string contents = File.ReadAllText(manifestFileName);
            contents = contents.Replace(" xmlns=\"\"", "");
            File.WriteAllText(manifestFileName, contents);
        }
#endif // !ENABLE_IL2CPP || ENABLE_WINMD_SUPPORT
        }
    }
#endif