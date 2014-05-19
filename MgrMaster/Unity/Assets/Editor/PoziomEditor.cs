using Assets.Editor.ExposeProperties;
using Assets.Skrypty;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
   [CustomEditor(typeof(Poziom))]
   public class PoziomEditor : UnityEditor.Editor
   {
      PropertyField[] m_fields;

      private Poziom _poziom;

      private readonly DzialaniaNaPoziomie _dzialania;

      private Warstwa _ostatniaWarstwa;

      private bool _pokazRogi = true;

      public Poziom Poziom
      {
         get { return _poziom ?? (_poziom = (Poziom)target); }
         set { _poziom = value; }
      }

      public PoziomEditor()
      {
         _dzialania = new DzialaniaNaPoziomie(this);
      }
      
      public void OnEnable()
      {
         _poziom = target as Poziom;
         _ostatniaWarstwa = _poziom.warstwa;
         m_fields = ExposeProperties.ExposeProperties.GetProperties(_poziom);
      }

      public override void OnInspectorGUI()
      {
         if (_poziom == null)
            return;
         DrawDefaultInspector();
         ExposeProperties.ExposeProperties.Expose(m_fields);

         ObsluzZmianyWeWlasciwosciach();

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
            Poziom._etap = Etap.TworzenieKomorekIRogow;
         }

         if (Poziom._etap >= Etap.TworzenieKomorekIRogow && GUILayout.Button("Utworz komorki i rogi"))
         {
            _dzialania.UkryjWezly();
            _dzialania.GenerujKomorkiIRogi();
            Poziom._etap = Etap.TworzenieMapyWysokosci;
         }

         
         if (Poziom._etap == Etap.TworzenieMapyWysokosci)
         {
            _pokazRogi = GUILayout.Toggle(_pokazRogi, "Pokaz rogi");

            if (!_pokazRogi)
               _dzialania.UkryjRogi();
            else
               _dzialania.PokazRogi();

            if (GUILayout.Button("Ustaw wysokoœci"))
            {
               _dzialania.UstawWysokosci();
            }

         }
      }

      private void ObsluzZmianyWeWlasciwosciach()
      {
         if (_poziom.warstwa != _ostatniaWarstwa)
         {
            _ostatniaWarstwa = _poziom.warstwa;
            if (_poziom.warstwa == Warstwa.Wysokosci)
               _dzialania.PokazWarstweWysokosci();
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
            punkt.Wysokosc = Mathf.PerlinNoise(punkt.Pozycja.x*.05f, punkt.Pozycja.z*.05f) * skalaWysokosci;
         }


            //punkt.Wysokosc = 6f + Mathf.Sin(punkt.Pozycja.x * .25f)*3f + Mathf.Cos(punkt.Pozycja.z * .25f)*3f;
            //punkt.Pozycja = new Vector3(punkt.Pozycja.x, punkt.Wysokosc, punkt.Pozycja.z);
      }
   }
}