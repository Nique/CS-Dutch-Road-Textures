using ICities;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

namespace DutchRoadTextures
{
    public class DutchRoadTexturesMod : IUserMod
    {
        public const UInt64 workshopId = 526126314;

        public string Name
        {
            get { return "Dutch Road Textures"; }
        }

        public string Description
        {
            get { return "Replaces the default road textures with dutch ones."; }
        }
    }

    public class DutchRoadTexturesLoader : LoadingExtensionBase
    {
        public static string getModPath()
        {
            string workshopPath = ".";
            foreach (PublishedFileId mod in Steam.workshop.GetSubscribedItems())
            {
                if (mod.AsUInt64 == DutchRoadTexturesMod.workshopId)
                {
                    workshopPath = Steam.workshop.GetSubscribedItemPath(mod);
                    //Debug.Log("dutchroadtextures: workshop path: " + workshopPath);
                    break;
                }
            }

            string localPath = DataLocation.modsPath + Path.DirectorySeparatorChar + "DutchRoadTextures";
            //Debug.Log("DutchRoadTextures: " + localPath);

            if(System.IO.Directory.Exists(localPath))
                return localPath;

            return workshopPath;
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            string path = getModPath();
            DutchRoadTextures.ReplaceRoadTextures(path);
        }
    }

    public class DutchRoadTextures : MonoBehaviour
    {
        public static Texture2D LoadTexture(string path)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(System.IO.File.ReadAllBytes(path));
            return texture;
        }

        public static void ReplaceRoadTextures(string modPath)
        {            
            Char DS = Path.DirectorySeparatorChar;
            String texturePath = modPath + DS + "Textures" + DS + "Road";
            
            // Replacement map
            Hashtable replaceMap = new Hashtable();
            
            // Bike lanes (Prefab.name, Filename)
            replaceMap.Add("Basic Road Bicycle", "Small_Bike");
            replaceMap.Add("Basic Road Elevated Bike", "Small_Elevated_Bike");
            replaceMap.Add("Basic Road Bridge Bike", "Small_Elevated_Bike");
            replaceMap.Add("Medium Road Bicycle", "Medium_Bike");
            replaceMap.Add("Medium Road Elevated Bike", "Medium_Elevated_Bike");
            replaceMap.Add("Medium Road Bridge Bike", "Medium_Elevated_Bike");            
            replaceMap.Add("Large Road Bicycle", "Large_Bike");
            replaceMap.Add("Large Road Elevated Bike", "Large_Elevated_Bike");
            replaceMap.Add("Large Road Bridge Bike", "Large_Elevated_Bike");

            // Bus bus lanes (Prefab.name, Filename)
            replaceMap.Add("Medium Road Bus", "Medium_Bus");
            replaceMap.Add("Medium Road Elevated Bus", "Medium_Elevated_Bus");
            replaceMap.Add("Medium Road Bridge Bus", "Medium_Elevated_Bus");
            replaceMap.Add("Large Road Bus", "Large_Bus");
            replaceMap.Add("Large Road Elevated Bus", "Large_Elevated_Bus");
            replaceMap.Add("Large Road Bridge Bus", "Large_Elevated_Bus");

            // Replace the textures
            NetCollection[] netCollections = FindObjectsOfType(typeof(NetCollection)) as NetCollection[];                
            foreach (var nc in netCollections)
            {
                foreach (NetInfo netPrefab in nc.m_prefabs)
                {
                    if(replaceMap.ContainsKey(netPrefab.name))
                    {
                        ReplaceSegmentTexture(netPrefab, texturePath + DS, replaceMap[netPrefab.name].ToString());
                    }
                }
            }                        
        }

        public static void ReplaceSegmentTexture(NetInfo netPrefab, String path, String fileName)
        {
            fileName = fileName + "_Segment";

            if (File.Exists(path + fileName + ".png")) 
            {
                bool isCombinedAvailable = File.Exists(path + "Combined.png");
                bool isLodAvailable = File.Exists(path + fileName + "_LOD.png");

                foreach (NetInfo.Segment segment in netPrefab.m_segments)
                {
                    segment.m_material.SetTexture("_MainTex", LoadTexture(path + fileName + ".png"));
                    segment.m_segmentMaterial.SetTexture("_MainTex", LoadTexture(path + fileName + ".png"));

                    if (isLodAvailable) segment.m_lodMaterial.SetTexture("_MainTex", LoadTexture(path + fileName + "_LOD.png"));
                    if (isCombinedAvailable) segment.m_combinedMaterial.SetTexture("_MainTex", LoadTexture(path + "Combined.png"));
                }

                netPrefab.UpdatePrefabInstances();
            }
        }

        // TODO: Replace the lanemarkings
        // CODE BELOW WORKS BUT DOESNT CHANGE ANYTHING.... I PROBABLY NEED TO CREATE MY OWN PROPS FOR THIS
        
        //public static void ReplaceLaneTexture(NetInfo netPrefab, String path, String fileName)
        //{


        //    String bikeLanePath = Path.Combine(path, "RoadMarkingBike_Lane");
        //    String busLanePath = Path.Combine(path, "RoadMarkingBus_Lane");

        //    foreach (NetInfo.Lane lane in netPrefab.m_lanes)
        //    {
        //        if (lane.m_laneProps)
        //        {

        //            if (lane.m_laneProps.name == "Props - Bikelane")
        //            {
        //                Debug.Log("Found props for bikelane");
        //                lane.m_laneProps.m_props[0].m_prop.m_material.SetTexture("_MainTex", LoadTexture(bikeLanePath + ".png"));
        //                lane.m_laneProps.m_props[0].m_finalProp.m_material.SetTexture("_MainTex", LoadTexture(bikeLanePath + ".png"));
        //            }
        //        }

        //    }
        //}

        // TODO: Optional road color changer from color picker in options panel. 
        // (only active if Extensions mod and road color changer mod is not active)
        
        //public static void ChangeRoadColors()
        //{
        //    Color32 highwayColor = new Color32(115, 115, 125, 255);
        //    Color32 largeRoadColor =  new Color32(105, 105, 115, 255);
        //    Color32 mediumRoadColor = new Color32(105, 105, 115, 255);
        //    Color32 smallRoadColor = new Color32(105, 105, 115, 255);
        //}
    }
}
