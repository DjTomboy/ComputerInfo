using System.Runtime.InteropServices;

namespace ComputerInfo
{
    internal static class OS
    {
        /// <summary>
        /// Windows verziókat tartalmazó enum
        /// </summary>
        internal enum WindowsVersion
        {
            /// <summary>
            /// Windows Vista
            /// </summary>
            Desktop_Vista = 6,

            /// <summary>
            /// Windows 7
            /// </summary>
            Desktop_7 = 7,

            /// <summary>
            /// Windows 8
            /// </summary>
            Desktop_8 = 8,

            /// <summary>
            /// Windows 8.1
            /// </summary>
            Desktop_8_1 = 9,

            /// <summary>
            /// Windows 10
            /// </summary>
            Desktop_10 = 10,

            /// <summary>
            /// Windows 11
            /// </summary>
            Desktop_11 = 11,

            /// <summary>
            /// Windows Server 2008
            /// </summary>
            Server_2008 = 2008,

            /// <summary>
            /// Windows Server 2008 R2
            /// </summary>
            Server_2008R2 = 2009,

            /// <summary>
            /// Windows Server 2012
            /// </summary>
            Server_2012 = 2012,

            /// <summary>
            /// Windows Server 2012 R2
            /// </summary>
            Server_2012R2 = 2013,

            /// <summary>
            /// Windows Server 2016
            /// </summary>
            Server_2016 = 2016,

            /// <summary>
            /// Windows Server 2019
            /// </summary>
            Server_2019 = 2019,

            /// <summary>
            /// Windows Server 2022
            /// </summary>
            Server_2022 = 2022,

            /// <summary>
            /// Egyéb ismeretlen verzió
            /// </summary>
            Other = 0
        }

        const int OS_ANYSERVER = 29;
        [DllImport("shlwapi.dll", SetLastError = true, EntryPoint = "#437")]
        private static extern bool IsOS(int os);

        /// <summary>
        /// Aktuális Windows operációs rendszert ellenőrzi a metódus, hogy szerver rendszer-e.
        /// </summary>
        /// <returns>Igazat ad vissza, ha az aktuális operációs rendszer szerver rendszer.</returns>
        internal static bool IsWindowsServer()
        {
            return IsOS(OS_ANYSERVER);
        }

        /// <summary>
        /// Aktuális Windows operációs rendszer verziójának lekérdezésére használható metódus.
        /// </summary>
        /// <returns>Az aktuális Windows verziót adja vissza.</returns>
        public static WindowsVersion GetWindowsVersion()
        {
            WindowsVersion winver;
            switch (Environment.OSVersion.Version.Major)
            {
                case 6:
                    winver = Environment.OSVersion.Version.Minor switch
                    {
                        0 => IsWindowsServer() ? WindowsVersion.Server_2008 : WindowsVersion.Desktop_Vista,
                        1 => IsWindowsServer() ? WindowsVersion.Server_2008R2 : WindowsVersion.Desktop_7,
                        2 => IsWindowsServer() ? WindowsVersion.Server_2012 : WindowsVersion.Desktop_8,
                        3 => IsWindowsServer() ? WindowsVersion.Server_2012R2 : WindowsVersion.Desktop_8_1,
                        _ => 0,
                    };
                    break;
                case 10:
                    if (IsWindowsServer())
                    {
                        //Windows Server 2019 build 17763-tól kezdődik, alatta Server 2016
                        if (Environment.OSVersion.Version.Build < 17763)
                        {
                            winver = WindowsVersion.Server_2016;
                        }
                        else
                        {
                            //Windows Server 2022 build 20000-től kezdődik, 17763-19999 build Windows Server 2019
                            winver = (Environment.OSVersion.Version.Build >= 20000) ? WindowsVersion.Server_2022 : WindowsVersion.Server_2019;
                        }
                    }
                    else
                    {
                        //Windows 11 build 22000-től kezdődik, Windows 10 build 10000-19999-ig
                        winver = (Environment.OSVersion.Version.Build >= 22000) ? WindowsVersion.Desktop_11 : WindowsVersion.Desktop_10;
                    }
                    break;
                default:
                    winver = 0;
                    break;
            }

            return winver;
        }
    }
}
