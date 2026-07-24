using AnkiFocusEnforcer.Core.Interfaces;
using AnkiFocusEnforcer.Services.Native;
using Microsoft.Toolkit.Uwp.Notifications;

public class MainForm : Form
{
    private readonly IFocusService _focusService;
    private Label lblTitle;
    private Label lblTimer;
    private Button btnStart;
    private NumericUpDown numMinutes;
    private Label lblMinutes;
    private NotifyIcon _notifyIcon;
    
    private readonly Color BgColor = Color.FromArgb(248, 249, 250);        
    private readonly Color TextPrimary = Color.FromArgb(33, 37, 41);       
    private readonly Color AccentBlue = Color.FromArgb(37, 99, 235);       
    private readonly Color AccentBlueHover = Color.FromArgb(29, 78, 216);  
    private readonly Color TextMuted = Color.FromArgb(108, 117, 125);      
    private readonly Color SuccessGreen = Color.FromArgb(16, 185, 129);    

    public MainForm(IFocusService focusService)
    {
        _focusService = focusService;
        InitializeUI();
        
        _focusService.OnTick += HandleTick;
        _focusService.OnSessionCompleted += HandleCompleted;
        _focusService.OnLockdownActivated += HandleLockdown; 
    }

    private async void btnStart_Click(object sender, EventArgs e)
    {
        int minutes = (int) numMinutes.Value;
        var windowService = new WindowLockService();
        if (!windowService.IsAppRunning("anki"))
        {
            MessageBox.Show("Không tìm thấy Anki! Hãy mở Anki lên trước rồi mới bấm Bắt đầu nhé.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        btnStart.Enabled = false;
        btnStart.BackColor = Color.FromArgb(209, 213, 219); // Xám khi disable
        numMinutes.Enabled = false;
        
        for (int i = 5; i > 0; i--)
        {
            lblTimer.Text = $"Khóa sau {i}s...";
            await Task.Delay(1000);
        }
        
        try
        {
            _focusService.StartSession(minutes);
            lblTimer.Text = "Đang tập trung...";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            btnStart.Enabled = true;
            btnStart.BackColor = AccentBlue;
            numMinutes.Enabled = true;
            lblTimer.Text = "Sẵn sàng tập trung";
        }
    }
    
    private void HandleLockdown()
    {
        Invoke(new Action(() =>
        {
            KeyboardBlocker.Block();
            TaskbarController.DisableTaskbarInteraction();
            DesktopController.HideDesktopIcons();
            PowerController.PreventSleep();
            
            int minutes = (int)numMinutes.Value;
            lblTimer.Text = $"Đã khóa! Còn: {minutes * 60:D2}s";
            
            string imagePath = Path.GetFullPath(@"Resources\elaina.jpg"); 

            new ToastContentBuilder()
                .AddText("Mizuki Nhắc Bạn =)") // Tiêu đề
                .AddText("Đã tạm dừng Media\nKhóa Taskbar & Desktop")

                .AddAppLogoOverride(new Uri(imagePath), ToastGenericAppLogoCrop.Circle)
                .Show();
        }));
    }

    private void HandleTick(int remainingSeconds)
    {
        Invoke(new Action(() =>
        {
            TimeSpan t = TimeSpan.FromSeconds(remainingSeconds);
            lblTimer.Text = $"Còn lại: {t.Minutes:D2}:{t.Seconds:D2}";
        }));
    }

    private void HandleCompleted()
    {
        Invoke(new Action(() =>
        {
            KeyboardBlocker.Unblock();
            TaskbarController.EnableTaskbarInteraction();
            DesktopController.ShowDesktopIcons();
            PowerController.AllowSleep();

            lblTimer.Text = "Hoàn thành phiên học!";
            btnStart.Enabled = true;
            btnStart.BackColor = AccentBlue;
            numMinutes.Enabled = true;
            
            Activate(); 
            MessageBox.Show("Tuyệt vời! Bạn đã hoàn thành phiên học.", "Thành công", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }));
    }

    private void InitializeUI()
    {
        Text = "Anki Focus Enforcer";
        Size = new Size(360, 230);
        
        _notifyIcon = new NotifyIcon {
            Icon = new Icon(@"Resources\favicon.ico"),
            Visible = true,
            Text = "Anki Focus Enforcer"
        };
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = BgColor;
        TopMost = true;
        
        lblTitle = new Label {
            Text = "ANKI FOCUS ENFORCER",
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = TextMuted,
            Top = 15,
            Left = 20,
            Width = 305,
            TextAlign = ContentAlignment.MiddleCenter
        };
        
        lblMinutes = new Label {
            Text = "Thời gian học (phút):",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            ForeColor = TextPrimary,
            Top = 50,
            Left = 25,
            AutoSize = true
        };
        
        numMinutes = new NumericUpDown {
            Value = 15,
            Minimum = 1,
            Maximum = 180,
            Top = 48,
            Left = 195,
            Width = 135,
            Font = new Font("Segoe UI", 10)
        };
        
        btnStart = new Button {
            Text = "BẮT ĐẦU KHÓA",
            Top = 90,
            Left = 25,
            Width = 305,
            Height = 40,
            FlatStyle = FlatStyle.Flat,
            BackColor = AccentBlue,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnStart.FlatAppearance.BorderSize = 0;
        btnStart.Click += btnStart_Click;
        btnStart.MouseEnter += (s, e) => { if (btnStart.Enabled) btnStart.BackColor = AccentBlueHover; };
        btnStart.MouseLeave += (s, e) => { if (btnStart.Enabled) btnStart.BackColor = AccentBlue; };
        
        lblTimer = new Label {
            Text = "Sẵn sàng tập trung",
            Top = 145,
            Left = 25,
            Width = 305,
            Height = 25,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = SuccessGreen,
            TextAlign = ContentAlignment.MiddleCenter
        };
        
        Controls.Add(lblTitle);
        Controls.Add(lblMinutes);
        Controls.Add(numMinutes);
        Controls.Add(btnStart);
        Controls.Add(lblTimer);
    }
    
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        KeyboardBlocker.Unblock();
        TaskbarController.EnableTaskbarInteraction();
        DesktopController.ShowDesktopIcons();

        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        base.OnFormClosing(e);
    }
}