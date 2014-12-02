using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nodeplay.Engine
{
    public static class TaskExtensions
    {
        /// <summary>
        /// method inserts tasks into the Q at the correct location
        /// directly behind callers or blocks of tasks that share 
        /// the same caller, we compare callers using the nodename/guid and another guid
        /// for the callsite
        /// TODO replace this method with linked list implementation
        /// </summary>
        /// <param name="task"></param>
        public static void InsertTask(this List<Task> taskSchedule, Task task)
        {
            //scan the current task list
            //looking for the index of the caller
            Debug.Log("<color=green>Task insertion:</color>I am a Task of type" + task.NodeCalled + " my caller task was " + task.Caller.NodeRunningOn);
            var callerindex = taskSchedule.FindLastIndex(x => x == task.Caller || x.Caller == task.Caller);
            Debug.Log("inserting at index " + callerindex);


            taskSchedule.Insert(callerindex + 1, task);

        }
    }
}
