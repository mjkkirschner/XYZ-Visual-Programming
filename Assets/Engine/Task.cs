using UnityEngine;
using System.Collections;
using System;

public class Task
{

    public Action MethodCall { get; set; }
    public YieldInstruction Yieldbehavior { get; set; }
    public NodeModel NodeRunningOn { get; set; }
    public NodeModel NodeCalled { get; set; }
    public Task Caller  {get;  set;}
    public Guid ID;

    private NodeModel findNodeCalled(int index)
    {
		if (NodeRunningOn.ExecutionOutputs.Count<1)
		{
			Debug.Log("this node, "+NodeRunningOn+ " does not have an execution output, so it will call no downstream nodes");
			return null;
		}
        var trigger = NodeRunningOn.ExecutionOutputs[index];
        if (trigger.IsConnected)
        {
            var nextNode = trigger.connectors[0].PEnd.Owner;
            return nextNode;
        }
        return null;
    }
    public Task(Task caller,NodeModel noderunningon,int triggerindex, Action methodCall, YieldInstruction yieldbehavior)
    {
        MethodCall = methodCall;
        Yieldbehavior = yieldbehavior;
        NodeRunningOn = noderunningon;
        Caller = caller;
		ID = Guid.NewGuid();

        NodeCalled = findNodeCalled(triggerindex);

    }


}

