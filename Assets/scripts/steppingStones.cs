using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class steppingStones : MonoBehaviour
{
    // Start is called before the first frame update
    // Randomly generated stepping paths over canyon
    //some of the stones should break if you step on them or disappear
    //there should be one path to start that branches off multiple times
    //at least One path should make it to the other side without
    [System.Serializable]
    public class SteppingStones {
          public float canyonLength = 45.0f;
          public float canyonWidth = 20.0f;
          public float stoneSize = 1.0f;
          public float stoneSpacing = 2.0f;
          public float stoneHeight = 1.0f;
          public Material stoneMaterial;
          public GameObject stonePrefab;

          public GameObject crumblingStonePrefab;

          //tree parameters
          public int branchingFactor = 3;
          public int treeHeight = 3;

          public int num_of_paths = 3;

    }

    [SerializeField]
    private SteppingStones stones;

    private GameObject[] stoneArray = null;

    private bool stonesOutOfDate = false;


    Vector3[] GeneratePath(Vector3 start, Vector3 end, int num_of_stones) {
          Vector3[] stonePositions = new Vector3[num_of_stones];
          for (int i = 0; i < num_of_stones; i++){
                Vector3 stonePosition = Vector3.Lerp(start, end, (float)i/(float)num_of_stones);
                stonePositions[i] = stonePosition;
          }   
          return stonePositions;     
    }

    Vector3 GenerateTreeNode(int i, float length, float width, int treeHeight, int branchingFactor) {
            int node_height = Mathf.FloorToInt(Mathf.Log(i * (branchingFactor - 1) + 1, branchingFactor));
            int layer_size = Mathf.FloorToInt(Mathf.Pow(branchingFactor, node_height));
            int node_index = i - (layer_size - 1) / (branchingFactor - 1);
            float node_x = width * ((float)(node_index+1) / (float)(layer_size + 1)) - width / 2;
            float node_z = length * (float)(node_height) / (float)(treeHeight - 1);
            return new Vector3(node_x, 0, node_z);
    }

    Vector3[] GenerateTree(float length, float width, int treeHeight, int branchingFactor, float stoneSize, float stonesSpacing){
        int stonesPerBranch = Mathf.FloorToInt((length/treeHeight) / (stoneSize + stonesSpacing));
        int num_of_nodes = Mathf.FloorToInt(Mathf.Pow(branchingFactor, treeHeight) - 1) / (branchingFactor - 1);
        Vector3[] nodePositions = new Vector3[num_of_nodes];
        for (int i = 0; i < num_of_nodes; i++) {
            nodePositions[i] = GenerateTreeNode(i, length, width, treeHeight, branchingFactor);
        }
        return nodePositions;
    }

    int[] UniqueRandomNumbersList(int minVal, int maxVal, int length) {
        List<int> numbers = new List<int>();
        while (numbers.Count < length) {
            int a = Random.Range (minVal, maxVal );
            if(!numbers.Contains(a)) {
                numbers.Add(a);
            }
        }
        return numbers.ToArray();
    }

    int[] GetAncestorsOfNode(int node, Vector3[] nodePositions, List<int> ancestors) {
      if (node == 0) {
        return ancestors.ToArray();
      }
      int parent = Mathf.FloorToInt((node - 1) / stones.branchingFactor);
      ancestors.Add(parent);
      return GetAncestorsOfNode(parent, nodePositions, ancestors);

    }

    Vector3[] GeneratePathsTowardRoot(int[] nodes, Vector3[] nodePositions, int branchingFactor, float stoneSize, float stonesSpacing) {
        List<Vector3> pathBuilder = new List<Vector3>();
        for (int i = 0; i < nodes.Length; i++) {
            int node = nodes[i];
            int parent = Mathf.FloorToInt((node - 1) / branchingFactor);
            float distance = Vector3.Distance(nodePositions[node], nodePositions[parent]);
            int stonesPerBranch = Mathf.FloorToInt((distance) / (stoneSize + stonesSpacing));
            Vector3[] path = GeneratePath(nodePositions[node], nodePositions[parent], stonesPerBranch);
            pathBuilder.AddRange(path);
        }
        return pathBuilder.ToArray();
    }

    Vector3[] GeneratePathsToRoot(int node, Vector3[] nodePositions, List<Vector3> pathBuilder, int branchingFactor, float stoneSize, float stonesSpacing) {
        //generate a path from the node to the root
        //add the path to the path builder
        //return the path builder
        if (node == 0) {
            return pathBuilder.ToArray();
        }
        int parent = Mathf.FloorToInt((node - 1) / branchingFactor);
        float distance = Vector3.Distance(nodePositions[node], nodePositions[parent]);
        int stonesPerBranch = Mathf.FloorToInt((distance) / (stoneSize + stonesSpacing));
        Vector3[] path = GeneratePath(nodePositions[node], nodePositions[parent], stonesPerBranch);
        pathBuilder.AddRange(path);
        return GeneratePathsToRoot(parent, nodePositions, pathBuilder, branchingFactor, stoneSize, stonesSpacing);
    }

    Vector3[] GenerateRandomsPathsFromTree(float length, float width, int treeHeight, int branchingFactor, float stoneSize, float stonesSpacing, int num_of_paths) {
          Vector3[] nodePositions = GenerateTree(length, width, treeHeight, branchingFactor, stoneSize, stonesSpacing);
          int num_of_nodes = nodePositions.Length;
          List<Vector3> stonePositions = new List<Vector3>();

          int leafMinIndex = Mathf.FloorToInt(Mathf.Pow(branchingFactor, treeHeight - 1) - 1) / (branchingFactor - 1);
          int leafMaxIndex = num_of_nodes;
          int[] leafNodesArray = UniqueRandomNumbersList(leafMinIndex, leafMaxIndex, num_of_paths);


          for (int i = 0; i < num_of_paths; i++) {
              // generate a path from the leaf to the root
              Vector3[] pathToRoot = GeneratePathsToRoot(leafNodesArray[i], nodePositions, new List<Vector3>(), branchingFactor, stoneSize, stonesSpacing);
              // lower one stone on the path to the root
              if (pathToRoot.Length == 0) {
                  continue;
              }
              int randint = Random.Range(0, pathToRoot.Length-1);
              pathToRoot[randint].y -= 0.1f;
              stonePositions.AddRange(pathToRoot);
          }

          // generate a path from the root to the leaf that is not lowered
          Vector3[] pathToRoot2 = GeneratePathsToRoot(num_of_paths - 2, nodePositions, new List<Vector3>(), branchingFactor, stoneSize, stonesSpacing);
          stonePositions.AddRange(pathToRoot2);

          return stonePositions.ToArray();
    }

    Vector3[] GenerateDirectTreePath(float length, float width, int treeHeight, int branchingFactor, float stoneSize, float stonesSpacing) {


        Vector3[] nodePositions = GenerateTree(length, width, treeHeight, branchingFactor, stoneSize, stonesSpacing);
        int num_of_nodes = nodePositions.Length;
        List<Vector3> stonePositions = new List<Vector3>();

        //need to generate all the points/nodes that would be in the tree
        //need to make sensable connections between the nodes, should be connected in a 
        
        for (int i = 1; i < num_of_nodes; i++) {
            int parent = Mathf.FloorToInt((i - 1) / branchingFactor);
            //connect the node to its parent
            //generate a path between the two nodes
            float distance = Vector3.Distance(nodePositions[i], nodePositions[parent]);
            int stonesPerBranch = Mathf.FloorToInt((distance) / (stoneSize + stonesSpacing));
            Vector3[] path = GeneratePath(nodePositions[i], nodePositions[parent], stonesPerBranch);
            //add the path to the stone positions
            stonePositions.AddRange(path);
        }
        return stonePositions.ToArray();
    }

    Vector3[] GenerateSquareTreePath(float length, float width, int treeHeight, int branchingFactor, float stoneSize, float stonesSpacing) {
        // 
        Vector3[] nodePositions = GenerateTree(length, width, treeHeight, branchingFactor, stoneSize, stonesSpacing);
        int num_of_nodes = nodePositions.Length;
        List<Vector3> stonePositions = new List<Vector3>();

        // need to generate points between the nodes, but instead of directly connecting them, we should just make vertical and horisontal paths
        // this means generating a straight vertical path from the child to the parents z position
        // then generating a straight horizontal path from the parent to the childs with the smallest x position, and the child with the greatest x position

        //vertical vertical paths
        int stonesPerBranch = Mathf.FloorToInt((length / treeHeight) / (stoneSize + stonesSpacing));
        for (int i = 1; i < num_of_nodes; i++) {
            int parent = Mathf.FloorToInt((i - 1) / branchingFactor);
            //connect the node to its parent
            //generate a path between the two nodes
            Vector3[] path = GeneratePath(nodePositions[i], new Vector3(nodePositions[i].x, nodePositions[i].y, nodePositions[parent].z), stonesPerBranch);
            stonePositions.AddRange(path);
        }

        //horizontal paths
        // for each parent node, generate a path to the child with the smallest x position, and the child with the greatest x position
        // only need to do this for nodes that have children, so we can skip the last layer
        // for (int i=0; i < (num_of_nodes - branchingFactor^(treeHeight - 1)); i++) {
        //     int child = i * branchingFactor;
        //     stonesPerBranch = Mathf.FloorToInt(Math.Abs(nodePositions[child].x - nodePositions[i].x) / (stoneSize + stonesSpacing));
        //     Debug.Log("stones size and stone spacing:" + stoneSize + " " + stonesSpacing);
        //     Debug.Log(stonesPerBranch);
        //     Vector3[] path = GeneratePath(nodePositions[i], new Vector3(nodePositions[child].x, nodePositions[i].y, nodePositions[i].z), stonesPerBranch);
        //     stonePositions.AddRange(path);

        //     child = i * branchingFactor + branchingFactor - 1;
        //     stonesPerBranch = Mathf.FloorToInt(Math.Abs(nodePositions[child].x - nodePositions[i].x) / (stoneSize + stonesSpacing));
        //     path = GeneratePath(nodePositions[i], new Vector3(nodePositions[child].x, nodePositions[i].y, nodePositions[i].z), stonesPerBranch);
        //     stonePositions.AddRange(path);
        // }

        return stonePositions.ToArray();
    }

    Vector3[] GenerateStraightPath() {


        //Stone position array
        int num_of_stones = (int)(stones.canyonLength / (stones.stoneSize + stones.stoneSpacing));

        Vector3 start = new Vector3(0, 0, 0);
        Vector3 end = new Vector3(0, 0, stones.canyonLength);

        Vector3[] stonePositions = GeneratePath(start, end, num_of_stones);

        return stonePositions;
    }

    GameObject GenerateStone(Vector3 stonePosition) {
          // make the stones relative to the object this script is on
          // make the stones a child of the object this script is on
          GameObject stone = Instantiate(stones.stonePrefab, new Vector3(0,0,0), Quaternion.identity);
          
          // if the stone is lowered then make it a crumbling stone
          if (stonePosition.y < 0) {
              stone = Instantiate(stones.crumblingStonePrefab, new Vector3(0,0,0), Quaternion.identity);
          }
          stone.transform.parent = this.transform;
          stone.transform.localPosition = stonePosition;
          stone.transform.localScale = new Vector3(stones.stoneSize, stones.stoneHeight, stones.stoneSize);
          return stone;
    }

    GameObject[] GenerateStones(Vector3[] stonePositions) {
        // Should generate stones at the positions given
        // Should return a list of stones
        // the stones should be relative to the object this script is on
        // the stones should be a child of the object this script is on

        //Stone array
        GameObject[] stones = new GameObject[stonePositions.Length];

        for (int i = 0; i < stonePositions.Length; i++) {
            //set stone position
            stones[i] = GenerateStone(stonePositions[i]);
        }

        return stones;
    }

    void Start()
    {
        stoneArray = GenerateStones(GenerateRandomsPathsFromTree(stones.canyonLength, stones.canyonWidth, stones.treeHeight, stones.branchingFactor, stones.stoneSize, stones.stoneSpacing, stones.num_of_paths));
    }


    private void OnValidate()
    {

        stonesOutOfDate = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (stonesOutOfDate) {
            // Destroy all the stones
            foreach (GameObject stone in stoneArray) {
                DestroyImmediate(stone);
            }
            // Generate new stones
            stoneArray = GenerateStones(GenerateRandomsPathsFromTree(stones.canyonLength, stones.canyonWidth, stones.treeHeight, stones.branchingFactor, stones.stoneSize, stones.stoneSpacing, stones.num_of_paths));
            stonesOutOfDate = false;
        }
        
    }
}
