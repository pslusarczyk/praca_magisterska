using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using NUnit.Framework;
using Should;

namespace Testy
{
   [TestFixture]
   public class TestyMechanizmuPrzetwarzania
   {
      private IMapa _mapa;
      private IPrzetwarzaczMapy _przetwarzacz1;
      private IPrzetwarzaczMapy _przetwarzacz2;
      private IPrzetwarzaczMapy _przetwarzacz3;

      private class Przetwarzacz1 : IPrzetwarzaczMapy
      {
         public IPrzetwarzaczMapy Nastepnik { get; set; }
         public void Przetwarzaj(IMapa mapa){}
      }

      private class Przetwarzacz2zParametrem : IPrzetwarzaczMapy
      {
         private int _x;
         public IPrzetwarzaczMapy Nastepnik { get; set; }
         public void Przetwarzaj(IMapa mapa){}

         public Przetwarzacz2zParametrem(int x) { _x = x; }
      }

      private class Przetwarzacz3 : IPrzetwarzaczMapy
      {
         public IPrzetwarzaczMapy Nastepnik { get; set; }
         public void Przetwarzaj(IMapa mapa){}
      }

      [SetUp]
      public void SetUp()
      {
         _mapa = new Mapa();
         _przetwarzacz1 = new Przetwarzacz1();
         _przetwarzacz2 = new Przetwarzacz2zParametrem(100);
         _przetwarzacz3 = new Przetwarzacz3();
      }

      // todo czy ten test jest potrzebny? To bardziej testowanie listy niż jakiejś funkcjonalności
      [Test]
      public void PoPojedyńczychPrzetwarzaniachPrzetwarzaczeDodająSięDoListyPrzetworzeńMapy()
      {
         _mapa.ZastosujPrzetwarzanie(_przetwarzacz1);

         _mapa.ZastosowanePrzetwarzacze.ElementAt(0).ShouldEqual(_przetwarzacz1);
         _mapa.ZastosowanePrzetwarzacze.Count.ShouldEqual(1);

         _mapa.ZastosujPrzetwarzanie(_przetwarzacz2);

         _mapa.ZastosowanePrzetwarzacze.ElementAt(0).ShouldEqual(_przetwarzacz1);
         _mapa.ZastosowanePrzetwarzacze.ElementAt(1).ShouldEqual(_przetwarzacz2);
         _mapa.ZastosowanePrzetwarzacze.Count.ShouldEqual(2);
      }

      [Test]
      public void KreatorCiąguPrzetwarzańPoprawnieDziała()
      {
         IList<IPrzetwarzaczMapy> przetwarzacze
            = new List<IPrzetwarzaczMapy>{_przetwarzacz1, _przetwarzacz2, _przetwarzacz3};
         KreatorCiaguPrzetwarzan.UstawNastepstwa(przetwarzacze);

         _przetwarzacz1.Nastepnik.ShouldEqual(_przetwarzacz2);
         _przetwarzacz2.Nastepnik.ShouldEqual(_przetwarzacz3);
         _przetwarzacz3.Nastepnik.ShouldBeNull();
      }
   }
}