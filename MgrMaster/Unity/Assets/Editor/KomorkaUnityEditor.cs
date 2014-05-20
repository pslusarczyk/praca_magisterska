using System;
using System.Collections.Generic;
using Assets.Editor.ExposeProperties;
using Assets.Skrypty;
using Assets.Skrypty.Narzedzia;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
   [CustomEditor(typeof(KomorkaUnity))]
   public class KomorkaUnityEditor : UnityEditor.Editor
   {
      PropertyField[] m_fields;
      private KomorkaUnity _komorkaUnity;

      private bool _pokazRogi = false;

      public KomorkaUnity KomorkaUnity
      {
         get { return _komorkaUnity ?? (_komorkaUnity = (KomorkaUnity)target); }
         set { _komorkaUnity = value; }
      }
      
      public void OnEnable()
      {
         _komorkaUnity = target as KomorkaUnity;
         m_fields = ExposeProperties.ExposeProperties.GetProperties(_komorkaUnity);
      }

      public void OnSceneGUI()
      {
         //Event e = Event.current;
         Handles.BeginGUI();
         var tlo = Resources.Load<Texture>("prototype_textures/Textures/tlo");
         GUI.BeginGroup(new Rect(0, 50, 180, 250), new GUIContent(tlo));
         {
            int przesuniecie = 0;
            foreach (var dana in WyswietlaneDane())
            {
               GUI.Label(new Rect(0, przesuniecie, 180, 45), String.Format("{0}: {1}", dana.Key, dana.Value));
               przesuniecie += 25;
            }
         }
         GUI.EndGroup();
         Handles.EndGUI();
      }

      private IEnumerable<KeyValuePair<string, string>> WyswietlaneDane()
      {
         if (KomorkaUnity.Komorka == null) 
            yield break;
         yield return new KeyValuePair<string, string>("Wysokoœæ", KomorkaUnity.Komorka.Punkt.Wysokosc.ToString());            
         yield return new KeyValuePair<string, string>("Pod³o¿e", KomorkaUnity.Komorka.Dane.Podloze.ToString());            
         yield return new KeyValuePair<string, string>("Typ", KomorkaUnity.Komorka.Dane.Typ.ToString());            
         yield return new KeyValuePair<string, string>("Brze¿noœæ", KomorkaUnity.Komorka.Dane.Brzeznosc.ToString());            
         yield return new KeyValuePair<string, string>("Temperatura", KomorkaUnity.Komorka.Dane.Temperatura.ToString());            
         yield return new KeyValuePair<string, string>("Wilgotnoœæ", KomorkaUnity.Komorka.Dane.Wilgotnosc.ToString());
      }

      public override void OnInspectorGUI()
      {
         if (_komorkaUnity == null)
            return;
         DrawDefaultInspector();
         ExposeProperties.ExposeProperties.Expose(m_fields);
      }
   }
}