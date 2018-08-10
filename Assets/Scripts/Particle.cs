using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
	#region Property
	[SerializeField]
	private Shader m_ShaderKernel;
	[SerializeField]
	private Material m_Material;

	private RenderTexture m_PositionBuffer1;
	private RenderTexture m_PositionBuffer2;

	private RenderTexture m_VelocityBuffer1;
	private RenderTexture m_VelocityBuffer2;

	private Material m_MaterialKernel;
	private MaterialPropertyBlock m_MaterialProperty;
	private Mesh m_Mesh;

	private bool m_IsNeedReset = true;
	private readonly int WIDTH = 1000;
	private readonly int HEIGHT = 1000;
	#endregion

	private RenderTexture CreateBuffer ()
	{
		RenderTexture _Buffer = new RenderTexture ( this.WIDTH, this.HEIGHT, 0, RenderTextureFormat.ARGBFloat );
		_Buffer.hideFlags = HideFlags.DontSave;
		_Buffer.filterMode = FilterMode.Point;
		_Buffer.wrapMode = TextureWrapMode.Repeat;
		return _Buffer;
	}

	private Material CreateMaterial ( Shader pShader )
	{
		Material _Material = new Material ( pShader );
		_Material.hideFlags = HideFlags.DontSave;
		return _Material;
	}

	private Mesh CreateMesh ()
	{
		int _NumPoint = this.HEIGHT * this.WIDTH;

		Vector3[] _Point_r = new Vector3[_NumPoint];
		Color[] _Color_r = new Color[_NumPoint];
		int[] _Index_r = new int[_NumPoint];
		Vector2 _UV = Vector2.zero;
		for ( int i = 0 ; i < _NumPoint ; i++ )
		{
			_UV.x = i % this.WIDTH;
			_UV.x /= this.WIDTH;

			_UV.y = i / this.HEIGHT;
			_UV.y /= this.HEIGHT;

			_Point_r[ i ] = new Vector3 ( _UV.x, _UV.y, 0.0f );
			_Color_r[ i ] = new Color (
				Random.Range ( 0.0f, 1.0f ),
				Random.Range ( 0.0f, 1.0f ),
				Random.Range ( 0.0f, 1.0f ),
				1.0f
			);
			_Index_r[ i ] = i;
		}

		Mesh _Mesh = new Mesh ();
		_Mesh.vertices = _Point_r;
		_Mesh.colors = _Color_r;
		_Mesh.SetIndices ( _Index_r, MeshTopology.Points, 0 );
		return _Mesh;
	}

	private void ResetResouce ()
	{
		this.m_Mesh = this.CreateMesh ();

		if ( this.m_PositionBuffer1 )
		{
			DestroyImmediate ( this.m_PositionBuffer1 );
		}
		this.m_PositionBuffer1 = this.CreateBuffer ();

		if ( this.m_PositionBuffer2 )
		{
			DestroyImmediate ( this.m_PositionBuffer2 );
		}
		this.m_PositionBuffer2 = this.CreateBuffer ();

		if ( this.m_VelocityBuffer1 )
		{
			DestroyImmediate ( this.m_VelocityBuffer1 );
		}
		this.m_VelocityBuffer1 = this.CreateBuffer ();

		if ( this.m_VelocityBuffer2 )
		{
			DestroyImmediate ( this.m_VelocityBuffer2 );
		}
		this.m_VelocityBuffer2 = this.CreateBuffer ();

		if ( !this.m_MaterialKernel )
		{
			this.m_MaterialKernel = this.CreateMaterial ( this.m_ShaderKernel );
		}

		this.InitBuffer ();

		this.m_IsNeedReset = false;
	}

	private void InitBuffer ()
	{
		Graphics.Blit ( null, this.m_PositionBuffer2, this.m_MaterialKernel, 0 );
		Graphics.Blit ( null, this.m_VelocityBuffer2, this.m_MaterialKernel, 1 );
	}

	private void SwapBufferAndInvokeKernel ()
	{
		RenderTexture _PositionTmp = this.m_PositionBuffer1;
		this.m_PositionBuffer1 = this.m_PositionBuffer2;
		this.m_PositionBuffer2 = _PositionTmp;

		RenderTexture _VelocityTmp = this.m_VelocityBuffer1;
		this.m_VelocityBuffer1 = this.m_VelocityBuffer2;
		this.m_VelocityBuffer2 = _VelocityTmp;

		this.m_MaterialKernel.SetTexture ( "_PositionBuffer", this.m_PositionBuffer1 );
		this.m_MaterialKernel.SetTexture ( "_VelocityBuffer", this.m_VelocityBuffer1 );
		Graphics.Blit ( null, this.m_PositionBuffer2, this.m_MaterialKernel, 2 );

		this.m_MaterialKernel.SetTexture ( "_PositionBuffer", this.m_PositionBuffer2 );
		Graphics.Blit ( null, this.m_VelocityBuffer2, this.m_MaterialKernel, 3 );
	}

	#region UnityEngine
	void Update ()
	{
		if ( this.m_IsNeedReset )
		{
			this.ResetResouce ();
		}

		if ( Input.GetMouseButtonDown ( 0 ) )
		{
			this.InitBuffer ();
		}

		this.SwapBufferAndInvokeKernel ();

		if ( this.m_MaterialProperty == null )
		{
			this.m_MaterialProperty = new MaterialPropertyBlock ();
		}
		this.m_MaterialProperty.SetTexture ( "_PositionBuffer", this.m_PositionBuffer2 );

		Graphics.DrawMesh (
			this.m_Mesh,
			transform.position,
			transform.rotation,
			this.m_Material,
			0,
			null,
			0,
			this.m_MaterialProperty
		);
	}

	void OnDestroy ()
	{
		if ( this.m_PositionBuffer1 )
		{
			DestroyImmediate ( this.m_PositionBuffer1 );
		}
		if ( this.m_PositionBuffer2 )
		{
			DestroyImmediate ( this.m_PositionBuffer2 );
		}

		if ( this.m_VelocityBuffer1 )
		{
			DestroyImmediate ( this.m_VelocityBuffer1 );
		}
		if ( this.m_VelocityBuffer2 )
		{
			DestroyImmediate ( this.m_VelocityBuffer2 );
		}

		if ( this.m_MaterialKernel )
		{
			DestroyImmediate ( this.m_MaterialKernel );
		}
	}
	#endregion
}
