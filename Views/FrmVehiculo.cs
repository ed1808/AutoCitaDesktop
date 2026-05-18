using AutoCita.Enums;
using AutoCita.Models;
using AutoCita.ViewModels;

namespace AutoCita.Views
{
    /// <summary>
    /// Formulario para búsqueda y creación de vehículos.
    /// </summary>
    public partial class FrmVehiculo : Form
    {
        private readonly VehiculoViewModel _vm;

        private TextBox  _txtPlacaBusqueda = new();
        private Button   _btnBuscar        = new();
        private Panel    _pnlResultado     = new();
        private Panel    _pnlCreacion      = new();
        private TextBox  _txtPlaca         = new();
        private ComboBox _cboTipo          = new();
        private Label    _lblError         = new();
        private Label    _lblExito         = new();
        private Button   _btnGuardar       = new();

        /// <summary>
        /// Se dispara cuando un vehículo fue seleccionado o creado.
        /// </summary>
        public event EventHandler<Vehiculo>? VehiculoSeleccionado;

        /// <summary>
        /// Constructor del formulario de vehículo.
        /// </summary>
        /// <param name="vm">ViewModel de vehículo.</param>
        public FrmVehiculo(VehiculoViewModel vm)
        {
            _vm = vm;
            _vm.VehiculoSeleccionado += (_, v) => VehiculoSeleccionado?.Invoke(this, v);
            _vm.PropertyChanged      += (_, e) => ActualizarUi(e.PropertyName);
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            Text            = "Gestión de Vehículos";
            Size            = new Size(560, 480);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.FromArgb(245, 245, 250);
            Font            = new Font("Segoe UI", 9.5f);

            var lblTitulo = new Label
            {
                Text     = "Búsqueda de Vehículo",
                Font     = new Font("Segoe UI", 12f, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var lblPlacaB = new Label { Text = "Placa del vehículo:", Location = new Point(20, 65), AutoSize = true };
            _txtPlacaBusqueda.Location       = new Point(20, 85);
            _txtPlacaBusqueda.Size           = new Size(240, 28);
            _txtPlacaBusqueda.PlaceholderText = "Ej: ABC123";
            _txtPlacaBusqueda.CharacterCasing = CharacterCasing.Upper;
            _txtPlacaBusqueda.TextChanged    += (_, _) => _vm.PlacaBusqueda = _txtPlacaBusqueda.Text;
            _txtPlacaBusqueda.KeyDown        += async (_, e) => { if (e.KeyCode == Keys.Enter) await _vm.BuscarPorPlacaAsync(); };

            _btnBuscar.Text      = "Buscar";
            _btnBuscar.Location  = new Point(270, 84);
            _btnBuscar.Size      = new Size(90, 30);
            _btnBuscar.BackColor = Color.FromArgb(30, 100, 200);
            _btnBuscar.ForeColor = Color.White;
            _btnBuscar.FlatStyle = FlatStyle.Flat;
            _btnBuscar.FlatAppearance.BorderSize = 0;
            _btnBuscar.Click    += async (_, _) => await _vm.BuscarPorPlacaAsync();

            // Panel de resultado
            _pnlResultado.Location  = new Point(20, 130);
            _pnlResultado.Size      = new Size(500, 80);
            _pnlResultado.BackColor = Color.FromArgb(230, 245, 230);
            _pnlResultado.Visible   = false;
            var lblResLabel  = new Label { Text = "Vehículo encontrado:", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            var lblResultado = new Label { Name = "lblResultado", Location = new Point(10, 30), AutoSize = true };
            _pnlResultado.Controls.AddRange([lblResLabel, lblResultado]);

            // Panel de creación
            _pnlCreacion.Location  = new Point(20, 130);
            _pnlCreacion.Size      = new Size(500, 260);
            _pnlCreacion.Visible   = false;
            _pnlCreacion.BackColor = Color.Transparent;

            var lblNewTitulo = new Label { Text = "Registrar nuevo vehículo", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(0, 0), AutoSize = true };

            var lblPlaca2 = new Label { Text = "Placa:", Location = new Point(0, 35), AutoSize = true };
            _txtPlaca.Location        = new Point(0, 55);
            _txtPlaca.Size            = new Size(200, 28);
            _txtPlaca.CharacterCasing = CharacterCasing.Upper;
            _txtPlaca.TextChanged    += (_, _) => _vm.Placa = _txtPlaca.Text;

            var lblTipo = new Label { Text = "Tipo de vehículo:", Location = new Point(0, 95), AutoSize = true };
            _cboTipo.Location     = new Point(0, 115);
            _cboTipo.Size         = new Size(200, 28);
            _cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboTipo.DataSource   = Enum.GetValues<TipoVehiculo>();
            _cboTipo.SelectedIndexChanged += (_, _) =>
            {
                if (_cboTipo.SelectedItem is TipoVehiculo t) _vm.TipoVehiculo = t;
            };

            _btnGuardar.Text      = "Guardar vehículo";
            _btnGuardar.Location  = new Point(0, 200);
            _btnGuardar.Size      = new Size(160, 36);
            _btnGuardar.BackColor = Color.FromArgb(30, 150, 80);
            _btnGuardar.ForeColor = Color.White;
            _btnGuardar.FlatStyle = FlatStyle.Flat;
            _btnGuardar.FlatAppearance.BorderSize = 0;
            _btnGuardar.Click    += async (_, _) => await _vm.GuardarAsync();

            _pnlCreacion.Controls.AddRange([lblNewTitulo, lblPlaca2, _txtPlaca, lblTipo, _cboTipo, _btnGuardar]);

            _lblError.Location  = new Point(20, 400);
            _lblError.Size      = new Size(500, 40);
            _lblError.ForeColor = Color.Red;

            _lblExito.Location  = new Point(20, 400);
            _lblExito.Size      = new Size(500, 40);
            _lblExito.ForeColor = Color.FromArgb(30, 150, 30);

            Controls.AddRange([lblTitulo, lblPlacaB, _txtPlacaBusqueda, _btnBuscar,
                _pnlResultado, _pnlCreacion, _lblError, _lblExito]);
        }

        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }

            if (propiedad == nameof(VehiculoViewModel.MensajeError))
            {
                _lblError.Text = _vm.MensajeError;
                _lblExito.Text = string.Empty;
            }
            if (propiedad == nameof(VehiculoViewModel.MensajeExito))
            {
                _lblExito.Text = _vm.MensajeExito;
                _lblError.Text = string.Empty;
            }
            if (propiedad == nameof(VehiculoViewModel.ModoCreacion))
            {
                _pnlCreacion.Visible  = _vm.ModoCreacion;
                _pnlResultado.Visible = false;
                if (_vm.ModoCreacion) _txtPlaca.Text = _vm.Placa;
            }
            if (propiedad == nameof(VehiculoViewModel.VehiculoEncontrado) && _vm.VehiculoEncontrado != null)
            {
                var lbl = _pnlResultado.Controls["lblResultado"] as Label;
                if (lbl != null)
                    lbl.Text = $"{_vm.VehiculoEncontrado.Placa} — {_vm.VehiculoEncontrado.TipoVehiculo} | Propietario: {_vm.NombreCliente}";
                _pnlResultado.Visible = true;
                _pnlCreacion.Visible  = false;
            }
        }

        /// <summary>
        /// Establece el ClienteId del propietario antes de que el usuario guarde el vehículo.
        /// </summary>
        /// <param name="clienteId">Identificador del cliente propietario.</param>
        /// <param name="nombreCliente">Nombre del cliente para mostrar en pantalla.</param>
        public void EstablecerPropietario(int clienteId, string nombreCliente)
        {
            _vm.ClienteId     = clienteId;
            _vm.NombreCliente = nombreCliente;
        }
    }
}
