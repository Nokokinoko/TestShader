using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRecorder : MonoBehaviour
{
	public int m_FrameRate = 60;
	public int m_Size;
	public bool m_IsAuto;

	private int m_Frame = -1;
	private bool m_IsRecording = false;

	void Start ()
	{
		if ( m_IsAuto )
		{
			DoRecord ();
		}
	}

	private void DoRecord ()
	{
		System.IO.Directory.CreateDirectory ( "Capture" );
		Time.captureFramerate = m_FrameRate;
		m_Frame = -1;
		m_IsRecording = true;
	}

	void Update ()
	{
		if ( m_IsRecording )
		{
			if ( Input.GetMouseButtonDown ( 0 ) )
			{
				m_IsRecording = false;
				enabled = false;
			}
			else
			{
				if ( 0 < m_Frame )
				{
					string path = "Capture/frame" + m_Frame.ToString ( "0000" ) + ".png";
					ScreenCapture.CaptureScreenshot ( path, m_Size );
				}

				m_Frame++;

				if ( 0 < m_Frame && m_Frame % 60 == 0 )
				{
					Debug.Log ( ( m_Frame / 60 ).ToString () + " seconds elapsed." );
				}
			}
		}
	}

	void OnGUI ()
	{
		if ( !m_IsRecording && GUI.Button ( new Rect ( 0, 0, 200, 50 ), "Rec" ) )
		{
			DoRecord ();
		}
	}
}
