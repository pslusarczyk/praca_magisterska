using LogikaGeneracji;
using UnityEngine;

namespace Assets.Skrypty
{
   public class RogUnity : MonoBehaviour
   {
      public IRog Rog { get; set; }

      public Material MaterialWysokosci { get; set; }

      void OnDrawGizmos()
      {
         //Gizmos.color = Color.yellow;
         if (Rog != null)
            foreach (IRog s in Rog.BliskieRogi)
            {
               //Gizmos.DrawLine(Rog.Punkt.Pozycja, s.Punkt.Pozycja);
            }
      }
   }
}