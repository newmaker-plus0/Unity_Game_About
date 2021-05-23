using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StoreSystem : MonoBehaviour
{
    public static StoreSystem instance;

    //單例
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Save()
    {
        //SaveBySerialization();
        SaveByJSON();
    }

    public int Load()
    {
        int exit = LoadByJSON();
        //exit = LoadByDeSerialization();
        return exit;
    }

    /*private Save CreateSaveGameObject()
    {
        Save save = new Save
        {
            //List<Struct.groupMode> tem = new List<Struct.groupMode>(Store.instance.group);
            //save.groupSave = tem;
            playerSave = Store.instance.player
        };

        return save;
    }*/

    private void SaveByJSON()
    {
        //Save save = CreateSaveGameObject();
        Struct.Info playerSave = Store.instance.player;
        //Struct.groupMode groupSave = Store.instance.group[0];

        string jsonString = JsonUtility.ToJson(playerSave);
        //string jsonString1 = JsonUtility.ToJson(groupSave);
        StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/Data.text");
        //StreamWriter sw1 = new StreamWriter(Application.persistentDataPath + "/DataGroup.text");
        sw.Write(jsonString);
        //sw1.Write(jsonString1);
        sw.Close();
        //sw1.Close();

        for(int i=0;i<Store.instance.group.Length;i++)
        {
            Struct.groupMode groupSave = Store.instance.group[i];
            string groupString = JsonUtility.ToJson(groupSave);
            for(int j=0;j<Store.instance.group[i].word.Length;j++)
            {
                Struct.wordMode wordSave = Store.instance.group[i].word[j];
                groupString += "\n" + JsonUtility.ToJson(wordSave);
            }
            //Debug.Log(groupString);
            StreamWriter swGroup = new StreamWriter(Application.persistentDataPath + "/DataGroup" + i + ".text");
            swGroup.Write(groupString);
            swGroup.Close();
        }
    }

    private int LoadByJSON()
    {
        int exit = 0;

        if (File.Exists(Application.persistentDataPath + "/Data.text"))
        {
            WaringSystem.instance.Show("加載存檔中...");

            StreamReader sr = new StreamReader(Application.persistentDataPath + "/Data.text");
            //StreamReader sr1 = new StreamReader(Application.persistentDataPath + "/DataGroup.text");
            string jsonString = sr.ReadToEnd();
            //string jsonString1 = sr1.ReadToEnd();
            sr.Close();
            //sr1.Close();
            //Debug.Log(jsonString);
            //Debug.Log(jsonString1);

            //Save save = JsonUtility.FromJson<Save>(jsonString);
            Store.instance.player = JsonUtility.FromJson<Struct.Info>(jsonString);
            //Store.instance.group[0] = JsonUtility.FromJson<Struct.groupMode>(jsonString1);

            for(int i=0;i<8;i++)
            {
                if(File.Exists(Application.persistentDataPath + "/DataGroup" + i + ".text"))
                {
                    StreamReader srGroup = new StreamReader(Application.persistentDataPath + "/DataGroup" + i + ".text");
                    string groupString = srGroup.ReadLine();

                    //轉成List添加群組
                    List<Struct.groupMode> temGroup = new List<Struct.groupMode>(Store.instance.group);
                    Struct.groupMode groupMode;
                    groupMode.name = "";
                    groupMode.wordNum = 0;
                    groupMode.correct = 0;
                    groupMode.error = 0;
                    groupMode.ratio = 0f;
                    groupMode.word = new Struct.wordMode[0];
                    temGroup.Add(groupMode);
                    
                    //轉回去
                    Store.instance.group = temGroup.ToArray();

                    //Debug.Log(groupString);
                    Store.instance.group[i] = JsonUtility.FromJson<Struct.groupMode>(groupString);
                    Store.instance.group[i].word = new Struct.wordMode[0];
                    int j = 0;

                    while(srGroup.Peek() >= 0)
                    {
                        string wordString = srGroup.ReadLine();
                        //Debug.Log(wordString);

                        //轉成List添加單字
                        List<Struct.wordMode> temWord = new List<Struct.wordMode>(Store.instance.group[i].word);
                        Struct.wordMode word;
                        word.foreign = "";
                        word.native = "";
                        word.correct = 0;
                        word.error = 0;
                        word.ratio = 0f;
                        temWord.Add(word);

                        //轉回去
                        Store.instance.group[i].word = temWord.ToArray();

                        Store.instance.group[i].word[j] = JsonUtility.FromJson<Struct.wordMode>(wordString);
                        j++;                       
                    }
                    
                    BasicSystem.instance.LoadStoreGroup();
                    srGroup.Close();
                }
            }

            exit = 1;
        }
        else
        {
            WaringSystem.instance.Show("無加載存檔...");
        }

        return exit;
    }

    /*private void SaveBySerialization()
    {
        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();

        FileStream fileStream = File.Create(Application.persistentDataPath + "/Data.text");

        bf.Serialize(fileStream, save);

        fileStream.Close();
    }

    private int LoadByDeSerialization()
    {
        int exit = 0;

        if(File.Exists(Application.persistentDataPath + "/Data.text"))
        {
            WaringSystem.instance.Show("加載存檔中...");

            BinaryFormatter bf = new BinaryFormatter();

            FileStream fileStream = File.Open(Application.persistentDataPath + "/Data.text", FileMode.Open);

            Save save = bf.Deserialize(fileStream) as Save;

            fileStream.Close();

            List<Struct.groupMode> tem = new List<Struct.groupMode>(save.groupSave);
            Store.instance.group = tem.ToArray();
            //Store.instance.player = save.playerSave;

            BasicSystem.instance.LoadStoreGroup();

            exit = 1;
        }
        else
        {
            WaringSystem.instance.Show("無加載存檔...");
        }

        return exit;
    }*/

    public void DeleteFile()
    {
        if (File.Exists(Application.persistentDataPath + "/Data.text"))
        {
            WaringSystem.instance.Show("存檔刪除中...");

            File.Delete(Application.persistentDataPath + "/Data.text");

            for(int i=0;i<8;i++)
            {
                if(File.Exists(Application.persistentDataPath + "/DataGroup" + i + ".text"))
                {
                    File.Delete(Application.persistentDataPath + "/DataGroup" + i + ".text");
                }
            }
        }
        else
        {
            WaringSystem.instance.Show("無存檔可刪除...");
        }
    }
}
