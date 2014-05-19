using System.Collections.Generic;
using Assets.Editor.ExposeProperties;
using Assets.Skrypty;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieFortunea;
using LogikaGeneracji.PrzetwarzanieMapy;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using ZewnetrzneBiblioteki.FortuneVoronoi;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Random = System.Random;

namespace Assets.Editor
{
   [CustomEditor(typeof(Poziom))]
   public class PoziomEditor : UnityEditor.Editor
   {
      PropertyField[] m_fields;

      private Poziom _poziom;

      private readonly DzialaniaNaPoziomie _dzialania;

      public PoziomEditor()
      {
         _dzialania = new DzialaniaNaPoziomie(this);
      }

      public Poziom Poziom
      {
         get { return _poziom ?? (_poziom = (Poziom)target); }
         set { _poziom = value; }
      }

      public void naZdarzenie () { Debug.Log("Eureka!"); }

      public void OnEnable()
      {
         _poziom = target as Poziom;
         m_fields = ExposeProperties.ExposeProperties.GetProperties(_poziom);
      }

      public override void OnInspectorGUI()
      {
         if (_poziom == null)
            return;

         DrawDefaultInspector();
         ExposeProperties.ExposeProperties.Expose(m_fields);

         if (Poziom._etap >= Etap.GenerowanieWezlow && GUILayout.Button("Resetuj"))
         {
            _dzialania.UsunWezlyRogiIKomorki();
            Poziom._etap = Etap.GenerowanieWezlow;
         }

         if (GUILayout.Button("Generuj wezly"))
         {
            _dzialania.GenerujWezly();
            Poziom._etap = Etap.ZaburzanieWezlow;
         }

         if (Poziom._etap >= Etap.ZaburzanieWezlow && GUILayout.Button("Zaburz wezly"))
         {
            _dzialania.ZaburzWezly(true);
            Poziom._etap = Etap.TworzenieDiagramuWoronoja;
         }

         if (Poziom._etap == Etap.TworzenieDiagramuWoronoja && GUILayout.Button("Komórki, rogi itp."))
         {
            _dzialania.UkryjWezly();
            _dzialania.GenerujKomorkiIRogi();
         }

         if (Poziom._etap == Etap.TworzenieDiagramuWoronoja && GUILayout.Button("Ustaw wysokoœci"))
         {
            _dzialania.UstawWysokosci();
            return;
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
   }


   internal class GeneratorWysokosci : BazaPrzetwarzacza
   {
      public override void Przetwarzaj(IMapa mapa)
      {
        // float[][] wys = PerlinTools.GeneratePerlinNoise(1000, 1000, 2);
         const float skalaWysokosci = 4f;
         foreach (IPunkt punkt in mapa.Punkty)
         {
            punkt.Wysokosc = Mathf.PerlinNoise(punkt.Pozycja.x*.5f, punkt.Pozycja.z*.5f) * skalaWysokosci;
         }


            //punkt.Wysokosc = 6f + Mathf.Sin(punkt.Pozycja.x * .25f)*3f + Mathf.Cos(punkt.Pozycja.z * .25f)*3f;
            //punkt.Pozycja = new Vector3(punkt.Pozycja.x, punkt.Wysokosc, punkt.Pozycja.z);

      }
   }
}