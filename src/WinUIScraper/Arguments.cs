using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

internal class Arguments
{
   public Arguments(string[] args)
   {
      if (args.Length < 2)
      {
         ShowHelp = true;
         return;
      }
      if (args[0] == "-hwnd")
         WindowHandle = new IntPtr(long.Parse(args[1], NumberStyles.HexNumber));
      else if (args[0] == "-pid")
         WindowHandle = Process.GetProcessById(int.Parse(args[1])).MainWindowHandle;
      else
         ShowHelp = true;

      MSAA = args.Any(a => a.ToLowerInvariant() == "msaa");
   }

   public IntPtr WindowHandle{ get; set; }

   public bool ShowHelp { get; set; }

   public bool MSAA { get; set; }
}