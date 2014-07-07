using UnityEngine;

namespace LogikaGeneracji
{
   public static class RozszerzeniaMojegoVectora3
   {
      public static Vector3 NaVector3(this MojVector3 moj)
      {
         return new Vector3(moj.x, moj.y, moj.z);
      }
   }
}