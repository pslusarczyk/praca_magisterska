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
   /// <summary>
   /// U�ywanie zasob�w: Resources.Load<Material>("prototype_textures/Materials/proto_blue 1");
   /// </summary>
   public class DzialaniaNaMapie
   {
      private readonly PoziomEditor _poziomEditor;

      public Poziom Poziom {
         get { return _poziomEditor.Poziom; }
         set { _poziomEditor.Poziom = value; } 
      }

      public DzialaniaNaMapie(PoziomEditor poziomEditor)
      {
         _poziomEditor = poziomEditor;
      }

      public void UkryjRogi()
      {
         foreach (RogUnity rog in _poziomEditor.Poziom._rogiUnity)
         {
            if (rog != null)
               rog.renderer.enabled = false;
         }
      }

      public void PokazRogi()
      {
         foreach (RogUnity rog in _poziomEditor.Poziom._rogiUnity)
         {
            rog.renderer.enabled = true;
         }
      }

      public void UsunWezlyRogiIKomorki()
      {
         if (_poziomEditor.Poziom.KomponentPojemnika != null)
            Object.DestroyImmediate(_poziomEditor.Poziom.KomponentPojemnika.gameObject);
         _poziomEditor.Poziom._wezly = null;
         _poziomEditor.Poziom._mapa = null;
         if (_poziomEditor.Poziom._komorkiUnity != null)
            _poziomEditor.Poziom._komorkiUnity.Clear();
         if (_poziomEditor.Poziom._rogiUnity != null)
            _poziomEditor.Poziom._rogiUnity.Clear();
         if (_poziomEditor.Poziom._krawedzieWoronoja != null)
            _poziomEditor.Poziom._krawedzieWoronoja.Clear();
      }

      public void GenerujWysokosci()
      {
         foreach (KomorkaUnity komorkaUnity in _poziomEditor.Poziom._komorkiUnity)
         {
            komorkaUnity.MaterialWysokosci = null;
         }
         var modyfikator = new ModyfikatorWysokosciPerlinem { Nastepnik = new AktualizatorNastepstwaMapyWysokosci() };
         modyfikator.Przetwarzaj(_poziomEditor.Poziom._mapa);

         UstawKomorkomMaterialWysokosci();
      }

      public void RozdzielZiemieIWode()
      {
         foreach (KomorkaUnity komorkaUnity in _poziomEditor.Poziom._komorkiUnity)
         {
            komorkaUnity.MaterialWysokosci = null;
         }
         IPrzetwarzaczMapy rozdzielacz = new RozdzielaczWodyIZiemi(1.9f);
         rozdzielacz.Przetwarzaj(_poziomEditor.Poziom._mapa);

         UstawKomorkomMaterialZiemiIWody();
      }

      private void UstawKomorkomMaterialWysokosci()
      {
         foreach (KomorkaUnity komorkaUnity in _poziomEditor.Poziom._komorkiUnity)
         {
            float wysokosc = komorkaUnity.Komorka.Punkt.Wysokosc;

            var kopiaMaterialu = new Material(komorkaUnity.renderer.sharedMaterial);
            kopiaMaterialu.color = new Color(.3f + wysokosc * .2f, .9f - wysokosc*.2f, .3f);
            komorkaUnity.MaterialWysokosci = kopiaMaterialu;
         }
      }

      private void UstawKomorkomMaterialZiemiIWody()
      {
         foreach (KomorkaUnity komorkaUnity in _poziomEditor.Poziom._komorkiUnity)
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
         foreach (KomorkaUnity komorkaUnity in _poziomEditor.Poziom._komorkiUnity)
         {
            komorkaUnity.renderer.material = komorkaUnity.MaterialWysokosci; 
         }
      }

      public void PokazWarstweZiemiIWody()
      {
         foreach (KomorkaUnity komorkaUnity in _poziomEditor.Poziom._komorkiUnity)
         {
            komorkaUnity.renderer.material = komorkaUnity.MaterialZiemiWody;
         }
      }

      public void ObsluzZmianyWeWlasciwosciach()
      {
         if (_poziomEditor.Poziom.warstwa != _poziomEditor.OstatniaWarstwa)
         {
            _poziomEditor.OstatniaWarstwa = _poziomEditor.Poziom.warstwa;
            if (_poziomEditor.Poziom.warstwa == Warstwa.Wysokosci)
               PokazWarstweWysokosci();
            if (_poziomEditor.Poziom.warstwa == Warstwa.ZiemiaWoda)
               PokazWarstweZiemiIWody();
         }
      }
   }
}