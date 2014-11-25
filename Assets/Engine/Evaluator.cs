using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;

namespace Nodeplay.Engine { 

public abstract class Evaluator : MonoBehaviour
{

		public abstract Dictionary<string,object> Evaluate(string script, List<string> variableNames, List<System.Object> variableValues, List<string> OutputNames, List<Tuple<string,Action>> ExecutionPointers);

		
}

}