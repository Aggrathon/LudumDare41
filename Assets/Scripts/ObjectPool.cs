using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

	private static ObjectPool instance;

	Dictionary<GameObject, List<GameObject>> pool;


	private void Awake()
	{
		instance = this;
		pool = new Dictionary<GameObject, List<GameObject>>();
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position)
	{
		List<GameObject> list;
		if (instance.pool.TryGetValue(prefab, out list))
		{
			if (list.Count == 0)
			{
				var go = Instantiate<GameObject>(prefab, position, Quaternion.identity, instance.transform);
				go.AddComponent<PoolObject>().prefab = prefab;
				return go;
			}
			else
			{
				var go = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				go.transform.position = position;
				go.SetActive(true);
				return go;
			}
		}
		else
		{
			list = new List<GameObject>();
			instance.pool.Add(prefab, list);
			var go = Instantiate<GameObject>(prefab, position, Quaternion.identity, instance.transform);
			go.AddComponent<PoolObject>().prefab = prefab;
			return go;
		}
	}

	public static void Recycle(PoolObject po)
	{
		if (instance != null)
			instance.pool[po.prefab].Add(po.gameObject);
	}

	public class PoolObject : MonoBehaviour
	{
		public GameObject prefab;

		private void OnDisable()
		{
			ObjectPool.Recycle(this);
		}
	}
}
