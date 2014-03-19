using System.Collections;
using System.Collections.Generic;
using LogikaGeneracji;
using Moq;
using NUnit.Framework;
using Should;
using UnityEngine;

namespace Testy
{
   [TestFixture]
   public class TestyMapyWysokosci
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
      public void NowaMapaWysokościPosiadaPunktyZWysokościąZero()
      {
         MapaWysokosci mapaWysokosci = PrzetwarzaczZbioruPunktow.NaMapeWysokosci(_mockZbioruPunktow);
         foreach (IPunktMapyWysokosci pmp in mapaWysokosci.Punkty)
         {
            pmp.Wysokosc.ShouldEqual(0);
         }
      }

      private IZbiorPunktow MockPrzykladowegoZbioruPunktowZSasiedztwem()
      {
         var punkt1 = new PunktMapyWysokosci();
         var punkt2 = new PunktMapyWysokosci();
         punkt1.Sasiedzi = new List<PunktMapyWysokosci>{punkt2};
         punkt2.Sasiedzi = new List<PunktMapyWysokosci>{punkt1};
         var punkty = new List<IPunktMapyWysokosci> {punkt1, punkt2};

         var mock = new Mock<IZbiorPunktow>();
         mock.Setup(zp => zp.Punkty).Returns(punkty);
         return mock.Object;
      }
   }
}