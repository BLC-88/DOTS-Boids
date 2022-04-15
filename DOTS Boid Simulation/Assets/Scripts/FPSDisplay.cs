using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] float spawnDelayTime = 2f;
    [HideInInspector] public bool measureFPS = false;

    float currentFPS;
    float averageFPS;
    [SerializeField] 
    [Tooltip("How many frames should be used to caluclate the average FPS")]
    int averagePercievedFrames = 100;
    Queue<float> averageFrames;

    float minimumFPS;
    float maximumFPS;

    [HideInInspector] public int boidCount;

    [Header("UI")]
    [SerializeField] Text boidCountText;
    [SerializeField] Text currentFPSText;
    [SerializeField] Text averageFPSText;
    [SerializeField] Text minimumFPSText;
    [SerializeField] Text maximumFPSText;

    void Start()
    {
        averageFrames = new Queue<float>();
        minimumFPS = Mathf.Infinity;
        maximumFPS = 0f;
        StartCoroutine(SpawnDelay());
    }

    void Update()
    {
        if (measureFPS)
        {
            currentFPS = 1f / Time.deltaTime;

            if (currentFPS < minimumFPS)
            {
                minimumFPS = currentFPS;
            }

            if (currentFPS > maximumFPS)
            {
                maximumFPS = currentFPS;
            }

            if (averageFrames.Count >= averagePercievedFrames)
            {
                averageFrames.Dequeue();
            }
            averageFrames.Enqueue(currentFPS);

            averageFPS = averageFrames.Average();
        }

        boidCountText.text = "Entities: " + boidCount.ToString("n0");
        currentFPSText.text = "FPS: " + currentFPS.ToString("f2");
        averageFPSText.text = "AVG: " + averageFPS.ToString("f2");
        minimumFPSText.text = "MIN: " + minimumFPS.ToString("f2");
        maximumFPSText.text = "MAX: " + maximumFPS.ToString("f2");
    }

    public IEnumerator SpawnDelay()
    {
        measureFPS = false;
        yield return new WaitForSeconds(spawnDelayTime);
        measureFPS = true;
    }
}
