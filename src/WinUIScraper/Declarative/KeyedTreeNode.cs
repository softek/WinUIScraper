namespace WinUIScraper.Declarative
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
