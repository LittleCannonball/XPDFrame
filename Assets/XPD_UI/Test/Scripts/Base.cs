using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TreeEditor;
using UnityEngine;
using XPD.UI;

//          1
//         / \
//        2   3
//       /|   |
//      4 5   6
//     / /|\
//    7 8 9 10

public class Base : MonoBehaviour
{
    void Start()
    {
        // 创建一棵树
        TreeNode root = new TreeNode(1);
        TreeNode node2 = new TreeNode(2);
        TreeNode node3 = new TreeNode(3);
        TreeNode node4 = new TreeNode(4);
        TreeNode node5 = new TreeNode(5);
        TreeNode node6 = new TreeNode(6);
        TreeNode node7 = new TreeNode(7);
        TreeNode node8 = new TreeNode(8);
        TreeNode node9 = new TreeNode(9);
        TreeNode node10 = new TreeNode(10);

        //root.children.Add(node2);
        //root.children.Add(node3);
        //node2.children.Add(node4);
        //node2.children.Add(node5);
        //node3.children.Add(node6);
        //node4.children.Add(node7);
        //node5.children.Add(node8);
        //node5.children.Add(node9);
        //node5.children.Add(node10);

        // 获取最深梯队的所有节点
        //List<TreeNode> deepestNodes = GetDeepestNodes(root);

        //// 输出最深梯队的所有节点
        //foreach (TreeNode node in deepestNodes)
        //{
        //    Debug.Log("最深梯队节点: " + node.value);
        //}

        UIFrame.Singleton.ShowUIPanel<MainParentController>();
    }

    List<TreeNode> GetDeepestNodes(TreeNode root)
    {
        if (root == null)
        {
            return new List<TreeNode>();
        }

        Queue<TreeNode> queue = new Queue<TreeNode>();
        queue.Enqueue(root);

        List<TreeNode> currentLevelNodes = new List<TreeNode>();

        while (queue.Count > 0)
        {
            int levelSize = queue.Count;
            currentLevelNodes.Clear();

            for (int i = 0; i < levelSize; i++)
            {
                TreeNode currentNode = queue.Dequeue();
                currentLevelNodes.Add(currentNode);

                foreach (TreeNode child in currentNode.children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        return currentLevelNodes;
    }
}


public class TreeNode
{
    public int value;
    public List<TreeNode> children;

    public TreeNode(int value)
    {
        this.value = value;
        this.children = new List<TreeNode>();
    }
}

public class Bast
{
    public Bast()
    {
        
    }
}

public class Chile : Bast
{

}


public class Chile2 : Bast
{
    public string name;
    ~Chile2()
    {
        Debug.Log(name);
    }
}