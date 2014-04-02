namespace LogikaGeneracji.PrzetwarzaczeMapy
{
   public interface IPrzetwarzaczMapy
   {
      IPrzetwarzaczMapy Nastepnik { get; set; }
      void Przetwarzaj(IMapa mapa);
   }
}