using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool<TGameObjectPoolable> : MonobehaviorSingleton<GameObjectPool<TGameObjectPoolable>> where TGameObjectPoolable : MonoBehaviour
{
    TGameObjectPoolable m_schemaInstance;
    private Transform m_activeRoot;
    private Transform m_inactiveRoot;
    private Queue<TGameObjectPoolable> m_inactiveGamePoolables = new Queue<TGameObjectPoolable>();

    private const string c_activeRootName = "Active Game Objects";
    private const string c_inactiveRootName = "Inactive Game Objects";

    public void Start()
    {
        m_activeRoot = new GameObject(c_activeRootName).transform;
        m_activeRoot.SetParent(transform);
        m_inactiveRoot = new GameObject(c_inactiveRootName).transform;
        m_inactiveRoot.SetParent(transform);
    }

    public void SetSchema(TGameObjectPoolable instance)
    {
        m_schemaInstance = instance;
    }

    public virtual TGameObjectPoolable Take()
    {
        TGameObjectPoolable instance;
        if (m_inactiveGamePoolables.Count == 0)
        {
            GameObject gameObject = Instantiate(m_schemaInstance.gameObject);
            instance = gameObject.GetComponent<TGameObjectPoolable>();
        }
        else
            instance = m_inactiveGamePoolables.Dequeue();

        instance.transform.parent = m_activeRoot;
        instance.gameObject.SetActive(true);
        return instance;
    }

    public virtual void Return(TGameObjectPoolable instance)
    {
        instance.gameObject.SetActive(false);
        instance.transform.parent = m_inactiveRoot;
        m_inactiveGamePoolables.Enqueue(instance);
    }
}
