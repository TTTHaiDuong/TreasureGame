using GameUI;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public Block BaseBlock;
    public bool Flag;

    private void Update()
    {
        if (QuestionFactory.QuestionTable != null && !Flag && Player.GetOwner() != null && Player.GetOwner().IsEnterGame) InitIsland();
    }

    public void InitIsland()
    {
        Debug.Log("InitIsland");
        Flag = true;

        float y = -1;
        foreach (Transform block in transform)
        {
            if (block.CompareTag("TopBlock"))
            {
                GameObject newBlock = Instantiate(BaseBlock.gameObject, block.transform.position, Quaternion.identity);
                Vector3 newPosition = newBlock.transform.position;
                newPosition.y = y;
                newBlock.transform.position = newPosition;
                newBlock.transform.parent = transform;
                Destroy(block.gameObject);
            }
            if (block.CompareTag("BottomBlock")) Destroy(block.gameObject);
        }

        BaseBlock.gameObject.SetActive(false);
        Player.GetOwner().RandomPositionSpawn();
        Debug.Log("Init island");
    }

    public Block[] GetAllBlocks()
    {
        List<Block> result = new();
        foreach (Transform transform in transform)
            if (transform.CompareTag("BaseBlock")) result.Add(transform.GetComponent<Block>());
        return result.ToArray();
    }
}
