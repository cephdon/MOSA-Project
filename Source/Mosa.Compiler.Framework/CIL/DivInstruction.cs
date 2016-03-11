﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.CIL
{
	/// <summary>
	///
	/// </summary>
	public sealed class DivInstruction : ArithmeticInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="DivInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public DivInstruction(OpCode opcode)
			: base(opcode)
		{
		}

		#endregion Construction
	}
}
