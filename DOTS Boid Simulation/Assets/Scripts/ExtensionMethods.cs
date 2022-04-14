using UnityEngine;
using Unity.Mathematics;
using System.Collections;

public static class ExtensionMethods
{
    public static float SqrMagnitude(this float3 vector)
    {
        return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
    }

    public static float Magnitude(this float3 vector)
    {
        if (vector.x == 0 && vector.y == 0 && vector.z == 0)
        {
            return 0;
        }
        return math.sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }

    public static float3 ClampMagnitude(this float3 vector, float maxLength)
    {
        if (vector.x == 0 && vector.y == 0 && vector.z == 0)
        {
            return vector;
        }
        float sqrMagnitude = vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        if (sqrMagnitude > maxLength * maxLength)
        {
            float num = (float)math.sqrt(sqrMagnitude);
            float num2 = vector.x / num;
            float num3 = vector.y / num;
            float num4 = vector.z / num;
            return new float3(num2 * maxLength, num3 * maxLength, num4 * maxLength);
        }
        return vector;
    }

    public static quaternion ToRotation(this float3 vector)
    {
        return quaternion.Euler(vector.x, vector.y, vector.z);
    }

    public static float3 Normalize(this float3 vector)
    {
        float sqrMagnitude = vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        float num = (float)math.sqrt(sqrMagnitude);
        float num2 = vector.x / num;
        float num3 = vector.y / num;
        float num4 = vector.z / num;
        return new float3(num2, num3, num4);
    }
}