using AutoCita.Models;
using AutoCita.ViewModels;

namespace AutoCita.Views
{
    /// <summary>
    /// Formulario para búsqueda y creación de clientes.
    /// </summary>
    public partial class FrmCliente : Form
    {
        private readonly ClienteViewModel _vm;

        private TextBox _txtDocBusqueda = new();
        private Button  _btnBuscar      = new();
        private Panel   _pnlResultado   = new();
        private Panel   _pnlCreacion    = new();
        private TextBox _txtDocumento   = new();
        private TextBox _txtNombre      = new();
        private TextBox _txtTelefono    = new();
        private Label   _lblError       = new();
        private Label   _lblExito       = new();
        private Button  _btnGuardar     = new();

        /// <summary>
        /// Se dispara cuando un cliente fue seleccionado o creado.
        /// </summary>
        public event EventHandler<Cliente>? ClienteSeleccionado;

        /// <summary>
        /// Constructor del formulario de cliente.
        /// </summary>
        /// <param name="vm">ViewModel de cliente.</param>
        public FrmCliente(ClienteViewModel vm)
        {
            _vm = vm;
            _vm.ClienteSeleccionado += (_, c) => ClienteSeleccionado?.Invoke(this, c);
            _vm.PropertyChanged     += (_, e) => ActualizarUi(e.PropertyName);
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            Text            = "Gestión de Clientes";
            Size            = new Size(560, 500);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.FromArgb(245, 245, 250);
            Font            = new Font("Segoe UI", 9.5f);

            var lblTitulo = new Label
            {
                Text      = "Búsqueda de Cliente",
                Font      = new Font("Segoe UI", 12f, FontStyle.Bold),
                Location  = new Point(20, 20),
                AutoSize  = true
            };

            // Búsqueda
            var lblDoc = new Label { Text = "Documento de identidad:", Location = new Point(20, 65), AutoSize = true };
            _txtDocBusqueda.Location       = new Point(20, 85);
            _txtDocBusqueda.Size           = new Size(300, 28);
            _txtDocBusqueda.PlaceholderText = "Ej: 1234567890";
            _txtDocBusqueda.TextChanged   += (_, _) => _vm.DocumentoBusqueda = _txtDocBusqueda.Text;
            _txtDocBusqueda.KeyDown       += async (_, e) => { if (e.KeyCode == Keys.Enter) await _vm.BuscarPorDocumentoAsync(); };

            _btnBuscar.Text      = "Buscar";
            _btnBuscar.Location  = new Point(330, 84);
            _btnBuscar.Size      = new Size(90, 30);
            _btnBuscar.BackColor = Color.FromArgb(30, 100, 200);
            _btnBuscar.ForeColor = Color.White;
            _btnBuscar.FlatStyle = FlatStyle.Flat;
            _btnBuscar.FlatAppearance.BorderSize = 0;
            _btnBuscar.Click    += async (_, _) => await _vm.BuscarPorDocumentoAsync();

            // Panel de resultado (cliente encontrado)
            _pnlResultado.Location  = new Point(20, 130);
            _pnlResultado.Size      = new Size(500, 80);
            _pnlResultado.BackColor = Color.FromArgb(230, 245, 230);
            _pnlResultado.Visible   = false;

            var lblResultLabel = new Label { Text = "Cliente encontrado:", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            var lblResultado   = new Label { Name = "lblResultado", Location = new Point(10, 30), AutoSize = true };
            _pnlResultado.Controls.AddRange([lblResultLabel, lblResultado]);

            // Panel de creación
            _pnlCreacion.Location  = new Point(20, 130);
            _pnlCreacion.Size      = new Size(500, 280);
            _pnlCreacion.Visible   = false;
            _pnlCreacion.BackColor = Color.Transparent;

            var lblNewTitulo = new Label { Text = "Registrar nuevo cliente", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(0, 0), AutoSize = true };

            AgregarCampo(_pnlCreacion, "Documento de identidad:", _txtDocumento, new Point(0, 28), new Point(0, 50));
            AgregarCampo(_pnlCreacion, "Nombre completo:",         _txtNombre,   new Point(0, 90), new Point(0, 112));
            AgregarCampo(_pnlCreacion, "Teléfono (WhatsApp):",     _txtTelefono, new Point(0, 152), new Point(0, 174));

            _txtDocumento.TextChanged += (_, _) => _vm.DocumentoIdentidad = _txtDocumento.Text;
            _txtNombre.TextChanged    += (_, _) => _vm.NombreCompleto     = _txtNombre.Text;
            _txtTelefono.TextChanged  += (_, _) => _vm.Telefono           = _txtTelefono.Text;

            _btnGuardar.Text      = "Guardar cliente";
            _btnGuardar.Location  = new Point(0, 218);
            _btnGuardar.Size      = new Size(160, 36);
            _btnGuardar.BackColor = Color.FromArgb(30, 150, 80);
            _btnGuardar.ForeColor = Color.White;
            _btnGuardar.FlatStyle = FlatStyle.Flat;
            _btnGuardar.FlatAppearance.BorderSize = 0;
            _btnGuardar.Click    += async (_, _) => await _vm.GuardarAsync();

            _pnlCreacion.Controls.AddRange([lblNewTitulo, _btnGuardar]);

            // Mensajes
            _lblError.Location  = new Point(20, 420);
            _lblError.Size      = new Size(500, 40);
            _lblError.ForeColor = Color.Red;

            _lblExito.Location  = new Point(20, 420);
            _lblExito.Size      = new Size(500, 40);
            _lblExito.ForeColor = Color.FromArgb(30, 150, 30);

            Controls.AddRange([lblTitulo, lblDoc, _txtDocBusqueda, _btnBuscar,
                _pnlResultado, _pnlCreacion, _lblError, _lblExito]);
        }

        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }

            if (propiedad == nameof(ClienteViewModel.MensajeError))
            {
                _lblError.Text  = _vm.MensajeError;
                _lblExito.Text  = string.Empty;
            }
            if (propiedad == nameof(ClienteViewModel.MensajeExito))
            {
                _lblExito.Text  = _vm.MensajeExito;
                _lblError.Text  = string.Empty;
            }
            if (propiedad == nameof(ClienteViewModel.ModoCreacion))
            {
                _pnlCreacion.Visible  = _vm.ModoCreacion;
                _pnlResultado.Visible = false;
                if (_vm.ModoCreacion)
                    _txtDocumento.Text = _vm.DocumentoIdentidad;
            }
            if (propiedad == nameof(ClienteViewModel.ClienteEncontrado) && _vm.ClienteEncontrado != null)
            {
                var lbl = _pnlResultado.Controls["lblResultado"] as Label;
                if (lbl != null)
                    lbl.Text = $"{_vm.ClienteEncontrado.NombreCompleto} | Tel: {_vm.ClienteEncontrado.Telefono}";
                _pnlResultado.Visible = true;
                _pnlCreacion.Visible  = false;
            }
        }

        private void AgregarCampo(Panel panel, string etiqueta, TextBox txt, Point posLabel, Point posTxt)
        {
            var lbl = new Label { Text = etiqueta, Location = posLabel, AutoSize = true };
            txt.Location = posTxt;
            txt.Size     = new Size(400, 28);
            panel.Controls.AddRange([lbl, txt]);
        }
    }
}
