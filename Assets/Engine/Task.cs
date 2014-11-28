using UnityEngine;
using System.Collections;
using System;

public class Task
{
	
	public Action MethodCall {get;set;}
	public YieldInstruction Yieldbehavior{get;set;}
	public NodeModel Node{get;set;}

	public Task(NodeModel node, Action methodCall, YieldInstruction yieldbehavior){
		MethodCall = methodCall;
		Yieldbehavior = yieldbehavior;
		Node = node;
	}


}

