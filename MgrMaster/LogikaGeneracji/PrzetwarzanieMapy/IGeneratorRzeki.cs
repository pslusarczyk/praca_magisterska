namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public interface IGeneratorRzeki
   {
      IPunkt PunktPoczatkowy { get; set; }
      bool? UdaloSieUtworzyc { get; set; }
   }
}