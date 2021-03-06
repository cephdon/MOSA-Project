﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// IR Instructions
	/// </summary>
	public static class IRInstruction
	{
		public static readonly AddFloatR4 AddFloatR4 = new AddFloatR4();
		public static readonly AddFloatR8 AddFloatR8 = new AddFloatR8();
		public static readonly AddressOf AddressOf = new AddressOf();
		public static readonly AddSigned AddSigned = new AddSigned();
		public static readonly AddUnsigned AddUnsigned = new AddUnsigned();
		public static readonly ArithmeticShiftRight ArithmeticShiftRight = new ArithmeticShiftRight();
		public static readonly BlockEnd BlockEnd = new BlockEnd();
		public static readonly BlockStart BlockStart = new BlockStart();
		public static readonly Break Break = new Break();
		public static readonly Call Call = new Call();
		public static readonly CallDirect CallDirect = new CallDirect();
		public static readonly CallDynamic CallDynamic = new CallDynamic();
		public static readonly CallInterface CallInterface = new CallInterface();
		public static readonly CallStatic CallStatic = new CallStatic();
		public static readonly CallVirtual CallVirtual = new CallVirtual();
		public static readonly CompareFloatR4 CompareFloatR4 = new CompareFloatR4();
		public static readonly CompareFloatR8 CompareFloatR8 = new CompareFloatR8();
		public static readonly CompareInteger CompareInteger = new CompareInteger();
		public static readonly CompareIntegerBranch CompareIntegerBranch = new CompareIntegerBranch();
		public static readonly ConversionFloatR4ToFloatR8 ConversionFloatR4ToFloatR8 = new ConversionFloatR4ToFloatR8();
		public static readonly ConversionFloatR4ToInteger ConversionFloatR4ToInteger = new ConversionFloatR4ToInteger();
		public static readonly ConversionFloatR8ToFloatR4 ConversionFloatR8ToFloatR4 = new ConversionFloatR8ToFloatR4();
		public static readonly ConversionFloatR8ToInteger ConversionFloatR8ToInteger = new ConversionFloatR8ToInteger();
		public static readonly ConversionIntegerToFloatR4 ConversionIntegerToFloatR4 = new ConversionIntegerToFloatR4();
		public static readonly ConversionIntegerToFloatR8 ConversionIntegerToFloatR8 = new ConversionIntegerToFloatR8();
		public static readonly DivFloatR4 DivFloatR4 = new DivFloatR4();
		public static readonly DivFloatR8 DivFloatR8 = new DivFloatR8();
		public static readonly DivSigned DivSigned = new DivSigned();
		public static readonly DivUnsigned DivUnsigned = new DivUnsigned();
		public static readonly Epilogue Epilogue = new Epilogue();
		public static readonly ExceptionEnd ExceptionEnd = new ExceptionEnd();
		public static readonly ExceptionStart ExceptionStart = new ExceptionStart();
		public static readonly FilterEnd FilterEnd = new FilterEnd();
		public static readonly FilterStart FilterStart = new FilterStart();
		public static readonly FinallyEnd FinallyEnd = new FinallyEnd();
		public static readonly FinallyStart FinallyStart = new FinallyStart();
		public static readonly Flow Flow = new Flow();
		public static readonly Gen Gen = new Gen();
		public static readonly GotoLeaveTarget GotoLeaveTarget = new GotoLeaveTarget();
		public static readonly IntrinsicMethodCall IntrinsicMethodCall = new IntrinsicMethodCall();
		public static readonly IsInstanceOfType IsInstanceOfType = new IsInstanceOfType();
		public static readonly IsInstanceOfInterfaceType IsInstanceOfInterfaceType = new IsInstanceOfInterfaceType();
		public static readonly Jmp Jmp = new Jmp();
		public static readonly Kill Kill = new Kill();
		public static readonly KillAll KillAll = new KillAll();
		public static readonly KillAllExcept KillAllExcept = new KillAllExcept();
		public static readonly LoadCompound LoadCompound = new LoadCompound();
		public static readonly LoadFloatR4 LoadFloatR4 = new LoadFloatR4();
		public static readonly LoadFloatR8 LoadFloatR8 = new LoadFloatR8();
		public static readonly LoadInteger LoadInteger = new LoadInteger();
		public static readonly LoadSignExtended LoadSignExtended = new LoadSignExtended();
		public static readonly LoadZeroExtended LoadZeroExtended = new LoadZeroExtended();
		public static readonly LoadParameterCompound LoadParameterCompound = new LoadParameterCompound();
		public static readonly LoadParameterFloatR4 LoadParameterFloatR4 = new LoadParameterFloatR4();
		public static readonly LoadParameterFloatR8 LoadParameterFloatR8 = new LoadParameterFloatR8();
		public static readonly LoadParameterInteger LoadParameterInteger = new LoadParameterInteger();
		public static readonly LoadParameterSignExtended LoadParameterSignExtended = new LoadParameterSignExtended();
		public static readonly LoadParameterZeroExtended LoadParameterZeroExtended = new LoadParameterZeroExtended();
		public static readonly LogicalAnd LogicalAnd = new LogicalAnd();
		public static readonly LogicalNot LogicalNot = new LogicalNot();
		public static readonly LogicalOr LogicalOr = new LogicalOr();
		public static readonly LogicalXor LogicalXor = new LogicalXor();
		public static readonly MemorySet MemorySet = new MemorySet();
		public static readonly MoveCompound MoveCompound = new MoveCompound();
		public static readonly MoveFloatR4 MoveFloatR4 = new MoveFloatR4();
		public static readonly MoveFloatR8 MoveFloatR8 = new MoveFloatR8();
		public static readonly MoveInteger MoveInteger = new MoveInteger();
		public static readonly MoveSignExtended MoveSignExtended = new MoveSignExtended();
		public static readonly MoveZeroExtended MoveZeroExtended = new MoveZeroExtended();
		public static readonly MulFloatR4 MulFloatR4 = new MulFloatR4();
		public static readonly MulFloatR8 MulFloatR8 = new MulFloatR8();
		public static readonly MulSigned MulSigned = new MulSigned();
		public static readonly MulUnsigned MulUnsigned = new MulUnsigned();
		public static readonly NewArray NewArray = new NewArray();
		public static readonly NewObject NewObject = new NewObject();
		public static readonly NewString NewString = new NewString();
		public static readonly Nop Nop = new Nop();
		public static readonly Phi Phi = new Phi();
		public static readonly Prologue Prologue = new Prologue();
		public static readonly RemFloatR4 RemFloatR4 = new RemFloatR4();
		public static readonly RemFloatR8 RemFloatR8 = new RemFloatR8();
		public static readonly RemSigned RemSigned = new RemSigned();
		public static readonly RemUnsigned RemUnsigned = new RemUnsigned();
		public static readonly SetReturn SetReturn = new SetReturn();
		public static readonly SetLeaveTarget SetLeaveTarget = new SetLeaveTarget();
		public static readonly ShiftLeft ShiftLeft = new ShiftLeft();
		public static readonly ShiftRight ShiftRight = new ShiftRight();
		public static readonly StableObjectTracking StableObjectTracking = new StableObjectTracking();
		public static readonly StoreCompound StoreCompound = new StoreCompound();
		public static readonly StoreFloatR4 StoreFloatR4 = new StoreFloatR4();
		public static readonly StoreFloatR8 StoreFloatR8 = new StoreFloatR8();
		public static readonly StoreInteger StoreInteger = new StoreInteger();
		public static readonly StoreParameterCompound StoreParameterCompound = new StoreParameterCompound();
		public static readonly StoreParameterFloatR4 StoreParameterFloatR4 = new StoreParameterFloatR4();
		public static readonly StoreParameterFloatR8 StoreParameterFloatR8 = new StoreParameterFloatR8();
		public static readonly StoreParameterInteger StoreParameterInteger = new StoreParameterInteger();
		public static readonly SubFloatR4 SubFloatR4 = new SubFloatR4();
		public static readonly SubFloatR8 SubFloatR8 = new SubFloatR8();
		public static readonly SubSigned SubSigned = new SubSigned();
		public static readonly SubUnsigned SubUnsigned = new SubUnsigned();
		public static readonly Switch Switch = new Switch();
		public static readonly Throw Throw = new Throw();
		public static readonly To64 To64 = new To64();
		public static readonly TryEnd TryEnd = new TryEnd();
		public static readonly TryStart TryStart = new TryStart();
		public static readonly UnstableObjectTracking UnstableObjectTracking = new UnstableObjectTracking();

		public static readonly Rethrow Rethrow = new Rethrow();
		public static readonly GetVirtualFunctionPtr GetVirtualFunctionPtr = new GetVirtualFunctionPtr();
		public static readonly MemoryCopy MemoryCopy = new MemoryCopy();
		public static readonly Box Box = new Box();
		public static readonly Box32 Box32 = new Box32();
		public static readonly Box64 Box64 = new Box64();
		public static readonly BoxR4 BoxR4 = new BoxR4();
		public static readonly BoxR8 BoxR8 = new BoxR8();
		public static readonly Unbox Unbox = new Unbox();
		public static readonly Unbox32 Unbox32 = new Unbox32();
		public static readonly Unbox64 Unbox64 = new Unbox64();
		public static readonly Split64 Split64 = new Split64();
	}
}
