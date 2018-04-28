﻿using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Core : MonoBehaviour {
    private static MapLoader mapLoader = new MapLoader();
    private static MapRenderer mapRenderer = new MapRenderer();

    public static MapLoader MapLoader {
        get { return mapLoader; }
    }

    public static MapRenderer MapRenderer {
        get { return mapRenderer; }
    }

    public Dropdown mapDropdown;    
    private Hashtable configs = new Hashtable();
    private static string CFG_NAME = "config.txt";

    void Start() {
        loadConfigs();

        Debug.Log("Loading GRF at " + configs["grf"] + "...");
        FileManager.loadGrf(configs["grf"] as string);
        Debug.Log("GRF loaded, filetable contains " + FileManager.Grf.files.Count + " files.");

        Debug.Log("Building map list...");
        MapSelector selector = new MapSelector(FileManager.Grf);
        selector.buildDropdown(mapDropdown);
        Debug.Log("Map list has " + selector.GetMapList().Count + " entries.");
    }

    private void loadConfigs() {
        
        string cfgTxt = null;
        if(Application.isMobilePlatform) {
                cfgTxt = "grf=" + Application.streamingAssetsPath + "/data.grf";
        } else{
            cfgTxt = FileManager.Load("config.txt") as string;

            if(cfgTxt == null) {
                FileStream stream = File.Open(Application.dataPath + "/" + CFG_NAME, FileMode.Create);

                string defaultCfg = "grf=" + Application.dataPath + "/data.grf";
                stream.Write(Encoding.UTF8.GetBytes(defaultCfg), 0, defaultCfg.Length);
                stream.Close();
                cfgTxt = defaultCfg;
            }
        }

        foreach(string s in cfgTxt.Split('\n')) {
            string[] properties = s.Split('=');
            if(properties.Length == 2) {
                configs.Add(properties[0], properties[1]);
            }
        }
    }

    void Update() {
        mapDropdown.gameObject.SetActive(Cursor.lockState != CursorLockMode.Locked);
    }

    public void OnPostRender() {
        if(mapRenderer.Ready) {
            mapRenderer.Render();
        }
    }

}