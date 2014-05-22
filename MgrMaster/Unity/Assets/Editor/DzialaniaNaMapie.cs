using System.Collections.Generic;
using System.Linq;
using Assets.Skrypty;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieFortunea;
using LogikaGeneracji.PrzetwarzanieMapy;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

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

      public void GenerujWysokosci()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.MaterialWysokosciZWoda = null;
         }
         foreach (RogUnity rogUnity in Poziom._rogiUnity)
         {
            rogUnity.MaterialWysokosciZWoda = null;
         }
         var modyfikator = new ModyfikatorWysokosciPerlinem { Nastepnik = new AktualizatorNastepstwaMapyWysokosci() };
         modyfikator.Przetwarzaj(Poziom._mapa);

         UstawKomorkomIRogomMaterialWysokosciIWody();
      }

      public void RozdzielZiemieIWode(float prog)
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.MaterialZiemiWody = null;
         }
         var rozdzielacz = new RozdzielaczWodyIZiemi(prog);
         rozdzielacz.Przetwarzaj(Poziom._mapa);

         UstawKomorkomIRogomMaterialWysokosciIWody(-prog);

      }

      public void ZatwierdzRozdzielenieZiemiIWody(float poziomMorza)
      {
         var liniowyModyfikatorWysokosci = new LiniowyModyfikatorWysokosci(-poziomMorza);
         liniowyModyfikatorWysokosci.Przetwarzaj(Poziom._mapa);

         var wyrownywaczWody = new WyrownywaczTerenuWody();
         wyrownywaczWody.Przetwarzaj(Poziom._mapa);
         
         UstawKomorkomIRogomMaterialWysokosciIWody();
      }

      private void UstawKomorkomIRogomMaterialWysokosciIWody(float modyfikator = 0f)
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            float wysokosc = komorkaUnity.Komorka.Punkt.Wysokosc + modyfikator;

            var kopiaMaterialu = new Material(komorkaUnity.renderer.sharedMaterial);
            if(komorkaUnity.Komorka.Dane.Podloze == Podloze.Woda)
            {
               kopiaMaterialu.color = (komorkaUnity.Komorka.Dane.Typ == TypKomorki.Morze) 
                                                   ? new Color(0f, .4f, .75f) : new Color(.2f, .3f, .95f);
            }
            else
               kopiaMaterialu.color = new Color(.3f + wysokosc * .2f, .9f - wysokosc*.2f, .3f);
            komorkaUnity.MaterialWysokosciZWoda = kopiaMaterialu;
         }
         foreach (RogUnity rogUnity in Poziom._rogiUnity)
         {
            float wysokosc = rogUnity.Rog.Punkt.Wysokosc;

            var kopiaMaterialu = new Material(rogUnity.renderer.sharedMaterial);
            kopiaMaterialu.color = new Color(.3f + wysokosc * .2f, .9f - wysokosc*.2f, .3f);
            rogUnity.MaterialWysokosciZWoda = kopiaMaterialu;
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

      public void RozdzielMorzeIJeziora(KomorkaUnity inicjatorZalewania)
      {
         var rozdzielaczMorzIJezior = new RozdzielaczMorzIJezior(inicjatorZalewania.Komorka);
         rozdzielaczMorzIJezior.Przetwarzaj(Poziom._mapa);

         var aktualizatorBrzeznosciKomorek = new AktualizatorBrzeznosciKomorek();
         aktualizatorBrzeznosciKomorek.Przetwarzaj(Poziom._mapa);
         var aktualizatorBrzeznosciRogow = new AktualizatorBrzeznosciKomorek();
         aktualizatorBrzeznosciRogow.Przetwarzaj(Poziom._mapa);

         UstawKomorkomIRogomMaterialWysokosciIWody();
      }
   }
}