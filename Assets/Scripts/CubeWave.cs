using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CubeWave : MonoBehaviour
{
	#region Property
	[SerializeField]
	private Shader m_ShaderKernel;
	[SerializeField]
	private Mesh[] m_Shape_r = new Mesh[1];
	[SerializeField]
	private Material m_Material;
	[SerializeField]
	private ShadowCastingMode m_CastShadow;
	[SerializeField]
	private bool m_IsReceiveShadow = false;

	private RenderTexture m_PositionBuffer1;
	private RenderTexture m_PositionBuffer2;

	private Material m_MaterialKernel;
	private MaterialPropertyBlock m_MaterialProperty;
	private BulkMesh m_BulkMesh;

	private bool m_IsNeedReset = true;
	#endregion

	private RenderTexture CreateBuffer ()
	{
		int _Width = this.m_BulkMesh.CountCopy;
		int _Height = 320;

		RenderTexture _Buffer = new RenderTexture ( _Width, _Height, 0, RenderTextureFormat.ARGBFloat );
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

	private void ResetResource ()
	{
		if ( this.m_BulkMesh == null )
		{
			this.m_BulkMesh = new BulkMesh ( this.m_Shape_r, 320 );
		}
		else
		{
			this.m_BulkMesh.Rebuild ( this.m_Shape_r );
		}

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
	}

	private void SwapBufferAndInvokeKernel ()
	{
		RenderTexture _PositionTmp = this.m_PositionBuffer1;
		this.m_PositionBuffer1 = this.m_PositionBuffer2;
		this.m_PositionBuffer2 = _PositionTmp;

		this.m_MaterialKernel.SetTexture ( "_PositionBuffer", this.m_PositionBuffer1 );
		Graphics.Blit ( null, this.m_PositionBuffer2, this.m_MaterialKernel, 1 );
	}

	#region UnityEngine
	void Update ()
	{
		if ( m_IsNeedReset )
		{
			ResetResource ();
		}

		this.SwapBufferAndInvokeKernel ();

		if ( this.m_MaterialProperty == null )
		{
			this.m_MaterialProperty = new MaterialPropertyBlock ();
		}
		this.m_MaterialProperty.SetTexture ( "_PositionBuffer", this.m_PositionBuffer2 );

		Mesh _Mesh = this.m_BulkMesh.Mesh;
		Vector3 _Position = transform.position;
		Quaternion _Rotation = transform.rotation;
		Material _Material = this.m_Material;
		Vector2 _UV = new Vector2 ( 0.5f / this.m_PositionBuffer2.width, 0 );
		for ( int i = 0 ; i < this.m_PositionBuffer2.height ; i++ )
		{
			_UV.y = ( 0.5f + i ) / this.m_PositionBuffer2.height;
			this.m_MaterialProperty.SetVector ( "_BufferOffset", _UV );
			Graphics.DrawMesh ( _Mesh, _Position, _Rotation, _Material, 0, null, 0, this.m_MaterialProperty, this.m_CastShadow, this.m_IsReceiveShadow );
		}
	}

	void OnDestroy ()
	{
		if ( this.m_BulkMesh != null )
		{
			this.m_BulkMesh.Release ();
		}

		if ( this.m_PositionBuffer1 )
		{
			DestroyImmediate ( this.m_PositionBuffer1 );
		}
		if ( this.m_PositionBuffer2 )
		{
			DestroyImmediate ( this.m_PositionBuffer2 );
		}

		if ( this.m_Material )
		{
			DestroyImmediate ( this.m_Material );
		}
	}
	#endregion
}
