using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Island : MonoBehaviour
{
    public Block BaseBlock;

    private void Start()
    {
        InitIsland();
    }

    public void InitIsland()
    {
        float y = -1;
        foreach (Transform block in transform)
        {
            if (block.CompareTag("TopBlock"))
            {
                GameObject newBlock = Instantiate(BaseBlock.gameObject, block.transform.position, block.transform.rotation);
                Vector3 newPosition = newBlock.transform.position;
                newPosition.y = y;
                newBlock.transform.position = newPosition;
                newBlock.transform.parent = transform;
                Destroy(block.gameObject);
            }
            if (block.CompareTag("BottomBlock")) Destroy(block.gameObject);
        }

        Destroy(BaseBlock);
    }

    public Block[] GetAllBlocks()
    {
        List<Block> result = new();
        foreach (Transform transform in transform)
            if (transform.CompareTag("BaseBlock")) result.Add(transform.GetComponent<Block>());
        return result.ToArray();
    }
}
