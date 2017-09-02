// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// Load Sign Extended
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.IR.BaseThreeOperandInstruction" />
	/// <remarks>
	/// The load instruction is used to load a value from a memory pointer and an offset. The types must be compatible.
	/// </remarks>
	public sealed class LoadSignExtended : BaseThreeOperandInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="LoadSignExtended"/>.
		/// </summary>
		public LoadSignExtended()
		{
		}

		#endregion Construction
	}
}
