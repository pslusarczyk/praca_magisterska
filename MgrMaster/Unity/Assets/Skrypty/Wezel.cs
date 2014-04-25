using System.Collections.Generic;
using UnityEngine;

namespace Assets.Skrypty
{
   public class Wezel : MonoBehaviour
   {
      public bool czySkrajny = false;
      public Vector3 pierwotnaPozycja;

      public void OnDrawGizmos()
      {
         Gizmos.color = Color.magenta;
         //if (_pokazSciany)
         //foreach (var s in _scianyKomorki)
         //{
         //   Gizmos.DrawLine(s.pierwszy, s.drugi);
         //   Gizmos.color = Gizmos.color*.75f;
         //}
      }
   }
}