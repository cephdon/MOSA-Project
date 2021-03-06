// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using Mosa.Compiler.Linker;
using Mosa.Compiler.MosaTypeSystem;
using System.IO;
using System.Text;

namespace Mosa.Platform.x86.CompilerStages
{
	/// <summary>
	/// Writes a multiboot v0.6.95 header into the generated binary.
	/// </summary>
	/// <remarks>
	/// This compiler stage writes a multiboot header into the
	/// the data section of the binary file and also creates a multiboot
	/// compliant entry point into the binary.<para/>
	/// The header and entry point written by this stage is compliant with
	/// the specification at
	/// http://www.gnu.org/software/grub/manual/multiboot/multiboot.html.
	/// </remarks>
	public sealed class Multiboot0695Stage : BaseCompilerStage
	{
		#region Constants

		/// <summary>
		/// This address is the top of the initial kernel stack.
		/// </summary>
		private const uint STACK_ADDRESS = 0x00A00000 - 8;

		/// <summary>
		/// Magic value in the multiboot header.
		/// </summary>
		private const uint HEADER_MB_MAGIC = 0x1BADB002U;

		/// <summary>
		/// Multiboot flag, which indicates that additional modules must be
		/// loaded on page boundaries.
		/// </summary>
		private const uint HEADER_MB_FLAG_MODULES_PAGE_ALIGNED = 0x00000001U;

		/// <summary>
		/// Multiboot flag, which indicates if memory information must be
		/// provided in the boot information structure.
		/// </summary>
		private const uint HEADER_MB_FLAG_MEMORY_INFO_REQUIRED = 0x00000002U;

		/// <summary>
		/// Multiboot flag, which indicates that the supported video mode
		/// table must be provided in the boot information structure.
		/// </summary>
		private const uint HEADER_MB_FLAG_VIDEO_MODES_REQUIRED = 0x00000004U;

		/// <summary>
		/// Multiboot flag, which indicates a non-elf binary to boot and that
		/// settings for the executable file should be read From the boot header
		/// instead of the executable header.
		/// </summary>
		private const uint HEADER_MB_FLAG_NON_ELF_BINARY = 0x00010000U;

		#endregion Constants

		#region Data members

		/// <summary>
		/// The multiboot method
		/// </summary>
		private MosaMethod multibootMethod;

		/// <summary>
		/// The multiboot header
		/// </summary>
		private LinkerSymbol multibootHeader;

		#endregion Data members

		public bool HasVideo { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Depth { get; set; }

		protected override void Setup()
		{
			HasVideo = CompilerOptions.GetCustomOptionAsBoolean("multiboot.video", false);
			Width = CompilerOptions.GetCustomOptionAsInteger("multiboot.width", 0);
			Height = CompilerOptions.GetCustomOptionAsInteger("multiboot.height", 0);
			Depth = CompilerOptions.GetCustomOptionAsInteger("multiboot.depth", 0);
		}

		protected override void RunPreCompile()
		{
			multibootHeader = Linker.CreateSymbol(MultibootHeaderSymbolName, SectionKind.Text, 1, 0x30);

			Linker.CreateSymbol(MultibootEAX, SectionKind.BSS, Architecture.NativeAlignment, Architecture.NativePointerSize);
			Linker.CreateSymbol(MultibootEBX, SectionKind.BSS, Architecture.NativeAlignment, Architecture.NativePointerSize);

			multibootMethod = Compiler.CreateLinkerMethod("MultibootInit");

			Linker.EntryPoint = Linker.GetSymbol(multibootMethod.FullName, SectionKind.Text);
		}

		protected override void RunPostCompile()
		{
			var eax = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EAX);
			var ebx = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EBX);
			var ebp = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EBP);
			var esp = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.ESP);

			var multibootEAX = Operand.CreateUnmanagedSymbolPointer(TypeSystem, Multiboot0695Stage.MultibootEAX);
			var multibootEBX = Operand.CreateUnmanagedSymbolPointer(TypeSystem, Multiboot0695Stage.MultibootEBX);

			var stackTop = Operand.CreateConstant(TypeSystem.BuiltIn.I4, STACK_ADDRESS);
			var zero = Operand.CreateConstant(TypeSystem.BuiltIn.I4, 0);
			var four = Operand.CreateConstant(TypeSystem.BuiltIn.I4, 4);

			var basicBlocks = new BasicBlocks();
			var block = basicBlocks.CreateBlock();
			basicBlocks.AddHeadBlock(block);
			var ctx = new Context(block);

			// Setup the stack and place the sentinel on the stack to indicate the start of the stack
			ctx.AppendInstruction(X86.Mov, InstructionSize.Size32, esp, stackTop);
			ctx.AppendInstruction(X86.Mov, InstructionSize.Size32, ebp, stackTop);
			ctx.AppendInstruction(X86.MovStore, InstructionSize.Size32, null, esp, zero, zero);
			ctx.AppendInstruction(X86.MovStore, InstructionSize.Size32, null, esp, four, zero);

			// Place the multiboot address into a static field
			ctx.AppendInstruction(X86.MovStore, InstructionSize.Size32, null, multibootEAX, zero, eax);
			ctx.AppendInstruction(X86.MovStore, InstructionSize.Size32, null, multibootEBX, zero, ebx);

			var startUpType = TypeSystem.GetTypeByName("Mosa.Runtime", "StartUp");
			var startUpMethod = startUpType.FindMethodByName("Initialize");

			var entryPoint = Operand.CreateSymbolFromMethod(TypeSystem, startUpMethod);
			ctx.AppendInstruction(X86.Call, null, entryPoint);

			ctx.AppendInstruction(X86.Ret);

			Compiler.CompileMethod(multibootMethod, basicBlocks);

			WriteMultibootHeader();
		}

		#region Internals

		private const string MultibootHeaderSymbolName = "<$>mosa-multiboot-header";
		public const string MultibootEAX = "<$>mosa-multiboot-eax";
		public const string MultibootEBX = "<$>mosa-multiboot-ebx";

		/// <summary>
		/// Writes the multiboot header.
		/// </summary>
		/// <param name="entryPoint">The virtualAddress of the multiboot compliant entry point.</param>
		private void WriteMultibootHeader()
		{
			// According to the multiboot specification this header must be within the first 8K of the kernel binary.
			Linker.SetFirst(multibootHeader);

			var stream = multibootHeader.Stream;

			var writer = new BinaryWriter(stream, Encoding.ASCII);

			// flags - multiboot flags
			uint flags = HEADER_MB_FLAG_MEMORY_INFO_REQUIRED | HEADER_MB_FLAG_MODULES_PAGE_ALIGNED;

			if (HasVideo)
				flags |= HEADER_MB_FLAG_VIDEO_MODES_REQUIRED;

			const uint load_addr = 0;

			// magic
			writer.Write(HEADER_MB_MAGIC);

			// flags
			writer.Write(flags);

			// checksum
			writer.Write(unchecked(0U - HEADER_MB_MAGIC - flags));

			// header_addr - load address of the multiboot header
			Linker.Link(LinkType.AbsoluteAddress, PatchType.I4, multibootHeader, (int)stream.Position, multibootHeader, 0);
			writer.Write(0);

			// load_addr - address of the binary in memory
			writer.Write(load_addr);

			// load_end_addr - address past the last byte to load from the image
			writer.Write(0);

			// bss_end_addr - address of the last byte to be zeroed out
			writer.Write(0);

			// entry_addr - address of the entry point to invoke
			Linker.Link(LinkType.AbsoluteAddress, PatchType.I4, multibootHeader, (int)stream.Position, Linker.EntryPoint, 0);
			writer.Write(0);

			// Write video settings if video has been specified, otherwise pad
			if (HasVideo)
			{
				writer.Write(0); // Mode, 0 = linear
				writer.Write(Width); // Width, 1280px
				writer.Write(Height); // Height, 720px
				writer.Write(Depth); // Depth, 24px
			}
			else
			{
				writer.Write(0);
				writer.Write(0);
				writer.Write(0);
				writer.Write(0);
			}
		}

		#endregion Internals
	}
}
