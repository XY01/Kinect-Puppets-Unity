using UnityEngine;
using System.Collections;

/* Thanks you for downloading this asset. We've tried to make this as similar to use as the PlayerPrefs
 * already in Unity, thus all you need to do to access it is use "PlayerPrefsPlus" instead of "PlayerPrefs";
 * you then have the ability to save and retreive any of the following data types.
 * - bool
 * - Color
 * - Vector2
 * - Vector3
 * - Vector4
 * - Quaternion
 * 
 * We hope this is as simple to use as we'd like and, whilst we'll continually update this with more types,
 * if you have any suggestions as to what we should add or find any problems you can reach us here:
 * 		assets@ninjapokestudios.net76.net
 * 
 * Thanks again,
 * -The NinjaPoke Studios team
*/

//	Copyright NinjaPoke Studios, You can change things but please don't redistrubute in any shape or form 
//	because we lose out :(

/* As a (rather pointless) example:
 * public class Test : MonoBehaviour{
 * 		void Start(){
 * 			PlayerPrefsPlus.SetBool("TestBoolean",true);
 * 		}
 * 
 * 		void Update(){
 * 			print( PlayerPrefsPlus.GetBool("TestBoolean") );
 * 		}
 * }
*/ 

public class PlayerPrefsPlus : MonoBehaviour {
	
	//############################################## bool ##############################################
	
	//Store bool as 0 or 1
	public static void SetBool(string key, bool value){
		if( value )
			PlayerPrefs.SetInt("PlayerPrefsPlus:bool:"+key,1);
		else
			PlayerPrefs.SetInt("PlayerPrefsPlus:bool:"+key,0);
	}
	
	public static bool GetBool(string key){
		return GetBool(key,false);
	}
	
	public static bool GetBool(string key, bool defaultValue){
		int value = PlayerPrefs.GetInt("PlayerPrefsPlus:bool:"+key, 2);
		if( value == 2 )		//Return default
			return defaultValue;
		else if( value == 1 )	//Return true
			return true;
		else					//Return false
			return false;
	}
	
	//############################################## Color ##############################################
	
	//Store Color data as RGBA floats
	public static void SetColour(string key, Color value){
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Colour:"+key+"-r",value.r);
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Colour:"+key+"-g",value.g);
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Colour:"+key+"-b",value.b);
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Colour:"+key+"-a",value.a);
	}
	
	//Rebuild Color data from RGBA floats
	public static Color GetColour(string key){
		return GetColour(key,Color.clear);
	}
	
	public static Color GetColour(string key, Color defaultValue){
		Color returnValue;
		returnValue.r = PlayerPrefs.GetFloat("PlayerPrefsPlus:Colour:"+key+"-r",defaultValue.r);
		returnValue.g = PlayerPrefs.GetFloat("PlayerPrefsPlus:Colour:"+key+"-g",defaultValue.g);
		returnValue.b = PlayerPrefs.GetFloat("PlayerPrefsPlus:Colour:"+key+"-b",defaultValue.b);
		returnValue.a = PlayerPrefs.GetFloat("PlayerPrefsPlus:Colour:"+key+"-a",defaultValue.a);
		return returnValue;
	}
	
	//############################################# Vector2 #############################################
	
	//Store Vector2 data as as x & y floats
	public static void SetVector2(string key, Vector2 value){
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Vector2:"+key+"-x",value.x);
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Vector2:"+key+"-y",value.y);
	}
	
	//Rebuild Vector2 from floats
	public static Vector2 GetVector2(string key){
		return GetVector2(key,Vector2.zero);
	}
	
	public static Vector2 GetVector2(string key, Vector2 defaultValue){
		Vector2 returnValue;
		returnValue.x = PlayerPrefs.GetFloat("PlayerPrefsPlus:Vector2:"+key+"-x",defaultValue.x);
		returnValue.y = PlayerPrefs.GetFloat("PlayerPrefsPlus:Vector2:"+key+"-y",defaultValue.y);
		return returnValue;
	}
	
	//############################################# Vector3 #############################################
	
	//Store Vector3 data as as x, y & z floats
	public static void SetVector3(string key, Vector3 value){
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Vector3:"+key+"-x",value.x);
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Vector3:"+key+"-y",value.y);
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Vector3:"+key+"-z",value.z);
	}
	
	//Rebuild Vector3 from floats
	public static Vector3 GetVector3(string key){
		return GetVector3(key,Vector3.zero);
	}
	
	public static Vector3 GetVector3(string key, Vector3 defaultValue){
		Vector3 returnValue;
		returnValue.x = PlayerPrefs.GetFloat("PlayerPrefsPlus:Vector3:"+key+"-x",defaultValue.x);
		returnValue.y = PlayerPrefs.GetFloat("PlayerPrefsPlus:Vector3:"+key+"-y",defaultValue.y);
		returnValue.z = PlayerPrefs.GetFloat("PlayerPrefsPlus:Vector3:"+key+"-z",defaultValue.z);
		return returnValue;
	}
	
	//############################################# Vector4 #############################################
	
	//Store Vector4 data as as x, y, z & w floats
	public static void SetVector4(string key, Vector4 value){
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Vector4:"+key+"-x",value.x);
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Vector4:"+key+"-y",value.y);
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Vector4:"+key+"-z",value.z);
		PlayerPrefs.SetFloat("PlayerPrefsPlus:Vector4:"+key+"-w",value.w);
	}
	
	//Rebuild Vector4 from floats
	public static Vector4 GetVector4(string key){
		return GetVector4(key,Vector4.zero);
	}
	
	public static Vector4 GetVector4(string key, Vector4 defaultValue){
		Vector4 returnValue;
		returnValue.x = PlayerPrefs.GetFloat("PlayerPrefsPlus:Vector4:"+key+"-x",defaultValue.x);
		returnValue.y = PlayerPrefs.GetFloat("PlayerPrefsPlus:Vector4:"+key+"-y",defaultValue.y);
		returnValue.z = PlayerPrefs.GetFloat("PlayerPrefsPlus:Vector4:"+key+"-z",defaultValue.z);
		returnValue.w = PlayerPrefs.GetFloat("PlayerPrefsPlus:Vector4:"+key+"-w",defaultValue.w);
		return returnValue;
	}
	
	//############################################ Quaternion ############################################
	
	//For simplicity we are just going to put Quaternions into Vector3s with "Quaternion" before the key
	public static void SetQuaternion(string key, Quaternion value){
		SetVector3("Quaternion:"+key,value.eulerAngles);
	}
	
	public static Quaternion GetQuaternion(string key){
		return Quaternion.Euler( GetVector3("Quaternion:"+key,Quaternion.identity.eulerAngles) );
	}
	
	public static Quaternion GetQuaternion(string key, Quaternion defaultValue){
		return Quaternion.Euler( GetVector3("Quaternion:"+key,defaultValue.eulerAngles) );
	}
}