using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Xml.Serialization;
using Assets.Skrypty.Generowanie;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace Assets.Skrypty
{
   [ExecuteInEditMode]
   [Serializable]
   public class Poziom : MonoBehaviour
   {
      // chowanie czego� � atrybut HideInInspector
      // pokazywanie w inspektorze nieautomatycznej w�a�ciwo�ci: http://wiki.unity3d.com/index.php?title=Expose_properties_in_inspector
      // UWAGA! � taka w�a�ciwo�� musi posiada� funkcje get i set, nawet je�li kt�ra� z nich ma nic nie robi�

      public HashSet<VoronoiEdge> _krawedzieWoronoja;

      [HideInInspector]
      public bool _pokazSciany = true;

      [HideInInspector]
      public Warstwa AktualnaWarstwa = Warstwa.Brak;
      
      [SerializeField] public Wezel[,] _wezly;
      [SerializeField] public IList<KomorkaUnity> _komorkiUnity;
      [SerializeField] public IList<RogUnity> _rogiUnity;

      [SerializeField] public Mapa _mapa;

      [SerializeField] private StanGeneratora _stanGeneratora;
      [SerializeField] private Pojemnik _komponentPojemnika;
      public static int Ziarno;

      public Poziom()
      {
         _komorkiUnity = new List<KomorkaUnity>();
         _rogiUnity = new List<RogUnity>();
         _stanGeneratora = new StanGeneratora();
      }

      public Pojemnik KomponentPojemnika
      {
         get { return _komponentPojemnika; }
         set { _komponentPojemnika = value; }
      }

      public StanGeneratora StanGeneratora
      {
         get { return _stanGeneratora; }
         set { _stanGeneratora = value; }
      }

      public void OnDrawGizmos()
      {
         Gizmos.color = Color.yellow;
         if (_pokazSciany)
            if (_wezly != null && _mapa != null && _mapa.Rogi.Any())
               foreach (IRog r in _mapa.Rogi)
               {
                  r.BliskieRogi.ToList().ForEach(s => Gizmos.DrawLine(r.Punkt.Pozycja.NaVector3(), s.Punkt.Pozycja.NaVector3()));
               }
      }

      // Use this for initialization
      public void Start()
      {
         Ziarno = (int) DateTime.Now.ToBinary();
      }

      // Update is called once per frame
      public void Update()
      {
      }
   }
}