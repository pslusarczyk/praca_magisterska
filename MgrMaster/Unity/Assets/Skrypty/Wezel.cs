using System.Collections.Generic;
using UnityEngine;

namespace Assets.Skrypty
{
   public class Para<T1, T2>
   {
      public T2 drugi;
      public T1 pierwszy;

      public Para(T1 t1, T2 t2)
      {
         pierwszy = t1;
         drugi = t2;
      }
   }

   public class Wezel : MonoBehaviour
   {
      public bool _pokazSciany = false;

      public IList<Para<Vector3, Vector3>> _scianyKomorki = new List<Para<Vector3, Vector3>>();
      public float _wysokosc;
      public bool czySkrajny = false;
      public Vector3 pierwotnaPozycja;


      public void OnDrawGizmos()
      {
         Gizmos.color = Color.magenta;
         if (_pokazSciany)
            foreach (var s in _scianyKomorki)
            {
               Gizmos.DrawLine(s.pierwszy, s.drugi);
               Gizmos.color = Gizmos.color*.75f;
            }
      }

      // Use this for initialization
      private void Start()
      {
      }

      // Update is called once per frame
      private void Update()
      {
      }

      public void GenerujKsztaltKomorki()
      {
         /*
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter==null){
		    Debug.LogError("MeshFilter not found!");
		    return;
		}
		 
		Vector3 p0 = new Vector3(0,0,0);
		Vector3 p1 = new Vector3(1,0,0);
		Vector3 p2 = new Vector3(0.5f,0,Mathf.Sqrt(0.75f));
		Vector3 p3 = new Vector3(0.5f,Mathf.Sqrt(0.75f),Mathf.Sqrt(0.75f)/3);
		
		if(_scianyKomorki.Count == 0)
			throw new InvalidOperationException("sciany komórki niewypełnione");
		
		IList<Vector3> wektory = _scianyKomorki.Select<Vector3>(p => p.pierwszy)
					  .Concat(_scianyKomorki.Select<Vector3>(p => p.drugi));
		wektory = wektory.Distinct().Select<Vector3>(w => transform.InverseTransformPoint(w));
		 
		Mesh mesh = meshFilter.sharedMesh;
		if (mesh == null){
		    meshFilter.mesh = new Mesh();
		    mesh = meshFilter.sharedMesh;
		}
		mesh.Clear();
		mesh.vertices = wektory.ToArray();
		mesh.triangles = new int[]{
		    0,1,2,
		    0,2,3,
		    2,1,3,
		    0,3,1
		};
		 
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();	
		*/
      }
   }
}