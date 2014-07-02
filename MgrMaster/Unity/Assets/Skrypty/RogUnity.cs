using LogikaGeneracji;
using UnityEditor;
using UnityEngine;

namespace Assets.Skrypty
{
   public class RogUnity : MonoBehaviour
   {
      private bool WyswietlajIdentyfikatoryWPoblizu = false;
      public IRog Rog { get; set; }

      public Material MaterialWysokosciZWoda { get; set; }

      void OnDrawGizmos()
      {
         if (WyswietlajIdentyfikatoryWPoblizu)
         if (Rog != null && Selection.activeGameObject && Vector3.Distance(Selection.activeGameObject.transform.position, transform.position) < 4f)
            Handles.Label(transform.position + Vector3.up * 1.2f, Rog.Punkt.Id.ToString(),
               new GUIStyle { normal = new GUIStyleState { textColor = Color.green } });

         Gizmos.color = Color.white;
         if (Rog!= null && Rog.Punkt.ZawieraRzeke && Rog.Punkt.Nastepnik != null)
            PomocePrzyRysowaniu.GrubaLinia(Rog.Punkt.Pozycja + Vector3.up, Rog.Punkt.Nastepnik.Pozycja + Vector3.up);
      

      }
   }
}