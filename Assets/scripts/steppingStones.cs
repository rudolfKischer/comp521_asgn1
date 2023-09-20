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


    public static int[] UniqueRandomNumbersList(int minVal, int maxVal, int length) {
        List<int> numbers = new List<int>();
        while (numbers.Count < length) {
            int a = Random.Range (minVal, maxVal );
            if(!numbers.Contains(a)) {
                numbers.Add(a);
            }
        }
        return numbers.ToArray();
    }
    [System.Serializable]
    public class SteppingStones {
          public float canyonLength = 85.0f;
          public float canyonWidth = 80.0f;
          public float stoneSize = 4.5f;
          public float stoneSpacing = 3.0f;
          public float stoneHeight = 0.2f;
          public Material stoneMaterial;
          public GameObject stonePrefab;
          public GameObject crumblingStonePrefab;

          public float perStoneRevelationTime = 0.3f;

          [Range(0.0f, 1.0f)]
          public float crumblePercentage = 0.5f;

          //tree parameters
          public int branchingFactor = 2;
          public int treeHeight = 3;

          public int numOfPaths = 3;

          public Vector3[] GeneratePath(Vector3 start, Vector3 end) {
                int num_of_stones = StonesPerDistance(Vector3.Distance(start, end));
                Vector3[] stonePositions = new Vector3[num_of_stones];
                for (int i = 0; i < num_of_stones; i++){
                      Vector3 stonePosition = Vector3.Lerp(start, end, (float)i/(float)num_of_stones);
                      stonePositions[i] = stonePosition;
                }   
                return stonePositions;     
          }

          private int StonesPerDistance(float distance) {
              return Mathf.FloorToInt(distance / (this.stoneSize + this.stoneSpacing));
          }

          private int LayerSize(int layer) {
              return Mathf.FloorToInt(Mathf.Pow(this.branchingFactor, layer));
          }

          private int NodeHeight(int nodeIndex) {
              return Mathf.FloorToInt(Mathf.Log(nodeIndex * (this.branchingFactor - 1) + 1, this.branchingFactor));
          }

          private int NodeLayerIndex(int nodeIndex) {
              int nodeHeight = NodeHeight(nodeIndex);
              int layerSize = LayerSize(nodeHeight);
              return nodeIndex - (layerSize - 1) / (this.branchingFactor - 1);
          }

          private int NodeParentIndex(int nodeIndex) {
              return Mathf.FloorToInt((nodeIndex - 1) / this.branchingFactor);
          }

          private int NumberOfNodes() {
              return (LayerSize(treeHeight) - 1) / (this.branchingFactor - 1);
          }

          private float normalizedLayerPosition(int nodeIndex) {
              int nodeHeight = NodeHeight(nodeIndex);
              int layerSize = LayerSize(nodeHeight);
              int nodeLayerIndex = NodeLayerIndex(nodeIndex);
              return (float)(nodeLayerIndex+1) / (float)(layerSize + 1);
          }

          private float normalizedHeightPosition(int nodeIndex) {
              int nodeHeight = NodeHeight(nodeIndex);
              return (float)(nodeHeight) / (float)(this.treeHeight - 1);
          }

          public Vector3 GenerateTreeNode(int i) {
                  float node_x = this.canyonWidth * (normalizedLayerPosition(i) - 0.5f);
                  float node_z = this.canyonLength * normalizedHeightPosition(i);
                  return new Vector3(node_x, 0, node_z);
          }

          public Vector3[] GenerateTree(){
              int numOfNodes = NumberOfNodes();
              Vector3[] nodePositions = new Vector3[numOfNodes];
              for (int i = 0; i < numOfNodes; i++) {
                  nodePositions[i] = GenerateTreeNode(i);
              }
              return nodePositions;
          }

          int[] GetAncestorsOfNode(int node) {
              List<int> ancestors = new List<int>();
              ancestors.Add(node);
              while (node != 0) {
                  int parent = NodeParentIndex(node);
                  ancestors.Add(parent);
                  node = parent;
              }
              return ancestors.ToArray();
          }

          int[] GetUniqueAncestorsOfNode(int[] leaves) {
              List<int> ancestors = new List<int>();
              foreach (int leaf in leaves) {
                  ancestors.AddRange(GetAncestorsOfNode(leaf));
              }
              return ancestors.Distinct().ToArray();
          }

          Vector3[] GeneratePathTowardRoot(int node, Vector3[] nodePositions) {
              if (node == 0) {
                  return new Vector3[0];
              }
              int parent = NodeParentIndex(node);
              return GeneratePath(nodePositions[node], nodePositions[parent]);
          }

          Vector3[] GeneratePathsTowardRoot(int[] nodes, Vector3[] nodePositions) {
              List<Vector3> pathBuilder = new List<Vector3>();
              foreach (int node in nodes) {
                  pathBuilder.AddRange(GeneratePathTowardRoot(node, nodePositions));
              }
              return pathBuilder.ToArray();
          }

          Vector3[] GeneratePathsToRoot(int node, Vector3[] nodePositions, List<Vector3> pathBuilder) {
              //generate a path from the node to the root
              //add the path to the path builder
              //return the path builder
              if (node == 0) {
                  return pathBuilder.ToArray();
              }
              int parent = NodeParentIndex(node);
              Vector3[] path = GeneratePath(nodePositions[node], nodePositions[parent]);
              pathBuilder.AddRange(path);
              return GeneratePathsToRoot(parent, nodePositions, pathBuilder);
          }

          public Vector3[] crumbleSomeStones(Vector3[] stones, float crumblePercentage ) {
              int crumbleStonesMax = (int)((float)stones.Length * crumblePercentage);
              int[] crumbleStoneIndexes = steppingStones.UniqueRandomNumbersList(0, stones.Length, crumbleStonesMax);

              for (int i = 0; i < crumbleStoneIndexes.Length ; i++) {
                  int stoneIndex = crumbleStoneIndexes[i];
                  stones[stoneIndex].y = -0.1f;
              }
              return stones;
          }

          public Vector3[] GenerateRandomPathsFromTree() {
              Vector3[] nodePositions = GenerateTree();
              int numOfNodes = nodePositions.Length;

              int leafMinIndex = LayerSize(treeHeight - 1) - 1 / (branchingFactor - 1);
              int leafMaxIndex = numOfNodes;

              int[] leafNodesArray = steppingStones.UniqueRandomNumbersList(leafMinIndex, leafMaxIndex, numOfPaths);
              int[] allUniqueAncestors = GetUniqueAncestorsOfNode(leafNodesArray);

              //choose one leaf to be the safe path
              //get its ancestors
              //remove its ancestors from all unique ancestors

              int safeLeaf = leafNodesArray[0];
              int[] safeLeafAncestors = GetAncestorsOfNode(safeLeaf);
              int[] unsafeAncestors = allUniqueAncestors.Except(safeLeafAncestors).ToArray();

              Vector3[] safeStonePosition = GeneratePathsTowardRoot(safeLeafAncestors, nodePositions);
              Vector3[] unsafeStonePositions = GeneratePathsTowardRoot(unsafeAncestors, nodePositions);

              // randomly lower some of the unsage stones
              unsafeStonePositions = crumbleSomeStones(unsafeStonePositions, crumblePercentage);

              List<Vector3> stonePositions = new List<Vector3>();
              stonePositions.AddRange(safeStonePosition);
              stonePositions.AddRange(unsafeStonePositions);

              return stonePositions.ToArray();
        }
    }

    [SerializeField]
    private SteppingStones stones;

    private GameObject[] stoneArray = null;

    private bool stonesOutOfDate = false;
    
    
    private IEnumerator animateStoneSummoning(GameObject stone) {
        // animate the stones being summoned
        float stoneStartHeight = -10.0f;
        float stoneEndHeight = stone.transform.localPosition.y;
        float stoneAnimationTime = 0.5f;

        stone.transform.localPosition = new Vector3(stone.transform.localPosition.x, stoneStartHeight, stone.transform.localPosition.z);
        
        // move the stone up to its final position over time asynchonously
        float startTime = Time.time;

        while (Time.time - startTime < stoneAnimationTime) {
            float t = (Time.time - startTime) / stoneAnimationTime;
            float stoneHeight = Mathf.Lerp(stoneStartHeight, stoneEndHeight, t);
            stone.transform.localPosition = new Vector3(stone.transform.localPosition.x, stoneHeight, stone.transform.localPosition.z);
            yield return null;
        }
        
    }

    public IEnumerator RevealStonesAsync() {
        // reveal in reverse order
        for (int i = stoneArray.Length - 1; i >= 0; i--) {
            stoneArray[i].SetActive(true);
            StartCoroutine(animateStoneSummoning(stoneArray[i]));
            yield return new WaitForSeconds(stones.perStoneRevelationTime);
        }

        // foreach (GameObject stone in stoneArray) {
        //     stone.SetActive(true);
        //     yield return new WaitForSeconds(stones.perStoneRevelationTime);
        // }
    }

    public void RevealStones() {
        StartCoroutine(RevealStonesAsync());
    }

    public void HideStones() {
        foreach (GameObject stone in stoneArray) {
            // make sure stone is not null and is not a missing reference
            if (stone && stone != null) {
                stone.SetActive(false);
            }
        }
    }

    GameObject GenerateStone(Vector3 stonePosition) {
          // make the stones relative to the object this script is on
          // make the stones a child of the object this script is on
          GameObject stone;
          
          // if the stone is lowered then make it a crumbling stone
          if (stonePosition.y < 0) {
              stone = Instantiate(stones.crumblingStonePrefab, new Vector3(0,0,0), Quaternion.identity);
          } else {
              stone = Instantiate(stones.stonePrefab, new Vector3(0,0,0), Quaternion.identity);
          }
          stone.transform.parent = this.transform;
          stone.transform.localPosition = stonePosition;
          stone.transform.localScale = new Vector3(stones.stoneSize, stones.stoneHeight, stones.stoneSize);
          stone.SetActive(false);
          return stone;
    }

    GameObject[] GenerateStones(Vector3[] stonePositions) {
        GameObject[] stones = new GameObject[stonePositions.Length];
        for (int i = 0; i < stonePositions.Length; i++) {
            //set stone position
            stones[i] = GenerateStone(stonePositions[i]);
        }
        return stones;
    }

    GameObject[] GenerateSteppingStones() {
      Vector3[] stonePositions = stones.GenerateRandomPathsFromTree();
      return GenerateStones(stonePositions);
    }

    void Start()
    {
        stoneArray = GenerateSteppingStones();

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
            stoneArray = GenerateSteppingStones();
            stonesOutOfDate = false;
        }
        
    }
}
