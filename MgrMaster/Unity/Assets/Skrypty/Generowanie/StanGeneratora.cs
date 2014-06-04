using System.Collections.Generic;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji.PrzetwarzanieMapy;
using UnityEngine;

namespace Assets.Skrypty.Generowanie
{
   public class StanGeneratora
   {
      public IList<Warstwa> _utworzoneWarstwy;
      private bool _pokazRogiPoprzedniaWartosc = true;
      private float _zasiegZaburzenia = Konf.PoczStopienZaburzeniaWezlow;
      public int _rozmiarX = Konf.PoczRozmiarX;
      public int _rozmiarZ = Konf.PoczRozmiarZ;
      public float _rozpietosc = Konf.PoczRozpietosc;
      private float _poziomMorza = Konf.PoczPoziomMorza;
      private float _poprzedniPoziomMorza = Konf.PoczPoziomMorza;
      private Etap _etap = Etap.GenerowanieWezlow;
      private ParametryPerlina _parametryPerlina;

      public ParametryPerlina ParametryPerlina
      {
         get { return _parametryPerlina; }
         set { _parametryPerlina = value; }
      }

      public StanGeneratora()
      {
         PokazRogi = false;
         _utworzoneWarstwy = new List<Warstwa>();
         ParametryPerlina = new ParametryPerlina
         {
            GestoscPoczatkowa = .05f,
            IloscWarstw = 3,
            SkalaPoczatkowa = 4f,
            SkokGestosci = 3f,
            StrataSkali = .7f
         };
      }

      public bool PokazRogi { get; set; }

      public bool PokazRogiPoprzedniaWartosc
      {
         set { _pokazRogiPoprzedniaWartosc = value; }
         get { return _pokazRogiPoprzedniaWartosc; }
      }

      public float ZasiegZaburzenia
      {
         set { _zasiegZaburzenia = value; }
         get { return _zasiegZaburzenia; }
      }

      public float PoziomMorza
      {
         set { _poziomMorza = value; }
         get { return _poziomMorza; }
      }

      public float PoprzedniPoziomMorza
      {
         set { _poprzedniPoziomMorza = value; }
         get { return _poprzedniPoziomMorza; }
      }

      public int NumerWybranejWarstwy { get; set; }

      public Etap Etap
      {
         set { _etap = value; }
         get { return _etap; }
      }

      public IEnumerable<KomorkaUnity> InicjatorzyZalewania { get; set; }
   }
}
