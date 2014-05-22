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

      public Vector3 PozycjaPunktu
      {
         get { return Komorka.Punkt.Pozycja; }
         set { Komorka.Punkt.Pozycja = value; }
      }

      void OnDrawGizmos()
      {
         Gizmos.color = Color.magenta;
         if (Komorka != null)
            foreach (IKomorka s in Komorka.PrzylegleKomorki)
            {
               //Gizmos.DrawLine(Komorka.Punkt.Pozycja, s.Punkt.Pozycja);
            }
      }
   }
}