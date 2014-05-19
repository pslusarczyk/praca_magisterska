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

   /*              Układ komórek i rogów używanych w testach:
    *          r1 r2/K3\r4
    *       K1 — K2     K5
    *             r3\K4/r5
    */

   [TestFixture]
   public class TestyPrzetwarzaczyZwiazanychZeZbiornikamiWodnymi
   {

      #region Deklaracje

      private class ProstyRozdzielaczWodyILądu : BazaPrzetwarzacza
      {
         public override void Przetwarzaj(IMapa mapa)
         {
            foreach (IKomorka komorka in mapa.Komorki)
            {
               komorka.Dane.Podloze = Podloze.Woda;
               komorka.Dane.Typ = TypKomorki.Morze;
            }
         }
      }

      private HashSet<IKomorka> _komorki;
      private HashSet<IRog> _rogi;

      #endregion

      #region Testy

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
         IKomorka inicjator = _komorki.ElementAt(indeksInicjatora);
         IPrzetwarzaczMapy rozdzielacz = new RozdzielaczMorzIJezior(inicjator);

         mapa.ZastosujPrzetwarzanie(rozdzielacz);

         _komorki.ElementAt(0).Dane.Typ.ShouldEqual(spodziewanyTypK1);
         _komorki.ElementAt(1).Dane.Typ.ShouldEqual(spodziewanyTypK2);
         _komorki.ElementAt(2).Dane.Typ.ShouldEqual(spodziewanyTypK3);
         _komorki.ElementAt(3).Dane.Typ.ShouldEqual(spodziewanyTypK4);
         _komorki.ElementAt(4).Dane.Typ.ShouldEqual(spodziewanyTypK5);
         
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
            IKomorka inicjator = _komorki.ElementAt(indeksInicjatora);
            var rozdzielacz = new RozdzielaczMorzIJezior(inicjator)
            {
               Nastepnik = new AktualizatorBrzeznosciKomorek()
            };

            mapa.ZastosujPrzetwarzanie(rozdzielacz);

            _komorki.ElementAt(0).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK1);
            _komorki.ElementAt(1).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK2);
            _komorki.ElementAt(2).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK3);
            _komorki.ElementAt(3).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK4);
            _komorki.ElementAt(4).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK5);

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
         var rozdzielacz = new RozdzielaczMorzIJezior(inicjator)
         {
            Nastepnik = new AktualizatorBrzeznosciRogow()
         };

         mapa.ZastosujPrzetwarzanie(rozdzielacz);

         _rogi.ElementAt(0).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR1);
         _rogi.ElementAt(1).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR2);
         _rogi.ElementAt(2).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR3);
         _rogi.ElementAt(3).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR4);
         _rogi.ElementAt(4).Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscR5);
      }

      [TestCase("1", 1)]
      [TestCase("2", 1)]
      [TestCase("3", 2)]
      [TestCase("4", 3)]
      [TestCase("5", 4)]
      [TestCase("1;2", 1)]
      [TestCase("2;3", 1)]
      [TestCase("4;5", 3)]
      [TestCase("1;2;3;4;5", 1)]
      // todo test sprawdzający dwa oddzielone jeziora?
      // todo test sprawdzający czy niejeziorne komórki się nie zmieniły? Ten i powyższy wykonane ręcznie 4 IV.
      public void WyrównywaczTerenuJezioraOdpowiednioModyfikujeWysokosc(string jeziora, float minWys)
      {
         _komorki = MockKomorek();
         _rogi = MockRogow(_komorki);
         IMapa mapa = MockKlasyMapa(_komorki, _rogi);
         _komorki.ToList().ForEach(k => k.Dane.Podloze = Podloze.Ziemia);
         _komorki.ToList().ForEach(k => k.Dane.Typ = TypKomorki.Lad);
         for (int i = 0; i < 5; ++i) // wysokość punktu = jego numer
         {
            _komorki.ElementAt(i).Punkt.Wysokosc = i+1;
            _rogi.ElementAt(i).Punkt.Wysokosc = i+1;
         }
         IEnumerable<int> indeksyJezior = jeziora.Split(';').Select(n => int.Parse(n) - 1);
         foreach (int indeks in indeksyJezior)
            _komorki.ElementAt(indeks).Dane.Typ = TypKomorki.Jezioro;
         
         mapa.ZastosujPrzetwarzanie(new WyrownywaczTerenuJeziora());

         _komorki.ElementAt(indeksyJezior.First()).Punkt.Wysokosc.ShouldEqual(minWys);
         _komorki.ElementAt(indeksyJezior.First()).Rogi.ToList().ForEach(r => r.Punkt.Wysokosc.ShouldEqual(minWys));
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
         k1.PrzylegleKomorki = new List<IKomorka> { k2 };
         k2.PrzylegleKomorki = new List<IKomorka> { k1, k3, k4 };
         k3.PrzylegleKomorki = new List<IKomorka> { k2, k5 };
         k4.PrzylegleKomorki = new List<IKomorka> { k2, k5 };
         k5.PrzylegleKomorki = new List<IKomorka> { k3, k4 };
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
         r1.BliskieRogi = new List<IRog> { r2, r3 };
         r2.BliskieRogi = new List<IRog> { r1, r3, r4 };
         r3.BliskieRogi = new List<IRog> { r1, r2, r5 };
         r4.BliskieRogi = new List<IRog> { r2, r5 };
         r5.BliskieRogi = new List<IRog> { r3, r4 };
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
         r1.Punkt.Sasiedzi = r1.BliskieRogi.Select(b => b.Punkt).Union(r1.Komorki.Select(k => k.Punkt)).ToList();
         r2.Punkt.Sasiedzi = r2.BliskieRogi.Select(b => b.Punkt).Union(r1.Komorki.Select(k => k.Punkt)).ToList();
         r3.Punkt.Sasiedzi = r3.BliskieRogi.Select(b => b.Punkt).Union(r1.Komorki.Select(k => k.Punkt)).ToList();
         r4.Punkt.Sasiedzi = r4.BliskieRogi.Select(b => b.Punkt).Union(r1.Komorki.Select(k => k.Punkt)).ToList();
         r5.Punkt.Sasiedzi = r5.BliskieRogi.Select(b => b.Punkt).Union(r1.Komorki.Select(k => k.Punkt)).ToList();
         return new HashSet<IRog>{r1, r2, r3, r4, r5};
      }

      #endregion
   }
}


