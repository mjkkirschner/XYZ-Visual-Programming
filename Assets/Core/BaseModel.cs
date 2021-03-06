﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;

public class BaseModel : MonoBehaviour, INotifyPropertyChanged
{
    //todo probably will need to readd location properties if I want to support the non-graph based workflows...$$$
    private Vector3 location;
    public Vector3 Location
	
    {
        get
        {
            return this.location;

        }

        set
        {
            if (value != this.location)
            {
                this.location = value;
                NotifyPropertyChanged("Location");
            }
        }
    }
	public Guid GUID{get;private set;}

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void NotifyPropertyChanged(String info)
    {
        //Debug.Log("sending " + info + " change notification");
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }

	protected virtual void OnEnable(){
		if (GUID == Guid.Empty)
		{
			GUID = Guid.NewGuid();
		}
	}

	public void SetGuidOnLoad(Guid id){
		GUID = id;

	}

    protected virtual void Start()
    {

    }


    protected virtual void Update()
    {
        Location = this.gameObject.transform.position;
        
    }

    /// <summary>
    /// this code will either be a method or expression for generating UI elements
    /// it may also point to UI prefab data to load
    /// the viewmodel will execute this code, it is stored here to avoid needing custom views
    /// for each element type with similar interaction logic
    /// </summary>
    public virtual GameObject BuildSceneElements()
    {
        //TODO implement
        return null;

    }

    
    

 

  




}
