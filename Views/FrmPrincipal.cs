using AutoCita.Data;
using AutoCita.Enums;
using AutoCita.Helpers;
using AutoCita.Repositories;
using AutoCita.Services;
using AutoCita.Strategies;
using AutoCita.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AutoCita.Views
{
    /// <summary>
    /// Formulario principal del sistema AutoCita.
    /// Contiene el menú lateral de navegación con visibilidad controlada por rol de usuario.
    /// </summary>
    public partial class FrmPrincipal : Form
    {
        private Panel          _pnlMenu        = new();
        private Panel          _pnlContenido   = new();
        private Label          _lblUsuario      = new();
        private Label          _lblRol          = new();
        private Button         _btnCitas        = new();
        private Button         _btnRecordatorio = new();
        private Button         _btnUsuarios     = new();
        private Button         _btnSedes        = new();
        private Button         _btnReportes     = new();
        private Button         _btnSalir        = new();
        private Label          _lblBadge        = new();
        private System.Windows.Forms.Timer _timerRecordatorio = new();

        private AutoCitaDbContext? _context;

        /// <summary>
        /// Constructor privado. Use <see cref="Crear"/> para instanciar.
        /// </summary>
        private FrmPrincipal() { }

        /// <summary>
        /// Crea una instancia del formulario principal inicializando el contexto de BD.
        /// </summary>
        public static FrmPrincipal Crear()
        {
            var frm = new FrmPrincipal();
            frm.InicializarContexto();
            frm.InicializarComponentes();
            frm.AjustarMenuPorRol();
            frm.IniciarTimerRecordatorio();
            return frm;
        }

        /// <summary>
        /// Inicializa el contexto de base de datos usando la cadena de conexión guardada.
        /// </summary>
        private void InicializarContexto()
        {
            var cadena = ConfiguracionHelper.LeerCadenaConexion()!;
            var opciones = new DbContextOptionsBuilder<AutoCitaDbContext>()
                .UseNpgsql(cadena)
                .Options;
            _context = new AutoCitaDbContext(opciones);
        }

        /// <summary>
        /// Construye y posiciona todos los controles del formulario principal.
        /// </summary>
        private void InicializarComponentes()
        {
            var usuario = SesionActual.Instancia.UsuarioLogueado!;

            Text            = "AutoCita — Gestión de citas";
            Size            = new Size(1100, 680);
            MinimumSize     = new Size(900, 600);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(245, 245, 250);
            Font            = new Font("Segoe UI", 9.5f);

            // ── Panel lateral ─────────────────────────────────────────────────
            _pnlMenu.Dock      = DockStyle.Left;
            _pnlMenu.Width     = 210;
            _pnlMenu.BackColor = Color.FromArgb(30, 100, 200);

            var lblLogo = new Label
            {
                Text      = "AutoCita",
                Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                Location  = new Point(15, 20),
                AutoSize  = true
            };

            _lblUsuario.Text      = usuario.Username;
            _lblUsuario.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            _lblUsuario.ForeColor = Color.FromArgb(200, 230, 255);
            _lblUsuario.Location  = new Point(15, 55);
            _lblUsuario.AutoSize  = true;

            _lblRol.Text      = usuario.Rol.ToString();
            _lblRol.ForeColor = Color.FromArgb(160, 200, 255);
            _lblRol.Location  = new Point(15, 72);
            _lblRol.AutoSize  = true;

            var separador = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Height      = 1,
                Location    = new Point(10, 100),
                Width       = 185
            };

            // Botones de navegación
            int yBtn = 115;
            ConstruirBotonMenu(_btnCitas,        "📅  Citas",          ref yBtn, AbrirCitas);
            ConstruirBotonMenu(_btnRecordatorio, "📞  Recordatorios",  ref yBtn, AbrirRecordatorio);
            ConstruirBotonMenu(_btnReportes,     "📊  Reportes",       ref yBtn, AbrirReportes);
            ConstruirBotonMenu(_btnUsuarios,     "👤  Usuarios",       ref yBtn, AbrirUsuarios);
            ConstruirBotonMenu(_btnSedes,        "🏢  Sedes",          ref yBtn, AbrirSedes);

            // Badge de recordatorio (sobre botón recordatorio)
            _lblBadge.Size      = new Size(22, 22);
            _lblBadge.Location  = new Point(170, _btnRecordatorio.Top + 8);
            _lblBadge.BackColor = Color.Red;
            _lblBadge.ForeColor = Color.White;
            _lblBadge.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            _lblBadge.TextAlign = ContentAlignment.MiddleCenter;
            _lblBadge.Visible   = false;

            // Botón salir
            _btnSalir.Text      = "⬅  Cerrar sesión";
            _btnSalir.Dock      = DockStyle.Bottom;
            _btnSalir.Height    = 42;
            _btnSalir.FlatStyle = FlatStyle.Flat;
            _btnSalir.FlatAppearance.BorderSize = 0;
            _btnSalir.ForeColor = Color.FromArgb(200, 220, 255);
            _btnSalir.Font      = new Font("Segoe UI", 9.5f);
            _btnSalir.TextAlign = ContentAlignment.MiddleLeft;
            _btnSalir.Padding   = new Padding(10, 0, 0, 0);
            _btnSalir.Click    += (_, _) => CerrarSesion();

            _pnlMenu.Controls.AddRange([lblLogo, _lblUsuario, _lblRol, separador,
                _btnCitas, _btnRecordatorio, _btnReportes, _btnUsuarios, _btnSedes,
                _lblBadge, _btnSalir]);

            // ── Panel de contenido ────────────────────────────────────────────
            _pnlContenido.Dock      = DockStyle.Fill;
            _pnlContenido.BackColor = Color.FromArgb(245, 245, 250);
            _pnlContenido.Padding   = new Padding(20);

            Controls.AddRange([_pnlContenido, _pnlMenu]);
        }

        /// <summary>
        /// Ajusta la visibilidad de los botones del menú según el rol del usuario logueado.
        /// </summary>
        private void AjustarMenuPorRol()
        {
            var rol = SesionActual.Instancia.UsuarioLogueado!.Rol;

            _btnUsuarios.Visible = rol == RolUsuario.Administrador;
            _btnSedes.Visible    = rol == RolUsuario.Administrador;
            _btnReportes.Visible = rol == RolUsuario.Administrador || rol == RolUsuario.Director;
        }

        /// <summary>
        /// Inicia el timer que verifica recordatorios cada 60 segundos.
        /// </summary>
        private void IniciarTimerRecordatorio()
        {
            _timerRecordatorio.Interval = 60_000;
            _timerRecordatorio.Tick    += async (_, _) => await VerificarRecordatoriosAsync();
            _timerRecordatorio.Start();
            // Verificar al arrancar también
            _ = VerificarRecordatoriosAsync();
        }

        /// <summary>
        /// Consulta citas pendientes de recordatorio y actualiza el badge visual.
        /// </summary>
        private async Task VerificarRecordatoriosAsync()
        {
            try
            {
                var servicio   = new RecordatorioServicio(new CitaRepositorio(_context!));
                var pendientes = await servicio.ObtenerCitasParaRecordatorioAsync();

                if (InvokeRequired) { Invoke(() => ActualizarBadge(pendientes.Count)); return; }
                ActualizarBadge(pendientes.Count);
            }
            catch { /* No bloquear la UI si falla el timer */ }
        }

        /// <summary>
        /// Actualiza el badge de recordatorios con el número de pendientes.
        /// </summary>
        /// <param name="cantidad">Número de citas pendientes de recordatorio.</param>
        private void ActualizarBadge(int cantidad)
        {
            _lblBadge.Visible = cantidad > 0;
            _lblBadge.Text    = cantidad > 9 ? "9+" : cantidad.ToString();
        }

        // ─── Navegación ───────────────────────────────────────────────────────

        /// <summary>Abre el módulo de gestión de citas.</summary>
        private void AbrirCitas()
        {
            var vm  = new ListaCitasViewModel(new CitaRepositorio(_context!));
            var frm = new FrmListaCitas(vm, _context!);
            MostrarEnContenido(frm);
        }

        /// <summary>Abre el módulo de recordatorios.</summary>
        private void AbrirRecordatorio()
        {
            var vm  = new RecordatorioViewModel(new RecordatorioServicio(new CitaRepositorio(_context!)));
            var frm = new FrmRecordatorio(vm);
            MostrarEnContenido(frm);
        }

        /// <summary>Abre el módulo de reportes.</summary>
        private void AbrirReportes()
        {
            var citaRepo  = new CitaRepositorio(_context!);
            var userRepo  = new UsuarioRepositorio(_context!);
            var sedeRepo  = new SedeRepositorio(_context!);
            var metaRepo  = new MetaProductividadRepositorio(_context!);
            var svc       = new ReporteServicio(new ReporteCitasPorAgenteStrategy(citaRepo));
            var vm        = new ReporteViewModel(svc, citaRepo, userRepo, sedeRepo, metaRepo);
            var frm       = new FrmReporte(vm);
            MostrarEnContenido(frm);
        }

        /// <summary>Abre el módulo de gestión de usuarios (solo Admin).</summary>
        private void AbrirUsuarios()
        {
            var vm  = new UsuarioViewModel(new UsuarioRepositorio(_context!), new SedeRepositorio(_context!));
            var frm = new FrmUsuario(vm);
            MostrarEnContenido(frm);
        }

        /// <summary>Abre el módulo de gestión de sedes (solo Admin).</summary>
        private void AbrirSedes()
        {
            var vm  = new SedeViewModel(new SedeRepositorio(_context!));
            var frm = new FrmSede(vm);
            MostrarEnContenido(frm);
        }

        /// <summary>
        /// Muestra un formulario hijo dentro del panel de contenido.
        /// </summary>
        /// <param name="frm">Formulario a mostrar.</param>
        private void MostrarEnContenido(Form frm)
        {
            foreach (Control ctrl in _pnlContenido.Controls)
                if (ctrl is Form f) { f.Hide(); }

            _pnlContenido.Controls.Clear();
            frm.TopLevel   = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock       = DockStyle.Fill;
            _pnlContenido.Controls.Add(frm);
            frm.Show();
        }

        /// <summary>
        /// Cierra la sesión actual y vuelve a la pantalla de Login.
        /// </summary>
        private void CerrarSesion()
        {
            _timerRecordatorio.Stop();
            SesionActual.Instancia.CerrarSesion();
            var cadena     = ConfiguracionHelper.LeerCadenaConexion()!;
            var opciones   = new DbContextOptionsBuilder<AutoCitaDbContext>().UseNpgsql(cadena).Options;
            var ctx        = new AutoCitaDbContext(opciones);
            var usuarioRepo = new UsuarioRepositorio(ctx);
            var authSvc    = new Services.AuthServicio(usuarioRepo);
            var loginVm    = new LoginViewModel(authSvc);
            var login      = new FrmLogin(loginVm);
            login.FormClosed += (_, _) => ctx.Dispose();
            login.Show();
            Close();
        }

        /// <summary>
        /// Helper para construir botones del menú lateral con estilo uniforme.
        /// </summary>
        private void ConstruirBotonMenu(Button btn, string texto, ref int y, Action onClick)
        {
            btn.Text      = texto;
            btn.Location  = new Point(0, y);
            btn.Size      = new Size(210, 44);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.White;
            btn.Font      = new Font("Segoe UI", 9.5f);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding   = new Padding(12, 0, 0, 0);
            btn.Click    += (_, _) => onClick();
            y += 46;
        }

        /// <summary>Libera los recursos del contexto al cerrar el formulario.</summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _timerRecordatorio.Stop();
            _context?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
