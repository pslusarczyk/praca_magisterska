using UnityEngine;
using PerlinTools;
using UnityEditor;

namespace Assets.Skrypty
{
    public class Teren : EditorWindow {
 
        Terrain terr; // terrain to modify
        int hmWidth; // heightmap width
        int hmHeight; // heightmap height
 

        [MenuItem("Window/Teren")]
        static void ShowWindow(){
            EditorWindow.GetWindow(typeof(Teren), false);		
        }
		
    		
        public void OnGui(){
            if(GUILayout.Button("Generuj teren [test]")){
                MakeTerrain();
            }
        }

 
        void MakeTerrain() {
 
            terr = Terrain.activeTerrain;
            hmWidth = terr.terrainData.heightmapWidth;
            hmHeight = terr.terrainData.heightmapHeight;
            Terrain.activeTerrain.heightmapMaximumLOD = 0;
		
		
		
            // get the heights of the terrain under this game object
            float[,] heights = terr.terrainData.GetHeights(0,0,hmWidth,hmHeight);
 
            // we set each sample of the terrain in the size to the desired height
            //for (int i=0; i<hmWidth; i++)
            //for (int j=0; j<hmHeight; j++){
            //heights[i,j] = (Mathf.Sin(i/10f)/100f + Mathf.Cos((i+j)/10f) / 100f) + 0.5f;
			
            float[][] mapa = PerlinNoise.GeneratePerlinNoise(hmWidth, hmHeight, 4);
            for(int x = 0; x < hmWidth; ++x)
                for(int z = 0; z < hmHeight; ++z)
                {
                    heights[x,z] = mapa[x][z] / 1000f;
                }
            //print(heights[i,j]);
            //}
            // set the new height
            terr.terrainData.SetHeights(0,0,heights);
 
        }
    }
}