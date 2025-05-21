using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objects;
    public int PropIndex;
    public int ObjectIndex;

    private void Start()
    {
        StartCoroutine(nameof(RandomFunc));
    }

    private IEnumerator RandomFunc()
    {
        while (true)
        {
            ObjectIndex = GetUniqueIndex(ObjectIndex);
            PropIndex = GetUniqueIndex(PropIndex);
            Debug.Log($"Object => {ObjectIndex} **** PropIndex => {Enum.GetValues(typeof(MyProp)).GetValue(PropIndex+1)}");
            switch (PropIndex)
            {
                case 0: // Rotate
                    _objects[ObjectIndex].transform.Rotate(90, 0f, 0);
                    break;
                case 1: //Scale
                    _objects[ObjectIndex].transform.localScale += new Vector3(0.5f, 0,0);
                    break;
                case 2: // Color
                    _objects[ObjectIndex].GetComponent<MeshRenderer>().material.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    break;
                case 3: // Move
                    _objects[ObjectIndex].transform.position += new Vector3(_objects[ObjectIndex].transform.position.x > 5 ? -0.15f : 0.5f, 0, 0);
                    break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private int GetUniqueIndex(int ignoreVal)
    {
        int new_val = ignoreVal;
        while (new_val == ignoreVal)
        {
            new_val = UnityEngine.Random.Range(0, 4);
        }
        return new_val;
    }
}

[System.Serializable]
public enum MyProp
{
    None = 0,
    Rotate = 1,
    Scale = 2,
    Color = 3,
    Move = 4
}