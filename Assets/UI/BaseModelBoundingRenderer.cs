using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System;

namespace Nodeplay.UI
{

	public class BaseModelBoundingRenderer : BoundingRenderer
	{

		public BaseModel Model;
		private List<BaseModel> modelsToBound;
		protected override void Start()
		{
			Model = this.GetComponentInParent<NodeModel>();
			Model.GetComponent<EvaluationResultsRenderer>().UpdatingResultsPositions += UpdateBounding;
			Model.PropertyChanged += HandlePropertyChanged;

		}
		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//get the most uptodate list of evaluation results 
			modelsToBound = Model.GetComponent<EvaluationResultsRenderer>().EvaluationResulsts.Select(x => x.GetComponent<BaseModel>()).ToList();
			initialzed = true;
			UpdateBounding();

		}

		protected override void UpdateBounding()
		{
			if (initialzed)
			{
				if (tempgeos.Count > 0)
				{
					tempgeos.ForEach(x => GameObject.DestroyImmediate(x));
				}

				//generate the bounds of all gameobjects 
				if (modelsToBound.Count > 0)
				{

					if (modelsToBound.Any(x => x.transform.hasChanged == true))
					{
						tempgeos.Add(GenerateBounds(PrimitiveType.Cube, modelsToBound.Select(x => x.gameObject).ToList(), Color.grey));
					}

				}

				//TODO we'll want some logic here to compare the visited list and the 
			}
		}
	}
}