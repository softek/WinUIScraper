using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIScraper.Declarative;

namespace WinUIScraper.UnitTests
{
   [TestClass]
   public class KeyedTreeTests
   {
      [TestMethod]
      public void KeyIsSetCorrectlyInConstructor()
      {
         var n = CreateNode("key", (string) null);
         Assert.AreEqual("key", n.Key, "Key");
      }

      [TestMethod]
      public void ValueIsSetCorrectlyInConstructor()
      {
         var val = new object();
         var n = CreateNode("key", val);
         Assert.AreEqual(val, n.Value, "Value");
      }

      [TestMethod]
      public void ChildrenArePresentAfterAdd()
      {
         var n = CreateNode("key", new object());
         var expected = new[] {
            CreateNode("a1", new object()),
            CreateNode("a2", new object()),
         };
         foreach (var c in expected)
            n.Add(c);
         CollectionAssert.AreEqual(expected, n.Children.ToList());
      }

      [TestMethod]
      public void ChildrenArePresentAfterAddRange()
      {
         var n = CreateNode("key", new object());
         var expected = new[] {
            CreateNode("c1", new object()),
            CreateNode("c2", new object()),
         };
         n.AddRange(expected);
         CollectionAssert.AreEqual(expected, n.Children.ToList());
      }

      [TestMethod]
      public void CanNestEasillyFromCode()
      {
         new KeyedTreeNode<string, object>("root", null)
            {
               new KeyedTreeNode<string, object>("root.a", null),
               new KeyedTreeNode<string, object>("root.b", null)
                  {
                     new KeyedTreeNode<string, object>("root.b.a", null)
                  },
            };
      }

      [TestMethod]
      public void CanEnumerateDescendantsWhenThereAreNoChildren()
      {
         var node = new KeyedTreeNode<string, object>("root", null);
         CollectionAssert.AreEqual(new object[0], node.EnumerateDescendants().ToList());
      }

      [TestMethod]
      public void CanEnumerateDescendantsWhenThereIsOneChild()
      {
         KeyedTreeNode<string, object> child = new KeyedTreeNode<string, object>("child", null);
         var node = new KeyedTreeNode<string, object>("root", null){child};
         CollectionAssert.AreEqual(new []{child}, node.EnumerateDescendants().ToList());
      }

      [TestMethod]
      public void CanEnumerateDescendantsWhenThereAreTwoChildren()
      {
         var children =
            new[]
               {
                  new KeyedTreeNode<string, object>("child1", null),
                  new KeyedTreeNode<string, object>("child2", null),
               };
         var node = new KeyedTreeNode<string, object>("root", null);
         node.AddRange(children);
         CollectionAssert.AreEqual(children, node.EnumerateDescendants().ToList());
      }

      [TestMethod]
      public void CanEnumerateDescendantsWhenThereAreTwoChildrenAndASharedGrandChild()
      {
         var c1c1 = new KeyedTreeNode<string, object>("child1.child1", null);
         var c1c2 = new KeyedTreeNode<string, object>("child1.child2", null);
         var c1 = new KeyedTreeNode<string, object>("child1", null) {c1c1, c1c2};
         var c2c1 = new KeyedTreeNode<string, object>("child2.child1", null);
         var c2c2 = new KeyedTreeNode<string, object>("child2.child2", null);
         var c2 = new KeyedTreeNode<string, object>("child2", null) { c2c1, c2c2 };
         var children = new[] {c1, c2 };
         var node = new KeyedTreeNode<string, object>("root", null);
         node.AddRange(children);
         CollectionAssert.AreEqual(new [] {c1,c1c1, c1c2, c2, c2c1, c2c2}, node.EnumerateDescendants().ToList());
      }

      private static KeyedTreeNode<TKey, TValue> CreateNode<TKey, TValue>(TKey key, TValue value)
      {
         return new KeyedTreeNode<TKey, TValue>(key, value);
      }
   }
}
