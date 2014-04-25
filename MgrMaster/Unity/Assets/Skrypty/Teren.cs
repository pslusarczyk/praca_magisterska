using UnityEngine;

namespace Assets.Skrypty
{
   public class Teren : MonoBehaviour
   {
      public void Update()
      {
         transform.Translate(.001f, 0f, 0f);
      }
   }
}