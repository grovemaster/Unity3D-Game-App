﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PrefabUtil
{
    public class PrefabsDictionary
    {
        Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

        public PrefabsDictionary()
        {
            string json = File.ReadAllText("prefabs.json");

            var gameobjects = JsonHelper.getJsonArray<GameObject>(json);

            for (int i = 0; i < gameobjects.Length; i++)
            {
                prefabs[gameobjects[i].name] = gameobjects[i];
            }
        }

        public GameObject Instantiate(string player)
        {
            return GameObject.Instantiate(prefabs[player]);
        }
    }
}