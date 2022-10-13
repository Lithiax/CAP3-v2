using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnxButtonSpawner : MonoBehaviour
{
    public GameObject SpawnArea;
    RectTransform spawnTransform;
    Vector3[] spawnAreaBounds;
    public GameObject ButtonPrefab;
    // Start is called before the first frame update
    void Start()
    {
        spawnTransform = SpawnArea.GetComponent<RectTransform>();
        spawnAreaBounds = new Vector3[4];
        spawnTransform.GetWorldCorners(spawnAreaBounds);

        for (int i = 0; i < 4; i++)
        {
            Debug.Log("World Corner " + i + " : " + spawnAreaBounds[i]);
        }

        InvokeRepeating("SpawnButton", 0f, 1f);
    }

    void SpawnButton()
    {
        GameObject button = GameObject.Instantiate(ButtonPrefab, SpawnArea.transform);
        button.transform.position = GetRandomSpawnInBounds();

        AnxButton script = button.GetComponent<AnxButton>();

        script.SetLifeSpan(3f);
    }

    Vector3 GetRandomSpawnInBounds()
    {
        float x = Random.Range(spawnAreaBounds[0].x, spawnAreaBounds[3].x);
        float y = Random.Range(spawnAreaBounds[0].y, spawnAreaBounds[1].y);
        float z = spawnAreaBounds[0].z;

        return new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {

        }
    }
}
