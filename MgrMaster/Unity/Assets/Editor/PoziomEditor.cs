using System;
using System.Collections.Generic;
using Assets.Skrypty;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieFortunea;
using LogikaGeneracji.PrzetwarzanieMapy;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using ZewnetrzneBiblioteki.FortuneVoronoi;
using UnityEditor;
using UnityEngine;
using System.Linq;
using ZewnetrzneBiblioteki.PerlinNoise;
using Random = System.Random;

namespace Assets.Editor
{
   [CustomEditor(typeof(Poziom))]
   public class PoziomEditor : UnityEditor.Editor
   {
      private Poziom _poziom;

      [SerializeField]
      private Random _random;

      private Poziom Poziom
      {
         get { return _poziom ?? (_poziom = (Poziom)target); }
         set { _poziom = value; }
      }

      private Random Random
      {
         get { return _random ?? (_random = new Random(Poziom.Ziarno)); }
         set { _random = value; }
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (GUILayout.Button("Generuj mapê wysokoœci [test]"))
         {
            GenerujTeren();
         }

         if (Poziom._etap >= Etap.GenerowanieWezlow && GUILayout.Button("Resetuj"))
         {
            UsunWezlyRogiIKomorki();
            Poziom._etap = Etap.GenerowanieWezlow;
         }

         if (GUILayout.Button("Generuj wezly"))
         {
            GenerujWezly();
            Poziom._etap = Etap.ZaburzanieWezlow;
         }

         if (Poziom._etap >= Etap.ZaburzanieWezlow && GUILayout.Button("Zaburz wezly"))
         {
            ZaburzWezly(true);
            Poziom._etap = Etap.TworzenieDiagramuWoronoja;
         }

         if (Poziom._etap == Etap.TworzenieDiagramuWoronoja && GUILayout.Button("Komórki, rogi itp."))
         {
            UkryjWezly();
            GenerujKomorkiIRogi();
         }

         if (Poziom._etap == Etap.TworzenieDiagramuWoronoja && GUILayout.Button("Ustaw wysokoœci"))
         {
            UstawWysokosci();
            foreach (GameObject go in  GameObject.FindGameObjectsWithTag("Rog"))
            {
               go.transform.position = go.GetComponent<RogUnity>().Rog.Punkt.Pozycja;
            }
            foreach (GameObject go in  GameObject.FindGameObjectsWithTag("Komorka"))
            {
               go.transform.position = go.GetComponent<KomorkaUnity>().Komorka.Punkt.Pozycja;
            }
         }
         /*if (Poziom._etap >= Etap.TworzenieDiagramuWoronoja && GUILayout.Button("Stworz diagram Woronoja"))
         {
            StworzDiagramWoronoja();
         }

         if (Poziom._etap >= Etap.TworzenieMapyWysokosci && GUILayout.Button("Generuj mape wysokosci"))
         {
            GenerujMapeWysokosci();
         }*/


      }

      private void UstawWysokosci()
      {
         IPrzetwarzaczMapy wysokosci = new GeneratorWysokosci {Nastepnik = new AktualizatorNastepstwaMapyWysokosci()};
         wysokosci.Przetwarzaj(Poziom._mapa);
      }

      private void UkryjWezly()
      {
         foreach (Wezel wezel in Poziom._wezly)
         {
            wezel.renderer.enabled = false;
         }

      }

      private void GenerujKomorkiIRogi()
      {
         Poziom._krawedzieWoronoja = Fortune.ComputeVoronoiGraph(WezlyNaWektory()).Edges;

         var pf = new PrzetwarzaczFortunea();

         IMapa mapa = pf.Przetwarzaj(Poziom._krawedzieWoronoja);

         Poziom._mapa = mapa;

         foreach (var komorka in mapa.Komorki)
         {
            var nowa = (GameObject)Instantiate(Resources.Load("KomorkaUnity"),
                                 komorka.Punkt.Pozycja, Quaternion.identity);
            nowa.transform.parent = GameObject.Find("Komorki").transform;
            nowa.GetComponent<KomorkaUnity>().Komorka = komorka;
         }

         foreach (var rog in mapa.Rogi)
         {
            Vector3 pozycja = rog.Punkt.Pozycja;
            if (float.IsInfinity(pozycja.x) || float.IsInfinity(pozycja.y) || float.IsInfinity(pozycja.z))
               continue;
            var nowy = (GameObject)Instantiate(Resources.Load("RogUnity"),
                                 rog.Punkt.Pozycja, Quaternion.identity);
            nowy.transform.parent = GameObject.Find("Rogi").transform; // todo pozmieniaæ te find jakoœ ¿eby nie polegaæ na nazwach
            nowy.GetComponent<RogUnity>().Rog = rog;
         }

         }

      private void UsunWezlyRogiIKomorki()
      {
         var obiektyDoUsuniecia = new List<GameObject>();
         foreach (GameObject go in GameObject.FindGameObjectsWithTag("Wezel") // todo szukaæ tylko w dzieciach danego poziomu
                                  .Concat(GameObject.FindGameObjectsWithTag("Rog")
                                  .Concat(GameObject.FindGameObjectsWithTag("Komorka")))) 
         {
               obiektyDoUsuniecia.Add(go.gameObject);
         }
         obiektyDoUsuniecia.ForEach(w => DestroyImmediate(w));
         Poziom._wezly = null;
         Poziom._mapa = null;
         Poziom._krawedzieWoronoja = null;
         Poziom._etap = Etap.GenerowanieWezlow;
      }

      public void GenerujWezly()
      {
         UsunWezlyRogiIKomorki();

         Poziom._wezly = new Wezel[Poziom._rozmiarX, Poziom._rozmiarZ];
         for (int x = 0; x < Poziom._rozmiarX; ++x)
            for (int z = 0; z < Poziom._rozmiarZ; ++z)
            {
               float rozpietosc = Poziom._rozpietosc;
               var wezelObject =
                   (GameObject)
                   Instantiate(Resources.Load("Wezel"),
                               Poziom.transform.position + new Vector3(x * rozpietosc, 0f, z * rozpietosc),
                               Quaternion.identity);
               wezelObject.transform.parent = GameObject.Find("Wezly").transform;
               wezelObject.GetComponent<Wezel>().pierwotnaPozycja = wezelObject.transform.position;
               Poziom._wezly[x, z] = wezelObject.GetComponent<Wezel>();
               if (x <= 1 || z <= 1 || x >= Poziom._rozmiarX - 2 || z >= Poziom._rozmiarZ - 2)
                  wezelObject.GetComponent<Wezel>().czySkrajny = true;
            }
         Poziom._etap = Etap.ZaburzanieWezlow;
      }

      public void ZaburzWezly(bool pozostawSkrajne)
      {
         ZresetujUstawienieWezlow();
         float limitPrzesuniecia = Poziom._rozpietosc * .8f;
         foreach (Wezel w in Poziom._wezly)
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
         foreach (Wezel w in Poziom._wezly)
         {
            w.transform.position = w.pierwotnaPozycja;
         }
      }

      private IEnumerable<Vector> WezlyNaWektory()
      {
         foreach (Wezel w in Poziom._wezly)
         {
            yield return new Vector(w.transform.position.x, w.transform.position.z);
         }
      }

      private void StworzDiagramWoronoja()
      {

         Poziom._krawedzieWoronoja = Fortune.ComputeVoronoiGraph(WezlyNaWektory()).Edges;
         foreach (VoronoiEdge k in Poziom._krawedzieWoronoja)
         {
             //Wezel lewy = k.LeftData._wezel;
             //lewy._scianyKomorki.Add(new Para<Vector3, Vector3>(k.VVertexA.ToVector3(), k.VVertexB.ToVector3()));
             //
             //Wezel prawy = k.RightData._wezel;
             //prawy._scianyKomorki.Add(new Para<Vector3, Vector3>(k.VVertexA.ToVector3(), k.VVertexB.ToVector3()));
         }
         Poziom._etap = Etap.TworzenieMapyWysokosci;
         SceneView.RepaintAll();
      }

      private void GenerujMapeWysokosci()
      {
         float skalaWysokosci = 4f;
         float[][] mapa = PerlinTools.GeneratePerlinNoise(Poziom._rozmiarX, Poziom._rozmiarZ, 2);
         for (int x = 0; x < Poziom._wezly.GetLength(0); ++x)
            for (int z = 0; z < Poziom._wezly.GetLength(1); ++z)
            {
               var wezel = Poziom._wezly[x, z].GetComponent<Wezel>();
               //wezel._wysokosc = mapa[x][z];
               wezel.transform.Translate(0f, mapa[x][z] * skalaWysokosci, 0f);
               var kopiaMaterialu = new Material(wezel.renderer.sharedMaterial);
               //if (wezel._wysokosc > Poziom._poziomMorza)
               //{
               //   kopiaMaterialu.color = new Color(mapa[x][z] * 4f, 0f, 0f);
               //}
               //else
               //   kopiaMaterialu.color = new Color(0f, .7f, .95f);
               wezel.renderer.sharedMaterial = kopiaMaterialu;
            }
      }

      public void GenerujTeren()
      {
         //throw new NotImplementedException ();
      }
   }

   internal class GeneratorWysokosci : IPrzetwarzaczMapy
   {
      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public void Przetwarzaj(IMapa mapa)
      {
         foreach (IPunkt punkt in mapa.Punkty)
         {
            punkt.Wysokosc = 6f + Mathf.Sin(punkt.Pozycja.x * .25f)*3f + Mathf.Cos(punkt.Pozycja.z * .25f)*3f;
            punkt.Pozycja = new Vector3(punkt.Pozycja.x, punkt.Wysokosc, punkt.Pozycja.z);
         }
      }
   }
}