// WinUIScraper - A declarative approach to screen scraping in Windows 
 
// Built on Wed 11/21/2012 16:08:15.77 
 
 
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