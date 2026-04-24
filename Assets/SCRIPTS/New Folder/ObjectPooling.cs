using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
/// <summary>
/// Como hacer object pooling con arreglos o listas
/// </summary>
public class ObjectSpawn : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize; //Cantidad de objetos que va a tener la fila
    [SerializeField] private int maxObjInScene; //Cantidad ibjetos maximos a la vez
    [SerializeField] private int enabledObjects; //Objetos Activos

    [SerializeField] private int spawnRate;
    [SerializeField] private int enQueueTime;

    Queue<GameObject> pool; //Esto es una fila
    private void Awake()
    {
        pool = new Queue<GameObject>();
    }
    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {

            GameObject instance = Instantiate(prefab, RandomPosition(), Quaternion.identity);
            instance.transform.SetParent(transform);
            instance.SetActive(false);
            pool.Enqueue(instance);

        }
        //spawnObjects = StartCoroutine(SpawnCollectibles());
        //StartCoroutine(ObjPooling());
    }
    #region Testeo ObjPooling en Corrutina
    private IEnumerator ObjPooling()
    {
        for (int i = 0; enabledObjects < maxObjInScene; i++)
        {
            yield return new WaitForSeconds(spawnRate);
            GameObject obj = ObjDeLaFila();
            obj.SetActive(true);
            enabledObjects++;
            StartCoroutine(MeterALaFila(obj));
        }
    }
    private GameObject ObjDeLaFila()
    {
        return pool.Dequeue();
    }
    private IEnumerator MeterALaFila(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(enQueueTime);
        pool.Enqueue(obj);
        enabledObjects--;
    }
    #endregion
    /// <summary>
    /// Debe de empezar a hacer aparecer los coleccionables, se debe detener cuando llegue al tope.
    /// Si alguien consigue un coleccionable, debe de volver a activarse para apawnear 
    /// los coleccionables faltantes para llegar al tope.
    /// </summary>
    /*private IEnumerator SpawnCollectibles()
    {
        //StartCorroutine(ObjectPooling());
    }*/

    /// <summary>
    /// Lo que hara este metodo es conseguir uno de los objetos que se encuentren activos
    /// </summary>
    private Vector3 RandomPosition()
    {
        float x = Random.Range(-5, 5);
        float y = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);

        return new Vector3(x,y,z);

    }

    [ContextMenu("Saludar")]
    public void Imprimir()
    {
        Debug.Log("Holi");
    }
}
