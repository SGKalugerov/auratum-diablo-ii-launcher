using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DiabloLauncher.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
#pragma warning disable CA1822 
        public string Greeting => "Welcome to Auratum D2!";


#pragma warning restore CA1822 

        [ObservableProperty]
        private bool isWindowed;

        [ObservableProperty]
        private bool isHD;

        public IRelayCommand PlayCommand { get; }

        public MainWindowViewModel()
        {
            PlayCommand = new RelayCommand(OnPlay);
        }
        private void AddDEPExceptionForGame(string gamePath)
        {
            try
            {
                string depRegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";

                Registry.SetValue(depRegistryKey, gamePath, "DisableNXShowUI", RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding DEP exception for game: {ex.Message}");
            }
        }
        private void OnPlay()
        {
            string launchArguments = string.Empty;

            if (IsWindowed)
                launchArguments += "-w ";

            if (IsHD)
                launchArguments += "-3dfx ";

            var gamePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game.exe");

            if (!File.Exists(gamePath))
            {
                return;
            }
            AddDEPExceptionForGame(gamePath);

            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = gamePath,
                    Arguments = launchArguments.Trim(),
                };

                Process.Start(processStartInfo);

                //if (process != null)
                //{
                //    Task.Run(async () =>
                //    {
                //        await Task.Delay(TimeSpan.FromMinutes(1));
                //        CheckForInjectedDLLs(process.Id);
                //    });
                //}
            }
            catch (Exception ex)
            {
                throw new Exception($"Error launching game: {ex.Message}");
            }
        }

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModules(IntPtr hProcess, IntPtr[] lphModule, uint cb, out uint lpcbNeeded);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpBaseName, uint nSize);

        private void CheckForInjectedDLLs(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);

                if (process == null)
                {
                    throw new Exception("Process not found.");
                }

                IntPtr[] hModules = new IntPtr[1024];
                uint cbNeeded;

                IntPtr hProcess = process.Handle;

                if (EnumProcessModules(hProcess, hModules, (uint)(IntPtr.Size * hModules.Length), out cbNeeded))
                {
                    int moduleCount = (int)(cbNeeded / IntPtr.Size);

                    for (int i = 0; i < moduleCount; i++)
                    {
                        StringBuilder moduleName = new StringBuilder(256);

                        GetModuleFileNameEx(hProcess, hModules[i], moduleName, (uint)moduleName.Capacity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking for injected DLLs: {ex.Message}");
            }
        }
    }
}
