// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.IR;
using Mosa.Compiler.Framework.Stages;
using Mosa.Compiler.Linker.Elf;
using Mosa.Platform.x86.CompilerStages;
using Mosa.Platform.x86.Stages;
using System.Diagnostics;

namespace Mosa.Platform.x86
{
	/// <summary>
	/// This class provides a common base class for architecture
	/// specific operations.
	/// </summary>
	public class Architecture : BaseArchitecture
	{
		/// <summary>
		/// Gets the endianness of the target architecture.
		/// </summary>
		/// <value>
		/// The endianness.
		/// </value>
		public override Endianness Endianness { get { return Endianness.Little; } }

		/// <summary>
		/// Gets the type of the elf machine.
		/// </summary>
		/// <value>
		/// The type of the elf machine.
		/// </value>
		public override MachineType MachineType { get { return MachineType.Intel386; } }

		/// <summary>
		/// Defines the register set of the target architecture.
		/// </summary>
		private static readonly Register[] registers = new Register[]
		{
			////////////////////////////////////////////////////////
			// 32-bit general purpose registers
			////////////////////////////////////////////////////////
			GeneralPurposeRegister.EAX,
			GeneralPurposeRegister.ECX,
			GeneralPurposeRegister.EDX,
			GeneralPurposeRegister.EBX,
			GeneralPurposeRegister.ESP,
			GeneralPurposeRegister.EBP,
			GeneralPurposeRegister.ESI,
			GeneralPurposeRegister.EDI,

			////////////////////////////////////////////////////////
			// SSE 128-bit floating point registers
			////////////////////////////////////////////////////////
			SSE2Register.XMM0,
			SSE2Register.XMM1,
			SSE2Register.XMM2,
			SSE2Register.XMM3,
			SSE2Register.XMM4,
			SSE2Register.XMM5,
			SSE2Register.XMM6,
			SSE2Register.XMM7,

			////////////////////////////////////////////////////////
			// Segmentation Registers
			////////////////////////////////////////////////////////
			//SegmentRegister.CS,
			//SegmentRegister.DS,
			//SegmentRegister.ES,
			//SegmentRegister.FS,
			//SegmentRegister.GS,
			//SegmentRegister.SS
		};

		/// <summary>
		/// Specifies the architecture features to use in generated code.
		/// </summary>
		private readonly ArchitectureFeatureFlags architectureFeatures;

		/// <summary>
		/// Initializes a new instance of the <see cref="Architecture"/> class.
		/// </summary>
		/// <param name="architectureFeatures">The features this architecture supports.</param>
		private Architecture(ArchitectureFeatureFlags architectureFeatures)
		{
			this.architectureFeatures = architectureFeatures;
		}

		/// <summary>
		/// Retrieves the native integer size of the x86 platform.
		/// </summary>
		/// <value>This property always returns 32.</value>
		public override int NativeIntegerSize
		{
			get { return 32; }
		}

		/// <summary>
		/// Gets the native alignment of the architecture in bytes.
		/// </summary>
		/// <value>This property always returns 4.</value>
		public override int NativeAlignment
		{
			get { return 4; }
		}

		/// <summary>
		/// Gets the native size of architecture in bytes.
		/// </summary>
		/// <value>This property always returns 4.</value>
		public override int NativePointerSize
		{
			get { return 4; }
		}

		/// <summary>
		/// Retrieves the register set of the x86 platform.
		/// </summary>
		public override Register[] RegisterSet
		{
			get { return registers; }
		}

		/// <summary>
		/// Retrieves the stack frame register of the x86.
		/// </summary>
		public override Register StackFrameRegister
		{
			get { return GeneralPurposeRegister.EBP; }
		}

		/// <summary>
		/// Retrieves the stack pointer register of the x86.
		/// </summary>
		public override Register StackPointerRegister
		{
			get { return GeneralPurposeRegister.ESP; }
		}

		/// <summary>
		/// Retrieves the scratch register of the x86.
		/// </summary>
		public override Register ScratchRegister
		{
			get { return GeneralPurposeRegister.EDX; }
		}

		/// <summary>
		/// Gets the return32 bit register.
		/// </summary>
		public override Register Return32BitRegister
		{
			get { return GeneralPurposeRegister.EAX; }
		}

		/// <summary>
		/// Gets the return64 bit register.
		/// </summary>
		public override Register Return64BitRegister
		{
			get { return GeneralPurposeRegister.EDX; }
		}

		/// <summary>
		/// Gets the return floating point register.
		/// </summary>
		public override Register ReturnFloatingPointRegister
		{
			get { return SSE2Register.XMM0; }
		}

		/// <summary>
		/// Retrieves the exception register of the architecture.
		/// </summary>
		public override Register ExceptionRegister
		{
			get { return GeneralPurposeRegister.EDI; }
		}

		/// <summary>
		/// Gets the finally return block register.
		/// </summary>
		public override Register LeaveTargetRegister
		{
			get { return GeneralPurposeRegister.ESI; }
		}

		/// <summary>
		/// Retrieves the program counter register of the x86.
		/// </summary>
		public override Register ProgramCounter
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the offset of first local.
		/// </summary>
		/// <value>
		/// The offset of first local.
		/// </value>
		public override int OffsetOfFirstLocal { get { return 0; } }

		/// <summary>
		/// Gets the offset of first parameter.
		/// </summary>
		/// <value>
		/// The offset of first parameter.
		/// </value>
		public override int OffsetOfFirstParameter { get { return 8; } }

		/// <summary>
		/// Gets the name of the platform.
		/// </summary>
		/// <value>
		/// The name of the platform.
		/// </value>
		public override string PlatformName { get { return "x86"; } }

		/// <summary>
		/// Factory method for the Architecture class.
		/// </summary>
		/// <returns>The created architecture instance.</returns>
		/// <param name="architectureFeatures">The features available in the architecture and code generation.</param>
		/// <remarks>
		/// This method creates an instance of an appropriate architecture class, which supports the specific
		/// architecture features.
		/// </remarks>
		public static BaseArchitecture CreateArchitecture(ArchitectureFeatureFlags architectureFeatures)
		{
			if (architectureFeatures == ArchitectureFeatureFlags.AutoDetect)
				architectureFeatures = ArchitectureFeatureFlags.MMX | ArchitectureFeatureFlags.SSE | ArchitectureFeatureFlags.SSE2 | ArchitectureFeatureFlags.SSE3 | ArchitectureFeatureFlags.SSE4;

			return new Architecture(architectureFeatures);
		}

		/// <summary>
		/// Extends the pre-compiler pipeline with x86 compiler stages.
		/// </summary>
		/// <param name="compilerPipeline">The pipeline to extend.</param>
		public override void ExtendCompilerPipeline(CompilerPipeline compilerPipeline)
		{
			compilerPipeline.Add(
				new StartUpStage()
			);

			compilerPipeline.Add(
				new InterruptVectorStage()
			);

			compilerPipeline.Add(
				new SSEInitStage()
			);
		}

		/// <summary>
		/// Extends the method compiler pipeline with x86 specific stages.
		/// </summary>
		/// <param name="compilerPipeline">The method compiler pipeline to extend.</param>
		public override void ExtendMethodCompilerPipeline(CompilerPipeline compilerPipeline)
		{
			compilerPipeline.InsertBefore<LowerIRStage>(
				new IRSubstitutionStage()
			);

			compilerPipeline.InsertAfterLast<PlatformStubStage>(
				new IMethodCompilerStage[]
				{
					new PlatformIntrinsicStage(),
					new LongOperandTransformationStage(),
					new IRTransformationStage(),
					new TweakTransformationStage(),
					new FixedRegisterAssignmentStage(),
					new SimpleDeadCodeRemovalStage(),
					new AddressModeConversionStage(),
					new FloatingPointStage(),
				});

			compilerPipeline.InsertAfterLast<StackLayoutStage>(
				new BuildStackStage()
			);

			compilerPipeline.InsertBefore<CodeGenerationStage>(
				new FinalTweakTransformationStage()
			);

			compilerPipeline.InsertBefore<CodeGenerationStage>(
				new JumpOptimizationStage()
			);
		}

		/// <summary>
		/// Gets the code emitter.
		/// </summary>
		/// <returns></returns>
		public override BaseCodeEmitter GetCodeEmitter()
		{
			return new X86CodeEmitter();
		}

		/// <summary>
		/// Create platform move.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="source">The source.</param>
		public override void InsertMoveInstruction(Context context, Operand destination, Operand source)
		{
			BaseInstruction instruction = X86.Mov;
			var size = InstructionSize.Size32;

			if (destination.IsR4)
			{
				instruction = X86.Movss;
				size = InstructionSize.Size32;
			}
			else if (destination.IsR8)
			{
				instruction = X86.Movsd;
				size = InstructionSize.Size64;
			}

			context.AppendInstruction(instruction, size, destination, source);
		}

		public override void InsertStoreInstruction(Context context, Operand destination, Operand offset, Operand value)
		{
			BaseInstruction instruction = X86.MovStore;
			InstructionSize size = InstructionSize.Size32;

			if (value.IsR4)
			{
				instruction = X86.MovssStore;
			}
			else if (value.IsR8)
			{
				instruction = X86.MovsdStore;
				size = InstructionSize.Size64;
			}

			context.AppendInstruction(instruction, size, null, destination, offset, value);
		}

		/// <summary>
		/// Inserts the load instruction.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="source">The source.</param>
		/// <param name="offset">The offset.</param>
		public override void InsertLoadInstruction(Context context, Operand destination, Operand source, Operand offset)
		{
			BaseInstruction instruction = X86.MovLoad;
			var size = InstructionSize.Size32;

			if (destination.IsR4)
			{
				instruction = X86.MovssLoad;
			}
			else if (destination.IsR8)
			{
				instruction = X86.MovsdLoad;
				size = InstructionSize.Size64;
			}

			context.AppendInstruction(instruction, size, destination, source, offset);
		}

		/// <summary>
		/// Create platform compound move.
		/// </summary>
		/// <param name="compiler">The compiler.</param>
		/// <param name="context">The context.</param>
		/// <param name="destinationBase">The destination base.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="sourceBase">The source base.</param>
		/// <param name="source">The source.</param>
		/// <param name="size">The size.</param>
		public override void InsertCompoundCopy(BaseMethodCompiler compiler, Context context, Operand destinationBase, Operand destination, Operand sourceBase, Operand source, int size)
		{
			const int LargeAlignment = 16;
			int alignedSize = size - (size % NativeAlignment);
			int largeAlignedTypeSize = size - (size % LargeAlignment);

			Debug.Assert(size > 0);

			var srcReg = compiler.CreateVirtualRegister(destinationBase.Type.TypeSystem.BuiltIn.I4);
			var dstReg = compiler.CreateVirtualRegister(destinationBase.Type.TypeSystem.BuiltIn.I4);

			context.AppendInstruction(IRInstruction.UnstableObjectTracking);

			context.AppendInstruction(X86.Lea, srcReg, sourceBase, source);
			context.AppendInstruction(X86.Lea, dstReg, destinationBase, destination);

			var tmp = compiler.CreateVirtualRegister(destinationBase.Type.TypeSystem.BuiltIn.I4);
			var tmpLarge = compiler.CreateVirtualRegister(destinationBase.Type.TypeSystem.BuiltIn.R8);

			for (int i = 0; i < largeAlignedTypeSize; i += LargeAlignment)
			{
				// Large aligned moves allow 128bits to be copied at a time
				var index = Operand.CreateConstant(destinationBase.Type.TypeSystem.BuiltIn.I4, i);
				context.AppendInstruction(X86.MovupsLoad, InstructionSize.Size128, tmpLarge, srcReg, index);
				context.AppendInstruction(X86.MovupsStore, InstructionSize.Size128, null, dstReg, index, tmpLarge);
			}
			for (int i = largeAlignedTypeSize; i < alignedSize; i += NativeAlignment)
			{
				var index = Operand.CreateConstant(destinationBase.Type.TypeSystem.BuiltIn.I4, i);
				context.AppendInstruction(X86.MovLoad, InstructionSize.Size32, tmp, srcReg, index);
				context.AppendInstruction(X86.MovStore, InstructionSize.Size32, null, dstReg, index, tmp);
			}
			for (int i = alignedSize; i < size; i++)
			{
				var index = Operand.CreateConstant(destinationBase.Type.TypeSystem.BuiltIn.I4, i);
				context.AppendInstruction(X86.MovLoad, InstructionSize.Size8, tmp, srcReg, index);
				context.AppendInstruction(X86.MovStore, InstructionSize.Size8, null, dstReg, index, tmp);
			}

			context.AppendInstruction(IRInstruction.StableObjectTracking);
		}

		/// <summary>
		/// Creates the swap.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="source">The source.</param>
		public override void InsertExchangeInstruction(Context context, Operand destination, Operand source)
		{
			if (source.IsR4)
			{
				// TODO
				throw new CompilerException("R4 not implemented in InsertExchangeInstruction method");
			}
			else if (source.IsR8)
			{
				// TODO
				throw new CompilerException("R8 not implemented in InsertExchangeInstruction method");
			}
			else
			{
				context.AppendInstruction2(X86.Xchg, destination, source, source, destination);
			}
		}

		/// <summary>
		/// Inserts the jump instruction.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="destination">The destination.</param>
		public override void InsertJumpInstruction(Context context, BasicBlock destination)
		{
			context.AppendInstruction(X86.Jmp, destination);
		}

		/// <summary>
		/// Determines whether [is instruction move] [the specified instruction].
		/// </summary>
		/// <param name="instruction">The instruction.</param>
		/// <returns></returns>
		public override bool IsInstructionMove(BaseInstruction instruction)
		{
			return instruction == X86.Mov || instruction == X86.Movsd || instruction == X86.Movss;
		}
	}
}
