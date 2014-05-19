using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Assets.Skrypty.Narzedzia;
using LogikaGeneracji;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

public enum Warstwa { Brak, Wysokosci, ZiemiaMorze }

namespace Assets.Skrypty
{
   [ExecuteInEditMode]
   [Serializable]
   public class Poziom : MonoBehaviour
   {
      // chowanie czegoœ — atrybut HideInInspector
      // pokazywanie w inspektorze nieautomatycznej w³aœciwoœci: http://wiki.unity3d.com/index.php?title=Expose_properties_in_inspector
      // UWAGA! — taka w³aœciwoœæ musi posiadaæ funkcje get i set, nawet jeœli któraœ z nich ma nic nie robiæ

      public Etap _etap = Etap.GenerowanieWezlow;

      [ExposeProperty]
      public string EtapTekst { get { return _etap.ToString(); } set{} }

      public Warstwa warstwa = Warstwa.Brak;

      public HashSet<VoronoiEdge> _krawedzieWoronoja;
      public bool _pokazSciany = true;

      [Range(0f, 1f)] public float _poziomMorza = 0.3f;

      [Range(10, 100)] public int _rozmiarX = 20;

      [Range(10, 100)] public int _rozmiarZ = 20;

      [Range(1f, 5f)] public float _rozpietosc = 2f;

      public Wezel[,] _wezly;
      public IList<KomorkaUnity> _komorkiUnity;
      public IList<RogUnity> _rogiUnity;
      public IMapa _mapa;
      public static int Ziarno;

      public Poziom()
      {
         _komorkiUnity = new List<KomorkaUnity>();
         _rogiUnity = new List<RogUnity>();
      }

      public void OnDrawGizmos()
      {
         Gizmos.color = Color.yellow;
         if (_pokazSciany)
            if (_wezly != null && _mapa != null && _mapa.Rogi.Any())
               foreach (IRog r in _mapa.Rogi)
               {
                  r.BliskieRogi.ToList().ForEach(s => Gizmos.DrawLine(r.Punkt.Pozycja, s.Punkt.Pozycja));
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