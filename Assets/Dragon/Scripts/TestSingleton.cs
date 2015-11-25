using UnityEngine;
using System.Collections;

public class TestSingleton : MonoBehaviour 
{
	#region Singleton Constructors
	static TestSingleton()
	{
	}
	
	TestSingleton()
	{
	}
	
	public static TestSingleton Instance 
	{
		get 
		{
			if (_instance == null) 
			{
				_instance = new GameObject ("TestSingleton").AddComponent<TestSingleton>();
			}
			
			return _instance;
		}
	}
	#endregion

	private static TestSingleton _instance = null;

	public float testVar = 1;

	void Update ()
	{
	
	}
}
