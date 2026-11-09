using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Zse.Bridge;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void QtNativeCallback(IntPtr userData);

public static partial class QtNative
{
    private const string LibName = "zse_bridge";
    private static readonly List<Delegate> s_pinnedDelegates = new();

    public static void RootDelegate(Delegate d)
    {
        lock (s_pinnedDelegates)
        {
            s_pinnedDelegates.Add(d);
        }
    }

    [LibraryImport(LibName, EntryPoint = "qt_app_create")]
    public static partial IntPtr AppCreate(int argc, IntPtr argv);

    [LibraryImport(LibName, EntryPoint = "qt_app_exec")]
    public static partial int AppExec(IntPtr app);

    [LibraryImport(LibName, EntryPoint = "qt_app_quit")]
    public static partial void AppQuit(IntPtr app);

    [LibraryImport(LibName, EntryPoint = "qt_app_set_dark_theme")]
    public static partial void AppSetDarkTheme(IntPtr app, int dark);

    [LibraryImport(LibName, EntryPoint = "qt_mainwindow_create")]
    public static partial IntPtr MainWindowCreate();

    [LibraryImport(LibName, EntryPoint = "qt_widget_show")]
    public static partial void WidgetShow(IntPtr widget);

    [LibraryImport(LibName, EntryPoint = "qt_widget_resize")]
    public static partial void WidgetResize(IntPtr widget, int w, int h);

    [LibraryImport(LibName, EntryPoint = "qt_widget_set_title", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void WidgetSetTitle(IntPtr widget, string title);

    [LibraryImport(LibName, EntryPoint = "qt_widget_set_central")]
    public static partial void WidgetSetCentral(IntPtr mainWindow, IntPtr widget);

    [LibraryImport(LibName, EntryPoint = "qt_widget_set_layout")]
    public static partial void WidgetSetLayout(IntPtr widget, IntPtr layout);

    [LibraryImport(LibName, EntryPoint = "qt_widget_create")]
    public static partial IntPtr WidgetCreate();

    [LibraryImport(LibName, EntryPoint = "qt_vbox_create")]
    public static partial IntPtr VBoxLayoutCreate();

    [LibraryImport(LibName, EntryPoint = "qt_hbox_create")]
    public static partial IntPtr HBoxLayoutCreate();

    [LibraryImport(LibName, EntryPoint = "qt_grid_create")]
    public static partial IntPtr GridLayoutCreate();

    [LibraryImport(LibName, EntryPoint = "qt_box_add_widget")]
    public static partial void BoxAddWidget(IntPtr boxLayout, IntPtr widget, int stretch);

    [LibraryImport(LibName, EntryPoint = "qt_box_add_layout")]
    public static partial void BoxAddLayout(IntPtr boxLayout, IntPtr childLayout);

    [LibraryImport(LibName, EntryPoint = "qt_box_add_stretch")]
    public static partial void BoxAddStretch(IntPtr boxLayout);

    [LibraryImport(LibName, EntryPoint = "qt_grid_add_widget")]
    public static partial void GridAddWidget(IntPtr gridLayout, IntPtr widget, int row, int col, int rowSpan, int colSpan);

    [LibraryImport(LibName, EntryPoint = "qt_tabwidget_create")]
    public static partial IntPtr TabWidgetCreate();

    [LibraryImport(LibName, EntryPoint = "qt_tabwidget_add_tab", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void TabWidgetAddTab(IntPtr tabs, IntPtr page, string title);

    [LibraryImport(LibName, EntryPoint = "qt_groupbox_create", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr GroupBoxCreate(string title);

    [LibraryImport(LibName, EntryPoint = "qt_scrollarea_create")]
    public static partial IntPtr ScrollAreaCreate(IntPtr contentWidget);

    [LibraryImport(LibName, EntryPoint = "qt_label_create", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr LabelCreate(string text);

    [LibraryImport(LibName, EntryPoint = "qt_label_set_text", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void LabelSetText(IntPtr label, string text);

    [LibraryImport(LibName, EntryPoint = "qt_pushbutton_create", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr PushButtonCreate(string text);

    [LibraryImport(LibName, EntryPoint = "qt_pushbutton_set_style", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void PushButtonSetStyle(IntPtr btn, string qss);

    [LibraryImport(LibName, EntryPoint = "qt_pushbutton_on_click")]
    public static partial void PushButtonOnClick(IntPtr btn, [MarshalAs(UnmanagedType.FunctionPtr)] QtNativeCallback cb, IntPtr userData);

    [LibraryImport(LibName, EntryPoint = "qt_checkbox_create", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr CheckBoxCreate(string text);

    [LibraryImport(LibName, EntryPoint = "qt_checkbox_is_checked")]
    public static partial int CheckBoxIsChecked(IntPtr cb);

    [LibraryImport(LibName, EntryPoint = "qt_checkbox_set_checked")]
    public static partial void CheckBoxSetChecked(IntPtr cb, int checkedVal);

    [LibraryImport(LibName, EntryPoint = "qt_spinbox_create")]
    public static partial IntPtr SpinBoxCreate(int minVal, int maxVal, int val);

    [LibraryImport(LibName, EntryPoint = "qt_spinbox_get_val")]
    public static partial int SpinBoxGetVal(IntPtr sb);

    [LibraryImport(LibName, EntryPoint = "qt_spinbox_set_val")]
    public static partial void SpinBoxSetVal(IntPtr sb, int val);

    [LibraryImport(LibName, EntryPoint = "qt_doublespinbox_create")]
    public static partial IntPtr DoubleSpinBoxCreate(double minVal, double maxVal, double val, double step);

    [LibraryImport(LibName, EntryPoint = "qt_doublespinbox_get_val")]
    public static partial double DoubleSpinBoxGetVal(IntPtr dsb);

    [LibraryImport(LibName, EntryPoint = "qt_doublespinbox_set_val")]
    public static partial void DoubleSpinBoxSetVal(IntPtr dsb, double val);

    [LibraryImport(LibName, EntryPoint = "qt_combobox_create")]
    public static partial IntPtr ComboBoxCreate();

    [LibraryImport(LibName, EntryPoint = "qt_combobox_add_item", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ComboBoxAddItem(IntPtr cmb, string text);

    [LibraryImport(LibName, EntryPoint = "qt_combobox_get_index")]
    public static partial int ComboBoxGetIndex(IntPtr cmb);

    [LibraryImport(LibName, EntryPoint = "qt_combobox_set_index")]
    public static partial void ComboBoxSetIndex(IntPtr cmb, int index);

    [LibraryImport(LibName, EntryPoint = "qt_combobox_clear")]
    public static partial void ComboBoxClear(IntPtr cmb);

    [LibraryImport(LibName, EntryPoint = "qt_combobox_on_change")]
    public static partial void ComboBoxOnChange(IntPtr cmb, QtNativeCallback cb, IntPtr userData);

    [LibraryImport(LibName, EntryPoint = "qt_textedit_create", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr TextEditCreate(string text, int readOnly);

    [LibraryImport(LibName, EntryPoint = "qt_textedit_set_text", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void TextEditSetText(IntPtr te, string text);

    [LibraryImport(LibName, EntryPoint = "qt_textedit_get_text")]
    public static partial int TextEditGetText(IntPtr te, Span<byte> outBuf, int outLen);

    [LibraryImport(LibName, EntryPoint = "qt_lineedit_create", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr LineEditCreate(string text);

    [LibraryImport(LibName, EntryPoint = "qt_lineedit_set_text", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void LineEditSetText(IntPtr le, string text);

    [LibraryImport(LibName, EntryPoint = "qt_lineedit_get_text")]
    public static partial int LineEditGetText(IntPtr le, Span<byte> outBuf, int outLen);

    [LibraryImport(LibName, EntryPoint = "qt_statusbar_message", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void StatusBarMessage(IntPtr mainWindow, string text);

    [LibraryImport(LibName, EntryPoint = "qt_message_box", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void MessageBox(IntPtr parent, int iconType, string title, string text);

    [LibraryImport(LibName, EntryPoint = "qt_dialog_get_directory", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int DialogGetDirectory(IntPtr parent, string title, string initialDir, Span<byte> outBuf, int outLen);

    [LibraryImport(LibName, EntryPoint = "qt_mainwindow_menu_bar")]
    public static partial IntPtr MainWindowMenuBar(IntPtr mainWindow);

    [LibraryImport(LibName, EntryPoint = "qt_menubar_add_menu", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr MenuBarAddMenu(IntPtr menubar, string title);

    [LibraryImport(LibName, EntryPoint = "qt_menu_add_action", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr MenuAddAction(IntPtr menu, string text, string shortcut, QtNativeCallback cb, IntPtr userData);

    [LibraryImport(LibName, EntryPoint = "qt_menu_add_separator")]
    public static partial void MenuAddSeparator(IntPtr menu);
}

// --- Idiomatic C# Wrappers over Qt Primitives ---

public class QWidget
{
    public IntPtr Handle { get; }
    public QWidget(IntPtr handle) => Handle = handle;
    public QWidget() : this(QtNative.WidgetCreate()) { }

    public void Show() => QtNative.WidgetShow(Handle);
    public void Resize(int w, int h) => QtNative.WidgetResize(Handle, w, h);
    public void SetTitle(string title) => QtNative.WidgetSetTitle(Handle, title);
    public void SetLayout(QLayout layout) => QtNative.WidgetSetLayout(Handle, layout.Handle);
}

public class QMainWindow : QWidget
{
    public QMainWindow() : base(QtNative.MainWindowCreate()) { }
    public void SetCentralWidget(QWidget widget) => QtNative.WidgetSetCentral(Handle, widget.Handle);
    public void SetStatusMessage(string msg) => QtNative.StatusBarMessage(Handle, msg);
    public QMenuBar MenuBar => new(QtNative.MainWindowMenuBar(Handle));
}

public class QMenuBar
{
    public IntPtr Handle { get; }
    public QMenuBar(IntPtr handle) => Handle = handle;
    public QMenu AddMenu(string title) => new(QtNative.MenuBarAddMenu(Handle, title));
}

public class QMenu
{
    public IntPtr Handle { get; }
    private readonly List<QtNativeCallback> _callbacks = new();

    public QMenu(IntPtr handle) => Handle = handle;

    public void AddAction(string text, Action onClick, string shortcut = "")
    {
        QtNativeCallback cb = _ => onClick();
        _callbacks.Add(cb);
        QtNative.RootDelegate(cb);
        QtNative.MenuAddAction(Handle, text, shortcut, cb, IntPtr.Zero);
    }

    public void AddSeparator() => QtNative.MenuAddSeparator(Handle);
}

public abstract class QLayout
{
    public IntPtr Handle { get; }
    protected QLayout(IntPtr handle) => Handle = handle;
}

public abstract class QBoxLayout : QLayout
{
    protected QBoxLayout(IntPtr handle) : base(handle) { }
    public void AddWidget(QWidget widget, int stretch = 0) => QtNative.BoxAddWidget(Handle, widget.Handle, stretch);
    public void AddLayout(QLayout layout) => QtNative.BoxAddLayout(Handle, layout.Handle);
    public void AddStretch() => QtNative.BoxAddStretch(Handle);
}

public class QVBoxLayout : QBoxLayout
{
    public QVBoxLayout() : base(QtNative.VBoxLayoutCreate()) { }
}

public class QHBoxLayout : QBoxLayout
{
    public QHBoxLayout() : base(QtNative.HBoxLayoutCreate()) { }
}

public class QGridLayout : QLayout
{
    public QGridLayout() : base(QtNative.GridLayoutCreate()) { }
    public void AddWidget(QWidget widget, int row, int col, int rowSpan = 1, int colSpan = 1)
        => QtNative.GridAddWidget(Handle, widget.Handle, row, col, rowSpan, colSpan);
}

public class QTabWidget : QWidget
{
    public QTabWidget() : base(QtNative.TabWidgetCreate()) { }
    public void AddTab(QWidget page, string title) => QtNative.TabWidgetAddTab(Handle, page.Handle, title);
}

public class QGroupBox : QWidget
{
    public QGroupBox(string title) : base(QtNative.GroupBoxCreate(title)) { }
}

public class QScrollArea : QWidget
{
    public QScrollArea(QWidget content) : base(QtNative.ScrollAreaCreate(content.Handle)) { }
}

public class QLabel : QWidget
{
    public QLabel(string text) : base(QtNative.LabelCreate(text)) { }
    public string Text
    {
        set => QtNative.LabelSetText(Handle, value);
    }
}

public class QPushButton : QWidget
{
    private QtNativeCallback? _callback; // keep alive to prevent GC collection

    public QPushButton(string text) : base(QtNative.PushButtonCreate(text)) { }

    public void SetStyle(string qss) => QtNative.PushButtonSetStyle(Handle, qss);

    public event Action? Clicked
    {
        add
        {
            _callback = _ => value?.Invoke();
            QtNative.RootDelegate(_callback);
            QtNative.PushButtonOnClick(Handle, _callback, IntPtr.Zero);
        }
        remove => _callback = null;
    }
}

public class QCheckBox : QWidget
{
    public QCheckBox(string text) : base(QtNative.CheckBoxCreate(text)) { }
    public bool IsChecked
    {
        get => QtNative.CheckBoxIsChecked(Handle) != 0;
        set => QtNative.CheckBoxSetChecked(Handle, value ? 1 : 0);
    }
}

public class QSpinBox : QWidget
{
    public QSpinBox(int minVal, int maxVal, int val) : base(QtNative.SpinBoxCreate(minVal, maxVal, val)) { }
    public int Value
    {
        get => QtNative.SpinBoxGetVal(Handle);
        set => QtNative.SpinBoxSetVal(Handle, value);
    }
}

public class QDoubleSpinBox : QWidget
{
    public QDoubleSpinBox(double minVal, double maxVal, double val, double step = 1.0)
        : base(QtNative.DoubleSpinBoxCreate(minVal, maxVal, val, step)) { }

    public double Value
    {
        get => QtNative.DoubleSpinBoxGetVal(Handle);
        set => QtNative.DoubleSpinBoxSetVal(Handle, value);
    }
}

public class QComboBox : QWidget
{
    private QtNativeCallback? _callback;

    public QComboBox() : base(QtNative.ComboBoxCreate()) { }
    public void AddItem(string text) => QtNative.ComboBoxAddItem(Handle, text);
    public void Clear() => QtNative.ComboBoxClear(Handle);
    public int SelectedIndex
    {
        get => QtNative.ComboBoxGetIndex(Handle);
        set => QtNative.ComboBoxSetIndex(Handle, value);
    }

    public event Action? SelectedIndexChanged
    {
        add
        {
            _callback = _ => value?.Invoke();
            QtNative.RootDelegate(_callback);
            QtNative.ComboBoxOnChange(Handle, _callback, IntPtr.Zero);
        }
        remove => _callback = null;
    }
}

public class QTextEdit : QWidget
{
    public QTextEdit(string text = "", bool readOnly = false)
        : base(QtNative.TextEditCreate(text, readOnly ? 1 : 0)) { }

    public string Text
    {
        get
        {
            Span<byte> buf = stackalloc byte[16384];
            int res = QtNative.TextEditGetText(Handle, buf, buf.Length);
            if (res == 0) return string.Empty;
            int len = buf.IndexOf((byte)0);
            if (len < 0) len = buf.Length;
            return Encoding.UTF8.GetString(buf[..len]);
        }
        set => QtNative.TextEditSetText(Handle, value);
    }
}

public class QLineEdit : QWidget
{
    public QLineEdit(string text = "") : base(QtNative.LineEditCreate(text)) { }

    public string Text
    {
        get
        {
            Span<byte> buf = stackalloc byte[512];
            int res = QtNative.LineEditGetText(Handle, buf, buf.Length);
            if (res == 0) return string.Empty;
            int len = buf.IndexOf((byte)0);
            if (len < 0) len = buf.Length;
            return Encoding.UTF8.GetString(buf[..len]);
        }
        set => QtNative.LineEditSetText(Handle, value);
    }
}

public static class QMessageBox
{
    public static void ShowInfo(QWidget? parent, string title, string message)
        => QtNative.MessageBox(parent?.Handle ?? IntPtr.Zero, 0, title, message);

    public static void ShowWarning(QWidget? parent, string title, string message)
        => QtNative.MessageBox(parent?.Handle ?? IntPtr.Zero, 1, title, message);

    public static void ShowError(QWidget? parent, string title, string message)
        => QtNative.MessageBox(parent?.Handle ?? IntPtr.Zero, 2, title, message);
}

public static class QFileDialog
{
    public static string? GetExistingDirectory(QWidget? parent, string title, string initialDir)
    {
        Span<byte> buf = stackalloc byte[1024];
        int res = QtNative.DialogGetDirectory(parent?.Handle ?? IntPtr.Zero, title, initialDir, buf, buf.Length);
        if (res == 0) return null;
        int len = buf.IndexOf((byte)0);
        if (len < 0) len = buf.Length;
        return Encoding.UTF8.GetString(buf[..len]);
    }
}

