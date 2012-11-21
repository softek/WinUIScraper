using System;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WinUIScraper.Providers.UIAutomation;

namespace WinUIScraper.UnitTests
{
   [TestClass]
   public class ElementDefinitionTests
   {
      [TestMethod]
      public void TreeScopeSetByConstructor()
      {
         const TreeScope treeScope = TreeScope.Descendants;
         var def = CreateElementDefinition(treeScope, AutomationElement.NameProperty, CreateValueProvider(AutomationElement.NameProperty), null);
         Assert.AreEqual(treeScope, def.TreeScope);
      }

      [TestMethod]
      public void TreeScope2SetByConstructor()
      {
         const TreeScope treeScope = TreeScope.Children;
         var def = CreateElementDefinition(treeScope, AutomationElement.NameProperty, CreateValueProvider(AutomationElement.NameProperty), null);
         Assert.AreEqual(treeScope, def.TreeScope);
      }

      [TestMethod]
      public void ValueProviderSetByConstructor()
      {
         var automationProperty = AutomationElement.NameProperty;
         var valueProvider = CreateValueProvider(automationProperty);
         var def = CreateElementDefinition(TreeScope.Descendants, automationProperty, valueProvider, null);
         Assert.AreEqual(valueProvider, def.ValueProvider);
      }

      [TestMethod]
      public void ConditionSetByConstructor()
      {
         var automationProperty = AutomationElement.NameProperty;
         var valueProvider = CreateValueProvider(automationProperty);
         var condition = new PropertyCondition(automationProperty, "");
         var def = CreateElementDefinition(TreeScope.Descendants, automationProperty, valueProvider, condition);
         Assert.IsTrue(ReferenceEquals(condition, def.Condition));
      }

      private static ElementDefinition CreateElementDefinition(TreeScope treeScope, AutomationProperty automationProperty, Func<AutomationElement, object> valueProvider, Condition condition)
      {
         return new ElementDefinition(treeScope, condition ?? new PropertyCondition(automationProperty, ""), valueProvider);
      }

      private static Func<AutomationElement, object> CreateValueProvider(AutomationProperty automationProperty)
      {
         return element => element.GetCurrentPropertyValue(automationProperty);
      }
   }
}
