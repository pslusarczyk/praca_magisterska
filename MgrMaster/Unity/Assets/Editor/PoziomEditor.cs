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

         if (GUILayout.Button("Zmieñ kolor komórki"))
         {
            GameObject komorka = GameObject.FindGameObjectWithTag("Komorka");
				var nowa = Resources.Load<Material>("prototype_textures/Materials/proto_blue 1");
            komorka.renderer.material = nowa;
         }
      }

      private void UstawWysokosci()
      {
         IPrzetwarzaczMapy wysokosci = new GeneratorWysokosci {Nastepnik = new AktualizatorNastepstwaMapyWysokosci()};
         wysokosci.Przetwarzaj(Poziom._mapa);
         AktualizujKomorkiIRogiUnity();
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
            Poziom._komorkiUnity.Add(nowa.GetComponent<KomorkaUnity>());
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
            Poziom._rogiUnity.Add(nowy.GetComponent<RogUnity>());
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

      private void AktualizujKomorkiIRogiUnity()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.transform.position =
               new Vector3(komorkaUnity.transform.position.x,
                  komorkaUnity.transform.position.x*1.5f, komorkaUnity.transform.position.z);
         }
      }
 
   }

   internal class GeneratorWysokosci : BazaPrzetwarzacza
   {
      public override void Przetwarzaj(IMapa mapa)
      {
        // float[][] wys = PerlinTools.GeneratePerlinNoise(1000, 1000, 2);
         const float skalaWysokosci = 4f;
         foreach (IPunkt punkt in mapa.Punkty)
         {
            punkt.Wysokosc = Mathf.PerlinNoise(punkt.Pozycja.x, punkt.Pozycja.z);
				int a =5;
			//var kopiaMaterialu = new Material(wezel.renderer.sharedMaterial);
            //if (wezel._wysokosc > Poziom._poziomMorza)
            //{
            //   kopiaMaterialu.color = new Color(mapa[x][z] * 4f, 0f, 0f);
            //}
            //else
            //   kopiaMaterialu.color = new Color(0f, .7f, .95f);
            //wezel.renderer.sharedMaterial = kopiaMaterialu;
         }


            //punkt.Wysokosc = 6f + Mathf.Sin(punkt.Pozycja.x * .25f)*3f + Mathf.Cos(punkt.Pozycja.z * .25f)*3f;
            //punkt.Pozycja = new Vector3(punkt.Pozycja.x, punkt.Wysokosc, punkt.Pozycja.z);

      }
   }
}