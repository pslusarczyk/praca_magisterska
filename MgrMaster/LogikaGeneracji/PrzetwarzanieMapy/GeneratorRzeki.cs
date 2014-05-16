using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class GeneratorRzeki : BazaPrzetwarzacza, IGeneratorRzeki
   {
      public const float GruboscJednostkowa = 1f;

      private float _aktualnaGrubosc;
      private IMapa _mapa;
      private IList<IOdcinekRzeki> _odcinki;

      public IPunkt PunktPoczatkowy { get; set; }
      public bool? UdaloSieUtworzyc { get; set; }

      public GeneratorRzeki(IPunkt punktPoczatkowy)
      {
         PunktPoczatkowy = punktPoczatkowy;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         _mapa = mapa;
         _aktualnaGrubosc = GruboscJednostkowa;
         _odcinki = new List<IOdcinekRzeki>();

         SprobujUtworzycRzeke();
      }

      // Todo Poprawiæ pod k¹tem zagnie¿d¿eñ.?
      private void SprobujUtworzycRzeke()
      {
         IPunkt aktualnyPunkt = PunktPoczatkowy;
         IPunkt nastepnyPunkt = aktualnyPunkt.Nastepnik;
         IRzeka kolizyjnaRzeka = null;
         while (nastepnyPunkt != null)
         {
            _odcinki.Add(new OdcinekRzeki
            {
               Grubosc = GruboscJednostkowa,
               PunktA = aktualnyPunkt,
               PunktB = nastepnyPunkt
            });
            kolizyjnaRzeka = _mapa.Rzeki.FirstOrDefault(rz => rz.Odcinki.Any(o => o.PunktA == nastepnyPunkt));
            if (kolizyjnaRzeka != null)
            {
               bool kolidujacaJestDluzsza = _odcinki.Count < kolizyjnaRzeka.DlugoscDoPunktu(nastepnyPunkt);
               ObsluzKolizje(kolidujacaJestDluzsza, kolizyjnaRzeka, nastepnyPunkt);
               if (kolidujacaJestDluzsza)
               {
                  aktualnyPunkt = nastepnyPunkt;
                  break;
               }
               else
               {
                  aktualnyPunkt = _odcinki.Last().PunktB;
                  nastepnyPunkt = aktualnyPunkt.Nastepnik;
                  continue;
               }
            }

            aktualnyPunkt = nastepnyPunkt;
            nastepnyPunkt = aktualnyPunkt.Nastepnik;
         }

         if (kolizyjnaRzeka != null || KoncoweMiejsceJestNaBrzegu(_mapa, aktualnyPunkt))
         {
            UdaloSieUtworzyc = true;
            _mapa.Rzeki.Add(new Rzeka {Odcinki = _odcinki});
            foreach (IPunkt punkt in _mapa.Punkty)
            {
               punkt.ZawieraRzeke = true;
            }
         }
         else
         {
            UdaloSieUtworzyc = false;
         }
      }

      private void ObsluzKolizje(bool kolidujacaJestDluzsza, IRzeka kolizyjnaRzeka, IPunkt punktKolizji)
      {
         if (kolidujacaJestDluzsza)
         {
            PogrubRzekeOdPunktu(kolizyjnaRzeka, punktKolizji, _aktualnaGrubosc);
         }
         else
         {
            float gruboscDoDodania = kolizyjnaRzeka.Odcinki.First(o => o.PunktA == punktKolizji).Grubosc;
            _aktualnaGrubosc += gruboscDoDodania;
            IList<IOdcinekRzeki> wycinek = WytnijCzescRzekiOdPunktu(kolizyjnaRzeka, punktKolizji);
            _odcinki = _odcinki.Concat(wycinek).ToList();
         }
      }

      private IList<IOdcinekRzeki> WytnijCzescRzekiOdPunktu(IRzeka rzeka, IPunkt punkt)
      {
         int indeks = rzeka.DlugoscDoPunktu(punkt);
         var wycinek = rzeka.Odcinki.Skip(indeks).ToList();
         rzeka.Odcinki = rzeka.Odcinki.Take(indeks).ToList();
         return wycinek;
      }

      private void PogrubRzekeOdPunktu(IRzeka rzeka, IPunkt punkt, float aktualnaGrubosc)
      {
         int indeksPunktu = rzeka.DlugoscDoPunktu(punkt);
         foreach (var odcinekRzeki in rzeka.Odcinki.Skip(indeksPunktu))
         {
            odcinekRzeki.Grubosc += aktualnaGrubosc;
         }
      }

      private static bool KoncoweMiejsceJestNaBrzegu(IMapa mapa, IPunkt aktualnyPunkt)
      {
         return mapa.Rogi.Any(r => r.Punkt == aktualnyPunkt && r.Dane.Brzeznosc == BrzeznoscRogu.Brzeg);
      }
   }
}