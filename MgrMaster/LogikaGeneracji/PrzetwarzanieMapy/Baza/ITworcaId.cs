namespace LogikaGeneracji.PrzetwarzanieMapy.Baza
{
   public interface ITworcaId
   {
      int UtworzId();
   }

   public class TworcaIdPunktow : ITworcaId
   {
      private static int _najwiekszyId = 0;

      public int UtworzId()
      {
         return _najwiekszyId++;
      }
   }

   public class TworcaIdRogow : ITworcaId
   {
      private static int _najwiekszyId = 0;

      public int UtworzId()
      {
         return _najwiekszyId++;
      }
   }

   public class TworcaIdKomorek : ITworcaId
   {
      private static int _najwiekszyId = 0;

      public int UtworzId()
      {
         return _najwiekszyId++;
      }
   }
}