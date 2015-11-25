using UnityEngine;
using System.Collections;

public class PerlinNoise
{
	const int B = 256;
	int[] m_perm = new int[B+B];
	Texture2D m_permTex;

	public PerlinNoise(int seed)
	{
		UnityEngine.Random.seed = seed;

		int i, j, k;
		for (i = 0 ; i < B ; i++) 
		{
			m_perm[i] = i;
		}

		while (--i != 0) 
		{
			k = m_perm[i];
			j = UnityEngine.Random.Range(0, B);
			m_perm[i] = m_perm[j];
			m_perm[j] = k;
		}
	
		for (i = 0 ; i < B; i++) 
		{
			m_perm[B + i] = m_perm[i];
		}
		
	}
	
	float FADE(float t) { return t * t * t * ( t * ( t * 6.0f - 15.0f ) + 10.0f ); }
	
	float LERP(float t, float a, float b) { return (a) + (t)*((b)-(a)); }
	
	float GRAD1(int hash, float x ) 
	{
		//This method uses the mod operator which is slower 
		//than bitwise operations but is included out of interest
//		int h = hash % 16;
//		float grad = 1.0f + (h % 8);
//		if((h%8) < 4) grad = -grad;
//		return ( grad * x );
		
		int h = hash & 15;
    	float grad = 1.0f + (h & 7);
    	if ((h&8) != 0) grad = -grad;
    	return ( grad * x );
	}
	
	float GRAD2(int hash, float x, float y)
	{
		//This method uses the mod operator which is slower 
		//than bitwise operations but is included out of interest
//		int h = hash % 16;
//    	float u = h<4 ? x : y;
//    	float v = h<4 ? y : x;
//		int hn = h%2;
//		int hm = (h/2)%2;
//    	return ((hn != 0) ? -u : u) + ((hm != 0) ? -2.0f*v : 2.0f*v);
		
    	int h = hash & 7;
    	float u = h<4 ? x : y;
    	float v = h<4 ? y : x;
    	return (((h&1) != 0)? -u : u) + (((h&2) != 0) ? -2.0f*v : 2.0f*v);
	}
	
	
	float GRAD3(int hash, float x, float y , float z)
	{
		//This method uses the mod operator which is slower 
		//than bitwise operations but is included out of interest
//    	int h = hash % 16;
//    	float u = (h<8) ? x : y;
//    	float v = (h<4) ? y : (h==12||h==14) ? x : z;
//		int hn = h%2;
//		int hm = (h/2)%2;
//    	return ((hn != 0) ? -u : u) + ((hm != 0) ? -v : v);
		
		int h = hash & 15;
    	float u = h<8 ? x : y;
    	float v = (h<4) ? y : (h==12 || h==14) ? x : z;
    	return (((h&1) != 0)? -u : u) + (((h&2) != 0)? -v : v);
	}
	
	float Noise1D( float x )
	{
		//returns a noise value between -0.5 and 0.5
	    int ix0, ix1;
	    float fx0, fx1;
	    float s, n0, n1;
	
	    ix0 = (int)Mathf.Floor(x); 	// Integer part of x
	    fx0 = x - ix0;       	// Fractional part of x
	    fx1 = fx0 - 1.0f;
	    ix1 = ( ix0+1 ) & 0xff;
	    ix0 = ix0 & 0xff;    	// Wrap to 0..255
		
	    s = FADE(fx0);
	
	    n0 = GRAD1(m_perm[ix0], fx0);
	    n1 = GRAD1(m_perm[ix1], fx1);
	    return 0.188f * LERP( s, n0, n1);
	}
	
	float Noise2D( float x, float y )
	{
		//returns a noise value between -0.75 and 0.75
	    int ix0, iy0, ix1, iy1;
	    float fx0, fy0, fx1, fy1, s, t, nx0, nx1, n0, n1;
	
	    ix0 = (int)Mathf.Floor(x); 	// Integer part of x
	    iy0 = (int)Mathf.Floor(y); 	// Integer part of y
	    fx0 = x - ix0;        	// Fractional part of x
	    fy0 = y - iy0;        	// Fractional part of y
	    fx1 = fx0 - 1.0f;
	    fy1 = fy0 - 1.0f;
	    ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
	    iy1 = (iy0 + 1) & 0xff;
	    ix0 = ix0 & 0xff;
	    iy0 = iy0 & 0xff;
	    
	    t = FADE( fy0 );
	    s = FADE( fx0 );
	
		nx0 = GRAD2(m_perm[ix0 + m_perm[iy0]], fx0, fy0);
	    nx1 = GRAD2(m_perm[ix0 + m_perm[iy1]], fx0, fy1);
		
	    n0 = LERP( t, nx0, nx1 );
	
	    nx0 = GRAD2(m_perm[ix1 + m_perm[iy0]], fx1, fy0);
	    nx1 = GRAD2(m_perm[ix1 + m_perm[iy1]], fx1, fy1);
		
	    n1 = LERP(t, nx0, nx1);
	
	    return 0.507f * LERP( s, n0, n1 );
	}
	
	float Noise3D( float x, float y, float z )
	{
		//returns a noise value between -1.5 and 1.5
	    int ix0, iy0, ix1, iy1, iz0, iz1;
	    float fx0, fy0, fz0, fx1, fy1, fz1;
	    float s, t, r;
	    float nxy0, nxy1, nx0, nx1, n0, n1;
	
	    ix0 = (int)Mathf.Floor( x ); // Integer part of x
	    iy0 = (int)Mathf.Floor( y ); // Integer part of y
	    iz0 = (int)Mathf.Floor( z ); // Integer part of z
	    fx0 = x - ix0;        // Fractional part of x
	    fy0 = y - iy0;        // Fractional part of y
	    fz0 = z - iz0;        // Fractional part of z
	    fx1 = fx0 - 1.0f;
	    fy1 = fy0 - 1.0f;
	    fz1 = fz0 - 1.0f;
	    ix1 = ( ix0 + 1 ) & 0xff; // Wrap to 0..255
	    iy1 = ( iy0 + 1 ) & 0xff;
	    iz1 = ( iz0 + 1 ) & 0xff;
	    ix0 = ix0 & 0xff;
	    iy0 = iy0 & 0xff;
	    iz0 = iz0 & 0xff;
	    
	    r = FADE( fz0 );
	    t = FADE( fy0 );
	    s = FADE( fx0 );
	
		nxy0 = GRAD3(m_perm[ix0 + m_perm[iy0 + m_perm[iz0]]], fx0, fy0, fz0);
	    nxy1 = GRAD3(m_perm[ix0 + m_perm[iy0 + m_perm[iz1]]], fx0, fy0, fz1);
	    nx0 = LERP( r, nxy0, nxy1 );
	
	    nxy0 = GRAD3(m_perm[ix0 + m_perm[iy1 + m_perm[iz0]]], fx0, fy1, fz0);
	    nxy1 = GRAD3(m_perm[ix0 + m_perm[iy1 + m_perm[iz1]]], fx0, fy1, fz1);
	    nx1 = LERP( r, nxy0, nxy1 );
	
	    n0 = LERP( t, nx0, nx1 );
	
	    nxy0 = GRAD3(m_perm[ix1 + m_perm[iy0 + m_perm[iz0]]], fx1, fy0, fz0);
	    nxy1 = GRAD3(m_perm[ix1 + m_perm[iy0 + m_perm[iz1]]], fx1, fy0, fz1);
	    nx0 = LERP( r, nxy0, nxy1 );
	
	    nxy0 = GRAD3(m_perm[ix1 + m_perm[iy1 + m_perm[iz0]]], fx1, fy1, fz0);
	   	nxy1 = GRAD3(m_perm[ix1 + m_perm[iy1 + m_perm[iz1]]], fx1, fy1, fz1);
	    nx1 = LERP( r, nxy0, nxy1 );
	
	    n1 = LERP( t, nx0, nx1 );
	    
	    return 0.936f * LERP( s, n0, n1 );
	}
	
	public float FractalNoise1D(float x, int octNum, float frq, float amp)
	{
		float gain = 1.0f;
		float sum = 0.0f;
	
		for(int i = 0; i < octNum; i++)
		{
			sum +=  Noise1D(x*gain/frq) * amp/gain;
			gain *= 2.0f;
		}
		return sum;
	}
	
	public float FractalNoise2D(float x, float y, int octNum, float frq, float amp)
	{
		float gain = 1.0f;
		float sum = 0.0f;
		
		for(int i = 0; i < octNum; i++)
		{
			sum += Noise2D(x*gain/frq, y*gain/frq) * amp/gain;
			gain *= 2.0f;
		}
		return sum;
	}
	
	public float FractalNoise3D(float x, float y, float z, int octNum, float frq, float amp)
	{
		float gain = 1.0f;
		float sum = 0.0f;
	
		for(int i = 0; i < octNum; i++)
		{
			sum +=  Noise3D(x*gain/frq, y*gain/frq, z*gain/frq) * amp/gain;
			gain *= 2.0f;
		}
		return sum;
	}
	
	public void LoadPermTableIntoTexture()
	{
		m_permTex = new Texture2D(256, 1, TextureFormat.Alpha8, false);
		m_permTex.filterMode = FilterMode.Point;
		m_permTex.wrapMode = TextureWrapMode.Clamp;
		
		for(int i = 0; i < 256; i++)
		{
			float v = (float)m_perm[i] / 255.0f;
				
			m_permTex.SetPixel(i, 0, new Color(0,0,0,v));
		}
		
		m_permTex.Apply();
	}
	
	public void RenderIntoTexture(Shader shader, RenderTexture renderTex, int octNum, float frq, float amp)
	{
		if(!m_permTex) LoadPermTableIntoTexture();
		
		Material mat = new Material(shader);
		
		mat.SetFloat("_Frq", frq);
		mat.SetFloat("_Amp", amp);
		mat.SetVector("_TexSize", new Vector4(renderTex.width-1.0f, renderTex.height-1.0f, 0, 0));
		mat.SetTexture("_Perm", m_permTex);
		
		float gain = 1.0f;
		for(int i = 0; i < octNum; i++)
		{
			mat.SetFloat("_Gain", gain);
		   
		    Graphics.Blit(null, renderTex, mat);
			
			gain *= 2.0f;
		}
	}

}













