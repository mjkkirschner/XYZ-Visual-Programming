using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nodeplay.Engine
{
		public class EvaluationPackage
		{
		public string Code{get;private set;}
		public Func<Dictionary<string,object>,Dictionary<string,object>,Dictionary<string,object>> CodePointer{get;private set;}
		public List<string> VariableNames{get;private set;}
		public List<System.Object> VariableValues{get;private set;}
		public List<string> OutputNames{get;private set;}
		public  List<Tuple<string,Action>> ExecutionPointers{get;private set;}
				
				
		public EvaluationPackage (string script, 
		                          List<string> variableNames, 
		                          List<System.Object> variableValues, 
		                          List<string> outputNames, 
		                          List<Tuple<string,Action>> executionPointers)
				{
			Code = script;
			VariableNames = variableNames;
			VariableValues = variableValues;
			OutputNames = outputNames ;
			ExecutionPointers = executionPointers;

				}

		public EvaluationPackage (Func<Dictionary<string,object>,Dictionary<string,object>,Dictionary<string,object>> codePointer, 
		                          List<string> variableNames, 
		                          List<System.Object> variableValues, 
		                          List<string> outputNames, 
		                          List<Tuple<string,Action>> executionPointers)
		{
			
			CodePointer = codePointer;
			VariableNames = variableNames;
			VariableValues = variableValues;
			OutputNames = outputNames ;
			ExecutionPointers = executionPointers;
		}

		}
}

