using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The extended Reingold-Tilford algorithm as described in the paper
 * "Drawing Non-layered Tidy Trees in Linear Time" by Atze van der Ploeg
 * 
 * C# port from Java code by Pierce Jackson
 * 
 */

public class NLT_Tree
{

    public static class Tree
    {
        #region vars region
        // Width and height
        public double w;
        public double h;

        public double x;
        public double y;
        public double prelim;
        public double mod;
        public double shift;
        public double change;

        //Left and right thread
        public Tree tl;
        public Tree tr;

        // Extreme left and right nodes
        public Tree el;
        public Tree er;

        // Sum of modifiers at the extreme 
        public double msel;
        public double mser;

        //List of children and number of children
        public List<Tree> c;
        public int cs;

        #endregion

        public Tree(double w, double h, double y, List<Tree> c)
        {
            this.w = w;
            this.h = h;
            this.y = y;
            this.c = c;
            this.cs = c.Count;
        }

		public Tree convert(UnityDirectory root)
		{
			if (root == null) return null;

			List<Tree> children = new List<Tree>;

			foreach (var child in root.GraphedChildren)
			{
				children.Add(convert(child));
			}

			return new Tree(root.width, root.height, root.y, children);
		}


		// --------------------------------------------------------------------------------------------------------------------
		// Built the layout
		public static void layout(Tree t)
        {
            NLT_Tree.firstWalk(t);
            NLT_Tree.secondWalk(t, 0);
        }
		// --------------------------------------------------------------------------------------------------------------------

		public static void firstWalk(Tree t)
		{
			if (t.cs == 0)
			{
				NLT_Tree.setExtremes(t);
				return;
			}
			NLT_Tree.firstWalk(t.c[0]);

			// Create siblings in contour minimal vertical coordinate and index list.
			var ih = NLT_Tree.updateIYL(NLT_Tree.bottom(t.c[0].el), 0, null);
			for (var i = 1; i < t.cs; i++)
			{
				NLT_Tree.firstWalk(t.c[i]);
				//Store lowest vertical coordinate while extreme nodes still point in current subtree.
				var minY = NLT_Tree.bottom(t.c[i].er);
				NLT_Tree.seperate(t, i, ih);
				ih = NLT_Tree.updateIYL(minY, i, ih);
			}
			NLT_Tree.positionRoot(t);
			NLT_Tree.setExtremes(t);
		}
		public static void setExtremes(Tree t)
		{
			if (t.cs == 0)
			{
				t.el = t;
				t.er = t;
				t.msel = t.mser = 0;
			}
			else
			{
				t.el = t.c[0].el;
				t.msel = t.c[0].msel;
				t.er = t.c[t.cs - 1].er;
				t.mser = t.c[t.cs - 1].mser;
			}
		}
		public static void seperate(Tree t, int i, IYL ih)
		{
			// Right contour node of left siblings and its sum of modfiers.  
			var sr = t.c[i - 1];
			var mssr = sr.mod;
			// Left contour node of current subtree and its sum of modfiers.  
			var cl = t.c[i];
			var mscl = cl.mod;
			while (sr != null && cl != null)
			{
				if (NLT_Tree.bottom(sr) > ih.lowY)
				{
					ih = ih.nxt;
				}
				// How far to the left of the right side of sr is the left side of cl?  
				var dist = (mssr + sr.prelim + sr.w) - (mscl + cl.prelim);
				if (dist > 0)
				{
					mscl += dist;
					NLT_Tree.moveSubtree(t, i, ih.index, dist);
				}
				var sy = NLT_Tree.bottom(sr);
				var cy = NLT_Tree.bottom(cl);
				// Advance highest node(s) and sum(s) of modifiers  
				if (sy <= cy)
				{
					sr = NLT_Tree.nextRightContour(sr);
					if (sr != null)
					{
						mssr += sr.mod;
					}
				}
				if (sy >= cy)
				{
					cl = NLT_Tree.nextLeftContour(cl);
					if (cl != null)
					{
						mscl += cl.mod;
					}
				}
			}

			// Set threads and update extreme nodes.  
			// In the first case, the current subtree must be taller than the left siblings.  
			if (sr == null && cl != null)
			{
				NLT_Tree.setLeftThread(t, i, cl, mscl);
			}
			else if (sr != null && cl == null)
			{
				NLT_Tree.setRightThread(t, i, sr, mssr);
			}
		}
		public static void moveSubtree(Tree t, int i, int si, double dist)
		{
			// Move subtree by changing mod.  
			t.c[i].mod += dist;
			t.c[i].msel += dist;
			t.c[i].mser += dist;
			NLT_Tree.distributeExtra(t, i, si, dist);
		}
		public static Tree nextLeftContour(Tree t)
		{
			return t.cs == 0 ? t.tl : t.c[0];
		}
		public static Tree nextRightContour(Tree t)
		{
			return t.cs == 0 ? t.tr : t.c[t.cs - 1];
		}
		public static double bottom(Tree t)
		{
			return t.y + t.h;
		}
		public static void setLeftThread(Tree t, int i, Tree cl, double modsumcl)
		{
			var li = t.c[0].el;
			li.tl = cl;
			// Change mod so that the sum of modifier after following thread is correct.  
			var diff = (modsumcl - cl.mod) - t.c[0].msel;
			li.mod += diff;
			// Change preliminary x coordinate so that the node does not move.  
			li.prelim -= diff;
			// Update extreme node and its sum of modifiers.  
			t.c[0].el = t.c[i].el;
			t.c[0].msel = t.c[i].msel;
		}
		// Symmetrical to setLeftThread.  
		public static void setRightThread(Tree t, int i, Tree sr, double modsumsr)
		{
			var ri = t.c[i].er;
			ri.tr = sr;
			var diff = (modsumsr - sr.mod) - t.c[i].mser;
			ri.mod += diff;
			ri.prelim -= diff;
			t.c[i].er = t.c[i - 1].er;
			t.c[i].mser = t.c[i - 1].mser;
		}
		public static void positionRoot(Tree t)
		{
			// Position root between children, taking into account their mod.  
			t.prelim = (t.c[0].prelim + t.c[0].mod + t.c[t.cs - 1].mod + t.c[t.cs - 1].prelim + t.c[t.cs - 1].w) / 2 - t.w / 2;
		}
		public static void secondWalk(Tree t, double modsum)
		{
			modsum += t.mod;
			// Set absolute (non-relative) horizontal coordinate.  
			t.x = t.prelim + modsum;
			NLT_Tree.addChildSpacing(t);
			for (var i = 0; i < t.cs; i++)
			{
				NLT_Tree.secondWalk(t.c[i], modsum);
			}
		}
		public static void distributeExtra(Tree t, int i, int si, double dist)
		{
			// Are there intermediate children?
			if (si != i - 1)
			{
				var nr = i - si;
				t.c[si + 1].shift += dist / nr;
				t.c[i].shift -= dist / nr;
				t.c[i].change -= dist - dist / nr;
			}
		}
		// Process change and shift to add intermediate spacing to mod.  
		public static void addChildSpacing(Tree t)
		{
			var d = 0;
			var modsumdelta = 0;
			for (var i = 0; i < t.cs; i++)
			{
				d += t.c[i].shift;
				modsumdelta += d + t.c[i].change;
				t.c[i].mod += modsumdelta;
			}
		}
		// A linked list of the indexes of left siblings and their lowest vertical coordinate.  
		static class IYL
		{
			public double lowY;
			public int index;
			public IYL nxt;
			public IYL(double lowY, int index, IYL nxt)
			{
				this.lowY = lowY;
				this.index = index;
				this.nxt = nxt;
			}
		}
		public static IYL updateIYL(double minY, int i, IYL ih)
		{
			// Remove siblings that are hidden by the new subtree.  
			while (ih != null && minY >= ih.lowY)
			{
				ih = ih.nxt;
			}
			// Prepend the new subtree.  
			return new IYL(minY, i, ih);
		}
	}
}