// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.HardwareSystem
{
	/// <summary>
	///
	/// </summary>
	public class PCIControllerManager
	{
		/// <summary>
		///
		/// </summary>
		protected DeviceManager deviceManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="PCIControllerManager"/> class.
		/// </summary>
		/// <param name="deviceManager">The device manager.</param>
		public PCIControllerManager(DeviceManager deviceManager)
		{
			this.deviceManager = deviceManager;
		}

		/// <summary>
		/// Probes for a PCI device.
		/// </summary>
		/// <param name="pciController">The pci controller.</param>
		/// <param name="bus">The bus.</param>
		/// <param name="slot">The slot.</param>
		/// <param name="fun">The fun.</param>
		/// <returns></returns>
		protected bool ProbeDevice(IPCIController pciController, byte bus, byte slot, byte fun)
		{
			uint value = pciController.ReadConfig32(bus, slot, fun, 0);

			return value != 0xFFFFFFFF;
		}

		/// <summary>
		/// Creates the partition devices.
		/// </summary>
		public void CreatePCIDevices()
		{
			// Find PCI controller devices
			var devices = deviceManager.GetDevices(new IsPCIController(), new IsOnline());

			if (devices.Count == 0)
				return;

			var pciController = devices[0] as IPCIController;

			// For each controller
			for (int bus = 0; bus < 255; bus++)
			{
				for (int slot = 0; slot < 16; slot++)
				{
					for (int fun = 0; fun < 7; fun++)
					{
						if (ProbeDevice(pciController, (byte)bus, (byte)slot, (byte)fun))
						{
							var pciDevice = new PCI.PCIDevice(pciController, (byte)bus, (byte)slot, (byte)fun);
							deviceManager.Add(pciDevice);
						}
					}
				}
			}
		}
	}
}
