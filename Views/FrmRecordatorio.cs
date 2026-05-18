using AutoCita.Models;
using AutoCita.ViewModels;

namespace AutoCita.Views
{
    /// <summary>
    /// Formulario de recordatorios de llamadas para citas del día siguiente (RN-05).
    /// Muestra el listado de citas programadas para mañana cuyo recordatorio aún no fue realizado.
    /// </summary>
    public partial class FrmRecordatorio : Form
    {
        private readonly RecordatorioViewModel _vm;

        private DataGridView _grid      = new();
        private Label        _lblInfo   = new();
        private Label        _lblError  = new();
        private Label        _lblExito  = new();
        private Button       _btnRecargar = new();

        /// <summary>
        /// Constructor del formulario de recordatorios.
        /// </summary>
        /// <param name="vm">ViewModel de recordatorios.</param>
        public FrmRecordatorio(RecordatorioViewModel vm)
        {
            _vm = vm;
            _vm.PropertyChanged += (_, e) => ActualizarUi(e.PropertyName);
            InicializarComponentes();
            _ = _vm.CargarAsync();
        }

        private void InicializarComponentes()
        {
            Text          = "Recordatorios de Llamadas";
            Size          = new Size(900, 540);
            StartPosition = FormStartPosition.CenterParent;
            BackColor     = Color.FromArgb(245, 245, 250);
            Font          = new Font("Segoe UI", 9.5f);

            var lblTitulo = new Label
            {
                Text     = "Llamadas de recordatorio — mañana",
                Font     = new Font("Segoe UI", 13f, FontStyle.Bold),
                Location = new Point(20, 18),
                AutoSize = true
            };

            var lblSub = new Label
            {
                Text      = "Clientes con cita mañana que aún no han recibido llamada de confirmación.",
                ForeColor = Color.Gray,
                Location  = new Point(20, 48),
                AutoSize  = true
            };

            _lblInfo.Location  = new Point(20, 72);
            _lblInfo.AutoSize  = true;
            _lblInfo.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            _lblInfo.ForeColor = Color.FromArgb(30, 100, 200);

            _btnRecargar.Text      = "⟳  Recargar";
            _btnRecargar.Location  = new Point(760, 15);
            _btnRecargar.Size      = new Size(110, 34);
            _btnRecargar.FlatStyle = FlatStyle.Flat;
            _btnRecargar.FlatAppearance.BorderSize = 0;
            _btnRecargar.Anchor    = AnchorStyles.Top | AnchorStyles.Right;
            _btnRecargar.Click    += async (_, _) => await _vm.CargarAsync();

            _grid.Location        = new Point(20, 100);
            _grid.Size            = new Size(850, 350);
            _grid.Anchor          = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _grid.ReadOnly        = true;
            _grid.SelectionMode   = DataGridViewSelectionMode.FullRowSelect;
            _grid.AllowUserToAddRows    = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.BackgroundColor       = Color.White;
            _grid.BorderStyle           = BorderStyle.None;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 100, 200);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            _grid.ColumnHeadersHeight   = 34;
            _grid.CellClick += GridCellClick;

            _lblError.Location  = new Point(20, 462);
            _lblError.Size      = new Size(850, 24);
            _lblError.ForeColor = Color.Red;
            _lblError.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _lblExito.Location  = new Point(20, 462);
            _lblExito.Size      = new Size(850, 24);
            _lblExito.ForeColor = Color.FromArgb(30, 150, 30);
            _lblExito.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Controls.AddRange([lblTitulo, lblSub, _lblInfo, _btnRecargar, _grid, _lblError, _lblExito]);
        }

        private void RefrescarGrid()
        {
            _grid.Rows.Clear();
            if (_grid.Columns.Count == 0)
            {
                _grid.Columns.Add("FechaHora", "Fecha y Hora");
                _grid.Columns.Add("Cliente",   "Cliente");
                _grid.Columns.Add("Telefono",  "Teléfono");
                _grid.Columns.Add("Vehiculo",  "Vehículo");
                _grid.Columns.Add("Sede",      "Sede");
                _grid.Columns.Add("Motivo",    "Motivo");
                var colMarcar = new DataGridViewButtonColumn
                {
                    Name       = "Marcar",
                    HeaderText = "",
                    Text       = "✔ Llamada realizada",
                    UseColumnTextForButtonValue = true,
                    FillWeight  = 18
                };
                _grid.Columns.Add(colMarcar);
            }

            foreach (var c in _vm.Citas)
            {
                _grid.Rows.Add(
                    c.FechaHora.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                    c.Vehiculo?.Cliente?.NombreCompleto ?? "—",
                    c.Vehiculo?.Cliente?.Telefono ?? "—",
                    c.Vehiculo != null ? $"{c.Vehiculo.Placa} ({c.Vehiculo.TipoVehiculo})" : "—",
                    c.Sede?.Nombre ?? "—",
                    c.Motivo
                );
            }
            _grid.Tag = _vm.Citas;
            _lblInfo.Text = _vm.TotalPendientes > 0
                ? $"Hay {_vm.TotalPendientes} llamada(s) pendiente(s) de realizar."
                : "No hay llamadas pendientes para mañana. ✔";
        }

        private async void GridCellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (_grid.Columns[e.ColumnIndex].Name != "Marcar") return;
            var citas = _grid.Tag as List<Cita>;
            if (citas == null || e.RowIndex >= citas.Count) return;
            await _vm.MarcarLlamadaRealizadaAsync(citas[e.RowIndex].Id);
        }

        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }
            if (propiedad == nameof(RecordatorioViewModel.Citas)) RefrescarGrid();
            if (propiedad == nameof(RecordatorioViewModel.MensajeError))
            { _lblError.Text = _vm.MensajeError; _lblExito.Text = string.Empty; }
            if (propiedad == nameof(RecordatorioViewModel.MensajeExito))
            { _lblExito.Text = _vm.MensajeExito; _lblError.Text = string.Empty; }
        }
    }
}
