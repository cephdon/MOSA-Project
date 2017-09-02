// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Tool.GDBDebugger.GDB;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Mosa.Tool.GDBDebugger.View
{
	public partial class WatchView : DebugDockContent
	{
		private BindingList<WatchEntry> watches = new BindingList<WatchEntry>();

		private class WatchEntry
		{
			public string Address { get { return "0x" + Watch.Address.ToString((Watch.Address <= uint.MaxValue) ? "X4" : "X8"); } }

			public string Name { get { return Watch.Name; } }

			[Browsable(false)]
			public bool Signed { get { return Watch.Signed; } }

			public string HexValue { get; set; }

			public ulong Value { get; set; }

			public uint Size { get { return Watch.Size; } }

			[Browsable(false)]
			public Watch Watch;

			public WatchEntry(Watch watch)
			{
				Watch = watch;
			}
		}

		public WatchView(MainForm mainForm)
			: base(mainForm)
		{
			InitializeComponent();
			dataGridView1.DataSource = watches;
			dataGridView1.AutoResizeColumns();
			dataGridView1.Columns[0].Width = 65;
			dataGridView1.Columns[1].Width = 250;
			cbLength.SelectedIndex = 2;
		}

		public override void OnPause()
		{
			foreach (var watch in watches)
			{
				watch.Value = 0;
				watch.HexValue = string.Empty;

				MemoryCache.ReadMemory(watch.Watch.Address, watch.Size, OnMemoryRead);
			}

			Refresh();
		}

		public override void OnWatchChange()
		{
			watches.Clear();
			foreach (var watch in MainForm.Watchs)
			{
				watches.Add(new WatchEntry(watch));
			}

			OnPause();
		}

		private void OnMemoryRead(ulong address, byte[] bytes)
		{
			MethodInvoker method = delegate ()
			{
				UpdateDisplay(address, bytes);
			};

			BeginInvoke(method);
		}

		private void UpdateDisplay(ulong address, byte[] bytes)
		{
			foreach (var watch in watches)
			{
				if (watch.Watch.Address != address || watch.Size != bytes.Length)
					continue;

				watch.Value = ToLong(bytes);
				watch.HexValue = BasePlatform.ToHex(watch.Value, watch.Size);
			}

			Refresh();
		}

		private static ulong ToLong(byte[] bytes)
		{
			ulong value = 0;

			for (int i = 0; i < bytes.Length; i++)
			{
				ulong shifted = (ulong)(bytes[i] << (i * 8));
				value |= shifted;
			}

			return value;
		}

		public void AddWatch(string name, ulong address, uint size, bool signed)
		{
			var watch = new Watch(name, address, size, signed);
			var watchEntry = new WatchEntry(watch);

			watches.Add(watchEntry);
		}

		private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Delete)
				return;

			if (dataGridView1.CurrentCell == null)
				return;

			var watchEntry = dataGridView1.CurrentCell.OwningRow.DataBoundItem as WatchEntry;

			MainForm.RemoveWatch(watchEntry.Watch);
		}

		private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
				return;

			if (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;

			dataGridView1.ClearSelection();
			dataGridView1.Rows[e.RowIndex].Selected = true;
			var relativeMousePosition = dataGridView1.PointToClient(Cursor.Position);

			var clickedEntry = dataGridView1.Rows[e.RowIndex].DataBoundItem as WatchEntry;

			var menu = new MenuItem(clickedEntry.HexValue + " - " + clickedEntry.Name);
			menu.Enabled = false;
			var m = new ContextMenu();
			m.MenuItems.Add(menu);
			m.MenuItems.Add(new MenuItem("Copy to &Clipboard", new EventHandler(MainForm.OnCopyToClipboard)) { Tag = clickedEntry.Address });
			m.MenuItems.Add(new MenuItem("&Delete watch", new EventHandler(MainForm.OnRemoveWatch)) { Tag = clickedEntry.Watch });

			m.Show(dataGridView1, relativeMousePosition);
		}

		private void btnDeleteAll_Click(object sender, EventArgs e)
		{
			MainForm.RemoveAllWatches();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			var address = MainForm.ParseMemoryAddress(tbAddress.Text);

			uint size = 0;

			switch (cbLength.SelectedIndex)
			{
				case 0: size = 1; break;
				case 1: size = 2; break;
				case 2: size = 4; break;
				case 3: size = 8; break;
				default: size = 4; break;
			}

			MainForm.AddWatch(null, address, size);
		}
	}
}
