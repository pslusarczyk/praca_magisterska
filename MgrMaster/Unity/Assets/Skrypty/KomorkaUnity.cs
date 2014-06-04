using LogikaGeneracji;
using UnityEngine;

namespace Assets.Skrypty
{
   public class KomorkaUnity : MonoBehaviour
   {
      public IKomorka Komorka { get; set; }

      public Material MaterialWysokosciZWoda { get; set; }
      public Material MaterialZiemiWody { get; set; }
      public Material MaterialLaduMorzaJeziora { get; set; }

      public bool DoPowodzi { get; set; }

      public Vector3 PozycjaPunktu
      {
         get { return Komorka.Punkt.Pozycja; }
         set { Komorka.Punkt.Pozycja = value; }
      }

      public KomorkaUnity()
      {
         DoPowodzi = false;
      }

      public void OnDrawGizmos()
      {
         Gizmos.color = Color.yellow;
         if(DoPowodzi)
            Gizmos.DrawSphere(transform.position + Vector3.up*2, .6f);
      }
   }
}