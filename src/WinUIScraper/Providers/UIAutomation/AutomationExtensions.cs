namespace WinUIScraper.Providers.UIAutomation
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Windows.Automation;

   public static class AutomationExtensions
   {
      public static AutomationElement FindFirstDescendantById(this AutomationElement ae, string automationId)
      {
         return ae.FindFirst(
            TreeScope.Descendants,
            new PropertyCondition(AutomationElement.AutomationIdProperty, automationId));
      }

      public static IEnumerable<AutomationElement> FindDescendantsById(this AutomationElement ae, string automationId)
      {
         return ae.FindDescendantsBy(AutomationElement.AutomationIdProperty, automationId);
      }

      public static IEnumerable<AutomationElement> FindDescendantsByClass(this AutomationElement ae, string className)
      {
         return ae.FindDescendantsBy(AutomationElement.ClassNameProperty, className);
      }

      public static IEnumerable<AutomationElement> FindDescendantsByControlType(this AutomationElement ae, ControlType className)
      {
         return ae.FindDescendantsBy(AutomationElement.ControlTypeProperty, className);
      }

      public static IEnumerable<AutomationElement> FindDescendantsBy(this AutomationElement ae, AutomationProperty property, object value)
      {
         return ae.FindAll(
            TreeScope.Descendants,
            new PropertyCondition(property, value))
            .Cast<AutomationElement>();
      }

      public static IEnumerable<AutomationElement> FindChildrenByName(this AutomationElement ae, string name)
      {
         return ae.FindDescendantsBy(AutomationElement.NameProperty, name);
      }

      public static IEnumerable<AutomationElement> FindChildrenBy(this AutomationElement ae, AutomationProperty property, object value)
      {
         return ae.FindAll(
            TreeScope.Children,
            new PropertyCondition(property, value))
            .Cast<AutomationElement>();
      }

      public static AutomationElement FindFirstAncestorByControlType(this AutomationElement ae, ControlType className)
      {
         var ancestor = TreeWalker.ControlViewWalker.GetParent(ae);
         for (; ancestor != null && className != ancestor.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty);
            ancestor = TreeWalker.ControlViewWalker.GetParent(ancestor))
         {
         }
         return ancestor;
      }

      public static void ExecuteWithUpdatedCache(this AutomationElement ae, CacheRequest cr, Action<AutomationElement> action)
      {
         using (cr.Activate())
            action(ae.GetUpdatedCache(cr));
      }

      public static string GetName(this AutomationElement ae)
      {
         return (string)ae.GetCurrentPropertyValue(AutomationElement.NameProperty);
      }

      public static string GetClassName(this AutomationElement ae)
      {
         return (string)ae.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
      }

      public static string GetStringValue(this AutomationElement ae)
      {
         object o = ae.GetCurrentPropertyValue(ValuePattern.ValueProperty);
         if (o == null)
            return null;
         return o.ToString();
      }

      public static string GetValueAsString(this AutomationElement ae)
      {
         object o;
         o = ae.GetCurrentPropertyValue(ValuePattern.ValueProperty);
         if (o == null)
            return null;
         if (o is string)
            return (string)o;
         return o.GetType().FullName + "   o=" + o;
         //o = ae.GetCurrentPattern(ValuePattern.Pattern);
         //if (o == null)
         //   return null;
         //IUIAutomationLegacyIAccessiblePattern 
         //return o.GetType().FullName + "   o="  + o;
      }

      public static string [] GetPropertyValues(this AutomationElement ae, params int[] propertyIds)
      {
         return propertyIds.Select(id => id + " -> " + ae.GetPropertyValueById(id)).ToArray();
      }

      public static string GetStringPassword(this AutomationElement ae)
      {
         return string.Join(Environment.NewLine,
            ae.GetPropertyValues(
#region ids
            30113,
30114,
30115,
30116,
30117,
30069,
30070,
30063,
30065,
30067,
30068,
30064,
30066,
30062,
30091,
30100,
30094,
30097,
30098,
30092,
30095,
30099,
30096,
30093,
30071,
30072,
30048,
30051,
30050,
30049,
30052,
30047,
30057,
30053,
30054,
30058,
30055,
30056,
30060,
30061,
30059,
30079,
30080,
30129,
30130,
30131,
30126,
30122,
30125,
30123,
30124,
30120,
30121,
30082,
30085,
30081,
30083,
30084,
30086,
30087,
30088,
30089,
30133,
30046,
30045,
30073,
30074,
30077,
30078,
30076,
30075
#endregion ids
));
      }

      public static string GetPropertyValueById(this AutomationElement ae, int id)
      {
         AutomationProperty property = AutomationProperty.LookupById(id);
         if (property == null)
            return "Could not find property: " + id;
         try
         {
            object o = ae.GetCurrentPropertyValue(property);
            if (o == null)
               return null;
            if (o is AutomationElement[])
            {
               AutomationElement[] elements = (AutomationElement[]) o;
               if (elements.Length == 0)
                  return "empty AutomationElement[]";
               return string.Join(">\r\n<",elements.Select(e => e.GetStringValue()).ToArray());
            }
            return o.ToString();
         }
         catch (Exception e)
         {
            Console.Error.WriteLine(property.Id + "   " + property.ProgrammaticName + "   " + property + "\r\n" + e.Message);
            return null;
         }
      }

      public static string GetStringPassword2(this AutomationElement ae)
      {
         try
         {
            object o = ae.GetCurrentPropertyValue(ValuePattern.ValueProperty);
            if (o == null)
               return null;
            return o.ToString();
         }
         catch(Exception e)
         {
            Console.Error.WriteLine(ValuePattern.ValueProperty.Id + "   " + ValuePattern.ValueProperty.ProgrammaticName + "   " + ValuePattern.ValueProperty + "\r\n" + e.Message);
            return null;
         }
      }

      public static CacheRequest BuildCacheRequest(TreeScope treeScope, params AutomationProperty[] properties)
      {
         var cr = new CacheRequest { TreeScope = treeScope };
         foreach (var property in properties)
            cr.Add(property);
         return cr;
      }

      public static IEnumerable<AutomationElement> OfType(this AutomationElementCollection items, ControlType controlType)
      {
         return 
            items
               .Cast<AutomationElement>()
               .Where(ae  => controlType == ae.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty));
      }
   }
}