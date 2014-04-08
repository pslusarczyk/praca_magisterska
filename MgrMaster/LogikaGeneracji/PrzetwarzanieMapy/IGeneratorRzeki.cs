using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public interface IGeneratorRzeki : IPrzetwarzaczMapy
   {
      IPunkt PunktPoczatkowy { get; set; }
      bool? UdaloSieUtworzyc { get; set; }
   }
}