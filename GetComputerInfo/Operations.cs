using ComputerInfo.Models;
using Newtonsoft.Json;
using SegedNet7;
using System.Diagnostics;
using System.Text;

namespace ComputerInfo
{
    internal static class Operations
    {
        static AppUpdate.RunModes runMode;

        internal static (SpeedtestModel? speedtestResult, Exception? errorMessage) Speedtest()
        {
            string speedtestExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "speedtest.exe");
            ProcessStartInfo startInfo = new()
            {
                FileName = speedtestExePath,
                Arguments = "-f json --accept-license --accept-gdpr",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
            };

            try
            {
                using var process = Process.Start(startInfo);
                var output = process?.StandardOutput.ReadToEnd();
                var error = process?.StandardError.ReadToEnd();
                process?.WaitForExit();

                if (process?.ExitCode != 0
                    || output is null)
                {
                    throw new Exception(error);
                }                

                SpeedtestModel? speedtestResult = JsonConvert.DeserializeObject<SpeedtestModel>(output);

                return speedtestResult is null 
                    ? throw new Exception() 
                    : (speedtestResult, null);
            }
            catch (Exception ex)
            {
                return (null, new Exception(ex.Message));
            }
        }

        internal static double ConvertBytesToMBits (int bytes)
        {
            return Math.Round(Convert.ToDouble(bytes) / 125000, 2);
        }

        internal static void StopSpeedtest()
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -ExecutionPolicy Bypass -Command Stop-Process -Name \"speedtest\"",
                UseShellExecute = false,                
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };

            try
            {
                using var process = Process.Start(startInfo);
            }
            catch
            {                
            }
        }

        internal static bool CheckForUpdate()
        {
            runMode = Environment.Is64BitProcess
                ? AppUpdate.RunModes.ComputerInfo_x64
                : AppUpdate.RunModes.ComputerInfo;
            Version currentVersion = AppUpdate.GetCurrentVersion(runMode);
            Version latestVersion = AppUpdate.GetLatestVersion(runMode).Result;

            return latestVersion != new Version("0.0.0.0")
                && currentVersion != new Version("0.0.0.0")
                && currentVersion.CompareTo(latestVersion) < 0;
        }

        internal static void StartUpdate()
        {
            string appUpdaterExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                        Environment.Is64BitOperatingSystem ? "AppUpdater_x64.exe" : "AppUpdater.exe");
            if (File.Exists(appUpdaterExePath))
            {
                using Process p = Process.Start(appUpdaterExePath, $"/{runMode}");
                p.WaitForExit();
            }
            else
            {
                Program.errorCollector.AppendLine($"Hiba történt programfrissítés indítása közben, nem létezik a AppUpdater program! ({appUpdaterExePath})");
            }
        }
    }
}
