using System.Management;
using System.Text;

StringBuilder computerInfo = new();
StringBuilder errorCollector = new();

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
    using var searcher = new ManagementObjectSearcher(@"ROOT\Microsoft\Windows\Storage", "SELECT * FROM MSFT_PhysicalDisk");
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

//fájlba írás
try
{
    File.WriteAllText($"{Environment.MachineName}_paraméterek.txt", computerInfo.ToString(), Encoding.UTF8);
}
catch (Exception ex)
{
    errorCollector.AppendLine($"Hiba történt az adatok fájlba írása közben: {ex.Message}");
}

Console.WriteLine(computerInfo.ToString());

if (errorCollector.Length > 0)
{
    Console.BackgroundColor = ConsoleColor.Red;
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(errorCollector.ToString());
}

Console.ReadLine();