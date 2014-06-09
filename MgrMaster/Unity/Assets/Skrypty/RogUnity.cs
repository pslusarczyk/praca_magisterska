using LogikaGeneracji;
using UnityEditor;
using UnityEngine;

namespace Assets.Skrypty
{
   public class RogUnity : MonoBehaviour
   {
      public IRog Rog { get; set; }

      public Material MaterialWysokosciZWoda { get; set; }

      void OnDrawGizmos()
      {
         if (Rog != null && Selection.activeGameObject && Vector3.Distance(Selection.activeGameObject.transform.position, transform.position) < 4f)
            Handles.Label(transform.position + Vector3.up * 1.2f, Rog.Punkt.Id.ToString(),
               new GUIStyle { normal = new GUIStyleState { textColor = Color.green } });

         //Gizmos.color = Color.yellow;
         if (Rog != null)
            foreach (IRog s in Rog.BliskieRogi)
            {
               //Gizmos.DrawLine(Rog.Punkt.Pozycja, s.Punkt.Pozycja);
            }

         //Handles.Label(transform.position, Rog.Id.ToString());
         
      }
   }
}