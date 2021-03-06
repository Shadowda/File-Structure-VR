using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The extended Reingold-Tilford algorithm as described in the paper
 * "Drawing Non-layered Tidy Trees in Linear Time" by Atze van der Ploeg
 * 
 * C# port from Java code by Pierce Jackson
 */
namespace NLT
{
	public class Node
	{
		#region vars region

		// Width and height
		public float w;
		public float h;

		public float x;
		public float y;
		public float prelim;
		public float mod;
		public float shift;
		public float change;

		// Left and right thread
		public NLT.Node tl;
		public NLT.Node tr;

		// Extreme left and right nodes
		public NLT.Node el;
		public NLT.Node er;

		// Sum of modifiers at the extreme 
		public float msel;
		public float mser;

		public NLT.Node Parent;

		// List of children and number of children
		public List<NLT.Node> c;
		public int cs;

		public FileRing Ring;

		public UnityDirectory Directory;
		public string Path;
		#endregion

		public Node(UnityDirectory directory, List<NLT.Node> children)
		{
			this.w = directory.width;
			this.h = directory.height;
			this.y = directory.y;

			this.c = children;
			this.cs = children.Count;

			this.Directory = directory;
			this.Path = directory.Path;

			this.Ring = new FileRing(ref directory, this);
		}

		// --------------------------------------------------------------------------------------------------------------------
		// Built the layout
		public void layout() 
		{
			firstWalk(this);
			secondWalk(this, 0);
		}
		// --------------------------------------------------------------------------------------------------------------------

		public Vector3 GetCenter() 
		{
			return new Vector3((x * 2 + w - 1) * 0.5f, 0, (y * 2 + h) * 0.5f);
		}

		public void PlaceFileRing() 
		{
			Ring.Place(GetCenter());
		}

		public void firstWalk(NLT.Node t) 
		{
			if (t.cs == 0) 
			{
				setExtremes(t);
				return;
			}
			firstWalk(t.c[0]);

			// Create siblings in contour minimal vertical coordinate and index list.
			IYL ih = updateIYL(bottom(t.c[0].el), 0, null);
			for (var i = 1; i < t.cs; i++) 
			{
				firstWalk(t.c[i]);
				//Store lowest vertical coordinate while extreme nodes still point in current subtree.
				var minY = bottom(t.c[i].er);
				//Debug.Log(ih);
				seperate(t, i, ih);
				ih = updateIYL(minY, i, ih);
			}
			positionRoot(t);
			setExtremes(t);
		}

		public void setExtremes(NLT.Node t) 
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

		public void seperate(NLT.Node t, int i, IYL ih) 
		{
			// Right contour node of left siblings and its sum of modfiers.  
			var sr = t.c[i - 1];
			var mssr = sr.mod;
			// Left contour node of current subtree and its sum of modfiers.  
			var cl = t.c[i];
			var mscl = cl.mod;
			while (sr != null && cl != null) 
			{
				if (bottom(sr) > ih.lowY) 
				{
					ih = ih.nxt;
				}
				// How far to the left of the right side of sr is the left side of cl?  
				var dist = (mssr + sr.prelim + sr.w) - (mscl + cl.prelim);
				if (dist > 0) 
				{
					mscl += dist;
					moveSubtree(t, i, ih.index, dist);
				}
				var sy = bottom(sr);
				var cy = bottom(cl);
				// Advance highest node(s) and sum(s) of modifiers  
				if (sy <= cy) 
				{
					sr = nextRightContour(sr);
					if (sr != null) 
					{
						mssr += sr.mod;
					}
				}
				if (sy >= cy) 
				{
					cl = nextLeftContour(cl);
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
				setLeftThread(t, i, cl, mscl);
			}
			else if (sr != null && cl == null) 
			{
				setRightThread(t, i, sr, mssr);
			}
		}

		#region region tree magic 
		public void moveSubtree(NLT.Node t, int i, int si, float dist) 
		{
			// Move subtree by changing mod.  
			t.c[i].mod += dist;
			t.c[i].msel += dist;
			t.c[i].mser += dist;
			distributeExtra(t, i, si, dist);
		}

		public NLT.Node nextLeftContour(NLT.Node t) 
		{
			return t.cs == 0 ? t.tl : t.c[0];
		}

		public NLT.Node nextRightContour(NLT.Node t) 
		{
			return t.cs == 0 ? t.tr : t.c[t.cs - 1];
		}

		public float bottom(NLT.Node t) 
		{
			return t.y + t.h;
		}

		public void setLeftThread(NLT.Node t, int i, NLT.Node cl, float modsumcl) 
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
		public void setRightThread(NLT.Node t, int i, NLT.Node sr, float modsumsr) 
		{
			var ri = t.c[i].er;
			ri.tr = sr;
			var diff = (modsumsr - sr.mod) - t.c[i].mser;
			ri.mod += diff;
			ri.prelim -= diff;
			t.c[i].er = t.c[i - 1].er;
			t.c[i].mser = t.c[i - 1].mser;
		}

		public void positionRoot(NLT.Node t) 
		{
			// Position root between children, taking into account their mod.  
			t.prelim = (t.c[0].prelim + t.c[0].mod + t.c[t.cs - 1].mod + t.c[t.cs - 1].prelim + t.c[t.cs - 1].w) / 2 - t.w / 2;
		}

		public void secondWalk(NLT.Node t, float modsum) 
		{
			modsum += t.mod;
			// Set absolute (non-relative) horizontal coordinate.  
			t.x = t.prelim + modsum;
			addChildSpacing(t);
			for (var i = 0; i < t.cs; i++) {
				secondWalk(t.c[i], modsum);
			}
		}

		public void distributeExtra(NLT.Node t, int i, int si, float dist) 
		{
			// Are there intermediate children?
			if (si != i - 1) {
				var nr = i - si;
				t.c[si + 1].shift += dist / nr;
				t.c[i].shift -= dist / nr;
				t.c[i].change -= dist - dist / nr;
			}
		}

		// Process change and shift to add intermediate spacing to mod.  
		public void addChildSpacing(NLT.Node t) 
		{
			float d = 0;
			float modsumdelta = 0;
			for (var i = 0; i < t.cs; i++) {
				d += t.c[i].shift;
				modsumdelta += d + t.c[i].change;
				t.c[i].mod += modsumdelta;
			}
		}
		#endregion

		// A linked list of the indexes of left siblings and their lowest vertical coordinate.  
		public class IYL
		{
			public float lowY;
			public int index;
			public IYL nxt;
			public IYL(float lowY, int index, IYL nxt) 
			{
				this.lowY = lowY;
				this.index = index;
				this.nxt = nxt;
			}
		}

		public IYL updateIYL(float minY, int i, IYL ih) 
		{
			// Remove siblings that are hidden by the new subtree.  
			while (ih != null && minY >= ih.lowY) 
			{
				ih = ih.nxt;
			}
			// Prepend the new subtree.  
			return new IYL(minY, i, ih);
		}

		public void LogPrint(NLT.Node node) 
		{
			foreach (var child in node.c) 
			{
				LogPrint(child);
			}

			Debug.Log(node.x);
			Debug.Log(node.y);
		}
	}
}