using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] float spawnDelayTime = 2f;
    [SerializeField] float measureTime = 10f;
    bool measureFPS = false;

    [SerializeField] float currentFPS;
    [SerializeField] float averageFPS;
    [SerializeField] int averagePercievedFrames = 100;
    Queue<float> averageFrames;

    [SerializeField] float minimumFPS;
    [SerializeField] float maximumFPS;

    public delegate void SpawnBoids();
    public static event SpawnBoids OnSpawnBoids;
    public int boidIncreaseSpawnAmount = 20;
    public int boidCount;

    //Writing to Spreadsheet
    [SerializeField] string testFilename = "new test";
    string filename = "";
    TextWriter tw;

    private void Start()
    {
        WaitTillSpawn();

        filename = Application.dataPath + "/" + testFilename + ".csv";
        tw = new StreamWriter(filename, false);
        tw.WriteLine("Boid Count, Average, Minimum, Maximum");
    }

    private void OnEnable()
    {
        OnSpawnBoids += WaitTillSpawn;
    }

    private void OnDisable()
    {
        OnSpawnBoids -= WaitTillSpawn;
        tw.Close();
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
    }

    private void WaitTillSpawn()
    {
        averageFrames = new Queue<float>();
        minimumFPS = Mathf.Infinity;
        maximumFPS = 0f;

        StopAllCoroutines();
        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(spawnDelayTime);
        measureFPS = true;
        yield return new WaitForSeconds(measureTime);
        measureFPS = false;

        //print("Boids: " + boidCount + " | Avg: " + averageFPS + " | Min: " + minimumFPS + " | Max: " + maximumFPS);

        //Write to Spreadsheet
        tw.WriteLine(boidCount + "," + averageFPS + "," + minimumFPS + "," + maximumFPS);

        if (averageFPS > 10f)
        {
            OnSpawnBoids();
        }
        else
        {
            print("done");
        }
    }
}
