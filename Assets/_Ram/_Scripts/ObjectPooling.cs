using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [Header("Reference Objects")]
    [SerializeField] private RectTransform[] endPoints;
    [SerializeField] private GameObject coinObj;
    [SerializeField] private GameObject pointObj;

    [Header("ObjectPooling")]
    private List<GameObject> objectPooling = new List<GameObject>();
    private GameObject currentObj;
    // 0 - x min *** 1 - x max *** 2 - y min *** 3 - y max
    private Vector3[] spawnArea = new Vector3[4]; 

    private void Start()
    {
        for (int i = 0; i < endPoints.Length; i++)
        {
            RectTransform point = endPoints[i];
            RectTransformUtility.ScreenPointToWorldPointInRectangle(point, point.position, Camera.main, out spawnArea[i]);
        }
        for (int i = 0; i < spawnArea.Length; i++)
        {
            Instantiate(pointObj, spawnArea[i], Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            objectPooling.Add(Instantiate(coinObj, transform.position, Quaternion.identity));
        }
    }

    public IEnumerator StartShowingCoins()
    {
        while (GameManager.instance.CurrentTime > 0)
        {
            currentObj = GetObjectPool();
            currentObj.transform.position = new Vector3(UnityEngine.Random.Range(spawnArea[0].x, spawnArea[1].x), UnityEngine.Random.Range(spawnArea[2].y, spawnArea[3].y));
            currentObj.SetActive(true);
            objectPooling.Add(currentObj);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0, 3));
            currentObj.SetActive(false);
        }
    }

    private GameObject GetObjectPool()
    {
        foreach (GameObject obj in objectPooling)
        {
            if (!obj.activeSelf)
            {
                objectPooling.Remove(obj);
                return obj;
            }
        }

        return objectPooling[0];
    }
}
