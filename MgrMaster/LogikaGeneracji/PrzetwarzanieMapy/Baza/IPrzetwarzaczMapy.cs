namespace LogikaGeneracji.PrzetwarzaczeMapy.Baza
{
   public interface IPrzetwarzaczMapy
   {
      IPrzetwarzaczMapy Nastepnik { get; set; }
      void Przetwarzaj(IMapa mapa);
   }
}