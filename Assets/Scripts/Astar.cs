using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> openList = new();
        List<Node> closedList = new();

        // Create a starting node
        Node startingNode = new()
        {
            position = startPos,
            GScore = 0,
            HScore = Vector2.Distance(startPos, endPos)
        }; 

        openList.Add(startingNode);
        
        while(openList.Count > 0)
        {
            // Fetch the lowest F score Node
            Node currentNode = FindLowestFScoreNode(openList);

            if(currentNode.position == endPos)
                return ReconstructPath(currentNode);
            
            // Update both lists
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Fetch neighbour cells
            List<Cell> neighbourCells = grid[currentNode.position.x, currentNode.position.y].GetNeighbours(grid);
            foreach (Cell cell in neighbourCells)
            {
                // Calculate the distance between the neighbor cell and the current cell
                Vector2Int direction = cell.gridPosition - currentNode.position;
                
                Wall currentWall, neighborWall;

                // Map the directions
                if(direction == Vector2Int.up) 
                {
                    currentWall = Wall.UP;
                    neighborWall = Wall.DOWN;
                }
                else if(direction == Vector2Int.right) 
                {
                    currentWall = Wall.RIGHT;
                    neighborWall = Wall.LEFT;
                }
                else if(direction == Vector2Int.down) 
                {
                    currentWall = Wall.DOWN;
                    neighborWall = Wall.UP;
                }
                else if(direction == Vector2Int.left) 
                {
                    currentWall = Wall.LEFT;
                    neighborWall = Wall.RIGHT;
                }
                else continue;

                if(grid[currentNode.position.x, currentNode.position.y].HasWall(currentWall) || cell.HasWall(neighborWall)) 
                    continue;
                
                // Store the position of the cell
                Vector2Int neighbourCell = cell.gridPosition;

                // Check if the node exists in the closed list
                if(IsNodeInList(closedList, neighbourCell))
                    continue; 
                
                // Calculate G score
                float newGscore = currentNode.GScore + Vector2.Distance(currentNode.position, neighbourCell);

                // Fetch the neighbour node
                Node neighbourNode = FetchNodeFromList(openList, neighbourCell);

                if(neighbourNode == null) 
                {
                    neighbourNode = new()
                    {
                        position = neighbourCell,
                        parent = currentNode,
                        GScore = newGscore,
                        HScore = Vector2.Distance(neighbourCell, endPos)
                    };

                    openList.Add(neighbourNode);
                }
                else if(newGscore < neighbourNode.GScore)
                {
                    neighbourNode.parent = currentNode;
                    neighbourNode.GScore = newGscore;
                }
            }
        }
        
        return null;
    }

    private bool IsNodeInList(List<Node> nodeList, Vector2Int position) 
    {
        foreach (Node node in nodeList)
        {
            if(node.position == position)
                return true;
        }

        return false;
    }

    private Node FetchNodeFromList(List<Node> nodeList, Vector2Int position) 
    {
        foreach (Node node in nodeList)
        {
            if(node.position == position) 
            {
                return node;
            }
        }

        return null;
    }

    private Node FindLowestFScoreNode(List<Node> nodeList) 
    {
        Node lowestFNode = null;
        float lowestFscore = float.MaxValue;
        
        foreach (Node node in nodeList)
        {   
            // Compare the F score with the max value
            if(node.FScore < lowestFscore) 
            {
                // Update the value with the F score of the node
                lowestFscore = node.FScore;
                lowestFNode = node;
            }
        }

        return lowestFNode;
    }

    private List<Vector2Int> ReconstructPath(Node endNode) 
    {
        List<Vector2Int> path = new();
        Node currentNode = endNode;

        while(currentNode != null) 
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        
        path.Reverse();
        
        return path;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
