using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskList {
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
        float fTaskTime;
        Vector3 target;
        Task(Type type)
        {
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
                    fTaskTime = 3.0f;
                    break;
            }
        }
    }
    Queue<Task> tasks;

}
