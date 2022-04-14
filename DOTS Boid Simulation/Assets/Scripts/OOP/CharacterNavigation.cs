using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class CharacterNavigation : MonoBehaviour {

    [SerializeField] Transform target;
    Vector3 startPos;
    Vector3 endPos;

    NavMeshAgent agent;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Target").transform;
    }

    void Start() {
        startPos = transform.position;
        endPos = new Vector3(transform.position.x, transform.position.y, target.transform.position.z);
    }

    void Update() {
        SetDestination();
        if (Vector3.Distance(agent.transform.position, endPos) <= 1f) {
            agent.Warp(startPos);
        }
    }

    void SetDestination() {
        if (target == null) return;
        agent.SetDestination(endPos);
    }
}
