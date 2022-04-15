using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

public class BoidBehaviour : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minSpeed = 2;
    [SerializeField] float maxSpeed = 5;
    [SerializeField] float sightRadius = 2.5f;
    [SerializeField] float maxSteerForce = 3;

    [Header("Weights")]
    [SerializeField] float separateWeight = 1;
    [SerializeField] float alignWeight = 1;
    [SerializeField] float cohesionWeight = 1;
    [SerializeField] float targetWeight = 1;

    Transform target;
    Vector3 velocity;
    List<GameObject> allBoids = new List<GameObject>();

    void Start()
    {
        target = FindObjectOfType<BoidTarget>().transform;
        //allBoids = FindObjectOfType<BoidSpawner>().boids;

        velocity = transform.forward;

        BoidBehaviour[] boids = FindObjectsOfType<BoidBehaviour>();
        for (int i = 0; i < boids.Length; i++)
        {
            allBoids.Add(boids[i].gameObject);
        }
    }

    void Update()
    {
        //allBoids = FindObjectsOfType<BoidBehaviour>();

        if (target != null)
        {
            velocity += SteerTowards(target.position - transform.position) * targetWeight * Time.deltaTime;
        }
        Separation();
        Alignment();
        Cohesion();

        Vector3 dir = velocity.normalized;
        float speed = Mathf.Clamp(velocity.magnitude, minSpeed, maxSpeed);

        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;
    }

    void Separation()
    {
        Vector3 separationSum = Vector3.zero;
        int nearby = 1;
        for (int i = 0; i < allBoids.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, allBoids[i].transform.position);
            if (dist <= sightRadius)
            {
                separationSum += transform.position - allBoids[i].transform.position;
                nearby++;
            }
        }
        velocity += SteerTowards(separationSum / nearby) * separateWeight * Time.deltaTime;
    }

    void Alignment()
    {
        Vector3 alignmentSum = Vector3.zero;
        int nearby = 1;
        for (int i = 0; i < allBoids.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, allBoids[i].transform.position);
            if (dist <= sightRadius)
            {
                alignmentSum += allBoids[i].transform.forward;
                nearby++;
            }
        }
        velocity += SteerTowards(alignmentSum / nearby) * alignWeight * Time.deltaTime;
    }

    Vector3 centerOfMass;
    void Cohesion()
    {
        centerOfMass = transform.position;
        int nearby = 1;
        for (int i = 0; i < allBoids.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, allBoids[i].transform.position);
            if (dist <= sightRadius)
            {
                centerOfMass += allBoids[i].transform.position;
                nearby++;
            }
        }
        centerOfMass /= nearby + 1;
        velocity += SteerTowards(transform.position - centerOfMass) * cohesionWeight * Time.deltaTime;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized;
        return Vector3.ClampMagnitude(vector, maxSteerForce);
    }
}
