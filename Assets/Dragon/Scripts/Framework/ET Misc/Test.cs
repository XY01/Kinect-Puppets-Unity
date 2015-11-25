using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour 
{
	public int m_Sides = 4;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos()
	{
		for( int i = 0; i < m_Sides; i++ )
		{			
			if( i == m_Sides - 1 )
				Gizmos.DrawLine( ET_Utils.GetTransformPointAroundCircle( transform, i, m_Sides ) ,  ET_Utils.GetTransformPointAroundCircle( transform, 0, m_Sides ) );
			else
				Gizmos.DrawLine(ET_Utils.GetTransformPointAroundCircle( transform, i, m_Sides ),  ET_Utils.GetTransformPointAroundCircle( transform, i + 1, m_Sides ) );
		}
	}
}
