using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using LogikaGeneracji.PrzetwarzaczeMapy;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
   public interface IRog
   {
      DaneRogu Dane { get; set; }
      IPunkt Punkt { get; set; }
      IList<IKomorka> Komorki { get; set; }
      IList<IRog> BliskieRogi { get; set; }
      void DodajKomorki(IKomorka lewa, IKomorka prawa);
   }

   public class Rog : IRog
   {
      public Rog(Vector wektorFortunea)
      {
         Punkt = new Punkt {Pozycja = NarzedziaPrzetwarzaniaFortunea.VectorNaVector3(wektorFortunea)};
         Komorki = new List<IKomorka>();
         BliskieRogi = new List<IRog>();
      }

      public IPunkt NajnizszySasiad { get; set; }
      public DaneRogu Dane { get; set; }
      public IPunkt Punkt { get; set; }
      public IList<IKomorka> Komorki { get; set; }
      public IList<IRog> BliskieRogi { get; set; }

      public void DodajKomorki(IKomorka lewa, IKomorka prawa)
      {
         if (!Komorki.Contains(lewa))
         {
            Komorki.Add(lewa);
            Punkt.Sasiedzi.Add(lewa.Punkt);
         }
         if (!Komorki.Contains(prawa))
         {
            Komorki.Add(prawa);
            Punkt.Sasiedzi.Add(prawa.Punkt);
         }
      }
   }

   public class DaneRogu
   {
      public BrzeznoscRogu? Brzeznosc { get; set; }
   }
}