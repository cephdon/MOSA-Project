﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.MosaTypeSystem;
using System;

namespace Mosa.Compiler.Framework.CIL
{
	/// <summary>
	/// Implements the internal representation for the IL conversion instructions.
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.CIL.UnaryArithmeticInstruction" />
	public sealed class ConversionInstruction : UnaryArithmeticInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="ConversionInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public ConversionInstruction(OpCode opcode)
			: base(opcode)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Validates the instruction operands and creates a matching variable for the result.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="compiler">The compiler.</param>
		public override void Resolve(Context context, BaseMethodCompiler compiler)
		{
			base.Resolve(context, compiler);

			// Validate the typecode & determine the resulting stack type
			MosaType resultType;

			switch (opcode)
			{
				case OpCode.Conv_u: resultType = compiler.TypeSystem.BuiltIn.U; break;
				case OpCode.Conv_i: resultType = compiler.TypeSystem.BuiltIn.I; break;
				case OpCode.Conv_i1: resultType = compiler.TypeSystem.BuiltIn.I1; break;
				case OpCode.Conv_i2: resultType = compiler.TypeSystem.BuiltIn.I2; break;
				case OpCode.Conv_i4: resultType = compiler.TypeSystem.BuiltIn.I4; break;
				case OpCode.Conv_i8: resultType = compiler.TypeSystem.BuiltIn.I8; break;
				case OpCode.Conv_r4: resultType = compiler.TypeSystem.BuiltIn.R4; break;
				case OpCode.Conv_r8: resultType = compiler.TypeSystem.BuiltIn.R8; break;
				case OpCode.Conv_u1: resultType = compiler.TypeSystem.BuiltIn.U1; break;
				case OpCode.Conv_u2: resultType = compiler.TypeSystem.BuiltIn.U2; break;
				case OpCode.Conv_u4: resultType = compiler.TypeSystem.BuiltIn.U4; break;
				case OpCode.Conv_u8: resultType = compiler.TypeSystem.BuiltIn.U8; break;
				case OpCode.Conv_ovf_i: goto case OpCode.Conv_i;
				case OpCode.Conv_ovf_u: goto case OpCode.Conv_u;
				case OpCode.Conv_ovf_i_un: goto case OpCode.Conv_i;
				case OpCode.Conv_ovf_u_un: goto case OpCode.Conv_u;
				case OpCode.Conv_r_un: resultType = compiler.TypeSystem.BuiltIn.R8; break;
				default: throw new NotSupportedException("Overflow checking conversions not supported.");
			}

			context.Result = compiler.CreateVirtualRegister(resultType.GetStackType());
		}

		#endregion Methods
	}
}
