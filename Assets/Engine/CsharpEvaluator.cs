using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;

using System.Text;
using System;
using System.IO;
using Nodeplay.Engine;
using Microsoft.Scripting.Hosting;

namespace Nodeplay.Engine
{	
	public class CsharpEvaluator : Evaluator
	{	
		public Func<Dictionary<string,object>,Dictionary<string,object>,Dictionary<string,object>> Code;
		public Dictionary<string, object> IntermediateOutValues;
		public String StdOut;


		public void Start()
		{
			//names = new List<String>() { "name1", "name2" };
			//vals = new List<System.Object>() { 1, 2 };
			//outnames = {range}
			//Debug.Log(Evaluate(code, names, vals));

		}

		/// <summary>
		// this compiledeval method calls executes a delegate which points to the compiledeval function on a nodemodel
		// we need to supply the nodemodel with a set of input values representing the state of the node
		// TODO this doesnt make much sense as the nodemodel already holds this updated inputvalue dictionary
		// and can retrieve these values directly.... maybe... have to check if these vals come from the node 
		// or from the evaluator Queue...
		// this evaluation which runs on the nodemodel must make sure to push intermediate values of output back
		// into the out dict that we supply as an out parameter.
		// when this function returns it must return the final outputs in a dictionary

		/// </summary>
		/// <param name="variableNames"></param>
		/// <param name="variableValues"></param>
		/// <param name="OutputNames"></param>
		/// <returns></returns>
		public Dictionary<string, object> CompiledEvaluation(Dictionary<string,object> inputValues,
		                                                     ref Dictionary<string,object> intermediateOutValues)
		{	
			//assign this out parameter to the current outvalues held by the evaluator
			//intermediateOutValues = IntermediateOutValues;
			IntermediateOutValues = intermediateOutValues;
			var outputDict = Code.Invoke(inputValues, intermediateOutValues);
			return outputDict;
		}

		public override Dictionary<string, object> PollScopeForOutputs(List<string> OutputNames)
		{
			var outdict = new Dictionary<string, object>();

			foreach (var outname in OutputNames)
			{
				if (IntermediateOutValues.ContainsKey(outname))
				{
					outdict[outname] = IntermediateOutValues[outname];
					//Debug.Log(outname + " was currently equal to" + outdict[outname]);
				}
				else
				{
					outdict[outname] = "No variable named" + outname + "was defined in the c# code";
				}
			}
			return outdict;
		}
		//
		public override Dictionary<string, object> Evaluate (EvaluationPackage evalpackage)
		{
			var outputNames = evalpackage.OutputNames;
			var variableNames = evalpackage.VariableNames;
			var variableValues = evalpackage.VariableValues;
			var executionPointers = evalpackage.ExecutionPointers;

			Code = evalpackage.CodePointer;
			//Debug.Log("CODE is of type" + Code.GetType().ToString());
			using (var memoryStream = new MemoryStream())
			{

				//TODO I think this needs to only happen once, or on a null check,
				//this dictionary will get overwritten anytime this function is called currently...
				//build the intermediatedict and assign keys from the output names
				var outdict = new Dictionary<string, object>();
				foreach (var outname in outputNames)
				{
					if (outdict.ContainsKey(outname))
					{
						outdict[outname] = "overwrote the previous entry";
					}
					else
					{
						//Debug.Log("added a space in the outdict in the csharp evaluator with name " + outname);
						outdict[outname] = null;
					}
				}


				//we expose all input variables and trigger delegates in the inputdict which we will pass to compiled eval
				var inputdict = new Dictionary<string,object>();
				foreach (var variable in variableNames)
				{
					var index = variableNames.IndexOf(variable);
					inputdict[variable] =  variableValues[index];
					//Debug.Log("setting" + variable + "to" + variableValues[index].ToString());
				}

				foreach (var pointer in executionPointers)
				{
					inputdict[pointer.First] = pointer.Second;
					//Debug.Log("setting " + pointer.First + " to " + pointer.Second.ToString() + " in csharp context");
				}

				//redirect the output for this delegate
				//Console.SetOut (new StreamWriter(memoryStream));

				try
				{
					outdict = CompiledEvaluation(inputdict,ref outdict);
				}
				catch (Exception e)
				{

					string error = e.Message + e.StackTrace;
					Debug.LogException(e);
				}
				finally
				{

					var length = (int)memoryStream.Length;
					var bytes = new byte[length];
					memoryStream.Seek(0, SeekOrigin.Begin);
					memoryStream.Read(bytes, 0, length);
					StdOut = Encoding.UTF8.GetString(bytes, 0, length).Trim();


				}
				//Console.OpenStandardOutput();
				return outdict;
			}


		}

	}
}


