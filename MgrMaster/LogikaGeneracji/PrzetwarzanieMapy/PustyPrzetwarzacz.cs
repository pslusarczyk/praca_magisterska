using LogikaGeneracji.PrzetwarzaczeMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class PustyPrzetwarzacz : IPrzetwarzaczMapy
   {
      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public void Przetwarzaj(IMapa mapa){}
   }
}