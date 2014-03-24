using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using Moq;
using NUnit.Framework;
using Should;
using UnityEngine;

namespace Testy
{
   [TestFixture]
   public class TestyTopologicznoGeograficzne
   {
      private class Punkt : IPunkt
      {
         public IEnumerable<IPunkt> Sasiedzi { get; set; }
         public Vector3 Pozycja { get; set; }
      }

      private class ProstyModyfikator : IModyfikatorWysokosci
      {
         public const float WysokoscPierwszego = 3f;
         public const float WysokoscDrugiego = 2f;
         public const float WysokoscTrzeciego = 1f;

         public void ModyfikujMape(IZbiorPunktowTopologicznych mapaWysokosci)
         {
            mapaWysokosci.PunktyTopologiczne.ElementAt(0).Wysokosc = WysokoscPierwszego;
            mapaWysokosci.PunktyTopologiczne.ElementAt(1).Wysokosc = WysokoscDrugiego;
            mapaWysokosci.PunktyTopologiczne.ElementAt(2).Wysokosc = WysokoscTrzeciego;
         }
      }

      private class ProstyRozdzielacz : IRozdzielaczWodyILądu
      {
         public void PrzetworzMape(MapaGeograficzna mapaGeograficzna)
         {
            foreach (var komorkaGeograficzna in mapaGeograficzna.KomorkiGeograficzne)
            {
               komorkaGeograficzna.Typ = TypKomorki.Morze;
            }
         }
      }

      private IZbiorPunktow _mapaProsta;

      [SetUp]
      public void SetUp()
      {
         _mapaProsta = MockPrzykladowegoZbioruPunktowZSasiedztwem();
      }

      [Test]
      public void NowaMapaGeograficznaPosiadaPunktyZWysokościąZero()
      {
         MapaGeograficzna mapaGeograficzna = PrzetwarzaczZbioruPunktow.NaMapeGeograficzna(_mapaProsta);
         foreach (IPunktTopologiczny pg in mapaGeograficzna.PunktyTopologiczne)
         {
            pg.Wysokosc.ShouldEqual(0);
         }
      }

      [Test]
      public void PunktyGeograficzneNowejMapyMająOdpowiednioPoustawianeSąsiedztwa()
      {
         MapaGeograficzna mapaGeograficzna = PrzetwarzaczZbioruPunktow.NaMapeGeograficzna(_mapaProsta);
         foreach (var p in mapaGeograficzna.MapaProsta.Punkty)
         {
            var odpowiednik = mapaGeograficzna.PunktyTopologiczne.First(pg => pg.Punkt.Pozycja == p.Pozycja);
            var punktySasiednieOdpowiednika = odpowiednik.Sasiedzi.Select(s => s.Punkt);
            CollectionAssert.AreEquivalent(punktySasiednieOdpowiednika, p.Sasiedzi);
         }
      }

      [Test]
      public void PoNalozeniuWysokosciPunktyGeograficzneMajaOdpowiednieWysokosci()
      {
         MapaGeograficzna mapaGeograficzna = PrzetwarzaczZbioruPunktow.NaMapeGeograficzna(_mapaProsta);
         var mojModyfikator = new ProstyModyfikator();
         mapaGeograficzna.ZastosujModyfikatorWysokosci(mojModyfikator);

         mapaGeograficzna.PunktyTopologiczne.ElementAt(0).Wysokosc
            .ShouldEqual(ProstyModyfikator.WysokoscPierwszego);
         mapaGeograficzna.PunktyTopologiczne.ElementAt(1).Wysokosc
            .ShouldEqual(ProstyModyfikator.WysokoscDrugiego);
         mapaGeograficzna.PunktyTopologiczne.ElementAt(2).Wysokosc
            .ShouldEqual(ProstyModyfikator.WysokoscTrzeciego);
      }


      [Test]
      public void PunktyGeograficznePrzetworzonejMapyMająOdpowiednioPoustawianychNastepnikow()
      {
         MapaGeograficzna mapaGeograficzna = PrzetwarzaczZbioruPunktow.NaMapeGeograficzna(_mapaProsta);
         var mojModyfikator = new ProstyModyfikator();
         mapaGeograficzna.ZastosujModyfikatorWysokosci(mojModyfikator);
         AktualizatorNastepstwaMapyWysokosci.Aktualizuj(mapaGeograficzna);
         var punkt1 = mapaGeograficzna.PunktyTopologiczne.ElementAt(0);
         var punkt2 = mapaGeograficzna.PunktyTopologiczne.ElementAt(1);
         var punkt3 = mapaGeograficzna.PunktyTopologiczne.ElementAt(2);
         punkt1.Nastepnik.ShouldEqual(punkt2);
         punkt2.Nastepnik.ShouldEqual(punkt3);
         punkt3.Nastepnik.ShouldBeNull();
      }

      [Test]
      public void NastepnicyPunktówGeograficznychSąIchSąsiadami()
      {
         IZbiorPunktowTopologicznych mapaGeograficzna = PrzetwarzaczZbioruPunktow.NaMapeGeograficzna(_mapaProsta);
         var mojModyfikator = new ProstyModyfikator();
         mapaGeograficzna.ZastosujModyfikatorWysokosci(mojModyfikator);
         AktualizatorNastepstwaMapyWysokosci.Aktualizuj(mapaGeograficzna);
         var punkt1 = mapaGeograficzna.PunktyTopologiczne.ElementAt(0);
         var punkt2 = mapaGeograficzna.PunktyTopologiczne.ElementAt(1);
         punkt1.Sasiedzi.ShouldContain(punkt1.Nastepnik);
         punkt2.Sasiedzi.ShouldContain(punkt2.Nastepnik);
      }

      [Test]
      public void RozdzielaczWodyILąduPrzypisujePunktomTypy() 
      {
        IRozdzielaczWodyILądu rozdzielacz = new ProstyRozdzielacz();
        IZbiorPunktowTopologicznych mapaGeograficzna = PrzetwarzaczZbioruPunktow.NaMapeGeograficzna(_mapaProsta);
        mapaGeograficzna.ZastosujRozdzielaczWodyILądu(rozdzielacz);
        foreach (var komorkaGeograficzna in mapaGeograficzna.KomorkiGeograficzne)
        {
           komorkaGeograficzna.Typ.ShouldNotBeSameAs(TypKomorki.Brak);
        }
      }

      private static IZbiorPunktow MockPrzykladowegoZbioruPunktowZSasiedztwem()
      {
         var punkt1 = new Punkt{Pozycja = new Vector3(10f, 3f, 2f)};
         var punkt2 = new Punkt{Pozycja = new Vector3(-4f, 1f, 3f)};
         var punkt3 = new Punkt{Pozycja = new Vector3(-3f, 0f, 5f)};
         punkt1.Sasiedzi = new List<Punkt>{punkt2};
         punkt2.Sasiedzi = new List<Punkt>{punkt1, punkt3};
         punkt3.Sasiedzi = new List<Punkt>{punkt2};
         var punkty = new List<IPunkt> {punkt1, punkt2, punkt3};

         var mock = new Mock<IZbiorPunktow>();
         mock.Setup(zp => zp.Punkty).Returns(punkty);
         return mock.Object;
      }

   }


   public class AktualizatorNastepstwaMapyWysokosci
   {
      public static void Aktualizuj(IZbiorPunktowTopologicznych mapaGeograficzna)
      {
         foreach (var punktGeograficzny in mapaGeograficzna.PunktyTopologiczne)
         {
            if (!punktGeograficzny.Sasiedzi.Any(s => s.Wysokosc < punktGeograficzny.Wysokosc))
               continue;

            float minimalnaWysokosc = punktGeograficzny.Sasiedzi.Min(s => s.Wysokosc);
            punktGeograficzny.Nastepnik = punktGeograficzny.Sasiedzi
                                                   .First(s => s.Wysokosc == minimalnaWysokosc);
         }
      }
   }
}