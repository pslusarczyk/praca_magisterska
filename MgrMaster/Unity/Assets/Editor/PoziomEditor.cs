using System.Collections.Generic;
using Assets.Skrypty;
using UnityEditor;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;
using ZewnetrzneBiblioteki.PerlinNoise;
using Random = System.Random;

namespace Assets.Editor
{
   [CustomEditor(typeof (Poziom))]
   public class PoziomEditor : UnityEditor.Editor
   {
      private Poziom _poziom;

      [SerializeField] private Random _random;

      private Poziom Poziom
      {
         get { return _poziom ?? (_poziom = (Poziom) target); }
         set { _poziom = value; }
      }

      private Random Random
      {
         get { return _random ?? (_random = new Random(Skrypty.Poziom.ziarno)); }
         set { _random = value; }
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (Skrypty.Poziom._etap >= Etap.GenerowanieWezlow && GUILayout.Button("Resetuj"))
         {
            UsunWezly();
            Skrypty.Poziom._etap = Etap.GenerowanieWezlow;
         }

         if (GUILayout.Button("Generuj wezly"))
         {
            GenerujWezly();
            Skrypty.Poziom._etap = Etap.ZaburzanieWezlow;
         }

         if (Skrypty.Poziom._etap >= Etap.ZaburzanieWezlow && GUILayout.Button("Zaburz wezly"))
         {
            ZaburzWezly(true);
            Skrypty.Poziom._etap = Etap.TworzenieDiagramuWoronoja;
         }

         if (Skrypty.Poziom._etap >= Etap.TworzenieDiagramuWoronoja && GUILayout.Button("Stworz diagram Woronoja"))
         {
            StworzDiagramWoronoja();
         }

         if (Skrypty.Poziom._etap >= Etap.TworzenieMapyWysokosci && GUILayout.Button("Generuj mape wysokosci"))
         {
            GenerujMapeWysokosci();
         }

         if (GUILayout.Button("Generuj teren [test]"))
         {
            GenerujTeren();
         }
      }

      private void UsunWezly()
      {
         var wezlyDoUsuniecia = new List<GameObject>();
         foreach (Transform d in Skrypty.Poziom.transform)
         {
            if (d.gameObject.CompareTag("Wezel"))
               wezlyDoUsuniecia.Add(d.gameObject);
         }
         wezlyDoUsuniecia.ForEach(w => DestroyImmediate(w));
         Skrypty.Poziom._wezly = null;
         Skrypty.Poziom._krawedzieWoronoja = null;
         Skrypty.Poziom._etap = Etap.GenerowanieWezlow;
      }

      public void GenerujWezly()
      {
         UsunWezly();

         Skrypty.Poziom._wezly = new Wezel[Skrypty.Poziom._rozmiarX, Skrypty.Poziom._rozmiarZ];
         for (int x = 0; x < Skrypty.Poziom._rozmiarX; ++x)
            for (int z = 0; z < Skrypty.Poziom._rozmiarZ; ++z)
            {
               float rozpietosc = Skrypty.Poziom._rozpietosc;
               var wezelObject =
                  (GameObject)
                     Instantiate(Resources.Load("Wezel"),
                        Skrypty.Poziom.transform.position + new Vector3(x*rozpietosc, 0f, z*rozpietosc),
                        Quaternion.identity);
               wezelObject.transform.parent = Skrypty.Poziom.transform;
               wezelObject.GetComponent<Wezel>().pierwotnaPozycja = wezelObject.transform.position;
               Skrypty.Poziom._wezly[x, z] = wezelObject.GetComponent<Wezel>();
               if (x <= 1 || z <= 1 || x >= Skrypty.Poziom._rozmiarX - 2 || z >= Skrypty.Poziom._rozmiarZ - 2)
                  wezelObject.GetComponent<Wezel>().czySkrajny = true;
            }
         Skrypty.Poziom._etap = Etap.ZaburzanieWezlow;
      }

      public void ZaburzWezly(bool pozostawSkrajne)
      {
         ZresetujUstawienieWezlow();
         float limitPrzesuniecia = Skrypty.Poziom._rozpietosc*.8f;
         foreach (Wezel w in Skrypty.Poziom._wezly)
         {
            if (pozostawSkrajne && w.czySkrajny)
               continue;
            var transpozycja = new Vector3(
               limitPrzesuniecia - 2*(float) System.Random.NextDouble()*limitPrzesuniecia,
               0f,
               limitPrzesuniecia - 2*(float) System.Random.NextDouble()*limitPrzesuniecia
               );
            w.transform.Translate(transpozycja);
         }
      }

      private void ZresetujUstawienieWezlow()
      {
         foreach (Wezel w in Skrypty.Poziom._wezly)
         {
            w.transform.position = w.pierwotnaPozycja;
         }
      }

      private IEnumerable<Vector> WezlyNaWektory()
      {
         foreach (Wezel w in Skrypty.Poziom._wezly)
         {
            yield return new Vector( /*w, */w.transform.position.x, w.transform.position.z);
         }
      }

      private void StworzDiagramWoronoja()
      {
         /*Poziom._krawedzieWoronoja = Fortune.ComputeVoronoiGraph(WezlyNaWektory()).Edges;
            foreach (VoronoiEdge k in Poziom._krawedzieWoronoja)
            {
                Wezel lewy = k.LeftData._wezel;
                lewy._scianyKomorki.Add(new Para<Vector3, Vector3>(k.VVertexA.ToVector3(), k.VVertexB.ToVector3()));

                Wezel prawy = k.RightData._wezel;
                prawy._scianyKomorki.Add(new Para<Vector3, Vector3>(k.VVertexA.ToVector3(), k.VVertexB.ToVector3()));
            }
            Poziom._etap = Etap.TworzenieMapyWysokosci;
            SceneView.RepaintAll();*/
      }

      private void GenerujMapeWysokosci()
      {
         float skalaWysokosci = 4f;
         float[][] mapa = PerlinTools.GeneratePerlinNoise(Skrypty.Poziom._rozmiarX, Skrypty.Poziom._rozmiarZ, 2);
         for (int x = 0; x < Skrypty.Poziom._wezly.GetLength(0); ++x)
            for (int z = 0; z < Skrypty.Poziom._wezly.GetLength(1); ++z)
            {
               var wezel = Skrypty.Poziom._wezly[x, z].GetComponent<Wezel>();
               wezel._wysokosc = mapa[x][z];
               wezel.transform.Translate(0f, mapa[x][z]*skalaWysokosci, 0f);
               var kopiaMaterialu = new Material(wezel.renderer.sharedMaterial);
               if (wezel._wysokosc > Skrypty.Poziom._poziomMorza)
               {
                  kopiaMaterialu.color = new Color(mapa[x][z]*4f, 0f, 0f);
               }
               else
                  kopiaMaterialu.color = new Color(0f, .7f, .95f);
               wezel.renderer.sharedMaterial = kopiaMaterialu;
            }
      }

      public void GenerujTeren()
      {
         //throw new NotImplementedException ();
      }
   }
}