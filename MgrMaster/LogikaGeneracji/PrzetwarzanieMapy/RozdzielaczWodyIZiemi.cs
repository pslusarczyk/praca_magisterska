using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class RozdzielaczWodyIZiemi : BazaPrzetwarzacza
   {
      private float _prog;

      public RozdzielaczWodyIZiemi(float prog)
      {
         _prog = prog;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            if (komorka.Punkt.Wysokosc > _prog)
               komorka.Dane.Podloze = Podloze.Ziemia;
            else
               komorka.Dane.Podloze = Podloze.Woda;
         }
      }
   }
}