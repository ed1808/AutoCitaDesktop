using AutoCita.ViewModels;

namespace AutoCita.Views
{
    /// <summary>
    /// Pantalla de inicio de sesión del sistema AutoCita.
    /// </summary>
    public partial class FrmLogin : Form
    {
        private readonly LoginViewModel _vm;

        private TextBox _txtUsername = new();
        private TextBox _txtPassword = new();
        private Label   _lblError    = new();
        private Button  _btnIngresar = new();

        /// <summary>
        /// Constructor del formulario de login.
        /// </summary>
        /// <param name="vm">ViewModel de autenticación.</param>
        /// <summary>
        /// Se dispara cuando el login fue exitoso; permite que el llamador reaccione sin abrir FrmPrincipal automáticamente.
        /// </summary>
        public event EventHandler? LoginExitoso;

        /// <summary>
        /// Constructor del formulario de login.
        /// </summary>
        public FrmLogin(LoginViewModel vm)
        {
            _vm = vm;
            _vm.SesionIniciada  += (_, _) => { LoginExitoso?.Invoke(this, EventArgs.Empty); if (!IsDisposed && Visible) AbrirPrincipal(); };
            _vm.PropertyChanged += (_, e) => ActualizarUi(e.PropertyName);

            InicializarComponentes();
        }

        /// <summary>
        /// Construye y posiciona todos los controles del formulario.
        /// </summary>
        private void InicializarComponentes()
        {
            Text            = "AutoCita — Iniciar sesión";
            Size            = new Size(420, 480);
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            BackColor       = Color.FromArgb(245, 245, 250);
            Font            = new Font("Segoe UI", 10f);

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.FromArgb(30, 100, 200) };
            var lblApp    = new Label
            {
                Text      = "AutoCita",
                Font      = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.White,
                Location  = new Point(20, 12),
                AutoSize  = true
            };
            var lblSub = new Label
            {
                Text      = "Taller de mecánica automotriz",
                ForeColor = Color.FromArgb(200, 220, 255),
                Location  = new Point(22, 55),
                AutoSize  = true
            };
            pnlHeader.Controls.AddRange([lblApp, lblSub]);

            // Cuerpo del formulario
            var pnlBody = new Panel { Location = new Point(0, 90), Size = new Size(420, 390), BackColor = Color.Transparent };

            var lblTitulo = new Label
            {
                Text      = "Iniciar sesión",
                Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
                Location  = new Point(40, 30),
                AutoSize  = true
            };

            var lblUser = new Label { Text = "Usuario:", Location = new Point(40, 80), AutoSize = true };
            _txtUsername.Location       = new Point(40, 100);
            _txtUsername.Size           = new Size(320, 30);
            _txtUsername.PlaceholderText = "Ingrese su nombre de usuario";
            _txtUsername.TextChanged    += (_, _) => _vm.Username = _txtUsername.Text;
            _txtUsername.KeyDown        += TxtEnter_KeyDown;

            var lblPass = new Label { Text = "Contraseña:", Location = new Point(40, 150), AutoSize = true };
            _txtPassword.Location        = new Point(40, 170);
            _txtPassword.Size            = new Size(320, 30);
            _txtPassword.PasswordChar    = '●';
            _txtPassword.PlaceholderText = "Ingrese su contraseña";
            _txtPassword.TextChanged     += (_, _) => _vm.Password = _txtPassword.Text;
            _txtPassword.KeyDown         += TxtEnter_KeyDown;

            _lblError.Location  = new Point(40, 215);
            _lblError.Size      = new Size(320, 40);
            _lblError.ForeColor = Color.Red;

            _btnIngresar.Text      = "Ingresar";
            _btnIngresar.Location  = new Point(40, 265);
            _btnIngresar.Size      = new Size(320, 42);
            _btnIngresar.BackColor = Color.FromArgb(30, 100, 200);
            _btnIngresar.ForeColor = Color.White;
            _btnIngresar.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
            _btnIngresar.FlatStyle = FlatStyle.Flat;
            _btnIngresar.FlatAppearance.BorderSize = 0;
            _btnIngresar.Click    += async (_, _) => await _vm.IniciarSesionAsync();

            pnlBody.Controls.AddRange([lblTitulo, lblUser, _txtUsername,
                lblPass, _txtPassword, _lblError, _btnIngresar]);

            Controls.AddRange([pnlHeader, pnlBody]);
        }

        /// <summary>
        /// Permite iniciar sesión presionando Enter desde los campos de texto.
        /// </summary>
        private async void TxtEnter_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                await _vm.IniciarSesionAsync();
        }

        /// <summary>
        /// Actualiza los controles en respuesta a cambios en el ViewModel.
        /// </summary>
        private void ActualizarUi(string? propiedad)
        {
            if (InvokeRequired) { Invoke(() => ActualizarUi(propiedad)); return; }

            if (propiedad == nameof(LoginViewModel.MensajeError))
                _lblError.Text = _vm.MensajeError;

            if (propiedad == nameof(LoginViewModel.Cargando))
                _btnIngresar.Enabled = !_vm.Cargando;
        }

        /// <summary>
        /// Abre el formulario principal y cierra el login al autenticarse correctamente.
        /// </summary>
        private void AbrirPrincipal()
        {
            if (InvokeRequired) { Invoke(AbrirPrincipal); return; }

            var frm = FrmPrincipal.Crear();
            frm.Show();
            Hide();
            frm.FormClosed += (_, _) => Close();
        }
    }
}
