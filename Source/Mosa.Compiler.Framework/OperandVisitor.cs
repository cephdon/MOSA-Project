﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections.Generic;

namespace Mosa.Compiler.Framework
{
	public struct OperandVisitor
	{
		private readonly InstructionNode node;

		public OperandVisitor(InstructionNode node)
		{
			this.node = node;
		}

		public IEnumerable<Operand> Input
		{
			get
			{
				foreach (var operand in node.Operands)
				{
					if (operand.IsVirtualRegister || operand.IsCPURegister)
					{
						yield return operand;
					}
				}
			}
		}

		public IEnumerable<Operand> Output
		{
			get
			{
				foreach (var operand in node.Results)
				{
					if (operand.IsVirtualRegister || operand.IsCPURegister)
					{
						yield return operand;
					}
				}
			}
		}
	}
}
