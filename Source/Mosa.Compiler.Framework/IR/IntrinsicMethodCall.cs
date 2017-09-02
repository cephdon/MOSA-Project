﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// Intermediate representation of call context.
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.IR.BaseIRInstruction" />
	public sealed class IntrinsicMethodCall : BaseIRInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="IntrinsicMethodCall"/> class.
		/// </summary>
		public IntrinsicMethodCall()
			: base(0, 0)
		{
		}

		#endregion Construction
	}
}
