using UnityEngine;
using System.Collections;
using BenTools.Data;
using BenTools.Mathematics; 
using System;

[ExecuteInEditMode]
[Serializable]
public class Poziom : MonoBehaviour {
	
	public bool _pokazWoronoja = true;
	public bool _pokazDelaunaya = true;
	
	[Range(10,100)]
	public int _rozmiarX = 20;
	
	[Range(10,100)]
	public int _rozmiarZ = 20;
	
	[Range(1f, 5f)]
	public float _rozpietosc = 2f;
	
	[Range(0f, 1f)]
	public float _poziomMorza = 0.3f;
	
	public int ziarno;
	
	[HideInInspector]
	public Etap _etap = Etap.GenerowanieWezlow;
	
	public Wezel[,] _wezly;
	
	public HashSet<VoronoiEdge> _krawedzieWoronoja;
	
	void OnDrawGizmos(){
		Gizmos.color = Color.green;
		if(_pokazDelaunaya)
			if(_krawedzieWoronoja != null)
				foreach(VoronoiEdge k in _krawedzieWoronoja)
			{
				var lewy = new Vector3((float)k.LeftData[0], 0f, (float)k.LeftData[1]);
				var prawy = new Vector3((float)k.RightData[0], 0f, (float)k.RightData[1]);
				Gizmos.DrawLine(lewy, prawy);
			}
		
		Gizmos.color = Color.red;
		if(_pokazWoronoja)
			if(_krawedzieWoronoja != null)
				foreach(VoronoiEdge k in _krawedzieWoronoja)
			{
				var lewy = new Vector3((float)k.VVertexA[0], 0f, (float)k.VVertexA[1]);
				var prawy = new Vector3((float)k.VVertexB[0], 0f, (float)k.VVertexB[1]);
				Gizmos.DrawLine(lewy, prawy);
		}
	}
	
	// Use this for initialization
	void Start () {
		ziarno = (int)DateTime.Now.ToBinary();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
