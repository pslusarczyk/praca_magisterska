using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using NUnit.Framework;
using Should;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace Testy
{
    [TestFixture]
    public class TestyPrzetwarzaniaFortunea
    {
        private IEnumerable _siatkaWezlow;
        private PrzetwarzaczGrafu _przetwarzacz;
        private HashSet<VoronoiEdge> _krawedzieWoronoja;

        [SetUp]
        public void SetUp()
        {
            _siatkaWezlow = PrzykladowaSiatka();
            _przetwarzacz = new PrzetwarzaczGrafu();
            _krawedzieWoronoja = Fortune.ComputeVoronoiGraph(_siatkaWezlow).Edges;
        }

        [Test]
        public void NowoutworzonyGrafZawieraNiepusteDwukrawedzie()
        {
            foreach (var krawedz in _krawedzieWoronoja)
            {
                krawedz.ShouldNotBeNull();
                krawedz.LeftData.ShouldNotBeNull();
                krawedz.RightData.ShouldNotBeNull();
                krawedz.VVertexA.ShouldNotBeNull();
                krawedz.VVertexB.ShouldNotBeNull();
            }
        }  
        
        [Test]
        public void WNowoUtworzonymGrafieIstniejaWspoldzieloneWierzcholki()
        {
            _siatkaWezlow = PrzykladowaSiatka();
            var wierzcholki = new List<Vector>();
            foreach (var krawedz in _krawedzieWoronoja)
            {
                wierzcholki.Add(krawedz.LeftData);
                wierzcholki.Add(krawedz.RightData);
                wierzcholki.Add(krawedz.VVertexA);
                wierzcholki.Add(krawedz.VVertexB);
            }
            bool saPowtorzenia = false;
            foreach (Vector wierzcholek in wierzcholki)
            {
                if(wierzcholki.Count(w => w == wierzcholek) != 1)
                   saPowtorzenia = true;
            }
            saPowtorzenia.ShouldBeTrue();
        }

        [Test]
        public void DoKażdejDwukrawędziPrzyczepioneSąDwieKomórkiIDwaRogi()
        {
            List<Dwukrawedz> dwukrawedzie = _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
            foreach (var dwu in dwukrawedzie)
            {
                dwu.Lewa.ShouldNotBeNull();
                dwu.Prawa.ShouldNotBeNull();
                dwu.Pierwszy.ShouldNotBeNull();
                dwu.Drugi.ShouldNotBeNull();
            }
        }

        [Test]
        public void WNowoUtworzonymZbiorzeDwukrawędziPunktySąWspółdzielone()
        {
            _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
            var punkty = new List<IPunkt>();
            foreach (var dwu in _przetwarzacz.Dwukrawedzie)
            {
                punkty.Add(dwu.Lewa);
                punkty.Add(dwu.Prawa);
                punkty.Add(dwu.Pierwszy);
                punkty.Add(dwu.Drugi);
            }
            bool saPowtorzenia = false;
            foreach (IPunkt punkt in punkty)
            {
                if (punkty.Count(p => p == punkt) != 1)
                    saPowtorzenia = true;
            }
            saPowtorzenia.ShouldBeTrue();
        }

        [Test]
        public void KomorkiMajaPoCoNajmniejDwaRogi()
            // w praktyce na nieregularnej siatce powinny co najmniej po 3 rogi
        {
            _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
            foreach (IKomorka komorka in _przetwarzacz.Komorki)
            {
                komorka.Rogi.Count().ShouldBeInRange(2, Int32.MaxValue);
            }
                        
        }

        [Test]
        public void RogiŁącząPoCoNajmniejDwieKomórki()
        // (w praktyce przy nieregularnej siatce róg o skończonej pozycji łączy zawsze 3 komórki)
        {
            _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
            foreach(IRog rog in _przetwarzacz.Rogi)
                rog.Komorki.Count().ShouldBeInRange(2, Int32.MaxValue);
        }

        [Test]
        public void KomórkiMająPoCoNajmniejDwiePrzyległe()
        {
            _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
            foreach (var komorka in _przetwarzacz.Komorki)
            {
                komorka.PrzylegleKomorki.Count().ShouldBeInRange(2, Int32.MaxValue);
            }
        }

        [Test]
        public void RogiMająPoCoNajmniejDwóchBliskich()
        {
            _przetwarzacz.Przetwarzaj(_krawedzieWoronoja);
            foreach (var rog in _przetwarzacz.Rogi)
            {
                rog.BliskieRogi.Count().ShouldBeInRange(2, Int32.MaxValue);
            }
        }

        private IEnumerable PrzykladowaSiatka(int rozmiar = 3)
        {
            for(float x=0; x<rozmiar; ++x)
                for(float z=0; z<rozmiar; ++z)
                    yield return new Vector(x, z);
        }
    }
}
