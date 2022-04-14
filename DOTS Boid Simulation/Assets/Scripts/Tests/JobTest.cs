using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public class JobTest : MonoBehaviour
{
    private void Start()
    {
        // Create input and output data
        NativeArray<float> firstNumber = new NativeArray<float>(100, Allocator.TempJob);
        NativeArray<float> secondNumber = new NativeArray<float>(100, Allocator.TempJob);
        NativeArray<float> sum = new NativeArray<float>(100, Allocator.TempJob);

        // Create Jobs
        AddJob addJob = new AddJob
        {
            inputA = firstNumber,
            inputB = secondNumber,
            output = sum
        };
        AddJobParallel addJobParallel = new AddJobParallel
        {
            inputA = firstNumber,
            inputB = secondNumber,
            output = sum
        };

        // Schedule Jobs
        JobHandle addJobHandle = addJob.Schedule();
        JobHandle addJobParallelHandle = addJobParallel.Schedule(firstNumber.Length, 64);

        // Wait for Jobs to Complete
        addJobHandle.Complete();
        addJobParallelHandle.Complete();

        // Clear Memory
        firstNumber.Dispose();
        secondNumber.Dispose();
        sum.Dispose();
    }
}

public struct AddJob : IJob
{
    [ReadOnly]
    public NativeArray<float> inputA;
    [ReadOnly]
    public NativeArray<float> inputB;
    [WriteOnly]
    public NativeArray<float> output;

    public void Execute()
    {
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = inputA[i] + inputB[i];
        }
    }
}

public struct AddJobParallel : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float> inputA;
    [ReadOnly]
    public NativeArray<float> inputB;
    [WriteOnly]
    public NativeArray<float> output;

    public void Execute(int i)
    {
        output[i] = inputA[i] + inputB[i];
    }
}

[BurstCompile]
public struct Job : IJob
{
    public void Execute()
    {
        //Perform Operation
    }
}
