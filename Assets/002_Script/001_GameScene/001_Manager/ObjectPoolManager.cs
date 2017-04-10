using UnityEngine;
using System.Collections;

public class ObjectPoolManager : MonoBehaviour {

    // Object Pool  [ 아군 몬스터 , 아군 장애물 ] //

    private static ObjectPoolManager instance;

    public static ObjectPoolManager getInstance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.Find("ObjectPool").GetComponent<ObjectPoolManager>();
            }
            return instance;
        }
    }

    public GameObject Prefabs;
    public GameObject ObjectSpace;
    
    private ArrayList []obj_list;
    
    enum Obj_Enum { one_Cemetry = 0, two_Cemetry,one_Ghost,two_Ghost, endEnum};


	// Use this for initialization
	void Start () {

        obj_list = new ArrayList[4];

        for(int i =0; i<(int)Obj_Enum.endEnum; i++)
        {
            obj_list[i] = new ArrayList();
        }

        CreateObject();

    }

    private const int createCount = 10;

    private void CreateObject(string objName = null)
    {
        if(objName == null)
        {
            int length = Prefabs.transform.childCount;

            for(int i =0; i< length; i++)
            {
                CreateObject(Prefabs.transform.GetChild(i).name);
            }


        }
        else
        {
            GameObject CopyObj = Prefabs.transform.FindChild(objName).gameObject;
            GameObject InstantiatedObj;
            for(int i =0; i<createCount;i++)
            {
                InstantiatedObj = Instantiate(CopyObj) as GameObject;
                InstantiatedObj.name = CopyObj.name;
                PoolObject(InstantiatedObj);
            }
        }
    }


    public void PoolObject(GameObject obj)
    {
        string obj_name = obj.name;

        switch (obj_name)
        {
            case "oneCemetry":
                obj_list[(int)Obj_Enum.one_Cemetry].Add(obj);
                break;
            case "twoCemetry":
                obj_list[(int)Obj_Enum.two_Cemetry].Add(obj);
                break;
            case "oneGhost":
                obj_list[(int)Obj_Enum.one_Ghost].Add(obj);
                break;
            case "twoGhost":
                obj_list[(int)Obj_Enum.two_Ghost].Add(obj);
                break;
            default:
                break;
        }
        obj.transform.parent = ObjectSpace.transform;
        obj.transform.localPosition = Vector3.zero;

    }

    public GameObject GetObject(string obj_name)
    {
        GameObject obj = null;

        switch (obj_name)
        {
            case "oneCemetry":
                if (obj_list[(int)Obj_Enum.one_Cemetry].Count > 0)
                {
                    obj =(GameObject)obj_list[(int)Obj_Enum.one_Cemetry][0];
                    obj_list[(int)Obj_Enum.one_Cemetry].RemoveAt(0);
                }
                else
                {
                    CreateObject(obj_name);
                    obj = GetObject(obj_name);
                }
                break;
            case "twoCemetry":
                if (obj_list[(int)Obj_Enum.two_Cemetry].Count > 0)
                {
                    obj = (GameObject)obj_list[(int)Obj_Enum.two_Cemetry][0];
                    obj_list[(int)Obj_Enum.two_Cemetry].RemoveAt(0);
                }
                else
                {
                    CreateObject(obj_name);
                    obj = GetObject(obj_name);
                }
                break;
            case "oneGhost":
                if (obj_list[(int)Obj_Enum.one_Ghost].Count > 0)
                {
                    obj = (GameObject)obj_list[(int)Obj_Enum.one_Ghost][0];
                    obj_list[(int)Obj_Enum.one_Ghost].RemoveAt(0);
                }
                else
                {
                    CreateObject(obj_name);
                    obj = GetObject(obj_name);
                }
                break;
            case "twoGhost":
                if (obj_list[(int)Obj_Enum.two_Ghost].Count > 0)
                {
                    obj = (GameObject)obj_list[(int)Obj_Enum.two_Ghost][0];
                    obj_list[(int)Obj_Enum.two_Ghost].RemoveAt(0);
                }
                else
                {
                    CreateObject(obj_name);
                    obj = GetObject(obj_name);
                }
                break;
            default:

                break;
        }
        
        return obj;
    }

}
