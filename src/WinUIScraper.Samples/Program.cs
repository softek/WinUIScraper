using System;
using System.Diagnostics;
using System.Threading;

namespace WinUIScraper.Samples
{
   class Program
   {
      static void Main()
      {
         while (true)
         {
            foreach (var process in Process.GetProcessesByName("devenv"))
               DescribeVisualStudioProcess(process);

            Console.WriteLine();
            Console.WriteLine("Sleeping... Press Ctrl+C to break");
            Thread.Sleep(10000);
         }
      }

      static void DescribeVisualStudioProcess(Process process)
      {
         Console.WriteLine("==============================================================");
         Console.WriteLine(process.MainWindowTitle);
         Console.WriteLine("  You are using " + VisualStudioSample.GetCurrentDocument(process));
         Console.WriteLine("==============================================================");
         Console.WriteLine();
      }
   }
}
