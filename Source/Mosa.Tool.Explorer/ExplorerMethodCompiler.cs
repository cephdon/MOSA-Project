﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Stages;
using Mosa.Compiler.MosaTypeSystem;

namespace Mosa.Tool.Explorer
{
	internal class ExplorerMethodCompiler : BaseMethodCompiler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExplorerMethodCompiler" /> class.
		/// </summary>
		/// <param name="compiler">The compiler.</param>
		/// <param name="method">The method.</param>
		/// <param name="basicBlocks">The basic blocks.</param>
		/// <param name="threadID">The thread identifier.</param>
		public ExplorerMethodCompiler(ExplorerCompiler compiler, MosaMethod method, BasicBlocks basicBlocks, int threadID)
			: base(compiler, method, basicBlocks, threadID)
		{
			var compilerOptions = Compiler.CompilerOptions;

			// Populate the pipeline
			Pipeline.Add(new IMethodCompilerStage[] {
				new CILDecodingStage(),
				new ExceptionPrologueStage(),
				new OperandAssignmentStage(),
				new StackSetupStage(),

				new CILProtectedRegionStage(),
				new ExceptionStage(),
				new StaticAllocationResolutionStage(),
				new CILTransformationStage(),
				new UnboxValueTypeStage(),

				(compilerOptions.EnableInlinedMethods) ? new InlineStage() : null,
				(compilerOptions.EnableSSA) ? new EdgeSplitStage() : null,
				(compilerOptions.EnableSSA) ? new EnterSSAStage() : null,
				(compilerOptions.EnableSparseConditionalConstantPropagation && compilerOptions.EnableSSA) ? new SparseConditionalConstantPropagationStage() : null,
				(compilerOptions.EnableIROptimizations) ? new IROptimizationStage() : null,
				new LowerIRStage(),

				(compilerOptions.EnableIRLongOperand && compiler.Architecture.NativePointerSize == 4) ? new LongOperandStage() : null,

				(compilerOptions.TwoPassOptimization && compilerOptions.EnableIROptimizations && compilerOptions.EnableSparseConditionalConstantPropagation && compilerOptions.EnableSSA) ? new SparseConditionalConstantPropagationStage() : null,
				(compilerOptions.TwoPassOptimization && compilerOptions.EnableIROptimizations && compilerOptions.EnableSparseConditionalConstantPropagation && compilerOptions.EnableSSA) ? new IROptimizationStage() : null,
				(compilerOptions.EnableSSA) ? new LeaveSSAStage() : null,
				new IRCleanupStage(),
				(compilerOptions.EnableInlinedMethods) ? new InlineEvaluationStage() : null,
				new DevirtualizeCallStage(),
				new CallStage(),
				new PlatformStubStage(),
				new PlatformEdgeSplitStage(),
				new VirtualRegisterRenameStage(),
				new GreedyRegisterAllocatorStage(),
				new StackLayoutStage(),
				new EmptyBlockRemovalStage(),
				new BlockOrderingStage(),
				new CodeGenerationStage(compilerOptions.EmitBinary),
				new PreciseGCStage(),
				new GraphVizStage(),
				(compilerOptions.EmitBinary) ? new ProtectedRegionLayoutStage() : null,
				(compilerOptions.EmitBinary) ? new DisassemblyStage() : null
			});
		}
	}
}
