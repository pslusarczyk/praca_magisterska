namespace LogikaGeneracji.PrzetwarzanieMapy.Baza
{
   public interface IPrzetwarzaczMapy
   {
      IPrzetwarzaczMapy Nastepnik { get; set; }
      void Przetwarzaj(IMapa mapa);
   }
}