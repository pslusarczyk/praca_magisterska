using LogikaGeneracji;
using UnityEditor;
using UnityEngine;

namespace Assets.Skrypty
{
   public class KomorkaUnity : MonoBehaviour
   {
      private readonly PomocePrzyRysowaniu _pomocePrzyRysowaniu;
      private bool WyswietlajIdentyfikatoryWPoblizu = false;
      public IKomorka Komorka { get; set; }

      public Material MaterialWysokosciZWoda { get; set; }
      public Material MaterialZiemiWody { get; set; }
      public Material MaterialLaduMorzaJeziora { get; set; }
      public Material MaterialWilgotnosci { get; set; }
      public Material MaterialTemperatury { get; set; }
      public Material MaterialBiomu { get; set; }

      public bool InicjatorPowodzi { get; set; }
      public bool PoleInicjatorPowodziWidoczne { get; set; }

      public Vector3 PozycjaPunktu
      {
         get { return Komorka.Punkt.Pozycja.NaVector3(); }
         set { Komorka.Punkt.Pozycja = new MojVector3(value.x,value.y,value.z); }
      }

      public KomorkaUnity()
      {
         InicjatorPowodzi = false;
         PoleInicjatorPowodziWidoczne = false;
      }

      public void OnDrawGizmos()
      {
         if(WyswietlajIdentyfikatoryWPoblizu)
         if (Komorka != null && Selection.activeGameObject && Vector3.Distance(Selection.activeGameObject.transform.position, transform.position) < 4f)
            Handles.Label(transform.position + Vector3.up * 1.2f, Komorka.Punkt.Id.ToString(),
               new GUIStyle{normal = new GUIStyleState{textColor = Color.green}});
          
         Gizmos.color = Color.magenta;
         if (PoleInicjatorPowodziWidoczne && InicjatorPowodzi)
            Gizmos.DrawWireSphere(transform.position, 1.8f);

         Gizmos.color = Color.white;
         if (Komorka != null && Komorka.Punkt.ZawieraRzeke && Komorka.Punkt.Nastepnik != null)
            PomocePrzyRysowaniu.GrubaLinia(Komorka.Punkt.Pozycja.NaVector3() + Vector3.up, Komorka.Punkt.Nastepnik.Pozycja.NaVector3() + Vector3.up);
      }
   }
}