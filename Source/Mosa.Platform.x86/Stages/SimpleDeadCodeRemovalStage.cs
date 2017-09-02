// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using Mosa.Compiler.Trace;
using System.Diagnostics;

namespace Mosa.Platform.x86.Stages
{
	/// <summary>
	/// The simple dead code removal stage remove useless instructions
	/// and NOP instructions are moved
	/// and a simple move propagation is performed as well.
	/// </summary>
	/// <seealso cref="Mosa.Platform.x86.BaseTransformationStage" />
	public sealed class SimpleDeadCodeRemovalStage : BaseTransformationStage
	{
		private bool changed;
		private TraceLog trace;
		private int instructionsRemovedCount = 0;

		protected override void PopulateVisitationDictionary()
		{
			// Nothing to do
		}

		protected override void Run()
		{
			bool changed = true;
			trace = CreateTraceLog();

			while (changed)
			{
				changed = false;

				foreach (var block in BasicBlocks)
				{
					for (var node = block.First; !node.IsBlockEndInstruction; node = node.Next)
					{
						if (node.IsEmpty)
							continue;

						// Remove Nop instructions
						if (node.Instruction == X86.Nop)
						{
							node.Empty();
							continue;
						}

						if (node.Instruction == X86.Call)
							continue;

						if (node.Instruction == X86.Mov)
						{
							Move(node);
						}

						if (node.IsEmpty)
							continue;

						RemoveUseless(node);
					}
				}
			}

			UpdateCounter("X86.SimpleDeadCodeRemovalStage.IRInstructionRemoved", instructionsRemovedCount);
		}

		private void RemoveUseless(InstructionNode node)
		{
			// Remove useless instructions
			if (node.ResultCount == 1 && node.Result.Uses.Count == 0 && node.Result.IsVirtualRegister)
			{
				// Check is split child, if so check is parent in use (IR.Return for example)
				if (node.Result.IsSplitChild && node.Result.SplitParent.Uses.Count != 0)
					return;

				if (trace.Active) trace.Log("REMOVED:\t" + node);

				node.Empty();
				changed = true;
				instructionsRemovedCount++;
			}
		}

		/// <summary>
		/// Simple copy propagation.
		/// </summary>
		/// <param name="node">The node.</param>
		private void Move(InstructionNode node)
		{
			if (node.Instruction != X86.Mov)
				return;

			var source = node.Operand1;
			var destination = node.Result;

			if (!destination.IsVirtualRegister)
				return;

			if (!source.IsVirtualRegister)
				return;

			if (destination.Definitions.Count != 1)
				return;

			if (source.Definitions.Count != 1)
				return;

			if (source.IsResolvedConstant)
				return;

			Debug.Assert(destination != source);

			foreach (var useNode in destination.Uses.ToArray())
			{
				for (int i = 0; i < useNode.OperandCount; i++)
				{
					var operand = useNode.GetOperand(i);

					if (destination == operand)
					{
						if (trace.Active) trace.Log("*** SimpleForwardCopyPropagation");
						if (trace.Active) trace.Log("BEFORE:\t" + useNode);
						useNode.SetOperand(i, source);
						changed = true;
						if (trace.Active) trace.Log("AFTER: \t" + useNode);
					}
				}
			}

			Debug.Assert(destination.Uses.Count == 0);

			if (trace.Active) trace.Log("REMOVED:\t" + node);
			node.Empty();
			instructionsRemovedCount++;
			changed = true;
		}
	}
}
