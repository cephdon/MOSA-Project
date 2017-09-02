﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.MosaTypeSystem;

namespace Mosa.Compiler.Framework.CIL
{
	/// <summary>
	/// Intermediate representation for stobj and stind.* IL instructions.
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.CIL.BinaryInstruction" />
	public sealed class StobjInstruction : BinaryInstruction
	{
		#region Data members

		/// <summary>
		/// Specifies the type of the value.
		/// </summary>
		private readonly MosaTypeCode? elementType;

		#endregion Data members

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="StobjInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public StobjInstruction(OpCode opcode)
			: base(opcode)
		{
			switch (opcode)
			{
				case OpCode.Stind_i1: elementType = MosaTypeCode.I1; break;
				case OpCode.Stind_i2: elementType = MosaTypeCode.I2; break;
				case OpCode.Stind_i4: elementType = MosaTypeCode.I4; break;
				case OpCode.Stind_i8: elementType = MosaTypeCode.I8; break;
				case OpCode.Stind_r4: elementType = MosaTypeCode.R4; break;
				case OpCode.Stind_r8: elementType = MosaTypeCode.R8; break;
				case OpCode.Stind_i: elementType = MosaTypeCode.I; break;
				case OpCode.Stind_ref: elementType = MosaTypeCode.Object; break;
				case OpCode.Stobj: elementType = null; break;
				default: throw new NotImplementCompilerException();
			}
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Decodes the specified instruction.
		/// </summary>
		/// <param name="node">The context.</param>
		/// <param name="decoder">The instruction decoder, which holds the code stream.</param>
		public override void Decode(InstructionNode node, IInstructionDecoder decoder)
		{
			// Decode base classes first
			base.Decode(node, decoder);

			MosaType type = (elementType == null)
				? type = (MosaType)decoder.Instruction.Operand
				: type = decoder.TypeSystem.GetTypeFromTypeCode(elementType.Value);

			node.MosaType = type;

			// FIXME: Check the value/destinations
		}

		/// <summary>
		/// Validates the instruction operands and creates a matching variable for the result.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="compiler">The compiler.</param>
		public override void Resolve(Context context, BaseMethodCompiler compiler)
		{
			base.Resolve(context, compiler);
		}

		#endregion Methods
	}
}
