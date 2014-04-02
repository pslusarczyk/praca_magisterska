using System.Collections.Generic;
using LogikaGeneracji;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Testy
{
   [TestFixture]
   public class TestyPrzetwarzaczyZwiazanychZWlasciwosciamiKomorekIRogow
   {

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
         var punkt1 = new Punkt { Pozycja = new Vector3(10f, 3f, 2f) };
         var punkt2 = new Punkt { Pozycja = new Vector3(-4f, 1f, 3f) };
         var punkt3 = new Punkt { Pozycja = new Vector3(-3f, 0f, 5f) };
         punkt1.Sasiedzi = new List<IPunkt> { punkt2 };
         punkt2.Sasiedzi = new List<IPunkt> { punkt1, punkt3 };
         punkt3.Sasiedzi = new List<IPunkt> { punkt2 };
         var punkty = new List<IPunkt> { punkt1, punkt2, punkt3 };

         var mock = new Mock<IMapa>();
         mock.Setup(zp => zp.Punkty).Returns(punkty);
         return mock.Object;
      }
   }
}


