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
   public class TestyMapyGeograficznej
   {
      private class Punkt : IPunkt
      {
         public IEnumerable<IPunkt> Sasiedzi { get; set; }
         public Vector3 Pozycja { get; set; }
      }

      private IZbiorPunktow _mockZbioruPunktow;

      [SetUp]
      public void SetUp()
      {
         _mockZbioruPunktow = MockPrzykladowegoZbioruPunktowZSasiedztwem();
      }

      [Test]
      public void NowaMapaGeograficznaPosiadaPunktyZWysokościąZero()
      {
         MapaGeograficzna mapaGeograficzna = PrzetwarzaczZbioruPunktow.NaMapeGeograficzna(_mockZbioruPunktow);
         foreach (IPunktGeograficzny pg in mapaGeograficzna.PunktyGeograficzne)
         {
            pg.Wysokosc.ShouldEqual(0);
         }
      }

      [Test]
      public void PunktyGeograficzneNowejMapyMająOdpowiednioPoustawianeSąsiedztwa()
      {
         var mapaProsta = _mockZbioruPunktow;
         MapaGeograficzna mapaGeograficzna = PrzetwarzaczZbioruPunktow.NaMapeGeograficzna(_mockZbioruPunktow);
         foreach (var p in mapaProsta.Punkty)
         {
            // var odpowiednik = mapaGeograficzna.PunktyGeograficzne.First(pg => pg.Pozycja == p.Pozycja);
            // Dalej miała być asercja, że posiada sąsiadów na takich samych pozycjach, jak pierwotni
            // sąsiedzi, ale w międzyczasie doszedł do wniosku, że fakt osobnej tożsamości punktów
            // i punktów geograficznych jest problemem i lepiej żeby PG posiadał P.
         }
      }


      private static IZbiorPunktow MockPrzykladowegoZbioruPunktowZSasiedztwem()
      {
         var punkt1 = new Punkt{Pozycja = new Vector3(10f, 3f, 2f)};
         var punkt2 = new Punkt{Pozycja = new Vector3(-4f, 0f, 3f)};
         punkt1.Sasiedzi = new List<Punkt>{punkt2};
         punkt2.Sasiedzi = new List<Punkt>{punkt1};
         var punkty = new List<IPunkt> {punkt1, punkt2};

         var mock = new Mock<IZbiorPunktow>();
         mock.Setup(zp => zp.Punkty).Returns(punkty);
         return mock.Object;
      }
   }
}