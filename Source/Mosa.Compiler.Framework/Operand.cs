// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;

using Mosa.Compiler.MosaTypeSystem;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mosa.Compiler.Framework
{
	/// <summary>
	/// Operand class
	/// </summary>
	public sealed class Operand
	{
		#region Properties

		/// <summary>
		/// Gets the constant double float point.
		/// </summary>
		/// <value>
		/// The constant double float point.
		/// </value>
		public double ConstantDoubleFloatingPoint { get; private set; }

		/// <summary>
		/// Gets or sets the constant signed integer.
		/// </summary>
		/// <value>
		/// The constant signed integer.
		/// </value>
		public int ConstantSignedInteger { get { return (int)ConstantUnsignedLongInteger; } set { ConstantUnsignedLongInteger = (ulong)value; } }

		/// <summary>
		/// Gets or sets the constant signed long integer.
		/// </summary>
		/// <value>
		/// The constant signed long integer.
		/// </value>
		public long ConstantSignedLongInteger { get { return (long)ConstantUnsignedLongInteger; } set { ConstantUnsignedLongInteger = (ulong)value; } }

		/// <summary>
		/// Gets the single double float point.
		/// </summary>
		/// <value>
		/// The single double float point.
		/// </value>
		public float ConstantSingleFloatingPoint { get; private set; }

		/// <summary>
		/// Gets the constant integer.
		/// </summary>
		/// <value>
		/// The constant integer.
		/// </value>
		public uint ConstantUnsignedInteger { get { return (uint)ConstantUnsignedLongInteger; } set { ConstantUnsignedLongInteger = value; } }

		/// <summary>
		/// Gets the constant long integer.
		/// </summary>
		/// <value>
		/// The constant long integer.
		/// </value>
		public ulong ConstantUnsignedLongInteger { get; set; }

		/// <summary>
		/// Holds a list of instructions, which define this operand.
		/// </summary>
		public List<InstructionNode> Definitions { get; }

		/// <summary>
		/// Retrieves the field.
		/// </summary>
		public MosaField Field { get; private set; }

		/// <summary>
		/// Gets or sets the high operand.
		/// </summary>
		/// <value>
		/// The high operand.
		/// </value>
		public Operand High { get; private set; }

		/// <summary>
		/// Gets the index.
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// Gets a value indicating whether [is 64 bit integer].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is signed integer]; otherwise, <c>false</c>.
		/// </value>
		public bool Is64BitInteger { get { return IsLong; } }

		public bool IsArray { get { return Type.IsArray; } }

		public bool IsBoolean { get { return Type.IsBoolean; } }

		public bool IsByte { get { return UnderlyingType.IsUI1; } }

		public bool IsChar { get { return UnderlyingType.IsChar; } }

		/// <summary>
		/// Determines if the operand is a constant variable.
		/// </summary>
		public bool IsConstant { get; private set; }

		/// <summary>
		/// Gets a value indicating whether [is constant one].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is constant one]; otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="InvalidCompilerException"></exception>
		public bool IsConstantOne
		{
			get
			{
				if (!IsResolvedConstant)
					return false;
				else if (IsInteger || IsBoolean || IsChar || IsPointer)
					return ConstantUnsignedLongInteger == 1;
				else if (IsStackLocal)
					return ConstantUnsignedLongInteger == 1;
				else if (IsParameter)
					return ConstantUnsignedLongInteger == 1;
				else if (IsR8)
					return ConstantDoubleFloatingPoint == 1;
				else if (IsR4)
					return ConstantSingleFloatingPoint == 1;
				else if (IsNull)
					return false;

				throw new InvalidCompilerException();
			}
		}

		/// <summary>
		/// Gets a value indicating whether [is constant zero].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is constant zero]; otherwise, <c>false</c>.
		/// </value>
		public bool IsConstantZero
		{
			get
			{
				if (!IsResolvedConstant)
					return false;
				else if (IsInteger || IsBoolean || IsChar || IsPointer)
					return ConstantUnsignedLongInteger == 0;
				else if (IsStackLocal)
					return ConstantUnsignedLongInteger == 0;
				else if (IsParameter)
					return ConstantUnsignedLongInteger == 0;
				else if (IsR8)
					return ConstantDoubleFloatingPoint == 0;
				else if (IsR4)
					return ConstantSingleFloatingPoint == 0;
				else if (IsNull)
					return true;

				throw new InvalidCompilerException();
			}
		}

		/// <summary>
		/// Determines if the operand is a cpu register operand.
		/// </summary>
		public bool IsCPURegister { get; private set; }

		/// <summary>
		/// Determines if the operand is a runtime member operand.
		/// </summary>
		public bool IsStaticField { get; private set; }

		public bool IsFunctionPointer { get { return Type.IsFunctionPointer; } }

		public bool IsI { get { return UnderlyingType.IsI; } }

		public bool IsI1 { get { return UnderlyingType.IsI1; } }

		public bool IsI2 { get { return UnderlyingType.IsI2; } }

		public bool IsI4 { get { return UnderlyingType.IsI4; } }

		public bool IsI8 { get { return UnderlyingType.IsI8; } }

		public bool IsInt { get { return UnderlyingType.IsUI4; } }

		public bool IsInteger { get { return UnderlyingType.IsInteger; } }

		/// <summary>
		/// Determines if the operand is a label operand.
		/// </summary>
		public bool IsLabel { get; private set; }

		public bool IsLong { get { return UnderlyingType.IsUI8; } }

		public bool IsManagedPointer { get { return Type.IsManagedPointer; } }

		/// <summary>
		/// Gets a value indicating whether [is null].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is null]; otherwise, <c>false</c>.
		/// </value>
		public bool IsNull { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this instance is on stack.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is on stack; otherwise, <c>false</c>.
		/// </value>
		public bool IsOnStack { get { return IsStackLocal || IsParameter; } }

		/// <summary>
		/// Determines if the operand is a local variable operand.
		/// </summary>
		public bool IsParameter { get; private set; }

		public bool IsPinned { get; private set; }

		public bool IsPointer { get { return Type.IsPointer; } }

		public bool IsR { get { return UnderlyingType.IsR; } }

		public bool IsR4 { get { return UnderlyingType.IsR4; } }

		public bool IsR8 { get { return UnderlyingType.IsR8; } }

		public bool IsReferenceType { get { return Type.IsReferenceType; } }

		public bool IsResolved { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance is resolved constant.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is resolved constant; otherwise, <c>false</c>.
		/// </value>
		public bool IsResolvedConstant { get { return IsConstant && IsResolved; } }

		/// <summary>
		/// Determines if the operand is a shift operand.
		/// </summary>
		public bool IsShift { get; }

		public bool IsShort { get { return UnderlyingType.IsUI2; } }

		public bool IsSigned { get { return UnderlyingType.IsSigned; } }

		/// <summary>
		/// Gets a value indicating whether this instance is split64 child.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is split64 child; otherwise, <c>false</c>.
		/// </value>
		public bool IsSplitChild { get { return SplitParent != null && !IsSSA; } }

		/// <summary>
		/// Determines if the operand is a ssa operand.
		/// </summary>
		public bool IsSSA { get; private set; }

		/// <summary>
		/// Determines if the operand is a local stack operand.
		/// </summary>
		public bool IsStackLocal { get; private set; }

		/// <summary>
		/// Determines if the operand is a symbol operand.
		/// </summary>
		public bool IsSymbol { get; private set; }

		public bool IsU { get { return UnderlyingType.IsU; } }

		public bool IsU1 { get { return UnderlyingType.IsU1; } }

		public bool IsU2 { get { return UnderlyingType.IsU2; } }

		public bool IsU4 { get { return UnderlyingType.IsU4; } }

		public bool IsU8 { get { return UnderlyingType.IsU8; } }

		public bool IsUnmanagedPointer { get { return Type.IsUnmanagedPointer; } }

		/// <summary>
		/// Gets a value indicating whether this instance is unresolved constant.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is unresolved constant; otherwise, <c>false</c>.
		/// </value>
		public bool IsUnresolvedConstant { get { return IsConstant && !IsResolved; } }

		public bool IsUnsigned { get { return UnderlyingType.IsUnsigned; } }

		public bool IsValueType { get { return UnderlyingType.IsValueType; } }

		public bool IsLinkerResolved { get { return IsLabel || IsStaticField || IsSymbol; } }

		/// <summary>
		/// Gets a value indicating whether this instance is this.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is this; otherwise, <c>false</c>.
		/// </value>
		public bool IsThis { get; private set; }

		/// <summary>
		/// Determines if the operand is a virtual register operand.
		/// </summary>
		public bool IsVirtualRegister { get; private set; }

		/// <summary>
		/// Gets or sets the low operand.
		/// </summary>
		/// <value>
		/// The low operand.
		/// </value>
		public Operand Low { get; private set; }

		/// <summary>
		/// Retrieves the method.
		/// </summary>
		/// <value>
		/// The method.
		/// </value>
		public MosaMethod Method { get; private set; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the offset.
		/// </summary>
		/// <value>
		/// The offset.
		/// </value>
		public long Offset
		{
			get
			{
				Debug.Assert(IsResolved);

				return ConstantSignedLongInteger;
			}
			set
			{
				Debug.Assert(!IsResolved);

				ConstantSignedLongInteger = value;
			}
		}

		/// <summary>
		/// Retrieves the register, where the operand is located.
		/// </summary>
		public Register Register { get; private set; }

		/// <summary>
		/// Gets the type of the shift.
		/// </summary>
		/// <value>
		/// The type of the shift.
		/// </value>
		public ShiftType ShiftType { get; }

		/// <summary>
		/// Gets the split64 parent.
		/// </summary>
		/// <value>
		/// The split64 parent.
		/// </value>
		public Operand SplitParent { get; private set; }

		/// <summary>
		/// Gets the base operand.
		/// </summary>
		public Operand SSAParent { get; private set; }

		/// <summary>
		/// Gets the ssa version.
		/// </summary>
		public int SSAVersion { get; private set; }

		/// <summary>
		/// Gets the string data.
		/// </summary>
		/// <value>
		/// The string data.
		/// </value>
		public string StringData { get; private set; }

		/// <summary>
		/// Returns the type of the operand.
		/// </summary>
		public MosaType Type { get; }

		/// <summary>
		/// Holds a list of instructions, which use this operand.
		/// </summary>
		public List<InstructionNode> Uses { get; }

		private MosaType UnderlyingType { get { return Type.GetEnumUnderlyingType(); } }

		#endregion Properties

		#region Construction

		private Operand()
		{
			Definitions = new List<InstructionNode>();
			Uses = new List<InstructionNode>();
			IsParameter = false;
			IsStackLocal = false;
			IsShift = false;
			IsConstant = false;
			IsVirtualRegister = false;
			IsLabel = false;
			IsCPURegister = false;
			IsSSA = false;
			IsSymbol = false;
			IsStaticField = false;
			IsParameter = false;
			IsResolved = false;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Operand"/>.
		/// </summary>
		/// <param name="type">The type of the operand.</param>
		private Operand(MosaType type)
				: this()
		{
			Debug.Assert(type != null);
			Type = type;
		}

		/// <summary>
		/// Prevents a default instance of the <see cref="Operand"/> class from being created.
		/// </summary>
		/// <param name="shiftType">Type of the shift.</param>
		private Operand(ShiftType shiftType)
				: this()
		{
			ShiftType = shiftType;
			IsShift = true;
			IsResolved = true;
		}

		#endregion Construction

		#region Static Factory Constructors

		/// <summary>
		/// Creates a new constant <see cref="Operand" /> for the given integral value.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value to create the constant operand for.</param>
		/// <returns>
		/// A new operand representing the value <paramref name="value" />.
		/// </returns>
		/// <exception cref="InvalidCompilerException"></exception>
		public static Operand CreateConstant(MosaType type, ulong value)
		{
			var operand = new Operand(type)
			{
				IsConstant = true,
				ConstantUnsignedLongInteger = value,
				IsNull = (type.IsReferenceType && value == 0),
				IsResolved = true
			};
			if (type.IsReferenceType && value != 0)
			{
				throw new InvalidCompilerException();
			}

			if (!(operand.IsInteger || operand.IsBoolean || operand.IsChar || operand.IsPointer || operand.IsReferenceType))
			{
				throw new InvalidCompilerException();
			}

			return operand;
		}

		/// <summary>
		/// Creates a new constant <see cref="Operand" /> for the given integral value.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value to create the constant operand for.</param>
		/// <returns>
		/// A new operand representing the value <paramref name="value" />.
		/// </returns>
		public static Operand CreateConstant(MosaType type, long value)
		{
			return CreateConstant(type, (ulong)value);
		}

		/// <summary>
		/// Creates a new constant <see cref="Operand" /> for the given integral value.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value to create the constant operand for.</param>
		/// <returns>
		/// A new operand representing the value <paramref name="value" />.
		/// </returns>
		public static Operand CreateConstant(MosaType type, int value)
		{
			return CreateConstant(type, (long)value);
		}

		/// <summary>
		/// Creates a new constant <see cref="Operand" /> for the given integral value.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="value">The value to create the constant operand for.</param>
		/// <returns>
		/// A new operand representing the value <paramref name="value" />.
		/// </returns>
		public static Operand CreateConstant(TypeSystem typeSystem, int value)
		{
			return new Operand(typeSystem.BuiltIn.I4)
			{
				IsConstant = true,
				ConstantSignedLongInteger = value,
				IsResolved = true
			};
		}

		/// <summary>
		/// Creates a new constant <see cref="Operand" /> for the given integral value.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="value">The value to create the constant operand for.</param>
		/// <returns>
		/// A new operand representing the value <paramref name="value" />.
		/// </returns>
		public static Operand CreateConstant(TypeSystem typeSystem, long value)
		{
			return new Operand(typeSystem.BuiltIn.I8)
			{
				IsConstant = true,
				ConstantSignedLongInteger = value,
				IsResolved = true
			};
		}

		/// <summary>
		/// Creates a new constant <see cref="Operand"/> for the given integral value.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="value">The value to create the constant operand for.</param>
		/// <returns>
		/// A new operand representing the value <paramref name="value"/>.
		/// </returns>
		public static Operand CreateConstant(TypeSystem typeSystem, float value)
		{
			return new Operand(typeSystem.BuiltIn.R4)
			{
				IsConstant = true,
				ConstantSingleFloatingPoint = value,
				IsResolved = true
			};
		}

		/// <summary>
		/// Creates a new constant <see cref="Operand"/> for the given integral value.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="value">The value to create the constant operand for.</param>
		/// <returns>
		/// A new operand representing the value <paramref name="value"/>.
		/// </returns>
		public static Operand CreateConstant(TypeSystem typeSystem, double value)
		{
			return new Operand(typeSystem.BuiltIn.R8)
			{
				IsConstant = true,
				ConstantDoubleFloatingPoint = value,
				IsResolved = true
			};
		}

		/// <summary>
		/// Creates a new physical register <see cref="Operand" />.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="register">The register.</param>
		/// <returns></returns>
		public static Operand CreateCPURegister(MosaType type, Register register)
		{
			return new Operand(type)
			{
				IsCPURegister = true,
				Register = register
			};
		}

		/// <summary>
		/// Creates a new runtime member <see cref="Operand"/>.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public static Operand CreateField(MosaField field)
		{
			Debug.Assert(field.IsStatic);

			return new Operand(field.FieldType)
			{
				IsStaticField = true,
				Offset = 0,
				Field = field,
				IsConstant = true
			};
		}

		/// <summary>
		/// Creates the low 32 bit portion of a 64-bit <see cref="Operand" />.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="longOperand">The long operand.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public static Operand CreateLowSplitForLong(TypeSystem typeSystem, Operand longOperand, int index)
		{
			Debug.Assert(longOperand.IsLong);
			Debug.Assert(longOperand.SplitParent == null);
			Debug.Assert(longOperand.Low == null);

			Operand operand = null;

			if (longOperand.IsParameter)
			{
				operand = CreateStackParameter(longOperand.Type, longOperand.Index, longOperand.Name + " (Low)", false);
				operand.Offset = longOperand.Offset;
				operand.IsResolved = true;
				operand.ConstantUnsignedLongInteger = longOperand.ConstantUnsignedLongInteger;
			}
			else if (longOperand.IsResolvedConstant)
			{
				operand = new Operand(typeSystem.BuiltIn.U4)
				{
					IsConstant = true,
					IsResolved = true,
					ConstantUnsignedLongInteger = longOperand.ConstantUnsignedLongInteger & uint.MaxValue
				};
			}
			else if (longOperand.IsVirtualRegister)
			{
				operand = new Operand(typeSystem.BuiltIn.U4)
				{
					IsVirtualRegister = true
				};
			}
			else if (longOperand.IsStackLocal)
			{
				operand = longOperand;
			}

			Debug.Assert(operand != null);

			operand.SplitParent = longOperand;

			if (!longOperand.IsStackLocal)
			{
				operand.Index = index;
			}

			longOperand.Low = operand;

			return operand;
		}

		/// <summary>
		/// Creates the high 32 bit portion of a 64-bit <see cref="Operand" />.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="longOperand">The long operand.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public static Operand CreateHighSplitForLong(TypeSystem typeSystem, Operand longOperand, int index)
		{
			Debug.Assert(longOperand.IsLong);
			Debug.Assert(longOperand.SplitParent == null || longOperand.SplitParent == longOperand);
			Debug.Assert(longOperand.High == null);

			Operand operand = null;

			if (longOperand.IsParameter)
			{
				operand = CreateStackParameter(longOperand.Type, longOperand.Index, longOperand.Name + " (High)", false);
				operand.Offset = longOperand.Offset + 4;
				operand.IsResolved = true;
				operand.ConstantUnsignedLongInteger = longOperand.ConstantUnsignedLongInteger + 4;
			}
			else if (longOperand.IsResolvedConstant)
			{
				operand = new Operand(typeSystem.BuiltIn.U4)
				{
					IsConstant = true,
					IsResolved = true,
					ConstantUnsignedLongInteger = ((uint)(longOperand.ConstantUnsignedLongInteger >> 32)) & uint.MaxValue
				};
			}
			else if (longOperand.IsVirtualRegister)
			{
				operand = new Operand(typeSystem.BuiltIn.U4)
				{
					IsVirtualRegister = true
				};
			}
			else if (longOperand.IsStackLocal)
			{
				operand = new Operand(typeSystem.BuiltIn.U4)
				{
					IsConstant = true,
					IsResolved = false
				};
			}

			Debug.Assert(operand != null);

			operand.SplitParent = longOperand;

			if (!longOperand.IsStackLocal)
			{
				operand.Index = index;
			}

			longOperand.High = operand;

			return operand;
		}

		/// <summary>
		/// Creates a new symbol <see cref="Operand" /> for the given symbol name.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="label">The label.</param>
		/// <returns></returns>
		public static Operand CreateLabel(MosaType type, string label)
		{
			var operand = new Operand(type)
			{
				IsLabel = true,
				Name = label,
				Offset = 0,
				IsConstant = true
			};
			return operand;
		}

		/// <summary>
		/// Creates the symbol.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static Operand CreateSymbol(MosaType type, string name)
		{
			var operand = new Operand(type)
			{
				IsSymbol = true,
				Name = name,
				IsConstant = true
			};
			return operand;
		}

		/// <summary>
		/// Creates the shifter.
		/// </summary>
		/// <param name="shiftType">Type of the shift.</param>
		/// <returns></returns>
		public static Operand CreateShifter(ShiftType shiftType)
		{
			var operand = new Operand(shiftType);
			return operand;
		}

		/// <summary>
		/// Creates the SSA <see cref="Operand"/>.
		/// </summary>
		/// <param name="ssa">The ssa operand.</param>
		/// <param name="version">The ssa version.</param>
		/// <returns></returns>
		public static Operand CreateSSA(Operand ssa, int version)
		{
			Debug.Assert(ssa.IsVirtualRegister);
			Debug.Assert(!ssa.IsConstant);
			Debug.Assert(!ssa.IsParameter);
			Debug.Assert(!ssa.IsStackLocal);
			Debug.Assert(!ssa.IsCPURegister);
			Debug.Assert(!ssa.IsLabel);
			Debug.Assert(!ssa.IsSymbol);
			Debug.Assert(!ssa.IsStaticField);

			return new Operand(ssa.Type)
			{
				IsVirtualRegister = true,
				IsSSA = true,
				SSAParent = ssa,
				SSAVersion = version
			};
		}

		/// <summary>
		/// Creates the stack local.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="index">The index.</param>
		/// <param name="pinned">if set to <c>true</c> [pinned].</param>
		/// <returns></returns>
		public static Operand CreateStackLocal(MosaType type, int index, bool pinned)
		{
			return new Operand(type)
			{
				Index = index,
				IsStackLocal = true,
				IsConstant = true,
				IsPinned = pinned
			};
		}

		/// <summary>
		/// Creates the stack parameter.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="index">The index.</param>
		/// <param name="name">The name.</param>
		/// <param name="isThis">if set to <c>true</c> [is this].</param>
		/// <returns></returns>
		public static Operand CreateStackParameter(MosaType type, int index, string name, bool isThis)
		{
			Debug.Assert(!isThis || (isThis && index == 0));

			return new Operand(type)
			{
				IsParameter = true,
				Index = index,
				IsConstant = true,
				Name = name,
				IsThis = isThis
			};
		}

		/// <summary>
		/// Creates the string symbol with data.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="name">The name.</param>
		/// <param name="data">The string data.</param>
		/// <returns></returns>
		public static Operand CreateStringSymbol(TypeSystem typeSystem, string name, string data)
		{
			Debug.Assert(data != null);

			return new Operand(typeSystem.BuiltIn.String)
			{
				IsSymbol = true,
				Name = name,
				StringData = data,
				IsConstant = true
			};
		}

		/// <summary>
		/// Creates a new symbol <see cref="Operand" /> for the given symbol name.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public static Operand CreateSymbolFromMethod(TypeSystem typeSystem, MosaMethod method)
		{
			var operand = CreateUnmanagedSymbolPointer(typeSystem, method.FullName);
			operand.Method = method;
			operand.IsConstant = true;
			return operand;
		}

		/// <summary>
		/// Creates the symbol.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static Operand CreateUnmanagedSymbolPointer(TypeSystem typeSystem, string name)
		{
			return new Operand(typeSystem.BuiltIn.Pointer)
			{
				IsSymbol = true,
				Name = name,
				IsConstant = true
			};
		}

		/// <summary>
		/// Creates a new virtual register <see cref="Operand" />.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public static Operand CreateVirtualRegister(MosaType type, int index)
		{
			return new Operand(type)
			{
				IsVirtualRegister = true,
				Index = index
			};
		}

		/// <summary>
		/// Gets the null constant <see cref="Operand" />.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <returns></returns>
		public static Operand GetNull(TypeSystem typeSystem)
		{
			return new Operand(typeSystem.BuiltIn.Object)
			{
				IsNull = true,
				IsConstant = true,
				IsResolved = true
			};
		}

		/// <summary>
		/// Gets the null constant <see cref="Operand" />.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static Operand GetNull(MosaType type)
		{
			return new Operand(type)
			{
				IsNull = true,
				IsConstant = true,
				IsResolved = true
			};
		}

		/// <summary>
		/// Creates the symbol.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private static Operand CreateManagedSymbolPointer(MosaType type, string name)
		{
			// NOTE: Not being used
			return new Operand(type.ToManagedPointer())
			{
				IsSymbol = true,
				Name = name,
				IsConstant = true
			};
		}

		#endregion Static Factory Constructors

		/// <summary>
		/// Returns a string representation of <see cref="Operand" />.
		/// </summary>
		/// <param name="full">if set to <c>true</c> [full].</param>
		/// <returns>
		/// A string representation of the operand.
		/// </returns>
		public string ToString(bool full)
		{
			if (IsSSA)
			{
				string ssa = SSAParent.ToString(full);
				int pos = ssa.IndexOf(' ');

				if (pos < 0)
					return ssa + "<" + SSAVersion + ">";
				else
					return ssa.Substring(0, pos) + "<" + SSAVersion + ">" + ssa.Substring(pos);
			}

			var sb = new StringBuilder();

			if (IsVirtualRegister)
			{
				sb.AppendFormat("V_{0}", Index);

				if (IsSplitChild)
				{
					sb.AppendFormat(" <V_{0}_{1}>", SplitParent.Index, (SplitParent.High == this) ? "High" : "Low");
				}
			}
			else if (IsParameter)
			{
				sb.AppendFormat("P_{0} ", Index);
			}
			else if (IsStackLocal && Name == null)
			{
				sb.AppendFormat("T_{0}", Index);
			}
			else if (IsStaticField)
			{
				sb.Append(" <");
				sb.Append(Field.FullName);
				sb.Append("> ");
			}
			else if (Name != null)
			{
				sb.Append(" <");
				sb.Append(Name);
				sb.Append("> ");
			}

			if (IsConstant)
			{
				sb.Append(" const=");

				if (!IsResolved)
				{
					sb.Append("unresolved");
				}
				else if (IsNull)
				{
					sb.Append("null");
				}
				else if (IsOnStack)
				{
					sb.AppendFormat("{0}", ConstantSignedLongInteger);
				}
				else if (IsUnsigned || IsBoolean || IsChar || IsPointer)
				{
					if (IsU8)
						sb.AppendFormat("{0}", ConstantUnsignedLongInteger);
					else
						sb.AppendFormat("{0}", ConstantUnsignedInteger);
				}
				else if (IsSigned)
				{
					if (IsI8)
						sb.AppendFormat("{0}", ConstantSignedLongInteger);
					else
						sb.AppendFormat("{0}", ConstantSignedInteger);
				}
				else if (IsR8)
				{
					sb.AppendFormat("{0}", ConstantDoubleFloatingPoint);
				}
				else if (IsR4)
				{
					sb.AppendFormat("{0}", ConstantSingleFloatingPoint);
				}

				sb.Append(' ');
			}
			else if (IsCPURegister)
			{
				sb.AppendFormat(" {0}", Register);
			}

			if (full)
			{
				sb.AppendFormat(" [{0}]", Type.FullName);
			}
			else
			{
				if (Type.IsReferenceType)
				{
					sb.AppendFormat(" [O]");
				}
				else
				{
					sb.AppendFormat(" [{0}]", ShortenTypeName(Type.FullName));
				}
			}

			return sb.ToString().Replace("  ", " ").Trim();
		}

		private static string ShortenTypeName(string value)
		{
			if (value.Length < 2)
				return value;

			string type = value;
			string end = string.Empty;

			if (value.EndsWith("*"))
			{
				type = value.Substring(0, value.Length - 1);
				end = "*";
			}
			if (value.EndsWith("&"))
			{
				type = value.Substring(0, value.Length - 1);
				end = "&";
			}
			if (value.EndsWith("[]"))
			{
				type = value.Substring(0, value.Length - 2);
				end = "[]";
			}

			return ShortenTypeName2(type) + end;
		}

		private static string ShortenTypeName2(string value)
		{
			switch (value)
			{
				case "System.Object": return "O";
				case "System.Char": return "C";
				case "System.Void": return "V";
				case "System.String": return "String";
				case "System.Byte": return "U1";
				case "System.SByte": return "I1";
				case "System.Boolean": return "B";
				case "System.Int8": return "I1";
				case "System.UInt8": return "U1";
				case "System.Int16": return "I2";
				case "System.UInt16": return "U2";
				case "System.Int32": return "I4";
				case "System.UInt32": return "U4";
				case "System.Int64": return "I8";
				case "System.UInt64": return "U8";
				case "System.Single": return "R4";
				case "System.Double": return "R8";
			}

			return value;
		}

		internal void RenameIndex(int index)
		{
			Debug.Assert(IsVirtualRegister);
			Debug.Assert(!IsStackLocal);

			Index = index;
		}

		#region Object Overrides

		public override string ToString()
		{
			return ToString(false);
		}

		#endregion Object Overrides
	}
}
