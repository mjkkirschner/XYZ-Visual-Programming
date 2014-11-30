using UnityEngine;
using System.Collections;
using System;

public class Task
{
	
	public Action MethodCall {get;set;}
	public YieldInstruction Yieldbehavior{get;set;}

	public Task(Action methodCall, YieldInstruction yieldbehavior){
		methodCall = methodCall;
		Yieldbehavior = yieldbehavior;
	}

}

