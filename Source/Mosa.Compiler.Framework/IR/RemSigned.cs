﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// Intermediate representation of the signed remainder operation.
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.IR.BaseThreeOperandInstruction" />
	/// <remarks>
	/// The instruction is a three-address instruction, where the result receives
	/// the remainder of the division of the first operand (index 0) by the second operand (index 1).
	/// <para />
	/// Both the first and second operand must be the same integral type. If the second operand
	/// is statically or dynamically equal to or larger than the number of bits in the first
	/// operand, the result is undefined.
	/// </remarks>
	public sealed class RemSigned : BaseThreeOperandInstruction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RemSigned"/>.
		/// </summary>
		public RemSigned()
		{
		}
	}
}
