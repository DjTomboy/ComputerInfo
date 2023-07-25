using ComputerInfo;
using System.Management;
using System.Text;

StringBuilder computerInfo = new();
StringBuilder errorCollector = new();

Console.WriteLine("Adatok lekérése folyamatban...");

//Számítógép név
computerInfo.AppendLine($"Számítógép név:\t\t{Environment.MachineName}");

//CPU
try
{
    using var searcher = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_Processor");
    foreach (var obj in searcher.Get())
    {
        double clockSpeedInGHz = Math.Round(Convert.ToDouble(obj["MaxClockSpeed"]) / 1000, 1);
        computerInfo.AppendLine($"{obj["DeviceID"]} név:\t\t{obj["Name"]}");
        computerInfo.AppendLine($"{obj["DeviceID"]} órajel:\t\t{clockSpeedInGHz} GHz");
        computerInfo.AppendLine($"{obj["DeviceID"]} magok száma:\t{obj["NumberOfCores"]}");
        computerInfo.AppendLine($"{obj["DeviceID"]} szálak száma:\t{obj["NumberOfLogicalProcessors"]}");
    }
}
catch (Exception ex)
{
    errorCollector.AppendLine($"Hiba történt a CPU adatok meghatározása közben: {ex.Message}");
}

//RAM
try
{
    using var searcher = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_PhysicalMemory");
    ulong totalMemoryBytes = 0;
    foreach (var obj in searcher.Get())
    {
        totalMemoryBytes += Convert.ToUInt64(obj["Capacity"]);
    }
    double totalMemoryInGB = Math.Round((double)totalMemoryBytes / 1073741824, 2);
    computerInfo.AppendLine($"RAM:\t\t\t{totalMemoryInGB} GB");
}
catch (Exception ex)
{
    errorCollector.AppendLine($"Hiba történt a RAM adatok meghatározása közben: {ex.Message}");
}

//Disk
try
{
    switch (OS.GetWindowsVersion())
    {
        case OS.WindowsVersion.Desktop_Vista:
        case OS.WindowsVersion.Desktop_7:
        case OS.WindowsVersion.Server_2008:
        case OS.WindowsVersion.Server_2008R2:
            using (var searcher = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_DiskDrive"))
                foreach (var obj in searcher.Get())
                {
                    ulong capacityInGB = Convert.ToUInt64(obj["Size"]) / 1073741824;
                    computerInfo.AppendLine($"DISK:\t\t\t{capacityInGB} GB\t ({obj["Model"]})\t");
                }
            errorCollector.AppendLine("Figyelmeztetés! Windows Vista, Windows 7, Windows Server 2008 és Windows Server 2008R2 operációs rendszerek esetén nem lehet megállapítani a DISK-ek típusát (HDD vagy SSD)");
            break;
        case OS.WindowsVersion.Desktop_8:
        case OS.WindowsVersion.Desktop_8_1:
        case OS.WindowsVersion.Desktop_10:
        case OS.WindowsVersion.Desktop_11:
        case OS.WindowsVersion.Server_2012:
        case OS.WindowsVersion.Server_2012R2:
        case OS.WindowsVersion.Server_2016:
        case OS.WindowsVersion.Server_2019:
        case OS.WindowsVersion.Server_2022:
            using (var searcher = new ManagementObjectSearcher(@"ROOT\Microsoft\Windows\Storage", "SELECT * FROM MSFT_PhysicalDisk"))
                foreach (var obj in searcher.Get())
                {
                    string mediaType = Convert.ToInt32(obj["MediaType"]) switch
                    {
                        3 => "HDD",
                        4 => "SSD",
                        _ => "Ismeretlen",
                    };
                    ulong capacityInGB = Convert.ToUInt64(obj["Size"]) / 1073741824;
                    computerInfo.AppendLine($"{mediaType}:\t\t\t{capacityInGB} GB\t ({obj["Model"]})\t");
                }
            break;
        case OS.WindowsVersion.Other:
            errorCollector.AppendLine($"DISK adatok meghatározása nem lehetséges, nem támogatott operációs rendszer verzió!");
            break;
    }
}
catch (Exception ex)
{
    errorCollector.AppendLine($"Hiba történt a DISK adatok meghatározása közben: {ex.Message}");
}

//OS
try
{
    using var searcher = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_OperatingSystem");
    var os = searcher.Get().OfType<ManagementObject>().FirstOrDefault();
    if (os != null)
    {
        computerInfo.AppendLine($"Operációs rendszer:\t{os["Caption"]} {os["OSArchitecture"]} ({os["Version"]})");
    }
}
catch (Exception ex)
{
    errorCollector.AppendLine($"Hiba történt az operációs rendszer adatok meghatározása közben: {ex.Message}");
}

//Printer
try
{
    using var searcher = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_Printer");
    foreach (var obj in searcher.Get())
    {
        computerInfo.AppendLine($"Nyomtató:\t\t{obj["Name"]}");
    }
}
catch (Exception ex)
{
    errorCollector.AppendLine($"Hiba történt a nyomtatók meghatározása közben: {ex.Message}");
}

//Internet speedtest -- csak 64 bites rendszeren
if (Environment.Is64BitOperatingSystem)
{
    try
    {
        Console.Clear();
    }
    catch
    { 
    }
    Console.WriteLine("Internet sebességmérés folyamatban...");
    var (speedtestResult, exception) = Operations.Speedtest();
    if (speedtestResult is null)
    {
        errorCollector.AppendLine($"Hiba történt internet sebességmérés közben! {exception?.Message}");
    }
    else
    {
        computerInfo.AppendLine();
        computerInfo.AppendLine("Internet sebességmérés adatok:");
        computerInfo.AppendLine($"Ping (tétlen):\t\t\t{speedtestResult.Ping?.GetLatencyString()}");
        computerInfo.AppendLine($"Ping (letöltés):\t\t{speedtestResult.Download?.Latency?.GetIqmString()}");
        computerInfo.AppendLine($"Ping (feltöltés):\t\t{speedtestResult.Upload?.Latency?.GetIqmString()}");
        computerInfo.AppendLine($"Letöltési sebesség:\t\t{speedtestResult.Download?.GetBandwidthInMbps()}");
        computerInfo.AppendLine($"Feltöltési sebesség:\t\t{speedtestResult.Upload?.GetBandwidthInMbps()}");
        computerInfo.AppendLine($"Internet szolgáltató\t\t{speedtestResult.Isp}");
        computerInfo.AppendLine($"Eredmény URL:\t\t\t{speedtestResult.Result?.Url}");
    }
}
else
{
    errorCollector.AppendLine("Internet sebesség mérése csak 64 bites operációs rendszeren lehetséges!");
}

//fájlba írás
try
{
    File.WriteAllText($"{Environment.MachineName}_paraméterek.txt", computerInfo.ToString(), Encoding.UTF8);
}
catch (Exception ex)
{
    errorCollector.AppendLine($"Hiba történt az adatok fájlba írása közben: {ex.Message}");
}

try 
{ 
    Console.Clear(); 
} 
catch
{ 
}
Console.WriteLine(computerInfo.ToString());

if (errorCollector.Length > 0)
{
    Console.BackgroundColor = ConsoleColor.Red;
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(errorCollector.ToString());
}

Console.ReadLine();