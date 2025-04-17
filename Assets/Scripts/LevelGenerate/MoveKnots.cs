using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct MoveKnots : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float3> KnotPositions;
    public float DeltaSpeed;
    public bool MoveAlongZ;
    public float4x4 LocalToWorldMatrix;
    public float4x4 WorldToLocalMatrix;

    public void Execute(int index)
    {
        float3 knotPosition = KnotPositions[index];

        if (knotPosition.Equals(float3.zero))
            return;


        var pos = knotPosition;

        float3 worldPos = math.mul(LocalToWorldMatrix, new float4(knotPosition, 1)).xyz;

        if (MoveAlongZ)
            worldPos.z += DeltaSpeed;
        else
            worldPos.x -= DeltaSpeed;

        knotPosition = math.mul(WorldToLocalMatrix, new float4(worldPos, 1)).xyz;

        KnotPositions[index] = knotPosition;

    }
}
