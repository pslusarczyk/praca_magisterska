using Assets.Editor.ExposeProperties;
using Assets.Editor.Konfiguracje;
using Assets.Skrypty;
using Assets.Skrypty.Narzedzia;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
   [CustomEditor(typeof(Poziom))]
   public class PoziomEditor : UnityEditor.Editor
   {
      PropertyField[] m_fields;
      private Poziom _poziom;
      private readonly DzialaniaNaMapie _dzialaniaNaMapie;
      private readonly DzialaniaNaWezlach _dzialaniaNaWezlach;
      private Warstwa _ostatniaWarstwa;

      private bool _pokazRogi = false;

      private float _zasiegZaburzenia = Konf.PoczStopienZaburzeniaWezlow;
      public int _rozmiarX = Konf.PoczRozmiarX;
      public int _rozmiarZ = Konf.PoczRozmiarZ;
      public float _rozpietosc = Konf.PoczRozpietosc;

      public bool PokazSciany
      {
         get { return Poziom._pokazSciany; }
         set { Poziom._pokazSciany = value; }
      }

      public Warstwa Warstwa
      {
         get { return _poziom._warstwa; }
         set { _poziom._warstwa = value; }
      }

      public Etap _etap = Etap.GenerowanieWezlow;

      [ExposeProperty]
      public string EtapTekst { get { return _etap.ToString(); } set { } } // musi byæ set ¿eby siê wyœwietla³o

      public Poziom Poziom
      {
         get { return _poziom ?? (_poziom = (Poziom)target); }
         set { _poziom = value; }
      }

      public Warstwa OstatniaWarstwa
      {
         set { _ostatniaWarstwa = value; }
         get { return _ostatniaWarstwa; }
      }

      public PoziomEditor()
      {
         _dzialaniaNaMapie = new DzialaniaNaMapie(this);
         _dzialaniaNaWezlach = new DzialaniaNaWezlach(this);
      }
      
      public void OnEnable()
      {
         _poziom = target as Poziom;
         _ostatniaWarstwa = Warstwa;
         m_fields = ExposeProperties.ExposeProperties.GetProperties(_poziom);
      }

      public override void OnInspectorGUI()
      {
         if (_poziom == null)
            return;
         DrawDefaultInspector();
         ExposeProperties.ExposeProperties.Expose(m_fields);
         _dzialaniaNaMapie.ObsluzZmianyWeWlasciwosciach();

         EditorGUILayout.LabelField("Generator poziomu", Konf.StylNaglowkaInspektora);

         GUI.color = new Color(.9f, .1f, .1f);
         if (GUILayout.Button("Resetuj"))
         {
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            _etap = Etap.GenerowanieWezlow;
         }
         GUI.color = Color.white;

         if (_etap == Etap.GenerowanieWezlow)
         {
            EditorGUILayout.LabelField("Okreœl wymiary poziomu:", Konf.StylNaglowkaInspektora);
            _rozmiarX = EditorGUILayout.IntSlider("Rozmiar X", _rozmiarX, Konf.MinRozmiar, Konf.MaksRozmiar);
            _rozmiarZ = EditorGUILayout.IntSlider("Rozmiar Z", _rozmiarZ, Konf.MinRozmiar, Konf.MaksRozmiar);
            _rozpietosc = EditorGUILayout.Slider("Rozpiêtoœæ", _rozpietosc, Konf.MinRozpietosc, Konf.MaksRozpietosc);
            if (GUILayout.Button("Generuj wezly"))
            {
               _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
               _dzialaniaNaWezlach.GenerujWezly();
               _etap = Etap.ZaburzanieWezlow;
            }
         }

         if ((_etap == Etap.ZaburzanieWezlow || _etap == Etap.TworzenieKomorekIRogow))
         {
            EditorGUILayout.LabelField("Okreœl stopieñ zaburzenia:", Konf.StylNaglowkaInspektora);
            _zasiegZaburzenia = EditorGUILayout.Slider("Stopieñ zaburzenia", _zasiegZaburzenia, 0f, 1f);
            if (GUILayout.Button("Zaburz wezly"))
            {
               _dzialaniaNaWezlach.ZaburzWezly(_zasiegZaburzenia * _rozpietosc, true);
               _etap = Etap.TworzenieKomorekIRogow;
            }
            if (GUILayout.Button("Utworz komorki i rogi"))
            {
               _dzialaniaNaWezlach.UkryjWezly();
            _dzialaniaNaWezlach.GenerujKomorkiIRogi();
            _etap = Etap.TworzenieMapyWysokosci;
            }
         }

         if (_etap >= Etap.TworzenieMapyWysokosci)
         {
            PokazSciany = GUILayout.Toggle(PokazSciany, "Poka¿ œciany");
            _pokazRogi = GUILayout.Toggle(_pokazRogi, "Pokaz rogi");

            if (!_pokazRogi)
               _dzialaniaNaMapie.UkryjRogi();
            else
               _dzialaniaNaMapie.PokazRogi();
         }

         if (_etap == Etap.TworzenieMapyWysokosci || _etap == Etap.RozdzielanieZiemiIWody)
         {
            if (GUILayout.Button("Generuj wysokoœci"))
            {
               _dzialaniaNaMapie.GenerujWysokosci();
               _dzialaniaNaMapie.PokazWarstweWysokosci();
               _etap = Etap.RozdzielanieZiemiIWody;
            } 
         }

         if (_etap == Etap.RozdzielanieZiemiIWody && GUILayout.Button("Rozdziel ziemie i wode"))
         {
            _dzialaniaNaMapie.RozdzielZiemieIWode();
            _dzialaniaNaMapie.PokazWarstweZiemiIWody();
         }

      }
   }
}