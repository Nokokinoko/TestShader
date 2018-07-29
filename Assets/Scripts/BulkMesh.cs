using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class CubeWave
{
	[System.Serializable]
	class BulkMesh
	{
		#region Property
		private Mesh m_Mesh;
		public Mesh Mesh { get { return this.m_Mesh; } }

		private int m_CountCopy;
		public int CountCopy { get { return this.m_CountCopy; } }

		private int m_CountCopyMax;
		#endregion

		#region PublicMethod
		// Constructor
		public BulkMesh ( Mesh[] pShape_r, int pCountCopyMax )
		{
			this.m_CountCopyMax = Mathf.Max ( pCountCopyMax, 4096 );
			this.CombineMesh ( pShape_r );
		}

		public void Rebuild ( Mesh[] pShape_r )
		{
			this.Release ();
			this.CombineMesh ( pShape_r );
		}

		public void Release ()
		{
			if ( this.Mesh )
			{
				DestroyImmediate ( this.Mesh );
				this.m_CountCopy = 0;
			}
		}
		#endregion

		#region PrivateMethod
		private struct StructShapeCache
		{
			private Vector3[] m_Vertex_r;
			public int CountVertex { get { return this.m_Vertex_r.Length; } }

			private Vector3[] m_Normal_r;
			private Vector4[] m_Tangent_r;
			private Vector2[] m_UV_r;

			private int[] m_Index_r;
			public int CountIndex { get { return this.m_Index_r.Length; } }

			// Constructor
			public StructShapeCache ( Mesh pMesh )
			{
				if ( pMesh )
				{
					this.m_Vertex_r = pMesh.vertices;
					this.m_Normal_r = pMesh.normals;
					this.m_Tangent_r = pMesh.tangents;
					this.m_UV_r = pMesh.uv;
					this.m_Index_r = pMesh.GetIndices ( 0 );
				}
				else
				{
					// Replaces with a two-sided quad.
					this.m_Vertex_r = new Vector3[] {
						new Vector3 ( -1, 1, 0 ),	new Vector3 ( 1, 1, 0 ),
						new Vector3 ( -1, -1, 0 ),	new Vector3 ( 1, -1, 0 ),
						new Vector3 ( 1, 1, 0 ),	new Vector3 ( -1, 1, 0 ),
						new Vector3 ( 1, -1, 0 ),	new Vector3 ( -1, -1, 0 ),
					};
					this.m_Normal_r = new Vector3[] {
						Vector3.forward,		Vector3.forward,
						Vector3.forward,		Vector3.forward,
						Vector3.forward * -1,	Vector3.forward * -1,
						Vector3.forward * -1,	Vector3.forward * -1,
					};
					this.m_Tangent_r = new Vector4[] {
						new Vector4 ( 1, 0, 0, 1 ),	new Vector4 ( 1, 0, 0, 1 ),
						new Vector4 ( 1, 0, 0, 1 ),	new Vector4 ( 1, 0, 0, 1 ),
						new Vector4 ( -1, 0, 0, 1 ),	new Vector4 ( -1, 0, 0, 1 ),
						new Vector4 ( -1, 0, 0, 1 ),	new Vector4 ( -1, 0, 0, 1 ),
					};
					this.m_UV_r = new Vector2[] {
						new Vector2 ( 0, 1 ),	new Vector2 ( 1, 1 ),
						new Vector2 ( 0, 0 ),	new Vector2 ( 1, 0 ),
						new Vector2 ( 1, 1 ),	new Vector2 ( 0, 1 ),
						new Vector2 ( 1, 0 ),	new Vector2 ( 0, 0 ),
					};
					this.m_Index_r = new int[] { 0, 1, 2, 3, 2, 1, 4, 5, 6, 7, 6, 5 };
				}
			}

			public void CopyVertexTo ( Vector3[] pDestination_r, int pPosition )
			{
				System.Array.Copy ( this.m_Vertex_r, 0, pDestination_r, pPosition, this.CountVertex );
			}

			public void CopyNormalTo ( Vector3[] pDestination_r, int pPosition )
			{
				System.Array.Copy ( this.m_Normal_r, 0, pDestination_r, pPosition, this.m_Normal_r.Length );
			}

			public void CopyTangentTo ( Vector4[] pDestination_r, int pPosition )
			{
				System.Array.Copy ( this.m_Tangent_r, 0, pDestination_r, pPosition, this.m_Tangent_r.Length );
			}

			public void CopyUVTo ( Vector2[] pDestination_r, int pPosition )
			{
				System.Array.Copy ( this.m_UV_r, 0, pDestination_r, pPosition, this.m_UV_r.Length );
			}

			public void CopyIndexTo ( int[] pDestination_r, int pPosition, int pOffset )
			{
				for ( int i = 0 ; i < this.CountIndex ; i++ )
				{
					pDestination_r[ pPosition + i ] = pOffset + this.m_Index_r[ i ];
				}
			}
		}

		private void CombineMesh ( Mesh[] pShape_r )
		{
			StructShapeCache[] _Cache_r;

			if ( pShape_r == null || pShape_r.Length <= 0 )
			{
				// Default shape
				_Cache_r = new StructShapeCache[] {
					new StructShapeCache ( null ),
				};
			}
			else
			{
				_Cache_r = new StructShapeCache[pShape_r.Length];
				for ( int i = 0 ; i < pShape_r.Length ; i++ )
				{
					_Cache_r[ i ] = new StructShapeCache ( pShape_r[ i ] );
				}
			}

			int _CountVertex = 0, _CountIndex = 0;
			foreach ( StructShapeCache _Cache in _Cache_r )
			{
				_CountVertex += _Cache.CountVertex;
			}
			if ( _CountVertex <= 0 )
			{
				return;
			}

			// コピー数を決定
			_CountVertex = 0;
			for ( this.m_CountCopy = 0 ; this.m_CountCopy < this.m_CountCopyMax ; this.m_CountCopy++ )
			{
				StructShapeCache _Cache = _Cache_r[ this.CountCopy % _Cache_r.Length ];
				if ( 65535 < _CountVertex + _Cache.CountVertex )
				{
					break;
				}

				_CountVertex += _Cache.CountVertex;
				_CountIndex += _Cache.CountIndex;
			}

			// バーテックス配列生成
			Vector3[] _Vertex_r = new Vector3[_CountVertex];
			Vector3[] _Normal_r = new Vector3[_CountVertex];
			Vector4[] _Tangent_r = new Vector4[_CountVertex];
			Vector2[] _UV1_r = new Vector2[_CountVertex];
			Vector2[] _UV2_r = new Vector2[_CountVertex];
			int[] _Index_r = new int[_CountIndex];
			for ( int iVertex = 0, iIndex = 0, iElm = 0 ; iVertex < _CountVertex ; iElm++ )
			{
				StructShapeCache _Cache = _Cache_r[ iElm % _Cache_r.Length ];

				_Cache.CopyVertexTo ( _Vertex_r, iVertex );
				_Cache.CopyNormalTo ( _Normal_r, iVertex );
				_Cache.CopyTangentTo ( _Tangent_r, iVertex );
				_Cache.CopyUVTo ( _UV1_r, iVertex );
				_Cache.CopyIndexTo ( _Index_r, iIndex, iVertex );

				Vector2 _Coord = new Vector2 ( (float)iElm / this.CountCopy, 0 );
				for ( int i = 0 ; i < _Cache.CountVertex ; i++ )
				{
					_UV2_r[ iVertex + i ] = _Coord;
				}

				iVertex += _Cache.CountVertex;
				iIndex += _Cache.CountIndex;
			}

			// メッシュオブジェクト生成
			this.m_Mesh = new Mesh ();
			this.m_Mesh.vertices = _Vertex_r;
			this.m_Mesh.normals = _Normal_r;
			this.m_Mesh.tangents = _Tangent_r;
			this.m_Mesh.uv = _UV1_r;
			this.m_Mesh.uv2 = _UV2_r;
			this.m_Mesh.SetIndices ( _Index_r, MeshTopology.Triangles, 0 );

			this.m_Mesh.hideFlags = HideFlags.DontSave; // 一時使用のため保存しない
			this.m_Mesh.bounds = new Bounds ( Vector3.zero, Vector3.one * 1000 ); // 淘汰されないよう制御
		}
		#endregion
	}
}
