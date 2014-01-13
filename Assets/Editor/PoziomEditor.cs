using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BenTools.Mathematics;
using BenTools.Data;
using PerlinTools;

[CustomEditor(typeof(Poziom))]
public class PoziomEditor : Editor 
{	
	private Poziom _poziom;
	private Poziom Poziom
	{
		get {return _poziom ?? (_poziom = (Poziom)target);}
		set {_poziom = value;}
	}
	
	private System.Random _random;
	private System.Random Random{
		get {return _random ?? (_random = new System.Random(Poziom.ziarno));}
		set {_random = value;}
	}
	
	public override void OnInspectorGUI(){
		
		DrawDefaultInspector();
		
		if(Poziom._etap >= Etap.GenerowanieWezlow && GUILayout.Button("Resetuj") )
		{	
			UsunWezly();
			Poziom._etap = Etap.GenerowanieWezlow;
		}
		
		if(GUILayout.Button("Generuj wezly"))
		{
			GenerujWezly();	
			Poziom._etap = Etap.ZaburzanieWezlow;
		}
		
		if(Poziom._etap >= Etap.ZaburzanieWezlow && GUILayout.Button("Zaburz wezly"))
		{
			ZaburzWezly();	
			Poziom._etap = Etap.TworzenieDiagramuWoronoja;
		}
		
		if(Poziom._etap >= Etap.TworzenieDiagramuWoronoja && GUILayout.Button("Stworz diagram Woronoja"))
		{
			StworzDiagramWoronoja();	
		}
		
		if(Poziom._etap >= Etap.TworzenieMapyWysokosci && GUILayout.Button("Generuj mape wysokosci"))
		{
			GenerujMapeWysokosci();	
		}
		
		if(GUILayout.Button("Generuj teren [test]")){
			GenerujTeren();
		}
		
	}
	
	private void UsunWezly()
	{
		var wezlyDoUsuniecia = new List<GameObject>();
		foreach(Transform d in Poziom.gameObject.transform){
			if(d.gameObject.CompareTag("Wezel"))
			wezlyDoUsuniecia.Add(d.gameObject);
		}
		wezlyDoUsuniecia.ForEach(w => DestroyImmediate(w));
		Poziom._wezly = null;
		Poziom._krawedzieWoronoja = null;
		Poziom._etap = Etap.GenerowanieWezlow;
	}

	public void GenerujWezly ()
	{		
		UsunWezly();
		
		Poziom._wezly = new GameObject[Poziom._rozmiarX, Poziom._rozmiarZ];
		for(int x=0; x<Poziom._rozmiarX; ++x)
			for(int z=0; z<Poziom._rozmiarZ; ++z)
				{
				float rozpietosc = Poziom._rozpietosc;
				GameObject wezel = (GameObject)Instantiate(Resources.Load("Wezel"), new Vector3(x*rozpietosc, 0f, z*rozpietosc), Quaternion.identity);
				wezel.transform.parent = Poziom.transform;
				wezel.GetComponent<Wezel>().pierwotnaPozycja = wezel.transform.position;
				Poziom._wezly[x,z] = wezel;
				if(x<=1 || z<=1 || x>=Poziom._rozmiarX-2 || z>=Poziom._rozmiarZ-2)
					wezel.GetComponent<Wezel>().czySkrajny = true;
				}
		Poziom._etap = Etap.ZaburzanieWezlow;
	}	

	public void ZaburzWezly (bool pozostawSkrajne = true)
	{
		ZresetujUstawienieWezlow();
		float limitPrzesuniecia = Poziom._rozpietosc * .8f;
		foreach(GameObject w in Poziom._wezly){
			if(pozostawSkrajne && w.GetComponent<Wezel>().czySkrajny == true)
				continue;
			Vector3 transpozycja = new Vector3(
												limitPrzesuniecia - 2*(float)Random.NextDouble()*limitPrzesuniecia, 
												0f, 
												limitPrzesuniecia - 2*(float)Random.NextDouble()*limitPrzesuniecia
											  );
			w.transform.Translate(transpozycja);
		}
	}

	private void ZresetujUstawienieWezlow()
	{
		foreach(GameObject w in Poziom._wezly){
			w.transform.position = w.GetComponent<Wezel>().pierwotnaPozycja;
		}
	}
	
	private IEnumerable<Vector> WezlyNaWektory(){
		foreach(GameObject w in Poziom._wezly){
			yield return new Vector(w.GetComponent<Wezel>(), w.transform.position.x, w.transform.position.z);	
		}
	}
	
	private void StworzDiagramWoronoja(){
		Poziom._krawedzieWoronoja = Fortune.ComputeVoronoiGraph(WezlyNaWektory()).Edges;
		foreach(VoronoiEdge k in Poziom._krawedzieWoronoja){
			Wezel lewy = k.LeftData._wezel;
			lewy._scianyKomorki.Add(new Para<Vector3, Vector3>(k.VVertexA.ToVector3(), k.VVertexB.ToVector3()));
			
			Wezel prawy = k.RightData._wezel;
			prawy._scianyKomorki.Add(new Para<Vector3, Vector3>(k.VVertexA.ToVector3(), k.VVertexB.ToVector3()));
			
			
		}
		Poziom._etap = Etap.TworzenieMapyWysokosci;
	}
	
	private void GenerujMapeWysokosci(){
		float skalaWysokosci = 4f;
		float[][] mapa = PerlinNoise.GeneratePerlinNoise(Poziom._rozmiarX, Poziom._rozmiarZ, 2);
		for(int x = 0; x < Poziom._wezly.GetLength(0); ++x)
			for(int z = 0; z < Poziom._wezly.GetLength(1); ++z)
			{
				var wezel = Poziom._wezly[x,z].GetComponent<Wezel>();
				wezel._wysokosc = mapa[x][z];
				wezel.transform.Translate(0f, mapa[x][z] * skalaWysokosci, 0f);
				var kopiaMaterialu = new Material(wezel.renderer.sharedMaterial);
				if(wezel._wysokosc > Poziom._poziomMorza)
				{
					kopiaMaterialu.color = new Color(mapa[x][z] * 4f, 0f, 0f);
				}
				else
					kopiaMaterialu.color = new Color(0f, .7f, .95f);
				wezel.renderer.sharedMaterial = kopiaMaterialu;
			}
	}

	public void GenerujTeren ()
	{
		//throw new NotImplementedException ();
	}
}
