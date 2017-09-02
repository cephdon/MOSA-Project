﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

// 82C54 CHMOS Programmable Interval Timer Datasheet
// http://bochs.sourceforge.net/techspec/intel-82c54-timer.pdf.gz

using Mosa.HardwareSystem;

namespace Mosa.DeviceDriver.ISA
{
	/// <summary>
	/// Programmable Interval Timer (PIT) Device Driver
	/// </summary>
	//[ISADeviceDriver(AutoLoad = true, BasePort = 0x40, PortRange = 4, IRQ = 0, Platforms = PlatformArchitecture.X86AndX64)]
	public class PIT : HardwareDevice, IDevice, IHardwareDevice
	{
		#region Definitions

		/// <summary>
		///
		/// </summary>
		private const byte SquareWave = 0x36;

		/// <summary>
		///
		/// </summary>
		private const uint Frequency = 1193182;

		/// <summary>
		///
		/// </summary>
		private const ushort Hz = 100;

		#endregion Definitions

		/// <summary>
		///
		/// </summary>
		protected IReadWriteIOPort modeControlPort;

		/// <summary>
		///
		/// </summary>
		protected IReadWriteIOPort counter0Divisor;

		/// <summary>
		///
		/// </summary>
		protected uint tickCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="PIT"/> class.
		/// </summary>
		public PIT()
		{
		}

		/// <summary>
		/// Setups this hardware device driver
		/// </summary>
		/// <returns></returns>
		public override bool Setup(HardwareResources hardwareResources)
		{
			this.HardwareResources = hardwareResources;
			base.Name = "PIT_0x" + base.HardwareResources.GetIOPort(0, 0).Address.ToString("X");

			modeControlPort = base.HardwareResources.GetIOPort(0, 3);
			counter0Divisor = base.HardwareResources.GetIOPort(0, 0);

			return true;
		}

		/// <summary>
		/// Starts this hardware device.
		/// </summary>
		/// <returns></returns>
		public override DeviceDriverStartStatus Start()
		{
			ushort timerCount = (ushort)(Frequency / Hz);

			// Set to Mode 3 - Square Wave Generator
			modeControlPort.Write8(SquareWave);
			counter0Divisor.Write8((byte)(timerCount & 0xFF));
			counter0Divisor.Write8((byte)((timerCount & 0xFF00) >> 8));

			tickCount = 0;

			base.DeviceStatus = DeviceStatus.Online;
			return DeviceDriverStartStatus.Started;
		}

		/// <summary>
		/// Called when an interrupt is received.
		/// </summary>
		/// <returns></returns>
		public override bool OnInterrupt()
		{
			tickCount++;
			return true;
		}

		/// <summary>
		/// Gets the tick count.
		/// </summary>
		/// <returns></returns>
		public uint GetTickCount()
		{
			return tickCount;
		}
	}
}
