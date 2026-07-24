using AnkiFocusEnforcer.Core.Interfaces;
using AnkiFocusEnforcer.Core.Model;
using AnkiFocusEnforcer.Services.Native;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AnkiFocusEnforcer.UI.Views;

public class MainForm : Form
{
    private readonly IFocusService _focusService;
    private readonly IMediaService _mediaService;
    private readonly AppSettings _settings;
    
    private Panel pnlLeft;
    private Panel pnlRight;
    private Label lblTitle;
    private Label lblTimer;
    private Label lblMinutes;
    private Label lblSettingsHeader;
    private NumericUpDown numMinutes;
    private Button btnStart;
    private FlowLayoutPanel pnlToggleContainer;
    private NotifyIcon _notifyIcon;
    private Button btnToggleSettings;
    private bool isPanelExpanded = true;
    private readonly List<Button> _toggleButtons = new List<Button>();
    
    private readonly Color BgColor = Color.FromArgb(248, 249, 250);
    private readonly Color PanelRightColor = Color.White;
    private readonly Color TextPrimary = Color.FromArgb(33, 37, 41);
    private readonly Color AccentBlue = Color.FromArgb(37, 99, 235);
    private readonly Color AccentBlueHover = Color.FromArgb(29, 78, 216);
    private readonly Color TextMuted = Color.FromArgb(108, 117, 125);
    private readonly Color SuccessGreen = Color.FromArgb(16, 185, 129);

    private readonly Color ActiveColor = Color.FromArgb(37, 99, 235);
    private readonly Color ActiveHoverColor = Color.FromArgb(29, 78, 216);
    private readonly Color InactiveColor = Color.FromArgb(229, 231, 235);
    private readonly Color InactiveHoverColor = Color.FromArgb(209, 213, 219);
    private readonly Color InactiveTextColor = Color.FromArgb(107, 114, 128);

    public MainForm(IFocusService focusService, IMediaService mediaService)
    {
        _focusService = focusService;
        _mediaService = mediaService;
        _settings = new AppSettings();
        InitializeUI();
        _focusService.OnTick += HandleTick;
        _focusService.OnSessionCompleted += HandleCompleted;
        _focusService.OnLockdownActivated += HandleLockdown;
    }

    private void InitializeUI()
    {
        Text = "Anki Focus Enforcer - Jukis Yuri";
        Size = new Size(680, 440); 
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = BgColor;
        TopMost = true;

        _notifyIcon = new NotifyIcon {
            Icon = new Icon(@"Resources\favicon.ico"),
            Visible = true,
            Text = "Anki Focus Enforcer"
        };
        
        pnlLeft = new Panel {
            Dock = DockStyle.Left,
            Width = 300,
            BackColor = BgColor
        };

        lblTitle = new Label {
            Text = "ANKI FOCUS ENFORCER",
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = AccentBlue,
            Top = 30,
            Left = 0,
            Width = 300,
            TextAlign = ContentAlignment.MiddleCenter
        };

        lblMinutes = new Label {
            Text = "Thời gian học (phút):",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            ForeColor = TextPrimary,
            Top = 100,
            Left = 20,
            AutoSize = true
        };

        numMinutes = new NumericUpDown {
            Value = _settings.DefaultFocusDuration,
            Minimum = 1,
            Maximum = 180,
            Top = 130,
            Left = 20,
            Width = 260,
            Font = new Font("Segoe UI", 10f)
        };

        btnStart = new Button {
            Text = "BẮT ĐẦU KHÓA",
            Top = 180,
            Left = 20,
            Width = 260,
            Height = 45,
            FlatStyle = FlatStyle.Flat,
            BackColor = AccentBlue,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnStart.FlatAppearance.BorderSize = 0;
        btnStart.Click += btnStart_Click;
        btnStart.MouseEnter += (s, e) => { if (btnStart.Enabled) btnStart.BackColor = AccentBlueHover; };
        btnStart.MouseLeave += (s, e) => { if (btnStart.Enabled) btnStart.BackColor = AccentBlue; };

        lblTimer = new Label {
            Text = "Sẵn sàng tập trung",
            Top = 250,
            Left = 0,
            Width = 300,
            Height = 30,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = SuccessGreen,
            TextAlign = ContentAlignment.MiddleCenter
        };
        
        // Khởi tạo nút Thu gọn / Mở rộng
        btnToggleSettings = new Button {
            Text = "⬅ Thu gọn",
            Top = 360,
            Left = 100,
            Width = 100,
            Height = 30,
            FlatStyle = FlatStyle.Flat,
            ForeColor = TextMuted,
            Font = new Font("Segoe UI", 9f, FontStyle.Regular),
            Cursor = Cursors.Hand
        };
        btnToggleSettings.FlatAppearance.BorderSize = 1;
        btnToggleSettings.FlatAppearance.BorderColor = InactiveColor;
        btnToggleSettings.Click += BtnToggleSettings_Click;

        pnlLeft.Controls.Add(lblTitle);
        pnlLeft.Controls.Add(lblMinutes);
        pnlLeft.Controls.Add(numMinutes);
        pnlLeft.Controls.Add(btnStart);
        pnlLeft.Controls.Add(lblTimer);
        pnlLeft.Controls.Add(btnToggleSettings);
        
        // Right Panel
        pnlRight = new Panel {
            Dock = DockStyle.Right,
            Width = 360,
            BackColor = PanelRightColor,
            Padding = new Padding(20)
        };

        lblSettingsHeader = new Label {
            Text = "TÙY CHỌN TÍNH NĂNG KHÓA",
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = TextMuted,
            Top = 25,
            Left = 20,
            AutoSize = true
        };

        pnlToggleContainer = new FlowLayoutPanel {
            Top = 60,
            Left = 20,
            Width = 340,
            Height = 330,
            BackColor = Color.Transparent,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        
        pnlToggleContainer.Controls.Add(CreateSettingItem("⌨️ Khóa phím tắt", "Chặn Alt+Tab, phím Windows để tránh thoát ứng dụng.", _settings.LockShortcuts, val => _settings.LockShortcuts = val));
        pnlToggleContainer.Controls.Add(CreateSettingItem("📌 Khóa Taskbar", "Vô hiệu hóa thanh Taskbar và Start Menu của Windows.", _settings.LockTaskbar, val => _settings.LockTaskbar = val));
        pnlToggleContainer.Controls.Add(CreateSettingItem("🖥️ Ẩn Desktop", "Làm mờ hoặc ẩn toàn bộ các biểu tượng trên màn hình nền.", _settings.HideDesktopIcons, val => _settings.HideDesktopIcons = val));
        pnlToggleContainer.Controls.Add(CreateSettingItem("🎵 Dừng Media", "Tự động tạm dừng nhạc hoặc video đang phát dưới nền.", _settings.AutoPauseMedia, val => _settings.AutoPauseMedia = val));
        pnlToggleContainer.Controls.Add(CreateSettingItem("🌙 Chống Sleep", "Ngăn máy tính tự động rơi vào chế độ ngủ khi đang học.", _settings.PreventSleep, val => _settings.PreventSleep = val));

        pnlRight.Controls.Add(lblSettingsHeader);
        pnlRight.Controls.Add(pnlToggleContainer);

        Controls.Add(pnlLeft);
        Controls.Add(pnlRight);
    }
    
    private Panel CreateSettingItem(string title, string description, bool initialState, Action<bool> onStateChanged)
    {
        Panel pnlItem = new Panel {
            Width = 310,
            Height = 70,
            Margin = new Padding(0, 0, 0, 8),
            BackColor = Color.Transparent
        };

        Label lblItemTitle = new Label {
            Text = title,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = TextPrimary,
            Top = 5,
            Left = 5,
            AutoSize = true
        };

        Label lblItemDesc = new Label {
            Text = description,
            Font = new Font("Segoe UI", 8f, FontStyle.Regular),
            ForeColor = TextMuted,
            Top = 26,
            Left = 5,
            AutoSize = true,
            MaximumSize = new Size(225, 0)
        };

        Button btnToggle = new Button {
            Text = GetToggleText(initialState),
            Font = new Font("Segoe UI", 8f, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Top = 20,
            Left = 240,
            Width = 60,
            Height = 28,
            Cursor = Cursors.Hand,
            Tag = initialState
        };
        btnToggle.FlatAppearance.BorderSize = 0;

        UpdateToggleStyle(btnToggle, initialState);

        btnToggle.MouseEnter += (s, e) => {
            if (btnToggle.Enabled) {
                bool active = (bool)btnToggle.Tag;
                btnToggle.BackColor = active ? ActiveHoverColor : InactiveHoverColor;
            }
        };

        btnToggle.MouseLeave += (s, e) => {
            if (btnToggle.Enabled) {
                bool active = (bool)btnToggle.Tag;
                btnToggle.BackColor = active ? ActiveColor : InactiveColor;
            }
        };

        btnToggle.Click += (s, e) => {
            bool newState = !(bool)btnToggle.Tag;
            btnToggle.Tag = newState;
            btnToggle.Text = GetToggleText(newState);
            UpdateToggleStyle(btnToggle, newState);
            onStateChanged?.Invoke(newState);
        };

        _toggleButtons.Add(btnToggle);

        pnlItem.Controls.Add(lblItemTitle);
        pnlItem.Controls.Add(lblItemDesc);
        pnlItem.Controls.Add(btnToggle);

        return pnlItem;
    }

    private string GetToggleText(bool isActive) => isActive ? "ON" : "OFF";

    private void UpdateToggleStyle(Button btn, bool isActive)
    {
        btn.BackColor = isActive ? ActiveColor : InactiveColor;
        btn.ForeColor = isActive ? Color.White : InactiveTextColor;
    }

    private void SetControlsState(bool enabled)
    {
        btnStart.Enabled = enabled;
        btnStart.BackColor = enabled ? AccentBlue : Color.FromArgb(209, 213, 219);
        numMinutes.Enabled = enabled;

        foreach (var btn in _toggleButtons)
        {
            btn.Enabled = enabled;
        }
    }
    
    private async void btnStart_Click(object sender, EventArgs e)
    {
        int minutes = (int)numMinutes.Value;
        var windowService = new WindowLockService();
        if (!windowService.IsAppRunning("anki"))
        {
            MessageBox.Show("Không tìm thấy Anki! Hãy mở Anki lên trước rồi mới bấm Bắt đầu nhé.", 
                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        SetControlsState(false);
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
            SetControlsState(true);
            lblTimer.Text = "Sẵn sàng tập trung";
        }
    }

    private void HandleLockdown()
    {
        Invoke(new Action(() =>
        {
            if (_settings.LockShortcuts) KeyboardBlocker.Block();
            if (_settings.LockTaskbar) TaskbarController.DisableTaskbarInteraction();
            if (_settings.HideDesktopIcons) DesktopController.HideDesktopIcons();
            if (_settings.PreventSleep) PowerController.PreventSleep();
            if (_settings.AutoPauseMedia) _mediaService.PauseAllMedia();

            int minutes = (int)numMinutes.Value;
            lblTimer.Text = $"Đã khóa! Còn: {minutes * 60:D2}s";

            string imagePath = Path.GetFullPath(@"Resources\elaina.jpg");
            new ToastContentBuilder()
                .AddText("Mizuki Nhắc Bạn =)")
                .AddText("Đã kích hoạt chế độ tập trung!")
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
            SetControlsState(true);

            Activate();
            MessageBox.Show("Tuyệt vời! Bạn đã hoàn thành phiên học.", "Thành công", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }));
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        KeyboardBlocker.Unblock();
        TaskbarController.EnableTaskbarInteraction();
        DesktopController.ShowDesktopIcons();
        PowerController.AllowSleep();

        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        base.OnFormClosing(e);
    }
    
    private void BtnToggleSettings_Click(object sender, EventArgs e)
    {
        isPanelExpanded = !isPanelExpanded;
    
        if (isPanelExpanded)
        {
            Width = 680; 
            btnToggleSettings.Text = "⬅ Thu gọn";
        }
        else
        {
            Width = 316; 
            btnToggleSettings.Text = "Tùy chỉnh";
        }
    }
}