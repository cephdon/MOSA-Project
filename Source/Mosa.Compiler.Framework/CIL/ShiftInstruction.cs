﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Diagnostics;

namespace Mosa.Compiler.Framework.CIL
{
	/// <summary>
	/// Intermediate representation for IL shift instructions.
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.CIL.BinaryInstruction" />
	public sealed class ShiftInstruction : BinaryInstruction
	{
		#region Operand Table

		/// <summary>
		/// This operand table conforms to ISO/IEC 23271:2006 (E), Partition III, §1.5, Table 6.
		/// </summary>
		private static readonly StackTypeCode[][] operandTable = new StackTypeCode[][] {
			new StackTypeCode[] { StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown },
			new StackTypeCode[] { StackTypeCode.Unknown, StackTypeCode.Int32,   StackTypeCode.Unknown, StackTypeCode.Int32,   StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown },
			new StackTypeCode[] { StackTypeCode.Unknown, StackTypeCode.Int64,   StackTypeCode.Int64,   StackTypeCode.Int64,   StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown },
			new StackTypeCode[] { StackTypeCode.Unknown, StackTypeCode.N,       StackTypeCode.Unknown, StackTypeCode.N,       StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown },
			new StackTypeCode[] { StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown },
			new StackTypeCode[] { StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown },
			new StackTypeCode[] { StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown },
			new StackTypeCode[] { StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown, StackTypeCode.Unknown },
		};

		#endregion Operand Table

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="ShiftInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public ShiftInstruction(OpCode opcode)
			: base(opcode, 1)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Validates the instruction operands and creates a matching variable for the result.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="compiler">The compiler.</param>
		/// <exception cref="ArgumentNullException">context</exception>
		/// <exception cref="InvalidOperationException">Invalid virtualLocal state for pairing (" + context.Operand1.Type.GetStackType() + ", " + context.Operand2.Type.GetStackType() + ")</exception>
		public override void Resolve(Context context, BaseMethodCompiler compiler)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			base.Resolve(context, compiler);

			var result = operandTable[(int)context.Operand1.Type.GetStackTypeCode()][(int)context.Operand2.Type.GetStackTypeCode()];
			Debug.Assert(StackTypeCode.Unknown != result, "Can't shift with the given virtualLocal operands.");

			if (StackTypeCode.Unknown == result)
				throw new InvalidOperationException("Invalid virtualLocal state for pairing (" + context.Operand1.Type.GetStackType() + ", " + context.Operand2.Type.GetStackType() + ")");

			context.Result = compiler.CreateVirtualRegister(compiler.TypeSystem.GetStackTypeFromCode(result));
		}

		#endregion Methods
	}
}
