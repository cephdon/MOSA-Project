﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using SharpDisasm;
using SharpDisasm.Translators;
using System;
using System.IO;

namespace Mosa.Tool.Disassembler.Intel
{
	internal static class Program
	{
		private static int Main(string[] args)
		{
			Console.WriteLine();
			Console.WriteLine("Mosa.Tool.Disassembler.Intel.Disassembler [www.mosa-project.org]");
			Console.WriteLine("Copyright 2016. New BSD License.");
			Console.WriteLine("Written by Phil Garcia (phil@thinkedge.com)");
			Console.WriteLine();

			if (args.Length < 2)
			{
				Console.WriteLine("Usage: Disassembler -offset <offset> -address <address> -o <output file> <source file>");
				Console.Error.WriteLine("ERROR: Missing arguments");
				return -1;
			}

			try
			{
				var options = new Options();
				options.LoadArguments(args);

				// Need a new instance of translator every time as they aren't thread safe
				var translator = new IntelTranslator()
				{
					// Configure the translator to output instruction addresses and instruction binary as hex
					IncludeAddress = true,
					IncludeBinary = true
				};

				var code2 = File.ReadAllBytes(options.InputFile);

				var code = new byte[code2.Length];

				for (ulong i = options.FileOffset; i < (ulong)code2.Length; i++)
				{
					code[i - options.FileOffset] = code2[i];
				}

				//using (var disasm = new SharpDisasm.Disassembler(code, ArchitectureMode.x86_32, options.StartingAddress, true, Vendor.Any, options.FileOffset))
				using (var disasm = new SharpDisasm.Disassembler(code, ArchitectureMode.x86_32, options.StartingAddress, true, Vendor.Any))
				{
					using (var dest = File.CreateText(options.OutputFile))
					{
						foreach (var instruction in disasm.Disassemble())
						{
							var inst = translator.Translate(instruction);
							dest.WriteLine(inst);

							if (options.Length != 0 && instruction.PC > options.StartingAddress + options.Length)
								break;
						}
					}
				}

				return 0;
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("Exception: {0}", e.ToString());
				return -1;
			}
		}
	}
}
