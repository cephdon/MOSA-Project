﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// An abstract intermediate representation of the start of an try block.
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.IR.BaseIRInstruction" />
	public sealed class TryStart : BaseIRInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="TryStart"/>.
		/// </summary>
		public TryStart() :
			base(0, 0)
		{
		}

		#endregion Construction

		#region Properties

		/// <summary>
		/// Gets a value indicating whether to [ignore during code generation].
		/// </summary>
		/// <value>
		/// <c>true</c> if [ignore during code generation]; otherwise, <c>false</c>.
		/// </value>
		public override bool IgnoreDuringCodeGeneration { get { return true; } }

		/// <summary>
		/// Gets a value indicating whether to [ignore instruction's basic block].
		/// </summary>
		/// <value>
		/// <c>true</c> if [ignore instruction basic block]; otherwise, <c>false</c>.
		/// </value>
		public override bool IgnoreInstructionBasicBlockTargets { get { return true; } }

		#endregion Properties
	}
}
