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
        ID = new Guid();

        NodeCalled = findNodeCalled(triggerindex);

    }


}

