using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieMapy;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
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

      private HashSet<IKomorka> _komorki;
      private HashSet<IRog> _rogi;
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
      public void PunktBędącyNieckąNieMaNastępnika() // todo to właściwie zostało przetestowane w aktualizatorze – usunąć?
      {
         var aktualizator = new AktualizatorNastepstwaMapyWysokosci();
         IPunkt punktPoczatkowy = _mapa.Komorki.ElementAt(2).Punkt;
         punktPoczatkowy.Wysokosc = 0; // niecka
         _mapa.ZastosujPrzetwarzanie(aktualizator);
         IGeneratorRzeki generatorRzeki = new GeneratorRzeki(punktPoczatkowy);

         generatorRzeki.PunktPoczatkowy.Nastepnik.ShouldBeNull();
      }

      [Test]
      public void PunktNiebędącyNieckąMaNastępnik() // todo to właściwie zostało przetestowane w aktualizatorze – usunąć?
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

         _mapa.ZastosujPrzetwarzanie(generatorRzeki as IPrzetwarzaczMapy);

         generatorRzeki.UdaloSieUtworzyc.Value.ShouldBeFalse();
         _mapa.Rzeki.Count().ShouldEqual(0);
      }

      [TestCase(null,null)]
      [TestCase(Podloze.Ziemia,TypKomorki.Jezioro)]
      public void RzekaSpływającaDoMorzaPrzezKomorkeTworzySięPoprawnie(Podloze podlozeK2, TypKomorki typK2)
      {
         var aktualizator = new AktualizatorNastepstwaMapyWysokosci();
         IPunkt punktPoczatkowy = _mapa.Komorki.ElementAt(2).Punkt;
         _mapa.Komorki.ElementAt(1).Dane.Podloze = podlozeK2;
         _mapa.Komorki.ElementAt(1).Dane.Typ = typK2;
         IRog brzeg = _mapa.Komorki.ElementAt(0).Rogi.First();
         brzeg.Dane.Brzeznosc = BrzeznoscRogu.Brzeg;
         _mapa.ZastosujPrzetwarzanie(aktualizator);

         IGeneratorRzeki generatorRzeki = new GeneratorRzeki(punktPoczatkowy);
         _mapa.ZastosujPrzetwarzanie(generatorRzeki as IPrzetwarzaczMapy);

         generatorRzeki.UdaloSieUtworzyc.Value.ShouldBeTrue();
         _mapa.Rzeki.Count().ShouldEqual(1);
         _mapa.Rzeki.First().Odcinki.Last().PunktB.ShouldEqual(brzeg.Punkt);
         _mapa.Rzeki.First().Odcinki.Count.ShouldEqual(2); // k3-r2-r1
         _mapa.Komorki.ElementAt(2).Punkt.ZawieraRzeke.ShouldBeTrue();
         _mapa.Rogi.ElementAt(1).Punkt.ZawieraRzeke.ShouldBeTrue();
         _mapa.Rogi.ElementAt(0).Punkt.ZawieraRzeke.ShouldBeTrue();
      }

      [TestCase(true)]
      [TestCase(false)]
      public void GdyDwieRzekiSięŁącząTaKrótszaSięKończyADłuższaPłynieDalejGrubsza(bool najpierwKrotsza)
      {
         var aktualizator = new AktualizatorNastepstwaMapyWysokosci();
         IPunkt punktPoczatkowyKrotszej = _mapa.Komorki.ElementAt(3).Punkt;
         IPunkt punktPoczatkowyDluzszej = _mapa.Komorki.ElementAt(4).Punkt;
         _mapa.Rogi.ElementAt(1).Punkt.Sasiedzi.Remove(_mapa.Rogi.ElementAt(0).Punkt);
         _mapa.Rogi.ElementAt(2).Punkt.Sasiedzi.Remove(_mapa.Rogi.ElementAt(0).Punkt); // usuwamy te sąsiedztwa, żeby rzeka płynęła przez k2
         IPunkt ujscie = _mapa.Komorki.ElementAt(1).Punkt;
         ujscie.Wysokosc = 1.5f; // żeby mieć pewność, że z r3 spływa do k2, a nie do r2
         IRog brzeg = _mapa.Komorki.ElementAt(0).Rogi.First();
         brzeg.Dane.Brzeznosc = BrzeznoscRogu.Brzeg;
         _mapa.ZastosujPrzetwarzanie(aktualizator);

         IGeneratorRzeki generatorKrotszejRzeki = new GeneratorRzeki(punktPoczatkowyKrotszej);
         IGeneratorRzeki generatorDluzszejRzeki = new GeneratorRzeki(punktPoczatkowyDluzszej);
         IRzeka krotsza; 
         IRzeka dluzsza; 
         if (najpierwKrotsza)
         {
            _mapa.ZastosujPrzetwarzanie(generatorKrotszejRzeki as IPrzetwarzaczMapy);

            krotsza = _mapa.Rzeki.ElementAt(0);
            generatorKrotszejRzeki.UdaloSieUtworzyc.Value.ShouldBeTrue();
            krotsza.Odcinki.Last().PunktB.ShouldEqual(brzeg.Punkt);

            _mapa.ZastosujPrzetwarzanie(generatorDluzszejRzeki as IPrzetwarzaczMapy);
            dluzsza =  _mapa.Rzeki.ElementAt(1);
         }
         else
         {
            _mapa.ZastosujPrzetwarzanie(generatorDluzszejRzeki as IPrzetwarzaczMapy);

            dluzsza = _mapa.Rzeki.ElementAt(0);
            generatorDluzszejRzeki.UdaloSieUtworzyc.Value.ShouldBeTrue();
            dluzsza.Odcinki.Last().PunktB.ShouldEqual(brzeg.Punkt);

            _mapa.ZastosujPrzetwarzanie(generatorKrotszejRzeki as IPrzetwarzaczMapy);
            krotsza = _mapa.Rzeki.ElementAt(1);
         }

         generatorDluzszejRzeki.UdaloSieUtworzyc.Value.ShouldBeTrue();
         _mapa.Rzeki.Count().ShouldEqual(2);
         krotsza.Odcinki.Last().PunktB.ShouldEqual(ujscie);
         dluzsza.Odcinki.Last().PunktB.ShouldEqual(brzeg.Punkt);
         krotsza.Odcinki.Count.ShouldEqual(2); // k4-r3-k2
         dluzsza.Odcinki.Count.ShouldEqual(4); // k5-r4-r2-k2-r1
         krotsza.Odcinki.ToList().ForEach(o => o.Grubosc.ShouldEqual(GeneratorRzeki.GruboscJednostkowa));
         dluzsza.Odcinki.Take(3).ToList().ForEach(o => o.Grubosc.ShouldEqual(GeneratorRzeki.GruboscJednostkowa));
         krotsza.Odcinki.Skip(3).ToList().ForEach(o => o.Grubosc.ShouldEqual(GeneratorRzeki.GruboscJednostkowa*2));
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

      private static IMapa MockKlasyMapa(HashSet<IKomorka> komorki, HashSet<IRog> rogi = null)
      {
         IMapa mapa = new Mapa();
         mapa.Komorki = (HashSet<IKomorka>) komorki;
         mapa.Rogi = (HashSet<IRog>) rogi;
         return mapa;
      }

      private static IList<IPunkt> MockPunktow()
      {
         var punktK1 = new Punkt { Pozycja = new MojVector3(10f, 3f, 2f) };
         var punktK2 = new Punkt { Pozycja = new MojVector3(-4f, 1f, 3f) };
         var punktK3 = new Punkt { Pozycja = new MojVector3(-3f, 0f, 5f) };
         var punktK4 = new Punkt { Pozycja = new MojVector3(-1f, 4f, 1f) };
         var punktK5 = new Punkt { Pozycja = new MojVector3(-2f, 2f, 2f) };    
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
         k1.Dane.Podloze = Podloze.Woda;
         k2.Dane.Podloze = Podloze.Woda;
         k3.Dane.Podloze = Podloze.Ziemia;
         k4.Dane.Podloze = Podloze.Ziemia;
         k5.Dane.Podloze = Podloze.Woda;
         k1.PrzylegleKomorki = new List<IKomorka>{k2};
         k2.PrzylegleKomorki = new List<IKomorka>{k1, k3, k4};
         k3.PrzylegleKomorki = new List<IKomorka>{k2, k5};
         k4.PrzylegleKomorki = new List<IKomorka>{k2, k5};
         k5.PrzylegleKomorki = new List<IKomorka>{k3, k4};
         var komorki = new HashSet<IKomorka>{k1, k2, k3, k4, k5};
         return komorki;
      }

      private HashSet<IRog> MockRogow(HashSet<IKomorka> komorki)
      {
         var r1 = new Rog{ Punkt = new Punkt()};
         var r2 = new Rog{ Punkt = new Punkt()};
         var r3 = new Rog{ Punkt = new Punkt()};
         var r4 = new Rog{ Punkt = new Punkt()};
         var r5 = new Rog{ Punkt = new Punkt()};
         r1.BliskieRogi = new List<IRog>{r2, r3};
         r2.BliskieRogi = new List<IRog>{r1, r3, r4};
         r3.BliskieRogi = new List<IRog>{r1, r2, r5};
         r4.BliskieRogi = new List<IRog>{r2, r5};
         r5.BliskieRogi = new List<IRog>{r3, r4};
         r1.Komorki = new List<IKomorka> {komorki.ElementAt(0), komorki.ElementAt(1)};
         r2.Komorki = new List<IKomorka> {komorki.ElementAt(1), komorki.ElementAt(2)};
         r3.Komorki = new List<IKomorka> {komorki.ElementAt(1), komorki.ElementAt(3)};
         r4.Komorki = new List<IKomorka> {komorki.ElementAt(2), komorki.ElementAt(4)};
         r5.Komorki = new List<IKomorka> {komorki.ElementAt(3), komorki.ElementAt(4)};
         r1.Punkt.Sasiedzi = r1.BliskieRogi.Select(b => b.Punkt).Union(r1.Komorki.Select(k => k.Punkt)).ToList();
         r2.Punkt.Sasiedzi = r2.BliskieRogi.Select(b => b.Punkt).Union(r2.Komorki.Select(k => k.Punkt)).ToList();
         r3.Punkt.Sasiedzi = r3.BliskieRogi.Select(b => b.Punkt).Union(r3.Komorki.Select(k => k.Punkt)).ToList();
         r4.Punkt.Sasiedzi = r4.BliskieRogi.Select(b => b.Punkt).Union(r4.Komorki.Select(k => k.Punkt)).ToList();
         r5.Punkt.Sasiedzi = r5.BliskieRogi.Select(b => b.Punkt).Union(r5.Komorki.Select(k => k.Punkt)).ToList();
         var k1 = komorki.ElementAt(0);
         var k2 = komorki.ElementAt(1);
         var k3 = komorki.ElementAt(2);
         var k4 = komorki.ElementAt(3);
         var k5 = komorki.ElementAt(4);
         k1.Rogi = new List<IRog> {r1};
         k2.Rogi = new List<IRog> {r1, r2, r3};
         k3.Rogi = new List<IRog> {r2, r4};
         k4.Rogi = new List<IRog> {r3, r5};
         k5.Rogi = new List<IRog> {r4, r5};
         k1.Punkt.Sasiedzi = k1.Rogi.Select(r => r.Punkt).ToList();
         k2.Punkt.Sasiedzi = k2.Rogi.Select(r => r.Punkt).ToList();
         k3.Punkt.Sasiedzi = k3.Rogi.Select(r => r.Punkt).ToList();
         k4.Punkt.Sasiedzi = k4.Rogi.Select(r => r.Punkt).ToList();
         k5.Punkt.Sasiedzi = k5.Rogi.Select(r => r.Punkt).ToList();
         return new HashSet<IRog>{r1, r2, r3, r4, r5};
      }

      #endregion
   }
}