﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 popfd instruction.
	/// </summary>
	public sealed class Popfd : X86Instruction
	{
		#region Data members

		private static readonly LegacyOpCode opcode = new LegacyOpCode(new byte[] { 0x9D });

		#endregion Data members

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Popfd"/>.
		/// </summary>
		public Popfd() :
			base(0, 0)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Computes the opcode.
		/// </summary>
		/// <param name="destination">The destination operand.</param>
		/// <param name="source">The source operand.</param>
		/// <param name="third">The third operand.</param>
		/// <returns></returns>
		internal override LegacyOpCode ComputeOpCode(Operand destination, Operand source, Operand third)
		{
			return opcode;
		}

		#endregion Methods
	}
}
