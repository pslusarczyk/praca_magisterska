using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Assets.Editor.ExposeProperties;
using Assets.Skrypty;
using Assets.Skrypty.Generowanie;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieMapy;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Assets.Editor
{
   [CustomEditor(typeof(Poziom))]
   public class PoziomEditor : UnityEditor.Editor
   {
      PropertyField[] m_fields;
      [SerializeField]
      private Poziom _poziom;
      [SerializeField]
      private Warstwa _poprzedniaWarstwa;
      
      public bool PokazSciany
      {
         get { return Poziom._pokazSciany; }
         set
         {  SceneView.RepaintAll(); Poziom._pokazSciany = value; }
      }

      public Warstwa AktualnaWarstwa
      {
         get { return _poziom.AktualnaWarstwa; }
         set { _poziom.AktualnaWarstwa = value; }
      }

      [SerializeField]
      private readonly DzialaniaNaMapie _dzialaniaNaMapie;

      [SerializeField]
      private readonly DzialaniaNaWezlach _dzialaniaNaWezlach;

      private float _poprzedniPoziomMorza = Konf.PoczPoziomMorza;
      [SerializeField]
      private int _ziarnoGenerowaniaRzek;

      [ExposeProperty]
      public string EtapTekst { get { return StanGeneratora.Etap.ToString(); } set {} } // musi byæ set ¿eby siê wyœwietla³o

      public Poziom Poziom
      {
         get { return _poziom ?? (_poziom = (Poziom)target); }
         set { _poziom = value; }
      }

      public Warstwa PoprzedniaWarstwa
      {
         set { _poprzedniaWarstwa = value; }
         get { return _poprzedniaWarstwa; }
      }

      public StanGeneratora StanGeneratora
      {
         get { return Poziom.StanGeneratora; }
      }

      public PoziomEditor()
      {
         _dzialaniaNaMapie = new DzialaniaNaMapie(this);
         _dzialaniaNaWezlach = new DzialaniaNaWezlach(this);
      }

      public int LiczbaMiejscNaJeziora {
         get { return Poziom._mapa.KomorkiNiecki.Count; }
      }

      public void OnEnable()
      {
         _poziom = target as Poziom;
         _poprzedniaWarstwa = Warstwa.Brak;
         AktualnaWarstwa = _poprzedniaWarstwa;
         m_fields = ExposeProperties.ExposeProperties.GetProperties(_poziom);
      }

      public int LiczbaJeziorDoWygenerowania;

      public override void OnInspectorGUI()
      {
         if (_poziom == null)
            return;
         DrawDefaultInspector();
         ExposeProperties.ExposeProperties.Expose(m_fields);

         EditorGUILayout.LabelField("Generator poziomu", Konf.StylNaglowkaInspektora);

         SekcjaResetowania();
         EditorGUILayout.LabelField("Etap: " + StanGeneratora.Etap, Konf.StylNaglowkaInspektora);
         EditorGUILayout.Space();

         // pilne usuñ jak bêdzie niepotrzebne LiczbaJeziorDoWygenerowania = EditorGUILayout.IntField("Jezior do wygenerowania", LiczbaJeziorDoWygenerowania);
         /////
         if (StanGeneratora.Etap == Etap.GenerowanieWezlow)
            SekcjaGenerowaniaWezlow();
         if ((StanGeneratora.Etap == Etap.ZaburzanieWezlow || StanGeneratora.Etap == Etap.TworzenieKomorekIRogow))
            SekcjaZaburzaniaITworzeniaKomorekIRogow();
         if (StanGeneratora.Etap >= Etap.TworzenieMapyWysokosci)
            SekcjaPokazywaniaIUkrywaniaScianIrogow();
         if (StanGeneratora.Etap == Etap.TworzenieMapyWysokosci || StanGeneratora.Etap == Etap.RozdzielanieZiemiIWody)
            SekcjaGenerowaniaMapyWysokosci();
         if (StanGeneratora.Etap == Etap.RozdzielanieZiemiIWody)
            SekcjaPoziomuMorza();
         if (StanGeneratora.Etap == Etap.WydzielanieMorza)
            SekcjaWydzielaniaMorza();
         if (StanGeneratora.Etap == Etap.TworzenieJezior)
            SekcjaTworzeniaJezior();
         if (StanGeneratora.Etap == Etap.TworzenieRzek)
            SekcjaTworzeniaRzek();
         if (StanGeneratora.Etap == Etap.WyznaczanieWilgotnosci)
            SekcjaWyznaczaniaWilgotnosci();
         if (StanGeneratora.Etap == Etap.WyznaczanieTemperatury)
            SekcjaWyznaczaniaTemperatury();
         if (StanGeneratora.Etap == Etap.WyznaczanieBiomow || (AktualnaWarstwa == Warstwa.Biomy))
            SekcjaWyznaczaniaBiomow();
      }
      
      private void SekcjaResetowania()
      {
         GUI.color = new Color(.9f, .1f, .1f);
         if (GUILayout.Button("Resetuj"))
         {
            StanGeneratora.PokazRogi = false;
            StanGeneratora.PokazRogiPoprzedniaWartosc = true;
            PokazSciany = false;
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            StanGeneratora.Etap = Etap.GenerowanieWezlow;
            StanGeneratora.UtworzoneWarstwy.Clear();
            StanGeneratora.NumerWybranejWarstwy = 0;
         }
         GUI.color = Color.white;
      }

      private void SekcjaGenerowaniaWezlow()
      {
         EditorGUILayout.LabelField("Okreœl wymiary poziomu:", Konf.StylNaglowkaInspektora);
         StanGeneratora.RozmiarX = EditorGUILayout.IntSlider("Rozmiar X", StanGeneratora.RozmiarX, Konf.MinRozmiar,
            Konf.MaksRozmiar);
         StanGeneratora.RozmiarZ = EditorGUILayout.IntSlider("Rozmiar Z", StanGeneratora.RozmiarZ, Konf.MinRozmiar,
            Konf.MaksRozmiar);
         StanGeneratora.Rozpietosc = EditorGUILayout.Slider("Rozpiêtoœæ", StanGeneratora.Rozpietosc, Konf.MinRozpietosc,
            Konf.MaksRozpietosc);
         if (GUILayout.Button("Generuj wêz³y"))
         {
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            _dzialaniaNaWezlach.GenerujWezly();
            StanGeneratora.Etap = Etap.ZaburzanieWezlow;
         }
      }

      private void SekcjaZaburzaniaITworzeniaKomorekIRogow()
      {
         EditorGUILayout.LabelField("Okreœl stopieñ zaburzenia:", Konf.StylNaglowkaInspektora);
         StanGeneratora.ZasiegZaburzenia = EditorGUILayout.Slider("Stopieñ zaburzenia", StanGeneratora.ZasiegZaburzenia, 0f,
            1f);
         if (GUILayout.Button("Zaburz wêz³y"))
         {
            _dzialaniaNaWezlach.ZaburzWezly(StanGeneratora.ZasiegZaburzenia*StanGeneratora.Rozpietosc, true);
            StanGeneratora.Etap = Etap.TworzenieKomorekIRogow;
         }
         if (GUILayout.Button("Utwórz komórki i rogi"))
         {
            _dzialaniaNaWezlach.UkryjWezly();
            _dzialaniaNaWezlach.GenerujKomorkiIRogi();
            StanGeneratora.Etap = Etap.TworzenieMapyWysokosci;
         }
      }

      private void SekcjaPokazywaniaIUkrywaniaScianIrogow()
      {
         PokazSciany = GUILayout.Toggle(PokazSciany, "Poka¿ œciany"); // todo czemu to siê nie zapisuje do stanu generatora?
         StanGeneratora.PokazRogi = GUILayout.Toggle(StanGeneratora.PokazRogi, "Poka¿ rogi");
         if (StanGeneratora.PokazRogi != StanGeneratora.PokazRogiPoprzedniaWartosc)
         {
            StanGeneratora.PokazRogiPoprzedniaWartosc = StanGeneratora.PokazRogi;
            if (!StanGeneratora.PokazRogi)
               _dzialaniaNaMapie.UkryjRogi();
            else
               _dzialaniaNaMapie.PokazRogi();
         }
      }

      private void SekcjaGenerowaniaMapyWysokosci()
      {
         GUILayout.BeginVertical(new GUIStyle
         {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(100, 100, 0, 0)
         });
         GUILayout.EndVertical();

         StanGeneratora.ParametryPerlina.Ziarno
            = EditorGUILayout.IntField("Ziarno", StanGeneratora.ParametryPerlina.Ziarno);

         StanGeneratora.ParametryPerlina.IloscWarstw
            = EditorGUILayout.IntSlider("Iloœæ warstw",
               StanGeneratora.ParametryPerlina.IloscWarstw, Konf.Perlin.MinIloscWarstw, Konf.Perlin.MaksIloscWarstw);

         StanGeneratora.ParametryPerlina.Skala
            = EditorGUILayout.Slider("Skala",
               StanGeneratora.ParametryPerlina.Skala, Konf.Perlin.MinSkala, Konf.Perlin.MaksSkala);

         StanGeneratora.ParametryPerlina.ZachowanieSkali
            = EditorGUILayout.Slider("Zachowanie skali",
               StanGeneratora.ParametryPerlina.ZachowanieSkali, Konf.Perlin.MinZachowanieSkali, Konf.Perlin.MaksZachowanieSkali);

         StanGeneratora.ParametryPerlina.SkokGestosci
            = EditorGUILayout.Slider("Skok gêstoœci",
               StanGeneratora.ParametryPerlina.SkokGestosci, Konf.Perlin.MinSkokGestosci, Konf.Perlin.MaksSkokGestosci);

         StanGeneratora.ParametryPerlina.Gestosc
            = EditorGUILayout.Slider("Gêstoœæ",
               StanGeneratora.ParametryPerlina.Gestosc, Konf.Perlin.MinGestosc, Konf.Perlin.MaksGestosc);
         if (GUILayout.Button("Generuj wysokoœci"))
         {
            _dzialaniaNaMapie.GenerujWysokosci(StanGeneratora.ParametryPerlina);
            if (!StanGeneratora.UtworzoneWarstwy.Contains(Warstwa.MapaFizyczna))
               StanGeneratora.UtworzoneWarstwy.Add(Warstwa.MapaFizyczna);
            AktualnaWarstwa = Warstwa.MapaFizyczna;

            _dzialaniaNaMapie.RozdzielZiemieIWode(StanGeneratora.PoziomMorza);
            AktualnaWarstwa = Warstwa.MapaFizyczna;
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
            StanGeneratora.Etap = Etap.RozdzielanieZiemiIWody;
         }
      }

      private void SekcjaPoziomuMorza()
      {
         EditorGUILayout.LabelField("Okreœl poziom morza", Konf.StylNaglowkaInspektora);
         StanGeneratora.PoziomMorza = EditorGUILayout.Slider("Poziom morza", StanGeneratora.PoziomMorza, Konf.MinPoziomMorza,
            Konf.MaksPoziomMorza);
         if (StanGeneratora.PoziomMorza != _poprzedniPoziomMorza)
         {
            _poprzedniPoziomMorza = StanGeneratora.PoziomMorza;
            _dzialaniaNaMapie.RozdzielZiemieIWode(StanGeneratora.PoziomMorza);
            AktualnaWarstwa = Warstwa.MapaFizyczna;
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
         }
         if (GUILayout.Button("Dalej"))
         {
            Debug.Log("jest mapa: " +(Poziom._mapa != null).ToString());
            Debug.Log("s¹ komórki: " +(Poziom._mapa.Komorki != null).ToString());
            Debug.Log("s¹ rogi: " +(Poziom._mapa.Rogi != null).ToString());
            _dzialaniaNaMapie.ZatwierdzRozdzielenieZiemiIWody(StanGeneratora.PoziomMorza);
            StanGeneratora.Etap = Etap.WydzielanieMorza;
            _dzialaniaNaMapie.UstawKomorkomWidocznoscPolaInicjatorPowodzi(true);
         }
      }

      private void SekcjaWydzielaniaMorza()
      {
         EditorGUILayout.LabelField("Oznacz komórki inicjuj¹ce morza", Konf.StylNaglowkaInspektora);
         StanGeneratora.InicjatorzyZalewania = Poziom._komorkiUnity.Where(k => k.InicjatorPowodzi);
         if (GUILayout.Button("Rozdziel morza i jeziora"))
         {
            _dzialaniaNaMapie.RozdzielMorzeIJeziora(StanGeneratora.InicjatorzyZalewania);
            _dzialaniaNaMapie.AktualizujBrzeznosci();
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
         }
         if (Poziom._mapa.Komorki.Any(k => k.Dane.Typ != null) && GUILayout.Button("Dalej"))
         {
            _dzialaniaNaMapie.UstawPunktomNastepstwaMapyWysokosci();
            _dzialaniaNaMapie.UstawKomorkomWidocznoscPolaInicjatorPowodzi(false);
            _dzialaniaNaMapie.WyznaczKomorkiNiecki();
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
            StanGeneratora.Etap = Etap.TworzenieJezior;
         }
      }

      private void SekcjaTworzeniaJezior()
      {
         if (LiczbaMiejscNaJeziora > 0)
         {
            EditorGUILayout.LabelField(String.Format("Pozosta³o {0} niecek na jeziora", LiczbaMiejscNaJeziora),
               Konf.StylNaglowkaInspektora);
            StanGeneratora.LiczbaJeziorDoWygenerowania = EditorGUILayout.IntSlider("Jezior do wygenerowania",
               StanGeneratora.LiczbaJeziorDoWygenerowania, 1, LiczbaMiejscNaJeziora);

            if (GUILayout.Button("Utwórz jeziora"))
            {
               _dzialaniaNaMapie.UtworzJezioraWNieckach(StanGeneratora.LiczbaJeziorDoWygenerowania);
               _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
               OdswiezZaznaczenieWarstwy();
            }
         }
         else
            EditorGUILayout.LabelField("Brak miejsc na wygenerowanie jezior.", Konf.StylNaglowkaInspektora);

         if (GUILayout.Button("Dalej"))
         {
            StanGeneratora.Etap = Etap.TworzenieRzek;
         }
      }

      private void SekcjaTworzeniaRzek()
      {
         StanGeneratora.LiczbaRzekDoWygenerowania = EditorGUILayout.IntSlider("Rzek do wygenerowania",
      StanGeneratora.LiczbaRzekDoWygenerowania, 1, Konf.MaksLiczbaRzekDoWygenerowaniaNaraz);

         if (GUILayout.Button("Utwórz rzeki"))
         {
            _dzialaniaNaMapie.UtworzRzeki(StanGeneratora.LiczbaRzekDoWygenerowania, new Random());
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
         }
         if (GUILayout.Button("Dalej"))
         {
            StanGeneratora.Etap = Etap.WyznaczanieWilgotnosci;
         }
      }

      private void SekcjaWyznaczaniaWilgotnosci()
      {
         PokazPanelWarstw();

         StanGeneratora.ParametryWilgotnosci.GlebokoscPrzeszukiwania = EditorGUILayout.IntSlider("G³êbokoœæ przeszukiwania",
      StanGeneratora.ParametryWilgotnosci.GlebokoscPrzeszukiwania, Konf.Wilg.MinGlebokoscPrzeszukiwania, Konf.Wilg.MaksGlebokoscPrzeszukiwania);
         StanGeneratora.ParametryWilgotnosci.WartoscJeziora = EditorGUILayout.Slider("Wilgotnoœæ jeziora",
      StanGeneratora.ParametryWilgotnosci.WartoscJeziora, Konf.Wilg.MinWartoscJezioraRzekiMorza, Konf.Wilg.MaksWartoscJezioraRzekiMorza);
         StanGeneratora.ParametryWilgotnosci.WartoscMorza = EditorGUILayout.Slider("Wilgotnoœæ morza",
      StanGeneratora.ParametryWilgotnosci.WartoscMorza, Konf.Wilg.MinWartoscJezioraRzekiMorza, Konf.Wilg.MaksWartoscJezioraRzekiMorza);
         StanGeneratora.ParametryWilgotnosci.WartoscRzeki = EditorGUILayout.Slider("Wilgotnoœæ rzeki",
      StanGeneratora.ParametryWilgotnosci.WartoscRzeki, Konf.Wilg.MinWartoscJezioraRzekiMorza, Konf.Wilg.MaksWartoscJezioraRzekiMorza);
         
         if (GUILayout.Button("Utwórz mapê wilgotnoœci"))
         {
            _dzialaniaNaMapie.UtworzMapeWilgotnosci(StanGeneratora.ParametryWilgotnosci);
            
            if (!StanGeneratora.UtworzoneWarstwy.Contains(Warstwa.Wilgotnosc))
               StanGeneratora.UtworzoneWarstwy.Add(Warstwa.Wilgotnosc);
            AktualnaWarstwa = Warstwa.Wilgotnosc;
            StanGeneratora.NumerWybranejWarstwy = StanGeneratora.UtworzoneWarstwy.IndexOf(AktualnaWarstwa);
            _dzialaniaNaMapie.PokazWarstweWilgotnosci();
         }
         if (StanGeneratora.UtworzoneWarstwy.Contains(Warstwa.Wilgotnosc))
         {
            if (GUILayout.Button("Dalej"))
               StanGeneratora.Etap = Etap.WyznaczanieTemperatury;
         }
      }

      private void SekcjaWyznaczaniaTemperatury()
      {
         PokazPanelWarstw();

         StanGeneratora.MnoznikTemperatury = EditorGUILayout.Slider("Mno¿nik temperatury", StanGeneratora.MnoznikTemperatury, 0f, 2f);
         if (GUILayout.Button("Utwórz mapê temperatury"))
         {
            _dzialaniaNaMapie.UtworzMapeTemperatury(StanGeneratora.MnoznikTemperatury);
            if (!StanGeneratora.UtworzoneWarstwy.Contains(Warstwa.Temperatura))
               StanGeneratora.UtworzoneWarstwy.Add(Warstwa.Temperatura);
            AktualnaWarstwa = Warstwa.Temperatura;
            StanGeneratora.NumerWybranejWarstwy = StanGeneratora.UtworzoneWarstwy.IndexOf(AktualnaWarstwa);
            _dzialaniaNaMapie.PokazWarstweTemperatury();
         }
         if (StanGeneratora.UtworzoneWarstwy.Contains(Warstwa.Temperatura))
            if (GUILayout.Button("Dalej"))
               StanGeneratora.Etap = Etap.WyznaczanieBiomow;
      }

      private void SekcjaWyznaczaniaBiomow()
      {
         PokazPanelWarstw();

         foreach (KonfiguracjaBiomu konfiguracjaBiomu in StanGeneratora.KonfiguracjaBiomow.ParametryBiomow)
         {
            konfiguracjaBiomu.Temperatura =
               EditorGUILayout.Slider(konfiguracjaBiomu.Biom.ToString() + " temp.", konfiguracjaBiomu.Temperatura, 0f,
                  1f);
            konfiguracjaBiomu.Wilgotnosc =
               EditorGUILayout.Slider(konfiguracjaBiomu.Biom.ToString() + " wilg.", konfiguracjaBiomu.Wilgotnosc, 0f, 1f);
         }

         if (GUILayout.Button("Utwórz mapê biomów"))
         {
            _dzialaniaNaMapie.UtworzMapeBiomow(StanGeneratora.KonfiguracjaBiomow);

            if (!StanGeneratora.UtworzoneWarstwy.Contains(Warstwa.Biomy))
               StanGeneratora.UtworzoneWarstwy.Add(Warstwa.Biomy);
            AktualnaWarstwa = Warstwa.Biomy;
            StanGeneratora.NumerWybranejWarstwy = StanGeneratora.UtworzoneWarstwy.IndexOf(AktualnaWarstwa);
            _dzialaniaNaMapie.PokazWarstweBiomow();
         }
         if (StanGeneratora.UtworzoneWarstwy.Contains(Warstwa.Biomy) && GUILayout.Button("Zakoñcz"))
         {
            StanGeneratora.Etap = Etap.Koniec;

            ZapiszPoziom();
         }
      }

      private void ZapiszPoziom()
      {
         var serializer = new XmlSerializer(typeof(Komorka), new[] { typeof(DaneKomorki), typeof(Punkt) });
         Stream stream = new FileStream("Poziom.xml", FileMode.Create, FileAccess.Write, FileShare.None);
         foreach (Komorka kom in Poziom._mapa.Komorki)
         {
            serializer.Serialize(stream, kom);
         }
         stream.Close();
      }

      private void PokazPanelWarstw()
      {
         if (!StanGeneratora.UtworzoneWarstwy.Any())
            return;
         GUI.color = new Color(.8f, .8f, .1f);
         GUILayout.Label("Warstwa:");
         StanGeneratora.NumerWybranejWarstwy = GUILayout.SelectionGrid(StanGeneratora.NumerWybranejWarstwy,
            StanGeneratora.UtworzoneWarstwy.ToList().Select(w => w.ToString()).ToArray(),
            1,GUILayout.Width(200));
         GUILayout.Space(20);
         GUI.color = Color.white;

         AktualnaWarstwa = StanGeneratora.UtworzoneWarstwy[StanGeneratora.NumerWybranejWarstwy];
         OdswiezZaznaczenieWarstwy();

         if (Poziom.AktualnaWarstwa != PoprzedniaWarstwa)
         {
            PoprzedniaWarstwa = Poziom.AktualnaWarstwa;
            if (Poziom.AktualnaWarstwa == Warstwa.MapaFizyczna)
            {
               _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
               OdswiezZaznaczenieWarstwy();
            }
            if (Poziom.AktualnaWarstwa == Warstwa.Wilgotnosc)
            {
               _dzialaniaNaMapie.PokazWarstweWilgotnosci();
               OdswiezZaznaczenieWarstwy();
            }
            if (Poziom.AktualnaWarstwa == Warstwa.Temperatura)
            {
               _dzialaniaNaMapie.PokazWarstweTemperatury();
               OdswiezZaznaczenieWarstwy();
            }
            if (Poziom.AktualnaWarstwa == Warstwa.Biomy)
            {
               _dzialaniaNaMapie.PokazWarstweBiomow();
               OdswiezZaznaczenieWarstwy();
            }
         }
      }

      public void OdswiezZaznaczenieWarstwy()
      {
         StanGeneratora.NumerWybranejWarstwy = StanGeneratora.UtworzoneWarstwy.IndexOf(AktualnaWarstwa);
         if (StanGeneratora.NumerWybranejWarstwy == -1)
            StanGeneratora.NumerWybranejWarstwy = 0;
      }
   }
}