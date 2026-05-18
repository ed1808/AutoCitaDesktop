using AutoCita.Models;
using AutoCita.Repositories;
using AutoCita.ViewModels;
using Microsoft.EntityFrameworkCore;
using AutoCita.Data;
using AutoCita.Helpers;

namespace AutoCita.Views
{
    /// <summary>
    /// Formulario principal de gestión de citas: agendamiento, cancelación y reprogramación.
    /// El flujo es: seleccionar cliente → seleccionar vehículo → completar datos → guardar.
    /// </summary>
    public partial class FrmCita : Form
    {
        private readonly CitaViewModel _vm;
        private bool _cargando = false;

        private Label    _lblCliente        = new();
        private Label    _lblVehiculo       = new();
        private Button   _btnSeleccionarCliente  = new();
        private Button   _btnSeleccionarVehiculo = new();
        private ComboBox _cboSede           = new();
        private DateTimePicker _dtpFecha    = new();
        private ComboBox _cboHoras          = new();
        private ComboBox _cboMinutos        = new();
        private ComboBox _cboAmPm           = new();
        private TextBox  _txtMotivo         = new();
        private TextBox  _txtObservaciones  = new();
        private Label    _lblDisponibilidad = new();
        private Label    _lblError          = new();
        private Label    _lblExito          = new();
        private Button   _btnGuardar        = new();
        private Button   _btnCancelarCita   = new();
        private Button   _btnReprogramar    = new();
        private Panel    _pnlAcciones       = new();

        /// <summary>
        /// Constructor del formulario de citas.
        /// </summary>
        /// <param name="vm">ViewModel de citas.</param>
        public FrmCita(CitaViewModel vm)
        {
            _vm = vm;
            _vm.PropertyChanged  += (_, e) => ActualizarUi(e.PropertyName);
            _vm.OperacionExitosa += (_, _) => ActualizarEstadoExitoso();
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
            Text          = "Gestión de Citas";
            Size          = new Size(680, 640);
            StartPosition = FormStartPosition.CenterParent;
            BackColor     = Color.FromArgb(245, 245, 250);
            Font          = new Font("Segoe UI", 9.5f);
            AutoScroll    = true;

            var lblTitulo = new Label
            {
                Text     = "Nueva Cita",
                Font     = new Font("Segoe UI", 13f, FontStyle.Bold),
                Location = new Point(20, 18),
                AutoSize = true
            };

            // ── Cliente ──────────────────────────────────────────────────────
            var lblClienteLabel = new Label { Text = "Cliente:", Location = new Point(20, 65), AutoSize = true };
            _lblCliente.Text     = "(Sin seleccionar)";
            _lblCliente.Location = new Point(20, 85);
            _lblCliente.Size     = new Size(380, 24);
            _lblCliente.ForeColor = Color.Gray;

            _btnSeleccionarCliente.Text      = "Seleccionar cliente";
            _btnSeleccionarCliente.Location  = new Point(410, 82);
            _btnSeleccionarCliente.Size      = new Size(180, 30);
            _btnSeleccionarCliente.BackColor = Color.FromArgb(30, 100, 200);
            _btnSeleccionarCliente.ForeColor = Color.White;
            _btnSeleccionarCliente.FlatStyle = FlatStyle.Flat;
            _btnSeleccionarCliente.FlatAppearance.BorderSize = 0;
            _btnSeleccionarCliente.Click    += AbrirSeleccionCliente;

            // ── Vehículo ─────────────────────────────────────────────────────
            var lblVehiculoLabel = new Label { Text = "Vehículo:", Location = new Point(20, 125), AutoSize = true };
            _lblVehiculo.Text     = "(Sin seleccionar)";
            _lblVehiculo.Location = new Point(20, 145);
            _lblVehiculo.Size     = new Size(380, 24);
            _lblVehiculo.ForeColor = Color.Gray;

            _btnSeleccionarVehiculo.Text      = "Seleccionar vehículo";
            _btnSeleccionarVehiculo.Location  = new Point(410, 142);
            _btnSeleccionarVehiculo.Size      = new Size(180, 30);
            _btnSeleccionarVehiculo.BackColor = Color.FromArgb(30, 100, 200);
            _btnSeleccionarVehiculo.ForeColor = Color.White;
            _btnSeleccionarVehiculo.FlatStyle = FlatStyle.Flat;
            _btnSeleccionarVehiculo.FlatAppearance.BorderSize = 0;
            _btnSeleccionarVehiculo.Click    += AbrirSeleccionVehiculo;
            _btnSeleccionarVehiculo.Enabled  = false;

            // ── Sede ─────────────────────────────────────────────────────────
            var lblSede = new Label { Text = "Sede:", Location = new Point(20, 185), AutoSize = true };
            _cboSede.Location      = new Point(20, 205);
            _cboSede.Size          = new Size(300, 28);
            _cboSede.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboSede.DisplayMember = nameof(Sede.Nombre);
            _cboSede.ValueMember   = nameof(Sede.Id);
            _cboSede.SelectedIndexChanged += async (_, _) =>
            {
                if (_cargando) return;
                if (_cboSede.SelectedItem is Sede s)
                {
                    _vm.SedeId = s.Id;
                    await _vm.VerificarDisponibilidadAsync();
                }
            };

            // ── Fecha y Hora ─────────────────────────────────────────────────
            var lblFecha = new Label { Text = "Fecha:", Location = new Point(20, 248), AutoSize = true };
            _dtpFecha.Location     = new Point(20, 268);
            _dtpFecha.Size         = new Size(200, 28);
            _dtpFecha.Format       = DateTimePickerFormat.Short;
            _dtpFecha.MinDate      = DateTime.Now.Date.AddDays(1);
            _dtpFecha.ValueChanged += async (_, _) => { if (_cargando) return; _vm.Fecha = _dtpFecha.Value; await _vm.VerificarDisponibilidadAsync(); };

            var lblHora = new Label { Text = "Hora:", Location = new Point(240, 248), AutoSize = true };

            // Horas: 01 a 12
            _cboHoras.Location      = new Point(240, 268);
            _cboHoras.Size          = new Size(60, 28);
            _cboHoras.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboHoras.Items.AddRange(Enumerable.Range(1, 12).Select(h => h.ToString("D2")).ToArray<object>());
            _cboHoras.SelectedItem  = "08";
            _cboHoras.SelectedIndexChanged += async (_, _) => await OnSelectorHoraCambiado();

            // Minutos: 00, 15, 30, 45
            _cboMinutos.Location      = new Point(308, 268);
            _cboMinutos.Size          = new Size(55, 28);
            _cboMinutos.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboMinutos.Items.AddRange(new object[] { "00", "15", "30", "45" });
            _cboMinutos.SelectedItem  = "00";
            _cboMinutos.SelectedIndexChanged += async (_, _) => await OnSelectorHoraCambiado();

            // AM / PM
            _cboAmPm.Location      = new Point(371, 268);
            _cboAmPm.Size          = new Size(55, 28);
            _cboAmPm.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboAmPm.Items.AddRange(new object[] { "AM", "PM" });
            _cboAmPm.SelectedItem  = "AM";
            _cboAmPm.SelectedIndexChanged += async (_, _) => await OnSelectorHoraCambiado();

            _lblDisponibilidad.Location  = new Point(20, 308);
            _lblDisponibilidad.Size      = new Size(580, 22);
            _lblDisponibilidad.ForeColor = Color.FromArgb(30, 100, 200);

            // ── Motivo ───────────────────────────────────────────────────────
            var lblMotivo = new Label { Text = "Motivo de ingreso:", Location = new Point(20, 340), AutoSize = true };
            _txtMotivo.Location     = new Point(20, 360);
            _txtMotivo.Size         = new Size(580, 28);
            _txtMotivo.PlaceholderText = "Ej: Cambio de aceite, revisión frenos...";
            _txtMotivo.TextChanged += (_, _) => _vm.Motivo = _txtMotivo.Text;

            var lblObs = new Label { Text = "Observaciones (opcional):", Location = new Point(20, 400), AutoSize = true };
            _txtObservaciones.Location    = new Point(20, 420);
            _txtObservaciones.Size        = new Size(580, 55);
            _txtObservaciones.Multiline   = true;
            _txtObservaciones.TextChanged += (_, _) => _vm.Observaciones = _txtObservaciones.Text;

            // ── Mensajes ─────────────────────────────────────────────────────
            _lblError.Location  = new Point(20, 488);
            _lblError.Size      = new Size(580, 36);
            _lblError.ForeColor = Color.Red;

            _lblExito.Location  = new Point(20, 488);
            _lblExito.Size      = new Size(580, 36);
            _lblExito.ForeColor = Color.FromArgb(30, 150, 30);

            // ── Panel de acciones ────────────────────────────────────────────
            _pnlAcciones.Location  = new Point(20, 535);
            _pnlAcciones.Size      = new Size(620, 50);
            _pnlAcciones.BackColor = Color.Transparent;

            _btnGuardar.Text      = "Agendar cita";
            _btnGuardar.Location  = new Point(0, 0);
            _btnGuardar.Size      = new Size(150, 38);
            _btnGuardar.BackColor = Color.FromArgb(30, 150, 80);
            _btnGuardar.ForeColor = Color.White;
            _btnGuardar.FlatStyle = FlatStyle.Flat;
            _btnGuardar.FlatAppearance.BorderSize = 0;
            _btnGuardar.Click    += async (_, _) => await _vm.GuardarAsync();

            _btnCancelarCita.Text      = "Cancelar cita";
            _btnCancelarCita.Location  = new Point(162, 0);
            _btnCancelarCita.Size      = new Size(140, 38);
            _btnCancelarCita.BackColor = Color.FromArgb(200, 60, 60);
            _btnCancelarCita.ForeColor = Color.White;
            _btnCancelarCita.FlatStyle = FlatStyle.Flat;
            _btnCancelarCita.FlatAppearance.BorderSize = 0;
            _btnCancelarCita.Visible  = false;
            _btnCancelarCita.Click   += async (_, _) =>
            {
                if (MessageBox.Show("¿Confirma la cancelación de la cita?", "Cancelar cita",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    await _vm.CancelarCitaAsync();
            };

            _btnReprogramar.Text      = "Reprogramar";
            _btnReprogramar.Location  = new Point(314, 0);
            _btnReprogramar.Size      = new Size(140, 38);
            _btnReprogramar.BackColor = Color.FromArgb(200, 130, 30);
            _btnReprogramar.ForeColor = Color.White;
            _btnReprogramar.FlatStyle = FlatStyle.Flat;
            _btnReprogramar.FlatAppearance.BorderSize = 0;
            _btnReprogramar.Visible  = false;
            _btnReprogramar.Click   += async (_, _) => await _vm.ReprogramarCitaAsync();

            _pnlAcciones.Controls.AddRange([_btnGuardar, _btnCancelarCita, _btnReprogramar]);

            Controls.AddRange([
                lblTitulo,
                lblClienteLabel, _lblCliente, _btnSeleccionarCliente,
                lblVehiculoLabel, _lblVehiculo, _btnSeleccionarVehiculo,
                lblSede, _cboSede,
                lblFecha, _dtpFecha, lblHora, _cboHoras, _cboMinutos, _cboAmPm, _lblDisponibilidad,
                lblMotivo, _txtMotivo, lblObs, _txtObservaciones,
                _lblError, _lblExito, _pnlAcciones
            ]);
        }

        private void CargarSedesEnCombo()
        {
            _cboSede.DataSource    = null;
            _cboSede.DataSource    = _vm.Sedes;
            _cboSede.DisplayMember = nameof(Sede.Nombre);
            _cboSede.ValueMember   = nameof(Sede.Id);
        }

        private void SincronizarHoraDesdeSelectores()
        {
            if (_cboHoras.SelectedItem is null || _cboMinutos.SelectedItem is null || _cboAmPm.SelectedItem is null) return;
            int hora  = int.Parse(_cboHoras.SelectedItem.ToString()!);
            int min   = int.Parse(_cboMinutos.SelectedItem.ToString()!);
            bool isPm = _cboAmPm.SelectedItem.ToString() == "PM";
            if (isPm  && hora != 12) hora += 12;
            if (!isPm && hora == 12) hora  = 0;
            _vm.Hora = new TimeSpan(hora, min, 0);
        }

        private async Task OnSelectorHoraCambiado()
        {
            if (_cargando) return;
            SincronizarHoraDesdeSelectores();
            await _vm.VerificarDisponibilidadAsync();
        }

        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }

            if (propiedad == nameof(CitaViewModel.MensajeError))
            {
                _lblError.Text = _vm.MensajeError;
                _lblExito.Text = string.Empty;
            }
            if (propiedad == nameof(CitaViewModel.MensajeExito))
            {
                _lblExito.Text = _vm.MensajeExito;
                _lblError.Text = string.Empty;
            }
            if (propiedad == nameof(CitaViewModel.MensajeDisponibilidad))
            {
                _lblDisponibilidad.ForeColor = _vm.HayDisponibilidad
                    ? Color.FromArgb(30, 150, 30)
                    : Color.Red;
                _lblDisponibilidad.Text = _vm.MensajeDisponibilidad;
            }
        }

        private void ActualizarEstadoExitoso()
        {
            if (InvokeRequired) { Invoke(ActualizarEstadoExitoso); return; }

            // Mostrar mensaje de confirmación visual al usuario
            if (!string.IsNullOrEmpty(_vm.MensajeExito))
            {
                MessageBox.Show(_vm.MensajeExito, "Operación exitosa", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            _btnCancelarCita.Visible  = false;
            _btnReprogramar.Visible   = false;
            _btnGuardar.Visible       = false;
        }

        /// <summary>
        /// Carga los datos de una cita existente para edición (cancelar/reprogramar).
        /// </summary>
        /// <param name="cita">Cita a cargar en el formulario.</param>
        public void CargarCita(Cita cita)
        {
            _cargando = true;
            try
            {
                _vm.CargarCita(cita);
                Text            = "Gestión de Cita — " + cita.FechaHora.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                _lblCliente.Text = cita.Vehiculo?.Cliente?.NombreCompleto ?? "(desconocido)";
                _lblCliente.ForeColor = Color.Black;
                _lblVehiculo.Text = cita.Vehiculo != null
                    ? $"{cita.Vehiculo.Placa} — {cita.Vehiculo.TipoVehiculo}"
                    : "(desconocido)";
                _lblVehiculo.ForeColor    = Color.Black;
                _dtpFecha.Value           = cita.FechaHora.ToLocalTime().Date;

                var t       = cita.FechaHora.ToLocalTime().TimeOfDay;
                int h12     = t.Hours % 12 == 0 ? 12 : t.Hours % 12;
                int minNorm = (t.Minutes / 15) * 15; // normaliza al intervalo de 15 min más cercano
                _cboHoras.SelectedItem   = h12.ToString("D2");
                _cboMinutos.SelectedItem = minNorm.ToString("D2");
                _cboAmPm.SelectedItem    = t.Hours < 12 ? "AM" : "PM";

                _txtMotivo.Text           = cita.Motivo;
                _txtObservaciones.Text    = cita.Observaciones ?? string.Empty;

                if (_cboSede.Items.Count > 0)
                    _cboSede.SelectedValue = cita.SedeId;

                _btnGuardar.Visible      = false;
                _btnCancelarCita.Visible = true;
                _btnReprogramar.Visible  = true;
                _btnSeleccionarCliente.Enabled  = false;
                _btnSeleccionarVehiculo.Enabled = false;
            }
            finally
            {
                _cargando = false;
            }
        }

        // ─── Diálogos de selección ────────────────────────────────────────────

        private void AbrirSeleccionCliente(object? sender, EventArgs e)
        {
            var cadena  = ConfiguracionHelper.LeerCadenaConexion()!;
            var opciones = new DbContextOptionsBuilder<AutoCitaDbContext>().UseNpgsql(cadena).Options;
            var ctx     = new AutoCitaDbContext(opciones);
            var vm      = new ClienteViewModel(new ClienteRepositorio(ctx));
            var frm     = new FrmCliente(vm);

            frm.ClienteSeleccionado += (_, cliente) =>
            {
                _vm.VehiculoId    = 0;
                _vm.NombreCliente = cliente.NombreCompleto;
                _lblCliente.Text      = $"{cliente.NombreCompleto} | {cliente.DocumentoIdentidad}";
                _lblCliente.ForeColor = Color.Black;
                _lblVehiculo.Text     = "(Sin seleccionar)";
                _lblVehiculo.ForeColor = Color.Gray;
                _btnSeleccionarVehiculo.Enabled = true;
                _btnSeleccionarVehiculo.Tag     = cliente;
                frm.Close();
                ctx.Dispose();
            };

            frm.ShowDialog(this);
        }

        private void AbrirSeleccionVehiculo(object? sender, EventArgs e)
        {
            if (_btnSeleccionarVehiculo.Tag is not Cliente cliente) return;

            var cadena  = ConfiguracionHelper.LeerCadenaConexion()!;
            var opciones = new DbContextOptionsBuilder<AutoCitaDbContext>().UseNpgsql(cadena).Options;
            var ctx     = new AutoCitaDbContext(opciones);
            var vm      = new VehiculoViewModel(new VehiculoRepositorio(ctx));
            vm.ClienteId     = cliente.Id;
            vm.NombreCliente = cliente.NombreCompleto;

            var frm = new FrmVehiculo(vm);

            frm.VehiculoSeleccionado += (_, vehiculo) =>
            {
                _vm.VehiculoId    = vehiculo.Id;
                _vm.PlacaVehiculo = vehiculo.Placa;
                _lblVehiculo.Text      = $"{vehiculo.Placa} — {vehiculo.TipoVehiculo}";
                _lblVehiculo.ForeColor = Color.Black;
                _ = _vm.VerificarDisponibilidadAsync();
                frm.Close();
                ctx.Dispose();
            };

            frm.ShowDialog(this);
        }
    }
}
