using System;
using System.Windows.Automation;
using WinUIScraper.Providers.UIAutomation;

class UIAutomationProgram
{
   public static void Run(Arguments arguments)
   {
      if (arguments.ShowHelp)
      {
         Usage();
         return;
      }

      AutomationElement.FromHandle(arguments.WindowHandle).ExecuteWithUpdatedCache(AutomationExtensions.BuildCacheRequest(TreeScope.Element | TreeScope.Descendants, AutomationElement.NameProperty),
         element => DumpValuesRecursive(element, 0));
   }

   private static void DumpValuesRecursive(AutomationElement element, int indent)
   {
      DumpElement(element.GetUpdatedCache(new CacheRequest()), indent);
      foreach (AutomationElement child in element.CachedChildren)
         DumpValuesRecursive(child, indent + 2);
   }

   private static void DumpElement(AutomationElement accessible, int indent)
   {
      Console.Write(new string(' ', indent) + "* ");
      Console.Write(accessible.GetName() ?? "<null name>");
      Console.Write('\t');
      Console.Write(accessible.GetClassName());
      Console.Write('\t');
      Console.Write(accessible.GetValueAsString() ?? "<null value>");
      //Console.Write('\t');
      //Console.Write(accessible.GetStateSafe() ?? "<null state>");
      //Console.Write('\t');
      //Console.Write(accessible.GetDescriptionSafe() ?? "<null description>");
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
