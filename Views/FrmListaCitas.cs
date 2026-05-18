using AutoCita.Data;
using AutoCita.Helpers;
using AutoCita.Models;
using AutoCita.Repositories;
using AutoCita.Services;
using AutoCita.Strategies;
using AutoCita.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AutoCita.Views
{
    /// <summary>
    /// Formulario de lista de citas agendadas.
    /// Permite crear nuevas citas y editar/gestionar las existentes.
    /// </summary>
    public partial class FrmListaCitas : Form
    {
        private readonly ListaCitasViewModel _vm;
        private readonly AutoCitaDbContext _context;

        private DataGridView _gridCitas = new();
        private Button _btnNuevaCita = new();
        private Label _lblError = new();
        private Label _lblTitulo = new();

        /// <summary>
        /// Constructor del formulario de lista de citas.
        /// </summary>
        /// <param name="vm">ViewModel de lista de citas.</param>
        /// <param name="context">Contexto de base de datos (para abrir subformularios).</param>
        public FrmListaCitas(ListaCitasViewModel vm, AutoCitaDbContext context)
        {
            _vm = vm;
            _context = context;
            _vm.PropertyChanged += (_, e) => ActualizarUi(e.PropertyName);
            InicializarComponentes();
            _ = InicializarAsync();
        }

        private async Task InicializarAsync()
        {
            await _vm.CargarCitasAsync();
            if (InvokeRequired) Invoke(RefrescarGrid);
            else RefrescarGrid();
        }

        private void InicializarComponentes()
        {
            Text = "Citas agendadas";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 245, 250);
            Font = new Font("Segoe UI", 9.5f);

            _lblTitulo.Text = "Citas agendadas";
            _lblTitulo.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            _lblTitulo.Location = new Point(20, 18);
            _lblTitulo.AutoSize = true;

            _btnNuevaCita.Text = "➕ Nueva cita";
            _btnNuevaCita.Location = new Point(820, 14);
            _btnNuevaCita.Size = new Size(150, 36);
            _btnNuevaCita.BackColor = Color.FromArgb(30, 150, 80);
            _btnNuevaCita.ForeColor = Color.White;
            _btnNuevaCita.FlatStyle = FlatStyle.Flat;
            _btnNuevaCita.FlatAppearance.BorderSize = 0;
            _btnNuevaCita.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            _btnNuevaCita.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnNuevaCita.Click += AbrirNuevaCita;

            _lblError.Location = new Point(20, 60);
            _lblError.Size = new Size(950, 24);
            _lblError.ForeColor = Color.Red;
            _lblError.Visible = false;
            _lblError.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _gridCitas.Location = new Point(20, 90);
            _gridCitas.Size = new Size(940, 450);
            _gridCitas.BackgroundColor = Color.White;
            _gridCitas.BorderStyle = BorderStyle.None;
            _gridCitas.AllowUserToAddRows = false;
            _gridCitas.AllowUserToDeleteRows = false;
            _gridCitas.ReadOnly = true;
            _gridCitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _gridCitas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _gridCitas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _gridCitas.RowHeadersVisible = false;
            _gridCitas.MultiSelect = false;
            _gridCitas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _gridCitas.DoubleClick += AbrirEdicionCita;

            ConfigurarColumnasGrid();

            Controls.AddRange([_lblTitulo, _btnNuevaCita, _lblError, _gridCitas]);
        }

        private void ConfigurarColumnasGrid()
        {
            _gridCitas.Columns.Clear();

            _gridCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                HeaderText = "Fecha/Hora",
                DataPropertyName = "FechaHora",
                FillWeight = 15
            });

            _gridCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cliente",
                HeaderText = "Cliente",
                FillWeight = 20
            });

            _gridCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Vehiculo",
                HeaderText = "Vehículo/Placa",
                FillWeight = 15
            });

            _gridCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Sede",
                HeaderText = "Sede",
                FillWeight = 15
            });

            _gridCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Motivo",
                HeaderText = "Motivo",
                FillWeight = 25
            });

            _gridCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                FillWeight = 10
            });
        }

        private void RefrescarGrid()
        {
            if (InvokeRequired) { Invoke(RefrescarGrid); return; }

            _gridCitas.Rows.Clear();

            foreach (var cita in _vm.Citas)
            {
                var fechaLocal = cita.FechaHora.ToLocalTime();
                var cliente = cita.Vehiculo?.Cliente?.NombreCompleto ?? "(desconocido)";
                var vehiculo = cita.Vehiculo != null
                    ? $"{cita.Vehiculo.Placa} — {cita.Vehiculo.TipoVehiculo}"
                    : "(desconocido)";
                var sede = cita.Sede?.Nombre ?? "(desconocida)";

                var idx = _gridCitas.Rows.Add(
                    fechaLocal.ToString("dd/MM/yyyy HH:mm"),
                    cliente,
                    vehiculo,
                    sede,
                    cita.Motivo,
                    cita.Estado.ToString()
                );

                _gridCitas.Rows[idx].Tag = cita;
            }
        }

        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }

            if (propiedad == nameof(ListaCitasViewModel.MensajeError))
            {
                _lblError.Text = _vm.MensajeError;
                _lblError.Visible = !string.IsNullOrEmpty(_vm.MensajeError);
            }

            if (propiedad == nameof(ListaCitasViewModel.Citas))
            {
                RefrescarGrid();
            }
        }

        private void AbrirNuevaCita(object? sender, EventArgs e)
        {
            var vm = new CitaViewModel(
                new CitaServicio(new CitaRepositorio(_context), new DisponibilidadVehiculoStrategy()),
                new SedeRepositorio(_context));
            var frm = new FrmCita(vm);

            frm.FormClosed += async (_, _) => await RecargarCitas();
            frm.ShowDialog(this);
        }

        private void AbrirEdicionCita(object? sender, EventArgs e)
        {
            if (_gridCitas.CurrentRow?.Tag is not Cita cita) return;

            var vm = new CitaViewModel(
                new CitaServicio(new CitaRepositorio(_context), new DisponibilidadVehiculoStrategy()),
                new SedeRepositorio(_context));
            var frm = new FrmCita(vm);
            frm.CargarCita(cita);

            frm.FormClosed += async (_, _) => await RecargarCitas();
            frm.ShowDialog(this);
        }

        private async Task RecargarCitas()
        {
            await _vm.CargarCitasAsync();
        }
    }
}
