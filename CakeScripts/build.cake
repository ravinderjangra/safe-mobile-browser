#addin nuget:?package=Cake.Android.Adb&version=3.0.0
#addin nuget:?package=Cake.Android.AvdManager&version=1.0.3
#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.AppleSimulator
#addin nuget:?package=Cake.FileHelpers
#addin "Cake.Powershell"
#tool "nuget:?package=NUnit.ConsoleRunner"


var target = Argument("target", "Default");
var PROJ_SLN_PATH ="../SafeMobileBrowser.sln";
var XAMARIN_UI_TEST_PROJ_PATH = "../SafeMobileBrowser.UITests/SafeMobileBrowser.UITests.csproj";
var TEST_DLL_FILE_PATH = "../SafeMobileBrowser.UITests/bin/Release/SafeMobileBrowser.UITests.dll";

Task("Default")
  .IsDependentOn("Restore-NuGet-Packages")
  .IsDependentOn("Rebuild-UITest")
  .IsDependentOn("Android-UITest")
  //IsDependentOn("iOS-UITest")
  .Does(() =>{ });

Task("Restore-NuGet-Packages")
    .Does(() => {
        NuGetRestore(PROJ_SLN_PATH);
});

Task("Rebuild-UITest")
    .Does(() => {
      MSBuild(XAMARIN_UI_TEST_PROJ_PATH, settings =>
            settings.SetConfiguration("Release")
            .UseToolVersion(MSBuildToolVersion.Default)
            .WithTarget("Rebuild")
            .SetVerbosity(Verbosity.Minimal)
        );
    }); 

var ANDROID_PROJ = "../SafeMobileBrowser/SafeMobileBrowser.Android/SafeMobileBrowser.Android.csproj";
var ANDROID_APK_PATH = "../SafeMobileBrowser/SafeMobileBrowser.Android/bin/Release/net.maidsafe.browser-Signed.apk";
var ANDROID_AVD = "new_test_device";
var ANDROID_PKG_NAME = "net.maidsafe.browser";
var ANDROID_EMU_TARGET = "system-images;android-28;google_apis_playstore;x86_64";
var ANDROID_EMU_DEVICE = "Nexus 6P";
var ANDROID_HOME = EnvironmentVariable("ANDROID_HOME");

Task("Android-UITest")
    .Does(() => {
        var avdSettings = new AndroidAvdManagerToolSettings { SdkRoot = ANDROID_HOME };
        Information("Creating AVD if necessary: {0}...", ANDROID_AVD);
        if (!AndroidAvdListAvds(avdSettings).Any(a => a.Name == ANDROID_AVD))
        {
            AndroidAvdCreate(ANDROID_AVD, ANDROID_EMU_TARGET, ANDROID_EMU_DEVICE, force: true, settings: avdSettings);
            Information("Creating new Android AVD");
        }
        
        var emulatorExt = IsRunningOnWindows() ? ".exe" : "";
        string emulatorPath = "emulator" + emulatorExt;
        if (ANDROID_HOME != null)
        {
            var andHome = new DirectoryPath(ANDROID_HOME);
            if (DirectoryExists(andHome))
            {
                emulatorPath = MakeAbsolute(andHome.Combine("tools").CombineWithFilePath("emulator" + emulatorExt)).FullPath;
                if (!FileExists(emulatorPath))
                    emulatorPath = MakeAbsolute(andHome.Combine("emulator").CombineWithFilePath("emulator" + emulatorExt)).FullPath;
                if (!FileExists(emulatorPath))
                    emulatorPath = "emulator" + emulatorExt;
                Information($"EmulatorPath = {emulatorPath}");    
            }
        }
        var emu = StartAndReturnProcess(emulatorPath, new ProcessSettings
        {
            Arguments = $"-avd {ANDROID_AVD} -no-audio -timezone Europe/Paris  -no-boot-anim"
        });
        var adbSettings = new AdbToolSettings { SdkRoot = ANDROID_HOME };

        var emuSerial = string.Empty;
        for (int i = 0; i < 100; i++)
        {
            foreach (var device in AdbDevices(adbSettings).Where(d => d.Serial.StartsWith("emulator-")))
            {
                if (AdbGetAvdName(device.Serial).Equals(ANDROID_AVD, StringComparison.OrdinalIgnoreCase))
                {
                    emuSerial = device.Serial;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(emuSerial))
                break;
            else
                System.Threading.Thread.Sleep(1000);
        }

        try
        {
            AdbUninstall(ANDROID_PKG_NAME, false, adbSettings);
            Information("Uninstalled old: {0}", ANDROID_PKG_NAME);
        }
        catch 
        { 

        }
        
        MSBuild(ANDROID_PROJ, c => {
            c.Configuration = "Release";
            c.ArgumentCustomization = args=>args.Append("/consoleloggerparameters:ErrorsOnly");
            c.Properties["AdbTarget"] = new List<string> { "-s " + emuSerial };
            c.Targets.Clear();
            c.Targets.Add("Install");
            c.SetVerbosity(Verbosity.Minimal);
        });

        var settings = new NUnit3Settings();
        settings.Test = "SafeMobileBrowser.UITests.Tests(Android)";
        var resultsFile = "AndroidUITestResult.xml";
        settings.Results = new[] { new NUnit3Result { FileName = resultsFile }};
        NUnit3(new [] { TEST_DLL_FILE_PATH }, settings);
        emu.Kill();
    })
.ReportError(exception => {
    Information(exception.Message);
});

var IOS_SIM_NAME = "iPhone X";
var IOS_SIM_RUNTIME = "iOS 12.2";
var IOS_PROJ = "../SafeMobileBrowser/SafeMobileBrowser.iOS/SafeMobileBrowser.iOS.csproj";
var IOS_BUNDLE_ID = "net.maidsafe.browser";
var IOS_IPA_PATH = "../SafeMobileBrowser/SafeMobileBrowser.iOS/bin/iPhoneSimulator/Debug/SafeMobileBrowser.iOS.app";

Task("iOS-UITest")
    .Does(() => {

        MSBuild(IOS_PROJ, c =>
        {
          c.ArgumentCustomization = args=>args.Append("/consoleloggerparameters:ErrorsOnly");
          c.Configuration = "Debug";
          c.Properties["Platform"] = new List<string> { "iPhoneSimulator" };
          c.Properties["BuildIpa"] = new List<string> { "true" };
          c.Targets.Clear();
          c.Targets.Add("Rebuild");
          c.SetVerbosity(Verbosity.Minimal);
        });
    
        // Look for a matching simulator on the system    
        var sim = ListAppleSimulators().First(s => s.Name == IOS_SIM_NAME && s.Runtime == IOS_SIM_RUNTIME);

        // Boot the simulator
        Information("Booting: {0} ({1} - {2} - {3})", sim.Name, sim.Runtime, sim.UDID, sim.Availability);

        if (!sim.State.ToLower().Contains("booted"))
        {
            BootAppleSimulator(sim.UDID);
            Information("boot started");
        }
    
        Information("waiting to boot");
        // Wait for it to be booted
        for (int i = 0; i < 100; i++)
        {
            if (ListAppleSimulators().Any(s => s.UDID == sim.UDID && s.State.ToLower().Contains("booted")))
            {
              break;
            } 
            System.Threading.Thread.Sleep(1000);
        }

        // Install the IPA that was previously built
        var ipaPath = new FilePath(IOS_IPA_PATH);
        Information("Installing: {0}", ipaPath);
        InstalliOSApplication(sim.UDID, MakeAbsolute(ipaPath).FullPath);

        // Launch the IPA
        Information("Launching: {0}", IOS_BUNDLE_ID);
        LaunchiOSApplication(sim.UDID, IOS_BUNDLE_ID);

        var resultsFile = "iOSUITestResult.xml";
        var settings = new NUnit3Settings();
        settings.Test = "SafeMobileBrowser.UITests.Tests(iOS)";
        settings.Results = new[] { new NUnit3Result { FileName = resultsFile }};
        NUnit3(new [] { TEST_DLL_FILE_PATH }, settings);

        // Close up simulators
        Information("Closing Simulator");
        ShutdownAllAppleSimulators();
    })
  .ReportError(exception => {
    Information(exception.Message);
  });



RunTarget(target);