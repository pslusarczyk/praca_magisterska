using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzaczeMapy;
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
      public void RozdzielaczMorzIJeziorOdpowiednioPrzypisujeBrzeżności(int indeksInicjatora,
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
            IPrzetwarzaczMapy rozdzielacz = new RozdzielaczMorzIJezior(inicjator);

            mapa.ZastosujPrzetwarzanie(rozdzielacz);

            k1.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK1);
            k2.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK2);
            k3.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK3);
            k4.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK4);
            k5.Dane.Brzeznosc.ShouldEqual(spodziewanaBrzeznoscK5);

         }


      private static IMapa MockKlasyMapa(ISet<IKomorka> komorki)
      {
         IMapa mapa = new Mapa();
         mapa.Komorki = (HashSet<IKomorka>) komorki;
         return mapa;
      }

      private static IEnumerable<IPunkt> MockPunktow()
      {
         /*              Układ:
          *               /k3\
          *       k1 — k2     k5
          *               \k4/
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
   }
}


