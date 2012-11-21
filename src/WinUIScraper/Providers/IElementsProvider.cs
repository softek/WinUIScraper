namespace WinUIScraper.Providers
{
   using System.Collections.Generic;

   public interface IElementsProvider<TSource, TData>
   {
      IEnumerable<TSource> GetElements(TSource element);
      TData GetValue(TSource element);
   }
}