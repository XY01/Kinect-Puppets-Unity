using UnityEngine;
using System.Collections;


public class ET_Timing : ET_ManagerBase
{
	static 			ET_Timing m_Instance { get; set; }
	public static 	ET_Timing Instance{ get { return m_Instance; } }
	
	/// <summary>
	/// TODO:
	///  - Copy and comment master speed controller
	///  - Integrate BPM
	/// </summary>
	
	void Start ()
	{
		base.Start();
	}	
	
	void Update ()
	{
	
	}
	
	protected override void DrawGUIWindow()
	{
		
	}
}
