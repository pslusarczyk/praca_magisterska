using System.Collections.Generic;
using Assets.Skrypty;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieFortunea;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;
using Random = System.Random;

namespace Assets.Editor
{
   public class DzialaniaNaWezlach
   {
      private readonly PoziomEditor _poziomEditor;
      private Random _random;

      public Poziom Poziom
      {
         get { return _poziomEditor.Poziom; }
         set { _poziomEditor.Poziom = value; }
      }

      private Random Random
      {
         get { return _random ?? (_random = new Random(Poziom.Ziarno)); }
         set { _random = value; }
      }

      public DzialaniaNaWezlach(PoziomEditor edytor)
      {
         _poziomEditor = edytor;
      }

      public void UkryjWezly()
      {
         foreach (Wezel wezel in _poziomEditor.Poziom._wezly)
         {
            wezel.renderer.enabled = false;
         }
      }

      public void GenerujWezly()
      {
         Poziom.KomponentPojemnika = UtworzPojemnik();

         _poziomEditor.Poziom._wezly = new Wezel[_poziomEditor.Poziom._rozmiarX, _poziomEditor.Poziom._rozmiarZ];
         for (int x = 0; x < _poziomEditor.Poziom._rozmiarX; ++x)
            for (int z = 0; z < _poziomEditor.Poziom._rozmiarZ; ++z)
            {
               float rozpietosc = _poziomEditor.Poziom._rozpietosc;
               var wezelObject =
                  (GameObject)
                     Object.Instantiate(Resources.Load("Wezel"), _poziomEditor.Poziom.transform.position + new Vector3(x * rozpietosc, 0f, z * rozpietosc),
                        Quaternion.identity);
               wezelObject.transform.parent = Poziom.KomponentPojemnika.Wezly.transform;
               wezelObject.GetComponent<Wezel>().pierwotnaPozycja = wezelObject.transform.position;
               _poziomEditor.Poziom._wezly[x, z] = wezelObject.GetComponent<Wezel>();
               if (x <= 1 || z <= 1 || x >= _poziomEditor.Poziom._rozmiarX - 2 || z >= _poziomEditor.Poziom._rozmiarZ - 2)
                  wezelObject.GetComponent<Wezel>().czySkrajny = true;
            }
      }

      private Pojemnik UtworzPojemnik()
      {
         var pojemnik = (GameObject) Object.Instantiate(Resources.Load("Pojemnik"), _poziomEditor.Poziom.transform.position,
            Quaternion.identity);
         pojemnik.name = _poziomEditor.Poziom.name + "_pojemnik";
         var komponentPojemnika = pojemnik.GetComponent<Pojemnik>();
         komponentPojemnika.Poziom = _poziomEditor.Poziom;
         komponentPojemnika.Komorki = komponentPojemnika.gameObject.transform.FindChild("Komorki").gameObject;
         komponentPojemnika.Rogi = komponentPojemnika.gameObject.transform.FindChild("Rogi").gameObject;
         komponentPojemnika.Wezly = komponentPojemnika.gameObject.transform.FindChild("Wezly").gameObject;
         return komponentPojemnika;
      }

      public void ZaburzWezly(bool pozostawSkrajne)
      {
         ZresetujUstawienieWezlow();
         float limitPrzesuniecia = _poziomEditor.Poziom._rozpietosc * .8f;
         foreach (Wezel w in _poziomEditor.Poziom._wezly)
         {
            if (pozostawSkrajne && w.czySkrajny)
               continue;
            var transpozycja = new Vector3(
               limitPrzesuniecia - 2 * (float)Random.NextDouble() * limitPrzesuniecia,
               0f,
               limitPrzesuniecia - 2 * (float)Random.NextDouble() * limitPrzesuniecia
               );
            w.transform.Translate(transpozycja);
         }
      }

      private void ZresetujUstawienieWezlow()
      {
         foreach (Wezel w in _poziomEditor.Poziom._wezly)
         {
            w.transform.position = w.pierwotnaPozycja;
         }
      }

      private IEnumerable<Vector> WezlyNaWektory()
      {
         foreach (Wezel w in _poziomEditor.Poziom._wezly)
         {
            yield return new Vector(w.transform.position.x, w.transform.position.z);
         }
      }

      public void GenerujKomorkiIRogi()
      {
         _poziomEditor.Poziom._krawedzieWoronoja = Fortune.ComputeVoronoiGraph(WezlyNaWektory()).Edges;

         var pf = new PrzetwarzaczFortunea();

         IMapa mapa = pf.Przetwarzaj(_poziomEditor.Poziom._krawedzieWoronoja);

         _poziomEditor.Poziom._mapa = mapa;

         foreach (var komorka in mapa.Komorki)
         {
            var nowa = (GameObject)Object.Instantiate(Resources.Load("KomorkaUnity"),
               komorka.Punkt.Pozycja, Quaternion.identity);
            nowa.transform.parent = _poziomEditor.Poziom.KomponentPojemnika.Komorki.transform;
            nowa.GetComponent<KomorkaUnity>().Komorka = komorka;
            _poziomEditor.Poziom._komorkiUnity.Add(nowa.GetComponent<KomorkaUnity>());
         }

         foreach (var rog in mapa.Rogi)
         {
            Vector3 pozycja = rog.Punkt.Pozycja;
            if (float.IsInfinity(pozycja.x) || float.IsInfinity(pozycja.y) || float.IsInfinity(pozycja.z))
               continue;
            var nowy = (GameObject)Object.Instantiate(Resources.Load("RogUnity"),
               rog.Punkt.Pozycja, Quaternion.identity);
            nowy.transform.parent = _poziomEditor.Poziom.KomponentPojemnika.Rogi.transform;
            nowy.GetComponent<RogUnity>().Rog = rog;
            _poziomEditor.Poziom._rogiUnity.Add(nowy.GetComponent<RogUnity>());
         }
      }
   }
}