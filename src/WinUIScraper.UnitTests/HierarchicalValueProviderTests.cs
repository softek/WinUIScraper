using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIScraper.Declarative;
using WinUIScraper.Providers;
using SutNode=WinUIScraper.Declarative.KeyedTreeNode<string, string>;

namespace WinUIScraper.UnitTests
{
   [TestClass]
   public class HierarchicalValueProviderTests
   {
      private static readonly TreeNodeElementProvider<string, string> SimpleTreeNodeElementProvider = new TreeNodeElementProvider<string, string>();
      private const string RootValue = "RootValue";

      [TestMethod]
      public void GetRootElement()
      {
         var root = CreateRoot();
         var provider = CreateHierarchicalValueProvider(root);
         SutNode element = provider.GetElement(GetSameElement);
         Assert.AreEqual(root, element);
      }

      [TestMethod]
      public void GetElementsForRoot()
      {
         var root = CreateRoot();
         var provider = CreateHierarchicalValueProvider(root);
         IEnumerable<SutNode> elements = provider.GetElements(GetChildElements);
         Assert.AreEqual(0, elements.Count());
      }

      [TestMethod]
      public void GetElementsForE()
      {
         var eDataTree = CreateEDataTree();
         var provider = CreateHierarchicalValueProvider(eDataTree);
         IEnumerable<SutNode> elements = provider.GetElements(GetChildElements);
         CollectionAssert.AreEqual(eDataTree.Children.ToList(), elements.ToList());
      }

      private static IEnumerable<SutNode> GetChildElements(SutNode node)
      {
         return node.Children;
      }

      [TestMethod]
      public void GetRootValue()
      {
         var provider =CreateHierarchicalValueProvider(CreateRoot());
         string value = provider.GetValue(GetSameElement, GetValue);
         Assert.AreEqual(RootValue, value);
      }

      [TestMethod]
      public void GetValuesE()
      {
         var provider = CreateHierarchicalValueProvider(CreateRoot());
         Dictionary<string, List<string>> values = provider.GetValues(GetETree());

         Assert.AreEqual(1, values.Count, "Count");
         var kvp = values.First();
         Assert.AreEqual("E", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] {"RootValue"}, kvp.Value, "Value");
      }

      [TestMethod]
      public void GetValuesEEdward()
      {
         var provider = CreateHierarchicalValueProvider(CreateEDataTree());
         Dictionary<string, List<string>> values = provider.GetValues(GetEEdwardPatternTree());

         Assert.AreEqual(2, values.Count, "Count");
         var kvp = values.First();
         Assert.AreEqual("E", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] { "RootValue" }, kvp.Value, "0:Value");
         kvp = values.Skip(1).First();
         Assert.AreEqual("Edward", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] { "EdwardValue" }, kvp.Value, "1:Value");
      }

      [TestMethod]
      public void GetValuesBBoyWhereNoneMatch()
      {
         var provider = CreateHierarchicalValueProvider(CreateBDataTree());
         Dictionary<string, List<string>> values = provider.GetValues(GetNoneMatchPatternTree());

         Assert.AreEqual(1, values.Count, "Count");
         var kvp = values.First();
         Assert.AreEqual("NoneMatch", kvp.Key, "Key");
         CollectionAssert.AreEqual(new object [0], kvp.Value, "0:Value");
      }

      [TestMethod]
      public void GetValuesBoys()
      {
         var provider = CreateHierarchicalValueProvider(CreateBDataTree());
         Dictionary<string, List<string>> values = provider.GetValues(GetChildrenPatternTree());

         Assert.AreEqual(1, values.Count, "Count");
         var kvp = values.First();
         Assert.AreEqual("Children", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] { "BananasValue" , "BBQValue"}, kvp.Value, "1:Value");
      }

      private static KeyedTreeNode<string, IElementsProvider<SutNode, string>> GetChildrenPatternTree()
      {
         return CreateKeyedTreeNode("Children", new TreeNodeElementsProvider<string, string>(r => r.Children));
      }

      [TestMethod]
      public void GetValuesBAndBoys()
      {
         var provider = CreateHierarchicalValueProvider(CreateBDataTree());
         Dictionary<string, List<string>> values = provider.GetValues(GetBAndBoysPatternTree());

         Assert.AreEqual(2, values.Count, "Count");
         var kvp = values.First();
         Assert.AreEqual("B", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] { "RootValue" }, kvp.Value, "0:Value");
         kvp = values.Skip(1).First();
         Assert.AreEqual("Boys", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] { "BananasValue", "BBQValue" }, kvp.Value, "1:Value");
      }

      private static KeyedTreeNode<string, IElementsProvider<SutNode, string>> GetBAndBoysPatternTree()
      {
         var b = CreateKeyedTreeNode("B", new TreeNodeElementsProvider<string, string>(r => new []{r}));
         b.Add(CreateKeyedTreeNode("Boys", new TreeNodeElementsProvider<string, string>(r => r.Children)));
         return b;
      }

      [TestMethod]
      public void GetValuesEChild()
      {
         var provider = CreateHierarchicalValueProvider(CreateEDataTree());
         Dictionary<string, List<string>> values = provider.GetValues(GetEChildPatternTree());

         Assert.AreEqual(2, values.Count, "Count");
         var kvp = values.First();
         Assert.AreEqual("E", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] { "RootValue" }, kvp.Value, "0:Value");
         kvp = values.Skip(1).First();
         Assert.AreEqual("Child", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] { "EdwardValue", "ElizaValue" }, kvp.Value, "1:Value");
      }

      [TestMethod]
      public void GetPatientIdAndAccessionFromMockUi()
      {
         var provider = CreateHierarchicalValueProvider(CreateMockMedUiDataTree());
         Dictionary<string, List<string>> values = provider.GetValues(GetMrnAndAccessionPatternTree());

         Assert.AreEqual(2, values.Count, "Count");
         var kvp = values.First();
         Assert.AreEqual("PatientId", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] { "p1234" }, kvp.Value, "PatientId");
         kvp = values.Skip(1).First();
         Assert.AreEqual("Accession", kvp.Key, "Key");
         CollectionAssert.AreEqual(new[] { "a123456789" }, kvp.Value, "acc");
      }

      private static SutNode CreateMockMedUiDataTree(CoPathPatientBanner info = null)
      {
         return CreateMockDesktop(CreateMockNotepad(), CreateMockCoPath(info), CreateMockNotepad());
      }

      private static SutNode CreateMockDesktop(params SutNode [] apps)
      {
         var desktop = new SutNode("Desktop", "(desktop)");
         desktop.AddRange(apps);
         return desktop;
      }

      class CoPathPatientBanner
      {
         private string name = "johnny q public";
         private string patientId = "p1234";
         private string dateOfBirth = "5/13/2000";
         private string accession = "a123456789";

         public string Name
         {
            get { return name; }
            set { name = value; }
         }

         public string PatientId
         {
            get { return patientId; }
            set { patientId = value; }
         }

         public string DateOfBirth
         {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
         }

         public string Accession
         {
            get { return accession; }
            set { accession = value; }
         }
      }
      private static SutNode CreateMockCoPath(CoPathPatientBanner info = null)
      {
         info = info ?? new CoPathPatientBanner();
         return new SutNode("CoPath", "CoPath blah blah blah")
                   {
                      new SutNode("Frame", "")
                         {
                            CreateMockPatientBanner(info)
                         },
                      new SutNode("Frame", "Junk")
                         {
                            new SutNode("Frame2", "")
                         }
                   };
      }

      private static SutNode CreateMockPatientBanner(CoPathPatientBanner info)
      {
         return
            new SutNode("Frame2", "")
               {
                  new SutNode("patient name", info.Name),
                  new SutNode("med_rec_number", info.PatientId),
                  new SutNode("patient dob", info.DateOfBirth),
                  new SutNode("accession", info.Accession),
               };
      }

      private static SutNode CreateMockNotepad()
      {
         return new SutNode("Notepad", "My cool file.txt - Notepad")
                   {
                      new SutNode("File", "&File"),
                      new SutNode("Edit", "&Edit")
                   };
      }

      private static KeyedTreeNode<string, IElementsProvider<SutNode, string>> GetMrnAndAccessionPatternTree()
      {
         var frame2 = CreateKeyedTreeNode(null, new TreeNodeElementsProvider<string, string>(r => r.EnumerateDescendants().Where(IsFrame2)));
         frame2.Add(CreateKeyedTreeNode("PatientId", new TreeNodeElementsProvider<string, string>(r => r.Children.Where(IsMrn))));
         frame2.Add(CreateKeyedTreeNode("Accession", new TreeNodeElementsProvider<string, string>(r => r.Children.Where(IsAccession))));
         var e = CreateKeyedTreeNode(null, new TreeNodeElementsProvider<string, string>(r => r.Children.Where(IsCoPath)));
         e.Add(frame2);
         return e;
      }

      private static bool IsCoPath(SutNode n)
      {
         return n.Key == "CoPath";
      }

      private static bool IsFrame2(SutNode n)
      {
         return n.Key == "Frame2";
      }

      private static bool IsAccession(SutNode n)
      {
         return n.Key == "accession";
      }

      private static bool IsMrn(SutNode n)
      {
         if (n.Key == "med_rec_number")
           return true;
         return false;
      }

      private static KeyedTreeNode<string, IElementProvider<SutNode, string>> GetETree()
      {
         return CreateKeyedTreeNode("E", SimpleTreeNodeElementProvider);
      }

      private static KeyedTreeNode<string, IElementProvider<SutNode, string>> CreateKeyedTreeNode(string key, TreeNodeElementProvider<string, string> treeNodeElementProvider)
      {
         return new KeyedTreeNode<string, IElementProvider<SutNode, string>>(key, treeNodeElementProvider);
      }

      private static KeyedTreeNode<string, IElementsProvider<SutNode, string>> CreateKeyedTreeNode(string key, TreeNodeElementsProvider<string, string> treeNodeElementsProvider)
      {
         return new KeyedTreeNode<string, IElementsProvider<SutNode, string>>(key, treeNodeElementsProvider);
      }

      private static KeyedTreeNode<string, IElementProvider<SutNode, string>> GetEEdwardPatternTree()
      {
         var e = CreateKeyedTreeNode("E", SimpleTreeNodeElementProvider);
         e.Add(CreateKeyedTreeNode("Edward", new TreeNodeElementProvider<string, string>(r => r.Children.First(n => n.Key == "Edward"))));
         return e;
      }

      private static KeyedTreeNode<string, IElementProvider<SutNode, string>> GetEChildPatternTree()
      {
         var e = CreateKeyedTreeNode("E", SimpleTreeNodeElementProvider);
         e.Add(CreateKeyedTreeNode("Child", new TreeNodeElementProvider<string, string>(r => r.Children.First(n => n.Key == "Edward"))));
         e.Add(CreateKeyedTreeNode("Child", new TreeNodeElementProvider<string, string>(r => r.Children.First(n => n.Key == "Eliza"))));
         return e;
      }

      private static KeyedTreeNode<string, IElementsProvider<SutNode, string>> GetNoneMatchPatternTree()
      {
         return CreateKeyedTreeNode("NoneMatch", new TreeNodeElementsProvider<string, string>(r => Enumerable.Empty<SutNode>()));
      }

      private static T GetSameElement<T>(T item)
      {
         return item;
      }

      private static TValue GetValue<TValue>(KeyedTreeNode<string,TValue> node)
      {
         return node.Value;
      }

      private static HierarchicalValueProvider<SutNode,string,string> CreateHierarchicalValueProvider(SutNode root)
      {
         return new HierarchicalValueProvider<SutNode,string,string>(root);
      }

      private static SutNode CreateRoot()
      {
         return new SutNode("root", RootValue);
      }

      private static SutNode CreateEDataTree()
      {
         return
            new SutNode("root", RootValue)
               {
                  new SutNode("Edward", "EdwardValue"),
                  new SutNode("Eliza", "ElizaValue"),
               };
      }

      private static SutNode CreateBDataTree()
      {
         return
            new SutNode("B", RootValue)
               {
                  new SutNode("Bananas", "BananasValue"),
                  new SutNode("BBQ", "BBQValue"),
               };
      }

   }
}
