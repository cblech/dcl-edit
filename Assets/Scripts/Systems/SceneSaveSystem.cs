using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SceneSaveSystem : MonoBehaviour
{
    public bool saveNow = false;
    public bool loadNow = false;

    void Update()
    {
        if(saveNow)
        {
            saveNow = false;
            Save();
        }
        if(loadNow)
        {
            loadNow = false;
            Load();
        }
    }

    public static void Save()
    {
        var jsonString = SceneManager.Entities.ToJson();
        //Debug.Log(jsonString);

        Directory.CreateDirectory(SceneManager.DclProjectPath + "/dcl-edit/saves");
        try
        {
            var fileWriter = new StreamWriter(SceneManager.DclProjectPath + "/dcl-edit/saves/save.json", false);

            fileWriter.WriteLine(jsonString);

            fileWriter.Close();

            HoverLabelManager.OpenLabel("Scene Saved");
        }
        catch (IOException)
        {
            Debug.LogError("Error while saving scene");
            HoverLabelManager.OpenLabel("Error while saving scene");
        }
    }

    public static void Load()
    {
        var saveFilePath = "";
        if (File.Exists(SceneManager.DclProjectPath + "/dcl-edit/saves/save.json"))
        {
            saveFilePath = SceneManager.DclProjectPath + "/dcl-edit/saves/save.json";
        }
        else if (File.Exists(SceneManager.DclProjectPath + "/scene/scene.json"))
        {
            saveFilePath = SceneManager.DclProjectPath + "/scene/scene.json";
        }
        
        if(saveFilePath != "")
        {
            var reader = new StreamReader(saveFilePath);
            var entities = reader.ReadToEnd().FromJson();
            reader.Close();

            Entity.uniqueNumberCounter = entities.entityNumberCounter;

            foreach (var entity in SceneManager.Entities)
            {
                entity.doomed = true;
                Destroy(entity.gameObject);
            }

            foreach (var entity in entities.entities)
            {
                var newEntityGameObject = Instantiate(SceneManager.EntityTemplate, SceneManager.EntityParent);
                var newEntity = newEntityGameObject.GetComponent<Entity>();

                newEntity.CustomName = entity.name;
                newEntity.uniqueNumber = entity.uniqueNumber;
                newEntity.Exposed = entity.exposed;

                foreach (var component in entity.components)
                {
                    EntityComponent newComponent = component.name switch
                    {
                        "transform" => newEntityGameObject.AddComponent<TransformComponent>(),
                        "sphereShape" => newEntityGameObject.AddComponent<SphereShapeComponent>(),
                        "boxShape" => newEntityGameObject.AddComponent<BoxShapeComponent>(),
                        "GLTFShape" => newEntityGameObject.AddComponent<GLTFShapeComponent>(),
                        _ => throw new NotImplementedException("Unknown component name: " + component.name)
                    };

                    newComponent.ApplySpecificJson(component.specifics);
                }
            }

            SceneManager.ChangedHierarchy();
        }
        else
        {
            Debug.Log("Creating new Scene...");
        }
    }
}

public static class SceneSaveJsonHelper
{
    //public static T[] FromJson<T>(string json)
    //{
    //    return wrapper.Items;
    //}

    [Serializable]
    public class EntityList
    {
        public int entityNumberCounter;
        public List<Entity.Json> entities;// = new List<Entity.Json>();
    };

    public static EntityList FromJson(this string jsonString)
    {
        return JsonUtility.FromJson<EntityList>(jsonString);
    }

    public static string ToJson(this Entity[] entities)
    {

        var entityList = new EntityList()
        {
            entities = entities.Select(e => new Entity.Json(e)).ToList(),
            entityNumberCounter = Entity.uniqueNumberCounter
        };

        return JsonUtility.ToJson(entityList,true);
    }

    public static string Indent(this String s)
    {
        return "    "+s.Replace("\n", "\n    ");
    }
    
}

