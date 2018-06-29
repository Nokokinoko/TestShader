using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileUpManager : MonoBehaviour
{
	private const string NAME_SHADER = "Custom/SnowShader";
	private const float TIME_DELAY = 5.0f;
	private const float MAX_PILE_UP = 2.0f;

	public GameObject[] m_PileUp_r;
	private List<Material> m_ListMaterial = new List<Material> ();
	private int m_IdSnow;

	private float m_Time = 0.0f;
	private float m_PileUp = 0.0f;

	void Start ()
	{
		foreach ( GameObject obj in m_PileUp_r )
		{
			Material _Material = obj.GetComponent<Renderer> ().material;
			if ( _Material != null )
			{
				if ( _Material.shader.name == NAME_SHADER )
				{
					m_ListMaterial.Add ( _Material );
				}
			}
		}

		m_IdSnow = Shader.PropertyToID ( "_Snow" );
	}

	void Update ()
	{
		if ( MAX_PILE_UP <= m_PileUp )
		{
			return;
		}

		m_Time += Time.deltaTime;
		if ( TIME_DELAY < m_Time )
		{
			float _PileUp = Mathf.FloorToInt ( ( m_Time - TIME_DELAY ) * 0.2f * 100.0f ) * 0.01f;
			if ( m_PileUp != _PileUp )
			{
				m_PileUp = ( MAX_PILE_UP < _PileUp ) ? MAX_PILE_UP : _PileUp;

				foreach ( Material material in m_ListMaterial )
				{
					material.SetFloat ( m_IdSnow, m_PileUp );
				}
			}
		}
	}
}
