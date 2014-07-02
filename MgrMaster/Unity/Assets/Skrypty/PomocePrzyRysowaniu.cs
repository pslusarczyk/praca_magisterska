using UnityEngine;

namespace Assets.Skrypty
{
   public class PomocePrzyRysowaniu
   {
      public static void GrubaLinia(Vector3 zrodlo, Vector3 cel, int grubosc = 5)
      {
         for (int i = 0; i < grubosc; ++i)
         {
            float przesuniecie = ((float) i)/30f;
            Gizmos.DrawLine(zrodlo + Vector3.left * przesuniecie, cel);
         }
      }
   }
}