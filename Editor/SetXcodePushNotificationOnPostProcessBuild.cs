using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Kogane
{
    public static class SetXcodePushNotificationOnPostProcessBuild
    {
        public static Func<bool> OnIsProduction { get; set; }

        [PostProcessBuild]
        private static void OnPostprocessBuild( BuildTarget buildTarget, string path )
        {
            if ( buildTarget != BuildTarget.iOS ) return;

            const string developmentEntitlementGuid = "67801e0dc6d9e4562ba9a76315d28d4e";
            const string productionEntitlementGuid  = "e102cf88fd74fa24f819f81ab17347f3";
            const string targetName                 = "Unity-iPhone";

            var project     = new PBXProject();
            var projectPath = PBXProject.GetPBXProjectPath( path );

            project.ReadFromFile( projectPath );

            var entitlementGuid = OnIsProduction != null && OnIsProduction()
                    ? productionEntitlementGuid
                    : developmentEntitlementGuid
                ;

            var sourceEntitlementFilePath      = AssetDatabase.GUIDToAssetPath( entitlementGuid );
            var applicationIdentifier          = PlayerSettings.GetApplicationIdentifier( NamedBuildTarget.iOS );
            var entitlementFileName            = $"{Path.GetExtension( applicationIdentifier ).Remove( 0, 1 )}.entitlements";
            var destinationEntitlementFilePath = $"{path}/{targetName}/{entitlementFileName}";

            File.Copy( sourceEntitlementFilePath, destinationEntitlementFilePath, true );

            var entitlementFilePath = $"{targetName}/{entitlementFileName}";

            project.AddFile( entitlementFilePath, entitlementFileName );

            var targetGuid = project.GetUnityMainTargetGuid();

            project.AddBuildProperty( targetGuid, "CODE_SIGN_ENTITLEMENTS", entitlementFilePath );
            project.AddFrameworkToProject( targetGuid, "UserNotifications.framework", false );

            var capabilityManager = new ProjectCapabilityManager( projectPath, entitlementFilePath, targetName );

            project.AddCapability( targetGuid, PBXCapabilityType.BackgroundModes );
            capabilityManager.AddBackgroundModes( BackgroundModesOptions.RemoteNotifications );
            capabilityManager.WriteToFile();
            project.WriteToFile( projectPath );
        }
    }
}