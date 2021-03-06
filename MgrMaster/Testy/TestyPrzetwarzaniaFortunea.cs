﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieFortunea;
using NUnit.Framework;
using Should;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace Testy
{
   [TestFixture]
   public class TestyPrzetwarzaniaFortunea
   {
      [SetUp]
      public void SetUp()
      {
         _siatkaWezlow = PrzykladowaSiatka();
         _przetwarzacz = new PrzetwarzaczFortunea();
         _krawedzieWoronoja = Fortune.ComputeVoronoiGraph(_siatkaWezlow).Edges;
      }

      private IEnumerable _siatkaWezlow;
      private PrzetwarzaczFortunea _przetwarzacz;
      private HashSet<VoronoiEdge> _krawedzieWoronoja;

      private IEnumerable PrzykladowaSiatka(int rozmiar = 15)
      {
         for (float x = 0; x < rozmiar; ++x)
            for (float z = 0; z < rozmiar; ++z)
               yield return new Vector(x, z);
      }

      [Test]
      public void DoKażdejDwukrawędziPrzyczepioneSąDwieKomórkiIDwaRogi()
      {
         List<Dwukrawedz> dwukrawedzie = _przetwarzacz.Przetwarzaj(_krawedzieWoronoja).Dwukrawedzie;
         foreach (Dwukrawedz dwu in dwukrawedzie)
         {
            dwu.Lewa.ShouldNotBeNull();
            dwu.Prawa.ShouldNotBeNull();
            dwu.Pierwszy.ShouldNotBeNull();
            dwu.Drugi.ShouldNotBeNull();
         }
      }

      [Test]
      public void KazdyPunktMaPoCoNajmniejDwóchSasiadow()
      {
         _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
         foreach (IPunkt punkt in _przetwarzacz.Mapa.Punkty)
         {
            punkt.Sasiedzi.Count().ShouldBeInRange(2, Int32.MaxValue);
         }
      }

      [Test]
      public void PunktKażdejKomórkiMaTyleSąsiadówIleOnaRogów_APunktKażdegoRoguTyleIleMaKomórekIBliskich()
      {
         _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
         foreach (IKomorka komorka in _przetwarzacz.Mapa.Komorki)
         {
            komorka.Punkt.Sasiedzi.Count().ShouldEqual(komorka.Rogi.Count);
         }
         foreach (IRog rog in _przetwarzacz.Mapa.Rogi)
         {
            rog.Punkt.Sasiedzi.Count().ShouldEqual(rog.BliskieRogi.Count + rog.Komorki.Count);
         }
      }

      [Test]
      public void KomorkiMajaPoCoNajmniejDwaRogi()
         // w praktyce na nieregularnej siatce powinny co najmniej po 3 rogi
      {
         _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
         foreach (IKomorka komorka in _przetwarzacz.Mapa.Komorki)
         {
            komorka.Rogi.Count().ShouldBeInRange(2, Int32.MaxValue);
         }
      }

      [Test]
      public void KomórkiMająPoCoNajmniejDwiePrzyległe()
      {
         _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
         foreach (IKomorka komorka in _przetwarzacz.Mapa.Komorki)
         {
            komorka.PrzylegleKomorki.Count().ShouldBeInRange(2, Int32.MaxValue);
         }
      }

      [Test]
      public void NowoutworzonyGrafZawieraNiepusteDwukrawedzie()
      {
         foreach (VoronoiEdge krawedz in _krawedzieWoronoja)
         {
            krawedz.ShouldNotBeNull();
            krawedz.LeftData.ShouldNotBeNull();
            krawedz.RightData.ShouldNotBeNull();
            krawedz.VVertexA.ShouldNotBeNull();
            krawedz.VVertexB.ShouldNotBeNull();
         }
      }

      [Test]
      public void RogiMająPoCoNajmniejDwóchBliskich()
      {
         _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
         foreach (IRog rog in _przetwarzacz.Mapa.Rogi)
         {
            rog.BliskieRogi.Count().ShouldBeInRange(2, Int32.MaxValue);
         }
      }

      [Test]
      public void RogiŁącząPoCoNajmniejDwieKomórki()
         // (w praktyce przy nieregularnej siatce róg o skończonej pozycji łączy zawsze 3 komórki)
      {
         _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
         foreach (IRog rog in _przetwarzacz.Mapa.Rogi)
            rog.Komorki.Count().ShouldBeInRange(2, Int32.MaxValue);
      }

      [Test]
      public void WNowoUtworzonymGrafieIstniejaWspoldzieloneWierzcholki()
      {
         _siatkaWezlow = PrzykladowaSiatka();
         var wierzcholki = new List<Vector>();
         foreach (VoronoiEdge krawedz in _krawedzieWoronoja)
         {
            wierzcholki.Add(krawedz.LeftData);
            wierzcholki.Add(krawedz.RightData);
            wierzcholki.Add(krawedz.VVertexA);
            wierzcholki.Add(krawedz.VVertexB);
         }
         bool saPowtorzenia = false;
         foreach (Vector wierzcholek in wierzcholki)
         {
            if (wierzcholki.Count(w => w == wierzcholek) != 1)
               saPowtorzenia = true;
         }
         saPowtorzenia.ShouldBeTrue();
      }

      [Test]
      public void WNowoUtworzonymZbiorzeDwukrawędziPunktySąWspółdzielone()
      {
         _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
         var punkty = new List<IPunkt>();
         foreach (Dwukrawedz dwu in _przetwarzacz.Mapa.Dwukrawedzie)
         {
            punkty.Add(dwu.Lewa.Punkt);
            punkty.Add(dwu.Prawa.Punkt);
            punkty.Add(dwu.Pierwszy.Punkt);
            punkty.Add(dwu.Drugi.Punkt);
         }
         bool saPowtorzenia = false;
         foreach (IPunkt punkt in punkty)
         {
            if (punkty.Count(p => p == punkt) != 1)
               saPowtorzenia = true;
         }
         saPowtorzenia.ShouldBeTrue();
      }
   }
}