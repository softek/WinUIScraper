namespace WinUIScraper.Declarative
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
