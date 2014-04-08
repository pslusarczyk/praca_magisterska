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
   public class TestyPrzetwarzaczyZwiazanychZRzekami
   {

      /*              Układ komórek i rogów używanych w testach:
       *          r1 r2/K3\r4
       *       K1 — K2     K5
       *             r3\K4/r5
       */

      #region Deklaracje

      private ISet<IKomorka> _komorki;
      private ISet<IRog> _rogi;
      private IMapa _mapa;

      #endregion

      #region Testy

      [SetUp]
      public void SetUp()
      {
         _komorki = MockKomorek();
         _rogi = MockRogow(_komorki);
         _mapa = MockKlasyMapa(_komorki, _rogi);
         InicjujWysokosciPunktowIndeksamiIchKomorekIRogow();
      }

      [Test]
      public void SpływSięNieUdajeGdyJestWNiecce() // todo to właściwie zostało przetestowane w aktualizatorze – usunąć?
      {
         var aktualizator = new AktualizatorNastepstwaMapyWysokosci();
         IPunkt punktPoczatkowy = _mapa.Komorki.ElementAt(2).Punkt;
         punktPoczatkowy.Wysokosc = 0; // niecka
         _mapa.ZastosujPrzetwarzanie(aktualizator);
         IGeneratorRzeki generatorRzeki = new GeneratorRzeki(punktPoczatkowy);

         generatorRzeki.PunktPoczatkowy.Nastepnik.ShouldBeNull();
      }

      [Test]
      public void SpływSięUdajeGdyNieJestWNiecce() // todo to właściwie zostało przetestowane w aktualizatorze – usunąć?
      {
         var aktualizator = new AktualizatorNastepstwaMapyWysokosci();
         IPunkt punktPoczatkowy = _mapa.Komorki.ElementAt(2).Punkt;
         punktPoczatkowy.Wysokosc = 100; // wysoko
         _mapa.ZastosujPrzetwarzanie(aktualizator);
         IGeneratorRzeki generatorRzeki = new GeneratorRzeki(punktPoczatkowy);

         generatorRzeki.PunktPoczatkowy.Nastepnik.ShouldNotBeNull();
      }


      [Test]
      public void RzekaSpływającaDoNieckiNieTworzySię()
      {
         _mapa.Komorki.ElementAt(1).Punkt.Wysokosc = 0; // niecka
         var aktualizator = new AktualizatorNastepstwaMapyWysokosci();
         IPunkt punktPoczatkowy = _mapa.Komorki.ElementAt(2).Punkt;
         IGeneratorRzeki generatorRzeki = new GeneratorRzeki(punktPoczatkowy);
         _mapa.ZastosujPrzetwarzanie(aktualizator);

         _mapa.ZastosujPrzetwarzanie(generatorRzeki);

         generatorRzeki.UdaloSieUtworzyc.Value.ShouldBeFalse();
         _mapa.Rzeki.Count().ShouldEqual(0);
      }


      [Test]
      public void RzekaSpływającaDoMorzaTworzySięIKończyNaBrzegu()
      {
         
      }

      [Test]
      public void RzekaPrzepływającaPrzezJezioroIWpadającaDoMorzaTworzySięIKończyNaBrzegu()
      {
         
      }

      [Test]
      public void GdyDwieRzekiSięŁącząWspólnyOdcinekNależyDoTejDłuższej()
      {
         
      }

      #endregion

      #region Funkcje pomocnicze

      private void InicjujWysokosciPunktowIndeksamiIchKomorekIRogow()
      {
         for (int i = 0; i < 5; ++i)
         {
            _mapa.Komorki.ElementAt(i).Punkt.Wysokosc = i + 1;
            _mapa.Rogi.ElementAt(i).Punkt.Wysokosc = i + 1;
         }
      }

      private static IMapa MockKlasyMapa(ISet<IKomorka> komorki, ISet<IRog> rogi = null)
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
         punktK1.Sasiedzi = new List<IPunkt> { punktK2 };
         punktK2.Sasiedzi = new List<IPunkt> { punktK1, punktK3, punktK4 };
         punktK3.Sasiedzi = new List<IPunkt> { punktK2, punktK5 };
         punktK4.Sasiedzi = new List<IPunkt> { punktK2, punktK5 };
         punktK5.Sasiedzi = new List<IPunkt> { punktK3, punktK4 };
         return new List<IPunkt> { punktK1, punktK2, punktK3, punktK4, punktK5 };
      }

      private static ISet<IKomorka> MockKomorek() 
      {
         var punkty = MockPunktow();
         var k1 = new Komorka {Punkt = punkty.ElementAt(0)};
         var k2 = new Komorka {Punkt = punkty.ElementAt(1)};
         var k3 = new Komorka {Punkt = punkty.ElementAt(2)};
         var k4 = new Komorka {Punkt = punkty.ElementAt(3)};
         var k5 = new Komorka {Punkt = punkty.ElementAt(4)};
         k1.Dane.Podloze = Podloze.Woda;
         k2.Dane.Podloze = Podloze.Woda;
         k3.Dane.Podloze = Podloze.Ziemia;
         k4.Dane.Podloze = Podloze.Ziemia;
         k5.Dane.Podloze = Podloze.Woda;
         var komorki = new HashSet<IKomorka>{k1, k2, k3, k4, k5};
         foreach (IKomorka komorka in komorki)
         {
            komorka.PrzylegleKomorki = komorki.Where(
               potencjalnySasiad => komorka.Punkt.Sasiedzi.Contains(potencjalnySasiad.Punkt)).ToList();
         }
         return komorki;
      }

      private ISet<IRog> MockRogow(ISet<IKomorka> komorki)
      {
         var r1 = new Rog{Punkt = new Punkt()};
         var r2 = new Rog{ Punkt = new Punkt()};
         var r3 = new Rog{ Punkt = new Punkt()};
         var r4 = new Rog{ Punkt = new Punkt()};
         var r5 = new Rog{ Punkt = new Punkt()};
         r1.Komorki = new List<IKomorka> {komorki.ElementAt(0), komorki.ElementAt(1)};
         r2.Komorki = new List<IKomorka> {komorki.ElementAt(1), komorki.ElementAt(2)};
         r3.Komorki = new List<IKomorka> {komorki.ElementAt(1), komorki.ElementAt(3)};
         r4.Komorki = new List<IKomorka> {komorki.ElementAt(2), komorki.ElementAt(4)};
         r5.Komorki = new List<IKomorka> {komorki.ElementAt(3), komorki.ElementAt(4)};
         komorki.ElementAt(0).Rogi = new List<IRog> {r1};
         komorki.ElementAt(1).Rogi = new List<IRog> {r1, r2, r3};
         komorki.ElementAt(2).Rogi = new List<IRog> {r2, r4};
         komorki.ElementAt(3).Rogi = new List<IRog> {r3, r5};
         komorki.ElementAt(4).Rogi = new List<IRog> {r4, r5};
         return new HashSet<IRog>{r1, r2, r3, r4, r5};
      }

      #endregion
   }
}