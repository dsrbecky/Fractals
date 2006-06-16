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
	}
}
