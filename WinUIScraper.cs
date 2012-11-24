// WinUIScraper - A declarative approach to screen scraping in Windows 
 
// Built on Sat 11/24/2012 10:00:14.68 
 
 
//////////////////////////////////////////////////////////////////////// 
// C:\code\jeremy.sellars\WinUIScraper\src\WinUIScraper\Declarative\HierarchicalValueProvider.cs 
 
﻿namespace WinUIScraper.Declarative
{
   using System;
   using System.Collections.Generic;
   using Providers;

   public class HierarchicalValueProvider<TSource, TKey, TData>
   {
      private readonly TSource source;

      public HierarchicalValueProvider(TSource source)
      {
         this.source = source;
      }

      public TSource GetElement(Func<TSource, TSource> getElement)
      {
         return getElement(source);
      }

      public IEnumerable<TSource> GetElements(Func<TSource, IEnumerable<TSource>> getElements)
      {
         return getElements(source);
      }

      public TData GetValue(Func<TSource, TSource> getElement, Func<TSource, TData> getValue)
      {
         return getValue(GetElement(getElement));
      }

      public Dictionary<TKey, List<TData>> GetValues(KeyedTreeNode<TKey, IElementProvider<TSource, TData>> pattern)
      {
         var dictionary = new Dictionary<TKey, List<TData>>();
         var helper = new HeiarchyFlattener(dictionary);
         helper.AddValuesRecursively(pattern, source);
         return dictionary;
      }

      private class HeiarchyFlattener
      {
         private readonly Dictionary<TKey, List<TData>> dictionary;

         public HeiarchyFlattener(Dictionary<TKey, List<TData>> dictionary)
         {
            this.dictionary = dictionary;
         }

         public void AddValuesRecursively(KeyedTreeNode<TKey, IElementProvider<TSource, TData>> pattern, TSource source)
         {
            var elementProvider = pattern.Value;
            var element = elementProvider.GetElement(source);
            if (pattern.Key != null)
               GetOrCreateList(pattern.Key).Add(elementProvider.GetValue(element));
            AddChildrenRecursively(pattern, element);
         }

         public void AddValuesRecursively(KeyedTreeNode<TKey, IElementsProvider<TSource, TData>> pattern, TSource source)
         {
            var elementProvider = pattern.Value;
            var elements = elementProvider.GetElements(source);

            if (pattern.Key == null)
            {
               foreach (var element in elements)
                  AddChildrenRecursively(pattern, element);
            }
            else
            {
               var values = GetOrCreateList(pattern.Key);
               foreach (var element in elements)
               {
                  values.Add(elementProvider.GetValue(element));
                  AddChildrenRecursively(pattern, element);
               }
            }
         }

         private void AddChildrenRecursively(KeyedTreeNode<TKey, IElementProvider<TSource, TData>> pattern, TSource element)
         {
            foreach (var child in pattern.Children)
               AddValuesRecursively(child, element);
         }

         private void AddChildrenRecursively(KeyedTreeNode<TKey, IElementsProvider<TSource, TData>> pattern, TSource element)
         {
            foreach (var child in pattern.Children)
               AddValuesRecursively(child, element);
         }

         private List<TData> GetOrCreateList(TKey key)
         {
            List<TData> list;
            if (!dictionary.TryGetValue(key, out list))
            {
               list = new List<TData>();
               dictionary.Add(key, list);
            }
            return list;
         }
      }

      public Dictionary<TKey, List<TData>> GetValues(KeyedTreeNode<TKey, IElementsProvider<TSource, TData>> pattern)
      {
         var dictionary = new Dictionary<TKey, List<TData>>();
         var helper = new HeiarchyFlattener(dictionary);
         helper.AddValuesRecursively(pattern, source);
         return dictionary;
      }
   }
}
 
//////////////////////////////////////////////////////////////////////// 
// C:\code\jeremy.sellars\WinUIScraper\src\WinUIScraper\Declarative\KeyedTreeNode.cs 
 
﻿namespace WinUIScraper.Declarative
{
   using System.Collections.Generic;

   public class KeyedTreeNode<TKey, TValue> : IEnumerable<KeyedTreeNode<TKey, TValue>>
   {
      private readonly List<KeyedTreeNode<TKey, TValue>> children;

      public KeyedTreeNode(TKey key, TValue value)
      {
         Key = key;
         Value = value;
         children = new List<KeyedTreeNode<TKey, TValue>>();
      }

      public void Add(KeyedTreeNode<TKey, TValue> item)
      {
         children.Add(item);
      }

      public void AddRange(IEnumerable<KeyedTreeNode<TKey, TValue>> items)
      {
         children.AddRange(items);
      }

      public TKey Key { get; private set; }
      public TValue Value { get; private set; }
      public IEnumerable<KeyedTreeNode<TKey,TValue>> Children
      {
         get { return children; }
      }

      public IEnumerator<KeyedTreeNode<TKey, TValue>> GetEnumerator()
      {
         return children.GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public IEnumerable<KeyedTreeNode<TKey, TValue>> EnumerateDescendants()
      {
         foreach (var child in children)
         {
            yield return child;
            foreach (var descendant in child.EnumerateDescendants())
               yield return descendant;
         }
      }
   }
}
 
//////////////////////////////////////////////////////////////////////// 
// C:\code\jeremy.sellars\WinUIScraper\src\WinUIScraper\Providers\DelegatedElementsProvider.cs 
 
﻿namespace WinUIScraper.Providers
{
   using System;
   using System.Collections.Generic;

   public class DelegatedElementsProvider<TSource, TData> : IElementsProvider<TSource, TData>
   {
      private readonly Func<TSource, IEnumerable<TSource>> sourcesSelector;
      private readonly Func<TSource, TData> dataSelector;

      public DelegatedElementsProvider(Func<TSource,IEnumerable<TSource>> sourcesSelector, Func<TSource,TData> dataSelector)
      {
         this.sourcesSelector = sourcesSelector;
         this.dataSelector = dataSelector;
      }

      public IEnumerable<TSource> GetElements(TSource element)
      {
         return sourcesSelector(element);
      }

      public TData GetValue(TSource element)
      {
         return dataSelector(element);
      }
   }
} 
//////////////////////////////////////////////////////////////////////// 
// C:\code\jeremy.sellars\WinUIScraper\src\WinUIScraper\Providers\IElementProvider.cs 
 
﻿namespace WinUIScraper.Providers
{
   public interface IElementProvider<TSource, TData>
   {
      TSource GetElement(TSource element);
      TData GetValue(TSource element);
   }
} 
//////////////////////////////////////////////////////////////////////// 
// C:\code\jeremy.sellars\WinUIScraper\src\WinUIScraper\Providers\IElementsProvider.cs 
 
﻿namespace WinUIScraper.Providers
{
   using System.Collections.Generic;

   public interface IElementsProvider<TSource, TData>
   {
      IEnumerable<TSource> GetElements(TSource element);
      TData GetValue(TSource element);
   }
} 
//////////////////////////////////////////////////////////////////////// 
// C:\code\jeremy.sellars\WinUIScraper\src\WinUIScraper\Providers\Msaa\AccRole.cs 
 
﻿namespace WinUIScraper.Providers.Msaa
{
   public enum AccRole
   {
      Titlebar = 0x1,
      Menubar = 0x2,
      Scrollbar = 0x3,
      Grip = 0x4,
      Sound = 0x5,
      Cursor = 0x6,
      Caret = 0x7,
      Alert = 0x8,
      Window = 0x9,
      Client = 0xa,
      Menupopup = 0xb,
      MenuItem = 0xc,
      ToolTip = 0xd,
      Application = 0xe,
      Document = 0xf,
      Pane = 0x10,
      Chart = 0x11,
      Dialog = 0x12,
      Border = 0x13,
      Grouping = 0x14,
      Separator = 0x15,
      Toolbar = 0x16,
      StatusBar = 0x17,
      Table = 0x18,
      ColumnHeader = 0x19,
      RowHeader = 0x1a,
      Column = 0x1b,
      Row = 0x1c,
      Cell = 0x1d,
      Link = 0x1e,
      HelpBalloon = 0x1f,
      Character = 0x20,
      List = 0x21,
      ListItem = 0x22,
      Outline = 0x23,
      OutlineItem = 0x24,
      PageTab = 0x25,
      PropertyPage = 0x26,
      Indicator = 0x27,
      Graphic = 0x28,
      StaticText = 0x29,
      Text = 0x2a,
      PushButton = 0x2b,
      CheckButton = 0x2c,
      RadioButton = 0x2d,
      Combobox = 0x2e,
      DropList = 0x2f,
      ProgressBar = 0x30,
      Dial = 0x31,
      HotkeyField = 0x32,
      Slider = 0x33,
      SpinButton = 0x34,
      Diagram = 0x35,
      Animation = 0x36,
      Equation = 0x37,
      ButtonDropDown = 0x38,
      ButtonMenu = 0x39,
      ButtonDropDownGrid = 0x3a,
      Whitespace = 0x3b,
      PageTabList = 0x3c,
      Clock = 0x3d,
      SplitButton = 0x3e,
      IPAddress = 0x3f,
      OutlineButton = 0x40,
   }
}
 
//////////////////////////////////////////////////////////////////////// 
// C:\code\jeremy.sellars\WinUIScraper\src\WinUIScraper\Providers\Msaa\UIAccessibleHelper.cs 
 
﻿namespace WinUIScraper.Providers.Msaa
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Runtime.InteropServices;

   using Accessibility;

   public static class UIAccessibleHelper
   {
      private const int OBJID_WINDOW = 0;

      [DllImport("oleacc.dll", PreserveSig = false)]
      [return: MarshalAs(UnmanagedType.Interface)]
      public static extern object AccessibleObjectFromWindow(IntPtr hWnd, uint dwId, ref Guid riid);

      //Declare Function AccessibleChildren Lib "oleacc" _
      //      (ByVal paccContainer As IAccessible, ByVal iChildStart As Long, _
      //       ByVal cChildren As Long, rgvarChildren As Variant, _
      //       pcObtained As Long) As Long
      [DllImport("oleacc.dll")]
      public static extern uint AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [Out] object[] rgvarChildren, out int pcObtained);
      
      public static KeyValuePair<string,string> [] GetValues (IntPtr hWnd)
      {
         var list = new List<KeyValuePair<string,string>>();
         AddValues(list, GetAccessibleObjectFromWindow(hWnd));
         return list.ToArray();
      }

      public static IAccessible GetAccessibleObjectFromWindow(IntPtr hWnd)
      {
         Guid uid = new Guid("618736e0-3c3d-11cf-810c-00aa00389b71");
         return (IAccessible) AccessibleObjectFromWindow(hWnd, OBJID_WINDOW, ref uid);
      }

      public static void AddValues (List<KeyValuePair<string,string>> values, IAccessible accObj)
      {
         string name = accObj.accName;
         object value = accObj.accValue ?? accObj.accName ?? accObj.accDescription ?? accObj.accRole ?? accObj.accState;
         if (value != null)
            values.Add(new KeyValuePair<string, string>(name, value as string ?? "<non string>"));
         int count;
         object [] myList = new object[100];
         AccessibleChildren(accObj, 0, myList.Length, myList, out count);
         foreach (var c in myList)
         {
            if (c == null)
               continue;
            var child = c as IAccessible;
            if (child == null)
               continue;
            try
            {
               AddValues(values, child);
            }
            catch (Exception e)
            {
               Console.Error.WriteLine(e);
            }
         }
      }

      public static string GetStringValue(this IAccessible element)
      {
         return element.accValue;
      }

      public static string GetValueAsString(this IAccessible element)
      {
         try
         {
            return element.accValue;
         }
         catch (Exception)
         {
            try
            {
               return element.accValue[0];
            }
            catch (Exception e2)
            {
               LogError(e2.ToString());
               return null;
            }
         }
      }

      public static string GetDescriptionSafe(this IAccessible element)
      {
         try
         {
            return element.accDescription;
         }
         catch (Exception)
         {
            try
            {
               return element.accDescription[0];
            }
            catch (Exception e2)
            {
               LogError(e2.ToString());
            }
            return null;
         }
      }
      public static object GetStateSafe(this IAccessible element)
      {
         try
         {
            return element.accState;
         }
         catch (Exception e)
         {
            try
            {
               return element.accState[0];
            }
            catch (Exception e2)
            {
               LogError(e2.ToString());
            }
            LogError(e.ToString());
            return null;
         }
      }

      public static string GetNameSafe(this IAccessible element)
      {
         try
         {
            return element.accName ?? element.accName[0];
         }
         catch (Exception e)
         {
            try
            {
               return element.accName[0];
            }
            catch (Exception e2)
            {
               LogError(e2.ToString());
               try
               {
                  return element.accName[1];
               }
               catch (Exception e3)
               {
                  LogError(e3.ToString());
                  try
                  {
                     return ((IAccessible)element.accChild[0]).GetNameSafe();
                  }
                  catch (Exception e4)
                  {
                     LogError(e4.ToString());
                  }
               }
            }
            LogError(e.ToString());
            return e.ToString();
         }
      }

      public static string GetName(this IAccessible element)
      {
         return element.accName;
      }

      public static AccRole GetRoleSafe(this IAccessible element)
      {
         try
         {
            try
            {
               return (AccRole)(int)element.accRole;
            }
            catch(ArgumentException)
            {
               try
               {
                  return (AccRole)(int)element.accRole[0];
               }
               catch (ArgumentException)
               {
                  return (AccRole)(int)element.accRole[1];
               }
            }
         }
         catch(Exception)
         {
            return (AccRole)(-1);
         }
      }

      public static bool IsRoleSafe(this IAccessible element, AccRole role)
      {
         var actualRole = element.GetRoleSafe();
         return actualRole == default(AccRole) || actualRole == role;
      }

      public static AccRole GetRole(IAccessible element)
      {
         try
         {
            return (AccRole)(int)element.accRole;
         }
         catch(ArgumentException)
         {
            try
            {
               return (AccRole)(int)element.accRole;
            }
            catch (ArgumentException)
            {
               return (AccRole)(int)element.accRole;
            }
         }
      }

      public static IEnumerable<IAccessible> GetChildren(this IAccessible element)
      {
         int count;
         object [] myList = new object[100];
         AccessibleChildren(element, 0, myList.Length, myList, out count);
         List<IAccessible> children = new List<IAccessible>(count);
         for (int i = 0; i < count; i++)
         {
            var c = myList[i];
            if (c is IAccessible)
               children.Add((IAccessible)c);
         }
         return children;
      }
      public static IEnumerable<IAccessible> FindChildrenByName(this IAccessible element, string name)
      {
         return GetChildren(element).Where(e => name == e.GetNameSafe());
      }

      public static IEnumerable<IAccessible> FindDescendantsNamed(this IAccessible element, string name)
      {
         return GetDescendants(element).Where(e => name == e.GetNameSafe());
      }

      public static IEnumerable<IAccessible> GetDescendants(this IAccessible element)
      {
         Stack<IEnumerator<IAccessible>> enumerators = new Stack<IEnumerator<IAccessible>>();
         enumerators.Push(element.GetChildren().GetEnumerator());
         while (enumerators.Count > 0)
         {
            var enumerator = enumerators.Peek();
            if (!enumerator.MoveNext())
               enumerators.Pop();
            else
            {
               var decendant = enumerator.Current;
               if (decendant == null)
                  continue;
               yield return decendant;
               enumerators.Push(decendant.GetChildren().GetEnumerator());
            }
         }
      }

      public static IEnumerable<IAccessible> FindDescendantsByRole(this IAccessible element, AccRole role)
      {
         return GetDescendants(element);
      }
      private static void LogError(string message)
      {
         Console.Error.WriteLine(Environment.NewLine + message + Environment.NewLine);
      }
   }
}
 
//////////////////////////////////////////////////////////////////////// 
// C:\code\jeremy.sellars\WinUIScraper\src\WinUIScraper\Providers\UIAutomation\AutomationExtensions.cs 
 
﻿namespace WinUIScraper.Providers.UIAutomation
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
//////////////////////////////////////////////////////////////////////// 
// C:\code\jeremy.sellars\WinUIScraper\src\WinUIScraper\Providers\UIAutomation\ElementDefinition.cs 
 
﻿namespace WinUIScraper.Providers.UIAutomation
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
