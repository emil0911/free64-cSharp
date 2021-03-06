using System;
using System.Windows.Forms;
using Free64.Information;
using Free64.CPUID;
using System.Globalization;
using System.Threading;
using Free64.Common;
using Free64.GraphicalTrace;
using System.Threading.Tasks;
using System.Diagnostics;
using Free64.Properties;

namespace Free64
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the <see cref="Free64"/> application.
        /// </summary>
        [STAThread] // He came into your apartment and make it single-threaded
        
        static void Main() // A main function. The program starts here.
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles(); // We're enabling visual styles.
            Application.SetCompatibleTextRenderingDefault(false); // We're switching off compatible text rendering.
            Application.Run(new fmMain()); // Don't make a fight :D. The application is started.
        }
    }

    public static class Constants
    {
        /// <summary>
        /// Version of <see cref="Free64"/> build. This field is constant.
        /// </summary>
        public const string Version = "0.5.1-alpha";

        /// <summary>
        /// Date, when build of <see cref="Free64"/> was compiled. This field is constant.
        /// </summary>
        public static readonly DateTime BuildDate = new(2021, 4, 15);

        /// <summary>
        /// Link to the <see cref="Free64"/> repository page. This field is constant.
        /// </summary>
        public const string Repository = "https://github.com/emildalalyan/Free64-Sharp";

        // We're defining the constants
    }

    public static class Forms
    {
        /// <summary>
        /// Instance of <see cref="Free64.fmMain"/>
        /// </summary>
        public static fmMain fmMain; // Instance of fmMain, it will be filled by fmMain constructor called by Program.Main()

        /// <summary>
        /// Instance of <see cref="Free64.CPUID.CPUIDTool"/> class
        /// </summary>
        public static CPUIDTool exCPUID { get; } = new();
    }

    public static partial class Free64Application
    {
        /// <summary>
        /// Instance of <see cref="GraphicalTraceListener"/> listener
        /// </summary>
        public static GraphicalTraceListener GraphicalTrace { get; } = new();

        /// <summary>
        /// Instance of <see cref="WMI"/> class
        /// </summary>
        public static WMI Information { get; } = new();

        /// <summary>
        /// Instance of <see cref="Registry"/> class
        /// </summary>
        public static Registry RegInfo { get; } = new();

        /// <summary>
        /// Instance of <see cref="Information.CPUID"/> class
        /// </summary>
        public static Information.CPUID CPUID { get; } = new();

        /// <summary>
        /// <see cref="Free64Application"/> static constructor
        /// </summary>
        static Free64Application()
        {
            Trace.Listeners.Add(GraphicalTrace);   
        }

        /// <summary>
        /// Initialize <see cref="Free64"/> and gather all information from all <see cref="IInformationSource"/>s
        /// </summary>
        public static void Initialize() // Initialization function
        {
            GraphicalTrace.Clear(); // We're cleaning the debug list

            // We're deleting previous gathered information with "Reset()" function

            Information.Reset();

            CPUID.Reset();

            RegInfo.Reset();

            Trace.WriteLine("Starting an initialization...");

            Trace.Write("Getting date/time point...");
            
            double time = 0;

            try
            {
                time = CommonThings.GetUNIXTime(); // We're figuring out the current UNIX time.

                Trace.WriteLine(" Completed Successfully...");
            }
            catch(Exception e)
            {
                Trace.WriteLine(" Failed...");

                Trace.WriteLine($"[Free64Application.Initialize()] {e.Message}");
            }

            Trace.WriteLine($"Free64 Extreme Version: {Constants.Version} [build from {Constants.BuildDate:d MMMM yyyy}]"); // Showing Free64 Version

            Trace.WriteLine($"Assembly platform: {(Environment.Is64BitProcess ? "Win64" : "Win32")}"); // Showing command line arguments

            Trace.WriteLine("Showing splash screen...");

            using fmSplash Splash = new();

            Splash.Show();

            _ = Information.Initialize();

            _ = RegInfo.Initialize();

            Trace.WriteLine("Working with the GUI...");

            foreach (ListViewItem a in Forms.fmMain.listView1.Items)
            {
                switch (a.Tag)
                {
                    case "osname":
                        {
                            a.SubItems[1].Text = RegInfo.OperatingSystem.ProductName.Length > 0 ? RegInfo.OperatingSystem.ProductName : Information.OperatingSystem.Caption.IfNullReturnNA();
                            break;
                        }
                    case "compname":
                        {
                            a.SubItems[1].Text = Information.OperatingSystem.ComputerSystemName.IfNullReturnNA();
                            break;
                        }
                    case "osbuild":
                        {
                            a.SubItems[1].Text = Information.OperatingSystem.BuildNumber.IfNullReturnNA() + $" (Release ID: {(!string.IsNullOrEmpty(RegInfo.OperatingSystem.DisplayVersion) ? RegInfo.OperatingSystem.DisplayVersion : RegInfo.OperatingSystem.ReleaseId.IfNullReturnNA())})";
                            break;
                        }
                    case "oskernel":
                        {
                            a.SubItems[1].Text = Information.OperatingSystem.Version.IfNullReturnNA();
                            break;
                        }
                    case "installdate":
                        {
                            a.SubItems[1].Text = Information.OperatingSystem.InstallDate.ToString("g");
                            break;
                        }
                    case "compuuid":
                        {
                            a.SubItems[1].Text = Information.ComputerSystemProduct.UUID.IfNullReturnNA();
                            break;
                        }
                    case "syspn":
                        {
                            a.SubItems[1].Text = Information.ComputerSystemProduct.SystemProductName.IfNullReturnNA();
                            break;
                        }
                }
            }

            _ = CPUID.Initialize();

            Trace.WriteLine("Initializing Free64.CPUID.CPUIDTool class...");

            Forms.exCPUID.Initialize(Information, RegInfo, CPUID);

            Trace.WriteLine($"Initializing is done! ({(CommonThings.GetUNIXTime() - time):F3} sec)");

            // I'm bad. You know it. :D
            // Clear from code, i'm so bad ;)
        }

        /// <summary>
        /// Close all forms, save settings, and exit program...
        /// </summary>
        public static void CloseProgram()
        {
            Forms.fmMain.SaveFormSettings();
            Free64Application.CloseAllForms();
            Application.Exit();
        }

        /// <summary>
        /// Restart program
        /// </summary>
        public static void RestartProgram()
        {
            Forms.fmMain.SaveFormSettings();
            Free64Application.CloseAllForms();
            Application.Restart();
        }

        // Will you tell us, that you're okay?
        // Especially after looking at my code ;)
    }
}