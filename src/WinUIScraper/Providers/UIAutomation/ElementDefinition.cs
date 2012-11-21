namespace WinUIScraper.Providers.UIAutomation
{
   using System;
   using System.Windows.Automation;

   public class ElementDefinition
   {
      private readonly TreeScope treeScope;
      private readonly Condition condition;
      private readonly Func<AutomationElement, object> valueProvider;

      public ElementDefinition(TreeScope treeScope, Condition condition, Func<AutomationElement, object> valueProvider)
      {
         this.treeScope = treeScope;
         this.condition = condition;
         this.valueProvider = valueProvider;
      }

      public TreeScope TreeScope
      {
         get { return treeScope; }
      }

      public Func<AutomationElement, object> ValueProvider
      {
         get { return valueProvider; }
      }

      public Condition Condition
      {
         get { return condition; }
      }
   }
}
