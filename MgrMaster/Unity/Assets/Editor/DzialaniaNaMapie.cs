using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Skrypty;
using Assets.Skrypty.Generowanie;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieFortunea;
using LogikaGeneracji.PrzetwarzanieMapy;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;
using Object = UnityEngine.Object;

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

      private void UstawKomorkomIRogomUnityWyglad(float modyfikator = 0f) // pilne zduplikowany kod dla k. i r.
      {
         const float mnoznikWysokosci = 1.2f;
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            float wysokosc = komorkaUnity.Komorka.Punkt.Wysokosc + modyfikator;
            if (komorkaUnity.Komorka.Dane.Podloze != Podloze.Woda)
            {
               float wysokoscFizyczna = .01f + wysokosc*mnoznikWysokosci*2;
               komorkaUnity.Komorka.Punkt.Pozycja = new Vector3(komorkaUnity.Komorka.Punkt.Pozycja.x, wysokoscFizyczna - .8f, komorkaUnity.Komorka.Punkt.Pozycja.z);
               komorkaUnity.transform.localScale = new Vector3(1f, wysokoscFizyczna, 1f);
               komorkaUnity.transform.localPosition = new Vector3(komorkaUnity.transform.localPosition.x, wysokosc * mnoznikWysokosci,
                  komorkaUnity.transform.localPosition.z);
            }
            else
            {
               komorkaUnity.transform.localScale = new Vector3(1f, .01f, 1f);
               komorkaUnity.transform.localPosition = new Vector3(komorkaUnity.transform.localPosition.x, 0f,
                  komorkaUnity.transform.localPosition.z);
            }
            var kopiaMaterialu = new Material(komorkaUnity.renderer.sharedMaterial);
            if (Poziom._mapa.KomorkiNiecki.Contains(komorkaUnity.Komorka))
            {
               kopiaMaterialu.color = new Color(.9f, .4f, .6f);
            } // pilne poni¿sz¹ posprz¹taæ!
            else if (komorkaUnity.Komorka.Dane.Podloze == Podloze.Woda || (komorkaUnity.Komorka.Dane.Podloze == Podloze.Ziemia && komorkaUnity.Komorka.Dane.Typ == TypKomorki.Jezioro))
            {
               kopiaMaterialu.color = (komorkaUnity.Komorka.Dane.Typ == TypKomorki.Jezioro)
                                                   ? new Color(.35f, .6f, .98f) : new Color(.1f, .3f, .65f);
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
               rogUnity.Rog.Punkt.Pozycja = new Vector3(rogUnity.Rog.Punkt.Pozycja.x, wysokoscFizyczna-.8f, rogUnity.Rog.Punkt.Pozycja.z);
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
            if (komorkaUnity.Komorka.Dane.Podloze == Podloze.Woda)
            {
               kopiaMaterialu.color = new Color(.35f, .6f, .98f);
            }
            else
            {
               kopiaMaterialu.color = Konf.KolorBiomu[komorkaUnity.Komorka.Dane.Biom];
            }
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

      public void UstawKomorkomWidocznoscPolaInicjatorPowodzi(bool wartosc) // todo niezbyt eleganckpo zaprojektowane — to pok³osie problemów z dostêpem do dzia³añ na mapie komórki Unity
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

      public void UtworzJezioraWNieckach()
      {
         var generatorJezior = new GeneratorJezior(25);
         generatorJezior.Przetwarzaj(Poziom._mapa);
         var wyrownywacz = new WyrownywaczTerenuJeziora();
         wyrownywacz.Przetwarzaj(Poziom._mapa);
         UstawKomorkomIRogomUnityWyglad();
      }

      public void UtworzRzeki(System.Random random)
      {
         int utworzonych = 0;
         for (int i = 0; i < 20; ++ i)
         {
            var komorkiKandydaci = Poziom._mapa.Komorki.Where(k => k.Dane.Podloze == Podloze.Ziemia
               && k.Punkt.Wysokosc > .5f).ToList(); // todo parametr

            int indeksKomorki = random.Next(komorkiKandydaci.Count());
            IPunkt punktPoczatkowy = komorkiKandydaci.ElementAt(indeksKomorki).Punkt;
            var generatorRzek = new GeneratorRzeki(punktPoczatkowy);
            generatorRzek.Przetwarzaj(Poziom._mapa);

            if (generatorRzek.UdaloSieUtworzyc == false)
               Debug.Log("Nie uda³o siê utworzyæ dla punktu o identyfikatorze " + punktPoczatkowy.Id + ". ");
            if (generatorRzek.UdaloSieUtworzyc == true)
               ++utworzonych;
         }
         Debug.Log("Utworzono " + utworzonych + " rzek.");
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

      public void UtworzMapeTemperatury()
      {
         var modyfikator = new ModyfikatorTemperaturyNaPodstawieWysokosci();

         modyfikator.Przetwarzaj(Poziom._mapa);
         UstawKomorkomUnityMaterialTemperatury();
      }

      public void UtworzMapeBiomow(float normTemp, float normWilg) 
      // todo konfig ma byæ sta³y, a parametry normalizacji podawane z zewn¹trz
      {
         var konfig = Konf.KonfiguracjaBiomow;
         konfig._normalizacjaTemperatury = normTemp;
         konfig._normalizacjaWilgotnosci = normWilg;
         var aktualizator = new AktualizatorBiomow(konfig);
         aktualizator.Przetwarzaj(Poziom._mapa);
         UstawKomorkomUnityMaterialBiomow();
      }
   }
}