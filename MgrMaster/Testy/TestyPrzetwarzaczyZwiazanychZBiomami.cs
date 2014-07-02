using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieMapy;
using NUnit.Framework;
using Should;
using UnityEngine;

namespace Testy
{
   [TestFixture]
   public class TestyPrzetwarzaczyZwiazanychZBiomami
   {

      /*              Układ komórek i rogów używanych w testach:
       *          r1 r2/K3\r4
       *       K1 — K2     K5
       *             r3\K4/r5
       */

      #region Deklaracje

      private HashSet<IKomorka> _komorki;
      private IMapa _mapa;

      #endregion

      #region Testy

      [SetUp]
      public void SetUp()
      {
         _komorki = MockKomorek();
         _mapa = MockKlasyMapa(_komorki);
      }

      [TestCase(0f, 0f, Biom.Bagna)]
      [TestCase(0.3f, 0.3f, Biom.Bagna)]
      [TestCase(0f, 1f, Biom.Pustynia)]
      [TestCase(1f, 0f, Biom.GoleGory)]
      [TestCase(.9f, .4f, Biom.GoleGory)]
      [TestCase(1f, 1f, Biom.Tajga)]
      public void AktualizatorBiomówPoprawniePrzypisujeBiomy(float wilg, float temp, Biom oczekiwanyBiom)
      {
         var aktualizator = new AktualizatorBiomow(new KonfigAktualizatoraBiomow(
            new List<KonfiguracjaBiomu>
            {
               new KonfiguracjaBiomu(0f, 0f, Biom.Bagna),
               new KonfiguracjaBiomu(0f, 1f, Biom.Pustynia),
               new KonfiguracjaBiomu(1f, 0f, Biom.GoleGory),
               new KonfiguracjaBiomu(1f, 1f, Biom.Tajga)
            }));
         var k1 = _mapa.Komorki.ElementAt(0);
         k1.Dane.Temperatura = temp;
         k1.Dane.Wilgotnosc = wilg;
         k1.Dane.Podloze = Podloze.Ziemia;

         aktualizator.Przetwarzaj(_mapa);

         k1.Dane.Biom.ShouldEqual(oczekiwanyBiom);
      }




      #endregion

      #region Funkcje pomocnicze

      private static IMapa MockKlasyMapa(HashSet<IKomorka> komorki)
      {
         IMapa mapa = new Mapa();
         mapa.Komorki = (HashSet<IKomorka>) komorki;
         return mapa;
      }

      private static HashSet<IKomorka> MockKomorek() 
      {
         var k1 = new Komorka();
         var komorki = new HashSet<IKomorka>{k1};
         return komorki;
      }

      #endregion
   }
}