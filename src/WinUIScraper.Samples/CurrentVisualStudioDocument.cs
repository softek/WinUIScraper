using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;

using WinUIScraper.Declarative;
using WinUIScraper.Providers;
using WinUIScraper.Providers.UIAutomation;

namespace WinUIScraper.Samples
{
   public static class VisualStudioSample
   {
      public static string GetCurrentDocument(Process visualStudioProcess)
      {
         // Declare what you are looking for
         Node uiElementsToFind = BuildVisualStudio2010Tree();
         // Pick the AutomationElement to get data from
         AutomationElement mainWindowElement = GetMainWindowElement(visualStudioProcess);
         // Get values of matched elements
         var dictionaryOfFoundValues = new HierarchicalValueProvider(mainWindowElement).GetValues(uiElementsToFind);
         // Read the flattened tree
         List<string> files = dictionaryOfFoundValues["file"];
         return files.FirstOrDefault();
      }

      static Node BuildVisualStudio2010Tree()
      {
         // AnonymousNodes have no name or values, but they are great as landmarks.
         // ValueNodes represent the values you are looking for.
         return
            new AnonymousNode(DescendentDocumentGroups()) // Find DocumentGroup
               {                                          // Foreach DocumentGroup
                  new AnonymousNode(SelectedChildren())   //   Find selected document
                  {                                       //   Foreach document
                     new ValueNode("file", GetName),      //     Save name as "file"
                  }
               };
      }

      static AutomationElement GetMainWindowElement(Process process)
      {
         return AutomationElement.FromHandle(process.MainWindowHandle);
      }

      static Func<AutomationElement, IEnumerable<AutomationElement>> DescendentDocumentGroups()
      {
         return element => element.FindDescendantsByControlType(ControlType.Tab).Where(e => "DocumentGroup" == e.GetClassName());
      }

      static Func<AutomationElement, IEnumerable<AutomationElement>> SelectedChildren()
      {
         return element => element.FindChildrenBy(SelectionItemPattern.IsSelectedProperty, true);
      }

      static string GetName(AutomationElement element)
      {
         return element.GetName();
      }
   }

   // This HierarchicalValueProvider using UIAutomation.  Keys are strings, and values are strings.
   // But of course you can use different types of library, keys, or values.
   class HierarchicalValueProvider : HierarchicalValueProvider<AutomationElement, string, string>
   {
      public HierarchicalValueProvider(AutomationElement element)
         : base(element)
      {
      }
   }

   class Node : KeyedTreeNode<string, IElementsProvider<AutomationElement, string>>
   {
      protected Node(string key, Func<AutomationElement, IEnumerable<AutomationElement>> sourcesSelector, Func<AutomationElement, string> dataSelector)
         : base(key, new AutomationElementsProvider(sourcesSelector, dataSelector))
      {
      }
   }
   class ValueNode : Node
   {
      public ValueNode(string key, Func<AutomationElement, string> dataSelector)
         : base(key, element => new[] { element }, dataSelector)
      {
      }
   }
   class AnonymousNode : Node
   {
      public AnonymousNode(Func<AutomationElement, IEnumerable<AutomationElement>> sourcesSelector)
         : base(null, sourcesSelector, null)
      {
      }
   }
   class AutomationElementsProvider : DelegatedElementsProvider<AutomationElement, string>
   {
      public AutomationElementsProvider(Func<AutomationElement, IEnumerable<AutomationElement>> sourcesSelector, Func<AutomationElement, string> dataSelector = null)
         : base(sourcesSelector, dataSelector)
      {
      }
   }

   static class DeclarativeExtensions
   {
      public static string GetClassName(this AutomationElement ae)
      {
         return (string)ae.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
      }
   }
}
