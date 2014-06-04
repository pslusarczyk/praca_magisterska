using System;
using System.Collections.Generic;
using Assets.Editor.ExposeProperties;
using Assets.Skrypty;
using Assets.Skrypty.Generowanie;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
   [CustomEditor(typeof(KomorkaUnity))]
   public class KomorkaUnityEditor : UnityEditor.Editor
   {
      PropertyField[] m_fields;
      private KomorkaUnity _komorkaUnity;

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
         int przesuniecie = 0;
         GUI.BeginGroup(new Rect(0, 50, 180, 250), new GUIContent(tlo));
         {
            foreach (var dana in WyswietlaneDane())
            {
               GUILayout.Label(String.Format("{0}: {1}", dana.Key, dana.Value));
               przesuniecie += 25;
            }
            if (KomorkaUnity.PoleInicjatorPowodziWidoczne && KomorkaUnity.Komorka.Dane.Podloze == Podloze.Woda)
               KomorkaUnity.InicjatorPowodzi = GUILayout.Toggle(KomorkaUnity.InicjatorPowodzi, "Inicjator powodzi morza");
         }
         GUI.EndGroup();
         Handles.EndGUI();
      }

      private IEnumerable<KeyValuePair<string, string>> WyswietlaneDane()
      {
         if (KomorkaUnity.Komorka == null) 
            yield break;
         yield return new KeyValuePair<string, string>("Wysoko��", KomorkaUnity.Komorka.Punkt.Wysokosc.ToString());            
         yield return new KeyValuePair<string, string>("Pod�o�e", KomorkaUnity.Komorka.Dane.Podloze.ToString());            
         yield return new KeyValuePair<string, string>("Typ", KomorkaUnity.Komorka.Dane.Typ.ToString());            
         yield return new KeyValuePair<string, string>("Brze�no��", KomorkaUnity.Komorka.Dane.Brzeznosc.ToString());            
         yield return new KeyValuePair<string, string>("Temperatura", KomorkaUnity.Komorka.Dane.Temperatura.ToString());            
         yield return new KeyValuePair<string, string>("Wilgotno��", KomorkaUnity.Komorka.Dane.Wilgotnosc.ToString());
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