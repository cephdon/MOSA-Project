﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// Retrieves the address of the variable represented by its operand.
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.IR.BaseTwoOperandInstruction" />
	/// <remarks>
	/// The address of instruction is used to retrieve the memory address
	/// of its sole operand. The operand may not represent a register.
	/// </remarks>
	public sealed class AddressOf : BaseTwoOperandInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="AddressOf"/>.
		/// </summary>
		public AddressOf()
		{
		}

		#endregion Construction
	}
}
