using System.Collections.Generic;
using Assets.Skrypty;
using BenTools.Mathematics;
using PerlinTools;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Assets.Editor
{
    [CustomEditor(typeof (Teren))]
    public class TerenEditor : UnityEditor.Editor
    {
        private Teren _Teren;

        [SerializeField] private Random _random;

        private Teren Teren
        {
            get { return _Teren ?? (_Teren = (Teren) target); }
            set { _Teren = value; }
        }

        private Random Random
        {
            get { return _random ?? (_random = new Random()); }
            set { _random = value; }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generuj"))
            {
                var terrainData = new TerrainData();
               
                const float noiseScale = 100f;
                var maps = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, 
                                                    terrainData.heightmapHeight);
                for (var y = 0; y < terrainData.heightmapHeight; y++)
                {
                    for (var x = 0; x < terrainData.heightmapWidth; x++)
                    {
                        var a0 = maps[x, y];

                        a0 += (float)Random.NextDouble() * noiseScale;

                        maps[x, y] = a0;
                    }
                }

                terrainData.SetHeights(0, 0, maps);

                GameObject terrain = Terrain.CreateTerrainGameObject(terrainData);
            }

            /*
            _Teren.terr = Terrain.activeTerrain;
            hmWidth = terr.terrainData.heightmapWidth;
            hmHeight = terr.terrainData.heightmapHeight;
            Terrain.activeTerrain.heightmapMaximumLOD = 0;



            // get the heights of the terrain under this game object
            float[,] heights = terr.terrainData.GetHeights(0, 0, hmWidth, hmHeight);

            // we set each sample of the terrain in the size to the desired height
            //for (int i=0; i<hmWidth; i++)
            //for (int j=0; j<hmHeight; j++){
            //heights[i,j] = (Mathf.Sin(i/10f)/100f + Mathf.Cos((i+j)/10f) / 100f) + 0.5f;

            float[][] mapa = PerlinNoise.GeneratePerlinNoise(hmWidth, hmHeight, 4);
            for (int x = 0; x < hmWidth; ++x)
                for (int z = 0; z < hmHeight; ++z)
                {
                    heights[x, z] = mapa[x][z] / 1000f;
                }
            //print(heights[i,j]);
            //}
            // set the new height
            terr.terrainData.SetHeights(0, 0, heights);
             
             */
        }


        

    }
}