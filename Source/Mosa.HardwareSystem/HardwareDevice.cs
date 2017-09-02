﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.HardwareSystem.PCI;

namespace Mosa.HardwareSystem
{
	/// <summary>
	/// Abstract class for hardware devices
	/// </summary>
	public abstract class HardwareDevice : Device, IHardwareDevice
	{
		/// <summary>
		/// Gets or sets the hardware resources.
		/// </summary>
		protected HardwareResources HardwareResources;

		/// <summary>
		/// Initializes a new instance of the <see cref="HardwareDevice"/> class.
		/// </summary>
		public HardwareDevice()
		{
			base.DeviceStatus = DeviceStatus.Initializing;
		}

		/// <summary>
		/// Pres the setup.
		/// </summary>
		/// <param name="pciDeviceResource">The pci device resource.</param>
		/// <returns></returns>
		public virtual bool PreSetup(IPCIDeviceResource pciDeviceResource) { return true; }

		/// <summary>
		/// Setups this hardware device driver
		/// </summary>
		/// <param name="hardwareResources"></param>
		/// <returns></returns>
		public abstract bool Setup(HardwareResources hardwareResources);

		/// <summary>
		/// Probes this instance.
		/// </summary>
		/// <remarks>Overide for ISA devices, if example</remarks>
		/// <returns></returns>
		public virtual bool Probe() { return true; }

		/// <summary>
		/// Starts this hardware device.
		/// </summary>
		/// <returns></returns>
		public abstract DeviceDriverStartStatus Start();

		/// <summary>
		/// Stops this hardware device.
		/// </summary>
		/// <returns></returns>
		public bool Stop()
		{
			return false;
		}

		/// <summary>
		/// Called when an interrupt is received.
		/// </summary>
		/// <returns></returns>
		public abstract bool OnInterrupt();
	}
}
