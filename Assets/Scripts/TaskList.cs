using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskList {
    public struct Task
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
        Vector3 target;

    }

}
