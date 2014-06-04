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
         Gizmos.color = Color.magenta;
         if (PoleInicjatorPowodziWidoczne && InicjatorPowodzi)
            Gizmos.DrawWireSphere(transform.position, 1.8f);
      }
   }
}