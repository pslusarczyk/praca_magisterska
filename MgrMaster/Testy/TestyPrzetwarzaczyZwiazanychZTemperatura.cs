using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieMapy;
using NUnit.Framework;
using Should;
using UnityEngine;

namespace Testy
{
   [TestFixture]
   public class TestyPrzetwarzaczyZwiazanychZTemperatura
   {

      /*              Układ komórek i rogów używanych w testach:
       *          r1 r2/K3\r4
       *       K1 — K2     K5
       *             r3\K4/r5
       */

      #region Deklaracje

      private HashSet<IKomorka> _komorki;
      private IMapa _mapa;

      #endregion

      #region Testy

      [SetUp]
      public void SetUp()
      {
         _komorki = MockKomorek();
         _mapa = MockKlasyMapa(_komorki);
         InicjujWysokosciPunktowNaPodstawiePozycji();
      }

      // todo testowanie zbyt prostej funkcjonalności (funkcja liniowa)?
      [Test]
      public void ModyfikatorTemperaturyNaPodstawieWysokosciPoprawniePrzypisujeTemperatury()
      {
         var modyfikator = new ModyfikatorTemperaturyNaPodstawieWysokosci
         {
            Konfiguracja = new KonfiguracjaModyfikatoraTemperaturyNaPodstawieWysokosci
            {
               Baza = 1f,
               ZmianaNaJednostke = 2f
            }
         };
         var poczatkoweTemperaturyKomorek = _mapa.Komorki.ToDictionary(k => k, k => k.Dane.Temperatura);
         var oczekiwaneTemperaturyKomorek = new Dictionary<IKomorka, float>
         {
            {_mapa.Komorki.ElementAt(0), 7f}, // na podstawie konfiguracji i wysokości punktów
            {_mapa.Komorki.ElementAt(1), 3f},
            {_mapa.Komorki.ElementAt(2), 1f},
            {_mapa.Komorki.ElementAt(3), 9f},
            {_mapa.Komorki.ElementAt(4), 5f},
         };

         modyfikator.Przetwarzaj(_mapa);

         foreach (IKomorka k in _mapa.Komorki)
         {
            float różnica = k.Dane.Temperatura - poczatkoweTemperaturyKomorek[k];
            float oczekiwanaRóżnica = oczekiwaneTemperaturyKomorek[k];
            różnica.ShouldEqual(oczekiwanaRóżnica);
         }

      }

      [Ignore]
      [Test]
      public void ModyfikatorTemperaturyGradientemPoprawniePrzypisujeTemperatury()
      {
         // todo gradienty odłożone na później
      }


      #endregion

      #region Funkcje pomocnicze

      private void InicjujWysokosciPunktowNaPodstawiePozycji()
      {
         foreach (var k in _mapa.Komorki)
         {
            k.Punkt.Wysokosc = k.Punkt.Pozycja.y;
         }
      }

      private static IMapa MockKlasyMapa(HashSet<IKomorka> komorki, HashSet<IRog> rogi = null)
      {
         IMapa mapa = new Mapa();
         mapa.Komorki = (HashSet<IKomorka>) komorki;
         mapa.Rogi = (HashSet<IRog>) rogi;
         return mapa;
      }

      private static IList<IPunkt> MockPunktow()
      {
         var punktK1 = new Punkt { Pozycja = new Vector3(10f, 3f, 2f) };
         var punktK2 = new Punkt { Pozycja = new Vector3(-4f, 1f, 3f) };
         var punktK3 = new Punkt { Pozycja = new Vector3(-3f, 0f, 5f) };
         var punktK4 = new Punkt { Pozycja = new Vector3(-1f, 4f, 1f) };
         var punktK5 = new Punkt { Pozycja = new Vector3(-2f, 2f, 2f) };    
         return new List<IPunkt> { punktK1, punktK2, punktK3, punktK4, punktK5 };
      }

      private static HashSet<IKomorka> MockKomorek() 
      {
         var punkty = MockPunktow();
         var k1 = new Komorka {Punkt = punkty.ElementAt(0)};
         var k2 = new Komorka {Punkt = punkty.ElementAt(1)};
         var k3 = new Komorka {Punkt = punkty.ElementAt(2)};
         var k4 = new Komorka {Punkt = punkty.ElementAt(3)};
         var k5 = new Komorka {Punkt = punkty.ElementAt(4)};
         k1.PrzylegleKomorki = new List<IKomorka>{k2};
         k2.PrzylegleKomorki = new List<IKomorka>{k1, k3, k4};
         k3.PrzylegleKomorki = new List<IKomorka>{k2, k5};
         k4.PrzylegleKomorki = new List<IKomorka>{k2, k5};
         k5.PrzylegleKomorki = new List<IKomorka>{k3, k4};
         var komorki = new HashSet<IKomorka>{k1, k2, k3, k4, k5};
         return komorki;
      }

      #endregion
   }
}