namespace MedicalDataSystem;

using MedicalDataSystem.Forms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        // ApplicationConfiguration.Initialize();
        //Application.Run(new LoginForm());

        ApplicationConfiguration.Initialize();

        // Vòng lặp này giúp duy trì ứng dụng khi người dùng Đăng xuất (Logout)
        while (true)
        {
            using (var login = new LoginForm())
            {
                // 1. Hiện màn hình Login. 
                // Nếu người dùng bấm X hoặc Cancel ở màn hình Login, thoát hẳn vòng lặp và đóng App.
                if (login.ShowDialog() != DialogResult.OK)
                    break;

                // 2. Chạy Form chức năng tương ứng (được gán vào TargetForm sau khi Login thành công)
                // Lưu ý: Duyên cần thêm public Form TargetForm { get; set; } vào class LoginForm.cs
                if (login.TargetForm != null)
                {
                    Application.Run(login.TargetForm);

                    // 3. Kiểm tra sau khi Form chức năng đóng lại:
                    // Nếu đóng do nhấn "Đăng xuất" (mình đã set DialogResult = OK ở BaseForm), 
                    // thì vòng lặp while sẽ chạy lại và hiện LoginForm.
                    // Nếu đóng do bấm dấu X ở góc màn hình (không phải OK), thoát hẳn App.
                    if (login.TargetForm.DialogResult != DialogResult.OK)
                        break;
                }
                else
                {
                    break;
                }
            }
        }
    }    
}