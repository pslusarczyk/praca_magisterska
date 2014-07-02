using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji;
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
      private Etap _etap = Etap.GenerowanieWezlow;
      public float MnoznikTemperatury = Konf.PoczMnoznikTemperatury;

      public ParametryPerlina ParametryPerlina { get; set; }
      public ParametryWilgotnosci ParametryWilgotnosci { get; set; }
      public KonfigAktualizatoraBiomow KonfiguracjaBiomow { get; set; }

      public StanGeneratora()
      {
         PokazRogi = false;
         _utworzoneWarstwy = new List<Warstwa>();
         ParametryPerlina = new ParametryPerlina
         {
            Ziarno = 0,
            Gestosc = Konf.Perlin.PoczGestosc,
            IloscWarstw = Konf.Perlin.PoczIloscWarstw,
            Skala = Konf.Perlin.PoczSkala,
            SkokGestosci = Konf.Perlin.PoczSkokGestosci,
            StrataSkali = Konf.Perlin.PoczStrataSkali
         };
         ParametryWilgotnosci = new ParametryWilgotnosci
         {
            GlebokoscPrzeszukiwania = Konf.Wilg.PoczGlebokoscPrzeszukiwania,
            WartoscJeziora = Konf.Wilg.PoczWartoscJeziora,
            WartoscRzeki = Konf.Wilg.PoczWartoscRzeki,
            WartoscMorza = Konf.Wilg.PoczWartoscMorza
         };
         KonfiguracjaBiomow = new KonfigAktualizatoraBiomow(Konf.KonfiguracjaBiomow.ParametryBiomow.
            Select(p => new KonfiguracjaBiomu(p.Wilgotnosc, p.Temperatura, p.Biom)).ToList());
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

      public int NumerWybranejWarstwy { get; set; }

      public Etap Etap
      {
         set { _etap = value; }
         get { return _etap; }
      }

      public IEnumerable<KomorkaUnity> InicjatorzyZalewania { get; set; }
   }
}
