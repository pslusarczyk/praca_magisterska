using System;
using Assets.Skrypty;
using UnityEditor;
using UnityEngine;
using ZewnetrzneBiblioteki.PerlinNoise;
using Random = System.Random;

namespace Assets.Editor
{
    [CustomEditor(typeof(Teren))]
    public class TerenEditor :UnityEditor.Editorr
    {
        private Teren _terrain;

        [SerializeField]
        private Random _random;

        private Teren Teren
        {
            get { return _terrain ?? (_terrain = (Teren)target); }
            set { _terrain = value; }
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
                var terrainData = new TerrainData { heightmapResolution = 65, size = new Vector3(50f, 20f, 50f) };
                
                int xSize = terrainData.heightmapHeight;
                int zSize = terrainData.heightmapWidth;

                const float noiseScale = 1.5f;

                var heights = terrainData.GetHeights(0, 0, xSize, zSize);

                float[][] noise = PerlinTools.GeneratePerlinNoise(xSize, zSize, 8);


                for (var x = 0; x < terrainData.heightmapHeight; x++)
                {
                    for (var z = 0; z < terrainData.heightmapWidth; z++)
                    {
                        heights[x, z] = noise[x][z] * noiseScale;
                    }
                }
                terrainData.SetHeights(0, 0, heights);


                var tekstura = Resources.Load<Texture2D>(
                                    "prototype_textures/Textures/proto_blue");
                if (tekstura != null)
                {
                    var sp = new SplatPrototype[1];
                    sp[0] = new SplatPrototype{texture = tekstura};
                    terrainData.splatPrototypes = sp;
                    var alphamaps = new float[xSize, zSize, sp.Length];
                    for(int ax = 0; ax < xSize; ++ax)
                        for(int az = 0; az < zSize; ++az)
                            for (int tex = 0; tex < sp.Length; ++tex)
                                alphamaps[ax, az, tex] = Math.Abs(Mathf.Sin(ax*1.2f + az*1.3f));
                    terrainData.SetAlphamaps(0, 0, alphamaps);
                }

                GameObject terrain = Terrain.CreateTerrainGameObject(terrainData);
            }

            /*
            _Terrain.terr = Terrain.activeTerrain;
            hmWidth = terr.terrainData.heightmapWidth;
            hmHeight = terr.terrainData.heightmapHeight;
            Terrain.activeTerrain.heightmapMaximumLOD = 0;



            // get the heights of the terrain under this game object
            float[,] heights = terr.terrainData.GetHeights(0, 0, hmWidth, hmHeight);

            // we set each sample of the terrain in the size to the desired height
            //for (int i=0; i<hmWidth; i++)
            //for (int j=0; j<hmHeight; j++){
            //heights[i,j] = (Mathf.Sin(i/10f)/100f + Mathf.Cos((i+j)/10f) / 100f) + 0.5f;

            float[][] mapa = PerlinTools.GeneratePerlinNoise(hmWidth, hmHeight, 4);
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