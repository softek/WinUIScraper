using System;
using Accessibility;
using WinUIScraper.Providers.Msaa;

class MsaaProgram
{
   public static void Run(Arguments arguments)
   {
      if (arguments.ShowHelp)
      {
         Usage();
         return;
      }
         
      var rootAccessible = UIAccessibleHelper.GetAccessibleObjectFromWindow(arguments.WindowHandle);
      DumpValuesRecursive(rootAccessible, 0);
   }

   private static void DumpValuesRecursive(IAccessible accessible, int indent)
   {
      DumpAccessible(accessible, indent);
      foreach (var child in accessible.GetChildren())
         DumpValuesRecursive(child, indent + 2);
   }

   private static void DumpAccessible(IAccessible accessible, int indent)
   {
      Console.Write(new string(' ', indent) + "* ");
      Console.Write(accessible.GetNameSafe() ?? "<null name>");
      Console.Write('\t');
      Console.Write(accessible.GetRoleSafe());
      Console.Write('\t');
      Console.Write(accessible.GetValueAsString() ?? "<null value>");
      Console.Write('\t');
      Console.Write(accessible.GetStateSafe() ?? "<null state>");
      Console.Write('\t');
      Console.Write(accessible.GetDescriptionSafe() ?? "<null description>");
      Console.WriteLine();
   }

   private static void Usage()
   {
      Console.WriteLine(@"Usage:
WinUIScraper -hwnd <window handle in hex>
WinUIScraper -pid <process id>
");
   }
}