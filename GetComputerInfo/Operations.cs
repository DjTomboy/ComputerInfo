using ComputerInfo.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace ComputerInfo
{
    internal static class Operations
    {
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
    }
}
