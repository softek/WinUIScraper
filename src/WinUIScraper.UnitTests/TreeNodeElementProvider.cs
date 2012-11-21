using System.Collections.Generic;
using WinUIScraper.Declarative;
using WinUIScraper.Providers;

namespace WinUIScraper.UnitTests
{
   class TreeNodeElementProvider<TKey, TValue> : IElementProvider<KeyedTreeNode<TKey, TValue>, TValue>
   {
      private readonly ElementProvider provider;

      private static KeyedTreeNode<TKey, TValue> SameElement(KeyedTreeNode<TKey, TValue> node)
      {
         return node;
      }

      public delegate KeyedTreeNode<TKey, TValue> ElementProvider(KeyedTreeNode<TKey, TValue> node);

      public TreeNodeElementProvider(ElementProvider provider = null)
      {
         this.provider = provider ?? new ElementProvider(SameElement);
      }
      public KeyedTreeNode<TKey, TValue> GetElement(KeyedTreeNode<TKey, TValue> element)
      {
         return provider(element);
      }
      public TValue GetValue (KeyedTreeNode<TKey,TValue> element)
      {
         return element.Value;
      }
   }
}
