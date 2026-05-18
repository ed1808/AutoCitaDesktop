using AutoCita.Models;
using AutoCita.ViewModels;

namespace AutoCita.Views
{
    /// <summary>
    /// Formulario de generación de reportes del sistema.
    /// Permite seleccionar tipo, rango de fechas y sede, luego muestra resultados en un grid.
    /// </summary>
    public partial class FrmReporte : Form
    {
        private readonly ReporteViewModel _vm;

        private ComboBox         _cboTipo   = new();
        private DateTimePicker   _dtpDesde  = new();
        private DateTimePicker   _dtpHasta  = new();
        private ComboBox         _cboSede   = new();
        private Button           _btnGenerar = new();
        private DataGridView     _grid       = new();
        private Label            _lblTitulo  = new();
        private Label            _lblError   = new();

        /// <summary>
        /// Constructor del formulario de reportes.
        /// </summary>
        /// <param name="vm">ViewModel de reportes.</param>
        public FrmReporte(ReporteViewModel vm)
        {
            _vm = vm;
            _vm.PropertyChanged += (_, e) => ActualizarUi(e.PropertyName);
            InicializarComponentes();
            _ = InicializarAsync();
        }

        private async Task InicializarAsync()
        {
            await _vm.CargarSedesAsync();
            if (InvokeRequired) Invoke(CargarSedesEnCombo);
            else CargarSedesEnCombo();
        }

        private void InicializarComponentes()
        {
            Text          = "Reportes";
            Size          = new Size(1000, 640);
            StartPosition = FormStartPosition.CenterParent;
            BackColor     = Color.FromArgb(245, 245, 250);
            Font          = new Font("Segoe UI", 9.5f);

            var lblHeader = new Label
            {
                Text     = "Generación de Reportes",
                Font     = new Font("Segoe UI", 13f, FontStyle.Bold),
                Location = new Point(20, 18),
                AutoSize = true
            };

            // ── Tipo de reporte ──────────────────────────────────────────────
            var lblTipo = new Label { Text = "Tipo de reporte:", Location = new Point(20, 65), AutoSize = true };
            _cboTipo.Location      = new Point(20, 85);
            _cboTipo.Size          = new Size(300, 28);
            _cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboTipo.DataSource    = _vm.TiposReporte;
            _cboTipo.SelectedIndexChanged += (_, _) => _vm.TipoReporteIndice = _cboTipo.SelectedIndex;

            // ── Fechas ───────────────────────────────────────────────────────
            var lblDesde = new Label { Text = "Desde:", Location = new Point(340, 65), AutoSize = true };
            _dtpDesde.Location    = new Point(340, 85);
            _dtpDesde.Size        = new Size(160, 28);
            _dtpDesde.Format      = DateTimePickerFormat.Short;
            _dtpDesde.Value       = _vm.FechaDesde;
            _dtpDesde.ValueChanged += (_, _) => _vm.FechaDesde = _dtpDesde.Value;

            var lblHasta = new Label { Text = "Hasta:", Location = new Point(520, 65), AutoSize = true };
            _dtpHasta.Location    = new Point(520, 85);
            _dtpHasta.Size        = new Size(160, 28);
            _dtpHasta.Format      = DateTimePickerFormat.Short;
            _dtpHasta.Value       = _vm.FechaHasta;
            _dtpHasta.ValueChanged += (_, _) => _vm.FechaHasta = _dtpHasta.Value;

            // ── Sede (filtro opcional) ────────────────────────────────────────
            var lblSede = new Label { Text = "Sede (opcional):", Location = new Point(700, 65), AutoSize = true };
            _cboSede.Location      = new Point(700, 85);
            _cboSede.Size          = new Size(200, 28);
            _cboSede.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboSede.DisplayMember = nameof(Sede.Nombre);
            _cboSede.ValueMember   = nameof(Sede.Id);
            _cboSede.SelectedIndexChanged += (_, _) =>
            {
                if (_cboSede.SelectedItem is Sede s && s.Id != 0)
                    _vm.SedeId = s.Id;
                else
                    _vm.SedeId = null;
            };

            // ── Botón generar ─────────────────────────────────────────────────
            _btnGenerar.Text      = "Generar reporte";
            _btnGenerar.Location  = new Point(20, 130);
            _btnGenerar.Size      = new Size(170, 38);
            _btnGenerar.BackColor = Color.FromArgb(30, 100, 200);
            _btnGenerar.ForeColor = Color.White;
            _btnGenerar.FlatStyle = FlatStyle.Flat;
            _btnGenerar.FlatAppearance.BorderSize = 0;
            _btnGenerar.Click    += async (_, _) => await _vm.GenerarAsync();

            // ── Título del resultado ──────────────────────────────────────────
            _lblTitulo.Location  = new Point(20, 182);
            _lblTitulo.AutoSize  = true;
            _lblTitulo.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
            _lblTitulo.ForeColor = Color.FromArgb(30, 100, 200);

            // ── Grid de resultados ────────────────────────────────────────────
            _grid.Location        = new Point(20, 208);
            _grid.Size            = new Size(950, 360);
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

            _lblError.Location  = new Point(20, 580);
            _lblError.Size      = new Size(950, 24);
            _lblError.ForeColor = Color.Red;
            _lblError.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _lblError.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Controls.AddRange([
                lblHeader,
                lblTipo, _cboTipo,
                lblDesde, _dtpDesde, lblHasta, _dtpHasta,
                lblSede, _cboSede,
                _btnGenerar, _lblTitulo, _grid, _lblError
            ]);
        }

        private void CargarSedesEnCombo()
        {
            var sedes = new List<Sede> { new Sede { Id = 0, Nombre = "(Todas)" } };
            sedes.AddRange(_vm.Sedes);
            _cboSede.DataSource    = sedes;
            _cboSede.DisplayMember = nameof(Sede.Nombre);
            _cboSede.ValueMember   = nameof(Sede.Id);
        }

        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }

            if (propiedad == nameof(ReporteViewModel.MensajeError))
                _lblError.Text = _vm.MensajeError;

            if (propiedad == nameof(ReporteViewModel.TituloReporte))
                _lblTitulo.Text = _vm.TituloReporte;

            if (propiedad == nameof(ReporteViewModel.Resultados))
                PoblarGrid();

            if (propiedad == nameof(ReporteViewModel.Cargando))
                _btnGenerar.Enabled = !_vm.Cargando;
        }

        private void PoblarGrid()
        {
            _grid.Rows.Clear();
            _grid.Columns.Clear();
            _lblError.Text = string.Empty;

            if (_vm.Resultados.Count == 0)
            {
                _lblTitulo.Text += " (sin resultados)";
                return;
            }

            // Construir columnas dinámicas a partir de las propiedades del primer objeto
            var primero = _vm.Resultados[0];
            var propiedades = primero.GetType().GetProperties();
            foreach (var p in propiedades)
                _grid.Columns.Add(p.Name, FormatearNombreColumna(p.Name));

            foreach (var item in _vm.Resultados)
            {
                var valores = propiedades.Select(p => p.GetValue(item)?.ToString() ?? string.Empty).ToArray<object>();
                _grid.Rows.Add(valores);
            }
        }

        private static string FormatearNombreColumna(string nombre)
        {
            // Convierte "NombreAgente" → "Nombre Agente"
            var resultado = System.Text.RegularExpressions.Regex.Replace(nombre, "([A-Z])", " $1").Trim();
            return char.ToUpper(resultado[0]) + resultado[1..];
        }
    }
}
