using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;
using Random = System.Random;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class LiniowyModyfikatorWysokosci : BazaPrzetwarzacza
   {
      private readonly float _zmiana;

      readonly Random Rand = new Random();

      public LiniowyModyfikatorWysokosci(float zmiana)
      {
         _zmiana = zmiana;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IPunkt punkt in mapa.Punkty)
         {
            punkt.Wysokosc += _zmiana;
         }
      }
   }
}