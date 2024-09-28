using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_boidObject;

    public float m_cohesion = 1f;
    public float m_alignment = 1f;
    public float m_seperation = 1f;
    public float m_target = 1f;
    public float m_rotate = 1f;

    public float m_cohesionDistance = 10f;
    public float m_alignmentDistance = 10f;
    public float m_seperationDistance = 2.5f;
    public float m_orbitDistance = 10f;

    public Transform targetCenter;

    private List<GameObject> m_boids;
    private List<Vector3> m_velocities;

    private void Start()
    {
        m_boids = new List<GameObject>();
        m_velocities = new List<Vector3>();

        for (int i = 0; i < 10; ++i)
        {
            m_boids.Add(Instantiate(m_boidObject));
            m_velocities.Add(Vector3.zero);
            m_boids[i].transform.position += new Vector3(Random.value - 0.5f, Random.value + 0.5f, Random.value - 0.5f) * 5f;
        }
    }

    private void FixedUpdate()
    {
        UpdateVelocities();

        for (int i = 0; i < m_boids.Count; ++i)
        {
            m_boids[i].transform.position += m_velocities[i] * Time.fixedDeltaTime;
        }
    }

    private void UpdateVelocities()
    {
        for (int i = 0; i < m_boids.Count; ++i)
        {
            // Cohesion
            Vector3 cohesionDelta = Vector3.zero;
            Vector3 avgPos = Vector3.zero;
            int cohesionBoids = 0;
            for (int k = 0; k < m_boids.Count; ++k)
            {
                if (i == k) continue;
                if (Vector3.Distance(m_boids[k].transform.position, m_boids[i].transform.position) > m_cohesionDistance) continue;

                ++cohesionBoids;
                avgPos += m_boids[k].transform.position;
            }

            if (cohesionBoids > 0)
            {
                avgPos /= cohesionBoids;
                cohesionDelta += (avgPos - m_boids[i].transform.position).normalized * m_cohesion;
            }

            // Alignment
            Vector3 alignmentDelta = Vector3.zero;
            Vector3 avgVel = Vector3.zero;
            int alignmentBoids = 0;
            for (int k = 0; k < m_boids.Count; ++k)
            {
                if (i == k) continue;
                if (Vector3.Distance(m_boids[k].transform.position, m_boids[i].transform.position) > m_alignmentDistance) continue;

                ++alignmentBoids;
                avgVel += m_velocities[i];
            }

            if (alignmentBoids > 0)
            {
                avgVel /= alignmentBoids;
                float magnitude = (avgVel - m_velocities[i]).magnitude;

                if (magnitude > 0f)
                {
                    magnitude = Mathf.Max(0.1f, magnitude);
                    alignmentDelta += (avgVel - m_velocities[i]) / magnitude * m_alignment;
                }
            }

            // Seperation
            Vector3 seperationDelta = Vector3.zero;
            Vector3 cummulativeOffset = Vector3.zero;
            for (int k = 0; k < m_boids.Count; ++k)
            {
                if (i == k) continue;
                if (Vector3.Distance(m_boids[k].transform.position, m_boids[i].transform.position) > m_seperationDistance) continue;

                float magnitude = (m_boids[i].transform.position - m_boids[k].transform.position).magnitude;
                if (magnitude > 0)
                {
                    cummulativeOffset += (m_boids[i].transform.position - m_boids[k].transform.position) / (magnitude * magnitude);
                }
            }
            seperationDelta += cummulativeOffset * m_seperation;

            // Target
            Vector3 targetDelta = Vector3.zero;
            Vector3 targetOffset = targetCenter.position - m_boids[i].transform.position;
            Vector3 target = new Vector3(-targetOffset.x, 0f, -targetOffset.z).normalized * m_orbitDistance;
            //target += new Vector3(targetCenter.position.x, m_boids[i].transform.position.y, targetCenter.position.z);
            target += targetCenter.position;
            targetDelta += (target - m_boids[i].transform.position).normalized * m_target;

            // Rotate
            Vector3 rotateDelta = Vector3.zero;
            rotateDelta += Vector3.Cross(targetOffset, Vector3.up).normalized * m_rotate;

            m_velocities[i] += cohesionDelta + alignmentDelta + seperationDelta + targetDelta + rotateDelta;
            //m_velocities[i] += targetDelta;
            float velMag = m_velocities[i].magnitude;
            m_velocities[i] = m_velocities[i].normalized * Mathf.Min(velMag, 15f);
        }
    }
}
