﻿using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class import : MonoBehaviour {
	Hashtable project = new Hashtable();
	private viewer viewer;
	private menu menu;
	// Use this for initialization
	void Start () {
		viewer = GameObject.Find("viewer").GetComponent<viewer>();
		menu = GameObject.Find("menu").GetComponent<menu>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void start_import(Hashtable project) {
		if(project.Contains("Location") && project.Contains("Name")) {
			if(checkProject(project["Location"].ToString(),project["Name"].ToString())) {
				this.project = project;
			} else {
				menu.setRender(true);
				menu.console("Das Project ist beschädigt.");
			}
		} else {
			menu.setRender(true);
			menu.console("Es wurde kein Project ausgewählt.");
		}
	}
	void deleteElement() {
		List<GameObject> tmpCoreelements = viewer.getCoreelements ();
		if (tmpCoreelements.Count>0) {
			viewer.destroyElement (tmpCoreelements [tmpCoreelements.Count -1]);
			deleteElement();
		}
	}
	public void setElements() {
		deleteElement ();

		Hashtable project = menu.getProject ();
		// safe location
		string TopLevelPath = @project["Location"].ToString();
		// projectpath wich should be created
		string ProjectPath = TopLevelPath;
		
		// relation file path
		// and creation
		string elementsfile = System.IO.Path.Combine(ProjectPath, project["Name"].ToString() + "_elements" + ".csv");
		// Open the file to read from.
		using (StreamReader sr = File.OpenText(elementsfile)) {
			string s = "";
			string[] words;
			while ((s = sr.ReadLine()) != null) {
				words = s.Split(new char[] {';'});
				if(words[0]!=null) {
					viewer.create_coreelement(float.Parse(words[2].ToString()), float.Parse(words[3].ToString()), float.Parse(words[4].ToString()), words[1].ToString(), int.Parse(words[5].ToString()), int.Parse(words[0].ToString()));
				}
			}
		}
	}

	/** Checks project if its consistent
	 * 
	 * @param	pathProject	projecttopfolder
	 * @param	projectname
	 * @return	boolean		consitent or inconsistent
	 * 
	 */
	public Boolean checkProject(string PathProject,string projectname) {
		menu.console("INFO: checking Project: "+PathProject);
		Boolean conf = false,rel=false,elm=false;
		System.IO.FileInfo[] fi = new System.IO.DirectoryInfo(PathProject).GetFiles();
		foreach(var fileInfo in fi)
		{
			if(fileInfo.Name.ToString() == projectname+".conf") {
				conf = true;
				menu.console("INFO: checking : "+projectname+".conf -> passed");
			}
			else if(fileInfo.Name.ToString() == projectname+"_relations.csv") {
				rel = true;
				menu.console("INFO: checking : "+projectname+"_relations.csv -> passed");
			}
			else if(fileInfo.Name.ToString() == projectname+"_elements.csv") {
				elm = true;
				menu.console("INFO: checking : "+projectname+"_elements.csv -> passed");
			}
		}
		if(elm && rel && conf) {
			menu.console("INFO: checking Project "+PathProject+" -> passed");
			return true;
		} else {
			menu.console("INFO: checking Project "+PathProject+" -> fails");
			return false;
		}
	}

}
