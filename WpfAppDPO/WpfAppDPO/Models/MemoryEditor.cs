using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WpfAppDPO.Models
{
    class MemoryEditor
    {
        public delegate void MemoryEditorStateHandler(int damage, int blocked);
        MemoryEditorStateHandler _del;

        public void RegisterHandler(MemoryEditorStateHandler del)
        {
            _del += del;
        }

        public void UnregisterHandler(MemoryEditorStateHandler del)
        {
            _del -= del;
        }

        public MemoryEditor(string process_name)
        {
            this.pName = process_name;
        }

        public int GetBaseAddress()
        {
            using (Process myProcess = new Process())
            {
                // Get the process start information of notepad.
                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(pName);
                // Assign 'StartInfo' of notepad to 'StartInfo' of 'myProcess' object.
                myProcess.StartInfo = myProcessStartInfo;
                // Create a notepad.
                myProcess.Start();
                System.Threading.Thread.Sleep(1000);
                ProcessModule myProcessModule;
                // Get all the modules associated with 'myProcess'.
                ProcessModuleCollection myProcessModuleCollection = myProcess.Modules;
                //Console.WriteLine("Base addresses of the modules associated "
                //    + "with 'notepad' are:");
                // Display the 'BaseAddress' of each of the modules.
                myProcessModule = myProcessModuleCollection[0];
                // Get the main module associated with 'myProcess'.
                myProcessModule = myProcess.MainModule;
                // Display the 'BaseAddress' of the main module.
                //Console.WriteLine("The process's main module's base address is: "
                //    + myProcessModule.BaseAddress);
                myProcess.CloseMainWindow();
                BA = (int)myProcessModule.BaseAddress;
                return this.BA;
            }
        }

        public int GetProcessByName()
        {
            this.pID = 0;
            IntPtr handleToSnapshot = IntPtr.Zero;
            try
            {
                PROCESSENTRY32 procEntry = new PROCESSENTRY32();
                procEntry.dwSize = (UInt32)Marshal.SizeOf(typeof(PROCESSENTRY32));
                handleToSnapshot = CreateToolhelp32Snapshot((uint)SnapshotFlags.Process, 0);
                if (Process32First(handleToSnapshot, ref procEntry))
                {
                    do
                    {
                        if (this.pName == procEntry.szExeFile)
                        {
                            this.pID = (int)procEntry.th32ProcessID;
                            break;
                        }
                    } while (Process32Next(handleToSnapshot, ref procEntry));
                }
                else
                {
                    throw new ApplicationException(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error()));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't get the process.", ex);
            }
            finally
            {
                CloseHandle(handleToSnapshot);
            }
            return this.pID;
        }

        public byte[] ReadMemory(IntPtr address, uint size)
        {
            byte[] buffer = new byte[size];
            IntPtr bytesRead = IntPtr.Zero;
            IntPtr hProcess = OpenProcess(ProcessAccessFlags.VirtualMemoryRead, false, this.pID);
            ReadProcessMemory(hProcess, address, buffer, size, out bytesRead);
            CloseHandle(hProcess);
            return buffer;
        }

        public int ReadPointer(int addr, Int32[] offsets)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                addr = BitConverter.ToInt32(ReadMemory((IntPtr)addr, (uint)4), 0) + offsets[i];
            }
            return addr;
        }

        public void DamageDone(object sender, EventArgs e)
        {
            Int32 Addr = BA + 0x02AB7090; // 1 базовый адрес 2 смещение
            Int32[] offsets = { 0x90, 0x0, 0x10, 0xBC, 0x64, 0x114, 0x24 };
            Damage = BitConverter.ToInt32(ReadMemory((IntPtr)ReadPointer(Addr, offsets), (uint)4), 0);
        }

        public void DamageBlocked(object sender, EventArgs e)
        {
            Int32 Addr = BA + 0x02AB7090; // 1 базовый адрес 2 смещение
            //Int32 baseAddr = 0x03797090;
            Int32[] offsets = { 0x90, 0x0, 0x10, 0xBC, 0x64, 0x114, 0x28 };
            Blocked = BitConverter.ToInt32(ReadMemory((IntPtr)ReadPointer(Addr, offsets), (uint)4), 0);
        }

        public int Damage 
        {
            get
            {
                return damage;
            }
            set
            {
                damage = value;
                _del?.Invoke(Damage, Blocked);
            }
        }

        public int Blocked
        {
            get
            {
                return blocked;
            }
            set
            {
                blocked = value;
                _del?.Invoke(Damage, Blocked);
            }
        }

        private int damage;
        private int blocked;

        public int BA;
        private string pName;
        public int pID;

        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            internal UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal UInt32 th32ModuleID;
            internal UInt32 cntThreads;
            internal UInt32 th32ParentProcessID;
            internal Int32 pcPriClassBase;
            internal UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr CreateToolhelp32Snapshot([In]UInt32 dwFlags, [In]UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32First([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32Next([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out]byte[] buffer, UInt32 size, out IntPtr lpNumberofBytesRead);
    }
}
