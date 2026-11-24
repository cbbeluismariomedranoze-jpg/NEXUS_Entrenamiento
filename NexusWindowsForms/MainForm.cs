using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace NexusWindowsForms;

public sealed class MainForm : Form
{
    private Label statusLabel;
    private RepositorioNexus repositorio = new();
    private NexusTraining.MundoNexus? mundoActual;
    private int? partidaActualId;

    public MainForm()
    {
        Text = "NEXUS V1.0 | Academia de Exploradores";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(850, 600);
        Size = new Size(1050, 700);
        BackColor = Color.FromArgb(10, 18, 36);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10F);
        BuildInterface();
    }

    private void BuildInterface()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(36),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        Controls.Add(root);

        var heading = new Panel { Dock = DockStyle.Fill };
        var title = new Label
        {
            Text = "N E X U S",
            Dock = DockStyle.Top,
            Height = 58,
            Font = new Font("Segoe UI", 28F, FontStyle.Bold),
            ForeColor = Color.FromArgb(80, 220, 245),
            TextAlign = ContentAlignment.MiddleCenter
        };
        var subtitle = new Label
        {
            Text = "ACADEMIA DE EXPLORADORES  |  AÑO 2297",
            Dock = DockStyle.Bottom,
            Height = 34,
            ForeColor = Color.FromArgb(200, 218, 235),
            TextAlign = ContentAlignment.MiddleCenter
        };
        heading.Controls.Add(subtitle);
        heading.Controls.Add(title);
        root.Controls.Add(heading, 0, 0);

        var menu = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(70, 10, 70, 10),
            BackColor = Color.FromArgb(16, 29, 52)
        };
        menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 5; i++)
            menu.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

        menu.Controls.Add(CreateButton("INICIAR ENTRENAMIENTO", "Inicia el juego con memoria persistente.", JugarPartida), 0, 0);
        menu.Controls.Add(CreateButton("LA ANOMALÍA PERDIDA", "Abre la misión de H3." , () => LaunchConsole(true)), 1, 0);
        menu.Controls.Add(CreateButton("DOCUMENTACIÓN", "Consulta el código y documentos del proyecto." , OpenDocumentation), 0, 1);
        menu.Controls.Add(CreateButton("ACERCA DE NEXUS", "Información de esta primera interfaz." , ShowAbout), 1, 1);
        menu.Controls.Add(CreateButton("COMPROBAR PROYECTO", "Verifica que se encuentre el código de consola." , CheckProject), 0, 2);
        menu.Controls.Add(CreateButton("NUEVA PARTIDA", "Registrar un nuevo explorador.", CrearPartida), 1, 2);
        menu.Controls.Add(CreateButton("PARTIDAS GUARDADAS", "Consultar partidas registradas.", MostrarPartidas), 0, 3);
        menu.Controls.Add(CreateButton("GUARDAR PARTIDA", "Guardar el progreso actual", GuardarPartida), 1, 3);
        menu.Controls.Add(CreateButton("PROBAR CONEXIÓN SQL", "Comprueba la conexión con NEXUS_DB.", ProbarConexionSQL), 0, 4);
        menu.Controls.Add(CreateButton("SALIR", "Cerrar la interfaz gráfica.", Close), 1, 4);

        root.Controls.Add(menu, 0, 1);

        statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = "SISTEMA LISTO  |  Selecciona una opción para continuar.",
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(80, 220, 245),
            BackColor = Color.FromArgb(16, 29, 52),
            Padding = new Padding(8)
        };
        root.Controls.Add(statusLabel, 0, 2);

        var footer = new Label
        {
            Dock = DockStyle.Fill,
            Text = "NEXUS H4.01  •  Windows Forms + C#  •  Prototipo gráfico",
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(155, 174, 197)
        };
        root.Controls.Add(footer, 0, 3);
    }


    private static Button CreateButton(string caption, string hint, Action action)
    {
        var button = new Button
        {
            Text = caption,
            AccessibleDescription = hint,
            Dock = DockStyle.Fill,
            Margin = new Padding(5),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(24, 48, 77),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand,
            Padding = new Padding(3),
            AutoSize = false
        };

        button.FlatAppearance.BorderColor = Color.FromArgb(48, 180, 210);
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(34, 68, 100);
        button.Click += (_, _) => action();

        return button;
    }


    private string? FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "NEXUS_Entrenamiento.sln")))
                return directory.FullName;
            directory = directory.Parent;
        }
        return null;
    }

    private void LaunchConsole(bool missionMode)
    {
        string? rootPath = FindRepositoryRoot();
        if (rootPath is null)
        {
            MessageBox.Show(
                "No se encontró NEXUS_Entrenamiento.sln. Abre la aplicación desde una copia completa del repositorio.",
                "No se encuentra el proyecto",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        string projectPath = Path.Combine(rootPath, "NexusTraining", "NexusTraining.csproj");
        if (!File.Exists(projectPath))
        {
            MessageBox.Show("No se encontró el proyecto de consola original en NexusTraining.", "Archivo no encontrado",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var start = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = rootPath,
            UseShellExecute = true,
            Arguments = $"run --project \"{projectPath}\"" + (missionMode ? " -- --v2" : "")
        };

        try
        {
            Process.Start(start);
            statusLabel.Text = missionMode
                ? "MODO MISIÓN solicitado: La anomalía perdida."
                : "ENTRENAMIENTO solicitado: modo de consola H3.";
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo iniciar el proyecto de consola. Comprueba que el SDK de .NET esté instalado.\n\n" + ex.Message,
                "Error al iniciar NEXUS", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenDocumentation()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/cbbeluismariomedranoze-jpg/NEXUS_Entrenamiento/tree/main/docs",
                UseShellExecute = true
            });
            statusLabel.Text = "Abriendo la documentación publicada en GitHub.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "No se pudo abrir el navegador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ShowAbout()
    {
        MessageBox.Show(
            "NEXUS V1.0 — Academia de Exploradores\n\n" +
            "Prototipo gráfico H4.01. Esta interfaz se integra con el proyecto de consola H3. " +
            "Las reglas de juego siguen en el proyecto NexusTraining.",
            "Acerca de NEXUS",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        statusLabel.Text = "NEXUS: interfaz gráfica inicial.";
    }

    private void CheckProject()
    {
        string? rootPath = FindRepositoryRoot();
        bool consoleExists = rootPath is not null &&
            File.Exists(Path.Combine(rootPath, "NexusTraining", "NexusTraining.csproj"));
        statusLabel.Text = consoleExists
            ? "VERIFICACIÓN CORRECTA: se encontró el proyecto original de consola."
            : "VERIFICACIÓN PENDIENTE: abre la aplicación desde la solución completa.";
        MessageBox.Show(statusLabel.Text, "Comprobación de archivos",
            MessageBoxButtons.OK,
            consoleExists ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
    private void ProbarConexionSQL()
    {
        bool conectado = ConexionBD.ProbarConexion();

        if (conectado)
        {
            statusLabel.Text = "CONEXIÓN SQL EXITOSA | NEXUS_DB";

            MessageBox.Show(
                "Conexión con NEXUS_DB exitosa.",
                "NEXUS",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        else
        {
            statusLabel.Text = "ERROR DE CONEXIÓN SQL";
        }
    }
    private void CrearPartida()
    {
        string nombre = Microsoft.VisualBasic.Interaction.InputBox(
            "Ingresa el nombre de tu explorador:",
            "NUEVA PARTIDA",
            "");

        if (string.IsNullOrWhiteSpace(nombre))
            return;

        try
        {
            var repositorio = new RepositorioNexus();
            int partidaId = repositorio.CrearNuevaPartida(nombre);
            mundoActual = new NexusTraining.MundoNexus(nombre);
            partidaActualId = partidaId;
            repositorio.GuardarPartida(partidaId, mundoActual);

            statusLabel.Text = "PARTIDA CREADA | ID: " + partidaId;

            MessageBox.Show(
                $"Explorador registrado correctamente.\nPartida ID: {partidaId}",
                "NEXUS",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "No se pudo crear la partida:\n" + ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }



    private void MostrarPartidas()
    {
        try
        {
            var partidas = repositorio.ObtenerPartidas();

            if (partidas.Count == 0)
            {
                MessageBox.Show("No hay partidas guardadas todavía.");
                return;
            }

            string lista = string.Join(
                Environment.NewLine + Environment.NewLine, partidas);

            string opcion = Microsoft.VisualBasic.Interaction.InputBox(
                lista +
                "\n\n¿Qué deseas hacer?" +
                "\n1. Cargar partida" +
                "\n2. Eliminar partida" +
                "\n0. Cancelar",
                "PARTIDAS GUARDADAS");

            if (opcion == "1")
            {
                string idTexto = Microsoft.VisualBasic.Interaction.InputBox(
                    "Ingresa el ID de la partida que deseas cargar:",
                    "CARGAR PARTIDA");

                if (!int.TryParse(idTexto, out int id))
                    return;

                mundoActual = repositorio.CargarPartida(id);
                partidaActualId = id;

                MessageBox.Show("¡Partida cargada correctamente!");
            }
            else if (opcion == "2")
            {
                EliminarPartida();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al gestionar las partidas:\n" + ex.Message);
        }
    }

    private void GuardarPartida()
    {
        if (partidaActualId == null || mundoActual == null)
        {
            MessageBox.Show("Primero debes crear o cargar una partida.");
            return;
        }

        try
        {
            repositorio.GuardarPartida(partidaActualId.Value, mundoActual);

            statusLabel.Text = "PARTIDA GUARDADA CORRECTAMENTE";

            MessageBox.Show(
                "Tu progreso se guardó correctamente en NEXUS_DB.",
                "NEXUS",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al guardar la partida:\n" + ex.Message);
        }
    }

    private void JugarPartida()
    {
        if (mundoActual == null || partidaActualId == null)
        {
            MessageBox.Show("Primero debes crear o cargar una partida.");
            return;
        }

        while (!mundoActual.Terminado && mundoActual.Jugador.Vivo)
        {
            string estado =
                mundoActual.Jugador.Describir() + "\n\n" +
                mundoActual.Mision.Estado() + "\n\n" +
                mundoActual.Mapa();

            string opcion = Microsoft.VisualBasic.Interaction.InputBox(
                estado +
                "\n\n1. Moverse de zona" +
                "\n2. Explorar" +
                "\n3. Resolver encuentro" +
                "\n4. Usar objeto" +
                "\n5. Guardar partida" +
                "\n0. Salir del juego",
                "NEXUS - OPERACIÓN MEMORIA");

            if (opcion == "0" || opcion == "")
                break;

            string resultado = "";

            switch (opcion)
            {
                case "1":
                    string destino = Microsoft.VisualBasic.Interaction.InputBox(
                        "Ingresa la zona de destino (0 a 3):",
                        "MOVIMIENTO");

                    if (int.TryParse(destino, out int zona))
                        resultado = mundoActual.Mover(zona);
                    else
                        resultado = "Zona inválida.";
                    break;

                case "2":
                    resultado = mundoActual.Explorar();
                    break;

                case "3":
                    string accion = Microsoft.VisualBasic.Interaction.InputBox(
                        "1. Atacar\n2. Escapar\n3. Analizar",
                        "ENCUENTRO");

                    if (int.TryParse(accion, out int accionElegida))
                        resultado = mundoActual.AccionEncuentro(accionElegida);
                    else
                        resultado = "Acción inválida.";
                    break;

                case "4":
                    string objeto = Microsoft.VisualBasic.Interaction.InputBox(
                        "0. Célula\n1. Escudo\n2. Módulo de ataque\n3. Analizador",
                        "INVENTARIO");

                    if (int.TryParse(objeto, out int objetoElegido) &&
                        Enum.IsDefined(typeof(NexusTraining.TipoObjeto), objetoElegido))
                    {
                        resultado = mundoActual.UsarObjeto(
                            (NexusTraining.TipoObjeto)objetoElegido);
                    }
                    else
                        resultado = "Objeto inválido.";
                    break;

                case "5":
                    repositorio.GuardarPartida(
                        partidaActualId.Value, mundoActual);

                    resultado = "Partida guardada correctamente.";
                    break;

                default:
                    resultado = "Opción inválida.";
                    break;
            }

            if (opcion != "5")
            {
                repositorio.GuardarPartida(
                    partidaActualId.Value, mundoActual);
            }

            MessageBox.Show(resultado, "NEXUS");
        }

        if (mundoActual.Terminado || !mundoActual.Jugador.Vivo)
        {
            repositorio.GuardarPartida(
                partidaActualId.Value, mundoActual);

            MessageBox.Show("La partida ha finalizado.");
        }
    }

    private void EliminarPartida()
    {
        string idTexto = Microsoft.VisualBasic.Interaction.InputBox(
            "Ingresa el ID de la partida que deseas eliminar:",
            "ELIMINAR PARTIDA");

        if (!int.TryParse(idTexto, out int id))
            return;

        var confirmacion = MessageBox.Show(
            "¿Seguro que deseas eliminar esta partida?",
            "CONFIRMAR ELIMINACIÓN",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmacion != DialogResult.Yes)
            return;

        try
        {
            repositorio.EliminarPartida(id);

            MessageBox.Show("Operación de eliminación realizada correctamente.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al eliminar la partida:\n" + ex.Message);
        }
    }

}
