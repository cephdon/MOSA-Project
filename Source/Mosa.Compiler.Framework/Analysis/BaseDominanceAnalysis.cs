﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections.Generic;

namespace Mosa.Compiler.Framework.Analysis
{
	/// <summary>
	/// A dominance provider allows other compilation stages to retrieve dominance relationships.
	/// </summary>
	public abstract class BaseDominanceAnalysis
	{
		/// <summary>
		/// Calculates dominance for the specified basic blocks.
		/// </summary>
		/// <param name="basicBlocks">The basic blocks.</param>
		/// <param name="entryBlock">The entry block.</param>
		public abstract void PerformAnalysis(BasicBlocks basicBlocks, BasicBlock entryBlock);

		/// <summary>
		/// Retrieves the immediate dominator of a block.
		/// </summary>
		/// <param name="block">The block to retrieve the immediate dominator for.</param>
		/// <returns>The immediate dominator of the block.</returns>
		public abstract BasicBlock GetImmediateDominator(BasicBlock block);

		/// <summary>
		/// Retrieves the dominators of the given block.
		/// </summary>
		/// <param name="block">The block to retrieve dominators for.</param>
		/// <returns>An array of dominators of a block. The first element (at index zero) is the block itself.</returns>
		public abstract List<BasicBlock> GetDominators(BasicBlock block);

		/// <summary>
		/// Retrieves the blocks which are in the dominance frontier of any other block.
		/// </summary>
		/// <returns>All blocks which are in a dominance frontier of another block.</returns>
		public abstract List<BasicBlock> GetDominanceFrontier();

		/// <summary>
		/// Retrieves the dominance frontier of the given block.
		/// </summary>
		/// <param name="block">The block to return the dominance frontier of.</param>
		/// <returns>An array of Blocks, which represent the dominance frontier.</returns>
		public abstract List<BasicBlock> GetDominanceFrontier(BasicBlock block);

		/// <summary>
		/// Gets the children.
		/// </summary>
		/// <param name="block">The block.</param>
		/// <returns></returns>
		public abstract List<BasicBlock> GetChildren(BasicBlock block);

		/// <summary>
		/// Retrieves blocks of the iterated dominance frontier.
		/// </summary>
		/// <param name="blocks">The blocks.</param>
		/// <returns></returns>
		public abstract List<BasicBlock> IteratedDominanceFrontier(List<BasicBlock> blocks);
	}
}
