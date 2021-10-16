/*


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawTreeTest
{

    public static class TreeHelpers
    {
        private static int nodeSize = 4;
        private static float siblingDistance = 2F;
        private static float treeDistance = 1F;

        //main
        public static void CalculateNodePositions(UnityDirectory rootNode)
        {
            List<int> yList = new List<int>();

            // initialize node x, y, and mod values
            InitializeNodes(rootNode, 0, yList);

            // assign initial X and Mod values for nodes
            CalculateInitialX(rootNode, yList);

            // ensure no node is being drawn off screen
           // CheckAllChildrenOnScreen(rootNode);

            // assign final X values to nodes
            CalculateFinalPositions(rootNode, 0);
        }

        // recusrively initialize x, y, and mod values of nodes
        private static void InitializeNodes(UnityDirectory node, int depth, List<int> yList)
        {
            node.X = -1;
            node.Y = depth;
            yList.Insert(depth, 0);
            node.Mod = 0;
            node.Z = 0;

            foreach (UnityDirectory child in node.GraphedChildren)
                InitializeNodes(child, depth + 1, yList);

            if (node.Parent != null)
            {
                foreach (UnityDirectory child in node.Parent.GraphedChildren)
                {
                    if (child.Size > yList[depth])
                    {
                        yList[depth] = child.Size;
                    }
                }
            }
        }

        private static void CalculateInitialX(UnityDirectory node, List<int> yList)
        {

            node.Z = yList[node.Y];

            foreach (UnityDirectory child in node.GraphedChildren)
                if (child.EntryType == UnityDirectory.Type.Directory)
                {
                    CalculateInitialX(child, yList);
                }
            // Postorder


            // if no children
            if (node.IsLeaf())
            {
                // if there is a previous sibling in this set, set X to prevous sibling + designated distance
                if (!node.IsLeftMost())
                    node.X = node.GetPreviousSibling().X + nodeSize + siblingDistance;
                else
                    // if this is the first node in a set, set X to 0
                    node.X = 0;
            }

            // if there is only one child
            else if (node.GraphedChildren.Count == 1)
            {
                // if this is the first node in a set, set it's X value equal to it's child's X value
                if (node.IsLeftMost())
                {
                    node.X = node.GraphedChildren[0].X;
                }
                else
                {
                    node.Log<string>(node.Path);
                    node.X = node.GetPreviousSibling().X + nodeSize + siblingDistance;
                    node.Mod = node.X - node.GraphedChildren[0].X;
                } 
            }

            // many children.
            else
            {
                var leftChild = node.GetLeftMostChild();
                var rightChild = node.GetRightMostChild();
                var mid = (leftChild.X + rightChild.X) / 2;
 
                if (node.IsLeftMost())
                {
                    node.X = mid;
                }
                else
                {
                    node.X = node.GetPreviousSibling().X + nodeSize + siblingDistance;
                    node.Mod = node.X - mid;
                }
            }

            if (node.GraphedChildren.Count > 0 && !node.IsLeftMost())
            {
                // Since subtrees can overlap, check for conflicts and shift tree right if needed
                //CheckForConflicts(node);
            }
 
        }

        private static void CalculateFinalPositions(UnityDirectory node, float modSum)
        {

            node.X += modSum;
            modSum += node.Mod;

            foreach (UnityDirectory child in node.GraphedChildren)
                if (child.EntryType == UnityDirectory.Type.Directory)
                {
                    CalculateFinalPositions(child, modSum);
                }
        }

        private static void CheckForConflicts(UnityDirectory node)
        {
            var minDistance = treeDistance + nodeSize;
            var shiftValue = 0F;
 
            var nodeContour = new Dictionary<int, float>();
            GetLeftContour(node, 0, ref nodeContour);
 
            var sibling = node.GetLeftMostSibling();
            while (sibling != null && sibling != node)
            {
                var siblingContour = new Dictionary<int, float>();
                GetRightContour(sibling, 0, ref siblingContour);
 
                for (int level = node.Y + 1; level <= Math.Min(siblingContour.Keys.Max(), nodeContour.Keys.Max()); level++)
                {
                    var distance = nodeContour[level] - siblingContour[level];
                    if (distance + shiftValue < minDistance)
                    {
                        shiftValue = minDistance - distance;
                    }
                }
 
                if (shiftValue > 0)
                {
                    node.X += shiftValue;
                    node.Mod += shiftValue;

                    CenterNodesBetween(node, sibling);

                    shiftValue = 0;
                }
 
                sibling = sibling.GetNextSibling();
            }
        }

        private static void CenterNodesBetween(UnityDirectory leftNode, UnityDirectory rightNode)
        {
            var leftIndex = leftNode.Parent.GraphedChildren.IndexOf(rightNode);
            var rightIndex = leftNode.Parent.GraphedChildren.IndexOf(leftNode);
                    
            var numNodesBetween = (rightIndex - leftIndex) - 1;

            if (numNodesBetween > 0)
            {
                var distanceBetweenNodes = (leftNode.X - rightNode.X) / (numNodesBetween + 1);

                int count = 1;
                for (int i = leftIndex + 1; i < rightIndex; i++)
                {
                    var middleNode = leftNode.Parent.GraphedChildren[i];

                    var desiredX = rightNode.X + (distanceBetweenNodes * count);
                    var offset = desiredX - middleNode.X;
                    middleNode.X += offset;
                    middleNode.Mod += offset;

                    count++;
                }

                CheckForConflicts(leftNode);
            }
        }
 
        private static void CheckAllChildrenOnScreen(UnityDirectory node)
        {
            var nodeContour = new Dictionary<int, float>();
            GetLeftContour(node, 0, ref nodeContour);

            float shiftAmount = 0;
            foreach (var y in nodeContour.Keys)
            {
                if (nodeContour[y] + shiftAmount < 0)
                    shiftAmount = (nodeContour[y] * -1);
            }

            if (shiftAmount > 0)
            {
                node.X += shiftAmount;
                node.Mod += shiftAmount;
            }
        }
 
        private static void GetLeftContour(UnityDirectory node, float modSum, ref Dictionary<int, float> values)
        {
            if (!values.ContainsKey(node.Y))
                values.Add(node.Y, node.X + modSum);
            else
                values[node.Y] = Math.Min(values[node.Y], node.X + modSum);
 
            modSum += node.Mod;
            foreach (var child in node.GraphedChildren)
            {
                GetLeftContour(child, modSum, ref values);
            }
        }
 
        private static void GetRightContour(UnityDirectory node, float modSum, ref Dictionary<int, float> values)
        {
            if (!values.ContainsKey(node.Y))
                values.Add(node.Y, node.X + modSum);
            else
                values[node.Y] = Math.Max(values[node.Y], node.X + modSum);
 
            modSum += node.Mod;
            if (node.GraphedChildren.Count == 0)
            {
                return;
            }
            foreach (var child in node.GraphedChildren)
            {
                GetRightContour(child, modSum, ref values);
            }
        }

    }
}
*/