using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieMapy;
using UnityEngine;

namespace Assets.Skrypty.Generowanie
{
   [Serializable]
   public class StanGeneratora
   {
      public IList<Warstwa> UtworzoneWarstwy;
      [SerializeField]
      private bool _pokazRogiPoprzedniaWartosc = true;
      [SerializeField]
      private float _zasiegZaburzenia = Konf.PoczStopienZaburzeniaWezlow;
      public int RozmiarX = Konf.PoczRozmiarX;
      public int RozmiarZ = Konf.PoczRozmiarZ;
      public float Rozpietosc = Konf.PoczRozpietosc;
      private float _poziomMorza = Konf.PoczPoziomMorza;
      [SerializeField]
      private Etap _etap = Etap.GenerowanieWezlow;
      public float MnoznikTemperatury = Konf.PoczMnoznikTemperatury;
      public int LiczbaJeziorDoWygenerowania;
      public int LiczbaRzekDoWygenerowania;
      [SerializeField]
      private int _numerWybranejWarstwy;

      [SerializeField]
      private ParametryPerlina _parametryPerlina;
      [SerializeField]
      private ParametryWilgotnosci _parametryWilgotnosci;
      [SerializeField]
      private KonfigAktualizatoraBiomow _konfiguracjaBiomow;

      public ParametryPerlina ParametryPerlina
      {
         get { return _parametryPerlina; }
         set { _parametryPerlina = value; }
      }

      public ParametryWilgotnosci ParametryWilgotnosci
      {
         get { return _parametryWilgotnosci; }
         set { _parametryWilgotnosci = value; }
      }

      public KonfigAktualizatoraBiomow KonfiguracjaBiomow
      {
         get { return _konfiguracjaBiomow; }
         set { _konfiguracjaBiomow = value; }
      }

      public StanGeneratora()
      {
         PokazRogi = false;
         UtworzoneWarstwy = new List<Warstwa>();
         ParametryPerlina = new ParametryPerlina
         {
            Ziarno = 0,
            Gestosc = Konf.Perlin.PoczGestosc,
            IloscWarstw = Konf.Perlin.PoczIloscWarstw,
            Skala = Konf.Perlin.PoczSkala,
            SkokGestosci = Konf.Perlin.PoczSkokGestosci,
            ZachowanieSkali = Konf.Perlin.PoczZachowanieSkali
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
         InicjatorzyZalewania = new List<KomorkaUnity>();
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

      public int NumerWybranejWarstwy
      {
         get { return _numerWybranejWarstwy; }
         set { _numerWybranejWarstwy = value; }
      }

      public Etap Etap
      {
         set { _etap = value; }
         get { return _etap; }
      }

      public IEnumerable<KomorkaUnity> InicjatorzyZalewania { get; set; }

   }
}
