using AutoCita.Enums;
using AutoCita.Models;
using AutoCita.ViewModels;

namespace AutoCita.Views
{
    /// <summary>
    /// Formulario de gestión de usuarios del sistema (solo rol Administrador).
    /// </summary>
    public partial class FrmUsuario : Form
    {
        private readonly UsuarioViewModel _vm;

        private DataGridView _grid        = new();
        private TextBox      _txtUsername = new();
        private TextBox      _txtPassword = new();
        private ComboBox     _cboRol      = new();
        private ComboBox     _cboSede     = new();
        private Label        _lblError    = new();
        private Label        _lblExito    = new();
        private Button       _btnNuevo    = new();
        private Button       _btnGuardar  = new();
        private Button       _btnCancelar = new();
        private Panel        _pnlForm     = new();

        /// <summary>
        /// Constructor del formulario de usuarios.
        /// </summary>
        /// <param name="vm">ViewModel de usuarios.</param>
        public FrmUsuario(UsuarioViewModel vm)
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
            Text          = "Gestión de Usuarios";
            Size          = new Size(900, 620);
            StartPosition = FormStartPosition.CenterParent;
            BackColor     = Color.FromArgb(245, 245, 250);
            Font          = new Font("Segoe UI", 9.5f);

            var lblTitulo = new Label
            {
                Text     = "Usuarios del sistema",
                Font     = new Font("Segoe UI", 13f, FontStyle.Bold),
                Location = new Point(20, 18),
                AutoSize = true
            };

            // ── Botón nuevo ───────────────────────────────────────────────────
            _btnNuevo.Text      = "+ Nuevo usuario";
            _btnNuevo.Location  = new Point(700, 15);
            _btnNuevo.Size      = new Size(160, 34);
            _btnNuevo.BackColor = Color.FromArgb(30, 100, 200);
            _btnNuevo.ForeColor = Color.White;
            _btnNuevo.FlatStyle = FlatStyle.Flat;
            _btnNuevo.FlatAppearance.BorderSize = 0;
            _btnNuevo.Click    += (_, _) => { _vm.NuevoUsuario(); MostrarFormulario(true); };

            // ── Grid ──────────────────────────────────────────────────────────
            _grid.Location        = new Point(20, 65);
            _grid.Size            = new Size(840, 280);
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

            // Botones Editar/Desactivar embebidos en la fila
            var colEditar = new DataGridViewButtonColumn
            {
                Name        = "Editar",
                HeaderText  = "",
                Text        = "Editar",
                UseColumnTextForButtonValue = true,
                FillWeight  = 10
            };
            var colDesact = new DataGridViewButtonColumn
            {
                Name        = "Desactivar",
                HeaderText  = "",
                Text        = "Desactivar",
                UseColumnTextForButtonValue = true,
                FillWeight  = 10
            };
            _grid.Columns.AddRange(colEditar, colDesact);
            _grid.CellClick += GridCellClick;

            // ── Formulario lateral ────────────────────────────────────────────
            _pnlForm.Location  = new Point(20, 360);
            _pnlForm.Size      = new Size(840, 220);
            _pnlForm.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _pnlForm.BackColor = Color.White;
            _pnlForm.Padding   = new Padding(12);
            _pnlForm.Visible   = false;

            var lblFTitulo = new Label { Text = "Datos del usuario", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(12, 12), AutoSize = true };

            AgregarCampoForm("Usuario:", _txtUsername, new Point(12, 40), new Point(12, 62));
            AgregarCampoForm("Contraseña (vacío = sin cambios):", _txtPassword, new Point(220, 40), new Point(220, 62));
            _txtPassword.PasswordChar = '•';
            _txtUsername.TextChanged += (_, _) => _vm.Username = _txtUsername.Text;
            _txtPassword.TextChanged += (_, _) => _vm.Password = _txtPassword.Text;

            var lblRol = new Label { Text = "Rol:", Location = new Point(12, 102), AutoSize = true };
            _cboRol.Location      = new Point(12, 122);
            _cboRol.Size          = new Size(180, 28);
            _cboRol.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboRol.DataSource    = Enum.GetValues<RolUsuario>();
            _cboRol.SelectedIndexChanged += (_, _) => { if (_cboRol.SelectedItem is RolUsuario r) _vm.Rol = r; };

            var lblSede = new Label { Text = "Sede (opcional):", Location = new Point(210, 102), AutoSize = true };
            _cboSede.Location      = new Point(210, 122);
            _cboSede.Size          = new Size(220, 28);
            _cboSede.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboSede.DisplayMember = nameof(Sede.Nombre);
            _cboSede.ValueMember   = nameof(Sede.Id);
            _cboSede.SelectedIndexChanged += (_, _) =>
            {
                _vm.SedeId = _cboSede.SelectedItem is Sede s ? s.Id : null;
            };

            _btnGuardar.Text      = "Guardar";
            _btnGuardar.Location  = new Point(12, 170);
            _btnGuardar.Size      = new Size(120, 36);
            _btnGuardar.BackColor = Color.FromArgb(30, 150, 80);
            _btnGuardar.ForeColor = Color.White;
            _btnGuardar.FlatStyle = FlatStyle.Flat;
            _btnGuardar.FlatAppearance.BorderSize = 0;
            _btnGuardar.Click    += async (_, _) => await _vm.GuardarAsync();

            _btnCancelar.Text      = "Cancelar";
            _btnCancelar.Location  = new Point(144, 170);
            _btnCancelar.Size      = new Size(100, 36);
            _btnCancelar.FlatStyle = FlatStyle.Flat;
            _btnCancelar.FlatAppearance.BorderSize = 0;
            _btnCancelar.Click    += (_, _) => { _vm.NuevoUsuario(); MostrarFormulario(false); };

            _pnlForm.Controls.AddRange([lblFTitulo, _cboRol, lblRol, _cboSede, lblSede, _btnGuardar, _btnCancelar]);

            // Mensajes
            _lblError.Location  = new Point(20, 590);
            _lblError.Size      = new Size(840, 24);
            _lblError.ForeColor = Color.Red;
            _lblError.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left;
            _lblExito.Location  = new Point(20, 590);
            _lblExito.Size      = new Size(840, 24);
            _lblExito.ForeColor = Color.FromArgb(30, 150, 30);
            _lblExito.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left;

            Controls.AddRange([lblTitulo, _btnNuevo, _grid, _pnlForm, _lblError, _lblExito]);
        }

        private void RefrescarGrid()
        {
            _grid.Rows.Clear();
            if (_grid.Columns.Count < 5)
            {
                _grid.Columns.Clear();
                _grid.Columns.Add("Username", "Usuario");
                _grid.Columns.Add("Rol", "Rol");
                _grid.Columns.Add("Sede", "Sede");
                var colEditar = new DataGridViewButtonColumn { Name = "Editar", HeaderText = "", Text = "Editar", UseColumnTextForButtonValue = true, FillWeight = 8 };
                var colDesact = new DataGridViewButtonColumn { Name = "Desactivar", HeaderText = "", Text = "Desactivar", UseColumnTextForButtonValue = true, FillWeight = 10 };
                _grid.Columns.AddRange(colEditar, colDesact);
            }

            foreach (var u in _vm.Usuarios)
                _grid.Rows.Add(u.Username, u.Rol.ToString(), u.Sede?.Nombre ?? "—");

            // Guardar referencia para acceso en CellClick
            _grid.Tag = _vm.Usuarios;

            // Cargar sedes en combo
            var sedes = new List<Sede> { new Sede { Id = 0, Nombre = "(Global)" } };
            sedes.AddRange(_vm.Sedes);
            _cboSede.DataSource    = sedes;
            _cboSede.DisplayMember = nameof(Sede.Nombre);
            _cboSede.ValueMember   = nameof(Sede.Id);
        }

        private async void GridCellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var usuarios = _grid.Tag as List<Usuario>;
            if (usuarios == null || e.RowIndex >= usuarios.Count) return;
            var usuario = usuarios[e.RowIndex];

            if (_grid.Columns[e.ColumnIndex].Name == "Editar")
            {
                _vm.EditarUsuario(usuario);
                _txtUsername.Text = _vm.Username;
                _txtPassword.Text = string.Empty;
                _cboRol.SelectedItem = _vm.Rol;
                MostrarFormulario(true);
            }
            else if (_grid.Columns[e.ColumnIndex].Name == "Desactivar")
            {
                if (MessageBox.Show($"¿Desactivar al usuario '{usuario.Username}'?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    await _vm.DesactivarAsync(usuario.Id);
            }
        }

        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }

            if (propiedad == nameof(UsuarioViewModel.MensajeError))
            {
                _lblError.Text = _vm.MensajeError; _lblExito.Text = string.Empty;
            }
            if (propiedad == nameof(UsuarioViewModel.MensajeExito))
            {
                _lblExito.Text = _vm.MensajeExito; _lblError.Text = string.Empty;
                RefrescarGrid();
                MostrarFormulario(false);
            }
            if (propiedad == nameof(UsuarioViewModel.Usuarios)) RefrescarGrid();
        }

        private void MostrarFormulario(bool visible)
        {
            _pnlForm.Visible = visible;
            Height           = visible ? 650 : 430;
        }

        private void AgregarCampoForm(string etiqueta, TextBox txt, Point posLabel, Point posTxt)
        {
            var lbl = new Label { Text = etiqueta, Location = posLabel, AutoSize = true };
            txt.Location = posTxt;
            txt.Size     = new Size(190, 28);
            _pnlForm.Controls.AddRange([lbl, txt]);
        }
    }
}
