// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.MosaTypeSystem;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mosa.Compiler.Framework
{
	/// <summary>
	/// Virtual Registers
	/// </summary>
	public sealed class VirtualRegisters : IEnumerable<Operand>
	{
		#region Data members

		private readonly List<Operand> virtualRegisters = new List<Operand>();

		#endregion Data members

		#region Properties

		public int Count { get { return virtualRegisters.Count; } }

		public Operand this[int index] { get { return virtualRegisters[index]; } }

		#endregion Properties

		/// <summary>
		/// Initializes a new instance of the <see cref="VirtualRegisters" /> class.
		/// </summary>
		public VirtualRegisters()
		{
		}

		/// <summary>
		/// Allocates the virtual register.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public Operand Allocate(MosaType type)
		{
			int index = virtualRegisters.Count + 1;

			Operand virtualRegister = Operand.CreateVirtualRegister(type, index);

			virtualRegisters.Add(virtualRegister);

			return virtualRegister;
		}

		public void SplitLongOperand(TypeSystem typeSystem, Operand longOperand)
		{
			Debug.Assert(longOperand.Is64BitInteger);

			if (longOperand.Low == null && longOperand.High == null)
			{
				var low = Operand.CreateLowSplitForLong(typeSystem, longOperand, virtualRegisters.Count + 1);
				var high = Operand.CreateHighSplitForLong(typeSystem, longOperand, virtualRegisters.Count + 1);

				if (longOperand.IsVirtualRegister)
				{
					virtualRegisters.Add(low);
					virtualRegisters.Add(high);
				}
			}
		}

		public IEnumerator<Operand> GetEnumerator()
		{
			foreach (var virtualRegister in virtualRegisters)
			{
				yield return virtualRegister;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal void ReOrdered(Operand virtualRegister, int index)
		{
			virtualRegisters[index - 1] = virtualRegister;
			virtualRegister.RenameIndex(index);
		}
	}
}
