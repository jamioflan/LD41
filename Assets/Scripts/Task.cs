using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task
{
    public enum Type
    {
        FETCH_RESOURCE,
        EAT,
        RELAX,
        WORK_ROOM,
        BUILD
    }
    public Type type;
    public Resource.ResourceType resourceType = Resource.ResourceType.NULL;
    public float fTaskTime;
    public int targetX;
    public int targetY;
    public Task(Type type_)
    {
        type = type_;
        switch (type)
        {
            case Type.FETCH_RESOURCE:
                fTaskTime = 0.5f;
                break;
            case Type.EAT:
                fTaskTime = 2.0f;
                break;
            case Type.RELAX:
                fTaskTime = 10.0f;
                break;
            case Type.WORK_ROOM:
                fTaskTime = float.PositiveInfinity;
                break;
            case Type.BUILD:
                fTaskTime = float.PositiveInfinity;
                break;
        }
    }
}

