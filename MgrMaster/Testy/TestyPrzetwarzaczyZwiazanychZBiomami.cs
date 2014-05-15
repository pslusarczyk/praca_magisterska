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
   public class TestyPrzetwarzaczyZwiazanychZBiomami
   {

      /*              Układ komórek i rogów używanych w testach:
       *          r1 r2/K3\r4
       *       K1 — K2     K5
       *             r3\K4/r5
       */

      #region Deklaracje

      private HashSet<IKomorka> _komorki;
      private HashSet<IRog> _rogi;
      private IMapa _mapa;

      #endregion

      #region Testy

      [SetUp]
      public void SetUp()
      {
         _komorki = MockKomorek();
         _mapa = MockKlasyMapa(_komorki, _rogi);
      }

      [Test]
      public void AktualizatorBiomówPoprawniePrzypisujeBiomy()
      {
         false.ShouldBeTrue(); // napisać test
         // źródła wody: morze w k1, rzeka w k2, jezioro w k5
         var aktualizator = new AktualizatorBiomow
         {
           
         };

         var k1 = _mapa.Komorki.ElementAt(0);
         var k2 = _mapa.Komorki.ElementAt(1);
         var k3 = _mapa.Komorki.ElementAt(2);
         var k4 = _mapa.Komorki.ElementAt(3);
         var k5 = _mapa.Komorki.ElementAt(4);

         aktualizator.Przetwarzaj(_mapa);

         k2.Dane.Biom.ShouldEqual(Biom.LasUmiarkowany);
         k3.Dane.Biom.ShouldEqual(Biom.LasUmiarkowany);
         k4.Dane.Biom.ShouldEqual(Biom.LasUmiarkowany);
      }




      #endregion

      #region Funkcje pomocnicze

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

      // todo Może utworzyć mechanizm/przetwarzacz łączący w pary komórki/rogi i, przy okazji, ich punkty?
      private static HashSet<IKomorka> MockKomorek() 
      {
         var punkty = MockPunktow();
         var k1 = new Komorka {Punkt = punkty.ElementAt(0)};
         var k2 = new Komorka {Punkt = punkty.ElementAt(1)};
         var k3 = new Komorka {Punkt = punkty.ElementAt(2)};
         var k4 = new Komorka {Punkt = punkty.ElementAt(3)};
         var k5 = new Komorka {Punkt = punkty.ElementAt(4)};
         var komorki = new HashSet<IKomorka>{k1, k2, k3, k4, k5};
         return komorki;
      }

      #endregion
   }
}