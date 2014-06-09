using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class WyrownywaczTerenuWody : BazaPrzetwarzacza
   {
      private readonly float _poziom;

      public WyrownywaczTerenuWody(float poziom = 0f)
      {
         _poziom = poziom;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            if (komorka.Dane.Podloze == Podloze.Woda)
            {
               komorka.Punkt.Wysokosc = _poziom;
            }
         }

         foreach (IRog rog in mapa.Rogi)
         {
            if (rog.Komorki.All(k => k.Dane.Podloze != Podloze.Ziemia))
               rog.Punkt.Wysokosc = _poziom;
            else if (rog.Punkt.Wysokosc < 0f)
               rog.Punkt.Wysokosc = 0f;
         }
      }
   }
}