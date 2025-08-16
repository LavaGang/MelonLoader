#if ANDROID
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using MelonLoader.Java;
using MelonLoader.Utils;

namespace MelonLoader.Bootstrap.Proxy.Android;

public static class AndroidBootstrap
{
    public static string? PackageName { get; private set; }
    public static string? DotnetDir { get; private set; }
    public static int ApiLevel { get; private set; } = 0;

    [RequiresDynamicCode("Calls Init then InitConfig ")]
    public static unsafe int LoadBootstrap()
    {
        // linux-bionic .NET logs everything to stdout/err, this allows us to see these logs in logcat with our logs
        StdRedirect.RedirectStdOut();
        StdRedirect.RedirectStdErr();

        Core.Init(NativeLibrary.Load("libmain.so"));
        return 1;
    }

    public static void CopyMelonLoaderData(DateTimeOffset date)
    {
        CopyAssetFolder("MelonLoader", Core.DataDir, Core.DataDir, date);
        CopyAssetFolder("dotnet", $"/data/data/{PackageName}/", $"/data/data/{PackageName}/", date);

        DotnetDir = Path.Combine($"/data/data/{PackageName}/dotnet");
    }

    private static void CopyAssetFolder(string assetName, string copyBase, string targetBase, DateTimeOffset cutoffDate)
    {
        string targetFolder = Path.Combine(targetBase, assetName);
        string markerFile = Path.Combine(targetFolder, ".copy_complete");

        bool needsCopy = true;

        if (Directory.Exists(targetFolder))
        {
            if (File.Exists(markerFile))
            {
                DateTime lastComplete = File.GetLastWriteTimeUtc(markerFile);
                if (lastComplete > cutoffDate)
                {
                    AndroidProxy.Log($"{assetName} folder is already up-to-date");
                    needsCopy = false;
                }
            }
        }

        if (needsCopy)
        {
            // TODO: put up toasts as initial copy can take awhile
            AndroidProxy.Log($"Copying {assetName} assets...");
            if (Directory.Exists(targetFolder))
                Directory.Delete(targetFolder, true);

            APKAssetManager.SaveItemToDirectory(assetName, copyBase, includeInitial: true);

            Directory.CreateDirectory(targetFolder);
            File.WriteAllText(markerFile, $"Copied at {DateTime.UtcNow:o}");
        }
    }
    
    public static string GetDataDir()
    {
        JClass unityPlayer = JNI.FindClass("com/unity3d/player/UnityPlayer");
        JFieldID activityFieldId = JNI.GetStaticFieldID(unityPlayer, "currentActivity", "Landroid/app/Activity;");
        using JObject currentActivityObj = JNI.GetStaticObjectField<JObject>(unityPlayer, activityFieldId);
        using JString callObjectMethod = JNI.CallObjectMethod<JString>(currentActivityObj, JNI.GetMethodID(JNI.GetObjectClass(currentActivityObj), "getPackageName", "()Ljava/lang/String;"));
        PackageName = callObjectMethod.GetString();
    
        JClass environment = JNI.FindClass("android/os/Environment");
        using JObject getExtDir = JNI.CallStaticObjectMethod<JObject>(environment, JNI.GetStaticMethodID(environment, "getExternalStorageDirectory", "()Ljava/io/File;"));
    
        JMethodID jMethodId = JNI.GetMethodID(JNI.GetObjectClass(getExtDir), "toString", "()Ljava/lang/String;");
        using JString objectMethod = JNI.CallObjectMethod<JString>(getExtDir, jMethodId);
        
        return Path.Combine(objectMethod.GetString(), "MelonLoader", PackageName);
    }
    
    // TODO: test multiple android versions
    public static bool EnsurePerms()
    {
        JClass versionClass = JNI.FindClass("android/os/Build$VERSION");
        JFieldID sdkIntField = JNI.GetStaticFieldID(versionClass, "SDK_INT", "I");
        ApiLevel = JNI.GetStaticField<int>(versionClass, sdkIntField);
        AndroidProxy.Log($"Android API Level: {ApiLevel}");

        JClass threadClass = JNI.FindClass("android/app/ActivityThread");
        JMethodID currentActivityThreadId = JNI.GetStaticMethodID(threadClass, "currentActivityThread", "()Landroid/app/ActivityThread;");
        using JObject currentActivityThread = JNI.CallStaticObjectMethod<JObject>(threadClass, currentActivityThreadId);

        JMethodID getApplicationMethodId = JNI.GetMethodID(JNI.GetObjectClass(currentActivityThread), "getApplication", "()Landroid/app/Application;");
        using JObject currentActivityObj = JNI.CallObjectMethod<JObject>(currentActivityThread, getApplicationMethodId);

        if (!CheckManageAllFilesPermission(currentActivityObj))
        {
            AndroidProxy.Log("Failed to get MANAGE_ALL_FILES permission.");
            return false;
        }

        if (!EnsurePermsWithUnity(currentActivityObj))
        {
            AndroidProxy.Log("Failed to ensure permissions with Unity.");
            return false;
        }
        
        return true;
    }

    private static bool CheckManageAllFilesPermission(JObject currentActivityObj)
    {
        if (ApiLevel < 30)
            return true; // This part of the API does not exist on Android versions below 11 (API level 30)

        const int MAX_WAIT = 30000; // in milliseconds

        JClass environment = JNI.FindClass("android/os/Environment");
        JClass uri = JNI.FindClass("android/net/Uri");
        JClass intent = JNI.FindClass("android/content/Intent");

        JMethodID isExternalStorageManagerMethodId = JNI.GetStaticMethodID(environment, "isExternalStorageManager", "()Z");
        bool isExternalStorageManager = JNI.CallStaticMethod<bool>(environment, isExternalStorageManagerMethodId);
        if (JNI.ExceptionCheck())
            return false;
            
        if (isExternalStorageManager)
            return true;
        
        using JString actionName = JNI.NewString("android.settings.MANAGE_APP_ALL_FILES_ACCESS_PERMISSION");
    
        using JString packageName = JNI.NewString($"package:{PackageName}");
    
        using JObject callStaticObjectMethod = JNI.CallStaticObjectMethod<JObject>(uri, JNI.GetStaticMethodID(uri, "parse", "(Ljava/lang/String;)Landroid/net/Uri;"), packageName);
        
        JMethodID intentConstructor = JNI.GetMethodID(intent, "<init>", "(Ljava/lang/String;Landroid/net/Uri;)V");
        using JObject initialIntent = JNI.NewObject<JObject>(intent, intentConstructor, actionName, callStaticObjectMethod);

        JMethodID addFlagsMethodId = JNI.GetMethodID(intent, "addFlags", "(I)Landroid/content/Intent;");
        int flag = 0x10000000; // FLAG_ACTIVITY_NEW_TASK
        using JObject flaggedIntent = JNI.CallObjectMethod<JObject>(initialIntent, addFlagsMethodId, new JValue(flag));
    
        JClass activityClass = JNI.GetObjectClass(currentActivityObj);
        JMethodID startActivityMethod = JNI.GetMethodID(activityClass, "startActivity", "(Landroid/content/Intent;)V");
        JNI.CallVoidMethod(currentActivityObj, startActivityMethod, new JValue(flaggedIntent));
    
        JNI.CheckExceptionAndThrow();
            
        // TODO: this shouldn't sleep in the main thread; not sure if there is a better method
        int totalWaitTime = 0;
        while (totalWaitTime < MAX_WAIT) {
            isExternalStorageManager = JNI.CallStaticMethod<bool>(environment, isExternalStorageManagerMethodId);
            if (JNI.ExceptionCheck())
                return false;

            if (isExternalStorageManager)
                return true;

            System.Threading.Thread.Sleep(250);
            totalWaitTime += 250;
        }

        AndroidProxy.Log("Timed out waiting for MANAGE_ALL_FILES permission, final check...");
        isExternalStorageManager = JNI.CallStaticMethod<bool>(environment, isExternalStorageManagerMethodId);
        if (JNI.ExceptionCheck())
            return false;

        if (isExternalStorageManager)
            return true;
        
        AndroidProxy.Log("Failed to get MANAGE_ALL_FILES permission after waiting.");
        return false;
    }

    private static bool EnsurePermsWithUnity(JObject currentActivityObj) {
        if (ApiLevel >= 30)
            return true; // Not necessary on Android 11+ as you need MANAGE_ALL_FILES_ACCESS_PERMISSION instead.

        string[] permissions = new string[]
        {
            "android.permission.WRITE_EXTERNAL_STORAGE",
            "android.permission.READ_EXTERNAL_STORAGE",
            "android.permission.MANAGE_EXTERNAL_STORAGE"
        };
        
        JClass unityPermissions = JNI.FindClass("com/unity3d/player/UnityPermissions");
        JClass unityWaitPermissionsClass = JNI.FindClass("com/unity3d/player/UnityPermissions$ModalWaitForPermissionResponse");

        using JObject waitPermission = JNI.NewObject<JObject>(unityWaitPermissionsClass, JNI.GetMethodID(unityWaitPermissionsClass, "<init>", "()V"));

        using JArray<JString> permissionArray = new JArray<JString>(permissions.Length);
        for (int i = 0; i < permissions.Length; i++)
        {
            permissionArray[i] = JNI.NewString(permissions[i]);
        }

        JMethodID requestUserPermissionsId = JNI.GetStaticMethodID(unityPermissions, "requestUserPermissions", "(Landroid/app/Activity;[Ljava/lang/String;Lcom/unity3d/player/IPermissionRequestCallbacks;)V");
        JNI.CallStaticVoidMethod(unityPermissions, requestUserPermissionsId, new JValue(currentActivityObj), new JValue(permissionArray), new JValue(waitPermission));

        if (JNI.ExceptionCheck())
        {
            AndroidProxy.Log("Failed to request permissions.");
            return false;
        }

        JMethodID waitForResponseId = JNI.GetMethodID(JNI.GetObjectClass(waitPermission), "waitForResponse", "()Z");
        JNI.CallVoidMethod(waitPermission, waitForResponseId);

        if (JNI.ExceptionCheck())
        {
            AndroidProxy.Log("Failed to wait for permission response.");
            return false;
        }

        return true;
    }
    
    public static DateTimeOffset GetApkModificationDate()
    { 
        var assetBytes = APKAssetManager.GetAssetBytes("lemon_patch_date.txt");
        string assetContent = Encoding.UTF8.GetString(assetBytes);
    
        // Now parse the string content into an RFC 3339 DateTime
        // RFC 3339 is essentially ISO 8601, so DateTime.Parse can handle it.
        if (DateTimeOffset.TryParse(assetContent, out DateTimeOffset date))
            return date;
    
        return default;
    }
}
#endif