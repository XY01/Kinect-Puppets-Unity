using UnityEngine;
using System;
using System.Collections.Generic;
//using LibNoise;

public class Utils
{	
	static int m_RegisteredWindowCount;
	
	public static int GetWindowIndex()
	{
		m_RegisteredWindowCount++;
		//Debug.Log( "Window index added " + m_RegisteredWindowCount );
		return m_RegisteredWindowCount;
	}
	
	public static bool IsGizmoDrawingAllowed()
	{
		// notes from paul:
		//
		// It is expected that Monobehaviour.OnDrawGizmos() is called within the
		// Unity editor when the game is not running.
		//
		// When running the game inside the Unity editor I have found that
		// OnDrawGizmos() can be called for a few frames at the start of the game
		// and for a few frames at the end of the game. This small overlap can
		// cause problems referencing game assets - for example, when a single
		// variable is used to reference an asset for a gizmo and also for runtime.
		//
		// For this reason, IsGizmoDrawingAllowed() should be called inside
		// OnDrawGizmos() to determine if the game is really running.
		//
		// At the moment this function uses values from UnityEngine.Time to make
		// this determination - rules I developed through trial and error.
		// This function is known to work with Unity V3.0.0 and the code may need
		// to be revisited if Unity is upgraded.


		//Debug.Log("Utils.IsGizmoDrawingAllowed() " + Time.time + " " + Time.renderedFrameCount);

		if (Time.renderedFrameCount <= 4)
			return false;	// game is still starting up

		if (Time.time > 0.0f)
			return false;	// game is still shutting down

		return true;
	}
	
	public static Transform PlaceAndRotateAndScaleBetweenTwoTransforms( Transform t1, Transform t2, Transform transformToPlace, float normalizedPosAlongVector )
	{
		Vector3 placementVector = t2.position - t1.position;
		transformToPlace.position = t1.position + ( placementVector * normalizedPosAlongVector );
			
		transformToPlace.LookAt( t1.position );
		transformToPlace.RotateAroundLocal( transformToPlace.TransformDirection( Vector3.up ), -90 * Mathf.Deg2Rad );
		
		transformToPlace.localScale = Vector3.one * placementVector.magnitude;
		
		return transformToPlace;
	}
	
	public static Vector3 NormalizedPositionBetweenTwoPoints( Vector3 pos1, Vector3 pos2, float normalizedPosAlongVector )
	{
		Vector3 pos = Vector3.zero;
		Vector3 placementVector = pos2 - pos1;
		pos = pos1 + ( placementVector * normalizedPosAlongVector );
			
		return pos;
	}
	
	
	
	
	public static float WrapFloat(float f, float min, float max)
	{
		float result = f;
		float difference = max - min;
		
		while(result < min || result > max)
		{		
			if(result < min)
				result += difference;
			else if(result > max)
				result -= difference;
		}
		
		return result;
	}
	
	
	public static int WrapInt(int i, int min, int max)
	{
		int result = i;
		
		//while(result < min || result > max)
		//{		
			if(result < min)
				result = max;
			else if(result > max)
				result = min;
		//}
		
		return result;
	}
	
	public static float RangedToNormalized( float val, float min, float max )
	{		
		float range = max - min;
		if( range == 0 )
			return 0;
		else
			return Mathf.Clamp01( ( val - min ) / range );
	}
	
	public static float RangedToUnitLength( float val, float min, float max ) // name might be wrong
	{
		bool isNegative = false;
		if( val < 0 )	isNegative = true;
		
		float range = max - min;
		if( range == 0 )
		{
			return 0;
		}
		else
		{
			val = Mathf.Clamp01( ( Mathf.Abs(val) - min ) / range );
			if( isNegative ) val = -val;
			
			return val;
		}
	}
	
	public static float NormalizedToRange( float normaliedVal, float min, float max )
	{
		float val = min + ( (max - min) * normaliedVal );
		val = Mathf.Clamp( val, min, max );
		return val;
	}
	
	public static float NormalizedToRangeWithClamp( float normaliedVal, float min, float max )
	{
		float val = min + ( (max - min) * normaliedVal );
		return Mathf.Clamp( val, min, max );
	}
	
	
	public static Vector3 VectorMaxMagnitude( Vector3 vec, float maxMagnitude )
    {
        if ( vec.magnitude > maxMagnitude ) vec = vec.normalized * maxMagnitude;
        return vec;
    }
	
	public static Vector3 RoundVector3ToOneDecimalPlace( Vector3 vec3 )
	{
		vec3.x = RoundToOneDecimalPlace( vec3.x );
		vec3.y = RoundToOneDecimalPlace( vec3.y );
		vec3.z = RoundToOneDecimalPlace( vec3.z );
		
		return vec3;
	}
	
	public static float RoundToOneDecimalPlace( float val )
	{
		return Mathf.Round( val * 10) / 10;
	}
	
	public static float RoundToTwoDecimalPlaces( float val )
	{
		return Mathf.Round( val * 100) / 100;
	}
	
	public static Vector3 DivideVectors(Vector3 v1, Vector3 v2)
    {
        Vector3 newVector = new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        return newVector;
    }
	
	public static Vector2 CartesianToPolar( Vector3 point )
	{
	    Vector2 polar;
	
	    //calc longitude
	    polar.y = Mathf.Atan2(point.x,point.z);
	
	    //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
	    float xzLen = new Vector2(point.x,point.z).magnitude; 
	    //atan2 does the magic
	    polar.x = Mathf.Atan2(-point.y,xzLen);
	
	    //convert to deg
	    polar *= Mathf.Rad2Deg;
	
	    return polar;
	}
	
	public static Vector3 CartesianToUnitSphere( Vector3 point )
	{
	    Vector2 polar;
	
	    //calc longitude
	    polar.y = Mathf.Atan2(point.x,point.z);
	
	    //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
	    float xzLen = new Vector2(point.x,point.z).magnitude; 
	    //atan2 does the magic
	    polar.x = Mathf.Atan2(-point.y,xzLen);
	
	    //convert to deg
	    polar *= Mathf.Rad2Deg;
	
	    Quaternion rotation = Quaternion.Euler( polar.x * 2, polar.y, 0);
		
		return  rotation * Vector3.up ;
	}


	public static Vector3 PolarToCartesian( Vector2 polar )
	{
	
	    //an origin vector, representing lat,lon of 0,0. 
	
	    Vector3 origin= new Vector3(0,0,1);
	    //build a quaternion using euler angles for lat,lon
	    Quaternion rotation = Quaternion.Euler(polar.x*2,polar.y,0);
	    //transform our reference vector by the rotation. Easy-peasy!
	    Vector3 point=rotation*origin;
	
	    return point;
	}
	/*
	float FlameNoise( Vector3 p ) //Thx to Las^Mercury
	{
		Vector3 i = floor(p);
		vec4 a = dot(i, vec3(1., 57., 21.)) + vec4(0., 57., 21., 78.);
		vec3 f = cos((p-i)*acos(-1.))*(-.5)+.5;
		a = mix(sin(cos(a)*a),sin(cos(1.+a)*(1.+a)), f.x);
		a.xy = mix(a.xz, a.yw, f.y);
		return mix(a.x, a.y, f.z);
	}
	*/
	static public float WaveFunc(float radius, float t, float amp, float waveLen, float phase, float decay)
	{
		if ( waveLen == 0.0f )
			waveLen = 0.0000001f;

		float ang = Mathf.PI * 2.0f * (radius / waveLen + phase);
		return amp * Mathf.Sin(ang) * Mathf.Exp(-decay * Mathf.Abs(radius));
	}
	
	public static Vector3 RandomVector( float xRange, float yRange, float zRange )
	{
		Vector3 randVec = new Vector3 ( UnityEngine.Random.Range( -xRange/2,xRange/2 ), UnityEngine.Random.Range( -yRange/2, yRange/2), UnityEngine.Random.Range( -zRange/2, zRange/2) ) ;
		return randVec;
	}
	
	public static Vector3 RandomVector( Vector3 vec3 )
	{
		Vector3 randVec = new Vector3 ( UnityEngine.Random.Range( -vec3.x/2,vec3.x/2 ), UnityEngine.Random.Range( -vec3.y/2, vec3.y/2), UnityEngine.Random.Range( -vec3.z/2, vec3.z/2) ) ;
		return randVec;
	}

	static public Mesh GetMesh(GameObject go)
	{
		if ( !Application.isPlaying )
			return GetSharedMesh(go);
		//return GetSharedMesh(go);

		MeshFilter meshFilter = (MeshFilter)go.GetComponent(typeof(MeshFilter));

		//	Mesh mesh;
		if ( meshFilter != null )
			return meshFilter.mesh;	//sharedMesh;	//sharedMesh;
		else
		{
			SkinnedMeshRenderer smesh = (SkinnedMeshRenderer)go.GetComponent(typeof(SkinnedMeshRenderer));

			if ( smesh != null )
				return smesh.sharedMesh;
		}

		return null;
	}
	
	static Vector3 perlAtPos;
	
	
	public static Vector3 GetPerlinAtWorldPos(Vector3 pos, float perlOffset)
	{
		pos.x = Mathf.Abs( pos.x );
		pos.y = Mathf.Abs( pos.y );
		pos.z = Mathf.Abs( pos.z );
		
		perlAtPos = new Vector3(
			Mathf.PerlinNoise( pos.y + perlOffset, pos.z + perlOffset) - .5f ,
			Mathf.PerlinNoise( pos.x + perlOffset, pos.z + perlOffset) - .5f,
			Mathf.PerlinNoise( pos.x + perlOffset, pos.y + perlOffset) - .5f);
		
		return perlAtPos;
	}
	
	static public Mesh GetSharedMesh(GameObject go)
	{
		//if ( Application.isPlaying )
			//return GetMesh(go);

		MeshFilter meshFilter = (MeshFilter)go.GetComponent(typeof(MeshFilter));

		//	Mesh mesh;
		if ( meshFilter != null )
			return meshFilter.sharedMesh;
		else
		{
			SkinnedMeshRenderer smesh = (SkinnedMeshRenderer)go.GetComponent(typeof(SkinnedMeshRenderer));

			if ( smesh != null )
				return smesh.sharedMesh;
		}

		return null;
	}

	//static UserInterfaceManager uiManager = null;
	public static bool IsCalledOutsideInit()
	{
		// For debug purposes.
		// We can use this function to identify if some slow 'setup' functionality, such
		// as Utils.FindObject(), is being called unexpectedly during normal gameplay.

		if (Time.time == 0.0f)
			return false;	// game is not running - must have got here via OnDrawGizmos()

		return false;	// JasonF temporary - until we have a UI manager or something
		//if (uiManager == null)
		//{
		//    // first call
		//    uiManager = GameObject.Find("UserInterface").GetComponent<UserInterfaceManager>();
		//    if (uiManager == null)
		//    {
		//        Debug.LogError("uh-oh");
		//        return true;
		//    }
		//}

		//return !uiManager.IsInitInProgress();
	}
	
	
	
	public static Rect ClampRectTo(Rect rect, float widthBounds, float heightBounds)
	{
		rect.x = Mathf.Clamp(rect.x,0, widthBounds - rect.width);
		
		rect.y = Mathf.Clamp(rect.y,0, heightBounds - rect.height);
		
		return rect;
	}
	

	//Finds a Transform among the children of the specified parent:
	public static Transform FindTransform(string objName, GameObject parentObj)
    {
		if (IsCalledOutsideInit())
			Debug.LogWarning("FindTransform(" + objName + ") called outside init");

        if (parentObj == null)
			return null;

        foreach (Transform trans in parentObj.transform)
        {
            if (trans.name == objName)
            {
                return trans;
            }
            Transform foundTransform = FindTransform(objName, trans.gameObject);
            if (foundTransform != null)
            {
                return foundTransform;
            }
        }

        return null;
    }
	
	
	public static GUILayoutOption[] SetHeightWidth(float height, float width)
	{
		GUILayoutOption[] layoutOptions = new GUILayoutOption[4];
		
		layoutOptions[0] = GUILayout.ExpandHeight(false);
		layoutOptions[1] = GUILayout.ExpandWidth(false);
			
		//layoutOptions[2] = GUILayout.MinWidth(width-1);
		layoutOptions[2] = GUILayout.MaxWidth(width);
				
		//layoutOptions[4] = GUILayout.MinHeight(height - 1);
		layoutOptions[3] = GUILayout.MaxHeight(height);
		
		return layoutOptions;
	}
	
	public static GUILayoutOption[] SetMinMaxWidthHeight(float width, float height)
	{
		GUILayoutOption[] layoutOptions = new GUILayoutOption[6];
		
		layoutOptions[0] = GUILayout.ExpandHeight(false);
		layoutOptions[1] = GUILayout.ExpandWidth(false);
			
		layoutOptions[2] = GUILayout.MinWidth(width-1);
		layoutOptions[3] = GUILayout.MaxWidth(width);
				
		layoutOptions[4] = GUILayout.MinHeight(height - 1);
		layoutOptions[5] = GUILayout.MaxHeight(height);
		
		return layoutOptions;
	}
	
	public static GUILayoutOption[] SetHeight(float height)
	{
		GUILayoutOption[] layoutOptions = new GUILayoutOption[4];
		
		layoutOptions[0] = GUILayout.ExpandHeight(false);
		layoutOptions[1] = GUILayout.Height(height);
		layoutOptions[2] = GUILayout.MinHeight(height);
		layoutOptions[3] = GUILayout.MaxHeight(height);
		
		return layoutOptions;
	}
	
	public static GUILayoutOption[] SetWidth(float width)
	{
		GUILayoutOption[] layoutOptions = new GUILayoutOption[4];
		
		layoutOptions[0] = GUILayout.ExpandWidth(false);
		layoutOptions[1] = GUILayout.Width(width);
		layoutOptions[2] = GUILayout.MinWidth(width);
		layoutOptions[3] = GUILayout.MaxWidth(width);
		
		return layoutOptions;
	}
	
	
	public static float BPMToBPS(float BPM)
	{
		return BPM/60;
	}
	
	
    //Finds a GameObject among the children of the specified parent:
    public static GameObject FindObject(string objName, GameObject parentObj)
    {
		if (IsCalledOutsideInit())
			Debug.LogWarning("FindObject(" + objName + ") called outside init");

		if (parentObj == null)
			return null;

        foreach (Transform trans in parentObj.transform)
        {
            if (trans.name == objName)
            {
                return trans.gameObject;
            }
            GameObject foundObject = FindObject(objName, trans.gameObject);
            if (foundObject != null)
            {
                return foundObject;
            }
        }

        return null;
    }
	
	public static void SetTransform(GameObject gO, Transform tform)
	{
		gO.transform.position = tform.position;
		gO.transform.rotation = tform.rotation;
		gO.transform.localScale = tform.localScale;
	}
	
	/*
	public static void SetTransformParentAndZero(GameObject gO, Transform parent)
	{
		gO.transform.parent = parent;
		
		gO.transform.localPosition = Vector3.zero;
		gO.transform.localRotation = Quaternion.identity;
		gO.transform.localScale = Vector3.one;
	}
	 */
	 
    //Finds a component among the children of the specified parent:
    public static T FindComponent<T>(string objName, GameObject parentObj) where T : Component
    {
		if (IsCalledOutsideInit())
			Debug.LogWarning("FindComponent(" + objName + ") called outside init");
		
		if (parentObj == null)
			return null;

        foreach (Transform trans in parentObj.transform)
        {
            if (trans.name == objName)
            {
                return trans.gameObject.GetComponent<T>();
            }
            T foundComponent = FindComponent<T>(objName, trans.gameObject);
            if (foundComponent != null)
            {
                return foundComponent;
            }
        }

        return null;
    }
	
	public static Color GetMatCol(Material mat)
	{
		if (mat.HasProperty("_TintColor"))
  			return mat.GetColor("_TintColor");
		else if(mat.HasProperty("_Color"))
  			return mat.GetColor("_Color");
		else if(mat.HasProperty("_MainColor"))
  			return mat.GetColor("_MainColor");
		
		return Color.white;
	}
	
	public static void ApplyMatCol(Material mat, Color col)
	{
		if( mat == null ) return;
		
		if (mat.HasProperty("_TintColor"))	mat.SetColor("_TintColor", col);
		else if(mat.HasProperty("_Color"))	mat.SetColor("_Color", col);
	}

	//Get component of a specified game object name
	static Dictionary<string, UnityEngine.Object> componentCache1 = new Dictionary<string, UnityEngine.Object>();
	public static T GetComponent<T>(string objectName) where T : Component
	{
		string componentName = typeof(T).ToString();
		string lookupName = objectName + "." + componentName;

		if (IsCalledOutsideInit())
			Debug.LogWarning("GetComponent(" + lookupName + ") called outside init");

		// faster to extract component from cache if component has been found before
		UnityEngine.Object cachedObject;
		if (componentCache1.TryGetValue(lookupName, out cachedObject))
		{
			if (cachedObject != null)
			{
				//Debug.Log("found " + lookupName + " in component cache");
				return (T)cachedObject;
			}

			Debug.LogError("ERROR: " + lookupName + " = null in component cache");
		}

		// slow look-up of game object (assumes there is only one game object called objectName)
		GameObject gameObject = GameObject.Find(objectName);

		if (gameObject == null)
		{
			Debug.LogError("ERROR: no gameObject with name " + objectName);
			return null;
		}

		// look-up all components of this type (expecting only one)
		T[] components = gameObject.GetComponents<T>();

		if (components.Length == 0)
		{
			Debug.LogError("ERROR: gameObject " + objectName + " doesn't have " + componentName + " component");
			return null;
		}

		if (components.Length > 1)
		{
			Debug.LogError("ERROR: gameObject " + objectName + " contains " + components.Length + " " + componentName + " components");
			return components[0];	// return first component without adding to component cache
		}

		// add to component cache for faster look-up on subsequent calls
		componentCache1.Add(lookupName, components[0]);

		//Debug.Log("found " + lookupName + " component");	// should only see this once for each lookupName

		return components[0];
	}
	
	public static bool ToggleBool(bool b)
	{
		if(b)
			return false;
		else
			return true;
	}

	//Get component in the scene (assumes there is only one in existance)
	static Dictionary<string, UnityEngine.Object> componentCache2 = new Dictionary<string, UnityEngine.Object>();
	public static T GetComponent<T>() where T : Component
	{
		return GetComponent<T>(true);	// critical by default
	}
	public static T GetComponent<T>(bool critical) where T : Component
	{
		string componentName = typeof(T).ToString();

		if (IsCalledOutsideInit())
			Debug.LogWarning("GetComponent(" + componentName + ") called outside init");

		// faster to extract component from cache if component has been found before
		UnityEngine.Object cachedObject;
		if (componentCache2.TryGetValue(componentName, out cachedObject))
		{
			if (cachedObject != null)
			{
				//Debug.Log("found " + componentName + " in component cache");
				return (T)cachedObject;
			}

			Debug.LogError("ERROR: " + componentName + " = null in component cache");
		}

		// slow look-up of all components of this type (expecting only one)
		T[] components = (T[])UnityEngine.Object.FindObjectsOfType(typeof(T));

		if (components.Length == 0)
		{
			if (critical)
				Debug.LogError("ERROR: could not find component " + componentName);
			return null;
		}

		if (components.Length > 1)
		{
			string list = "";
			foreach (T component in components)
				list += " " + component.name;
			Debug.LogError("ERROR: " + componentName + " component found in multiple game objects:" + list);
			return components[0];	// return first component without adding to component cache
		}

		// add to component cache for faster look-up on subsequent calls
		componentCache2.Add(componentName, components[0]);

		//Debug.Log("found " + componentName + " component");	// should only see this once for each componentName

		return components[0];
	}
	
	public static int LoopIncrement(int index, int min, int max)
	{
		index++;
		if(index > max)
			return min;
		else
			return index;
	}
	
	public static int LoopDecrement(int index, int min, int max)
	{
		index--;
		if(index < min)
			return max;
		else
			return index;
	}
    
    public static List<GameObject> FindAllChildren(GameObject gO, int maxDepth)
    {        
        List<GameObject> children = new List<GameObject>();
        List<GameObject> uncheckedChildren = new List<GameObject>();
        bool FoundAllChildren = false;

        if (gO.transform.childCount != 0)
        {
            uncheckedChildren = GetChildList(gO);
            int depthCount = 0;

            while (uncheckedChildren.Count > 0 && depthCount < maxDepth)
            {
                List<GameObject> recentlyChecked = new List<GameObject>();
                List<GameObject> recentNewChildren = new List<GameObject>();
                int withoutChildren = 0;

                foreach (GameObject uncheckedGO in uncheckedChildren)
                {
                    List<GameObject> newChildren = new List<GameObject>();
                    newChildren = GetChildList(uncheckedGO);

                    if (newChildren.Count == 0)
                        withoutChildren++;
                    else
                    {
                        foreach (GameObject newChild in newChildren)
                            recentNewChildren.Add(newChild);
                    }

                    recentlyChecked.Add(uncheckedGO);
                }

                //Remove recently checked from uncehcked and add them to chilren
                foreach (GameObject gO1 in recentlyChecked)
                {
                    uncheckedChildren.Remove(gO1);
                    children.Add(gO1);
                }
                recentlyChecked.Clear();

                //Add recent new children to unchecked
                foreach (GameObject gO2 in recentNewChildren)                
                    uncheckedChildren.Add(gO2);

                recentNewChildren.Clear();

                depthCount++;
            }         
        }
        return children;        
    }


    public static List<GameObject> GetChildList(GameObject gO)
    {
         List<GameObject> children = new List<GameObject>();

         for (int i = 0; i < gO.transform.childCount; i++)
         {
             children.Add(gO.transform.GetChild(i).gameObject);
         }

         return children;
    }


	//static string lastLevelName;
	//static int lastLevelID;
	//public static int GetTrackIDLoaded()
	//{
	//    string levelName = Application.loadedLevelName;

	//    // a short cut
	//    if (levelName == lastLevelName)
	//        return lastLevelID;

	//    // currently there are less than 10 tracks and so simply look
	//    // at the final character of the level name
	//    int id;
	//    if (int.TryParse(levelName.Substring(levelName.Length - 1), out id))
	//    {
	//        lastLevelName = levelName;
	//        lastLevelID = id;
	//        return id;
	//    }

	//    lastLevelName = "";
	//    lastLevelID = -1;

	//    return -1;	// no track loaded
	//}


	public static float CalcSplineDistanceDiff(float dist0, float dist1, float totalSplineDist)
	{
		// returns the shortest distance between two spline locations represented by dist0 and dist1.
		// (dist0 and dist1 do not need to be normalised - they can represent any number of laps)

		float halfTotalDist = totalSplineDist * 0.5f;
		float diff = dist1 - dist0;

		while (diff < -halfTotalDist)
			diff += totalSplineDist;
		while (diff > halfTotalDist)
			diff -= totalSplineDist;

		return Mathf.Abs(diff);	// range is 0 -> halfTotalDist
	}


	static float EvalRamp01(float t, float factor)
	{
		// factor needs to be a positive value
		// - values above 1.0 create "smooth in" curve
		// - values below 1.0 create "smooth out" curve

		if (factor <= 0.0f || factor == 1.0f)
			return t;

		t = (1.0f / (1.0f - t + (t / factor))) - 1.0f;
		return t * (1.0f / (factor - 1.0f));
	}


	public static float Ramp01SmoothIn(float t, float rampScale)
	{
		// - the "smooth in" curve is flat at the start and then ramps up quickly at the end
		// - the higher the value of rampScale the more extreme the bend in the curve
		//   (zero rampScale means linear relationship)

		t = Mathf.Clamp01(t);	// expect t in range 0.0 -> 1.0
		
		if (rampScale <= 0.0f)
			return t;

		return EvalRamp01(t, rampScale + 1.0f);		// output range 0.0 -> 1.0
	}


	public static float Ramp01SmoothOut(float t, float rampScale)
	{
		// - the "smooth out" curve ramps up quickly at the start and then flattens out at the end
		// - the higher the value of rampScale the more extreme the bend in the curve
		//   (zero rampScale means linear relationship)

		t = Mathf.Clamp01(t);	// expect t in range 0.0 -> 1.0

		if (rampScale <= 0.0f)
			return t;

		return EvalRamp01(t, 1.0f / (rampScale + 1.0f));	// output range 0.0 -> 1.0
	}


	public static float Ramp01SmoothInOut(float t)
	{
		// currently the shape of this curve is not adjustable
		// this curve is flat at the start and also at the end (symmetrical)

		// 3t^2 - 2t^3
		t = Mathf.Clamp01(t);	// expect t in range 0.0 -> 1.0
		float sq = t * t;
		return (3*sq) - (2*sq*t);	// output range 0.0 -> 1.0
	}


	public static float StepTowards(float src, float targ, float step)
	{
		// hmm...  i think this is the same as Mathf.MoveTowards()

		if (step < 0.0f)
			step = -step;
		return src + Mathf.Clamp((targ - src), -step, step);
	}


	// alternative to Mathf.Lerp() - if value lies within epsilon of targ, then targ is returned
	public static float Lerp(float src, float targ, float t, float epsilon)
	{
		if (t <= 0.0f)
			return src;

		if (t >= 1.0f)
			return targ;

		float result = src + ((targ - src)*t);

		if (Mathf.Abs(targ - result) <= epsilon)
			return targ;

		return result;
	}
	
	// alternative to Mathf.Lerp() - if value lies within epsilon of targ, then targ is returned
	public static Vector3 LerpVector3( Vector3 vec, Vector3 targVec, float t, float epsilon)
	{
		vec.x = Lerp( vec.x, targVec.x, t, epsilon );
		vec.y = Lerp( vec.y, targVec.y, t, epsilon );
		vec.z = Lerp( vec.z, targVec.z, t, epsilon );
		
		return vec;
	}

	//public static bool IsOnTempo(int beatInterval)
	//{
	//    AudioOptions audioOptions = GetComponent<AudioOptions>();

	//    return audioOptions != null ? audioOptions.IsOnTempo(beatInterval) : false;
	//}

	//public static AudioOptions GetAudioOptions()
	//{
	//    GameObject audioOptionsGameObj = GameObject.Find("Audio Options");
	//    if (audioOptionsGameObj == null)
	//    {
	//        Debug.LogError("ERROR: no gameObject with tag 'Audio Options'");
	//    }
	//    else
	//    {
	//        audioOptions = audioOptionsGameObj.GetComponent<AudioOptions>();
	//        if (audioOptions == null)
	//        {
	//            Debug.LogError("ERROR: Audio Options gameObject doesn't have AudioOptions component");
	//        }

	//        return audioOptions;
	//    }

	//    return null;
	//}

	//public static CollisionSounds GetCollisionSounds()
	//{
	//    CollisionSounds collisionSounds = null;

	//    GameObject gameObj = GameObject.Find("CollisionSounds");

	//    if (gameObj == null)
	//    {
	//        Debug.LogError("ERROR: no gameObject called CollisionSounds");
	//    }
	//    else
	//    {
	//        collisionSounds = gameObj.GetComponent<CollisionSounds>();
	//        if (collisionSounds == null)
	//        {
	//            Debug.LogError("ERROR: ColisionSounds gameObject doesn't have CollisionSounds component");
	//        }
	//    }

	//    return collisionSounds;
	//}

    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        int childCount = obj.transform.GetChildCount();
        for (int i = 0; i < childCount; i++)
        {
            SetLayerRecursively(obj.transform.GetChild(i).gameObject, layer);
        }
    }


    public static float LerpNoClamp(float x, float x0, float x1, float y0, float y1)
	{
		float frac = (x - x0) / (x1 - x0);
		return y0 + frac * (y1 - y0);
	}


	public static float LerpWithClamp(float x, float x0, float x1, float y0, float y1)
	{
		if (x0 == x1 || y0 == y1)
			return y0;

		if (x0 < x1)
		{
			if (x <= x0)
				return y0;
			if (x >= x1)
				return y1;
		}
		else // x0 > x1
		{
			if (x >= x0)
				return y0;
			if (x <= x1)
				return y1;
		}

		float frac = (x - x0) / (x1 - x0);
		float y = y0 + frac*(y1 - y0);
		return Mathf.Clamp(y, Mathf.Min(y0, y1), Mathf.Max(y0, y1));
	}


	//public static string GenerateRaceTimeString(float raceTime)
	//{
	//    if (raceTime == RaceHandler.RaceTimeDNF)
	//    {
	//        return "--:--.--";
	//    }
	//    else if (raceTime <= 0.0f)
	//    {
	//        return "0:00.00";
	//    }
	//    else
	//    {
	//        int totalHundredths = (int)(raceTime * 100);
	//        int totalSeconds = totalHundredths / 100;
	//        int totalMinutes = totalSeconds / 60;

	//        if (totalMinutes >= 60)
	//            return "59:59.99";
	//        else
	//            return string.Format("{0:0}:{1:00}.{2:00}", totalMinutes, totalSeconds % 60, totalHundredths % 100);
	//    }
	//}


	public class SweptSpheresHitData
	{
		public float timeOfImpact;		// time when spheres first touched (0.0 -> 1.0)
		public Vector3 impactPos1;		// sphere1 position at time of impact
		public Vector3 impactPos2;		// sphere2 position at time of impact
	}


	public static bool CollideSweptSpheres(
		Vector3 start1, Vector3 end1, float radius1,	// sweeping sphere 1
		Vector3 start2, Vector3 end2, float radius2)	// sweeping sphere 2
	{
		return CollideSweptSpheres(start1, end1, radius1, start2, end2, radius2, null);
	}


	public static bool CollideSweptSpheres(
		Vector3 start1, Vector3 end1, float radius1,	// sweeping sphere 1
		Vector3 start2, Vector3 end2, float radius2,	// sweeping sphere 2
		SweptSpheresHitData hitData)
	{
		// Constant velocity assumed.

		Vector3 dca = start2 - start1;	// separation of sphere centre points at start positions
		float rsum = radius1 + radius2;

		// early-out test
		// if an upper bound can be placed on the speed of the spheres, then the collision test can
		// be skipped based on the initial separation and the size of the time step
		// (worst case: spheres are traveling along same line in opposing directions)
		//float limit = rsum + (MaxSphereSpeed * Time.deltaTime * 2);	// max separation for each dimension
		float limit = rsum + 25.0f;	// for now assume initial separation of 25 metres means no collision possible
		if (dca.x<-limit || dca.x>limit || dca.y<-limit || dca.y>limit || dca.z<-limit || dca.z>limit)
		{
			// no collision
			return false;
		}

		// since all dimensions are within 'limit', the max starting separation is sqrt(3*(limit^2))


		// check if spheres touch at start positions
		float mdc = dca.sqrMagnitude - (rsum * rsum);
		if (mdc <= 0.0f)
		{
			if (hitData != null)
			{
				if (mdc < 0.0f)		// if spheres are penetrating at start positions
				{
					// separate the spheres to avoid penetration

					// ***TODO***

					hitData.timeOfImpact = 0.0f;
					hitData.impactPos1 = start1;
					hitData.impactPos2 = start2;
				}
				else
				{
					hitData.timeOfImpact = 0.0f;
					hitData.impactPos1 = start1;
					hitData.impactPos2 = start2;
				}
			}
			return true;
		}


		Vector3 delta1 = end1 - start1;
		Vector3 delta2 = end2 - start2;
		Vector3 ddd = delta2 - delta1;

		float mda = ddd.sqrMagnitude;
		float mdb = Vector3.Dot(dca, ddd);

		// time when sphere centre points are closest to each other (not time
		// of impact) is defined by:
		//     tc = -mdb/mda
		//
		// check if tc lies outside range 0.0 -> 1.0
		{
			// check if tc<=0.0 (sphere centre points are closest at start positions) or if
			// spheres have identical position deltas (spheres are moving in parallel)
			if (mdb >= 0.0f || mda == 0.0f)	// we know mda>=0
			{
				// no collision since we know spheres weren't touching at start positions
				return false;
			}

			// check if tc>=1.0 (sphere centre points are closest at end positions)
			if (-mdb >= mda)	// we know mdb<0 and mda>0
			{
				// check if spheres touch at end positions
				Vector3 dbd = end2 - end1;	// separation at end positions
				if (dbd.sqrMagnitude > (rsum * rsum))
				{
					// no collision
					return false;
				}

				// sphere centre points are closest at end positions and spheres are
				// touching or penetrating at end positions
				// - this means time of impact occurred in range 0.0 -> 1.0
				if (hitData == null)
				{
					// if hit data isn't needed then no further calculations are required
					return true;
				}
			}
		}


		// solve quadratic to find the time when spheres first touched
		// a.t^2 + b.t + c = 0
		// where:
		//    a = ddd*ddd
		//    b = 2*dca*ddd
		//    c = dca*dca - (r1+r2)^2

		mdb *= 2;
		float qu = (mdb * mdb) - (4 * mda * mdc);	// b^2-4ac
		if (qu < 0.0f)		// if no real solution to quadratic formula
			return false;	// no collision (spheres were closest at 0.0<t<1.0, but did not touch)

		if (hitData != null)
		{
			// calculate time of impact
			// choose -sqrt because we want the *first* time the spheres were touching
			// (+sqrt would provide the 'exit' positions after they passed through each other)
			float ti = (-mdb - Mathf.Sqrt(qu)) / (mda*2);
			ti = Mathf.Clamp01(ti);

			hitData.timeOfImpact = ti;
			hitData.impactPos1 = start1 + (delta1 * ti);
			hitData.impactPos2 = start2 + (delta2 * ti);
		}

		return true;
	}

    public static Material CloneMaterial(Renderer renderer, bool destroyOnLoad)
    {
        Material mat = (Material)Material.Instantiate(renderer.material);
        renderer.material = mat;

        if (!destroyOnLoad)
            UnityEngine.Object.DontDestroyOnLoad(mat);

        return mat;
    }

    //Brads Hax

    public static Vector3 GetPointOnXYPlane( Vector2 pos, float AtZ ) //Move to utilities
    {
        //Get point at Y on ray
        Ray ray = Camera.main.ScreenPointToRay(pos);
        Vector3 MouseOnXYPlane = ray.GetPoint( (0 + AtZ - ray.origin.z ) / ray.direction.z );
        return MouseOnXYPlane;
    }

    public static Vector3 GetPerlinVector(float noiseIndex)
    {
        Vector3 perlVector = new Vector3(Mathf.PerlinNoise(noiseIndex / 2, noiseIndex* 5), Mathf.PerlinNoise(noiseIndex / 2, noiseIndex * 2), Mathf.PerlinNoise(noiseIndex / 2, noiseIndex * 3));
        return perlVector;
    }

    public static Vector3 MultiplyVectors(Vector3 v1, Vector3 v2)
    {
        Vector3 newVector = new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        return newVector;
    }
	
	

    public static float SignedAngleBetweenVectors( Vector3 A, Vector3 B )
    {
        float angle = 0;
        angle = Mathf.Atan2(A.x * B.y - A.y * B.x, A.x * B.x + A.y * B.y);
        return angle * Mathf.Rad2Deg;
    }

    public static Vector2 RandomVec2Normalized()
    {
        Vector2 randVec2 = (new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))).normalized;
        return randVec2;
    }

    public static Vector3 RandomVec3Normalized()
    {
        Vector3 randVec3 = (new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))).normalized;
        return randVec3;
    }
	
	public static Color RandomColor( float alpha )
    {
		Color col = new Color( UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), alpha );
        return col;
    }

    public static bool IsPointIn2DTriangle(Vector3 point, Vector3[] triPoints)
    {
        // REF: http://www.blackpawn.com/texts/pointinpoly/default.html  Bottom of page
        // Compute vectors      
        Vector2 v0 = triPoints[2] - triPoints[0];   //v0 = C - A
        Vector2 v1 = triPoints[1] - triPoints[0];   //v1 = B - A
        Vector2 v2 = point - triPoints[0];          //v2 = P - A

        // Compute dot products
        float dot00 = Vector3.Dot(v0, v0);    //dot00 = dot(v0, v0)
        float dot01 = Vector3.Dot(v0, v1);    //dot01 = dot(v0, v1)
        float dot02 = Vector3.Dot(v0, v2);    //dot02 = dot(v0, v2)
        float dot11 = Vector3.Dot(v1, v1);    //dot11 = dot(v1, v1)
        float dot12 = Vector3.Dot(v1, v2);    //dot12 = dot(v1, v2)

        // Compute barycentric coordinates
        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check if point is in triangle
        return (u > 0) && (v > 0) && (u + v < 1);
    }

    public static Vector3 ClampVector(Vector3 VectorToClamp, Vector3 min, Vector3 max)
    {
        VectorToClamp.x = Mathf.Clamp(VectorToClamp.x, min.x, max.x);
        VectorToClamp.y = Mathf.Clamp(VectorToClamp.y, min.y, max.y);
        VectorToClamp.z = Mathf.Clamp(VectorToClamp.z, min.z, max.z);

        return VectorToClamp;
    }

    public static bool ContainsNaN(Vector3 point)
    {
        if (float.IsNaN(point.x) || float.IsNaN(point.y) || float.IsNaN(point.z))
        {
            return true;
        }

        return false;
    }

    public static bool ContainsNaN(Vector3 point, string name)
    {
        if (float.IsNaN(point.x) || float.IsNaN(point.y) || float.IsNaN(point.z))
        {
            Debug.LogError(name + " contains NaN!");
            return true;
        }

        return false;
    }

	//gets the name of the directory where the assets for this project are stored
	public static string GetProjectDir()
	{
#if UNITY_EDITOR
		return UnityEditor.EditorApplication.currentScene.Replace("/main.unity", "");
#else
		return Application.dataPath;
#endif
	}
}

