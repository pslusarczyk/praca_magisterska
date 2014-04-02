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
      private class PustyPrzetwarzaczZNastepnikiem : IPrzetwarzaczMapy
      {
         public IPrzetwarzaczMapy Nastepnik { get; set; }

         public PustyPrzetwarzaczZNastepnikiem()
         {
            Nastepnik = new PustyPrzetwarzacz();
         }

         public void Przetwarzaj(IMapa mapa){}
      }

      private class ProstyModyfikatorWysokosci : IPrzetwarzaczMapy
      {
         public const float WysokoscPierwszego = 3f;
         public const float WysokoscDrugiego = 2f;
         public const float WysokoscTrzeciego = 1f;
         public IPrzetwarzaczMapy Nastepnik { get; set; }

         public void Przetwarzaj(IMapa mapa)
         {
            mapa.Punkty.ElementAt(0).Wysokosc = WysokoscPierwszego;
            mapa.Punkty.ElementAt(1).Wysokosc = WysokoscDrugiego;
            mapa.Punkty.ElementAt(2).Wysokosc = WysokoscTrzeciego;
         }

         public ProstyModyfikatorWysokosci()
         {
            Nastepnik = new AktualizatorNastepstwaMapyWysokosci();
         }
      }

      private IMapa _mapa;

      [SetUp]
      public void SetUp()
      {
         _mapa = MockInterfejsuMapa();
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
         _mapa.ZastosujPrzetwarzanie(modyfikator);
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
         _mapa = MockKlasyMapa();
         var modyfikator = new ProstyModyfikatorWysokosci();
         _mapa.ZastosujPrzetwarzanie(modyfikator);
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
         _mapa = MockKlasyMapa();
         var modyfikator = new ProstyModyfikatorWysokosci();
         _mapa.ZastosujPrzetwarzanie(modyfikator);

         var punkt1 = _mapa.Punkty.ElementAt(0);
         var punkt2 = _mapa.Punkty.ElementAt(1);
         punkt1.Sasiedzi.ShouldContain(punkt1.Nastepnik);
         punkt2.Sasiedzi.ShouldContain(punkt2.Nastepnik);
      }

      [Test]
      public void PrzetwarzaczWysokosciNaAdekwatneDoPozycjiPoprawnieDziała()
      {
         var przetwarzacz = new ModyfikatorWysokosciNaAdekwatneDoPozycji();
         _mapa.ZastosujPrzetwarzanie(przetwarzacz);
         IPunkt p1 = _mapa.Punkty.ElementAt(0);
         IPunkt p2 = _mapa.Punkty.ElementAt(0);
         IPunkt p3 = _mapa.Punkty.ElementAt(0);
         p1.Wysokosc.ShouldEqual(p1.Pozycja.y);
         p2.Wysokosc.ShouldEqual(p2.Pozycja.y);
         p3.Wysokosc.ShouldEqual(p3.Pozycja.y);
      }

      [Test]
      public void PrzetwarzaczZNastępnikiemWywołujeGoPoSobie()
      {
         _mapa = new Mapa(); // nie mock, bo testujemy zachowanie związane z rejestracją przetwarzaczy
         var przetwarzacz = new PustyPrzetwarzaczZNastepnikiem();
         _mapa.ZastosujPrzetwarzanie(przetwarzacz);
         _mapa.ZastosowanePrzetwarzacze.ElementAt(0).ShouldBeType<PustyPrzetwarzaczZNastepnikiem>();
         _mapa.ZastosowanePrzetwarzacze.ElementAt(1).ShouldBeType<PustyPrzetwarzacz>();
      }

      private static IMapa MockInterfejsuMapa()
      {
         var punkty = MockPunktow();
         var mock = new Mock<IMapa>{};
         mock.Setup(m => m.Punkty).Returns(punkty);
         mock.Setup(m => m.ZastosujPrzetwarzanie(It.IsAny<IPrzetwarzaczMapy>()))
            .Callback((IPrzetwarzaczMapy p) => p.Przetwarzaj(mock.Object));
         return mock.Object;
      }

      private static IMapa MockKlasyMapa()
      {
         var punkty = MockPunktow();
         var mock = new Mock<Mapa>{CallBase = true};
         mock.Setup(m => m.Punkty).Returns(punkty);
         return mock.Object;
      }

      private static IEnumerable<IPunkt> MockPunktow()
      {
         var punkt1 = new Punkt { Pozycja = new Vector3(10f, 3f, 2f) };
         var punkt2 = new Punkt { Pozycja = new Vector3(-4f, 1f, 3f) };
         var punkt3 = new Punkt { Pozycja = new Vector3(-3f, 0f, 5f) };
         punkt1.Sasiedzi = new List<IPunkt> { punkt2 };
         punkt2.Sasiedzi = new List<IPunkt> { punkt1, punkt3 };
         punkt3.Sasiedzi = new List<IPunkt> { punkt2 };
         return new List<IPunkt> { punkt1, punkt2, punkt3 };
      }
   }
}