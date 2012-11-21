namespace WinUIScraper.Providers.Msaa
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
