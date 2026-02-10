namespace MedicalDataSystem.Forms;

/// <summary>
/// Main application dispatcher form (loads role-specific UI)
/// </summary>
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    // TODO: This form dispatches to role-specific forms
    // - CoordinatorForm for Coordinator role
    // - DoctorForm for Doctor/Nurse role
    // - TechnicianForm for Technician role
    // - PatientForm for Patient role
}
