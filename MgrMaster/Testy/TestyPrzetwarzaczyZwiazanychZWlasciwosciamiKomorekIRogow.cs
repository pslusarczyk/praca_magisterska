using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzaczeMapy;
using LogikaGeneracji.PrzetwarzaczeMapy.Baza;
using LogikaGeneracji.PrzetwarzanieMapy;
using NUnit.Framework;
using Should;
using UnityEngine;

namespace Testy
{

   [TestFixture]
   public class TestyPrzetwarzaczyZwiazanychZWlasciwosciamiKomorekIRogow
   {
      private class ProstyRozdzielaczWodyILądu : IPrzetwarzaczMapy
      {
         public IPrzetwarzaczMapy Nastepnik { get; set; }
         public void Przetwarzaj(IMapa mapa)
         {
            foreach (IKomorka komorka in mapa.Komorki)
            {
               komorka.Dane.Podloze = Podloze.Woda;
               komorka.Dane.Typ = TypKomorki.Morze;
            }
         }
      }

      private ISet<IKomorka> _komorki;
      private ISet<IRog> _rogi;

      [Test]
      public void RozdzielaczWodyILąduPrzypisujeKomorkomTypy()
      {
         IPrzetwarzaczMapy rozdzielacz = new ProstyRozdzielaczWodyILądu();
         _komorki = MockKomorek();
         IMapa mapa = MockKlasyMapa(_komorki);
         mapa.ZastosujPrzetwarzanie(rozdzielacz);

         foreach (IKomorka komorka in mapa.Komorki)
         {
            komorka.Dane.Podloze.ShouldNotEqual(null);
            komorka.Dane.Typ.ShouldNotEqual(null);
         }
      }

      [TestCase(0, TypKomorki.Morze, TypKomorki.Morze, TypKomorki.Lad, TypKomorki.Lad, TypKomorki.Jezioro)]
      [TestCase(1, TypKomorki.Morze, TypKomorki.Morze, TypKomorki.Lad, TypKomorki.Lad, TypKomorki.Jezioro)]
      [TestCase(4, TypKomorki.Jezioro, TypKomorki.Jezioro, TypKomorki.Lad, TypKomorki.Lad, TypKomorki.Morze)]
      public void RozdzielaczMorzIJeziorOdpowiednioPrzypisujeTypy(int indeksInicjatora,
         TypKomorki spodziewanyTypK1, TypKomorki spodziewanyTypK2,
         TypKomorki spodziewanyTypK3, TypKomorki spodziewanyTypK4, TypKomorki spodziewanyTypK5)
      {
         _komorki = MockKomorek();
         IMapa mapa = MockKlasyMapa(_komorki);
         IKomorka k1 = _komorki.ElementAt(0);
         IKomorka k2 = _komorki.ElementAt(1);
         IKomorka k3 = _komorki.ElementAt(2);
         IKomorka k4 = _komorki.ElementAt(3);
         IKomorka k5 = _komorki.ElementAt(4);
         IKomorka inicjator = _komorki.ElementAt(indeksInicjatora);
         IPrzetwarzaczMapy rozdzielacz = new RozdzielaczMorzIJezior(inicjator);

         mapa.ZastosujPrzetwarzanie(rozdzielacz);

         k1.Dane.Typ.ShouldEqual(spodziewanyTypK1);
         k2.Dane.Typ.ShouldEqual(spodziewanyTypK2);
         k3.Dane.Typ.ShouldEqual(spodziewanyTypK3);
         k4.Dane.Typ.ShouldEqual(spodziewanyTypK4);
         k5.Dane.Typ.ShouldEqual(spodziewanyTypK5);
         
      }

      [TestCase(0, BrzeznoscKomorki.OtwarteMorze, BrzeznoscKomorki.MorzePrzybrzezne, BrzeznoscKomorki.BrzegMorza,
         BrzeznoscKomorki.BrzegMorza, BrzeznoscKomorki.OtwartyLad)]
      [TestCase(1, BrzeznoscKomorki.OtwarteMorze, BrzeznoscKomorki.MorzePrzybrzezne, BrzeznoscKomorki.BrzegMorza,
         BrzeznoscKomorki.BrzegMorza, BrzeznoscKomorki.OtwartyLad)]
      [TestCase(4, BrzeznoscKomorki.OtwartyLad, BrzeznoscKomorki.OtwartyLad, BrzeznoscKomorki.BrzegMorza,
         BrzeznoscKomorki.BrzegMorza, BrzeznoscKomorki.MorzePrzybrzezne)]
      public void AktualizatorBrzeżnościKomórekOdpowiednioPrzypisujeBrzeżnościKomórek(int indeksInicjatora,
      BrzeznoscKomorki spodziewanaBrzeznoscK1, BrzeznoscKomorki spodziewanaBrzeznoscK2,
      BrzeznoscKomorki spodziewanaBrzeznoscK3, BrzeznoscKomorki spodziewanaBrzeznoscK4, BrzeznoscKomorki spodziewanaBrzeznoscK5)
         {
            _komorki = MockKomorek();
            IMapa mapa = MockKlasyMapa(_komorki);
            IKomorka k1 = _komorki.ElementAt(0);
            IKomorka k2 = _komorki.ElementAt(1);
            IKomorka k3 = _komorki.ElementAt(2);
            IKomorka k4 = _komorki.ElementAt(3);
            IKomorka k5 = _komorki.ElementAt(4);
            IKomorka inicjator = _komorki.ElementAt(indeksInicjatora);
            var rozdzielacz = new RozdzielaczMorzIJezior(inicjator)
            {
               Nastepnik = new AktualizatorBrzeznosciKomorek()
            };

            mapa.ZastosujPrzetwarzanie(rozdzielacz);

            k1.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK1);
            k2.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK2);
            k3.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK3);
            k4.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK4);
            k5.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK5);

         }

      [TestCase(0, BrzeznoscRogu.OtwarteMorze, BrzeznoscRogu.Brzeg, BrzeznoscRogu.Brzeg,
   BrzeznoscRogu.OtwartyLad, BrzeznoscRogu.OtwartyLad)]
      [TestCase(1, BrzeznoscRogu.OtwarteMorze, BrzeznoscRogu.Brzeg, BrzeznoscRogu.Brzeg,
   BrzeznoscRogu.OtwartyLad, BrzeznoscRogu.OtwartyLad)]
      [TestCase(4, BrzeznoscRogu.OtwartyLad, BrzeznoscRogu.OtwartyLad, BrzeznoscRogu.OtwartyLad,
   BrzeznoscRogu.Brzeg, BrzeznoscRogu.Brzeg)]
      public void AktualizatorBrzeżnościRogówOdpowiednioJePrzypisuje(int indeksInicjatora,
      BrzeznoscRogu spodziewanaBrzeznoscR1, BrzeznoscRogu spodziewanaBrzeznoscR2,
      BrzeznoscRogu spodziewanaBrzeznoscR3, BrzeznoscRogu spodziewanaBrzeznoscR4, BrzeznoscRogu spodziewanaBrzeznoscR5)
      {
         _komorki = MockKomorek();
         _rogi = MockRogow(_komorki);
         IMapa mapa = MockKlasyMapa(_komorki, _rogi);
         IKomorka inicjator = _komorki.ElementAt(indeksInicjatora);
         IRog r1 = _rogi.ElementAt(0);
         IRog r2 = _rogi.ElementAt(1);
         IRog r3 = _rogi.ElementAt(2);
         IRog r4 = _rogi.ElementAt(3);
         IRog r5 = _rogi.ElementAt(4);
         var rozdzielacz = new RozdzielaczMorzIJezior(inicjator)
         {
            Nastepnik = new AktualizatorBrzeznosciRogow()
         };

         mapa.ZastosujPrzetwarzanie(rozdzielacz);

         r1.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR1);
         r2.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR2);
         r3.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR3);
         r4.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR4);
         r5.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR5);
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
         /*              Układ:
          *          r1 r2/K3\r4
          *       K1 — K2     K5
          *             r3\K4/r5
          */

         var punkt1 = new Punkt { Pozycja = new Vector3(10f, 3f, 2f) };
         var punkt2 = new Punkt { Pozycja = new Vector3(-4f, 1f, 3f) };
         var punkt3 = new Punkt { Pozycja = new Vector3(-3f, 0f, 5f) };
         var punkt4 = new Punkt { Pozycja = new Vector3(-1f, 4f, 1f) };
         var punkt5 = new Punkt { Pozycja = new Vector3(-2f, 2f, 2f) };
         punkt1.Sasiedzi = new List<IPunkt> { punkt2 };
         punkt2.Sasiedzi = new List<IPunkt> { punkt1, punkt3, punkt4 };
         punkt3.Sasiedzi = new List<IPunkt> { punkt2, punkt5 };
         punkt4.Sasiedzi = new List<IPunkt> { punkt2, punkt5 };
         punkt5.Sasiedzi = new List<IPunkt> { punkt3, punkt4 };
         return new List<IPunkt> { punkt1, punkt2, punkt3, punkt4, punkt5 };
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
         var r1 = new Rog();
         var r2 = new Rog();
         var r3 = new Rog();
         var r4 = new Rog();
         var r5 = new Rog();
         r1.Komorki = new List<IKomorka> {komorki.ElementAt(0), komorki.ElementAt(1)};
         r2.Komorki = new List<IKomorka> {komorki.ElementAt(1), komorki.ElementAt(2)};
         r3.Komorki = new List<IKomorka> {komorki.ElementAt(1), komorki.ElementAt(3)};
         r4.Komorki = new List<IKomorka> {komorki.ElementAt(2), komorki.ElementAt(4)};
         r5.Komorki = new List<IKomorka> {komorki.ElementAt(3), komorki.ElementAt(4)};
         return new HashSet<IRog>{r1, r2, r3, r4, r5};
      }
   }
}


