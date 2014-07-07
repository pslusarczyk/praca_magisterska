using System.CodeDom;

namespace LogikaGeneracji
{
   public struct MojVector3
   {
      public float x;
      public float y;
      public float z;

      public MojVector3( float ax, float ay, float az)
      {
         x = ax;
         y = ay;
         z = az;
      }

      public MojVector3( double ax, double ay, double az)
      {
         x = (float)ax;
         y = (float)ay;
         z = (float)az;
      }
   }
}