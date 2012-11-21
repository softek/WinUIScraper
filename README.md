WinUIScraper
============

Harvest data from other windows applications

Screen Scraping
------------------------------------

Use a declarative approach to screen scraping with WinUIScraper.

For your convenience, [WinUIScraper.cs](blob/master/WinUIScraper.cs) contains the entire library as a single C# file.  Of course if you'd rather use the Visual Studio Solution, that's found [here](tree/master/src).

Here's a [quick example](blob/master/src/WinUIScraper.UnitTests/NotepadTests.cs) of extracting data from Notepad.

    [TestClass]
    public class NotepadTests
    {
      private static readonly string[] NotepadMenuNames = new[] {"File", "Edit", "Format", "View", "Help"};

      [TestMethod]
      public void StartAndFindNotepad()
      {
         RunTestWithNotepad(
            windowElement => Assert.AreEqual("Untitled - Notepad", windowElement.GetName()));
      }

      private static void RunTestWithNotepad(Action<AutomationElement> test)
      {
         using (var process = StartNotepadAndWaitForInputIdle())
         {
            try
            {
               test(GetMainWindowOf(process));
            }
            finally
            {
               process.Kill();
            }
         }
      }

      private static Process StartNotepadAndWaitForInputIdle()
      {
         var process = Process.Start(@"notepad.exe");
         if (process == null) 
            throw new Exception("Notepad did not start");
         process.WaitForInputIdle();
         return process;
      }

      private static AutomationElement GetMainWindowOf(Process process)
      {
         return AutomationElement.FromHandle(process.MainWindowHandle);
      }

      [TestMethod]
      public void EnsureNotepadHasExpectedMenus()
      {
         RunTestWithNotepad(EnsureNotepadHasExpectedMenus);
      }

      private static void EnsureNotepadHasExpectedMenus(AutomationElement windowElement)
      {
         var menuBar = windowElement.FindFirstDescendantById("MenuBar");
         menuBar.ExecuteWithUpdatedCache(
            AutomationExtensions.BuildCacheRequest(TreeScope.Element | TreeScope.Children, AutomationElement.NameProperty),
            m => CollectionAssert.AreEqual(NotepadMenuNames, GetChildNames(m)));
      }

      private static List<string> GetChildNames(AutomationElement ae)
      {
         return ae.CachedChildren.Cast<AutomationElement>().Select(AutomationExtensions.GetName).ToList();
      }
      [TestMethod]
      public void EnsureNotepadHasExpectedMenusWithHierarchicalValueProvider()
      {
         RunTestWithNotepad(EnsureNotepadHasExpectedMenusWithHierarchicalValueProvider);
      }

      private static void EnsureNotepadHasExpectedMenusWithHierarchicalValueProvider(AutomationElement windowElement)
      {
         var hvp = new HierarchicalValueProvider<AutomationElement, string, string>(windowElement);
         var menuBar = windowElement.FindFirstDescendantById("MenuBar");
         menuBar.ExecuteWithUpdatedCache(
            AutomationExtensions.BuildCacheRequest(TreeScope.Element | TreeScope.Children, AutomationElement.NameProperty),
            m =>
               {
                  var values = hvp.GetValues(BuildNotepadMenuTree());
                  CollectionAssert.AreEqual(NotepadMenuNames, values["menu"]);
               });
      }

      private static KeyedTreeNode<string, IElementsProvider<AutomationElement, string>> BuildNotepadMenuTree()
      {
         var menubar = new KeyedTreeNode<string, IElementsProvider<AutomationElement, string>>(
            null,
            new AutomationElementsProvider(
               windowElement => new[] {windowElement.FindFirstDescendantById("MenuBar")},
               null));
         menubar.Add(new KeyedTreeNode<string, IElementsProvider<AutomationElement, string>>(
            "menu",
            new AutomationElementsProvider(
               windowElement => windowElement.CachedChildren.Cast<AutomationElement>(),
               AutomationExtensions.GetName)));
         return menubar;
      }

      private class AutomationElementsProvider : DelegatedElementsProvider<AutomationElement, string>
      {
         public AutomationElementsProvider(Func<AutomationElement, IEnumerable<AutomationElement>> sourcesSelector, Func<AutomationElement,string> dataSelector)
            : base (sourcesSelector, dataSelector)
         {
         }
      }
    }
