using System;
using System.Diagnostics;
using System.Globalization;
using Accessibility;

class Program
{
   static void Main(string[] args)
   {
      var arguments = new Arguments(args);
      if (arguments.MSAA)
         MsaaProgram.Run(arguments);
      else
         UIAutomationProgram.Run(arguments);
   }
}
