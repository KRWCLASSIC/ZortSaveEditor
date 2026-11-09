using System;
using System.IO;
using System.Runtime.InteropServices;
using Zse.Bridge;
using Zse.Services;
using Zse.UI;

namespace Zse;

public static partial class Program
{
    [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetDllDirectoryW(string lpPathName);

    [STAThread]
    public static int Main(string[] args)
    {
        // 1. Direct Windows DLL loader to "lib/" subfolder for clean distribution layout
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string libDir = Path.Combine(baseDir, "lib");
        if (Directory.Exists(libDir))
        {
            SetDllDirectoryW(libDir);
        }

        string platformsDir = Path.Combine(libDir, "platforms");
        if (Directory.Exists(platformsDir))
        {
            Environment.SetEnvironmentVariable("QT_QPA_PLATFORM_PLUGIN_PATH", platformsDir);
        }

        // 2. Initialize save manager & scan default directories
        var saveManager = new SaveManager();
        saveManager.LoadAll();

        // 3. Create Qt Application via our thin C-ABI bridge
        IntPtr app = QtNative.AppCreate(0, IntPtr.Zero);
        QtNative.AppSetDarkTheme(app, saveManager.Config.IsDarkMode ? 1 : 0);

        // 4. Construct and display the entire UI in C#
        var window = new MainWindow(app, saveManager);
        window.Show();

        // 5. Enter Qt Event Loop
        return QtNative.AppExec(app);
    }
}
