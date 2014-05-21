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
      private readonly PoziomEditor PoziomEditor;
      private Poziom _poziom;

      public Poziom Poziom { get { return _poziom ?? (_poziom = PoziomEditor.Poziom); } }

      public DzialaniaNaMapie(PoziomEditor poziomEditor)
      {
         PoziomEditor = poziomEditor;
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
            komorkaUnity.MaterialWysokosci = null;
         }
         foreach (RogUnity rogUnity in Poziom._rogiUnity)
         {
            rogUnity.MaterialWysokosci = null;
         }
         var modyfikator = new ModyfikatorWysokosciPerlinem { Nastepnik = new AktualizatorNastepstwaMapyWysokosci() };
         modyfikator.Przetwarzaj(Poziom._mapa);

         UstawKomorkomIRogomMaterialWysokosci();
         if (!PoziomEditor._utworzoneWarstwy.Contains(Warstwa.Wysokosci))
            PoziomEditor._utworzoneWarstwy.Add(Warstwa.Wysokosci);

         PoziomEditor.AktualnaWarstwa = Warstwa.Wysokosci;
      }

      public void RozdzielZiemieIWode()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.MaterialWysokosci = null;
         }
         IPrzetwarzaczMapy rozdzielacz = new RozdzielaczWodyIZiemi(1.9f);
         rozdzielacz.Przetwarzaj(Poziom._mapa);

         UstawKomorkomMaterialZiemiIWody();
         if (!PoziomEditor._utworzoneWarstwy.Contains(Warstwa.ZiemiaWoda))
            PoziomEditor._utworzoneWarstwy.Add(Warstwa.ZiemiaWoda);

         PoziomEditor.AktualnaWarstwa = Warstwa.ZiemiaWoda;
      }

      private void UstawKomorkomIRogomMaterialWysokosci()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            float wysokosc = komorkaUnity.Komorka.Punkt.Wysokosc;

            var kopiaMaterialu = new Material(komorkaUnity.renderer.sharedMaterial);
            kopiaMaterialu.color = new Color(.3f + wysokosc * .2f, .9f - wysokosc*.2f, .3f);
            komorkaUnity.MaterialWysokosci = kopiaMaterialu;
         }
         foreach (RogUnity rogUnity in Poziom._rogiUnity)
         {
            float wysokosc = rogUnity.Rog.Punkt.Wysokosc;

            var kopiaMaterialu = new Material(rogUnity.renderer.sharedMaterial);
            kopiaMaterialu.color = new Color(.3f + wysokosc * .2f, .9f - wysokosc*.2f, .3f);
            rogUnity.MaterialWysokosci = kopiaMaterialu;
         }
      }

      private void UstawKomorkomMaterialZiemiIWody()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            var kopiaMaterialu = new Material(komorkaUnity.renderer.sharedMaterial);
            if (komorkaUnity.Komorka.Dane.Podloze == Podloze.Woda)
               kopiaMaterialu.color = new Color(0f, .2f, .7f);
            else
               kopiaMaterialu.color = new Color(.6f, .5f, .1f);
            komorkaUnity.MaterialZiemiWody = kopiaMaterialu;
         }
      }

      public void PokazWarstweWysokosci()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.renderer.material = komorkaUnity.MaterialWysokosci; 
         }
         foreach (RogUnity rogUnity in Poziom._rogiUnity)
         {
            rogUnity.renderer.material = rogUnity.MaterialWysokosci; 
         }
         PoziomEditor.OdswiezZaznaczenieWarstwy();
      }

      public void PokazWarstweZiemiIWody()
      {
         foreach (KomorkaUnity komorkaUnity in Poziom._komorkiUnity)
         {
            komorkaUnity.renderer.material = komorkaUnity.MaterialZiemiWody;
         }
         PoziomEditor.OdswiezZaznaczenieWarstwy();
      }
   }
}