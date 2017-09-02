﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.CompilerStages;
using Mosa.Compiler.MosaTypeSystem;

namespace Mosa.Tool.Explorer
{
	internal class ExplorerCompiler : BaseCompiler
	{
		/// <summary>
		/// Extends the compiler setup.
		/// </summary>
		public override void ExtendCompilerSetup()
		{
			CompilePipeline.Add(new ICompilerStage[] {
				new PlugStage(),

				new TypeInitializerSchedulerStage(),
				new MethodLookupTableStage(),
				new MethodExceptionLookupTableStage(),
				new MetadataStage(),
			});
		}

		/// <summary>
		/// Creates a method compiler
		/// </summary>
		/// <param name="method">The method to compile.</param>
		/// <param name="basicBlocks">The basic blocks.</param>
		/// <param name="threadID">The thread identifier.</param>
		/// <returns>
		/// An instance of a MethodCompilerBase for the given type/method pair.
		/// </returns>
		protected override BaseMethodCompiler CreateMethodCompiler(MosaMethod method, BasicBlocks basicBlocks, int threadID)
		{
			return new ExplorerMethodCompiler(this, method, basicBlocks, threadID);
		}
	}
}
