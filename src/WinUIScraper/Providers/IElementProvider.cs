namespace WinUIScraper.Providers
{
   public interface IElementProvider<TSource, TData>
   {
      TSource GetElement(TSource element);
      TData GetValue(TSource element);
   }
}