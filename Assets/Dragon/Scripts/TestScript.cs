using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour
{


	// Use this for initialization
	void Start ()
	{

		for (int i = 0; i < 100; i++)
		{
			//GameObject go = (GameObject)Instantiate( m_GO );
			//go.name = "" + i;

		}
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Profiler.BeginSample ("Static");
		float staticVal = 0;
		for (int i = 0; i < 10000; i++)
		{
			staticVal += TestStaticNew.testVar;
		}
		Profiler.EndSample ();

		Profiler.BeginSample ("Singelton");
		float singeltonVal = 0;
		for (int i = 0; i < 10000; i++)
		{
			singeltonVal += TestSingleton.Instance.testVar;
		}
		Profiler.EndSample ();
	
	}
}
