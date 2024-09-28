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

    public float m_cohesionDistance = 10f;
    public float m_alignmentDistance = 10f;
    public float m_seperationDistance = 2.5f;

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
                alignmentDelta += (avgVel - m_velocities[i]).normalized * m_alignment;
            }

            // Seperation
            Vector3 seperationDelta = Vector3.zero;
            Vector3 cummulativeOffset = Vector3.zero;
            for (int k = 0; k < m_boids.Count; ++k)
            {
                if (i == k) continue;
                if (Vector3.Distance(m_boids[k].transform.position, m_boids[i].transform.position) > m_seperationDistance) continue;

                cummulativeOffset += m_boids[i].transform.position - m_boids[k].transform.position;
            }
            seperationDelta += cummulativeOffset.normalized * m_seperation;

            // Target
            Vector3 targetDelta = Vector3.zero;
            Vector3 targetOffset = (targetCenter.position - m_boids[i].transform.position).normalized;
            targetDelta = targetOffset * m_target;

            m_velocities[i] += cohesionDelta + alignmentDelta + seperationDelta + targetDelta;
        }
    }
}
