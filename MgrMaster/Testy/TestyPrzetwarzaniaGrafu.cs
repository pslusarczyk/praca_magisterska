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
    public class TestyPrzetwarzaniaGrafu
    {
        [Test]
        public void NowoutworzonyGrafZawieraNiepusteDwukrawedzie()
        {
            var siatka = PrzykladowaSiatka();
            var krawedzieWoronoja = Fortune.ComputeVoronoiGraph(siatka).Edges;
            foreach (var krawedz in krawedzieWoronoja)
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
            var siatka = PrzykladowaSiatka();
            var krawedzieWoronoja = Fortune.ComputeVoronoiGraph(siatka).Edges;
            var wierzcholki = new List<Vector>();
            foreach (var krawedz in krawedzieWoronoja)
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
        public void DoKazdejDwukrawedziPrzyczepioneSaDwieKomorki()
        {
            var siatka = PrzykladowaSiatka();
            var krawedzieWoronoja = Fortune.ComputeVoronoiGraph(siatka).Edges;
            var dwukrawedzie = PrzetwarzaczGrafu.NaDwukrawedzie(krawedzieWoronoja);
            //var komorki = PrzetwarzaczGrafu.UtworzKomorki(krawedzieWoronoja);

        }


        private IEnumerable PrzykladowaSiatka(int rozmiar = 3)
        {
            for(float x=0; x<rozmiar; ++x)
                for(float z=0; z<rozmiar; ++z)
                    yield return new Vector(x, z);
        }
    }
}
