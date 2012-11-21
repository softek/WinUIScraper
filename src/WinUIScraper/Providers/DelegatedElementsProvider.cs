namespace WinUIScraper.Providers
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