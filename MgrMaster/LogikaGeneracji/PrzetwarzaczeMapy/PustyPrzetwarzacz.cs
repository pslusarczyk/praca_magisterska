namespace LogikaGeneracji.PrzetwarzaczeMapy
{
   public class PustyPrzetwarzacz : IPrzetwarzaczMapy
   {
      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public void Przetwarzaj(IMapa mapa){}
   }
}