using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Editor.ExposeProperties;
using Assets.Skrypty;
using Assets.Skrypty.Generowanie;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
   [CustomEditor(typeof(RogUnity))]
   public class RogUnityEditor : UnityEditor.Editor
   {
      PropertyField[] m_fields;
      private RogUnity _rogUnity;

      public RogUnity RogUnity
      {
         get { return _rogUnity ?? (_rogUnity = (RogUnity)target); }
         set { _rogUnity = value; }
      }

      public void OnEnable()
      {
         _rogUnity = target as RogUnity;
         m_fields = ExposeProperties.ExposeProperties.GetProperties(_rogUnity);
      }

      public void OnSceneGUI()
      {
         //Event e = Event.current;
         Handles.BeginGUI();
         var tlo = Resources.Load<Texture>("prototype_textures/Textures/tlo");
         int przesuniecie = 0;
         GUI.BeginGroup(new Rect(0, 50, 180, 300), new GUIContent(tlo));
         {
            foreach (var dana in WyswietlaneDane())
            {
               GUILayout.Label(String.Format("{0}: {1}", dana.Key, dana.Value));
               przesuniecie += 25;
            }
         }
         GUI.EndGroup();
         Handles.EndGUI();
      }

      private IEnumerable<KeyValuePair<string, string>> WyswietlaneDane()
      {
         if (RogUnity.Rog == null) 
            yield break;
         yield return new KeyValuePair<string, string>("Identyfikator", RogUnity.Rog.Punkt.Id.ToString());
         string idBliskich = String.Join(", ", RogUnity.Rog.BliskieRogi.Select(b => b.Punkt.Id.ToString()).ToArray());
         string idKomorek = String.Join(", ", RogUnity.Rog.Komorki.Select(k => k.Punkt.Id.ToString()).ToArray());
         yield return new KeyValuePair<string, string>("Bliskie", idBliskich);
         yield return new KeyValuePair<string, string>("Komórki", idKomorek);
         yield return new KeyValuePair<string, string>("Nastêpnik", RogUnity.Rog.Punkt.Nastepnik != null ? RogUnity.Rog.Punkt.Nastepnik.Id.ToString() : "—");
         yield return new KeyValuePair<string, string>("Wysokoœæ", RogUnity.Rog.Punkt.Wysokosc.ToString());
         yield return new KeyValuePair<string, string>("Brze¿noœæ", RogUnity.Rog.Dane.Brzeznosc.ToString());
      }

      public override void OnInspectorGUI()
      {
         if (_rogUnity == null)
            return;
         DrawDefaultInspector();
         ExposeProperties.ExposeProperties.Expose(m_fields);
      }
   }
}