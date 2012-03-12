/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 */


using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Operands;
using Mosa.Compiler.Linker;
using Mosa.Compiler.TypeSystem;
using IR = Mosa.Compiler.Framework.IR;

namespace Mosa.Tool.Compiler.Stages
{
	/// <summary>
	/// Schedules type initializers and creates a hidden mosacl_main method,
	/// which runs all type initializers in sequence.
	/// </summary>
	/// <remarks>
	/// Dependencies are not resolved, it is hoped that dependencies are resolved
	/// by the high-level language compiler by placing cctors in some order in
	/// metadata.
	/// </remarks>
	public sealed class TypeInitializerSchedulerStage : BaseAssemblyCompilerStage, IAssemblyCompilerStage, IPipelineStage, ITypeInitializerSchedulerStage
	{
		#region Data Members

		private InstructionSet instructionSet;

		/// <summary>
		/// Hold the current context
		/// </summary>
		private Context ctx;

		/// <summary>
		/// Holds the method for the type initalizer
		/// </summary>
		private LinkerGeneratedMethod method;

		#endregion // Data Members

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeInitializerSchedulerStage"/> class.
		/// </summary>
		public TypeInitializerSchedulerStage()
		{
			instructionSet = new InstructionSet(1024);
			ctx = new Context(instructionSet);

			ctx.AppendInstruction(IR.Instruction.PrologueInstruction);
			//ctx.Other = 0; // stacksize
		}

		#endregion // Construction

		#region Properties

		/// <summary>
		/// Gets the intializer method.
		/// </summary>
		/// <value>The method.</value>
		public LinkerGeneratedMethod TypeInitializerMethod
		{
			get { return method; }
		}

		#endregion

		#region IAssemblyCompilerStage Members

		void IAssemblyCompilerStage.Setup(AssemblyCompiler compiler)
		{
			base.Setup(compiler);
		}

		/// <summary>
		/// Performs stage specific processing on the compiler context.
		/// </summary>
		void IAssemblyCompilerStage.Run()
		{
			ITypeModule mainTypeModule = typeSystem.MainTypeModule;

			if (mainTypeModule.MetadataModule.EntryPoint.RID != 0)
			{
				RuntimeMethod entrypoint = mainTypeModule.GetMethod(mainTypeModule.MetadataModule.EntryPoint);

				Schedule(entrypoint);
			}

			ctx.AppendInstruction(IR.Instruction.EpilogueInstruction);
			//ctx.Other = 0;

			method = LinkTimeCodeGenerator.Compile(compiler, @"AssemblyInit", instructionSet, typeSystem);
		}

		#endregion // IAssemblyCompilerStage Members

		#region Methods

		/// <summary>
		/// Schedules the specified method for invocation in the main.
		/// </summary>
		/// <param name="method">The method.</param>
		public void Schedule(RuntimeMethod method)
		{
			SymbolOperand symbolOperand = SymbolOperand.FromMethod(method);
			ctx.AppendInstruction(IR.Instruction.CallInstruction, null, symbolOperand);
		}

		#endregion // Methods

	}
}
