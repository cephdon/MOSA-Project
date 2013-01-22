﻿/*
 * (c) 2012 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mosa.Compiler.Common;

namespace Mosa.Compiler.Framework
{

	/// <summary>
	/// 
	/// </summary>
	public sealed class OperandVisitor
	{
		private Context context;

		public OperandVisitor(Context context)
		{
			this.context = context;
		}

		public IEnumerable<Operand> Input
		{
			get
			{
				foreach (Operand operand in context.Operands)
				{
					if (operand.IsVirtualRegister || operand.IsCPURegister)
					{
						yield return operand;
					}
					else if (operand.IsMemoryAddress)
					{
						if (operand.BaseOperand != null)
						{
							yield return operand.BaseOperand;
						}

						if (operand.OffsetBase != null)
						{
							yield return operand.OffsetBase;
						}
					}
				}

				foreach (Operand operand in context.Results)
				{
					if (operand.IsMemoryAddress)
					{
						if (operand.IsVirtualRegister || operand.IsCPURegister)
						{
							yield return operand;
						}

						if (operand.BaseOperand != null)
						{
							yield return operand.BaseOperand;
						}

						if (operand.OffsetBase != null)
						{
							yield return operand.OffsetBase;
						}
					}
				}

			}
		}

		public IEnumerable<Operand> Output
		{
			get
			{
				foreach (Operand operand in context.Results)
				{
					if (operand.IsVirtualRegister || operand.IsCPURegister)
					{
						yield return operand;
					}
				}
			}
		}

		public IEnumerable<Operand> Temp
		{
			get
			{
				// TODO: add scratch registers (register which are not in Operand.Results)
				yield break;
			}
		}

	}

}

