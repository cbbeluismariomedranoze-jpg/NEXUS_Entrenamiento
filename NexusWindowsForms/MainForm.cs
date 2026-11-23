using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace NexusWindowsForms;

public sealed class MainForm : Form
{
    private readonly Label statusLabel;

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
            RowCount = 3,
            Padding = new Padding(70, 24, 70, 24),
            BackColor = Color.FromArgb(16, 29, 52)
        };
        menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 3; i++)
            menu.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));

        menu.Controls.Add(CreateButton("INICIAR ENTRENAMIENTO", "Abre el modo de consola original." , () => LaunchConsole(false)), 0, 0);
        menu.Controls.Add(CreateButton("LA ANOMALÍA PERDIDA", "Abre la misión de H3." , () => LaunchConsole(true)), 1, 0);
        menu.Controls.Add(CreateButton("DOCUMENTACIÓN", "Consulta el código y documentos del proyecto." , OpenDocumentation), 0, 1);
        menu.Controls.Add(CreateButton("ACERCA DE NEXUS", "Información de esta primera interfaz." , ShowAbout), 1, 1);
        menu.Controls.Add(CreateButton("COMPROBAR PROYECTO", "Verifica que se encuentre el código de consola." , CheckProject), 0, 2);
        menu.Controls.Add(CreateButton("SALIR", "Cerrar la interfaz gráfica." , Close), 1, 2);
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
            Text = caption + Environment.NewLine + hint,
            Dock = DockStyle.Fill,
            Margin = new Padding(12),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(24, 48, 77),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Padding = new Padding(8)
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
}
