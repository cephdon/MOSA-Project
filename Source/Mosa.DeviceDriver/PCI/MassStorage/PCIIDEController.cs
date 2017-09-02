﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

// References
// http://www.t13.org/Documents/UploadedDocuments/docs2004/d1572r3-EDD3.pdf
// http://www.osdever.net/tutorials/lba.php
// http://www.nondot.org/sabre/os/files/Disk/IDE-tech.html

using Mosa.ClassLib;
using Mosa.DeviceSystem;
using Mosa.HardwareSystem;
using Mosa.HardwareSystem.PCI;

namespace Mosa.DeviceDriver.PCI.MassStorage
{
	/// <summary>
	///
	/// </summary>
	//[PCIDeviceDriver(VendorID = 0x8086, DeviceID = 0x7010, Platforms = PlatformArchitecture.X86AndX64)]
	//, ProgIF = 0x80
	//[PCIDeviceDriver(ClassCode = 0x01, SubClassCode = 0x01, Platforms = PlatformArchitecture.X86AndX64)]
	public class PCIIDEController : HardwareDevice, IDiskControllerDevice
	{
		#region Definitions

		/// <summary>
		///
		/// </summary>
		internal struct IDECommands
		{
			internal const byte ReadSectorsWithRetry = 0x20;
			internal const byte WriteSectorsWithRetry = 0x30;
			internal const byte IdentifyDrive = 0xEC;
		}

		/// <summary>
		///
		/// </summary>
		internal struct IdentifyDrive
		{
			internal const uint GeneralConfig = 0x00;
			internal const uint LogicalCylinders = 0x02;
			internal const uint LogicalHeads = 0x08;
			internal const uint LogicalSectors = 0x06 * 2;
			internal const uint SerialNbr = 0x14;
			internal const uint ControllerType = 0x28;
			internal const uint BufferSize = 0x15 * 2;
			internal const uint FirmwareRevision = 0x17 * 2;
			internal const uint ModelNumber = 0x1B * 2;
			internal const uint SupportDoubleWord = 0x30 * 2;

			internal const uint CommandSetSupported83 = 83 * 2; // 1 word
			internal const uint MaxLBA28 = 60 * 2; // 2 words
			internal const uint MaxLBA48 = 100 * 2; // 3 words
		}

		#endregion Definitions

		/// <summary>
		///
		/// </summary>
		protected SpinLock spinLock;

		/// <summary>
		/// The drives per conroller
		/// </summary>
		public const uint DrivesPerConroller = 2; // the maximum supported

		/// <summary>
		/// The data port
		/// </summary>
		protected IReadWriteIOPort DataPort;

		/// <summary>
		/// The feature port
		/// </summary>
		protected IReadWriteIOPort FeaturePort;

		/// <summary>
		/// The error port
		/// </summary>
		protected IReadOnlyIOPort ErrorPort;

		/// <summary>
		/// The sector count port
		/// </summary>
		protected IReadWriteIOPort SectorCountPort;

		/// <summary>
		/// The lba low port
		/// </summary>
		protected IReadWriteIOPort LBALowPort;

		/// <summary>
		/// The lba mid port
		/// </summary>
		protected IReadWriteIOPort LBAMidPort;

		/// <summary>
		/// The lba high port
		/// </summary>
		protected IReadWriteIOPort LBAHighPort;

		/// <summary>
		/// The device head port
		/// </summary>
		protected IReadWriteIOPort DeviceHeadPort;

		/// <summary>
		/// The status port
		/// </summary>
		protected IReadOnlyIOPort StatusPort;

		/// <summary>
		/// The command port
		/// </summary>
		protected IWriteOnlyIOPort CommandPort;

		//protected IRQHandler IdeIRQ;

		public enum LBAType { LBA28, LBA48 }

		/// <summary>
		///
		/// </summary>
		protected struct DriveInfo
		{
			/// <summary>
			/// The present
			/// </summary>
			public bool Present;

			/// <summary>
			/// The maximum lba
			/// </summary>
			public uint MaxLBA;

			/// <summary>
			/// The lba type
			/// </summary>
			public LBAType LBAType;
		}

		/// <summary>
		/// The drive information
		/// </summary>
		protected DriveInfo[] driveInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="PCIIDEController"/> class.
		/// </summary>
		public PCIIDEController()
		{
			driveInfo = new DriveInfo[DrivesPerConroller];
		}

		public override bool PreSetup(IPCIDeviceResource pciDeviceResource)
		{
			//pciDeviceResource.CommandRegister;
			return true;
		}

		/// <summary>
		/// Setups this hardware device driver
		/// </summary>
		/// <param name="hardwareResources"></param>
		/// <returns></returns>
		public override bool Setup(HardwareResources hardwareResources)
		{
			this.HardwareResources = hardwareResources;
			base.Name = "PCI_IDE_0x" + base.HardwareResources.GetIOPort(0, 0).Address.ToString("X");

			DataPort = base.HardwareResources.GetIOPort(0, 0);
			ErrorPort = base.HardwareResources.GetIOPort(0, 1);
			FeaturePort = base.HardwareResources.GetIOPort(0, 1);
			SectorCountPort = base.HardwareResources.GetIOPort(0, 2);
			LBALowPort = base.HardwareResources.GetIOPort(0, 3);
			LBAMidPort = base.HardwareResources.GetIOPort(0, 4);
			LBAHighPort = base.HardwareResources.GetIOPort(0, 5);
			DeviceHeadPort = base.HardwareResources.GetIOPort(0, 6);
			CommandPort = base.HardwareResources.GetIOPort(0, 7);
			StatusPort = base.HardwareResources.GetIOPort(0, 7);

			for (int drive = 0; drive < DrivesPerConroller; drive++)
			{
				driveInfo[drive].Present = false;
				driveInfo[drive].MaxLBA = 0;
			}

			base.DeviceStatus = DeviceStatus.Online;
			return true;
		}

		/// <summary>
		/// Probes this instance.
		/// </summary>
		/// <returns></returns>
		public override bool Probe()
		{
			LBALowPort.Write8(0x88);

			if (LBALowPort.Read8() != 0x88)
				return false;

			return true;
		}

		/// <summary>
		/// Starts this hardware device.
		/// </summary>
		/// <returns></returns>
		public override DeviceDriverStartStatus Start()
		{
			DeviceHeadPort.Write8(0xA0);

			HAL.Sleep(1000 / 250); // wait 1/250th of a second

			if ((StatusPort.Read8() & 0x40) == 0x40)
				driveInfo[0].Present = true;

			DeviceHeadPort.Write8(0xB0);

			HAL.Sleep(1000 / 250); // wait 1/250th of a second

			if ((StatusPort.Read8() & 0x40) == 0x40)
				driveInfo[1].Present = true;

			return DeviceDriverStartStatus.Started;
		}

		/// <summary>
		/// Called when an interrupt is received.
		/// </summary>
		/// <returns></returns>
		public override bool OnInterrupt()
		{
			return true;
		}

		/// <summary>
		/// Waits for register ready.
		/// </summary>
		/// <returns></returns>
		protected bool WaitForRegisterReady()
		{
			while (true)
			{
				uint status = StatusPort.Read8();

				if ((status & 0x08) == 0x08)
					return true;

				//TODO: add timeout check
			}

			//return false;
		}

		/// <summary>
		///
		/// </summary>
		protected enum SectorOperation
		{
			/// <summary>
			///
			/// </summary>
			Read,

			/// <summary>
			///
			/// </summary>
			Write
		}

		/// <summary>
		/// Performs the LBA28.
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <param name="drive">The drive NBR.</param>
		/// <param name="lba">The lba.</param>
		/// <param name="data">The data.</param>
		/// <param name="offset">The offset.</param>
		/// <returns></returns>
		protected bool PerformLBA28(SectorOperation operation, uint drive, uint lba, byte[] data, uint offset)
		{
			if (drive > MaximunDriveCount)
				return false;

			FeaturePort.Write8(0);
			SectorCountPort.Write8(1);

			LBALowPort.Write8((byte)(lba & 0xFF));
			LBAMidPort.Write8((byte)((lba >> 8) & 0xFF));
			LBAHighPort.Write8((byte)((lba >> 16) & 0xFF));

			DeviceHeadPort.Write8((byte)(0xE0 | (drive << 4) | ((lba >> 24) & 0x0F)));

			if (operation == SectorOperation.Write)
				CommandPort.Write8(IDECommands.WriteSectorsWithRetry);
			else
				CommandPort.Write8(IDECommands.ReadSectorsWithRetry);

			if (!WaitForRegisterReady())
				return false;

			var sector = new DataBlock(data);

			//TODO: Don't use PIO
			if (operation == SectorOperation.Read)
			{
				for (uint index = 0; index < 256; index++)
					sector.SetUShort(offset + (index * 2), DataPort.Read16());
			}
			else
			{
				for (uint index = 0; index < 256; index++)
					DataPort.Write16(sector.GetUShort(offset + (index * 2)));
			}

			return true;
		}

		/// <summary>
		/// Reads the LBA48.
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <param name="drive">The drive.</param>
		/// <param name="lba">The lba.</param>
		/// <param name="data">The data.</param>
		/// <param name="offset">The offset.</param>
		/// <returns></returns>
		protected bool ReadLBA48(SectorOperation operation, uint drive, uint lba, byte[] data, uint offset)
		{
			if (drive > MaximunDriveCount)
				return false;

			FeaturePort.Write8(0);
			FeaturePort.Write8(0);

			SectorCountPort.Write8(0);
			SectorCountPort.Write8(1);

			LBALowPort.Write8((byte)((lba >> 24) & 0xFF));
			LBALowPort.Write8((byte)(lba & 0xFF));

			LBAMidPort.Write8((byte)((lba >> 32) & 0xFF));
			LBAMidPort.Write8((byte)((lba >> 8) & 0xFF));

			LBAHighPort.Write8((byte)((lba >> 40) & 0xFF));
			LBAHighPort.Write8((byte)((lba >> 16) & 0xFF));

			DeviceHeadPort.Write8((byte)(0x40 | (drive << 4)));

			if (operation == SectorOperation.Write)
				CommandPort.Write8(0x34);
			else
				CommandPort.Write8(0x24);

			if (!WaitForRegisterReady())
				return false;

			var sector = new DataBlock(data);

			//TODO: Don't use PIO
			if (operation == SectorOperation.Read)
			{
				for (uint index = 0; index < 256; index++)
					sector.SetUShort(offset + (index * 2), DataPort.Read16());
			}
			else
			{
				for (uint index = 0; index < 256; index++)
					DataPort.Write16(sector.GetUShort(offset + (index * 2)));
			}

			return true;
		}

		/// <summary>
		/// Opens the specified drive NBR.
		/// </summary>
		/// <param name="drive">The drive NBR.</param>
		/// <returns></returns>
		public bool Open(uint drive)
		{
			if (drive > MaximunDriveCount)
				return false;

			if (!driveInfo[drive].Present)
				return false;

			if (drive == 0)
				DeviceHeadPort.Write8(0xA0);
			else
				if (drive == 1)
				DeviceHeadPort.Write8(0xB0);
			else
				return false;

			CommandPort.Write8(IDECommands.IdentifyDrive);

			if (!WaitForRegisterReady())
				return false;

			var info = new DataBlock(512);

			for (uint index = 0; index < 256; index++)
				info.SetUShort(index * 2, DataPort.Read16());

			driveInfo[drive].MaxLBA = info.GetUInt(IdentifyDrive.MaxLBA28);

			return true;
		}

		/// <summary>
		/// Releases the specified drive NBR.
		/// </summary>
		/// <param name="drive">The drive NBR.</param>
		/// <returns></returns>
		public bool Release(uint drive)
		{
			return true;
		}

		/// <summary>
		/// Gets the maximum drive count.
		/// </summary>
		/// <value>The drive count.</value>
		public uint MaximunDriveCount { get { return 2; } }

		/// <summary>
		/// Gets the size of the sector.
		/// </summary>
		/// <param name="drive">The drive NBR.</param>
		/// <returns></returns>
		public uint GetSectorSize(uint drive)
		{
			return 512;
		}

		/// <summary>
		/// Gets the total sectors.
		/// </summary>
		/// <param name="drive">The drive NBR.</param>
		/// <returns></returns>
		public uint GetTotalSectors(uint drive)
		{
			if (drive > MaximunDriveCount)
				return 0;

			return driveInfo[drive].MaxLBA;
		}

		/// <summary>
		/// Determines whether this instance can write to the specified drive.
		/// </summary>
		/// <param name="drive">The drive NBR.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can write to the specified drive; otherwise, <c>false</c>.
		/// </returns>
		public bool CanWrite(uint drive)
		{
			return true;
		}

		/// <summary>
		/// Reads the block.
		/// </summary>
		/// <param name="drive">The drive NBR.</param>
		/// <param name="block">The block.</param>
		/// <param name="count">The count.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public bool ReadBlock(uint drive, uint block, uint count, byte[] data)
		{
			if (drive > MaximunDriveCount)
				return false;

			if (data.Length < count * 512)
				return false;

			try
			{
				spinLock.Enter();
				for (uint index = 0; index < count; index++)
				{
					if (!PerformLBA28(SectorOperation.Read, drive, block + index, data, index * 512))
						return false;
				}
				return true;
			}
			finally
			{
				spinLock.Exit();
			}
		}

		/// <summary>
		/// Writes the block.
		/// </summary>
		/// <param name="drive">The drive NBR.</param>
		/// <param name="block">The block.</param>
		/// <param name="count">The count.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public bool WriteBlock(uint drive, uint block, uint count, byte[] data)
		{
			if (drive > MaximunDriveCount)
				return false;

			if (data.Length < count * 512)
				return false;

			try
			{
				spinLock.Enter();
				for (uint index = 0; index < count; index++)
				{
					if (!PerformLBA28(SectorOperation.Write, drive, block + index, data, index * 512))
						return false;
				}
				return true;
			}
			finally
			{
				spinLock.Exit();
			}
		}
	}
}
