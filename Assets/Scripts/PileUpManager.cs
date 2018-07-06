using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileUpManager : MonoBehaviour
{
	private const string NAME_SHADER = "Custom/SnowShader";
	private const float TIME_DELAY = 5.0f;

	public GameObject[] m_PileUp_r;
	private List<Material> m_ListMaterial = new List<Material> ();
	private int m_IdSnow = -1;

	private float m_Time = 0.0f;
	[HideInInspector]
	public float m_PileUp = 0.0f;

	void Start ()
	{
		resetListMaterial ();
		m_Time = 0.0f;
		m_PileUp = 0.0f;
	}

	void Update ()
	{
		if ( getMaxPileUp () <= m_PileUp )
		{
			return;
		}

		m_Time += Time.deltaTime;
		if ( TIME_DELAY < m_Time )
		{
			float _PileUp = Mathf.FloorToInt ( ( m_Time - TIME_DELAY ) * 0.2f * 100.0f ) * 0.01f;
			if ( m_PileUp != _PileUp )
			{
				m_PileUp = ( getMaxPileUp () < _PileUp ) ? getMaxPileUp () : _PileUp;
				DoPileUp ();
			}
		}
	}

	public float getMaxPileUp ()
	{
		return 2.0f;
	}

	public void resetListMaterial ()
	{
		m_ListMaterial.Clear ();

		foreach ( GameObject obj in m_PileUp_r )
		{
			Material _Material = obj.GetComponent<Renderer> ().sharedMaterial;
			if ( _Material != null )
			{
				if ( _Material.shader.name == NAME_SHADER )
				{
					m_ListMaterial.Add ( _Material );
				}
			}
		}
	}

	private int getPropertySnow ()
	{
		if ( m_IdSnow < 0 )
		{
			m_IdSnow = Shader.PropertyToID ( "_Snow" );
		}
		return m_IdSnow;
	}

	public void DoPileUp ()
	{
		foreach ( Material material in m_ListMaterial )
		{
			material.SetFloat ( getPropertySnow (), m_PileUp );
		}
	}
}
