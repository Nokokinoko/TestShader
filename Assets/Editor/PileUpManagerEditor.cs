using UnityEditor;
using UnityEngine;

[CustomEditor ( typeof ( PileUpManager ) )]
public class PileUpManagerEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		PileUpManager _Mgr = (PileUpManager)target;

		if ( GUILayout.Button ( "Set Material" ) )
		{
			_Mgr.resetListMaterial ();
		}

		EditorGUI.BeginChangeCheck ();

		_Mgr.m_PileUp = EditorGUILayout.Slider ( "Pile Up", _Mgr.m_PileUp, 0.0f, _Mgr.getMaxPileUp () );

		if ( EditorGUI.EndChangeCheck () )
		{
			_Mgr.DoPileUp ();
		}
	}
}
