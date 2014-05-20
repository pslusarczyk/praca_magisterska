using Assets.Editor.ExposeProperties;
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
         _ostatniaWarstwa = _poziom.warstwa;
         m_fields = ExposeProperties.ExposeProperties.GetProperties(_poziom);
      }

      public override void OnInspectorGUI()
      {
         if (_poziom == null)
            return;
         DrawDefaultInspector();
         ExposeProperties.ExposeProperties.Expose(m_fields);

         _dzialaniaNaMapie.ObsluzZmianyWeWlasciwosciach();

         if (GUILayout.Button("Resetuj"))
         {
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            Poziom._etap = Etap.GenerowanieWezlow;
         }

         if (Poziom._etap == Etap.GenerowanieWezlow && GUILayout.Button("Generuj wezly"))
         {
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            _dzialaniaNaWezlach.GenerujWezly();
            Poziom._etap = Etap.ZaburzanieWezlow;
         }

         if ((Poziom._etap == Etap.ZaburzanieWezlow || Poziom._etap == Etap.TworzenieKomorekIRogow))
         {
            if (GUILayout.Button("Zaburz wezly"))
            {
               _dzialaniaNaWezlach.ZaburzWezly(true);
               Poziom._etap = Etap.TworzenieKomorekIRogow;
            }
            if (GUILayout.Button("Utworz komorki i rogi"))
            {
               _dzialaniaNaWezlach.UkryjWezly();
            _dzialaniaNaWezlach.GenerujKomorkiIRogi();
            Poziom._etap = Etap.TworzenieMapyWysokosci;
            }
         }

         if (Poziom._etap >= Etap.TworzenieMapyWysokosci)
         {
            _pokazRogi = GUILayout.Toggle(_pokazRogi, "Pokaz rogi");

            if (!_pokazRogi)
               _dzialaniaNaMapie.UkryjRogi();
            else
               _dzialaniaNaMapie.PokazRogi();
         }

         if (Poziom._etap == Etap.TworzenieMapyWysokosci || Poziom._etap == Etap.RozdzielanieZiemiIWody)
         {
            if (GUILayout.Button("Generuj wysokoœci"))
            {
               _dzialaniaNaMapie.GenerujWysokosci();
               _dzialaniaNaMapie.PokazWarstweWysokosci();
               Poziom._etap = Etap.RozdzielanieZiemiIWody;
            } 
         }

         if (Poziom._etap == Etap.RozdzielanieZiemiIWody && GUILayout.Button("Rozdziel ziemie i wode"))
         {
            _dzialaniaNaMapie.RozdzielZiemieIWode();
            _dzialaniaNaMapie.PokazWarstweZiemiIWody();
         }

      }
   }
}