﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections.Generic;

namespace Mosa.HardwareSystem
{
	/// <summary>
	/// The Interrupt Manager dispatches interrupts to the appropriate hardware device drivers
	/// </summary>
	public class InterruptManager
	{
		/// <summary>
		/// The maximum interrupts
		/// </summary>
		public const ushort MaxInterrupts = 16;

		/// <summary>
		/// The interrupt handlers
		/// </summary>
		protected List<IHardwareDevice>[] interruptHandlers;

		/// <summary>
		/// The spin lock
		/// </summary>
		protected SpinLock spinLock;

		/// <summary>
		/// Initializes a new instance of the <see cref="InterruptManager"/> class.
		/// </summary>
		public InterruptManager()
		{
			interruptHandlers = new List<IHardwareDevice>[MaxInterrupts];

			for (int i = 0; i < MaxInterrupts; i++)
			{
				interruptHandlers[i] = new List<IHardwareDevice>();
			}
		}

		/// <summary>
		/// Processes the interrupt.
		/// </summary>
		/// <param name="irq">The irq.</param>
		public void ProcessInterrupt(byte irq)
		{
			try
			{
				spinLock.Enter();

				foreach (var hardwareDevice in interruptHandlers[irq])
				{
					hardwareDevice.OnInterrupt();
				}
			}
			finally
			{
				spinLock.Exit();
			}
		}

		/// <summary>
		/// Adds the interrupt handler.
		/// </summary>
		/// <param name="irq">The irq.</param>
		/// <param name="hardwareDevice">The hardware device.</param>
		public void AddInterruptHandler(byte irq, IHardwareDevice hardwareDevice)
		{
			if (irq >= MaxInterrupts)
				return;

			try
			{
				spinLock.Enter();
				interruptHandlers[irq].Add(hardwareDevice);
			}
			finally
			{
				spinLock.Exit();
			}
		}

		/// <summary>
		/// Releases the interrupt handler.
		/// </summary>
		/// <param name="irq">The irq.</param>
		/// <param name="hardwareDevice">The hardware device.</param>
		public void ReleaseInterruptHandler(byte irq, IHardwareDevice hardwareDevice)
		{
			if (irq >= MaxInterrupts)
				return;

			try
			{
				spinLock.Enter();
				interruptHandlers[irq].Remove(hardwareDevice);
			}
			finally
			{
				spinLock.Exit();
			}
		}
	}
}
