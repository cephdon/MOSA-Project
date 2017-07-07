// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Linker;
using System.IO;

namespace Mosa.Compiler.Framework.Stages
{
	/// <summary>
	/// An compilation stage, which generates a map file of the built binary file.
	/// </summary>
	public sealed class DebugFileGenerationStage : BaseCompilerStage
	{
		#region Data members

		public string DebugFile { get; set; }

		/// <summary>
		/// Holds the text writer used to emit the map file.
		/// </summary>
		private TextWriter writer;

		#endregion Data members

		protected override void Setup()
		{
			DebugFile = CompilerOptions.DebugFile;
		}

		protected override void RunPostCompile()
		{
			if (string.IsNullOrEmpty(DebugFile))
				return;

			using (writer = new StreamWriter(DebugFile))
			{
				EmitSections();
				EmitSymbols();
				EmitEntryPoint();
				EmitTypes();
				EmitMethods();
				EmitParameters();
			}
		}

		#region Internals

		private void EmitSections()
		{
			writer.WriteLine("[Sections]");
			writer.WriteLine("Address\tOffset\tSize\tKind\tName");
			foreach (var section in Linker.LinkerSections)
			{
				writer.WriteLine(
					"{0:x8}\t{1}\t{2}\t{3}\t{4}",
					section.VirtualAddress,
					section.FileOffset,
					section.Size,
					section.SectionKind.ToString(),
					section.Name);
			}
		}

		private void EmitSymbols()
		{
			writer.WriteLine("[Symbols]");
			writer.WriteLine("Address\tOffset\tSize\tType\tName");

			foreach (var symbol in Linker.Symbols)
			{
				writer.WriteLine(
					"{0:x8}\t{1}\t{2}\t{3}\t{4}",
					symbol.VirtualAddress,
					symbol.SectionOffset,
					symbol.Size,
					symbol.SectionKind.ToString(),
					symbol.Name);
			}
		}

		private void EmitEntryPoint()
		{
			if (Linker.EntryPoint != null)
			{
				writer.WriteLine("[EntryPoint]");
				writer.WriteLine("Address\tName");
				writer.WriteLine(
					"{0:x8}\t{1}",
					Linker.EntryPoint.VirtualAddress,
					Linker.EntryPoint.Name);
			}
		}

		private void EmitTypes()
		{
			writer.WriteLine("[Types]");
			writer.WriteLine("TypeDef\tSize\tFullName\tParent Type\tDeclaring Type\tElement Type");
			foreach (var type in TypeSystem.AllTypes)
			{
				if (type.IsModule)
					continue;

				writer.WriteLine(
					"{0:x8}\t{1}\t{2}\t{3}\t{4}\t{5}",
					Linker.GetSymbol(type.FullName + Metadata.TypeDefinition, SectionKind.ROData).VirtualAddress,
					TypeLayout.GetTypeSize(type),
					type.FullName,
					(type.BaseType != null) ? type.BaseType.FullName : string.Empty,
					(type.DeclaringType != null) ? type.DeclaringType.FullName : string.Empty,
					(type.ElementType != null) ? type.ElementType.FullName : string.Empty
				);
			}
		}

		private void EmitMethods()
		{
			writer.WriteLine("[Methods]");
			writer.WriteLine("Address\tSize\tMethodDef\tFullName\tType\tReturnType\tStackSize\tParameterStackSize\tAttributes");

			foreach (var type in TypeSystem.AllTypes)
			{
				if (type.IsModule)
					continue;

				foreach (var method in type.Methods)
				{
					var symbol = Linker.GetSymbol(method.FullName, SectionKind.Text);

					writer.WriteLine(
						"{0:x8}\t{1}\t{2:x8}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}",
						symbol.VirtualAddress,
						symbol.Size,
						Linker.GetSymbol(method.FullName + Metadata.MethodDefinition, SectionKind.ROData).VirtualAddress,
						method.FullName,
						type.FullName,
						method.Signature.ReturnType.FullName,
						TypeLayout.GetMethodStackSize(method),
						TypeLayout.GetMethodParameterStackSize(method),
						(int)method.MethodAttributes
					);
				}
			}
		}

		private void EmitParameters()
		{
			writer.WriteLine("[Parameters]");
			writer.WriteLine("Index\tOffset\tFullName\tName\tType\tMethod\tAttributes");

			foreach (var type in TypeSystem.AllTypes)
			{
				if (type.IsModule)
					continue;

				foreach (var method in type.Methods)
				{
					int index = 0;

					foreach (var parameter in method.Signature.Parameters)
					{
						writer.WriteLine(
							"{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
							index++,
							0,  // todo: offset to parameter
							parameter.FullName,
							parameter.Name,
							parameter.ParameterType.FullName,
							method.FullName,
							(int)parameter.ParameterAttributes
						);
					}
				}
			}
		}

		#endregion Internals
	}
}