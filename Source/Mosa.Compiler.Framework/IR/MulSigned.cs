﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// Intermediate representation of the signed multiplication operation.
	/// </summary>
	/// <remarks>
	/// The mul instruction is a three-address instruction, where the result receives
	/// the value of the first operand (index 0) multiplied with the second operand (index 1).
	/// <para />
	/// Both the first and second operand must be the same integral type. If the second operand
	/// is statically or dynamically equal to or larger than the number of bits in the first
	/// operand, the result is undefined.
	/// </remarks>
	public sealed class MulSigned : ThreeOperandInstruction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MulSigned"/>.
		/// </summary>
		public MulSigned()
		{
		}
	}
}
