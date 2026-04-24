using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{

    [SerializeField] private List <string> buffs;
    ObjectPooling2 objectPooling;
    private void Awake()
    {
        objectPooling = FindObjectOfType<ObjectPooling2>();

    }

    private void OnTriggerEnter(Collider other)
    {
        

        switch (other.tag)
        {
            case "TicketBase":
                {
                    objectPooling. ReturnToList(other.transform,other.gameObject);
                    break;
                }
            case "TicketMediano":
                {
                    objectPooling.ReturnToList(other.transform, other.gameObject);
                    break;
                }
            case "TicketGrande":
                {
                    objectPooling.ReturnToList(other.transform, other.gameObject);
                    break;
                }

            case var buff when buffs.Contains(other.tag):
                {
                    break;
                }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
    }
}
