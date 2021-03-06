using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Skrypty;
using Assets.Skrypty.Generowanie;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieMapy;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Assets.Editor
{
   public class DzialaniaNaMapie
   {
      private readonly PoziomEditor _poziomEditor;
      private Poziom _poziom;

      public Poziom Poziom { get { return _poziom ?? (_poziom = _poziomEditor.Poziom); } }

      public DzialaniaNaMapie(PoziomEditor poziomEditor)
      {
         _poziomEditor = poziomEditor;
      }

      public void UkryjRogi()
      {
         foreach (RogUnity rog in Poziom._rogiUnity)
         {
            if (rog != null)
               rog.renderer.enabled = false;
         }
      }

      public void PokazRogi()
      {
         foreach (RogUnity rog in Poziom._rogiUnity)
         {
            rog.renderer.enabled = true;
         }
      }

      public void UsunWezlyRogiIKomorki()
      {
         if (Poziom.KomponentPojemnika != null)
            Object.DestroyImmediate(Poziom.KomponentPojemnika.gameObject);
         Poziom._wezly = null;
         Poziom._mapa = null;
         if (Poziom._komorkiUnity != null)
            Poziom._komorkiUnity.Clear();
         if (Poziom._rogiUnity != null)
            Poziom._rogiUnity.Clear();
         if (Poziom._krawedzieWoronoja != null)
            Poziom._krawedzieWoronoja.Clear();
      }

      public void GenerujWysokosci(ParametryPerlina parametryPerlina)
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.MaterialWysokosciZWoda = null;
            komorkaUnity.Komorka.Dane.Podloze = null;
         }
         foreach (RogUnity rogUnity in Poziom._rogiUnity)
         {
            rogUnity.MaterialWysokosciZWoda = null;
         }
         var modyfikator = new ModyfikatorWysokosciPerlinem(parametryPerlina);
         modyfikator.Przetwarzaj(Poziom._mapa);

         UstawKomorkomIRogomUnityWyglad();
      }

      public void RozdzielZiemieIWode(float prog)
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.MaterialZiemiWody = null;
         }
         var rozdzielacz = new RozdzielaczWodyIZiemi(prog);
         rozdzielacz.Przetwarzaj(Poziom._mapa);

         UstawKomorkomIRogomUnityWyglad(-prog);

      }

      public void ZatwierdzRozdzielenieZiemiIWody(float poziomMorza)
      {
         var liniowyModyfikatorWysokosci = new LiniowyModyfikatorWysokosci(-poziomMorza);
         liniowyModyfikatorWysokosci.Przetwarzaj(Poziom._mapa);

         var wyrownywaczWody = new WyrownywaczTerenuWody();
         wyrownywaczWody.Przetwarzaj(Poziom._mapa);
         
         UstawKomorkomIRogomUnityWyglad();
      }

      public void RozdzielMorzeIJeziora(IEnumerable<KomorkaUnity> inicjatorzyZalewania)
      {
         var rozdzielaczMorzIJezior = new RozdzielaczMorzIJezior(inicjatorzyZalewania.Select(i => i.Komorka));
         rozdzielaczMorzIJezior.Przetwarzaj(Poziom._mapa);
         UstawKomorkomIRogomUnityWyglad();
      }

      public void AktualizujBrzeznosci()
      {
         var aktualizatorBrzeznosciKomorek = new AktualizatorBrzeznosciKomorek();
         aktualizatorBrzeznosciKomorek.Przetwarzaj(Poziom._mapa);
         var aktualizatorBrzeznosciRogow = new AktualizatorBrzeznosciRogow();
         aktualizatorBrzeznosciRogow.Przetwarzaj(Poziom._mapa);
      }

      private void UstawKomorkomIRogomUnityWyglad(float modyfikator = 0f) // todo zduplikowany kod dla k. i r.
      {
         const float mnoznikWysokosci = 1.2f;
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            float wysokosc = komorkaUnity.Komorka.Punkt.Wysokosc + modyfikator;
            float wysokoscFizyczna = .01f + wysokosc * mnoznikWysokosci * 2;
            if (komorkaUnity.Komorka.Dane.Podloze != Podloze.Woda)
            {
               
               komorkaUnity.Komorka.Punkt.Pozycja = new MojVector3(komorkaUnity.Komorka.Punkt.Pozycja.x, wysokoscFizyczna - .8f, komorkaUnity.Komorka.Punkt.Pozycja.z);
               komorkaUnity.Komorka.Punkt.WysFiz = wysokoscFizyczna;
               komorkaUnity.transform.localScale = new Vector3(1f, wysokoscFizyczna, 1f);
               komorkaUnity.transform.localPosition = new Vector3(komorkaUnity.transform.localPosition.x, wysokosc * mnoznikWysokosci,
                  komorkaUnity.transform.localPosition.z);
            }
            else
            {
               komorkaUnity.Komorka.Punkt.Pozycja = new MojVector3(komorkaUnity.Komorka.Punkt.Pozycja.x, wysokoscFizyczna - .8f, komorkaUnity.Komorka.Punkt.Pozycja.z);
               komorkaUnity.transform.localScale = new Vector3(1f, .01f, 1f);
               komorkaUnity.transform.localPosition = new Vector3(komorkaUnity.transform.localPosition.x, 0f,
                  komorkaUnity.transform.localPosition.z);
            }

            var kopiaMaterialu = new Material(komorkaUnity.renderer.sharedMaterial);
            if (Poziom._mapa.KomorkiNiecki.Contains(komorkaUnity.Komorka))
            {
               kopiaMaterialu.color = new Color(.9f, .4f, .6f);
            } // pilne poni�sz� posprz�ta�!
            else if (komorkaUnity.Komorka.Dane.Typ == TypKomorki.Jezioro || komorkaUnity.Komorka.Dane.Podloze == null)
            {
               kopiaMaterialu.color = new Color(.35f, .6f, .98f);
            }
            else if (komorkaUnity.Komorka.Dane.Podloze == Podloze.Woda)
            {
               kopiaMaterialu.color = new Color(.1f, .3f, .65f);
            }
            else
               kopiaMaterialu.color = new Color(.3f + wysokosc * .3f, .9f - wysokosc*.2f, .3f);
            komorkaUnity.MaterialWysokosciZWoda = kopiaMaterialu;
         }

         foreach (RogUnity rogUnity in Poziom._rogiUnity)
         {
            float wysokosc = rogUnity.Rog.Punkt.Wysokosc + modyfikator;
            if (rogUnity.Rog.Dane.Brzeznosc != BrzeznoscRogu.OtwarteMorze)
            {
               float wysokoscFizyczna = .01f + wysokosc * mnoznikWysokosci * 2;
               rogUnity.Rog.Punkt.Pozycja = new MojVector3(rogUnity.Rog.Punkt.Pozycja.x, wysokoscFizyczna-.8f, rogUnity.Rog.Punkt.Pozycja.z);
               rogUnity.transform.localScale = new Vector3(rogUnity.transform.localScale.x, wysokoscFizyczna, rogUnity.transform.localScale.z);
               rogUnity.transform.localPosition = new Vector3(rogUnity.transform.localPosition.x, wysokosc * mnoznikWysokosci,
                  rogUnity.transform.localPosition.z);
            }
            else
            {
               rogUnity.transform.localScale = new Vector3(rogUnity.transform.localScale.x, .01f, rogUnity.transform.localScale.x);
               rogUnity.transform.localPosition = new Vector3(rogUnity.transform.localPosition.x, 0f,
                  rogUnity.transform.localPosition.z);
            }
            var kopiaMaterialu = new Material(rogUnity.renderer.sharedMaterial);
            if (rogUnity.Rog.Dane.Brzeznosc == BrzeznoscRogu.OtwarteMorze)
            {
               kopiaMaterialu.color = new Color(.35f, .6f, .98f); 
            }
            else
               kopiaMaterialu.color = new Color(.3f + wysokosc * .3f, .9f - wysokosc * .2f, .3f);
            rogUnity.MaterialWysokosciZWoda = kopiaMaterialu;
         }
      }

      private void UstawKomorkomUnityMaterialWilgotnosci() 
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            var kopiaMaterialu = new Material(komorkaUnity.renderer.sharedMaterial);
            if (komorkaUnity.Komorka.Dane.Podloze == Podloze.Woda)
            {
               kopiaMaterialu.color = new Color(.1f, .2f, .2f);
            }
            else
            {
               float wilgotnosc = komorkaUnity.Komorka.Dane.Wilgotnosc;
               kopiaMaterialu.color = new Color(.8f - wilgotnosc*.3f, .1f + wilgotnosc*.08f, .1f + wilgotnosc*.15f);
            }
            komorkaUnity.MaterialWilgotnosci = kopiaMaterialu;
         }
      }

      private void UstawKomorkomUnityMaterialTemperatury() 
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            var kopiaMaterialu = new Material(komorkaUnity.renderer.sharedMaterial);
            if (komorkaUnity.Komorka.Dane.Podloze == Podloze.Woda)
            {
               kopiaMaterialu.color = new Color(.1f, .2f, .2f);
            }
            else
            {
               float temperatura = komorkaUnity.Komorka.Dane.Temperatura;
               float czerwony = .15f + temperatura*.01f + (float)Math.Pow(temperatura, 2)*0.001f;
               float zielony = 1.1f - Math.Abs(temperatura-20f)*0.06f; 
               float niebieski = .9f-temperatura*0.04f;
               kopiaMaterialu.color = new Color(czerwony, zielony, niebieski);
            }
           komorkaUnity.MaterialTemperatury = kopiaMaterialu;
         }
      }

      private void UstawKomorkomUnityMaterialBiomow() 
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            var kopiaMaterialu = new Material(komorkaUnity.renderer.sharedMaterial);
            if (komorkaUnity.Komorka.Dane.Typ == TypKomorki.Morze)
               kopiaMaterialu.color = new Color(.1f, .3f, .65f);
            else if (komorkaUnity.Komorka.Dane.Typ == TypKomorki.Jezioro)
               kopiaMaterialu.color = new Color(.35f, .6f, .98f);
            else if (komorkaUnity.Komorka.Dane.Biom.HasValue)
               kopiaMaterialu.color = Konf.KolorBiomu[komorkaUnity.Komorka.Dane.Biom.Value];
            komorkaUnity.MaterialBiomu = kopiaMaterialu;
         }
      }

      public void PokazWarstweWysokosciIWody()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.renderer.material = komorkaUnity.MaterialWysokosciZWoda; 
         }
         foreach (RogUnity rogUnity in Poziom._rogiUnity)
         {
            rogUnity.renderer.material = rogUnity.MaterialWysokosciZWoda; 
         }
      }

      public void PokazWarstweWilgotnosci()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.renderer.material = komorkaUnity.MaterialWilgotnosci; 
         }
      }

      public void PokazWarstweTemperatury()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.renderer.material = komorkaUnity.MaterialTemperatury; 
         }
      }

      public void PokazWarstweBiomow()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.renderer.material = komorkaUnity.MaterialBiomu; 
         }
      }

      public void UstawKomorkomWidocznoscPolaInicjatorPowodzi(bool wartosc) // todo niezbyt eleganckpo zaprojektowane � to pok�osie problem�w z dost�pem do dzia�a� na mapie kom�rki Unity
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.PoleInicjatorPowodziWidoczne = wartosc;
         }
      }

      public void UstawPunktomNastepstwaMapyWysokosci()
      {
         var aktualizator = new AktualizatorNastepstwaMapyWysokosci();
         aktualizator.Przetwarzaj(Poziom._mapa);
      }

      public void WyznaczKomorkiNiecki()
      {
         var wydzielacz = new WydzielaczKomorekNiecek();
         wydzielacz.Przetwarzaj(Poziom._mapa);
         UstawKomorkomIRogomUnityWyglad();
      }

      public void UtworzJezioraWNieckach(int ile)
      {
         var generatorJezior = new GeneratorJezior(ile);
         generatorJezior.Przetwarzaj(Poziom._mapa);
         var wyrownywacz = new WyrownywaczTerenuJeziora();
         wyrownywacz.Przetwarzaj(Poziom._mapa);
         UstawKomorkomIRogomUnityWyglad();
      }

      public void UtworzRzeki(int ile, Random gen)
      {
         int wykonanychProb = 0;
         for (int utworzonych = 0; utworzonych < ile; ++wykonanychProb)
         {
            if (wykonanychProb > ile*Konf.LimitProbUtworzeniaSrednioJednejRzeki)
            {
               Debug.LogWarning("Przekroczono dopuszczaln� ilo�� pr�b wygenerowania rzeki.");
               return;
            }
            var komorkiKandydaci = Poziom._mapa.Komorki.Where(k => k.Dane.Podloze == Podloze.Ziemia
               && k.Punkt.Wysokosc > Konf.MinimalnaWysokoscZrodlaRzeki).ToList(); 

            int indeksKomorki = gen.Next(komorkiKandydaci.Count());
            IPunkt punktPoczatkowy = komorkiKandydaci.ElementAt(indeksKomorki).Punkt;
            var generatorRzek = new GeneratorRzeki(punktPoczatkowy);
            generatorRzek.Przetwarzaj(Poziom._mapa);
            if (generatorRzek.UdaloSieUtworzyc == true)
            {
               ++utworzonych;
            }
         }
      }

      public void UtworzMapeWilgotnosci(ParametryWilgotnosci parametry)
      {
         var aktualizator = new AktualizatorWilgotnosci
         {
            GlebokoscPrzeszukiwania = parametry.GlebokoscPrzeszukiwania,
            WartoscJeziora = parametry.WartoscJeziora,
            WartoscRzeki = parametry.WartoscRzeki,
            WartoscMorza = parametry.WartoscMorza,
            MnoznikWartosci = Konf.Wilg.MnoznikWartosci
         };

         aktualizator.Przetwarzaj(Poziom._mapa);
         UstawKomorkomUnityMaterialWilgotnosci();
      }

      public void UtworzMapeTemperatury(float mnoznikTemperatury)
      {
         var modyfikator = new ModyfikatorTemperaturyNaPodstawieWysokosci(mnoznikTemperatury);

         modyfikator.Przetwarzaj(Poziom._mapa);
         UstawKomorkomUnityMaterialTemperatury();
      }

      public void UtworzMapeBiomow(KonfigAktualizatoraBiomow konfiguracjaBiomow) 
      {
         var aktualizator = new AktualizatorBiomow(konfiguracjaBiomow, -5f, 35f, 0f, 16f);
         aktualizator.Przetwarzaj(Poziom._mapa);
         UstawKomorkomUnityMaterialBiomow();
      }
   }
}