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
   public class TestyMapyGeograficznej
   {
      private class Punkt : IPunkt
      {
         public IEnumerable<IPunkt> SasiedniePunkty { get; set; }
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
         foreach (IPunktGeograficzny pg in mapaGeograficzna.Punkty)
         {
            pg.Wysokosc.ShouldEqual(0);
         }
      }

      private IZbiorPunktow MockPrzykladowegoZbioruPunktowZSasiedztwem()
      {
         var punkt1 = new PunktGeograficzny();
         var punkt2 = new PunktGeograficzny();
         punkt1.SasiedniePunkty = new List<PunktGeograficzny>{punkt2};
         punkt2.SasiedniePunkty = new List<PunktGeograficzny>{punkt1};
         var punkty = new List<IPunktGeograficzny> {punkt1, punkt2};

         var mock = new Mock<IZbiorPunktow>();
         mock.Setup(zp => zp.Punkty).Returns(punkty);
         return mock.Object;
      }
   }
}