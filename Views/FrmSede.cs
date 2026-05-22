using AutoCita.Models;
using AutoCita.ViewModels;
using System.Drawing.Drawing2D;

namespace AutoCita.Views
{
    public partial class FrmSede : Form
    {
        private readonly SedeViewModel _vm;

        private DataGridView _grid = new();
        private TextBox _txtNombre = new();
        private TextBox _txtDireccion = new();
        private Label _lblError = new();
        private Label _lblExito = new();
        private Button _btnNuevo = new();
        private Button _btnGuardar = new();
        private Button _btnCancelar = new();
        private Panel _pnlForm = new();
        private Form? _tooltipForm;

        private readonly ToolTip _toolTip = new()
        {
            InitialDelay = 300,
            ShowAlways = true,
            AutoPopDelay = 4000,
            ReshowDelay = 100
        };

        public FrmSede(SedeViewModel vm)
        {
            _vm = vm;
            _vm.PropertyChanged += (_, e) => ActualizarUi(e.PropertyName);
            InicializarComponentes();
            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            await _vm.CargarAsync();
            if (InvokeRequired) Invoke(RefrescarGrid);
            else RefrescarGrid();
        }

        private void InicializarComponentes()
        {
            Text = "Gestión de Sedes";
            Size = new Size(760, 500);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 245, 250);
            Font = new Font("Segoe UI", 9.5f);

            var lblTitulo = new Label
            {
                Text = "Sedes del sistema",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                Location = new Point(20, 18),
                AutoSize = true
            };

            _btnNuevo.Text = "+ Nueva sede";
            _btnNuevo.Location = new Point(580, 15);
            _btnNuevo.Size = new Size(150, 34);
            _btnNuevo.BackColor = Color.FromArgb(30, 100, 200);
            _btnNuevo.ForeColor = Color.White;
            _btnNuevo.FlatStyle = FlatStyle.Flat;
            _btnNuevo.FlatAppearance.BorderSize = 0;
            _btnNuevo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnNuevo.Click += (_, _) => { _vm.NuevaSede(); MostrarFormulario(true); };
            _toolTip.SetToolTip(_btnNuevo, "Agregar una nueva sede");

            _grid.Location = new Point(20, 65);
            _grid.Size = new Size(710, 230);
            _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _grid.ReadOnly = true;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.BackgroundColor = Color.White;
            _grid.BorderStyle = BorderStyle.None;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 100, 200);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 34;
            _grid.ShowCellToolTips = true;

            _grid.CellClick += GridCellClick;
            _grid.CellPainting += GridCellPainting;
            _grid.CellMouseEnter += GridCellMouseEnter;
            _grid.CellMouseLeave += GridCellMouseLeave;

            _pnlForm.Location = new Point(20, 310);
            _pnlForm.Size = new Size(710, 160);
            _pnlForm.BackColor = Color.White;
            _pnlForm.Padding = new Padding(12);
            _pnlForm.Visible = false;

            var lblFTitulo = new Label { Text = "Datos de la sede", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(12, 12), AutoSize = true };
            var lblNombre = new Label { Text = "Nombre:", Location = new Point(12, 40), AutoSize = true };
            _txtNombre.Location = new Point(12, 60);
            _txtNombre.Size = new Size(260, 28);
            _txtNombre.TextChanged += (_, _) => _vm.Nombre = _txtNombre.Text;

            var lblDir = new Label { Text = "Dirección:", Location = new Point(290, 40), AutoSize = true };
            _txtDireccion.Location = new Point(290, 60);
            _txtDireccion.Size = new Size(400, 28);
            _txtDireccion.TextChanged += (_, _) => _vm.Direccion = _txtDireccion.Text;

            _btnGuardar.Text = "Guardar";
            _btnGuardar.Location = new Point(12, 110);
            _btnGuardar.Size = new Size(120, 36);
            _btnGuardar.BackColor = Color.FromArgb(30, 150, 80);
            _btnGuardar.ForeColor = Color.White;
            _btnGuardar.FlatStyle = FlatStyle.Flat;
            _btnGuardar.FlatAppearance.BorderSize = 0;
            _btnGuardar.Click += async (_, _) => await _vm.GuardarAsync();
            _toolTip.SetToolTip(_btnGuardar, "Guardar cambios de la sede");

            _btnCancelar.Text = "Cancelar";
            _btnCancelar.Location = new Point(144, 110);
            _btnCancelar.Size = new Size(100, 36);
            _btnCancelar.FlatStyle = FlatStyle.Flat;
            _btnCancelar.FlatAppearance.BorderSize = 0;
            _btnCancelar.Click += (_, _) => { _vm.NuevaSede(); MostrarFormulario(false); };
            _toolTip.SetToolTip(_btnCancelar, "Cancelar y cerrar el formulario");

            _pnlForm.Controls.AddRange([lblFTitulo, lblNombre, _txtNombre, lblDir, _txtDireccion, _btnGuardar, _btnCancelar]);

            _lblError.Location = new Point(20, 480);
            _lblError.Size = new Size(710, 24);
            _lblError.ForeColor = Color.Red;
            _lblError.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _lblExito.Location = new Point(20, 480);
            _lblExito.Size = new Size(710, 24);
            _lblExito.ForeColor = Color.FromArgb(30, 150, 30);
            _lblExito.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            Controls.AddRange([lblTitulo, _btnNuevo, _grid, _pnlForm, _lblError, _lblExito]);
        }

        private void GridCellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var col = _grid.Columns[e.ColumnIndex].Name;
            if (col != "Editar" && col != "Desactivar") return;

            e.PaintBackground(e.ClipBounds, true);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = e.CellBounds.X + e.CellBounds.Width / 2;
            int cy = e.CellBounds.Y + e.CellBounds.Height / 2;
            int r = 13;

            using var bgBrush = new SolidBrush(Color.FromArgb(30, 100, 200));
            g.FillEllipse(bgBrush, cx - r, cy - r, r * 2, r * 2);
            using var borderPen = new Pen(Color.White, 1.2f);
            g.DrawEllipse(borderPen, cx - r, cy - r, r * 2, r * 2);

            using var iconPen = new Pen(Color.White, 1.6f) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round };
            using var iconBrush = new SolidBrush(Color.White);

            if (col == "Editar")
            {
                var cuerpo = new PointF[]
                {
                    new(cx - 6f, cy + 2f),
                    new(cx - 2f, cy + 6f),
                    new(cx + 6f, cy - 2f),
                    new(cx + 2f, cy - 6f),
                };
                g.FillPolygon(iconBrush, cuerpo);
                g.DrawPolygon(iconPen, cuerpo);

                using var gomaPen = new Pen(Color.FromArgb(180, 210, 255), 2f);
                g.DrawLine(gomaPen, cx + 1f, cy - 6f, cx + 5f, cy - 2f);

                var punta = new PointF[]
                {
                    new(cx - 6f, cy + 2f),
                    new(cx - 2f, cy + 6f),
                    new(cx - 9f, cy + 8f),
                };
                using var puntaBrush = new SolidBrush(Color.FromArgb(200, 220, 255));
                g.FillPolygon(puntaBrush, punta);
                g.DrawPolygon(iconPen, punta);

                using var mineBrush = new SolidBrush(Color.FromArgb(150, 180, 230));
                g.FillEllipse(mineBrush, cx - 10f, cy + 8f, 2.5f, 2.5f);
            }
            else
            {
                g.DrawArc(iconPen, cx - 5, cy - 10, 10, 10, 180, 180);
                using var candadoPen = new Pen(Color.White, 1.8f);
                g.DrawRectangle(candadoPen, cx - 6, cy - 2, 12, 9);
                g.DrawEllipse(iconPen, cx - 2, cy + 0.5f, 4, 4);
                g.DrawLine(iconPen, cx, cy + 4.5f, cx, cy + 7);
            }

            e.Handled = true;
        }

        private void GridCellMouseEnter(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var col = _grid.Columns[e.ColumnIndex].Name;
            if (col != "Editar" && col != "Desactivar") return;

            _grid.Cursor = Cursors.Hand;

            _tooltipForm?.Close();
            _tooltipForm?.Dispose();
            _tooltipForm = null;

            var texto = col == "Editar" ? "Editar" : "Eliminar";

            _tooltipForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                BackColor = Color.FromArgb(45, 45, 48),
                Size = new Size(60, 24),
                ShowInTaskbar = false,
                TopMost = true
            };

            var lbl = new Label
            {
                Text = texto,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            _tooltipForm.Controls.Add(lbl);

            var rect = _grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            var punto = _grid.PointToScreen(new Point(rect.X + rect.Width / 2 - 30, rect.Bottom + 4));
            _tooltipForm.Location = punto;
            _tooltipForm.Show();
        }

        private void GridCellMouseLeave(object? sender, DataGridViewCellEventArgs e)
        {
            _grid.Cursor = Cursors.Default;
            _tooltipForm?.Close();
            _tooltipForm?.Dispose();
            _tooltipForm = null;
        }

        private void RefrescarGrid()
        {
            _grid.Rows.Clear();
            if (_grid.Columns.Count < 4)
            {
                _grid.Columns.Clear();
                _grid.Columns.Add("Nombre", "Nombre");
                _grid.Columns.Add("Direccion", "Dirección");

                var colEditar = new DataGridViewImageColumn
                {
                    Name = "Editar",
                    HeaderText = "",
                    Image = new Bitmap(1, 1),
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 40,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Resizable = DataGridViewTriState.False
                };
                var colDesact = new DataGridViewImageColumn
                {
                    Name = "Desactivar",
                    HeaderText = "",
                    Image = new Bitmap(1, 1),
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 40,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Resizable = DataGridViewTriState.False
                };
                _grid.Columns.AddRange(colEditar, colDesact);
            }
            foreach (var s in _vm.Sedes)
                _grid.Rows.Add(s.Nombre, s.Direccion);
            _grid.Tag = _vm.Sedes;
        }

        private async void GridCellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var sedes = _grid.Tag as List<Sede>;
            if (sedes == null || e.RowIndex >= sedes.Count) return;
            var sede = sedes[e.RowIndex];

            if (_grid.Columns[e.ColumnIndex].Name == "Editar")
            {
                _vm.EditarSede(sede);
                _txtNombre.Text = _vm.Nombre;
                _txtDireccion.Text = _vm.Direccion;
                MostrarFormulario(true);
            }
            else if (_grid.Columns[e.ColumnIndex].Name == "Desactivar")
            {
                if (MessageBox.Show($"¿Desactivar la sede '{sede.Nombre}'?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    await _vm.DesactivarAsync(sede.Id);
            }
        }

        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }
            if (propiedad == nameof(SedeViewModel.MensajeError))
            { _lblError.Text = _vm.MensajeError; _lblExito.Text = string.Empty; }
            if (propiedad == nameof(SedeViewModel.MensajeExito))
            { _lblExito.Text = _vm.MensajeExito; _lblError.Text = string.Empty; RefrescarGrid(); MostrarFormulario(false); }
            if (propiedad == nameof(SedeViewModel.Sedes)) RefrescarGrid();
        }

        private void MostrarFormulario(bool visible)
        {
            _pnlForm.Visible = visible;
            if (visible) _pnlForm.BringToFront();
        }
    }
}