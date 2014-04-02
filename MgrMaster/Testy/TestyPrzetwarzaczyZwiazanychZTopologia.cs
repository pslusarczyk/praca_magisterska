using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzaczeMapy;
using Moq;
using NUnit.Framework;
using Should;
using UnityEngine;

namespace Testy
{
   [TestFixture]
   public class TestyPrzetwarzaczyZwiazanychZTopologia
   {
      private class ProstyModyfikatorWysokosci : IPrzetwarzaczMapy
      {
         public const float WysokoscPierwszego = 3f;
         public const float WysokoscDrugiego = 2f;
         public const float WysokoscTrzeciego = 1f;

         public virtual void Przetwarzaj(IMapa mapa)
         {
            mapa.Punkty.ElementAt(0).Wysokosc = WysokoscPierwszego;
            mapa.Punkty.ElementAt(1).Wysokosc = WysokoscDrugiego;
            mapa.Punkty.ElementAt(2).Wysokosc = WysokoscTrzeciego;
         }
      }

      private class ProstyRozdzielacz : IRozdzielaczWodyILądu
      {
         public void PrzetworzMape(IMapa mapa)
         {
            throw new System.NotImplementedException();
         }
      }

      private IMapa _mapa;

      [SetUp]
      public void SetUp()
      {
         _mapa = MockPrzykladowegoZbioruPunktowZSasiedztwem();
      }

      [Test]
      public void NowaMapaGeograficznaPosiadaPunktyZWysokościąZero()
      {
         foreach (IPunkt pg in _mapa.Punkty)
         {
            pg.Wysokosc.ShouldEqual(0);
         }
      }

      [Test]
      public void SąsiedztwoPunktówNowejMapyJestWzajemne()
      {
         foreach (var punkt in _mapa.Punkty)
         {
            foreach (IPunkt sasiad in punkt.Sasiedzi)
            {
               Assert.That(sasiad.Sasiedzi.Contains(punkt));
            }
         }
      }

      [Test]
      public void PoNalozeniuWysokosciPunktyGeograficzneMajaOdpowiednieWysokosci()
      {
         var modyfikator = new ProstyModyfikatorWysokosci();
         modyfikator.Przetwarzaj(_mapa);
         _mapa.Punkty.ElementAt(0).Wysokosc
            .ShouldEqual(ProstyModyfikatorWysokosci.WysokoscPierwszego);
         _mapa.Punkty.ElementAt(1).Wysokosc
            .ShouldEqual(ProstyModyfikatorWysokosci.WysokoscDrugiego);
         _mapa.Punkty.ElementAt(2).Wysokosc
            .ShouldEqual(ProstyModyfikatorWysokosci.WysokoscTrzeciego);
      }

      [Test]
      public void PunktyGeograficznePrzetworzonejMapyMająOdpowiednioPoustawianychNastepnikow()
      {
         var modyfikator = new ProstyModyfikatorWysokosci();
         modyfikator.Przetwarzaj(_mapa);
         new AktualizatorNastepstwaMapyWysokosci().Przetwarzaj(_mapa);
         var punkt1 = _mapa.Punkty.ElementAt(0);
         var punkt2 = _mapa.Punkty.ElementAt(1);
         var punkt3 = _mapa.Punkty.ElementAt(2);
         punkt1.Nastepnik.ShouldEqual(punkt2);
         punkt2.Nastepnik.ShouldEqual(punkt3);
         punkt3.Nastepnik.ShouldBeNull();
      }

      [Test]
      public void NastepnicyPunktówGeograficznychSąIchSąsiadami()
      {
         var modyfikator = new ProstyModyfikatorWysokosci();
         modyfikator.Przetwarzaj(_mapa);
         new AktualizatorNastepstwaMapyWysokosci().Przetwarzaj(_mapa);
         var punkt1 = _mapa.Punkty.ElementAt(0);
         var punkt2 = _mapa.Punkty.ElementAt(1);
         punkt1.Sasiedzi.ShouldContain(punkt1.Nastepnik);
         punkt2.Sasiedzi.ShouldContain(punkt2.Nastepnik);
      }

      [Test]
      public void PrzetwarzaczWysokosciNaAdekwatneDoPozycjiPoprawnieDziała()
      {
         var przetwarzacz = new ModyfikatorWysokosciNaAdekwatneDoPozycji();
         przetwarzacz.Przetwarzaj(_mapa);
         IPunkt p1 = _mapa.Punkty.ElementAt(0);
         IPunkt p2 = _mapa.Punkty.ElementAt(0);
         IPunkt p3 = _mapa.Punkty.ElementAt(0);
         p1.Wysokosc.ShouldEqual(p1.Pozycja.y);
         p2.Wysokosc.ShouldEqual(p2.Pozycja.y);
         p3.Wysokosc.ShouldEqual(p3.Pozycja.y);
      }

     /* [Test]
      public void RozdzielaczWodyILąduPrzypisujePunktomTypy() 
      {
        IRozdzielaczWodyILądu rozdzielacz = new ProstyRozdzielacz();
        IZbiorPunktowTopologicznych mapa = PrzetwarzaczZbioruPunktow.NaMapeGeograficzna(_mapa);
        mapa.ZastosujRozdzielaczWodyILądu(rozdzielacz);
        foreach (var komorkaGeograficzna in mapa.KomorkiGeograficzne)
        {
           komorkaGeograficzna.Typ.ShouldNotBeSameAs(TypKomorki.Brak);
        }
      }*/


      private static IMapa MockPrzykladowegoZbioruPunktowZSasiedztwem()
      {
         var punkt1 = new Punkt{Pozycja = new Vector3(10f, 3f, 2f)};
         var punkt2 = new Punkt{Pozycja = new Vector3(-4f, 1f, 3f)};
         var punkt3 = new Punkt{Pozycja = new Vector3(-3f, 0f, 5f)};
         punkt1.Sasiedzi = new List<IPunkt>{punkt2};
         punkt2.Sasiedzi = new List<IPunkt>{punkt1, punkt3};
         punkt3.Sasiedzi = new List<IPunkt>{punkt2};
         var punkty = new List<IPunkt> {punkt1, punkt2, punkt3};

         var mock = new Mock<IMapa>();
         mock.Setup(zp => zp.Punkty).Returns(punkty);
         return mock.Object;
      }

   }
}