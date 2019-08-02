using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedPooler;

    //public Dictionary<int, GameObject> pooledObjects = new Dictionary <int, GameObject>();
    
    public List<List<GameObject>> pooledObjects = new List<List<GameObject>>();

    public List<GameObject> objectsToPool;
    public List<int> amountsToPool;

    public List<AudioClip> SoundEffects;

    public List<Material> NeededMaterials;

    public List<AI> AItypes;

    public AudioSource Music;
    public AudioSource Sound;

    void Awake()
    {
        SharedPooler = this;
        for (int j = 0; j < objectsToPool.Count; j++)
        {
            pooledObjects.Add(new List<GameObject>());
            for (int i = 0; i < amountsToPool[j]; i++)
            {
                GameObject obj = (GameObject)Instantiate(objectsToPool[j]);
                obj.SetActive(false);
                pooledObjects[j].Add(obj);
            }  
        }
    }

    public GameObject GetPooledObject(int index)
    {
        for (int i = 0; i < pooledObjects[index].Count; i++)
        {
            if (!pooledObjects[index][i].activeInHierarchy)
                return pooledObjects[index][i];
        }

        GameObject obj = (GameObject)Instantiate(objectsToPool[index]);
        obj.SetActive(false);
        pooledObjects[index].Add(obj);
        return obj;
    }

    public void PlaySound(int sound, bool Musictrack)
	{
		if(Musictrack)
		{
			Music.clip = SoundEffects[sound];
			Music.volume = GameControl.control.rules.Musicvolume;
			Music.Play();
		}
		else
		{
			Sound.PlayOneShot(SoundEffects[sound], GameControl.control.rules.Soundvolume);
		}
	}
}