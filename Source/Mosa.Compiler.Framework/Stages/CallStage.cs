﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.Framework.IR;
using Mosa.Compiler.MosaTypeSystem;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mosa.Compiler.Framework.Stages
{
	/// <summary>
	/// Lower Call Stage
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.BaseCodeTransformationStage" />
	public sealed class CallStage : BaseCodeTransformationStage
	{
		protected override void PopulateVisitationDictionary()
		{
			AddVisitation(IRInstruction.CallInterface, CallInterface);
			AddVisitation(IRInstruction.CallStatic, CallStatic);
			AddVisitation(IRInstruction.CallVirtual, CallVirtual);
		}

		private int CalculateMethodTableOffset(MosaMethod invokeTarget)
		{
			int slot = TypeLayout.GetMethodTableOffset(invokeTarget);

			return NativePointerSize * slot;
		}

		private void CallStatic(InstructionNode node)
		{
		}

		private void CallVirtual(InstructionNode node)
		{
			var method = node.InvokeMethod;
			var thisPtr = node.Operand1;
			var resultOperand = node.Result;
			var operands = new List<Operand>(node.Operands);

			var typeDefinition = AllocateVirtualRegister(TypeSystem.BuiltIn.Pointer);
			var methodPtr = AllocateVirtualRegister(TypeSystem.BuiltIn.Pointer);

			Debug.Assert(!method.DeclaringType.IsInterface);

			var context = new Context(node);

			// Same as above except for methodPointer
			int methodPointerOffset = CalculateMethodTableOffset(method) + (NativePointerSize * 14);

			// Get the TypeDef pointer
			context.SetInstruction(IRInstruction.LoadInteger, NativeInstructionSize, typeDefinition, thisPtr, ConstantZero);

			// Get the address of the method
			context.AppendInstruction(IRInstruction.LoadInteger, NativeInstructionSize, methodPtr, typeDefinition, Operand.CreateConstant(TypeSystem, methodPointerOffset));

			MakeCall(context, method, methodPtr, resultOperand, operands);
		}

		private int CalculateInterfaceSlot(MosaType interaceType)
		{
			return TypeLayout.GetInterfaceSlotOffset(interaceType);
		}

		private int CalculateInterfaceSlotOffset(MosaMethod invokeTarget)
		{
			return CalculateInterfaceSlot(invokeTarget.DeclaringType) * NativePointerSize;
		}

		private void CallInterface(InstructionNode node)
		{
			var method = node.InvokeMethod;
			var thisPtr = node.Operand1;
			var resultOperand = node.Result;
			var operands = new List<Operand>(node.Operands);

			var typeDefinition = AllocateVirtualRegister(TypeSystem.BuiltIn.Pointer);
			var methodPtr = AllocateVirtualRegister(TypeSystem.BuiltIn.Pointer);

			Debug.Assert(method.DeclaringType.IsInterface);

			// Offset for InterfaceSlotTable in TypeDef
			int interfaceSlotTableOffset = (NativePointerSize * 11);

			// Offset for InterfaceMethodTable in InterfaceSlotTable
			int interfaceMethodTableOffset = (NativePointerSize * 1) + CalculateInterfaceSlotOffset(method);

			// Offset for MethodDef in InterfaceMethodTable
			int methodDefinitionOffset = (NativePointerSize * 2) + CalculateMethodTableOffset(method);

			// Offset for Method pointer in MethodDef
			int methodPointerOffset = (NativePointerSize * 4);

			// Operands to hold pointers
			var interfaceSlotPtr = AllocateVirtualRegister(TypeSystem.BuiltIn.Pointer);
			var interfaceMethodTablePtr = AllocateVirtualRegister(TypeSystem.BuiltIn.Pointer);
			var methodDefinition = AllocateVirtualRegister(TypeSystem.BuiltIn.Pointer);

			var context = new Context(node);

			// Get the TypeDef pointer
			context.SetInstruction(IRInstruction.LoadInteger, NativeInstructionSize, typeDefinition, thisPtr, ConstantZero);

			// Get the Interface Slot Table pointer
			context.AppendInstruction(IRInstruction.LoadInteger, NativeInstructionSize, interfaceSlotPtr, typeDefinition, Operand.CreateConstant(TypeSystem, interfaceSlotTableOffset));

			// Get the Interface Method Table pointer
			context.AppendInstruction(IRInstruction.LoadInteger, NativeInstructionSize, interfaceMethodTablePtr, interfaceSlotPtr, Operand.CreateConstant(TypeSystem, interfaceMethodTableOffset));

			// Get the MethodDef pointer
			context.AppendInstruction(IRInstruction.LoadInteger, NativeInstructionSize, methodDefinition, interfaceMethodTablePtr, Operand.CreateConstant(TypeSystem, methodDefinitionOffset));

			// Get the address of the method
			context.AppendInstruction(IRInstruction.LoadInteger, NativeInstructionSize, methodPtr, methodDefinition, Operand.CreateConstant(TypeSystem, methodPointerOffset));

			MakeCall(context, method, methodPtr, resultOperand, operands);
		}

		private int CalculateReturnSize(MosaMethod method)
		{
			if (MosaTypeLayout.IsStoredOnStack(method.Signature.ReturnType))
			{
				return TypeLayout.GetTypeSize(method.Signature.ReturnType);
			}

			return 0;
		}

		private void MakeCall(Context context, MosaMethod method, Operand target, Operand result, List<Operand> operands)
		{
			var scratch = Operand.CreateCPURegister(TypeSystem.BuiltIn.Pointer, Architecture.ScratchRegister);

			context.Empty();

			int stackSize = TypeLayout.GetMethodParameterStackSize(method);
			int returnSize = CalculateReturnSize(method);

			int totalStack = returnSize + stackSize;

			ReserveStackSizeForCall(context, totalStack, scratch);
			PushOperands(context, method, operands, totalStack, scratch);

			// the mov/call two-instructions combo is to help facilitate the register allocator
			context.AppendInstruction(IRInstruction.MoveInteger, scratch, target);
			context.AppendInstruction(IRInstruction.CallDirect, null, target);
			context.InvokeMethod = method;

			GetReturnValue(context, result);
			FreeStackAfterCall(context, totalStack);
		}

		private void ReserveStackSizeForCall(Context context, int stackSize, Operand scratch)
		{
			if (stackSize == 0)
				return;

			var stackPointer = Operand.CreateCPURegister(TypeSystem.BuiltIn.Pointer, Architecture.StackPointerRegister);

			context.AppendInstruction(IRInstruction.SubSigned, stackPointer, stackPointer, Operand.CreateConstant(TypeSystem.BuiltIn.I4, stackSize));
			context.AppendInstruction(IRInstruction.MoveInteger, scratch, stackPointer);
		}

		private void FreeStackAfterCall(Context context, int stackSize)
		{
			if (stackSize == 0)
				return;

			var stackPointer = Operand.CreateCPURegister(TypeSystem.BuiltIn.Pointer, Architecture.StackPointerRegister);
			context.AppendInstruction(IRInstruction.AddSigned, stackPointer, stackPointer, Operand.CreateConstant(TypeSystem.BuiltIn.I4, stackSize));
		}

		private void PushOperands(Context context, MosaMethod method, List<Operand> operands, int space, Operand scratch)
		{
			Debug.Assert((method.Signature.Parameters.Count + (method.HasThis ? 1 : 0) == operands.Count)
						|| (method.DeclaringType.IsDelegate && method.Signature.Parameters.Count == operands.Count));

			for (int index = operands.Count - 1; index >= 0; index--)
			{
				var operand = operands[index];

				Architecture.GetTypeRequirements(TypeLayout, operand.Type, out int size, out int alignment);

				size = Alignment.AlignUp(size, alignment);

				space -= size;

				Push(context, operand, space, scratch);
			}
		}

		private void Push(Context context, Operand operand, int offset, Operand scratch)
		{
			var offsetOperand = Operand.CreateConstant(TypeSystem, offset);

			if (operand.IsInteger)
			{
				// same as 32-bit
				context.AppendInstruction(IRInstruction.StoreInteger, null, scratch, offsetOperand, operand);
			}
			else if (operand.IsR4)
			{
				context.AppendInstruction(IRInstruction.StoreFloatR4, null, scratch, offsetOperand, operand);
			}
			else if (operand.IsR8)
			{
				context.AppendInstruction(IRInstruction.StoreFloatR8, null, scratch, offsetOperand, operand);
			}
			else if (MosaTypeLayout.IsStoredOnStack(operand.Type))
			{
				// TODO: This is probably wrong!
				context.AppendInstruction(IRInstruction.MoveCompound, operand, offsetOperand);
			}
			else
			{
				throw new InvalidCompilerException();
			}
		}

		private void GetReturnValue(Context context, Operand result)
		{
			if (result == null)
				return;

			if (result.Is64BitInteger && Architecture.NativeIntegerSize == 4)
			{
				var returnLow = Operand.CreateCPURegister(result.Type, Architecture.Return32BitRegister);
				var returnHigh = Operand.CreateCPURegister(TypeSystem.BuiltIn.U4, Architecture.Return64BitRegister);

				context.AppendInstruction(IRInstruction.Gen, returnLow);
				context.AppendInstruction(IRInstruction.Gen, returnHigh);
				context.AppendInstruction(IRInstruction.To64, result, returnLow, returnHigh);
			}
			else if (result.IsInteger)
			{
				var returnLow = Operand.CreateCPURegister(result.Type, Architecture.Return32BitRegister);
				context.AppendInstruction(IRInstruction.Gen, returnLow);
				context.AppendInstruction(IRInstruction.MoveInteger, result, returnLow);
			}
			else if (result.IsR4)
			{
				var returnFP = Operand.CreateCPURegister(result.Type, Architecture.ReturnFloatingPointRegister);
				context.AppendInstruction(IRInstruction.Gen, returnFP);
				context.AppendInstruction(IRInstruction.MoveFloatR4, result, returnFP);
			}
			else if (result.IsR8)
			{
				var returnFP = Operand.CreateCPURegister(result.Type, Architecture.ReturnFloatingPointRegister);
				context.AppendInstruction(IRInstruction.Gen, returnFP);
				context.AppendInstruction(IRInstruction.MoveFloatR8, result, returnFP);
			}
			else if (MosaTypeLayout.IsStoredOnStack(result.Type))
			{
				// TODO: This is probably wrong!
				Debug.Assert(result.IsStackLocal);

				int size = TypeLayout.GetTypeSize(result.Type);
				var offsetOperand = Operand.CreateConstant(TypeSystem, size);

				context.AppendInstruction(IRInstruction.MoveCompound, result, offsetOperand);
			}
			else
			{
				throw new InvalidCompilerException();
			}
		}

		private void __OLD__ProcessInvokeInstruction(Context context, MosaMethod method, Operand symbolOperand, Operand resultOperand, List<Operand> operands)
		{
			Debug.Assert(method != null);

			context.AppendInstruction(IRInstruction.Call, (byte)(operands.Count + 1), (byte)(resultOperand == null ? 0 : 1));
			context.InvokeMethod = method;

			if (resultOperand != null)
			{
				context.Result = resultOperand;
			}

			int index = 0;
			context.SetOperand(index++, symbolOperand);
			foreach (var operand in operands)
			{
				context.SetOperand(index++, operand);
			}
		}
	}
}
