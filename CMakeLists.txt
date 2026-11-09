cmake_minimum_required(VERSION 3.20)
project(zse_bridge LANGUAGES CXX)

set(CMAKE_EXPORT_COMPILE_COMMANDS ON)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Auto-detect local .deps/qt6 if present
set(LOCAL_QT_CANDIDATE "${CMAKE_CURRENT_SOURCE_DIR}/.deps/qt6/6.8.0/msvc2022_64")
if(EXISTS "${LOCAL_QT_CANDIDATE}")
    list(PREPEND CMAKE_PREFIX_PATH "${LOCAL_QT_CANDIDATE}")
endif()

find_package(Qt6 REQUIRED COMPONENTS Core Gui Widgets)

qt_standard_project_setup()

add_library(zse_bridge SHARED
    src/bridge/zse_bridge.h
    src/bridge/zse_bridge.cpp
)

target_include_directories(zse_bridge PUBLIC
    src/bridge
)

target_link_libraries(zse_bridge PRIVATE
    Qt6::Core
    Qt6::Gui
    Qt6::Widgets
)

target_compile_definitions(zse_bridge PRIVATE
    ZSE_BUILD_DLL
)

# Output directly to publish/lib folder (across both single-config and multi-config generators)
set_target_properties(zse_bridge PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/publish/lib"
    RUNTIME_OUTPUT_DIRECTORY_RELEASE "${CMAKE_CURRENT_SOURCE_DIR}/publish/lib"
    LIBRARY_OUTPUT_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/publish/lib"
    LIBRARY_OUTPUT_DIRECTORY_RELEASE "${CMAKE_CURRENT_SOURCE_DIR}/publish/lib"
    ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/publish/lib"
    ARCHIVE_OUTPUT_DIRECTORY_RELEASE "${CMAKE_CURRENT_SOURCE_DIR}/publish/lib"
)

