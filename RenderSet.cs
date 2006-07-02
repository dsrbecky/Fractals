using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Fractals
{
	public class RenderSet
	{
		SortedDictionary<RenderOperation, object> renderSet = new SortedDictionary<RenderOperation, object>();
		
		public IEnumerable<RenderOperation> RenderOperations {
			get {
				return renderSet.Keys;
			}
		}
		
		public int Count {
			get {
				return renderSet.Count;
			}
		}
		
		public RenderSet(RenderOperation root, int numberOfFragmentsToRender)
		{
			renderSet.Add(root, null);
			while (renderSet.Count < numberOfFragmentsToRender) {
				foreach (KeyValuePair<RenderOperation, object> kvp in renderSet) {
					RenderOperation first = kvp.Key;
					first.Fragment.MakeChilds();
					renderSet.Remove(first);
					AddOperation(first.LeftTopQuater);
					AddOperation(first.RightTopQuater);
					AddOperation(first.LeftBottomQuater);
					AddOperation(first.RightBottomQuater);
					break;
				}
			}
		}
		
		void AddOperation(RenderOperation op)
		{
			if (op.IsInViewFrustum) {
				renderSet.Add(op, null);
			}
		}
		
		public IEnumerable<RenderOperation> MakeRefineOperations(double pixelSize)
		{
			while(renderSet.Count > 0) {
				foreach (KeyValuePair<RenderOperation, object> kvp in renderSet) {
					RenderOperation first = kvp.Key;
					renderSet.Remove(first);
					
					// Terminate refinement?
					double AAfactor = 1d + Math.Min(first.Fragment.MaxColorDifference / 16d, 3d);
					if (first.TexelSize / 2 * AAfactor < pixelSize) break;
					
					first.Fragment.MakeChilds();
					if (first.LeftTopQuater.IsInViewFrustum) {
						yield return first.LeftTopQuater;
						renderSet.Add(first.LeftTopQuater, null);
					}
					if (first.RightTopQuater.IsInViewFrustum) {
						yield return first.RightTopQuater;
						renderSet.Add(first.RightTopQuater, null);
					}
					if (first.LeftBottomQuater.IsInViewFrustum) {
						yield return first.LeftBottomQuater;
						renderSet.Add(first.LeftBottomQuater, null);
					}
					if (first.RightBottomQuater.IsInViewFrustum) {
						yield return first.RightBottomQuater;
						renderSet.Add(first.RightBottomQuater, null);
					}
					break;
				}
			}
		}
	}
}
