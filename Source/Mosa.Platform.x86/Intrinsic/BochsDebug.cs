﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Intrinsic
{
	/// <summary>
	/// Representations the x86 BochsDebug instruction.
	/// </summary>
	internal sealed class BochsDebug : IIntrinsicPlatformMethod
	{
		#region Methods

		/// <summary>
		/// Replaces the intrinsic call site
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="typeSystem">The type system.</param>
		void IIntrinsicPlatformMethod.ReplaceIntrinsicCall(Context context, BaseMethodCompiler methodCompiler)
		{
			// xchg	bx, bx
			var bx = Operand.CreateCPURegister(methodCompiler.TypeSystem.BuiltIn.U2, GeneralPurposeRegister.EBX);
			context.SetInstruction2(X86.Xchg, bx, bx, bx, bx);
		}

		#endregion Methods
	}
}
