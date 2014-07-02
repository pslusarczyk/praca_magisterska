using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class AktualizatorBiomow : BazaPrzetwarzacza
   {
      private readonly KonfigAktualizatoraBiomow _konfiguracja;
      private readonly float _minTemp;
      private readonly float _maksTemp;
      private readonly float _minWilg;
      private readonly float _maksWilg;

      public AktualizatorBiomow(KonfigAktualizatoraBiomow konfig, 
                  float minTemp = 0f, float maksTemp = 1f, float minWilg = 0f, float maksWilg = 1f)
      {
         _konfiguracja = konfig;
         _minTemp = minTemp;
         _maksTemp = maksTemp;
         _minWilg = minWilg;
         _maksWilg = maksWilg;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki.Where(k => k.Dane.Typ == TypKomorki.Lad))
         {
            float normTemp = Mathf.Clamp01(     (komorka.Dane.Temperatura + _minTemp)/(_maksTemp - _minTemp));
            float normWilg = Mathf.Clamp01(     (komorka.Dane.Wilgotnosc + _minWilg) / (_maksWilg - _minWilg));
            komorka.Dane.Biom = _konfiguracja.PobierzBiom(normTemp, normWilg);
         }
      }
   }
}