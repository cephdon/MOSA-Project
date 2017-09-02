// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using System.Diagnostics;

namespace Mosa.Platform.x86.Stages
{
	/// <summary>
	/// Fixed Register Assignment Stage
	/// </summary>
	/// <seealso cref="Mosa.Platform.x86.BaseTransformationStage" />
	public sealed class FixedRegisterAssignmentStage : BaseTransformationStage
	{
		protected override void PopulateVisitationDictionary()
		{
			AddVisitation(X86.Cdq, Cdq);
			AddVisitation(X86.Div, Div);
			AddVisitation(X86.IDiv, IDiv);
			AddVisitation(X86.IMul, IMul);
			AddVisitation(X86.In, In);
			AddVisitation(X86.Mul, Mul);
			AddVisitation(X86.Out, Out);
			AddVisitation(X86.Rcr, Rcr);
			AddVisitation(X86.Sar, Sar);
			AddVisitation(X86.Shl, Shl);
			AddVisitation(X86.Shld, Shld);
			AddVisitation(X86.Shr, Shr);
			AddVisitation(X86.Shrd, Shrd);
		}

		#region Visitation Methods

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Cdq"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Cdq(Context context)
		{
			if (context.Result.IsCPURegister && context.Result2.IsCPURegister && context.Operand1.IsCPURegister)
				return;

			Operand operand1 = context.Operand1;
			Operand result = context.Result;
			Operand result2 = context.Result2;

			Operand EAX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EAX);
			Operand EDX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EDX);

			context.SetInstruction(X86.Mov, EAX, operand1);
			context.AppendInstruction2(X86.Cdq, EDX, EAX, EAX);
			context.AppendInstruction(X86.Mov, result, EDX);
			context.AppendInstruction(X86.Mov, result2, EAX);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Div"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Div(Context context)
		{
			if (context.Result.IsCPURegister
				&& context.Result2.IsCPURegister
				&& context.Operand1.IsCPURegister
				&& context.Result.Register == GeneralPurposeRegister.EDX
				&& context.Result2.Register == GeneralPurposeRegister.EAX
				&& context.Operand1.Register == GeneralPurposeRegister.EDX
				&& context.Operand2.Register == GeneralPurposeRegister.EAX)
				return;

			Operand operand1 = context.Operand1;
			Operand operand2 = context.Operand2;
			Operand operand3 = context.Operand3;
			Operand result = context.Result;
			Operand result2 = context.Result2;

			Operand EAX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EAX);
			Operand EDX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EDX);

			context.SetInstruction(X86.Mov, EDX, operand1);
			context.AppendInstruction(X86.Mov, EAX, operand2);

			if (operand3.IsCPURegister)
			{
				context.AppendInstruction2(X86.Div, EDX, EAX, EDX, EAX, operand3);
			}
			else
			{
				Operand v3 = AllocateVirtualRegister(TypeSystem.BuiltIn.I4);
				context.AppendInstruction(X86.Mov, v3, operand3);
				context.AppendInstruction2(X86.Div, EDX, EAX, EDX, EAX, v3);
			}

			context.AppendInstruction(X86.Mov, result2, EAX);
			context.AppendInstruction(X86.Mov, result, EDX);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.IDiv"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void IDiv(Context context)
		{
			if (context.Result.IsCPURegister
				&& context.Result2.IsCPURegister
				&& context.Operand1.IsCPURegister
				&& context.Operand2.IsCPURegister
				&& context.Result.Register == GeneralPurposeRegister.EDX
				&& context.Result2.Register == GeneralPurposeRegister.EAX
				&& context.Operand1.Register == GeneralPurposeRegister.EDX
				&& context.Operand2.Register == GeneralPurposeRegister.EAX)
				return;

			Operand operand1 = context.Operand1;
			Operand operand2 = context.Operand2;
			Operand operand3 = context.Operand3;
			Operand result = context.Result;
			Operand result2 = context.Result2;

			Operand EAX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EAX);
			Operand EDX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EDX);

			context.SetInstruction(X86.Mov, EDX, operand1);
			context.AppendInstruction(X86.Mov, EAX, operand2);

			if (operand3.IsCPURegister)
			{
				context.AppendInstruction2(X86.IDiv, EDX, EAX, EDX, EAX, operand3);
			}
			else
			{
				Operand v3 = AllocateVirtualRegister(TypeSystem.BuiltIn.I4);
				context.AppendInstruction(X86.Mov, v3, operand3);
				context.AppendInstruction2(X86.IDiv, EDX, EAX, EDX, EAX, v3);
			}

			context.AppendInstruction(X86.Mov, result2, EAX);
			context.AppendInstruction(X86.Mov, result, EDX);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.IMul" /> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void IMul(Context context)
		{
			if (!context.Operand2.IsConstant)
				return;

			Operand v1 = AllocateVirtualRegister(context.Operand2.Type);
			Operand operand2 = context.Operand2;

			context.Operand2 = v1;
			context.InsertBefore().SetInstruction(X86.Mov, v1, operand2);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.In"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void In(Context context)
		{
			if (context.Result.IsCPURegister
				&& context.Operand1.IsCPURegister
				&& context.Result.Register == GeneralPurposeRegister.EAX
				&& (context.Operand1.Register == GeneralPurposeRegister.EDX || context.Operand1.IsConstant))
				return;

			Operand result = context.Result;
			Operand operand1 = context.Operand1;

			Operand EDX = Operand.CreateCPURegister(operand1.Type, GeneralPurposeRegister.EDX);
			Operand EAX = Operand.CreateCPURegister(TypeSystem.BuiltIn.U4, GeneralPurposeRegister.EAX);

			var size = context.Size;

			context.SetInstruction(X86.Mov, EDX, operand1);
			context.AppendInstruction(X86.In, size, EAX, EDX);
			context.AppendInstruction(X86.Mov, result, EAX);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Mul"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Mul(Context context)
		{
			if (context.Result.IsCPURegister
				&& context.Result2.IsCPURegister
				&& context.Operand1.IsCPURegister
				&& !context.Operand2.IsConstant
				&& context.Result.Register == GeneralPurposeRegister.EDX
				&& context.Result2.Register == GeneralPurposeRegister.EAX
				&& context.Operand1.Register == GeneralPurposeRegister.EAX)
				return;

			Operand operand1 = context.Operand1;
			Operand operand2 = context.Operand2;
			Operand result = context.Result;
			Operand result2 = context.Result2;

			Operand EAX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EAX);
			Operand EDX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EDX);

			context.SetInstruction(X86.Mov, EAX, operand1);

			if (operand2.IsConstant)
			{
				Operand v3 = AllocateVirtualRegister(TypeSystem.BuiltIn.I4);
				context.AppendInstruction(X86.Mov, v3, operand2);
				operand2 = v3;
			}

			Debug.Assert(operand2.IsCPURegister || operand2.IsVirtualRegister);

			context.AppendInstruction2(X86.Mul, EDX, EAX, EAX, operand2);
			context.AppendInstruction(X86.Mov, result, EDX);
			context.AppendInstruction(X86.Mov, result2, EAX);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Out"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Out(Context context)
		{
			// TRANSFORM: OUT <= EDX, EAX && OUT <= imm8, EAX
			var size = context.Size;

			if (context.Operand1.IsCPURegister
				&& context.Operand2.IsCPURegister
				&& (context.Operand1.Register == GeneralPurposeRegister.EDX || context.Operand1.IsConstant)
				&& context.Operand2.Register == GeneralPurposeRegister.EAX)
				return;

			Operand operand1 = context.Operand1;
			Operand operand2 = context.Operand2;

			Operand EDX = Operand.CreateCPURegister(operand1.Type, GeneralPurposeRegister.EDX);
			Operand EAX = Operand.CreateCPURegister(operand2.Type, GeneralPurposeRegister.EAX);

			context.SetInstruction(X86.Mov, EDX, operand1);
			context.AppendInstruction(X86.Mov, EAX, operand2);
			context.AppendInstruction(X86.Out, size, null, EDX, EAX);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Rcr"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Rcr(Context context)
		{
			HandleShiftOperation(context, X86.Rcr);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Sar"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Sar(Context context)
		{
			HandleShiftOperation(context, X86.Sar);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Shl"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Shl(Context context)
		{
			HandleShiftOperation(context, X86.Shl);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Shld"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Shld(Context context)
		{
			if (context.Operand3.IsConstant || context.Operand3.IsCPURegister)
				return;

			if (context.Operand3.Register == GeneralPurposeRegister.ECX)
				return;

			Operand operand1 = context.Operand1;
			Operand operand2 = context.Operand2;
			Operand operand3 = context.Operand3;
			Operand result = context.Result;

			Debug.Assert(result == operand1);

			Operand ECX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.ECX);

			context.SetInstruction(X86.Mov, ECX, operand3);
			context.AppendInstruction(X86.Shld, result, operand1, operand2, ECX);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Shr"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Shr(Context context)
		{
			HandleShiftOperation(context, X86.Shr);
		}

		/// <summary>
		/// Visitation function for <see cref="IX86Visitor.Shrd"/> instructions.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Shrd(Context context)
		{
			if (context.Operand3.IsConstant || context.Operand3.IsCPURegister)
				return;

			if (context.Operand3.Register == GeneralPurposeRegister.ECX)
				return;

			Operand operand1 = context.Operand1;
			Operand operand2 = context.Operand2;
			Operand operand3 = context.Operand3;
			Operand result = context.Result;

			Debug.Assert(result == operand1);

			Operand ECX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.ECX);

			context.SetInstruction(X86.Mov, ECX, operand3);
			context.AppendInstruction(X86.Shrd, result, operand1, operand2, ECX);
		}

		#endregion Visitation Methods

		private void HandleShiftOperation(Context context, BaseInstruction instruction)
		{
			if (context.Operand2.IsConstant || context.Operand2.IsCPURegister)
				return;

			Operand operand1 = context.Operand1;
			Operand operand2 = context.Operand2;
			Operand result = context.Result;

			Operand ECX = Operand.CreateCPURegister(TypeSystem.BuiltIn.I4, GeneralPurposeRegister.ECX);

			context.SetInstruction(X86.Mov, ECX, operand2);
			context.AppendInstruction(X86.Mov, result, operand1);
			context.AppendInstruction(instruction, result, result, ECX);
		}
	}
}
