using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using BoticaMarFarmaTransaccional.Pilas;
using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional
{
    public partial class Form1 : Form
    {
        // Estructuras enlazadas que gestionan el carrito, la navegación de auditoría y las campañas.
        private PilaDetalle pilaCarrito = new PilaDetalle();
        private ListaDobleTransacciones listaHistorial = new ListaDobleTransacciones();
        private ListaCircularCampanas carruselCampanas = new ListaCircularCampanas();

        // Datos en memoria: catálogos, numeración de registros y control del temporizador.
        private List<Cliente> listaClientes = new List<Cliente>();
        private List<Producto> listaProductos = new List<Producto>();
        private int contadorOperaciones = 1041;
        private int contadorCampanas = 5;
        private bool timerActivo = true;

        // Contenedores y botones del menú lateral y de las vistas principales.
        private Panel pnlSidebar;
        private Panel pnlContenedorPrincipal;
        private Button btnNavVenta;
        private Button btnNavHistorial;
        private Button btnNavAuditoria;
        private Button btnNavCarrusel;

        private Panel pnlVistaVenta;
        private Panel pnlVistaHistorial;
        private Panel pnlVistaAuditoria;
        private Panel pnlVistaCarrusel;

        // Controles de selección del cliente, del producto y de los datos de cobro.
        private Label lblBannerPromocionVenta;
        private RadioButton rbClienteExistente;
        private RadioButton rbClienteNuevo;
        private ComboBox cmbClientes;
        private TextBox txtNuevoClienteNombre;
        private TextBox txtNuevoClienteDni;
        private Button btnGuardarCliente;
        private Label lblClienteActivo;

        private RadioButton rbProdExistente;
        private RadioButton rbProdNuevo;
        private ComboBox cmbProductos;
        private TextBox txtNuevoProdNombre;
        private TextBox txtNuevoProdPrecio;
        private Button btnGuardarProducto;
        private NumericUpDown nudCantidad;
        private ComboBox cmbCanalCobro;
        private Button btnAgregarAlCarrito;

        private DataGridView dgvCarrito;
        private Button btnDeshacerPila;
        private Button btnLimpiarCarrito;
        private Label lblSubtotalVenta;
        private Label lblIgvVenta;
        private Label lblTotalVenta;
        private Button btnProcesarCobro;

        // Controles para mostrar la tabla y las métricas del historial.
        private DataGridView dgvHistorialCompleto;
        private Label lblMetricaTotalOperaciones;
        private Label lblMetricaRecaudacionCaja;

        // Controles para navegar por las transacciones y mostrar su comprobante.
        private Button btnAuditoriaAtras;
        private Button btnAuditoriaAdelante;
        private Label lblTicketAuditado;
        private Label lblDetalleAuditoria;

        // Controles para mostrar, recorrer e incorporar campañas al carrusel.
        private Panel pnlTarjetonAnuncio;
        private Label lblCardTitulo;
        private Label lblCardBeneficio;
        private Label lblCardCanalYVigencia;
        private Button btnAvanzarPromoManual;
        private Button btnPausarReanudarTimer;
        private ListBox lstTrazasAnillo;
        private TextBox txtNuevaPromoTitulo;
        private TextBox txtNuevaPromoBeneficio;
        private ComboBox cmbNuevaPromoCanal;
        private DateTimePicker dtpVigenciaPromo;
        private Button btnAgregarPromoAnillo;
        private System.Windows.Forms.Timer timerCarrusel;

        // Configura la ventana, crea las vistas, carga datos, enlaza eventos e inicia el carrusel.
        public Form1()
        {
            this.Text = "SISTEMA TRANSACCIONAL - BOTICA MARFARMA [CIIN1190P - EVALUACIÓN PARCIAL]";
            this.Size = new Size(1320, 800);
            this.MinimumSize = new Size(1200, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 246, 250);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            ConstruirSidebar();
            ConstruirVistaVenta();
            ConstruirVistaHistorial();
            ConstruirVistaAuditoria();
            ConstruirVistaCarrusel();

            CargarDatosSemilla();
            VincularEventos();

            timerCarrusel = new System.Windows.Forms.Timer { Interval = 3500 };
            timerCarrusel.Tick += (s, e) => AvanzarCarruselCircular();
            timerCarrusel.Start();

            ActualizarBannerEnPantalla();
            CambiarVista(pnlVistaVenta, btnNavVenta);
        }

        // Construye el menú lateral y el contenedor donde se muestran las distintas vistas.
        private void ConstruirSidebar()
        {
            pnlSidebar = new Panel
            {
                Size = new Size(270, 800),
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(34, 47, 62)
            };

            Label lblTit = new Label
            {
                Text = "BOTICA MARFARMA",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 20),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                Text = "Agente BCP & Billeteras Móviles",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(178, 190, 195),
                Location = new Point(15, 45),
                AutoSize = true
            };

            Panel sep = new Panel { Location = new Point(15, 95), Size = new Size(240, 1), BackColor = Color.FromArgb(87, 101, 116) };

            btnNavVenta = CrearBotonMenu(" 1. Nueva Venta ", 110);
            btnNavHistorial = CrearBotonMenu(" 2. Historial General", 170);
            btnNavAuditoria = CrearBotonMenu(" 3. Auditoría Caja ", 230);
            btnNavCarrusel = CrearBotonMenu(" 4. Carrusel Promos ", 290);

            pnlSidebar.Controls.AddRange(new Control[] { lblTit, lblSub, sep, btnNavVenta, btnNavHistorial, btnNavAuditoria, btnNavCarrusel });

            pnlContenedorPrincipal = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 246, 250) };
            this.Controls.Add(pnlContenedorPrincipal);
            this.Controls.Add(pnlSidebar);
        }

        // Crea un botón del menú con un estilo común y la posición vertical indicada.
        private Button CrearBotonMenu(string texto, int y)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(10, y),
                Size = new Size(250, 48),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(223, 230, 233),
                BackColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // Oculta las vistas anteriores, resalta la opción seleccionada y muestra su panel.
        private void CambiarVista(Panel destino, Button btnActivo)
        {
            pnlVistaVenta.Visible = false;
            pnlVistaHistorial.Visible = false;
            pnlVistaAuditoria.Visible = false;
            pnlVistaCarrusel.Visible = false;

            Button[] btns = { btnNavVenta, btnNavHistorial, btnNavAuditoria, btnNavCarrusel };
            foreach (var b in btns) { b.BackColor = Color.FromArgb(44, 62, 80); b.ForeColor = Color.FromArgb(223, 230, 233); }

            btnActivo.BackColor = Color.FromArgb(41, 128, 185);
            btnActivo.ForeColor = Color.White;
            destino.Visible = true;
            destino.BringToFront();
        }

        // Crea los controles para seleccionar cliente y producto, gestionar el carrito y cobrar.
        private void ConstruirVistaVenta()
        {
            pnlVistaVenta = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25) };

            lblBannerPromocionVenta = new Label
            {
                Text = " CAMPAÑA DEL DÍA: Cargando promociones cíclicas...",
                Location = new Point(25, 12),
                Size = new Size(975, 34),
                BackColor = Color.FromArgb(254, 249, 231),
                ForeColor = Color.FromArgb(183, 119, 0),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            GroupBox grpEntradas = new GroupBox
            {
                Text = "Datos de Atención al Paciente",
                Location = new Point(25, 55),
                Size = new Size(420, 520)
            };

            Label lblSecCli = new Label { Text = "1. Paciente / Cliente:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(15, 22), AutoSize = true };
            rbClienteExistente = new RadioButton { Text = "Cliente Registrado", Location = new Point(15, 42), AutoSize = true, Checked = true };
            rbClienteNuevo = new RadioButton { Text = "Nuevo Cliente", Location = new Point(160, 42), AutoSize = true };

            cmbClientes = new ComboBox { Location = new Point(15, 68), Size = new Size(385, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            txtNuevoClienteNombre = new TextBox { Location = new Point(15, 68), Size = new Size(180, 23), PlaceholderText = "Nombre completo", Visible = false };
            txtNuevoClienteDni = new TextBox { Location = new Point(200, 68), Size = new Size(95, 23), PlaceholderText = "DNI", Visible = false };
            btnGuardarCliente = new Button { Text = "Guardar", Location = new Point(300, 67), Size = new Size(100, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Visible = false };

            lblClienteActivo = new Label
            {
                Text = "Seleccione un paciente",
                Location = new Point(15, 98),
                Size = new Size(385, 24),
                BackColor = Color.FromArgb(240, 243, 244),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblSecMed = new Label { Text = "2. Selección de Medicamento:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(15, 135), AutoSize = true };
            rbProdExistente = new RadioButton { Text = "Del Catálogo", Location = new Point(15, 155), AutoSize = true, Checked = true };
            rbProdNuevo = new RadioButton { Text = "Crear Nuevo", Location = new Point(160, 155), AutoSize = true };

            cmbProductos = new ComboBox { Location = new Point(15, 180), Size = new Size(385, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            txtNuevoProdNombre = new TextBox { Location = new Point(15, 180), Size = new Size(180, 23), PlaceholderText = "Medicamento", Visible = false };
            txtNuevoProdPrecio = new TextBox { Location = new Point(200, 180), Size = new Size(95, 23), PlaceholderText = "Precio S/", Visible = false };
            btnGuardarProducto = new Button { Text = "Guardar", Location = new Point(300, 179), Size = new Size(100, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Visible = false };

            Label lCant = new Label { Text = "Cantidad:", Location = new Point(15, 220), AutoSize = true };
            nudCantidad = new NumericUpDown { Location = new Point(15, 240), Size = new Size(80, 23), Minimum = 1, Maximum = 100, Value = 1 };

            Label lCanal = new Label { Text = "Canal de Cobro:", Location = new Point(120, 220), AutoSize = true };
            cmbCanalCobro = new ComboBox { Location = new Point(120, 240), Size = new Size(280, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCanalCobro.Items.AddRange(new object[] { "Yape (Billetera Móvil)", "BCP QR Interoperable", "Agente BCP (Depósito)", "Efectivo Mostrador" });
            cmbCanalCobro.SelectedIndex = 0;

            btnAgregarAlCarrito = new Button
            {
                Text = " + Añadir",
                Location = new Point(15, 290),
                Size = new Size(385, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            grpEntradas.Controls.AddRange(new Control[] {
                lblSecCli, rbClienteExistente, rbClienteNuevo, cmbClientes, txtNuevoClienteNombre, txtNuevoClienteDni, btnGuardarCliente, lblClienteActivo,
                lblSecMed, rbProdExistente, rbProdNuevo, cmbProductos, txtNuevoProdNombre, txtNuevoProdPrecio, btnGuardarProducto,
                lCant, nudCantidad, lCanal, cmbCanalCobro, btnAgregarAlCarrito
            });

            GroupBox grpCarrito = new GroupBox
            {
                
                Location = new Point(460, 55),
                Size = new Size(540, 520)
            };

            dgvCarrito = new DataGridView
            {
                Location = new Point(15, 25),
                Size = new Size(510, 260),
                BackgroundColor = Color.White,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvCarrito.Columns.Add("Prod", "Medicamento");
            dgvCarrito.Columns.Add("Cant", "Cant.");
            dgvCarrito.Columns.Add("Precio", "P. Unit");
            dgvCarrito.Columns.Add("Sub", "Subtotal");

            btnDeshacerPila = new Button
            {
                Text = "<- Deshacer Último Medicamento",
                Location = new Point(15, 295),
                Size = new Size(360, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiarCarrito = new Button
            {
                Text = "Vaciar",
                Location = new Point(385, 295),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            lblSubtotalVenta = new Label { Text = "Subtotal: S/ 0.00", Location = new Point(350, 350), AutoSize = true };
            lblIgvVenta = new Label { Text = "IGV (18%): S/ 0.00", Location = new Point(350, 372), AutoSize = true };
            lblTotalVenta = new Label { Text = "TOTAL: S/ 0.00", Location = new Point(350, 395), AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(192, 57, 43) };

            btnProcesarCobro = new Button
            {
                Text = " PROCESAR VENTA / COBRO",
                Location = new Point(15, 435),
                Size = new Size(510, 55),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            grpCarrito.Controls.AddRange(new Control[] {
                dgvCarrito, btnDeshacerPila, btnLimpiarCarrito,
                lblSubtotalVenta, lblIgvVenta, lblTotalVenta, btnProcesarCobro
            });

            pnlVistaVenta.Controls.AddRange(new Control[] { lblBannerPromocionVenta, grpEntradas, grpCarrito });
            pnlContenedorPrincipal.Controls.Add(pnlVistaVenta);
        }

        // Prepara la tabla cronológica de ventas y los indicadores de operaciones y recaudación.
        private void ConstruirVistaHistorial()
        {
            pnlVistaHistorial = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), Visible = false };

            Label lblTit = new Label { Text = "HISTORIAL GENERAL DE OPERACIONES ", Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(25, 15), AutoSize = true };

            lblMetricaTotalOperaciones = new Label { Text = "Operaciones Totales: 0", Location = new Point(25, 55), Size = new Size(250, 40), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
            lblMetricaRecaudacionCaja = new Label { Text = "Caja Total: S/ 0.00", Location = new Point(290, 55), Size = new Size(250, 40), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(39, 174, 96) };

            dgvHistorialCompleto = new DataGridView
            {
                Location = new Point(25, 110),
                Size = new Size(980, 480),
                BackgroundColor = Color.White,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvHistorialCompleto.Columns.Add("Cod", "Código");
            dgvHistorialCompleto.Columns.Add("Fecha", "Hora");
            dgvHistorialCompleto.Columns.Add("Cliente", "Paciente / Cliente");
            dgvHistorialCompleto.Columns.Add("Canal", "Canal Cobro");
            dgvHistorialCompleto.Columns.Add("Total", "Monto Total");
            dgvHistorialCompleto.Columns.Add("Estado", "Estado");
            dgvHistorialCompleto.Columns.Add("Items", "Medicamentos");

            pnlVistaHistorial.Controls.AddRange(new Control[] { lblTit, lblMetricaTotalOperaciones, lblMetricaRecaudacionCaja, dgvHistorialCompleto });
            pnlContenedorPrincipal.Controls.Add(pnlVistaHistorial);
        }

        // Prepara la navegación bidireccional y el área de visualización del comprobante.
        private void ConstruirVistaAuditoria()
        {
            pnlVistaAuditoria = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), Visible = false };

            Label lblTit = new Label { Text = "AUDITORÍA Y ARQUEO BIDIRECCIONAL ", Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(25, 15), AutoSize = true };

            btnAuditoriaAtras = new Button { Text = "<- Operación Anterior ", Location = new Point(25, 55), Size = new Size(290, 45), BackColor = Color.FromArgb(52, 73, 94), ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnAuditoriaAdelante = new Button { Text = "Operación Siguiente -> ", Location = new Point(330, 55), Size = new Size(290, 45), BackColor = Color.FromArgb(52, 73, 94), ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), FlatStyle = FlatStyle.Flat };

            GroupBox grpTicket = new GroupBox { Text = "", Location = new Point(25, 115), Size = new Size(980, 460) };
            lblTicketAuditado = new Label { Text = "Sin transacciones auditadas.", Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 128, 185), Location = new Point(20, 25), Size = new Size(940, 30) };
            lblDetalleAuditoria = new Label { Text = "Aún no se han procesado ventas.", Font = new Font("Consolas", 10F), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Location = new Point(20, 65), Size = new Size(940, 370) };

            grpTicket.Controls.AddRange(new Control[] { lblTicketAuditado, lblDetalleAuditoria });
            pnlVistaAuditoria.Controls.AddRange(new Control[] { lblTit, btnAuditoriaAtras, btnAuditoriaAdelante, grpTicket });
            pnlContenedorPrincipal.Controls.Add(pnlVistaAuditoria);
        }

        // Construye la tarjeta promocional, el formulario de alta y la inspección del anillo.
        private void ConstruirVistaCarrusel()
        {
            pnlVistaCarrusel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), Visible = false };

            Label lblTit = new Label
            {
                Text = "CARRUSEL DE CAMPAÑAS Y PROMOCIONES ",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(25, 15),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                //Text = "Estructura en anillo: cola.Siguiente enlaza directamente con cabeza, garantizando rotación en tiempo constante O(1).",
                //Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                //ForeColor = Color.FromArgb(127, 140, 141),
                //Location = new Point(27, 40),
                //AutoSize = true
            };

            pnlTarjetonAnuncio = new Panel
            {
                Location = new Point(25, 70),
                Size = new Size(540, 240),
                BackColor = Color.FromArgb(41, 128, 185),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblCardTitulo = new Label { Text = "CAMPAÑA: Cargando...", Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 15), Size = new Size(500, 35) };
            lblCardBeneficio = new Label { Text = "Beneficio: ...", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(241, 196, 15), Location = new Point(20, 55), Size = new Size(500, 35) };
            lblCardCanalYVigencia = new Label { Text = "Canal: ... | Vigencia: ...", Font = new Font("Segoe UI", 9.5F), ForeColor = Color.FromArgb(236, 240, 241), Location = new Point(20, 100), Size = new Size(500, 45) };

            btnAvanzarPromoManual = new Button
            {
                Text = "Siguiente Anuncio -> ",
                Location = new Point(20, 170),
                Size = new Size(260, 45),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(41, 128, 185),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnPausarReanudarTimer = new Button
            {
                Text = " Pausar ",
                Location = new Point(290, 170),
                Size = new Size(150, 45),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            pnlTarjetonAnuncio.Controls.AddRange(new Control[] {
                lblCardTitulo, lblCardBeneficio, lblCardCanalYVigencia, btnAvanzarPromoManual, btnPausarReanudarTimer
            });

            GroupBox grpNuevaPromo = new GroupBox
            {
                Text = "Añadir Nueva Campaña al Anillo Circular",
                Location = new Point(585, 70),
                Size = new Size(420, 240)
            };

            Label lp1 = new Label { Text = "Título Campaña:", Location = new Point(15, 20), AutoSize = true };
            txtNuevaPromoTitulo = new TextBox { Location = new Point(15, 38), Size = new Size(385, 23) };

            Label lp2 = new Label { Text = "Beneficio / Descuento:", Location = new Point(15, 65), AutoSize = true };
            txtNuevaPromoBeneficio = new TextBox { Location = new Point(15, 83), Size = new Size(385, 23) };

            Label lp3 = new Label { Text = "Canal Cobro:", Location = new Point(15, 110), AutoSize = true };
            cmbNuevaPromoCanal = new ComboBox { Location = new Point(15, 128), Size = new Size(185, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbNuevaPromoCanal.Items.AddRange(new object[] { "Yape", "BCP QR", "Agente BCP", "General" });
            cmbNuevaPromoCanal.SelectedIndex = 0;

            Label lp4 = new Label { Text = "Válido Hasta:", Location = new Point(215, 110), AutoSize = true };
            dtpVigenciaPromo = new DateTimePicker
            {
                Location = new Point(215, 128),
                Size = new Size(185, 23),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(15)
            };

            btnAgregarPromoAnillo = new Button
            {
                Text = " + Insertar en el Anillo Circular",
                Location = new Point(15, 175),
                Size = new Size(385, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            grpNuevaPromo.Controls.AddRange(new Control[] {
                lp1, txtNuevaPromoTitulo, lp2, txtNuevaPromoBeneficio,
                lp3, cmbNuevaPromoCanal, lp4, dtpVigenciaPromo, btnAgregarPromoAnillo
            });

            Label lblTrazas = new Label
            {
                Text = "Inspección de Enlaces",
                Location = new Point(25, 325),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            lstTrazasAnillo = new ListBox
            {
                Location = new Point(25, 350),
                Size = new Size(980, 230),
                Font = new Font("Consolas", 10F),
                BackColor = Color.White
            };

            pnlVistaCarrusel.Controls.AddRange(new Control[] {
                lblTit, lblSub, pnlTarjetonAnuncio, grpNuevaPromo, lblTrazas, lstTrazasAnillo
            });
            pnlContenedorPrincipal.Controls.Add(pnlVistaCarrusel);
        }

        // Carga clientes, productos y campañas iniciales y actualiza los selectores.
        private void CargarDatosSemilla()
        {
            listaClientes.Add(new Cliente("72458912", "Rosmery Mendoza"));
            listaClientes.Add(new Cliente("43891245", "Carlos Alcantara"));
            listaClientes.Add(new Cliente("70123456", "Maria Fernandez"));
            RefrescarComboClientes();

            listaProductos.Add(new Producto("MED-01", "Paracetamol 500mg", 12.00m));
            listaProductos.Add(new Producto("MED-02", "Amoxicilina Jarabe", 18.50m));
            listaProductos.Add(new Producto("MED-03", "Ibuprofeno 400mg", 8.50m));
            listaProductos.Add(new Producto("MED-04", "Alcohol 70° 500ml", 6.00m));
            RefrescarComboProductos();

            carruselCampanas.InsertarAlFinal(new CampanaSalud("CAMP-01", "Despistaje de Presión Arterial", "Medición gratuita por compras > S/ 20", "General", DateTime.Now.AddDays(7)));
            carruselCampanas.InsertarAlFinal(new CampanaSalud("CAMP-02", "Miércoles de Yape", "15% descuento en jarabes pediátricos", "Yape", DateTime.Now.AddDays(14)));
            carruselCampanas.InsertarAlFinal(new CampanaSalud("CAMP-03", "Campaña Dental MarFarma", "2x1 en pastas y cepillos dentales", "BCP QR", DateTime.Now.AddDays(30)));
            carruselCampanas.InsertarAlFinal(new CampanaSalud("CAMP-04", "Botiquín Seguro Cajamarca", "10% desc. en alcohol y gasas", "General", DateTime.Now.AddDays(60)));
        }

        // Vuelve a cargar el selector de clientes y deja seleccionado el primero si existe.
        private void RefrescarComboClientes()
        {
            cmbClientes.Items.Clear();
            foreach (var c in listaClientes) cmbClientes.Items.Add(c);
            if (cmbClientes.Items.Count > 0) cmbClientes.SelectedIndex = 0;
        }

        // Vuelve a cargar el selector de productos y deja seleccionado el primero si existe.
        private void RefrescarComboProductos()
        {
            cmbProductos.Items.Clear();
            foreach (var p in listaProductos) cmbProductos.Items.Add(p);
            if (cmbProductos.Items.Count > 0) cmbProductos.SelectedIndex = 0;
        }

        // Asocia acciones de botones y cambios de selección con las operaciones de la aplicación.
        private void VincularEventos()
        {
            btnNavVenta.Click += (s, e) => CambiarVista(pnlVistaVenta, btnNavVenta);
            btnNavHistorial.Click += (s, e) => { ActualizarGrillaHistorial(); CambiarVista(pnlVistaHistorial, btnNavHistorial); };
            btnNavAuditoria.Click += (s, e) => { RefrescarAuditoriaVisual(); CambiarVista(pnlVistaAuditoria, btnNavAuditoria); };
            btnNavCarrusel.Click += (s, e) => { RefrescarVistaCarrusel(); CambiarVista(pnlVistaCarrusel, btnNavCarrusel); };

            rbClienteExistente.CheckedChanged += (s, e) =>
            {
                bool modo = rbClienteExistente.Checked;
                cmbClientes.Visible = modo;
                txtNuevoClienteNombre.Visible = !modo;
                txtNuevoClienteDni.Visible = !modo;
                btnGuardarCliente.Visible = !modo;
            };

            cmbClientes.SelectedIndexChanged += (s, e) =>
            {
                if (cmbClientes.SelectedItem is Cliente c)
                    lblClienteActivo.Text = $"Atendiendo a: {c.NombreCompleto} | DNI: {c.Dni}";
            };

            btnGuardarCliente.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNuevoClienteNombre.Text) || string.IsNullOrWhiteSpace(txtNuevoClienteDni.Text))
                {
                    MessageBox.Show("Complete el nombre y DNI del paciente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Cliente nuevo = new Cliente(txtNuevoClienteDni.Text.Trim(), txtNuevoClienteNombre.Text.Trim());
                listaClientes.Add(nuevo);
                RefrescarComboClientes();
                cmbClientes.SelectedItem = nuevo;
                rbClienteExistente.Checked = true;
                txtNuevoClienteNombre.Clear();
                txtNuevoClienteDni.Clear();
            };

            rbProdExistente.CheckedChanged += (s, e) =>
            {
                bool modo = rbProdExistente.Checked;
                cmbProductos.Visible = modo;
                txtNuevoProdNombre.Visible = !modo;
                txtNuevoProdPrecio.Visible = !modo;
                btnGuardarProducto.Visible = !modo;
            };

            btnGuardarProducto.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNuevoProdNombre.Text) || !decimal.TryParse(txtNuevoProdPrecio.Text, out decimal precio))
                {
                    MessageBox.Show("Ingrese un medicamento válido y su precio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Producto nuevo = new Producto($"MED-{listaProductos.Count + 1:D2}", txtNuevoProdNombre.Text.Trim(), precio);
                listaProductos.Add(nuevo);
                RefrescarComboProductos();
                cmbProductos.SelectedItem = nuevo;
                rbProdExistente.Checked = true;
                txtNuevoProdNombre.Clear();
                txtNuevoProdPrecio.Clear();
            };

            btnAgregarAlCarrito.Click += (s, e) =>
            {
                if (cmbProductos.SelectedItem is Producto prod)
                {
                    int cant = (int)nudCantidad.Value;
                    DetalleVenta item = new DetalleVenta(prod, cant);
                    pilaCarrito.Push(item); // O(1)
                    ActualizarCarritoVisual();
                }
            };

            btnDeshacerPila.Click += (s, e) =>
            {
                DetalleVenta retirado = pilaCarrito.Pop(); // O(1)
                if (retirado != null)
                {
                    MessageBox.Show($"[PILA POP O(1)]: Se anuló '{retirado.ProductoItem.Nombre}' del carrito.", "Deshacer Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ActualizarCarritoVisual();
                }
                else
                {
                    MessageBox.Show("No hay medicamentos para deshacer.", "Pila Vacía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnLimpiarCarrito.Click += (s, e) => { pilaCarrito.Limpiar(); ActualizarCarritoVisual(); };

            btnProcesarCobro.Click += (s, e) =>
            {
                if (pilaCarrito.EstaVacia())
                {
                    MessageBox.Show("El carrito de venta se encuentra vacío.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Cliente cli = cmbClientes.SelectedItem as Cliente;
                string canal = cmbCanalCobro.SelectedItem.ToString();
                decimal total = CalcularTotal();

                string resumen = "";
                foreach (var d in pilaCarrito.ObtenerElementos())
                    resumen += $"{d.Cantidad}x {d.ProductoItem.Nombre}; ";

                string codigoOp = $"OP-{contadorOperaciones++}";
                TransaccionVenta nueva = new TransaccionVenta(codigoOp, cli, canal, total, resumen);

                listaHistorial.InsertarAlFinal(nueva); // Inserción dinámica O(1)
                MessageBox.Show($"Transacción {codigoOp} procesada vía {canal}.\nMonto: S/ {total:F2}", "Venta Confirmada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pilaCarrito.Limpiar();
                ActualizarCarritoVisual();
            };

            btnAuditoriaAtras.Click += (s, e) => {
                TransaccionVenta anterior = listaHistorial.Retroceder();
                RefrescarAuditoriaVisual();
                if (anterior == null) MessageBox.Show("Inicio del registro alcanzado.", "Auditoría", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnAuditoriaAdelante.Click += (s, e) => {
                TransaccionVenta siguiente = listaHistorial.Avanzar();
                RefrescarAuditoriaVisual();
                if (siguiente == null) MessageBox.Show("Última operación alcanzada.", "Auditoría", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnAvanzarPromoManual.Click += (s, e) => AvanzarCarruselCircular();

            btnPausarReanudarTimer.Click += (s, e) =>
            {
                timerActivo = !timerActivo;
                if (timerActivo)
                {
                    timerCarrusel.Start();
                    btnPausarReanudarTimer.Text = " Pausar Timer";
                    btnPausarReanudarTimer.BackColor = Color.FromArgb(52, 73, 94);
                }
                else
                {
                    timerCarrusel.Stop();
                    btnPausarReanudarTimer.Text = " Reanudar Timer";
                    btnPausarReanudarTimer.BackColor = Color.FromArgb(39, 174, 96);
                }
            };

            btnAgregarPromoAnillo.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNuevaPromoTitulo.Text) || string.IsNullOrWhiteSpace(txtNuevaPromoBeneficio.Text))
                {
                    MessageBox.Show("Complete el título y beneficio de la campaña.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CampanaSalud nueva = new CampanaSalud(
                    $"CAMP-{contadorCampanas++:D2}",
                    txtNuevaPromoTitulo.Text.Trim(),
                    txtNuevaPromoBeneficio.Text.Trim(),
                    cmbNuevaPromoCanal.SelectedItem.ToString(),
                    dtpVigenciaPromo.Value
                );

                carruselCampanas.InsertarAlFinal(nueva); // O(1)
                RefrescarVistaCarrusel();

                txtNuevaPromoTitulo.Clear();
                txtNuevaPromoBeneficio.Clear();
                MessageBox.Show($"Campaña '{nueva.Titulo}' añadida al anillo circular con vigencia hasta {nueva.FechaVigenciaFin:dd/MM/yyyy}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
        }

        // Mueve la campaña actual al siguiente nodo del anillo y refresca sus representaciones.
        private void AvanzarCarruselCircular()
        {
            CampanaSalud promo = carruselCampanas.AvanzarSiguiente(); // O(1)
            ActualizarBannerEnPantalla();
            if (pnlVistaCarrusel.Visible)
            {
                RefrescarVistaCarrusel();
            }
        }

        // Copia los datos de la campaña actual a los banners de venta y del carrusel.
        private void ActualizarBannerEnPantalla()
        {
            CampanaSalud promo = carruselCampanas.ObtenerActual();
            if (promo != null)
            {
                lblBannerPromocionVenta.Text = $" [{promo.CanalAsociado}] {promo.Titulo}: {promo.BeneficioDescuento} (Válido hasta: {promo.FechaVigenciaFin:dd/MM/yyyy})";
                lblCardTitulo.Text = $"CAMPAÑA: {promo.Titulo}";
                lblCardBeneficio.Text = $"Beneficio: {promo.BeneficioDescuento}";
                lblCardCanalYVigencia.Text = $"Canal: {promo.CanalAsociado} | Registrado: {promo.FechaRegistro:dd/MM/yyyy HH:mm} | Vence: {promo.FechaVigenciaFin:dd/MM/yyyy}";
            }
        }

        // Actualiza la tarjeta actual y reconstruye la lista de enlaces del anillo.
        private void RefrescarVistaCarrusel()
        {
            ActualizarBannerEnPantalla();
            lstTrazasAnillo.Items.Clear();
            foreach (var traza in carruselCampanas.ObtenerTrazasDelAnillo())
                lstTrazasAnillo.Items.Add(traza);
        }

        // Reconstruye la tabla del carrito y calcula subtotal, IGV y total para mostrarlos.
        private void ActualizarCarritoVisual()
        {
            dgvCarrito.Rows.Clear();
            decimal sub = 0;
            foreach (var d in pilaCarrito.ObtenerElementos())
            {
                dgvCarrito.Rows.Add(d.ProductoItem.Nombre, d.Cantidad, $"S/ {d.ProductoItem.PrecioUnitario:F2}", $"S/ {d.Subtotal:F2}");
                sub += d.Subtotal;
            }
            decimal igv = sub * 0.18m;
            decimal tot = sub + igv;
            lblSubtotalVenta.Text = $"Subtotal: S/ {sub:F2}";
            lblIgvVenta.Text = $"IGV (18%): S/ {igv:F2}";
            lblTotalVenta.Text = $"TOTAL: S/ {tot:F2}";
        }

        // Suma los subtotales de la pila y devuelve el total incluyendo el 18 % de IGV.
        private decimal CalcularTotal()
        {
            decimal sub = 0;
            foreach (var d in pilaCarrito.ObtenerElementos()) sub += d.Subtotal;
            return sub * 1.18m;
        }

        // Vuelca las transacciones en la tabla y actualiza el recuento y la recaudación.
        private void ActualizarGrillaHistorial()
        {
            dgvHistorialCompleto.Rows.Clear();
            decimal recaudacion = 0;
            var historial = listaHistorial.ObtenerHistorial();
            foreach (var t in historial)
            {
                dgvHistorialCompleto.Rows.Add(t.CodigoOperacion, t.FechaHora.ToString("HH:mm:ss"), t.ClienteAsociado.NombreCompleto, t.CanalPago, $"S/ {t.MontoTotal:F2}", t.Estado, t.ResumenProductos);
                recaudacion += t.MontoTotal;
            }
            lblMetricaTotalOperaciones.Text = $"Operaciones Totales: {historial.Length}";
            lblMetricaRecaudacionCaja.Text = $"Caja Total: S/ {recaudacion:F2}";
        }

        // Presenta los datos de la transacción seleccionada o el mensaje de historial vacío.
        private void RefrescarAuditoriaVisual()
        {
            TransaccionVenta actual = listaHistorial.ObtenerActual();
            if (actual != null)
            {
                lblTicketAuditado.Text = $"AUDITANDO: {actual.CodigoOperacion} | {actual.CanalPago} | Estado: {actual.Estado}";
                lblDetalleAuditoria.Text =
                    $"========================================================\n" +
                    $" COMPROBANTE ELECTRÓNICO - BOTICA MARFARMA\n" +
                    $"========================================================\n" +
                    $" Código Operación    : {actual.CodigoOperacion}\n" +
                    $" Fecha y Hora        : {actual.FechaHora:dd/MM/yyyy HH:mm:ss}\n" +
                    $" Paciente            : {actual.ClienteAsociado.NombreCompleto}\n" +
                    $" DNI                 : {actual.ClienteAsociado.Dni}\n" +
                    $" Canal de Pago       : {actual.CanalPago}\n" +
                    $" Estado              : {actual.Estado}\n" +
                    $"--------------------------------------------------------\n" +
                    $" Medicamentos:\n {actual.ResumenProductos}\n" +
                    $"--------------------------------------------------------\n" +
                    $" TOTAL REGISTRADO    : S/ {actual.MontoTotal:F2}\n" +
                    $"========================================================";
            }
            else
            {
                lblTicketAuditado.Text = "Sin transacciones auditadas.";
                lblDetalleAuditoria.Text = "Aún no se han procesado operaciones .";
            }
        }
    }
}