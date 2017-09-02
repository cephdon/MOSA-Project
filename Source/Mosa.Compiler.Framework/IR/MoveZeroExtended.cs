﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// Intermediate representation of a signed conversion context.
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.IR.BaseTwoOperandInstruction" />
	/// <remarks>
	/// This instruction takes the source operand and converts to the request size maintaining its sign.
	/// </remarks>
	public sealed class MoveZeroExtended : BaseTwoOperandInstruction
	{
	}
}
