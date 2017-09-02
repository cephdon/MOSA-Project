// Copyright (c) MOSA Project. Licensed under the New BSD License.

using SharpDisasm;
using System.Windows.Forms;

namespace Mosa.Tool.GDBDebugger.View
{
	public partial class StatusView : DebugDockContent
	{
		public StatusView(MainForm mainForm)
			: base(mainForm)
		{
			InitializeComponent();
		}

		public override void OnRunning()
		{
			tbInstruction.Text = "Running...";
			tbIP.Text = string.Empty;
			tbMethod.Text = string.Empty;
		}

		public override void OnPause()
		{
			if (Platform == null)
				return;

			if (Platform.Registers == null)
				return;

			tbIP.Text = Platform.InstructionPointer.ToHex();
			tbInstruction.Text = string.Empty;

			MemoryCache.ReadMemory(Platform.InstructionPointer.Value, 16, OnMemoryRead);

			var symbol = DebugSource.GetFirstSymbol(Platform.InstructionPointer.Value);

			tbMethod.Text = symbol == null ? string.Empty : symbol.CommonName;
		}

		private void OnMemoryRead(ulong address, byte[] bytes)
		{
			MethodInvoker method = delegate ()
			{
				UpdateDisplay(address, bytes);
			};

			BeginInvoke(method);
		}

		private void UpdateDisplay(ulong address, byte[] memory)
		{
			if (address != Platform.InstructionPointer.Value)
				return;

			var mode = ArchitectureMode.x86_32; // todo:

			try
			{
				using (var disasm = new Disassembler(memory, mode, address, true))
				{
					var translator = new SharpDisasm.Translators.IntelTranslator()
					{
						IncludeAddress = false,
						IncludeBinary = false
					};

					foreach (var instruction in disasm.Disassemble())
					{
						var asm = translator.Translate(instruction);
						tbInstruction.Text = asm;
						break;
					}
				}
			}
			catch
			{
				tbInstruction.Text = "Unable to decode!";
			}
		}
	}
}
