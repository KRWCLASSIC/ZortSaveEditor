#ifndef ZSE_BRIDGE_H
#define ZSE_BRIDGE_H

#ifdef __cplusplus
extern "C" {
#endif

#ifdef _WIN32
#  define ZSE_API __declspec(dllexport)
#else
#  define ZSE_API __attribute__((visibility("default")))
#endif

typedef void (*qt_callback_t)(void* user_data);

// Application
ZSE_API void* qt_app_create(int argc, char** argv);
ZSE_API int   qt_app_exec(void* app);
ZSE_API void  qt_app_quit(void* app);
ZSE_API void  qt_app_set_dark_theme(void* app, int dark);

// Core Window & Widgets
ZSE_API void* qt_mainwindow_create();
ZSE_API void  qt_widget_show(void* widget);
ZSE_API void  qt_widget_resize(void* widget, int w, int h);
ZSE_API void  qt_widget_set_title(void* widget, const char* title);
ZSE_API void  qt_widget_set_central(void* main_window, void* widget);
ZSE_API void  qt_widget_set_layout(void* widget, void* layout);
ZSE_API void* qt_widget_create();

// Layouts
ZSE_API void* qt_vbox_create();
ZSE_API void* qt_hbox_create();
ZSE_API void* qt_grid_create();
ZSE_API void  qt_box_add_widget(void* box_layout, void* widget, int stretch);
ZSE_API void  qt_box_add_layout(void* box_layout, void* child_layout);
ZSE_API void  qt_box_add_stretch(void* box_layout);
ZSE_API void  qt_grid_add_widget(void* grid_layout, void* widget, int row, int col, int row_span, int col_span);

// Tabs & Containers
ZSE_API void* qt_tabwidget_create();
ZSE_API void  qt_tabwidget_add_tab(void* tabs, void* page, const char* title);
ZSE_API void* qt_groupbox_create(const char* title);
ZSE_API void* qt_scrollarea_create(void* content_widget);

// Basic Controls
ZSE_API void* qt_label_create(const char* text);
ZSE_API void  qt_label_set_text(void* label, const char* text);

ZSE_API void* qt_pushbutton_create(const char* text);
ZSE_API void  qt_pushbutton_set_style(void* btn, const char* qss);
ZSE_API void  qt_pushbutton_on_click(void* btn, qt_callback_t cb, void* user_data);

ZSE_API void* qt_checkbox_create(const char* text);
ZSE_API int   qt_checkbox_is_checked(void* cb);
ZSE_API void  qt_checkbox_set_checked(void* cb, int checked);

ZSE_API void* qt_spinbox_create(int min_val, int max_val, int val);
ZSE_API int   qt_spinbox_get_val(void* sb);
ZSE_API void  qt_spinbox_set_val(void* sb, int val);

ZSE_API void*  qt_doublespinbox_create(double min_val, double max_val, double val, double step);
ZSE_API double qt_doublespinbox_get_val(void* dsb);
ZSE_API void   qt_doublespinbox_set_val(void* dsb, double val);

ZSE_API void* qt_combobox_create();
ZSE_API void  qt_combobox_add_item(void* cmb, const char* text);
ZSE_API int   qt_combobox_get_index(void* cmb);
ZSE_API void  qt_combobox_set_index(void* cmb, int index);
ZSE_API void  qt_combobox_clear(void* cmb);
ZSE_API void  qt_combobox_on_change(void* cmb, qt_callback_t cb, void* user_data);

ZSE_API void* qt_textedit_create(const char* text, int read_only);
ZSE_API void  qt_textedit_set_text(void* te, const char* text);
ZSE_API int   qt_textedit_get_text(void* te, char* out_buf, int out_len);

ZSE_API void* qt_lineedit_create(const char* text);
ZSE_API void  qt_lineedit_set_text(void* le, const char* text);
ZSE_API int   qt_lineedit_get_text(void* le, char* out_buf, int out_len);

// Dialogs & Status
ZSE_API void qt_statusbar_message(void* main_window, const char* text);
ZSE_API void qt_message_box(void* parent, int icon_type, const char* title, const char* text);
ZSE_API int  qt_dialog_get_directory(void* parent, const char* title, const char* initial_dir, char* out_buf, int out_len);

// Menus
ZSE_API void* qt_mainwindow_menu_bar(void* main_window);
ZSE_API void* qt_menubar_add_menu(void* menubar, const char* title);
ZSE_API void* qt_menu_add_action(void* menu, const char* text, const char* shortcut, qt_callback_t cb, void* user_data);
ZSE_API void  qt_menu_add_separator(void* menu);

#ifdef __cplusplus
}
#endif

#endif // ZSE_BRIDGE_H
