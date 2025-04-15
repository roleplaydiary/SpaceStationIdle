using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBenchController : MonoBehaviour
{
    [SerializeField] private Transform workPosition;
    
    public void Start()
    {
        this.gameObject.SetActive(false);
    }

    public Transform GetWorkPosition()
    {
        return this.workPosition;
    }
}
