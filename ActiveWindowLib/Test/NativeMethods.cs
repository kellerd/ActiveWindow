using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ActiveWindowLib;

public class NativeMethods
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern int GetWindowThreadProcessId(IntPtr windowHandle, out int processId);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(int hWnd, StringBuilder text, int count);

    public static int GetWindowThreadProcessId(IntPtr windowHandle)
    {
        int processId;
        GetWindowThreadProcessId(windowHandle, out processId);
        return processId;
    }
    public static StringBuilder GetWindowText(int hWnd, int count)
    {
        var sb = new StringBuilder(count);
        GetWindowText(hWnd, sb, count);
        return sb;
    }
    public static ProcessInfo GetCurrentProcessInfo() {
        var activeWindowId = NativeMethods.GetForegroundWindow();
                        // no (valid) foreground window => no trackable data!
                        if (activeWindowId.Equals(0))
                        {
                           return new NoProcessInfo();
                        }
                        int processId = NativeMethods.GetWindowThreadProcessId(activeWindowId);
                        // no (valid) process for window => no trackable data!
                        if (processId == 0)
                        {
                            return new NoProcessInfo();
                        }

                        using (var foregroundProcess = Process.GetProcessById(processId))
                        {
                               return new ProcessInfo() {
                                    ProcessName = foregroundProcess.ProcessName,
                                    FileName = foregroundProcess.MainModule.FileName,
                                    FileDescription = foregroundProcess.MainModule.FileVersionInfo.FileDescription,
                                    ProductName = foregroundProcess.MainModule.FileVersionInfo.ProductName,
                                    WindowTitle = foregroundProcess.MainWindowTitle ?? NativeMethods.GetWindowText((int)activeWindowId,  1024).ToString()
                                };
                        }
        }

}
