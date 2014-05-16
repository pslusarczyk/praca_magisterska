namespace LogikaGeneracji.PrzetwarzanieMapy.Baza
{
   public abstract class BazaPrzetwarzacza : IPrzetwarzaczMapy
   {
      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public abstract void Przetwarzaj(IMapa mapa);
   }
}