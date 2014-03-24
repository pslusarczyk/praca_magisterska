namespace LogikaGeneracji
{
   public interface IKomorkaGeograficzna
   {
      IPunktTopologiczny PunktTopologiczny { get; set; }
      IKomorka Komorka { get; set; }
      TypKomorki Typ { get; set; }
   }
}