using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

class MemoryReader
{
    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    //Key Code
    const int PROCESS_WM_READ = 0x0010;

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    //DLL Imports
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, ref int lpBytesRead);

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    //ReadMemoryString
    public static string ReadMemoryString(int address, Int32[] offsets, string processName, int processNo)
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

            byte[] resultBuffer = new byte[15];
            int bytesReadResult = 0;
            ReadProcessMemory(processHandle, address, resultBuffer, resultBuffer.Length, ref bytesReadResult);

            string resultText = Encoding.UTF8.GetString(resultBuffer, 0, bytesReadResult).TrimEnd('\0');
            return resultText.Split('\0')[0];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return null;
        }
    }
    
} //Class
