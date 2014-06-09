using LogikaGeneracji;
using UnityEditor;
using UnityEngine;

namespace Assets.Skrypty
{
   public class KomorkaUnity : MonoBehaviour
   {
      public IKomorka Komorka { get; set; }

      public Material MaterialWysokosciZWoda { get; set; }
      public Material MaterialZiemiWody { get; set; }
      public Material MaterialLaduMorzaJeziora { get; set; }

      public bool InicjatorPowodzi { get; set; }
      public bool PoleInicjatorPowodziWidoczne { get; set; }

      public Vector3 PozycjaPunktu
      {
         get { return Komorka.Punkt.Pozycja; }
         set { Komorka.Punkt.Pozycja = value; }
      }

      public KomorkaUnity()
      {
         InicjatorPowodzi = false;
         PoleInicjatorPowodziWidoczne = false;
      }

      public void OnDrawGizmos()
      {
         if (Komorka != null && Selection.activeGameObject && Vector3.Distance(Selection.activeGameObject.transform.position, transform.position) < 4f)
            Handles.Label(transform.position + Vector3.up * 1.2f, Komorka.Id.ToString(),
               new GUIStyle{normal = new GUIStyleState{textColor = Color.green}});
          
         Gizmos.color = Color.magenta;
         if (PoleInicjatorPowodziWidoczne && InicjatorPowodzi)
            Gizmos.DrawWireSphere(transform.position, 1.8f);
      }

   }
}