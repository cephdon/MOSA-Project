﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework
{
	/// <summary>
	/// Pipeline Stage Interface
	/// </summary>
	public interface IPipelineStage
	{
		/// <summary>
		/// Retrieves the name of the compilation stage.
		/// </summary>
		/// <value>The name of the compilation stage.</value>
		string Name { get; }
	}
}
