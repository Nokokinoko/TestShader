using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( MeshFilter ), typeof ( MeshRenderer ) )]
public class Snow : MonoBehaviour
{
	private const int VERTEX_NUM = 4;
	private const int SNOW_NUM = 10000;

	private Vector3[] m_Vertex_r;
	private int[] m_Triangle_r;
	private Vector2[] m_UV_r;

	private float m_Range;
	private float m_RangeR;
	private Vector3 m_Move = Vector3.zero;
	private Material m_Material;

	private struct ShaderProperty
	{
		public int _IdTargetPosition;
		public int _IdRange;
		public int _IdRangeR;
		public int _IdSize;
		public int _IdMoveTotal;
		public int _IdCamUp;
	}
	private ShaderProperty m_ShaderProperty;

	void Start ()
	{
		m_Range = 16.0f;
		m_RangeR = 1.0f / m_Range;
		m_Vertex_r = new Vector3[SNOW_NUM * VERTEX_NUM];
		for ( int i = 0 ; i < SNOW_NUM ; i++ )
		{
			int _Idx = i * VERTEX_NUM;
			Vector3 _Point = new Vector3 ( makeRandom (), makeRandom (), makeRandom () );
			m_Vertex_r[ _Idx ] = _Point;
			m_Vertex_r[ _Idx + 1 ] = _Point;
			m_Vertex_r[ _Idx + 2 ] = _Point;
			m_Vertex_r[ _Idx + 3 ] = _Point;
		}

		m_Triangle_r = new int[SNOW_NUM * 6];
		for ( int i = 0 ; i < SNOW_NUM ; i++ )
		{
			int _Idx = i * 6;
			int _IdxVertex = i * VERTEX_NUM;
			// Polygon 1
			m_Triangle_r[ _Idx ] = _IdxVertex;
			m_Triangle_r[ _Idx + 1 ] = _IdxVertex + 1;
			m_Triangle_r[ _Idx + 2 ] = _IdxVertex + 2;
			// Polygon 2
			m_Triangle_r[ _Idx + 3 ] = _IdxVertex + 2;
			m_Triangle_r[ _Idx + 4 ] = _IdxVertex + 1;
			m_Triangle_r[ _Idx + 5 ] = _IdxVertex + 3;
		}

		m_UV_r = new Vector2[SNOW_NUM * VERTEX_NUM];
		for ( int i = 0 ; i < SNOW_NUM ; i++ )
		{
			int _Idx = i * VERTEX_NUM;
			m_UV_r[ _Idx ] = new Vector2 ( 0.0f, 0.0f );
			m_UV_r[ _Idx + 1 ] = new Vector2 ( 1.0f, 0.0f );
			m_UV_r[ _Idx + 2 ] = new Vector2 ( 0.0f, 1.0f );
			m_UV_r[ _Idx + 3 ] = new Vector2 ( 1.0f, 1.0f );
		}

		Mesh _Mesh = new Mesh ();
		_Mesh.name = "MeshSnowFlake";
		_Mesh.vertices = m_Vertex_r;
		_Mesh.triangles = m_Triangle_r;
		_Mesh.uv = m_UV_r;
		_Mesh.bounds = new Bounds ( Vector3.zero, Vector3.one * 99999999 );
		MeshFilter _Filter = GetComponent<MeshFilter> ();
		_Filter.sharedMesh = _Mesh;

		m_Material = GetComponent<Renderer> ().material;
		m_ShaderProperty = new ShaderProperty ();
		m_ShaderProperty._IdTargetPosition = Shader.PropertyToID ( "_TargetPosition" );
		m_ShaderProperty._IdRange = Shader.PropertyToID ( "_Range" );
		m_ShaderProperty._IdRangeR = Shader.PropertyToID ( "_RangeR" );
		m_ShaderProperty._IdSize = Shader.PropertyToID ( "_Size" );
		m_ShaderProperty._IdMoveTotal = Shader.PropertyToID ( "_MoveTotal" );
		m_ShaderProperty._IdCamUp = Shader.PropertyToID ( "_CamUp" );
	}

	void LateUpdate ()
	{
		Vector3 _Position = Camera.main.transform.TransformPoint ( Vector3.forward * m_Range );
		m_Material.SetVector ( m_ShaderProperty._IdTargetPosition, _Position );
		m_Material.SetFloat ( m_ShaderProperty._IdRange, m_Range );
		m_Material.SetFloat ( m_ShaderProperty._IdRangeR, m_RangeR );
		m_Material.SetFloat ( m_ShaderProperty._IdSize, 0.1f );
		m_Material.SetVector ( m_ShaderProperty._IdMoveTotal, m_Move );
		m_Material.SetVector ( m_ShaderProperty._IdCamUp, Camera.main.transform.up );

		m_Move += new Vector3 (
			( Mathf.PerlinNoise ( 0.0f, Time.time * 0.1f ) - 0.5f ) * 10.0f,
			-2.0f,
			( Mathf.PerlinNoise ( Time.time * 0.1f, 0.0f ) - 0.5f ) * 10.0f
		) * Time.deltaTime;
		m_Move.x = Mathf.Repeat ( m_Move.x, m_Range * 2.0f );
		m_Move.y = Mathf.Repeat ( m_Move.y, m_Range * 2.0f );
		m_Move.z = Mathf.Repeat ( m_Move.z, m_Range * 2.0f );
	}

	private float makeRandom ()
	{
		return Random.Range ( m_Range * -1, m_Range );
	}
}
