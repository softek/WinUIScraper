using System.Collections.Generic;
using WinUIScraper.Declarative;
using WinUIScraper.Providers;

namespace WinUIScraper.UnitTests
{
   class TreeNodeElementsProvider<TKey, TValue> : IElementsProvider<KeyedTreeNode<TKey, TValue>, TValue>
   {
      private readonly ElementsProvider provider;

      private static IEnumerable<KeyedTreeNode<TKey, TValue>> SameElement(KeyedTreeNode<TKey, TValue> node)
      {
         return new [] {node};
      }

      public delegate IEnumerable<KeyedTreeNode<TKey, TValue>> ElementsProvider(KeyedTreeNode<TKey, TValue> node);

      public TreeNodeElementsProvider(ElementsProvider provider = null)
      {
         this.provider = provider ?? new ElementsProvider(SameElement);
      }
      public IEnumerable<KeyedTreeNode<TKey, TValue>> GetElements(KeyedTreeNode<TKey, TValue> element)
      {
         return provider(element);
      }
      public TValue GetValue(KeyedTreeNode<TKey, TValue> element)
      {
         return element.Value;
      }
   }
}