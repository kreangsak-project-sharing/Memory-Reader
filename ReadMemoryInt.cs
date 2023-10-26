using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class MemoryReader
{
    const int PROCESS_WM_READ = 0x0010;

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, ref int lpBytesRead);

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    //ReadMemoryInt
    public static int ReadMemoryInt(int address, Int32[] offsets, string processName, int processNo)
    {
        try
        {
            Process process = Process.GetProcessesByName(processName)[processNo];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

            if (offsets != null)
            {
                int addressBase = process.MainModule.BaseAddress.ToInt32() + address;
                
                foreach (int offset in offsets)
                {
                    byte[] buffer = new byte[IntPtr.Size];
                    int bytesRead = 0;

                    ReadProcessMemory(processHandle, addressBase, buffer, buffer.Length, ref bytesRead);
                    addressBase = BitConverter.ToInt32(buffer, 0) + offset;
                }

                address = addressBase;
            }

            byte[] resultBuffer = new byte[4]; // Assuming Int32 size is 4 bytes
            int bytesReadResult = 0;
            ReadProcessMemory(processHandle, address, resultBuffer, resultBuffer.Length, ref bytesReadResult);

            int result = BitConverter.ToInt32(resultBuffer, 0);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 0; // Return a default value or handle the error according to your needs
        }
    }

} //Class
