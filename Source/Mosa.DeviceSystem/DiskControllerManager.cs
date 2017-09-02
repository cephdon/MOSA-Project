// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.HardwareSystem;
using System.Collections.Generic;

namespace Mosa.DeviceSystem
{
	/// <summary>
	///
	/// </summary>
	public class DiskControllerManager
	{
		/// <summary>
		/// The device manager
		/// </summary>
		protected DeviceManager deviceManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="DiskControllerManager"/> class.
		/// </summary>
		/// <param name="deviceManager">The device manager.</param>
		public DiskControllerManager(DeviceManager deviceManager)
		{
			this.deviceManager = deviceManager;
		}

		/// <summary>
		/// Creates the devices.
		/// </summary>
		/// <param name="diskControllerDevice">The disk controller device.</param>
		/// <returns></returns>
		private List<IDevice> CreateDevices(IDiskControllerDevice diskControllerDevice)
		{
			var devices = new List<IDevice>();

			for (uint drive = 0; drive < diskControllerDevice.MaximunDriveCount; drive++)
			{
				if (diskControllerDevice.Open(drive))
				{
					if (diskControllerDevice.GetTotalSectors(drive) == 0)
						continue;

					var diskDevice = new DiskDevice(diskControllerDevice, drive, false);
					devices.Add(diskDevice as IDevice);
				}
			}

			return devices;
		}

		/// <summary>
		/// Creates the disk devices.
		/// </summary>
		public void CreateDiskDevices()
		{
			// FIXME: Do not create disk devices if this method executed more than once

			// Find disk controller devices
			var controllers = deviceManager.GetDevices(new IsDiskControllerDevice(), new IsOnline());

			// For each controller
			foreach (var device in controllers)
			{
				var controller = device as IDiskControllerDevice;

				// Create disk devices
				var disks = CreateDevices(controller);

				// Add them to the device manager
				foreach (var disk in disks)
				{
					deviceManager.Add(disk);
				}
			}
		}
	}
}
