﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// Intermediate representation of the shift right operation.
	/// </summary>
	/// <remarks>
	/// The shift instruction is a three-address instruction, where the result receives
	/// the value of the first operand (index 0) shifted by the number of bits specified by
	/// the second operand (index 1).
	/// <para />
	/// Both the first and second operand must be the same integral type. If the second operand
	/// is statically or dynamically equal to or larger than the number of bits in the first
	/// operand, the result is undefined.
	/// <para/>
	/// The most significant bits will be filled with zeroes by this context. To preserve
	/// the sign of the shifted value (inserting ones if it is negative), use the
	/// <see cref="ArithmeticShiftRight"/> instead.
	/// </remarks>
	public sealed class ShiftRight : ThreeOperandInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="ShiftRight"/>.
		/// </summary>
		public ShiftRight()
		{
		}

		#endregion Construction
	}
}
