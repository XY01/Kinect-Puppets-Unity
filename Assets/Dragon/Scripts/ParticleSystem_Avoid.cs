using UnityEngine;
using System.Collections;

public class ParticleSystem_Avoid : MonoBehaviour
{
    public ParticleSystem   m_PSys;
    public Transform        m_Avoid;

    public float            m_Attraction = 1;

    public float            m_Radius = 2;
    public float m_Smoothing = 10;

    public bool m_2D = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[m_PSys.particleCount];
        m_PSys.GetParticles(particles);

        if (particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                if (m_2D)
                {
                    Vector2 vectorTo = new Vector2(particles[i].position.x, particles[i].position.y) - new Vector2(m_Avoid.position.x, m_Avoid.position.y);
                    float distance = vectorTo.magnitude;

                    if (distance < m_Radius)
                    {
                        float effect = (distance / m_Radius);
                        effect = effect.ScaleFrom01(.5f, 1);
                        Vector2 targetVel = vectorTo.normalized * effect * m_Attraction;
                        particles[i].velocity = Vector3.Lerp(particles[i].velocity, targetVel, Time.deltaTime * m_Smoothing);
                        
                       
                        particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y, 0f);
                    }                   
                }
                else
                {
                    Vector3 vectorTo = particles[i].position - m_Avoid.position;
                    float distance = vectorTo.magnitude;

                    if (distance < m_Radius)
                    {
                        float effect = (distance / m_Radius);
                        effect = effect.ScaleFrom01(.5f, 1);
                        Vector3 targetVel = vectorTo.normalized * effect * m_Attraction;
                        particles[i].velocity = Vector3.Lerp(particles[i].velocity, targetVel, Time.deltaTime * m_Smoothing);
                    }
                }
            }
        }

        m_PSys.SetParticles(particles, particles.Length);	
	
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(m_Avoid.position, m_Radius);
    }
}
