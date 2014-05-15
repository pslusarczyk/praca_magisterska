using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class AktualizatorBiomow : IPrzetwarzaczMapy
   {
      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public void Przetwarzaj(IMapa mapa)
      {
         throw new System.NotImplementedException();
      }
   }
}