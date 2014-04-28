using System;
using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace Assets.Skrypty
{
   [ExecuteInEditMode]
   [Serializable]
   public class Poziom : MonoBehaviour
   {
      [HideInInspector] public Etap _etap = Etap.GenerowanieWezlow;
      public HashSet<VoronoiEdge> _krawedzieWoronoja;
      public bool _pokazDelaunaya = true;
      public bool _pokazWoronoja = true;
      public bool _pokazSciany = true;

      [Range(0f, 1f)] public float _poziomMorza = 0.3f;

      [Range(10, 100)] public int _rozmiarX = 20;

      [Range(10, 100)] public int _rozmiarZ = 20;

      [Range(1f, 5f)] public float _rozpietosc = 2f;

      public Wezel[,] _wezly;
      public IMapa _mapa;
      public static int Ziarno;

      public void OnDrawGizmos()
      {
         Gizmos.color = Color.green;
         if (_pokazDelaunaya)
            if (_krawedzieWoronoja != null)
               foreach (VoronoiEdge k in _krawedzieWoronoja)
               {
                  var lewy = new Vector3((float) k.LeftData[0], 0f, (float) k.LeftData[1]);
                  var prawy = new Vector3((float) k.RightData[0], 0f, (float) k.RightData[1]);
                  Gizmos.DrawLine(lewy, prawy);
               }

         Gizmos.color = Color.red;
         if (_pokazWoronoja)
            if (_krawedzieWoronoja != null)
               foreach (VoronoiEdge k in _krawedzieWoronoja) 
               {
                  var lewy = new Vector3((float) k.VVertexA[0], 0f, (float) k.VVertexA[1]);
                  var prawy = new Vector3((float) k.VVertexB[0], 0f, (float) k.VVertexB[1]);
                  Gizmos.DrawLine(lewy, prawy);
               }
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