#include "zse_bridge.h"

#include <QApplication>
#include <QMainWindow>
#include <QWidget>
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QGridLayout>
#include <QTabWidget>
#include <QGroupBox>
#include <QScrollArea>
#include <QLabel>
#include <QPushButton>
#include <QCheckBox>
#include <QSpinBox>
#include <QDoubleSpinBox>
#include <QComboBox>
#include <QLineEdit>
#include <QTextEdit>
#include <QStatusBar>
#include <QMenuBar>
#include <QMenu>
#include <QAction>
#include <QKeySequence>
#include <QMessageBox>
#include <QFileDialog>
#include <QStyleFactory>
#include <QPalette>
#include <QCoreApplication>
#include <QDir>
#include <cstring>

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <windows.h>

void* qt_app_create(int argc, char** argv)
{
    wchar_t exePath[MAX_PATH];
    DWORD len = GetModuleFileNameW(NULL, exePath, MAX_PATH);
    if (len > 0 && len < MAX_PATH) {
        wchar_t* lastSlash = wcsrchr(exePath, L'\\');
        if (lastSlash) {
            *lastSlash = L'\0';
            QString appDir = QString::fromWCharArray(exePath);
            QString libDir = appDir + "/lib";
            QCoreApplication::addLibraryPath(libDir);
            SetDllDirectoryW(reinterpret_cast<LPCWSTR>(libDir.utf16()));
        }
    }
    return new QApplication(argc, argv);
}

int qt_app_exec(void* app)
{
    return static_cast<QApplication*>(app)->exec();
}

void qt_app_quit(void* app)
{
    static_cast<QApplication*>(app)->quit();
}

void qt_app_set_dark_theme(void* app, int dark)
{
    QApplication* a = static_cast<QApplication*>(app);
    if (!a) return;

    if (dark) {
        a->setStyle(QStyleFactory::create("Fusion"));
        QPalette p;
        p.setColor(QPalette::Window, QColor(32, 33, 36));
        p.setColor(QPalette::WindowText, QColor(232, 234, 237));
        p.setColor(QPalette::Base, QColor(24, 25, 27));
        p.setColor(QPalette::AlternateBase, QColor(40, 42, 45));
        p.setColor(QPalette::ToolTipBase, QColor(232, 234, 237));
        p.setColor(QPalette::ToolTipText, QColor(24, 25, 27));
        p.setColor(QPalette::Text, QColor(232, 234, 237));
        p.setColor(QPalette::Button, QColor(48, 51, 56));
        p.setColor(QPalette::ButtonText, QColor(232, 234, 237));
        p.setColor(QPalette::BrightText, Qt::red);
        p.setColor(QPalette::Link, QColor(138, 180, 248));
        p.setColor(QPalette::Highlight, QColor(138, 180, 248));
        p.setColor(QPalette::HighlightedText, Qt::black);
        a->setPalette(p);
    } else {
        a->setStyle(QStyleFactory::create("Fusion"));
        QPalette p;
        p.setColor(QPalette::Window, QColor(240, 240, 240));
        p.setColor(QPalette::WindowText, QColor(0, 0, 0));
        p.setColor(QPalette::Base, QColor(255, 255, 255));
        p.setColor(QPalette::AlternateBase, QColor(245, 245, 245));
        p.setColor(QPalette::ToolTipBase, QColor(255, 255, 220));
        p.setColor(QPalette::ToolTipText, QColor(0, 0, 0));
        p.setColor(QPalette::Text, QColor(0, 0, 0));
        p.setColor(QPalette::Button, QColor(240, 240, 240));
        p.setColor(QPalette::ButtonText, QColor(0, 0, 0));
        p.setColor(QPalette::BrightText, Qt::red);
        p.setColor(QPalette::Link, QColor(0, 102, 204));
        p.setColor(QPalette::Highlight, QColor(0, 120, 215));
        p.setColor(QPalette::HighlightedText, QColor(255, 255, 255));
        a->setPalette(p);
    }
}

void* qt_mainwindow_create()
{
    return new QMainWindow();
}

void qt_widget_show(void* widget)
{
    static_cast<QWidget*>(widget)->show();
}

void qt_widget_resize(void* widget, int w, int h)
{
    static_cast<QWidget*>(widget)->resize(w, h);
}

void qt_widget_set_title(void* widget, const char* title)
{
    static_cast<QWidget*>(widget)->setWindowTitle(QString::fromUtf8(title));
}

void qt_widget_set_central(void* main_window, void* widget)
{
    static_cast<QMainWindow*>(main_window)->setCentralWidget(static_cast<QWidget*>(widget));
}

void qt_widget_set_layout(void* widget, void* layout)
{
    static_cast<QWidget*>(widget)->setLayout(static_cast<QLayout*>(layout));
}

void* qt_widget_create()
{
    return new QWidget();
}

void* qt_vbox_create()
{
    return new QVBoxLayout();
}

void* qt_hbox_create()
{
    return new QHBoxLayout();
}

void* qt_grid_create()
{
    return new QGridLayout();
}

void qt_box_add_widget(void* box_layout, void* widget, int stretch)
{
    static_cast<QBoxLayout*>(box_layout)->addWidget(static_cast<QWidget*>(widget), stretch);
}

void qt_box_add_layout(void* box_layout, void* child_layout)
{
    static_cast<QBoxLayout*>(box_layout)->addLayout(static_cast<QLayout*>(child_layout));
}

void qt_box_add_stretch(void* box_layout)
{
    static_cast<QBoxLayout*>(box_layout)->addStretch();
}

void qt_grid_add_widget(void* grid_layout, void* widget, int row, int col, int row_span, int col_span)
{
    static_cast<QGridLayout*>(grid_layout)->addWidget(static_cast<QWidget*>(widget), row, col, row_span, col_span);
}

void* qt_tabwidget_create()
{
    return new QTabWidget();
}

void qt_tabwidget_add_tab(void* tabs, void* page, const char* title)
{
    static_cast<QTabWidget*>(tabs)->addTab(static_cast<QWidget*>(page), QString::fromUtf8(title));
}

void* qt_groupbox_create(const char* title)
{
    return new QGroupBox(QString::fromUtf8(title));
}

void* qt_scrollarea_create(void* content_widget)
{
    QScrollArea* sa = new QScrollArea();
    sa->setWidget(static_cast<QWidget*>(content_widget));
    sa->setWidgetResizable(true);
    return sa;
}

void* qt_label_create(const char* text)
{
    return new QLabel(QString::fromUtf8(text));
}

void qt_label_set_text(void* label, const char* text)
{
    static_cast<QLabel*>(label)->setText(QString::fromUtf8(text));
}

void* qt_pushbutton_create(const char* text)
{
    return new QPushButton(QString::fromUtf8(text));
}

void qt_pushbutton_set_style(void* btn, const char* qss)
{
    static_cast<QPushButton*>(btn)->setStyleSheet(QString::fromUtf8(qss));
}

void qt_pushbutton_on_click(void* btn, qt_callback_t cb, void* user_data)
{
    QPushButton* b = static_cast<QPushButton*>(btn);
    QObject::connect(b, &QPushButton::clicked, [cb, user_data]() {
        if (cb) cb(user_data);
    });
}

void* qt_checkbox_create(const char* text)
{
    return new QCheckBox(QString::fromUtf8(text));
}

int qt_checkbox_is_checked(void* cb)
{
    return static_cast<QCheckBox*>(cb)->isChecked() ? 1 : 0;
}

void qt_checkbox_set_checked(void* cb, int checked)
{
    static_cast<QCheckBox*>(cb)->setChecked(checked != 0);
}

void* qt_spinbox_create(int min_val, int max_val, int val)
{
    QSpinBox* sb = new QSpinBox();
    sb->setRange(min_val, max_val);
    sb->setValue(val);
    return sb;
}

int qt_spinbox_get_val(void* sb)
{
    return static_cast<QSpinBox*>(sb)->value();
}

void qt_spinbox_set_val(void* sb, int val)
{
    static_cast<QSpinBox*>(sb)->setValue(val);
}

void* qt_doublespinbox_create(double min_val, double max_val, double val, double step)
{
    QDoubleSpinBox* dsb = new QDoubleSpinBox();
    dsb->setRange(min_val, max_val);
    dsb->setValue(val);
    dsb->setSingleStep(step);
    return dsb;
}

double qt_doublespinbox_get_val(void* dsb)
{
    return static_cast<QDoubleSpinBox*>(dsb)->value();
}

void qt_doublespinbox_set_val(void* dsb, double val)
{
    static_cast<QDoubleSpinBox*>(dsb)->setValue(val);
}

void* qt_combobox_create()
{
    return new QComboBox();
}

void qt_combobox_add_item(void* cmb, const char* text)
{
    static_cast<QComboBox*>(cmb)->addItem(QString::fromUtf8(text));
}

int qt_combobox_get_index(void* cmb)
{
    return static_cast<QComboBox*>(cmb)->currentIndex();
}

void qt_combobox_set_index(void* cmb, int index)
{
    static_cast<QComboBox*>(cmb)->setCurrentIndex(index);
}

void qt_combobox_clear(void* cmb)
{
    if (!cmb) return;
    static_cast<QComboBox*>(cmb)->clear();
}

void qt_combobox_on_change(void* cmb, qt_callback_t cb, void* user_data)
{
    if (!cmb || !cb) return;
    auto* combo = static_cast<QComboBox*>(cmb);
    QObject::connect(combo, &QComboBox::currentIndexChanged, combo, [cb, user_data](int) {
        cb(user_data);
    });
}

void* qt_textedit_create(const char* text, int read_only)
{
    QTextEdit* te = new QTextEdit();
    if (text) te->setPlainText(QString::fromUtf8(text));
    te->setReadOnly(read_only != 0);
    return te;
}

void qt_textedit_set_text(void* te, const char* text)
{
    static_cast<QTextEdit*>(te)->setPlainText(QString::fromUtf8(text));
}

int qt_textedit_get_text(void* te, char* out_buf, int out_len)
{
    if (!te || !out_buf || out_len <= 0) return 0;
    QByteArray utf8 = static_cast<QTextEdit*>(te)->toPlainText().toUtf8();
    int copy_len = qMin(utf8.size(), out_len - 1);
    memcpy(out_buf, utf8.constData(), copy_len);
    out_buf[copy_len] = '\0';
    return 1;
}

void* qt_lineedit_create(const char* text)
{
    QLineEdit* le = new QLineEdit();
    if (text) le->setText(QString::fromUtf8(text));
    return le;
}

void qt_lineedit_set_text(void* le, const char* text)
{
    static_cast<QLineEdit*>(le)->setText(QString::fromUtf8(text));
}

int qt_lineedit_get_text(void* le, char* out_buf, int out_len)
{
    if (!le || !out_buf || out_len <= 0) return 0;
    QByteArray utf8 = static_cast<QLineEdit*>(le)->text().toUtf8();
    int copy_len = qMin(utf8.size(), out_len - 1);
    memcpy(out_buf, utf8.constData(), copy_len);
    out_buf[copy_len] = '\0';
    return 1;
}

void qt_statusbar_message(void* main_window, const char* text)
{
    QMainWindow* mw = static_cast<QMainWindow*>(main_window);
    if (mw && mw->statusBar() && text) {
        mw->statusBar()->showMessage(QString::fromUtf8(text));
    }
}

void qt_message_box(void* parent, int icon_type, const char* title, const char* text)
{
    QWidget* p = static_cast<QWidget*>(parent);
    QString t = QString::fromUtf8(title);
    QString msg = QString::fromUtf8(text);
    switch (icon_type) {
    case 1: QMessageBox::warning(p, t, msg); break;
    case 2: QMessageBox::critical(p, t, msg); break;
    default: QMessageBox::information(p, t, msg); break;
    }
}

int qt_dialog_get_directory(void* parent, const char* title, const char* initial_dir, char* out_buf, int out_len)
{
    QWidget* p = static_cast<QWidget*>(parent);
    QString dir = QFileDialog::getExistingDirectory(p, QString::fromUtf8(title), QString::fromUtf8(initial_dir));
    if (!dir.isEmpty() && out_buf && out_len > 0) {
        QByteArray utf8 = dir.toUtf8();
        int copy_len = qMin(utf8.size(), out_len - 1);
        memcpy(out_buf, utf8.constData(), copy_len);
        out_buf[copy_len] = '\0';
        return 1;
    }
    return 0;
}

void* qt_mainwindow_menu_bar(void* main_window)
{
    QMainWindow* mw = static_cast<QMainWindow*>(main_window);
    return mw ? mw->menuBar() : nullptr;
}

void* qt_menubar_add_menu(void* menubar, const char* title)
{
    QMenuBar* mb = static_cast<QMenuBar*>(menubar);
    return mb ? mb->addMenu(QString::fromUtf8(title)) : nullptr;
}

void* qt_menu_add_action(void* menu, const char* text, const char* shortcut, qt_callback_t cb, void* user_data)
{
    QMenu* m = static_cast<QMenu*>(menu);
    if (!m) return nullptr;
    QAction* act = m->addAction(QString::fromUtf8(text));
    if (shortcut && *shortcut) {
        act->setShortcut(QKeySequence(QString::fromUtf8(shortcut)));
    }
    if (cb) {
        QObject::connect(act, &QAction::triggered, [cb, user_data]() {
            cb(user_data);
        });
    }
    return act;
}

void qt_menu_add_separator(void* menu)
{
    QMenu* m = static_cast<QMenu*>(menu);
    if (m) m->addSeparator();
}
