using AutoCita.ViewModels;

namespace AutoCita.Views
{
    /// <summary>
    /// Asistente de configuración inicial (onboarding) del sistema AutoCita.
    /// Guía al usuario en 3 pasos: conexión BD → creación de tablas → creación de usuario Admin.
    /// </summary>
    public partial class FrmOnboarding : Form
    {
        private readonly OnboardingViewModel _vm;

        // ─── Controles del Paso 1 ──────────────────────────────────────────────
        private Panel      _pnlPaso1       = new();
        private Label      _lblTitulo1     = new();
        private Label      _lblDesc1       = new();
        private Label      _lblCadena      = new();
        private TextBox    _txtCadena      = new();
        private Button     _btnValidar     = new();
        private Label      _lblEstado      = new();
        private Label      _lblError1      = new();

        // ─── Controles del Paso 2 (Admin) ─────────────────────────────────────
        private Panel      _pnlPaso2       = new();
        private Label      _lblTitulo2     = new();
        private Label      _lblDesc2       = new();
        private Label      _lblUsername    = new();
        private TextBox    _txtUsername    = new();
        private Label      _lblPassword    = new();
        private TextBox    _txtPassword    = new();
        private Label      _lblConfirmar   = new();
        private TextBox    _txtConfirmar   = new();
        private Button     _btnCrearAdmin  = new();
        private Label      _lblError2      = new();

        // ─── Controles del Paso 3 (Éxito) ─────────────────────────────────────
        private Panel      _pnlPaso3       = new();
        private Label      _lblTitulo3     = new();
        private Label      _lblDescExito   = new();
        private Button     _btnIrLogin     = new();

        /// <summary>
        /// Evento que se dispara cuando el onboarding finaliza y se debe mostrar el Login.
        /// </summary>
        public event EventHandler? OnboardingFinalizado;

        /// <summary>
        /// Constructor del formulario de onboarding.
        /// </summary>
        /// <param name="vm">ViewModel de onboarding.</param>
        public FrmOnboarding(OnboardingViewModel vm)
        {
            _vm = vm;
            _vm.PasoCambiado        += (_, paso) => MostrarPaso(paso);
            _vm.OnboardingCompletado += (_, _) => MostrarPaso(3);
            _vm.PropertyChanged      += (_, e) => ActualizarUi(e.PropertyName);

            InicializarComponentes();
            MostrarPaso(1);
        }

        /// <summary>
        /// Construye y posiciona todos los controles del formulario.
        /// </summary>
        private void InicializarComponentes()
        {
            Text            = "AutoCita — Configuración inicial";
            Size            = new Size(600, 440);
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            BackColor       = Color.FromArgb(245, 245, 250);
            Font            = new Font("Segoe UI", 9.5f);

            // ── Encabezado ────────────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 70,
                BackColor = Color.FromArgb(30, 100, 200)
            };
            var lblApp = new Label
            {
                Text      = "AutoCita",
                Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.White,
                Location  = new Point(20, 10),
                AutoSize  = true
            };
            var lblSub = new Label
            {
                Text      = "Asistente de configuración inicial",
                ForeColor = Color.FromArgb(200, 220, 255),
                Location  = new Point(22, 44),
                AutoSize  = true
            };
            pnlHeader.Controls.AddRange([lblApp, lblSub]);

            // ── Paso 1: Cadena de conexión ────────────────────────────────────
            _pnlPaso1.Location  = new Point(0, 70);
            _pnlPaso1.Size      = new Size(600, 370);
            _pnlPaso1.BackColor = Color.Transparent;

            _lblTitulo1.Text     = "Paso 1 — Conexión a la base de datos";
            _lblTitulo1.Font     = new Font("Segoe UI", 11f, FontStyle.Bold);
            _lblTitulo1.Location = new Point(30, 25);
            _lblTitulo1.AutoSize = true;

            _lblDesc1.Text      = "Ingrese la cadena de conexión al servidor PostgreSQL:";
            _lblDesc1.Location  = new Point(30, 60);
            _lblDesc1.AutoSize  = true;
            _lblDesc1.ForeColor = Color.Gray;

            _lblCadena.Text      = "Cadena de conexión:";
            _lblCadena.Location  = new Point(30, 95);
            _lblCadena.AutoSize  = true;

            _txtCadena.Location   = new Point(30, 118);
            _txtCadena.Size       = new Size(520, 28);
            _txtCadena.PlaceholderText = "Host=localhost;Port=5432;Database=autocita;Username=postgres;Password=...";
            _txtCadena.TextChanged += (_, _) => _vm.CadenaConexion = _txtCadena.Text;

            _btnValidar.Text      = "Probar conexión y continuar";
            _btnValidar.Location  = new Point(30, 165);
            _btnValidar.Size      = new Size(220, 36);
            _btnValidar.BackColor = Color.FromArgb(30, 100, 200);
            _btnValidar.ForeColor = Color.White;
            _btnValidar.FlatStyle = FlatStyle.Flat;
            _btnValidar.FlatAppearance.BorderSize = 0;
            _btnValidar.Click    += async (_, _) => await _vm.ValidarConexionAsync();

            _lblEstado.Location  = new Point(30, 215);
            _lblEstado.AutoSize  = true;
            _lblEstado.ForeColor = Color.FromArgb(30, 150, 30);

            _lblError1.Location  = new Point(30, 245);
            _lblError1.Size      = new Size(520, 60);
            _lblError1.ForeColor = Color.Red;

            _pnlPaso1.Controls.AddRange([_lblTitulo1, _lblDesc1, _lblCadena, _txtCadena,
                _btnValidar, _lblEstado, _lblError1]);

            // ── Paso 2: Crear Admin ───────────────────────────────────────────
            _pnlPaso2.Location  = new Point(0, 70);
            _pnlPaso2.Size      = new Size(600, 370);
            _pnlPaso2.BackColor = Color.Transparent;

            _lblTitulo2.Text     = "Paso 2 — Crear usuario administrador";
            _lblTitulo2.Font     = new Font("Segoe UI", 11f, FontStyle.Bold);
            _lblTitulo2.Location = new Point(30, 25);
            _lblTitulo2.AutoSize = true;

            _lblDesc2.Text      = "No existe ningún administrador. Cree el usuario inicial del sistema:";
            _lblDesc2.Location  = new Point(30, 60);
            _lblDesc2.AutoSize  = true;
            _lblDesc2.ForeColor = Color.Gray;

            ConstruirCampoAdmin(_lblUsername, _txtUsername, "Nombre de usuario:",    new Point(30, 98),  new Point(30, 120));
            ConstruirCampoAdmin(_lblPassword, _txtPassword, "Contraseña:",           new Point(30, 158), new Point(30, 180));
            ConstruirCampoAdmin(_lblConfirmar, _txtConfirmar, "Confirmar contraseña:", new Point(30, 218), new Point(30, 240));
            _txtPassword.PasswordChar  = '●';
            _txtConfirmar.PasswordChar = '●';

            _txtUsername.TextChanged  += (_, _) => _vm.UsernameAdmin    = _txtUsername.Text;
            _txtPassword.TextChanged  += (_, _) => _vm.PasswordAdmin    = _txtPassword.Text;
            _txtConfirmar.TextChanged += (_, _) => _vm.ConfirmarPassword = _txtConfirmar.Text;

            _btnCrearAdmin.Text      = "Crear administrador";
            _btnCrearAdmin.Location  = new Point(30, 285);
            _btnCrearAdmin.Size      = new Size(200, 36);
            _btnCrearAdmin.BackColor = Color.FromArgb(30, 100, 200);
            _btnCrearAdmin.ForeColor = Color.White;
            _btnCrearAdmin.FlatStyle = FlatStyle.Flat;
            _btnCrearAdmin.FlatAppearance.BorderSize = 0;
            _btnCrearAdmin.Click    += async (_, _) => await _vm.CrearAdminAsync();

            _lblError2.Location  = new Point(30, 330);
            _lblError2.Size      = new Size(520, 40);
            _lblError2.ForeColor = Color.Red;

            _pnlPaso2.Controls.AddRange([_lblTitulo2, _lblDesc2,
                _lblUsername, _txtUsername, _lblPassword, _txtPassword,
                _lblConfirmar, _txtConfirmar, _btnCrearAdmin, _lblError2]);

            // ── Paso 3: Éxito ─────────────────────────────────────────────────
            _pnlPaso3.Location  = new Point(0, 70);
            _pnlPaso3.Size      = new Size(600, 370);
            _pnlPaso3.BackColor = Color.Transparent;

            _lblTitulo3.Text     = "✔  Configuración completada";
            _lblTitulo3.Font     = new Font("Segoe UI", 14f, FontStyle.Bold);
            _lblTitulo3.ForeColor = Color.FromArgb(30, 150, 30);
            _lblTitulo3.Location = new Point(30, 60);
            _lblTitulo3.AutoSize = true;

            _lblDescExito.Text      = "El sistema está listo para usar.\nPuede iniciar sesión con su usuario administrador.";
            _lblDescExito.Location  = new Point(30, 110);
            _lblDescExito.AutoSize  = true;
            _lblDescExito.ForeColor = Color.Gray;

            _btnIrLogin.Text      = "Ir al inicio de sesión";
            _btnIrLogin.Location  = new Point(30, 180);
            _btnIrLogin.Size      = new Size(200, 36);
            _btnIrLogin.BackColor = Color.FromArgb(30, 100, 200);
            _btnIrLogin.ForeColor = Color.White;
            _btnIrLogin.FlatStyle = FlatStyle.Flat;
            _btnIrLogin.FlatAppearance.BorderSize = 0;
            _btnIrLogin.Click    += (_, _) => { OnboardingFinalizado?.Invoke(this, EventArgs.Empty); Close(); };

            _pnlPaso3.Controls.AddRange([_lblTitulo3, _lblDescExito, _btnIrLogin]);

            Controls.AddRange([pnlHeader, _pnlPaso1, _pnlPaso2, _pnlPaso3]);
        }

        /// <summary>
        /// Muestra el panel del paso indicado y oculta los demás.
        /// </summary>
        /// <param name="paso">Número del paso a mostrar (1, 2 o 3).</param>
        private void MostrarPaso(int paso)
        {
            if (InvokeRequired) { Invoke(() => MostrarPaso(paso)); return; }

            _pnlPaso1.Visible = paso == 1;
            _pnlPaso2.Visible = paso == 2;
            _pnlPaso3.Visible = paso == 3;
        }

        /// <summary>
        /// Actualiza los controles visuales en respuesta a cambios en el ViewModel.
        /// </summary>
        /// <param name="propiedad">Nombre de la propiedad que cambió.</param>
        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }

            switch (propiedad)
            {
                case nameof(OnboardingViewModel.MensajeEstado):
                    _lblEstado.Text = _vm.MensajeEstado;
                    break;
                case nameof(OnboardingViewModel.MensajeError):
                    if (_vm.PasoActual == 1) _lblError1.Text = _vm.MensajeError;
                    else                     _lblError2.Text = _vm.MensajeError;
                    break;
            }
        }

        /// <summary>
        /// Helper para construir un par Label + TextBox en el paso 2.
        /// </summary>
        private void ConstruirCampoAdmin(Label lbl, TextBox txt, string texto, Point posLbl, Point posTxt)
        {
            lbl.Text     = texto;
            lbl.Location = posLbl;
            lbl.AutoSize = true;

            txt.Location = posTxt;
            txt.Size     = new Size(300, 28);
        }
    }
}
